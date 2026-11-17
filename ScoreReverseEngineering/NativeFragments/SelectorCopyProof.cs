using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using ScoreModern.RecoveredFragments;

class SelectorCopyProof
{
    // Constrained interpreter of the ORIGINAL instruction bytes, not a second
    // expression of the translated predicate. Refuses unknown instructions.
    static bool Execute(byte[] code, byte[] memory, ushort si, bool df)
    {
        int pc = 0x22, steps = 0;
        ushort ax = 0x5555, bx = 0xaaaa, di = 0xbbbb, ds = 0x1234, es = 0xffff;
        bool z = false, s = false, o = false;
        var stack = new Stack<ushort>();
        while (true)
        {
            if (pc == 0x62) return false;
            if (pc == 0x44) { if (stack.Count != 0) throw new Exception("Stack imbalance"); return true; }
            if (++steps > 40) throw new Exception("Instruction budget exceeded");
            int op = code[pc++];
            switch (op)
            {
                case 0x83:
                    if (code[pc++] != 0xfe) throw new Exception("CMP encoding");
                    ushort right = unchecked((ushort)(short)(sbyte)code[pc++]);
                    ushort result = unchecked((ushort)(si - right));
                    z = result == 0; s = (result & 0x8000) != 0;
                    o = ((si ^ right) & (si ^ result) & 0x8000) != 0;
                    break;
                case 0x7e:
                case 0x7d:
                    int displacement = (sbyte)code[pc++];
                    if (op == 0x7e ? z || s != o : s == o) pc += displacement;
                    break;
                case 0x8b:
                    int modrm = code[pc++];
                    if (modrm == 0xde) bx = si;
                    else if (modrm == 0xc3) ax = bx;
                    else throw new Exception("MOV encoding");
                    break;
                case 0xd1:
                    if (code[pc++] != 0xe3) throw new Exception("SHL encoding");
                    bx = unchecked((ushort)(bx << 1)); break;
                case 0x03:
                    if (code[pc++] != 0xd8) throw new Exception("ADD encoding");
                    bx = unchecked((ushort)(bx + ax)); break;
                case 0x56: stack.Push(si); break;
                case 0xbf: di = BitConverter.ToUInt16(code, pc); pc += 2; break;
                case 0x8d:
                    if (code[pc++] != 0xb7) throw new Exception("LEA encoding");
                    si = unchecked((ushort)(bx + BitConverter.ToUInt16(code, pc))); pc += 2; break;
                case 0x1e: stack.Push(ds); break;
                case 0x07: es = stack.Pop(); break;
                case 0xa5:
                    if (es != ds) throw new Exception("Unexpected destination segment");
                    ushort word = (ushort)(memory[si] | memory[unchecked((ushort)(si + 1))] << 8);
                    memory[di] = (byte)word; memory[unchecked((ushort)(di + 1))] = (byte)(word >> 8);
                    si = unchecked((ushort)(si + (df ? -2 : 2)));
                    di = unchecked((ushort)(di + (df ? -2 : 2))); break;
                case 0x5e: si = stack.Pop(); break;
                default: throw new Exception("Unsupported opcode " + op.ToString("x2"));
            }
        }
    }
    static byte Pattern(int i, int seed) { return (byte)(i * 73 + (i >> 8) * 19 + seed); }
    static int Main(string[] args)
    {
        byte[] overlay = File.ReadAllBytes(args[0]), code = new byte[106];
        string sourceHash;
        using (var sha = SHA256.Create()) sourceHash = BitConverter.ToString(sha.ComputeHash(overlay)).Replace("-", "").ToLowerInvariant();
        if (sourceHash != "1a21a97099b53a2b4a1a02fb19a9edee75ef6c7fe37bce71c9886222cb74d781")
            throw new Exception("This proof requires the recorded original SCORE.EXE.");
        int overlayStart = 0x60400;
        if (BitConverter.ToUInt16(overlay, overlayStart) != 0x5a4d) throw new Exception("Overlay header mismatch");
        int imageStart = overlayStart + BitConverter.ToUInt16(overlay, overlayStart + 8) * 16;
        Array.Copy(overlay, imageStart + 0x3db6, code, 0, code.Length);
        string hash;
        using (var sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(code)).Replace("-", "").ToLowerInvariant();
        if (hash != "1cd8ea34794c6e5021f1a880215a53634166df7ce9cd35be8d0bd88666566f92") throw new Exception("Original instruction-byte hash mismatch");
        int cases = 0, copyCases = 0;
        foreach (int seed in new int[] {17, 93})
        foreach (bool df in new bool[] {false, true})
        {
            var original = new byte[65536]; var native = new byte[65536];
            for (int i = 0; i < 65536; i++) original[i] = native[i] = Pattern(i, seed);
            for (int value = 0; value < 65536; value++)
            {
                for (int i = 0x3c60; i < 0x3c80; i++) original[i] = native[i] = Pattern(i, seed);
                bool expected = Execute(code, original, (ushort)value, df);
                bool actual = SelectorCopy.Apply(native, unchecked((short)value), df);
                if (expected != actual) throw new Exception("Branch mismatch at " + value);
                for (int i = 0x3c60; i < 0x3c80; i++)
                    if (original[i] != native[i]) throw new Exception("Memory mismatch at " + value);
                if (expected)
                {
                    copyCases++;
                    for (int i = 0; i < 65536; i++)
                        if (original[i] != native[i]) throw new Exception("Full-segment mismatch");
                }
                cases++;
            }
            for (int i = 0; i < 65536; i++)
                if (original[i] != native[i]) throw new Exception("Final full-segment mismatch");
        }
        Console.WriteLine("PASS: " + cases + " cases; all 65536 state words, both DF values, two memory patterns; " + copyCases + " copying cases.");
        Console.WriteLine("Scope: copy-block DS memory effects and branch decision only; not the full routine, ABI, formatting calls or application equivalence.");
        Console.WriteLine("Original 106-byte routine SHA256: " + hash);
        return 0;
    }
}
