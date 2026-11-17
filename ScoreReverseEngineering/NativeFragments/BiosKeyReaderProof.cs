using System;
using System.IO;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;

class BiosKeyReaderProof
{
    sealed class Memory : IBiosKeyContext
    {
        public ushort Key;
        public int Reads;
        public readonly List<string> Events=new List<string>();
        public readonly Dictionary<int,byte> Bytes=new Dictionary<int,byte>();
        public byte[] RuntimeData;
        public ushort ReadBiosKey(){Reads++;Events.Add("INT16:00");return Key;}
        public void WriteByte(ushort segment,ushort offset,byte value)
        {
            Events.Add(segment.ToString("x4")+":"+offset.ToString("x4")+"="+value.ToString("x2"));
            if(RuntimeData!=null){if(segment!=0x871e)throw new Exception("Runtime segment mismatch");RuntimeData[offset]=value;}
            else Bytes[(segment<<4)+offset]=value;
        }
        public void Reset(ushort key){Key=key;Reads=0;Events.Clear();Bytes.Clear();}
    }
    static void Interpret(byte[] code,Memory memory,ushort aSeg,ushort aOff,ushort sSeg,ushort sOff)
    {
        var stack=new ushort[32768];ushort sp=0xff00,bp=0x1234,ax=0xffff,bx=0,es=0;int pc=0,steps=0;
        stack[sp/2]=0xbeef;stack[(sp+2)/2]=0x1111;
        stack[(sp+4)/2]=aOff;stack[(sp+6)/2]=aSeg;stack[(sp+8)/2]=sOff;stack[(sp+10)/2]=sSeg;
        while(true)
        {
            if(++steps>20)throw new Exception("Instruction budget");
            int op=code[pc++];
            switch(op)
            {
                case 0x55:sp-=2;stack[sp/2]=bp;break;
                case 0x8b:if(code[pc++]!=0xec)throw new Exception("MOV");bp=sp;break;
                case 0xb4:ax=(ushort)((ax&255)|(code[pc++]<<8));break;
                case 0xcd:
                    if(code[pc++]!=0x16||(ax>>8)!=0)throw new Exception("INT encoding");
                    ax=memory.ReadBiosKey();break;
                case 0xc4:
                    if(code[pc++]!=0x5e)throw new Exception("LES");
                    int displacement=(sbyte)code[pc++];
                    bx=stack[unchecked((ushort)(bp+displacement))/2];es=stack[unchecked((ushort)(bp+displacement+2))/2];break;
                case 0x26:
                    if(code[pc++]!=0x88)throw new Exception("ES MOV");
                    int modrm=code[pc++];
                    if(modrm==7)memory.WriteByte(es,bx,(byte)(ax&255));
                    else if(modrm==0x27)memory.WriteByte(es,bx,(byte)(ax>>8));
                    else throw new Exception("Byte-store encoding");break;
                case 0x5d:bp=stack[sp/2];sp+=2;break;
                case 0xca:
                    ushort cleanup=BitConverter.ToUInt16(code,pc);pc+=2;
                    if(stack[sp/2]!=0xbeef||stack[(sp+2)/2]!=0x1111)throw new Exception("Return frame");
                    sp=(ushort)(sp+4+cleanup);
                    if(sp!=0xff0c||bp!=0x1234||pc!=code.Length)throw new Exception("Frame restoration");
                    return;
                default:throw new Exception("Unsupported opcode");
            }
        }
    }
    static int Main(string[] args)
    {
        byte[] image=File.ReadAllBytes(args[0]),code=new byte[23];Array.Copy(image,0x16fa0+0x841,code,0,23);
        string hex=BitConverter.ToString(code).Replace("-","").ToLowerInvariant();
        if(hex!="558becb400cd16c45e06268807c45e0a2688275dca0800")throw new Exception("Original code differs");
        var reference=new Memory();var native=new Memory();int cases=0;
        foreach(int layout in new int[]{0,1,2})for(int key=0;key<65536;key++)
        {
            ushort aSeg=0x1000,aOff=0x100,sSeg=layout==2?(ushort)0x1010:(ushort)0x1000,sOff=layout==0?(ushort)0x102:layout==1?(ushort)0x100:(ushort)0;
            reference.Reset((ushort)key);native.Reset((ushort)key);
            Interpret(code,reference,aSeg,aOff,sSeg,sOff);BiosKeyReader.Execute(native,aSeg,aOff,sSeg,sOff);
            if(reference.Reads!=1||native.Reads!=1||string.Join(";",reference.Events)!=string.Join(";",native.Events)||reference.Bytes.Count!=native.Bytes.Count)throw new Exception("Event mismatch");
            foreach(var item in reference.Bytes)if(native.Bytes[item.Key]!=item.Value)throw new Exception("Memory mismatch");
            cases++;
        }
        byte[] before=File.ReadAllBytes(args[1]),after=File.ReadAllBytes(args[2]),frame=File.ReadAllBytes(args[3]);
        if(before.Length!=65536||after.Length!=65536)throw new Exception("Runtime dump length");
        native.Reset(0x011b);native.RuntimeData=(byte[])before.Clone();
        BiosKeyReader.Execute(native,BitConverter.ToUInt16(frame,8),BitConverter.ToUInt16(frame,6),BitConverter.ToUInt16(frame,12),BitConverter.ToUInt16(frame,10));
        for(int i=0;i<65536;i++)if(native.RuntimeData[i]!=after[i])throw new Exception("Original runtime capture mismatch at "+i);
        Console.WriteLine("PASS: "+cases+" original-byte interpreter cases; all BIOS key words with distinct, same-pointer and physical-alias outputs.");
        Console.WriteLine("PASS: compiled fragment matches the actual original ESC-reader runtime capture across all 65536 DS bytes.");
        Console.WriteLine("Scope: BIOS result and ordered output-byte effects. BIOS device behavior, Windows key mapping, complete ABI and application equivalence remain unverified.");
        return 0;
    }
}
