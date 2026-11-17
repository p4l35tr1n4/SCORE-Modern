using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ScoreModern {
 internal static class NativeDrawDocument {
  internal const int Capacity=4000;

  internal static IEnumerable<Tuple<int,int,int>> FillPreview(Glyph glyph,float originX,float originY,
    float zoomX,float zoomY,float increment,int viewportWidth){
   if(zoomX<=0||zoomY<=0||viewportWidth<=0)yield break;
   int start=Array.IndexOf(glyph.Operations,(short)2);
   if(start<0)yield break;
   int end=start+1;
   while(end<glyph.Points.Length&&glyph.Operations[end]!=1&&glyph.Operations[end]!=2)end++;
   float left=glyph.Points[start].X,right=left;
   var edges=new List<int>();
   for(int i=start+1;i<end;i++){
    PointF point=glyph.Points[i];
    left=Math.Min(left,point.X);
    right=Math.Max(right,point.X);
    if(glyph.Operations[i]!=3&&point.X!=glyph.Points[i-1].X)edges.Add(i);
   }
   float step=(increment>0?increment:3)/zoomX;
   var crossings=new List<float>();
   for(float scan=(float)(left-.5);;){
    float next=(float)((double)scan+step);
    if(next>=right||next<=scan)yield break;
    scan=next;
    int x=(int)((double)originX+(double)scan*zoomX);
    if(x>=viewportWidth)yield break;
    if(x<0)continue;
    crossings.Clear();
    foreach(int edge in edges){
     PointF a=glyph.Points[edge-1],b=glyph.Points[edge];
     if(scan<=Math.Min(a.X,b.X)||scan>Math.Max(a.X,b.X))continue;
     float product=(float)(((double)b.Y-a.Y)*((double)scan-a.X));
     crossings.Add((float)((double)product/(b.X-a.X)+a.Y));
    }
    crossings.Sort();
    for(int i=1;i<crossings.Count;i+=2){
     int top=(int)originY-(int)Math.Round((double)crossings[i]*zoomY,MidpointRounding.AwayFromZero);
     int bottom=(int)originY-(int)Math.Round((double)crossings[i-1]*zoomY,MidpointRounding.AwayFromZero);
     yield return Tuple.Create(x,top,bottom);
    }
   }
  }

  internal static Glyph ReadVectors(string path){
   var points=new List<PointF>();
   var operations=new List<short>();
   int lineNumber=0;
   foreach(string line in File.ReadLines(path,Encoding.GetEncoding(437))){
    if(points.Count==Capacity)break;
    lineNumber++;
    if(String.IsNullOrWhiteSpace(line))continue;
    var parts=line.Length==15?new[]{line.Substring(0,5),line.Substring(5,5),line.Substring(10,5)}:
     line.Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
    short x,y,operation;
    if(parts.Length!=3||!TryWord(parts[0],out x)||!TryWord(parts[1],out y)||
       !TryWord(parts[2],out operation)||operation< -3||operation>3)
     throw new InvalidDataException("Invalid DRAW vector on line "+lineNumber+".");
    points.Add(new PointF(x,y));
    operations.Add(operation);
   }
   return new Glyph{Source=path,Name="SHAPE",Points=points.ToArray(),Operations=operations.ToArray()};
  }

  static bool TryWord(string value,out short word){
   return Int16.TryParse(value,NumberStyles.Integer,CultureInfo.InvariantCulture,out word);
  }

  internal static void Move(Glyph glyph,int dx,int dy){
   Transform(glyph,point=>(double)point.X+dx,point=>(double)point.Y+dy);
  }

  internal static void Resize(Glyph glyph,float x,float y){
   Transform(glyph,point=>(double)point.X*x,point=>(double)point.Y*y);
  }

  internal static void Rotate(Glyph glyph,float degrees){
   float radians=(float)((double)degrees/57.2957763671875);
   float cosine=(float)Math.Cos(radians),sine=(float)Math.Sin(radians);
   Transform(glyph,point=>(double)point.X*cosine+(double)point.Y*sine,
    point=>(double)point.Y*cosine-(double)point.X*sine);
  }

  internal static void Clean(Glyph glyph){
   var points=glyph.Points.ToList();
   var operations=glyph.Operations.ToList();
   int previousSlope=BitConverter.ToInt32(BitConverter.GetBytes(-9999f),0);
   for(int i=1;i<points.Count;i++){
    double dy=(double)points[i].Y-points[i-1].Y;
    float slope=dy==0?9999:(float)(((double)points[i].X-points[i-1].X)/dy);
    int bits=BitConverter.ToInt32(BitConverter.GetBytes(slope),0);
    if(bits!=previousSlope)previousSlope=bits;
    else if(operations[i-1]==0){
     points.RemoveAt(i-1);operations.RemoveAt(i-1);
     i--;
    }
   }
   glyph.Points=points.ToArray();glyph.Operations=operations.ToArray();
  }

  internal static void Reorder(Glyph glyph){
   if(glyph.Points.Length==0)return;
   if(glyph.Operations[0]<=0)
    throw new InvalidDataException("The first vector must start a contour before reordering.");
   var starts=Enumerable.Range(0,glyph.Points.Length).Where(index=>glyph.Operations[index]>0)
    .Concat(new[]{glyph.Points.Length}).ToArray();
   var order=Enumerable.Range(0,starts.Length-1).OrderBy(index=>glyph.Points[starts[index]].X)
    .SelectMany(index=>Enumerable.Range(starts[index],starts[index+1]-starts[index])).ToArray();
   glyph.Points=order.Select(index=>glyph.Points[index]).ToArray();
   glyph.Operations=order.Select(index=>glyph.Operations[index]).ToArray();
  }

  internal static void AddArc(Glyph glyph,int first,int last,PointF start,float radiusX,float radiusY,int step){
   if(first< -359||first>360||last<0||last>361||step<1||step>360)
    throw new InvalidDataException("Enter valid degree bounds and a positive degree increment.");
   if(first<0)first+=360;
   if(last<first)last+=360;
   if(first==0)first=1;
   if(last==0)last=361;
   if(radiusX==0)radiusX=50;
   if(radiusY==0)radiusY=radiusX;
   float xScale=(float)((double)radiusX/100),yScale=(float)((double)radiusY/100);
   PointF firstPoint=CirclePoint(first-1);
   float cx=(float)((double)start.X-(float)((double)firstPoint.X*xScale));
   float cy=(float)((double)start.Y-(float)((double)firstPoint.Y*yScale));
   var points=new List<PointF>(glyph.Points);
   var operations=new List<short>(glyph.Operations);
   int original=points.Count;
   PointF previous=new PointF(-9999,-9999);
   for(int degree=first;degree<=last;){
    PointF circle=CirclePoint(degree-1);
    double x=Math.Round((double)circle.X*xScale+cx,MidpointRounding.AwayFromZero);
    double y=Math.Round((double)circle.Y*yScale+cy,MidpointRounding.AwayFromZero);
    if(!(x>=Int16.MinValue&&x<=Int16.MaxValue&&y>=Int16.MinValue&&y<=Int16.MaxValue))
     throw new InvalidDataException("The arc leaves DRAW's coordinate range.");
    var point=new PointF((float)x,(float)y);
    if(points.Count==original||previous!=point){
     if(points.Count==Capacity)throw new InvalidDataException("The arc exceeds DRAW's 4,000-vector capacity.");
     points.Add(point);
     operations.Add(points.Count==original+1?(short)1:(short)0);
    }else operations[operations.Count-1]=0;
    previous=point;
    degree+=step;
    if(degree<=last&&degree>=362){degree=1;last-=361;}
   }
   glyph.Points=points.ToArray();glyph.Operations=operations.ToArray();
  }

  static PointF CirclePoint(int degree){
   // The DOS lookup table stores sine and cosine times 100 to three
   // decimals. Retain the measured exceptions to ordinary rounding.
   switch(degree){
    case 53:return new PointF(79.864f,60.181f);
    case 122:return new PointF(84.805f,-52.99f);
    case 143:return new PointF(60.181f,-79.864f);
    case 233:return new PointF(-79.864f,-60.181f);
    case 241:return new PointF(-87.46f,-48.48f);
    case 243:return new PointF(-89.1f,-45.399f);
    case 318:return new PointF(-66.913f,74.315f);
    case 323:return new PointF(-60.181f,79.864f);
    case 324:return new PointF(-58.778f,80.902f);
    case 337:return new PointF(-39.073f,92.051f);
   }
   double theta=degree*(Math.PI/180);
   return new PointF((float)Math.Round(100*Math.Sin(theta),3),(float)Math.Round(100*Math.Cos(theta),3));
  }

  static void Transform(Glyph glyph,Func<PointF,double> newX,Func<PointF,double> newY){
   var moved=new PointF[glyph.Points.Length];
   for(int index=0;index<moved.Length;index++){
    PointF point=glyph.Points[index];
    double x=Math.Round(newX(point),MidpointRounding.AwayFromZero),y=Math.Round(newY(point),MidpointRounding.AwayFromZero);
    if(!(x>=Int16.MinValue&&x<=Int16.MaxValue&&y>=Int16.MinValue&&y<=Int16.MaxValue))
     throw new InvalidDataException("The operation places a vector outside DRAW's coordinate range.");
    moved[index]=new PointF((float)x,(float)y);
   }
   glyph.Points=moved;
  }

  internal static byte[] WriteVectors(Glyph glyph){
   var text=new StringBuilder();
   for(int index=0;index<glyph.Points.Length;index++){
    PointF point=glyph.Points[index];
    text.Append(((short)point.X).ToString(CultureInfo.InvariantCulture).PadLeft(5))
     .Append(((short)point.Y).ToString(CultureInfo.InvariantCulture).PadLeft(5))
     .Append(glyph.Operations[index].ToString(CultureInfo.InvariantCulture).PadLeft(5)).Append("\r\n");
   }
   return Encoding.ASCII.GetBytes(text.ToString());
  }

  internal static string ValidateName(string value){
   string name=value.Trim().ToUpperInvariant();
   if(name.Length<1||name.Length>5||name.Any(c=>c<33||c>126))
    throw new InvalidDataException("Enter a DRAW item name of one to five printable characters without spaces.");
   return name;
  }

  internal static byte[] RenameItem(string path,int slot,string value){
   var entries=DrawingLibrary.Load(path);
   if(slot<0||slot>=entries.Length||entries[slot].Name.Length==0)
    throw new InvalidDataException("Choose an existing bank item to rename.");
   string name=ValidateName(value);
   if(entries.Where((entry,index)=>index!=slot)
    .Any(entry=>String.Equals(entry.Name,name,StringComparison.OrdinalIgnoreCase)))
    throw new InvalidDataException("This bank already has an item called "+name+".");
   var bytes=File.ReadAllBytes(path);
   Encoding.GetEncoding(437).GetBytes(name.PadRight(5)).CopyTo(bytes,24+slot*5);
   return bytes;
  }

  internal static byte[] DeleteItem(string path,int slot){
   var entries=DrawingLibrary.Load(path);
   if(slot<0||slot>=entries.Length||entries[slot].Name.Length==0)
    throw new InvalidDataException("Choose an existing bank item to delete.");
   var records=FortranRecords.Read(File.ReadAllBytes(path),true);
   byte[] header=records[0],data=records[1];
   int start=BitConverter.ToUInt16(header,slot*2)-1;
   int words=entries[slot].Points.Length*3;
   var remaining=new byte[data.Length-words*2];
   Buffer.BlockCopy(data,0,remaining,0,start*2);
   Buffer.BlockCopy(data,(start+words)*2,remaining,start*2,remaining.Length-start*2);
   for(int index=slot+1;index<9;index++){
    ushort pointer=unchecked((ushort)(BitConverter.ToUInt16(header,(index+1)*2)-words));
    BitConverter.GetBytes(pointer).CopyTo(header,index*2);
   }
   byte[] end=BitConverter.GetBytes(checked((ushort)(remaining.Length/2+1)));
   if(slot<9&&entries[9].Name.Length!=0&&!NativeEditCompatibility.ReproduceInvalidStates)
    end.CopyTo(header,18);
   end.CopyTo(header,20);
   int nameStart=22+slot*5;
   Array.Copy(header,nameStart+5,header,nameStart,header.Length-nameStart-5);
   for(int index=header.Length-5;index<header.Length;index++)header[index]=32;
   return FortranRecords.Write(header,remaining);
  }

  internal static byte[] WriteBank(IList<Glyph> entries){
   if(entries.Count!=10)throw new ArgumentException("DRAW banks contain ten slots.");
   if(entries.Where(entry=>entry!=null&&entry.Name.Length!=0).Sum(entry=>entry.Points.Length)>Capacity)
    throw new InvalidDataException("The bank exceeds DRAW's 4,000-vector capacity.");
   using(var header=new MemoryStream())
   using(var data=new MemoryStream())
   using(var h=new BinaryWriter(header))
   using(var v=new BinaryWriter(data)){
    var pointers=new ushort[11];
    for(int slot=0;slot<entries.Count;slot++){
     var glyph=entries[slot];
     if(glyph==null||String.IsNullOrWhiteSpace(glyph.Name))continue;
     if(glyph.Points.Length>Capacity||glyph.Points.Length!=glyph.Operations.Length)
      throw new InvalidDataException("DRAW item exceeds the vector capacity or has inconsistent coordinates.");
     pointers[slot]=checked((ushort)(data.Length/2+1));
     for(int n=0;n<glyph.Points.Length;n++){
      v.Write(checked((short)glyph.Points[n].X));
      v.Write(checked((short)glyph.Points[n].Y));
      v.Write(glyph.Operations[n]);
     }
     pointers[slot+1]=checked((ushort)(data.Length/2+1));
    }
    pointers[0]=1;
    pointers[10]=checked((ushort)(data.Length/2+1));
    foreach(ushort pointer in pointers)h.Write(pointer);
    foreach(var glyph in entries)
     h.Write(Encoding.GetEncoding(437).GetBytes((glyph==null?"":glyph.Name).PadRight(5)));
    return FortranRecords.Write(header.ToArray(),data.ToArray());
   }
  }

 }
}
