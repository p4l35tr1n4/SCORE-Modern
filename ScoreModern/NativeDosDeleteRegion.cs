using System;
using System.Collections.Generic;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal static class NativeDosDeleteRegion {
  // 316D:0A3A takes the removed vector extent; 23F5:0042/018B
  // clears the inclusive, byte-aligned VGA rectangle.
  internal static void Replay(NativeDosRaster current,NativeFreshDisplayArray before,int ordinal,NativeFreshDisplayArray after,short bottom,bool full){
   int lowX=32760,highX=-32760,lowY=32760,highY=-32760;
   for(int n=before.Pointers[ordinal-1];n<before.Pointers[ordinal];n++){
    int x=before.X[n];if(x< -32600)continue;
    int y=before.Y[n-1];lowX=Math.Min(lowX,x);highX=Math.Max(highX,x);lowY=Math.Min(lowY,y);highY=Math.Max(highY,y);
   }
   if(highX<lowX)return;
   int left=Clamp(lowX/current.HorizontalDivisor+current.OriginX,full?0:1,full?639:638);
   int right=Clamp(highX/current.HorizontalDivisor+current.OriginX,full?0:1,full?639:638);
   int top=Clamp(current.OriginY-highY/current.VerticalDivisor,current.GraphicsTop,bottom);
   int end=Clamp(current.OriginY-lowY/current.VerticalDivisor+1,current.GraphicsTop,bottom);
   var replay=new NativeDosRaster(current.HorizontalDivisor,current.VerticalDivisor,current.OriginX,current.OriginY,current.GraphicsTop,bottom,full);
   Append(replay,after,1,after.X.Length);
   int start=left/8,length=Math.Min(80-start,(right-left)/8+2);
   for(int y=top;y<=end;y++)Buffer.BlockCopy(replay.Plane,y*80+start,current.Plane,y*80+start,length);
  }
  internal static void Append(NativeDosRaster raster,NativeFreshDisplayArray array,int start,int end){
   var points=new List<PrimitiveTextPoint>();bool move=false;
   for(int n=start;n<end;n++){
    int x=array.X[n];
    if(x<= -32600){move=true;continue;}
    points.Add(new PrimitiveTextPoint{X=x,Y=array.Y[n-1],Operation=(short)(move?1:0)});move=false;
   }
   raster.Draw(points);
  }
  static int Clamp(int value,int min,int max){return Math.Max(min,Math.Min(max,value));}
 }
}
