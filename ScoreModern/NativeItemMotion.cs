using System;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativeItemMotion {
  static bool Nonzero(Item item,int n){return BitConverter.ToInt32(BitConverter.GetBytes(item.P(n)),0)!=0;}
  static short Word(float value){long result;if(!FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out result))throw new NotSupportedException("Movement control outside the verified finite conversion range.");return unchecked((short)result);}
  static void Add(Item item,int n,float value){item.Set(n,item.P(n)+value);}
  // Positive-only fields use original2788:14EE..1548; zero/negative sentinels stay unchanged.
  // Whole-item horizontal path: original overlay5 segment2788:1586..1858.
  public static void Horizontal(Item item,float distance){
   if(distance!=0)TransformHorizontal(item,value=>value+distance);
  }
  internal static void TransformHorizontal(Item item,Func<float,float> transform){
   int code=item.Code,count=item.Count;
   short beamControl=code==6?Word(item.P(7)):(short)0;
   Action<int> apply=n=>item.Set(n,transform(item.P(n)));Action<int> positive=n=>{if(item.P(n)>0)apply(n);};
   apply(3);
   if(code==4||code==5||code==6){if(code==6&&count>=11&&beamControl%10!=0){positive(11);if(beamControl>=0){if(count>=12)positive(12);if(count>=14)positive(14);if(count>=15)positive(15);}}positive(6);if(code==4&&item.P(7)==100)for(int n=8;n<=16;n+=2){if(!Nonzero(item,n))break;apply(n);}}
   else if(code==7&&count>=6){if(count>=8&&item.P(7)>=20)positive(8);positive(6);}
   else if(code==8)positive(6);
  }
  public static void Vertical(Item item,float distance){if(distance==0||item.Code==14)return;Add(item,4,distance);if(item.Code==5||item.Code==6||item.Code==4&&item.P(5)<999){if(item.Code==4&&item.P(7)==100)for(int n=9;n<=17;n+=2){if(!Nonzero(item,n-1))break;Add(item,n,distance);}Add(item,5,distance);}}
  internal static void CheckEndpointMotion(){
   var polyline=Item.Make(4,1,20,8,9,50,100,35,11,42,13);
   MoveEndpoint(polyline,1,1,1);
   if(!System.Linq.Enumerable.SequenceEqual(polyline.Bytes,Item.Make(4,1,21,9,9,50,100,36,11,43,13).Bytes))
    throw new InvalidOperationException("DOS left polyline endpoint differs.");
   MoveEndpoint(polyline,2,1,1);
   if(!System.Linq.Enumerable.SequenceEqual(polyline.Bytes,Item.Make(4,1,21,9,10,51,100,37,12,44,14).Bytes))
    throw new InvalidOperationException("DOS right polyline endpoint differs.");
   var hairpin=Item.Make(4,1,56,7,999,90,1);
   MoveEndpoint(hairpin,2,1,1);
   if(!System.Linq.Enumerable.SequenceEqual(hairpin.Bytes,Item.Make(4,1,56,8,999,91,1).Bytes))
    throw new InvalidOperationException("DOS sentinel hairpin endpoint differs.");
   var beam=Item.Make(6,1,22,8,9,49,1,0,0,0,25,45,0,28,43);
   MoveEndpoint(beam,3,1,1);
   if(!System.Linq.Enumerable.SequenceEqual(beam.Bytes,Item.Make(6,1,22,9,10,49,1,0,0,0,25,45,0,28,43,.51f).Bytes))
    throw new InvalidOperationException("DOS beam midpoint differs.");
  }
  // Overlay 05 branches on the edit lock before choosing the parameter slots.
  internal static void MoveEndpoint(Item item,int endpoint,float x,float y){
   if(endpoint==3){
    if(item.Code!=5&&item.Code!=6)throw new NotSupportedException("Midpoint movement requires a slur or beam.");
    if(x!=0){
     int parameter=item.Code==5?10:16;
     float position=Nonzero(item,parameter)?item.P(parameter):.5f;
     item.Set(parameter,position+(float)((double)x/100));
    }
    if(y!=0){
     if(item.Code==5)Add(item,7,y);
     else Vertical(item,y);
    }
    return;
   }
   bool right=endpoint==2;
   // Original UI01: Codes7/8 lock only their horizontal endpoint.
   // Both vertical locks move P4; P5 is not a right-end ordinate.
   if(item.Code==7||item.Code==8){
    if(x!=0)Add(item,right&&item.P(6)>0?6:3,x);
    if(y!=0)Add(item,4,y);return;
   }
   if(item.Code<4||item.Code>6)throw new NotSupportedException("Endpoint movement requires a line, slur or beam.");
   bool polyline=item.Code==4&&item.P(7)==100;
   if(x!=0){
    if(!right||item.P(6)>0)Add(item,right?6:3,x);
    if(polyline)for(int n=8;n<=16&&Nonzero(item,n);n+=2)Add(item,n,x);
    if(item.Code==6&&item.Count>=11){
     short control=Word(item.P(7));
     if(control%10!=0){
      if(!right&&item.P(11)>0)Add(item,11,x);
      if(control>=0&&right&&item.Count>=12&&item.P(12)>0)Add(item,12,x);
      if(control>=0&&!right&&item.Count>=14&&item.P(14)>0)Add(item,14,x);
      if(control>=0&&right&&item.Count>=15&&item.P(15)>0)Add(item,15,x);
     }
    }
   }
   if(y!=0){
    if(right&&item.Code==4&&item.P(5)>=999){Add(item,4,y);return;}
    if(right&&polyline)for(int n=9;n<=17&&Nonzero(item,n-1);n+=2)Add(item,n,y);
    Add(item,right?5:4,y);
   }
  }
 }
}
