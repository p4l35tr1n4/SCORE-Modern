using System;using ScoreModern.RecoveredFragments;
class RasterArithmeticFlagsProof {
 static int Main(){try{int count=0;uint f=0xA55AF71Bu;for(int a=0;a<256;a++)for(int b=0;b<256;b++){if(RasterStatusFlags.Sub8(f,(byte)a,(byte)b)!=IndependentFlagArithmetic.Apply(f,(ushort)a,(ushort)b,8,false))throw new Exception("SUB8");count++;}
 ushort[] values={0,1,0x7fff,0x8000,0xffff};for(int a=0;a<65536;a++)foreach(ushort b in values){if(RasterStatusFlags.Add16(f,(ushort)a,b)!=IndependentFlagArithmetic.Apply(f,(ushort)a,b,16,true)||RasterStatusFlags.Sub16(f,(ushort)a,b)!=IndependentFlagArithmetic.Apply(f,(ushort)a,b,16,false))throw new Exception("Word arithmetic");count+=2;}
 for(int a=0;a<65536;a++)for(uint cf=0;cf<2;cf++){uint flags=(f&~1u)|cf;if(RasterStatusFlags.Inc16(flags,(ushort)a)!=((IndependentFlagArithmetic.Apply(flags,(ushort)a,1,16,true)&~1u)|cf)||RasterStatusFlags.Dec16(flags,(ushort)a)!=((IndependentFlagArithmetic.Apply(flags,(ushort)a,1,16,false)&~1u)|cf))throw new Exception("INC/DEC carry");count+=2;}
 for(int a=0;a<256;a++){if(RasterStatusFlags.Logical8(f,(byte)a)!=IndependentFlagArithmetic.Result(f,0,(ushort)a,8))throw new Exception("Logical");count++;}
 Console.WriteLine("PASS "+count+" independent arithmetic flag comparisons: all byte subtraction operands, word signed boundaries, all INC/DEC inputs with both carry states, every logical result byte.");return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
