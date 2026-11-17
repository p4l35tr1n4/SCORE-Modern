using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using ScoreModern.RecoveredFragments;

class SelectorRoutineProof
{
    sealed class Context : ISelectorContext
    {
        public readonly byte[] Data = new byte[65536];
        public readonly StringBuilder Events = new StringBuilder();
        public ushort DataSegment { get { return 0x1240; } }
        public bool DirectionFlag { get; set; }
        public ushort First, Second, ReturnedAx;
        public int ParameterReads, Calls;
        public void Reset(ushort first, ushort second, bool df, int seed, ushort returned)
        {
            First=first; Second=second; DirectionFlag=df; ReturnedAx=returned;
            ParameterReads=Calls=0; Events.Length=0;
            for(int i=0x3c40;i<0x3c90;i++) Data[i]=(byte)(i*73+(i>>8)*19+seed);
        }
        public ushort ReadWord(ushort segment, ushort offset)
        {
            ushort value;
            if(segment==0x2450 && offset==0x0080) value=ParameterReads++==0?First:Second;
            else if(segment==DataSegment) value=(ushort)(Data[offset]|Data[unchecked((ushort)(offset+1))]<<8);
            else throw new Exception("Unexpected data read");
            Events.Append('R').Append(segment.ToString("x4")).Append(':').Append(offset.ToString("x4")).Append('=').Append(value.ToString("x4")).Append(';');
            return value;
        }
        public void WriteWord(ushort segment, ushort offset, ushort value)
        {
            if(segment!=DataSegment) throw new Exception("Unexpected data write");
            Data[offset]=(byte)value; Data[unchecked((ushort)(offset+1))]=(byte)(value>>8);
            Events.Append('W').Append(segment.ToString("x4")).Append(':').Append(offset.ToString("x4")).Append('=').Append(value.ToString("x4")).Append(';');
        }
        public ushort CallFar(ushort segment, ushort offset, ushort[] arguments)
        {
            Calls++;
            Events.Append('C').Append(segment.ToString("x4")).Append(':').Append(offset.ToString("x4")).Append('[');
            foreach(ushort x in arguments) Events.Append(x.ToString("x4")).Append(',');
            Events.Append("];");
            if(segment!=0x1f08) throw new Exception("Unexpected callee segment");
            // Explicit test doubles, not implementations of original callees.
            if(offset==0x2ac6) { Data[0x3c6e]^=0x5a; return ReturnedAx; }
            if(offset==0x0003) return 0xabcd;
            throw new Exception("Unexpected callee offset");
        }
    }
    sealed class Original
    {
        readonly byte[] code;
        readonly ushort[] stack=new ushort[32768];
        ushort ax,bx,si,di,bp,sp,es;
        bool z,s,o;
        int pc;
        public Original(byte[] originalCode) { code=originalCode; }
        void Push(ushort x) { sp=unchecked((ushort)(sp-2)); stack[sp/2]=x; }
        ushort Pop() { ushort x=stack[sp/2]; sp=unchecked((ushort)(sp+2)); return x; }
        void Cmp(ushort left, ushort right)
        {
            ushort result=unchecked((ushort)(left-right));
            z=result==0; s=(result&0x8000)!=0;
            o=((left^right)&(left^result)&0x8000)!=0;
        }
        ushort Word() { ushort x=BitConverter.ToUInt16(code,pc); pc+=2; return x; }
        void Require(int expected) { if(code[pc++]!=expected) throw new Exception("Unsupported encoding at "+(pc-1)); }
        public void Execute(Context context)
        {
            ax=0x1111; bx=0x2222; si=0x3333; di=0x4444; bp=0x5555; es=0x6666;
            sp=0xff00; pc=0; int steps=0;
            stack[sp/2]=0xbeef; stack[(sp+2)/2]=0x7777;
            stack[(sp+4)/2]=0x0080; stack[(sp+6)/2]=0x2450;
            while(true)
            {
                if(++steps>100) throw new Exception("Instruction budget exceeded");
                int op=code[pc++];
                switch(op)
                {
                    case 0x55:Push(bp);break;
                    case 0x57:Push(di);break;
                    case 0x56:Push(si);break;
                    case 0x50:Push(ax);break;
                    case 0x1e:Push(context.DataSegment);break;
                    case 0x07:es=Pop();break;
                    case 0x5e:si=Pop();break;
                    case 0x5f:di=Pop();break;
                    case 0x5d:bp=Pop();break;
                    case 0xc4:
                        Require(0x5e);Require(0x06);
                        bx=stack[unchecked((ushort)(bp+6))/2]; es=stack[unchecked((ushort)(bp+8))/2];break;
                    case 0x26:
                        int next=code[pc++];
                        if(next==0x83){Require(0x3f);Cmp(context.ReadWord(es,bx),unchecked((ushort)(short)(sbyte)code[pc++]));}
                        else if(next==0x8b){Require(0x37);si=context.ReadWord(es,bx);}
                        else throw new Exception("Unsupported ES override");
                        break;
                    case 0x8b:
                        int modrm=code[pc++];
                        if(modrm==0xec)bp=sp;
                        else if(modrm==0xe5)sp=bp;
                        else if(modrm==0xde)bx=si;
                        else if(modrm==0xc3)ax=bx;
                        else throw new Exception("Unsupported MOV");
                        break;
                    case 0x83:
                        int kind=code[pc++];
                        ushort immediate=unchecked((ushort)(short)(sbyte)code[pc++]);
                        if(kind==0xfe)Cmp(si,immediate);
                        else if(kind==0xc4)sp=unchecked((ushort)(sp+immediate));
                        else throw new Exception("Unsupported group 83");
                        break;
                    case 0x75:case 0x7e:case 0x7d:case 0xeb:
                        int displacement=(sbyte)code[pc++];
                        bool taken=op==0xeb || (op==0x75?!z:op==0x7e?z||s!=o:s==o);
                        if(taken)pc+=displacement;
                        break;
                    case 0xb8:ax=Word();break;
                    case 0xbf:di=Word();break;
                    case 0xd1:Require(0xe3);bx=unchecked((ushort)(bx<<1));break;
                    case 0x03:Require(0xd8);bx=unchecked((ushort)(bx+ax));break;
                    case 0x8d:Require(0xb7);si=unchecked((ushort)(bx+Word()));break;
                    case 0xa5:
                        ushort value=context.ReadWord(context.DataSegment,si);
                        context.WriteWord(es,di,value);
                        si=unchecked((ushort)(si+(context.DirectionFlag?-2:2)));
                        di=unchecked((ushort)(di+(context.DirectionFlag?-2:2)));break;
                    case 0x9a:
                        ushort offset=Word(),segment=Word();
                        // Argument count comes from the original caller's
                        // following ADD SP,n, not an assumed callee signature.
                        if(code[pc]!=0x83||code[pc+1]!=0xc4)throw new Exception("Missing caller cleanup");
                        int count=code[pc+2]/2;
                        var words=new ushort[count];
                        for(int i=0;i<count;i++)words[i]=stack[unchecked((ushort)(sp+i*2))/2];
                        Push(0x2796);Push((ushort)pc);
                        ax=context.CallFar(segment,offset,words);
                        if(Pop()!=pc||Pop()!=0x2796)throw new Exception("Far-call stack mismatch");
                        break;
                    case 0xca:
                        ushort cleanup=Word();
                        if(Pop()!=0xbeef||Pop()!=0x7777)throw new Exception("Return address mismatch");
                        sp=unchecked((ushort)(sp+cleanup));
                        if(sp!=0xff08||si!=0x3333||di!=0x4444||bp!=0x5555)throw new Exception("Reference frame restoration mismatch");
                        return;
                    default:throw new Exception("Unsupported opcode "+op.ToString("x2"));
                }
            }
        }
    }
    static byte[] OriginalCode(string path)
    {
        byte[] source=File.ReadAllBytes(path);string hash;
        using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(source)).Replace("-","").ToLowerInvariant();
        if(hash!="1a21a97099b53a2b4a1a02fb19a9edee75ef6c7fe37bce71c9886222cb74d781")throw new Exception("Original executable hash mismatch");
        int start=0x60400+BitConverter.ToUInt16(source,0x60408)*16+0x3db6;
        byte[] code=new byte[106];Array.Copy(source,start,code,0,106);
        using(var sha=SHA256.Create())hash=BitConverter.ToString(sha.ComputeHash(code)).Replace("-","").ToLowerInvariant();
        if(hash!="1cd8ea34794c6e5021f1a880215a53634166df7ce9cd35be8d0bd88666566f92")throw new Exception("Routine hash mismatch");
        return code;
    }
    static void Check(Original original,Context reference,Context native,ushort first,ushort second,bool df,int seed,ushort returned)
    {
        reference.Reset(first,second,df,seed,returned);native.Reset(first,second,df,seed,returned);
        original.Execute(reference);SelectorRoutine.Execute(native,0x2450,0x0080);
        if(reference.Events.ToString()!=native.Events.ToString())throw new Exception("Event mismatch for "+first+"/"+second+" DF="+df);
        for(int i=0x3c40;i<0x3c90;i++)if(reference.Data[i]!=native.Data[i])throw new Exception("Data mismatch");
        if(reference.ParameterReads!=native.ParameterReads||reference.Calls!=native.Calls)throw new Exception("Read/call count mismatch");
    }
    static int Main(string[] args)
    {
        var original=new Original(OriginalCode(args[0]));var reference=new Context();var native=new Context();int cases=0;
        foreach(int seed in new int[]{17,93})foreach(bool df in new bool[]{false,true})
        {
            ushort returned=seed==17?(df?(ushort)0xffff:(ushort)0):(df?(ushort)0x8000:(ushort)0x1234);
            for(int value=0;value<65536;value++)
            {Check(original,reference,native,(ushort)value,(ushort)value,df,seed,returned);cases++;}
            foreach(ushort first in new ushort[]{0,1,0xffff})foreach(ushort second in new ushort[]{0,1,2,3,4,5,6,0xffff})
            {Check(original,reference,native,first,second,df,seed,returned);cases++;}
            for(int i=0;i<65536;i++)if(reference.Data[i]!=native.Data[i])throw new Exception("Full-segment mismatch");
        }
        Console.WriteLine("PASS: "+cases+" cases comparing original-byte control flow with compiled translation.");
        Console.WriteLine("Checked ordered data reads/writes, call targets, stack-word arguments, returned-AX forwarding and final DS memory; all states, both DF values, two patterns, four return values and changing parameter reads.");
        Console.WriteLine("Limit: callees are explicit test doubles. No original-callee behavior, incoming context, physical stack aliasing, interrupts, complete ABI or application equivalence is certified.");
        return 0;
    }
}
