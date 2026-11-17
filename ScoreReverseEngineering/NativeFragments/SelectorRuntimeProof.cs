using System;
using System.IO;
using ScoreModern.RecoveredFragments;

class SelectorRuntimeProof
{
    sealed class AtOriginalCallBoundary : Exception { }
    sealed class Context : ISelectorContext
    {
        public byte[] Data;
        public ushort Segment;
        public bool Df;
        public ushort ParameterOffset;
        public int ParameterReads,Writes,Calls;
        public ushort TargetSegment,TargetOffset;
        public ushort[] Arguments;
        public ushort DataSegment { get { return Segment; } }
        public bool DirectionFlag { get { return Df; } }
        public ushort ReadWord(ushort segment,ushort offset)
        {
            if(segment!=Segment)throw new Exception("Uncaptured segment");
            if(offset==ParameterOffset)ParameterReads++;
            return (ushort)(Data[offset]|Data[unchecked((ushort)(offset+1))]<<8);
        }
        public void WriteWord(ushort segment,ushort offset,ushort value)
        {
            if(segment!=Segment)throw new Exception("Unexpected destination segment");
            Writes++;Data[offset]=(byte)value;Data[unchecked((ushort)(offset+1))]=(byte)(value>>8);
        }
        public ushort CallFar(ushort segment,ushort offset,ushort[] arguments)
        {
            Calls++;TargetSegment=segment;TargetOffset=offset;Arguments=arguments;
            // Stop before supplying ANY inferred behavior for this original callee.
            throw new AtOriginalCallBoundary();
        }
    }
    static int Main(string[] args)
    {
        byte[] before=File.ReadAllBytes(args[0]),after=File.ReadAllBytes(args[1]),entryStack=File.ReadAllBytes(args[2]),calleeStack=File.ReadAllBytes(args[3]);
        ushort originalSp=Convert.ToUInt16(args[4],16),ds=Convert.ToUInt16(args[5],16);
        bool df=args[6]=="1";
        if(before.Length!=65536||after.Length!=65536)throw new Exception("Capture size");
        ushort pointer=BitConverter.ToUInt16(entryStack,4),pointerSegment=BitConverter.ToUInt16(entryStack,6);
        ushort originalState=BitConverter.ToUInt16(before,pointer);
        var context=new Context {Data=(byte[])before.Clone(),Segment=ds,Df=df,ParameterOffset=pointer};
        bool reached=false;
        try{SelectorRoutine.Execute(context,pointerSegment,pointer);}catch(AtOriginalCallBoundary){reached=true;}
        if(!reached||context.Calls!=1||context.Writes!=3||context.ParameterReads!=2)throw new Exception("Control-flow/event mismatch");
        if(context.TargetSegment!=0x1f08||context.TargetOffset!=0x2ac6||context.Arguments.Length!=3)throw new Exception("Call target mismatch");
        for(int i=0;i<3;i++)if(context.Arguments[i]!=BitConverter.ToUInt16(calleeStack,4+2*i))throw new Exception("Actual original stack argument mismatch");
        int checkedBytes=0;
        for(int i=0;i<65536;i++)
        {
            // Exactly the original selector's 6-byte saved frame, 6-byte
            // argument pushes and 4-byte far return address. Native ABI is
            // outside this semantic fragment; no other differences are masked.
            if(i>=originalSp-16&&i<originalSp)continue;
            if(context.Data[i]!=after[i])throw new Exception("Actual original DS mismatch at "+i.ToString("x4"));
            checkedBytes++;
        }
        Console.WriteLine("PASS: original runtime selector state "+originalState+", DF="+(df?1:0)+"; native copy/control flow and all three actual call-argument words match.");
        Console.WriteLine("Compared "+checkedBytes+" DS bytes; excluded exactly 16 known physical-stack bytes. Callee was not executed or simulated by the native proof.");
        return 0;
    }
}
