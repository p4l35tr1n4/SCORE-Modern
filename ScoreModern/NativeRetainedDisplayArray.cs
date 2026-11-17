using System;using System.Collections.Generic;using System.Drawing;
namespace ScoreModern {
 // Analysis/integration model only. The owner must establish zeroed startup
 // storage and supply every display producer in order. DPY uses verified load
 // histories and verified note/staff scalar edit routes. Other producers remain
 // guarded until their ordered storage effects have been measured.
 public sealed class NativeRetainedDisplayArray {
  readonly short[] x=new short[31001],y=new short[31000];
  readonly NativeDatHeadCache heads=new NativeDatHeadCache(true);
  int itemStart;
  int used;
  int previewLength;
  int[] lastPointers;
  bool drawingVectors;
  public NativeRetainedDisplayArray(){heads.ReadMarkerY=offset=>y[checked(itemStart+offset)];}
  void WriteMarker(int at,ScoreModern.RecoveredFragments.PrimitiveTextPoint point){
   x[at+1]=unchecked((short)(point.Operation-32700));
   if(point.MarkerWordBack>0)y[at]=y[at-point.MarkerWordBack];
   else if(point.RetainedMarkerY.HasValue)y[at]=point.RetainedMarkerY.Value;
  }
  public NativeFreshDisplayArray Produce(Canvas canvas){
   if(canvas==null||canvas.Score==null)throw new ArgumentNullException("canvas");
   if(drawingVectors!=canvas.TextLayout.ShowActualDatFonts){
    drawingVectors=canvas.TextLayout.ShowActualDatFonts;
    heads.Reset();
   }
   var prior=NativeNotationView.Mode;var pointers=new List<int>();int count=0;
   using(canvas.TextLayout.PreserveAlignmentMetricHistory())try{
    NativeNotationView.SetForScan(NotationView.Dat);canvas.LayoutScore();var dat=new NativeDatGlyphs(canvas.Library.Root,drawingVectors);short font=0;
    foreach(var item in canvas.Score.Items){
     pointers.Add(count+1);
     if(canvas.DisplayHidden(item))continue;
     itemStart=count;heads.MarkerPrefix=0;short next;
     var points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,dat,heads,font,out next,drawingVectors);font=next;
     if(item.Code==8){if(item.Staff==1)points.InsertRange(0,NativeScanStructuralVectors.ScreenGuide(item,canvas.PageDocument));if(!canvas.TextLayout.ShowActualDatFonts)foreach(var number in NativeDosOverlays.Numbers(canvas.Score,canvas.PageDocument,canvas.TextLayout,canvas.DisappearingStaffs))if(number.Staff==item.Staff)points.AddRange(number.Points);}
     foreach(var p in points){
      if(p.Operation>0||p.CommandOnly){if(count>=31000)throw new NotSupportedException("Retained display capacity exceeded.");WriteMarker(count,p);count++;}
      if(!p.CommandOnly){if(count>=31000)throw new NotSupportedException("Retained display capacity exceeded.");x[count+1]=NativeNotationContext.Word(p.X,true);y[count]=NativeNotationContext.Word(p.Y,false);count++;}
     }
    }
   }finally{NativeNotationView.SetForScan(prior);}
   pointers.Add(count+1);used=count;lastPointers=pointers.ToArray();x[0]=checked((short)(count+1));return Snapshot();
  }
  NativeFreshDisplayArray Snapshot(){var resultX=new short[used+1];var resultY=new short[used];Array.Copy(x,resultX,resultX.Length);Array.Copy(y,resultY,resultY.Length);return new NativeFreshDisplayArray{X=resultX,Y=resultY,Pointers=(int[])lastPointers.Clone()};}
  // Isolated edit-producer integration surface. The ordinary edit scanner
  // appends working vectors beyond the declared active end. Commit compacts
  // the tail including that working block and its following word, then replaces
  // the indexed record. Inclusive tail copy matches both complete captured banks.
  internal int PreviewItem(Canvas canvas,int ordinal,int code){
   if(lastPointers==null||ordinal<1||ordinal>=lastPointers.Length||canvas.Score.Items.Count!=lastPointers.Length-1)throw new NotSupportedException("Display preview pointer history is not available.");
   var item=canvas.Score.Items[ordinal-1];if(item.Code!=code||code==1&&!SupportsNotePreview(item))throw new NotSupportedException("Display edit producer controls need verification.");
   var prior=NativeNotationView.Mode;List<ScoreModern.RecoveredFragments.PrimitiveTextPoint> points;
   using(canvas.TextLayout.PreserveAlignmentMetricHistory())try{NativeNotationView.SetForScan(NotationView.Dat);canvas.LayoutScore();itemStart=used;heads.MarkerPrefix=0;short next;points=NativeDatDrawing.Prepare(item,canvas.PageDocument.Contexts[item],canvas.PageDocument,canvas.Library,canvas.TextLayout,new NativeDatGlyphs(canvas.Library.Root),heads,0,out next);if(code==8){if(item.Staff==1)points.InsertRange(0,NativeScanStructuralVectors.ScreenGuide(item,canvas.PageDocument));if(!canvas.TextLayout.ShowActualDatFonts)foreach(var number in NativeDosOverlays.Numbers(canvas.Score,canvas.PageDocument,canvas.TextLayout,canvas.DisappearingStaffs))if(number.Staff==item.Staff)points.AddRange(number.Points);}}finally{NativeNotationView.SetForScan(prior);}
   return WritePreviewPoints(points);
  }
  internal void ReorderItem(Canvas canvas,int from,int to){
   try{
    int start=lastPointers[from-1]-1,end=lastPointers[from]-1;
    int length=PreviewItem(canvas,from,canvas.Score.Items[from-1].Code);
    if(length!=end-start)throw new NotSupportedException("Display reorder changes the active vector length.");
    int destination=lastPointers[to-1]-1;
    if(from<to){
     int next=lastPointers[to]-1;
     Array.Copy(x,end+1,x,start+1,next-end);
     Array.Copy(y,end,y,start,next-end);
     destination=next-length;
     for(int i=from-1;i<to-1;i++)lastPointers[i]=lastPointers[i+1]-length;
    }else{
     Array.Copy(x,destination+1,x,destination+length+1,start-destination);
     Array.Copy(y,destination,y,destination+length,start-destination);
     for(int i=from-1;i>=to;i--)lastPointers[i]=lastPointers[i-1]+length;
    }
    Array.Copy(x,used+1,x,destination+1,length);
    Array.Copy(y,used,y,destination,length);
    lastPointers[to-1]=destination+1;
   }
   finally{previewLength=0;}
  }
  internal void WriteInputStaffGuides(Canvas canvas,int staff,float leftPosition){
   // 2E85:015A–0215 draws the upper and lower lines through 14DF:0567,
   // then puts the active end back at its prior value.
   float scale=canvas.PageDocument.Tables.Scales[staff];
   float x=(float)(Math.Max(15.0,leftPosition)*150),half=(float)(5.0*scale);
   float left=(float)((double)x-half),right=(float)((double)x+half);
   float step=(float)(14.0*scale);
   var points=new List<ScoreModern.RecoveredFragments.PrimitiveTextPoint>();
   for(int band=0;band<2;band++){
    float y=canvas.PageDocument.Tables.Vertical(staff,band==0?21:-7);
    for(int line=0;line<5;line++){
     points.Add(new ScoreModern.RecoveredFragments.PrimitiveTextPoint{X=left,Y=y,Operation=1});
     points.Add(new ScoreModern.RecoveredFragments.PrimitiveTextPoint{X=right,Y=y});
     y=(float)((double)y+(band==0?-step:step));
    }
   }
   WriteInputScratch(points);
  }
  internal void WriteInputScratch(IEnumerable<ScoreModern.RecoveredFragments.PrimitiveTextPoint> points){
   try{WritePreviewPoints(points);}
   finally{previewLength=0;}
  }
  int WritePreviewPoints(IEnumerable<ScoreModern.RecoveredFragments.PrimitiveTextPoint> points){
   int at=used;foreach(var p in points){if(p.Operation>0||p.CommandOnly){if(at>=31000)throw new NotSupportedException("Display edit scratch capacity exceeded.");WriteMarker(at,p);at++;}if(!p.CommandOnly){if(at>=31000)throw new NotSupportedException("Display edit scratch capacity exceeded.");x[at+1]=NativeNotationContext.Word(p.X,true);y[at]=NativeNotationContext.Word(p.Y,false);at++;}}
   previewLength=at-used;return previewLength;
  }
  internal void WriteCenteredRestPreviews(Canvas canvas,IEnumerable<int> ordinals){
   var prior=NativeNotationView.Mode;
   using(canvas.TextLayout.PreserveAlignmentMetricHistory())try{
    NativeNotationView.SetForScan(NotationView.Dat);
    canvas.LayoutScore();
    var dat=new NativeDatGlyphs(canvas.Library.Root);
    // Original 1797:0AC0 submits each rest to the shared vector writer.
    // The active end is unchanged, so a shorter later rest overwrites only
    // the beginning of the previous preview in the retained X/Y tail.
    foreach(int index in ordinals){
     var rest=canvas.Score.Items[index];
     if(rest.Code!=2)throw new NotSupportedException("Display centering preview requires a rest.");
     WritePreviewPoints(NativeDatRestAttachments.PrepareItem(rest,canvas.PageDocument.Contexts[rest],dat,canvas.Library,canvas.TextLayout));
    }
   }finally{previewLength=0;NativeNotationView.SetForScan(prior);}
  }
  internal bool CanTrackNote(Canvas canvas,Item item){NativeNotationContext context;if(canvas.PageDocument==null||!canvas.PageDocument.Contexts.TryGetValue(item,out context))return false;float scale,x,y;NativeScanMusicVectors.HeadProjection(item,context,out scale,out x,out y);return heads.MatchesScale(scale);}
  internal NativeFreshDisplayArray ReadActive(){return Snapshot();}
  internal Point? RetainedLastItemPoint(){
   if(lastPointers==null||lastPointers.Length<2)return null;
   int index=lastPointers[lastPointers.Length-2]+1;
   if(index>=x.Length)return null;
   return new Point(x[index],y[index-1]);
  }
  internal static bool SupportsNote(Item item){
   if(item==null||item.Code!=1||item.Count<7||item.Count>9||
      item.P(5)!=10&&item.P(5)!=20||item.P(6)!=0||item.P(8)!=0||!(Math.Abs(item.P(4))<100))return false;
   float duration=item.P(7),flags=item.P(9);
   return duration==2&&flags==0||duration==1&&(flags==0||flags==1)||
    duration==.5f&&flags==1||duration==.25f&&flags==2||
    duration==.125f&&flags==3||duration==.0625f&&flags==4;
  }
  internal static bool SupportsNotePreview(Item item){
   if(SupportsNote(item))return true;
   if(item==null||item.Code!=1||item.Count<7||item.Count>9||!(Math.Abs(item.P(4))<100))return false;
   float style=item.P(5),duration=item.P(7),stem=item.P(8),flags=item.P(9);
   if(style==0)return item.P(6)==2&&duration==4&&(stem==0||stem==1)&&flags==0;
   if(item.P(6)!=0||style!=10&&style!=20&&style!=22)return false;
   return duration==.25f&&Math.Abs(stem)<100&&flags==0||duration==1.5f&&stem==0&&flags==10;
  }
  internal bool WriteSingleNoteNormalizationScratch(Score input,int staff){
   // Original 2DC2:0A80..0ABF, 0B2A..0B40. Its isolated-note
   // pass aliases the duration, position, mode and ordinal vectors into Y.
   var ordinals=new List<int>();
   for(int n=0;n<input.Items.Count;n++)if(input.Items[n].Staff==staff&&input.Items[n].Code==1)ordinals.Add(n);
   if(ordinals.Count==0||ordinals.Count>100)return false;
   for(int n=0;n<ordinals.Count;n++){
    var item=input.Items[ordinals[n]];
    WriteYSingle(200+2*n,item.P(7));
    y[400+n]=0;
    WriteYSingle(800+2*n,item.P(3));
    y[1000+n]=checked((short)(ordinals[n]+1));
   }
   return true;
  }
  internal void WriteSeparatedChordNormalizationScratch(System.Collections.Generic.IList<Item> records,int staff){
   int slot=0;
   for(int index=0;index<records.Count;){
    var item=records[index];int next=index+1;
    if(item.Code!=1||item.Staff!=staff||item.P(3)<0||item.P(3)>220){index=next;continue;}
    while(next<records.Count&&records[next].Code==1&&records[next].Staff==staff&&records[next].P(3)==item.P(3))next++;
    int root=index;
    while(root<next&&(int)records[root].P(5)%100/10==0)root++;
    if(root==next)throw new NotSupportedException("SEP display normalization requires a chord root.");
    for(int n=0;n<next-index;n++){
     int ordinal=index+(root-index+n)%(next-index);
     var note=records[ordinal];
     WriteYSingle(200+2*slot,n==0?note.P(7):0);
     y[400+slot]=0;
     WriteYSingle(800+2*slot,note.P(3));
     y[1000+slot]=checked((short)(ordinal+1));
     slot++;
    }
    index=next;
   }
  }
  void WriteYSingle(int index,float value){
   int bits=BitConverter.ToInt32(BitConverter.GetBytes(value),0);
   y[index]=unchecked((short)bits);y[index+1]=unchecked((short)(bits>>16));
  }
  internal NativeFreshDisplayArray Restart(){
   used=0;previewLength=0;lastPointers=new[]{1};x[0]=1;
   return Snapshot();
  }
  internal NativeFreshDisplayArray AppendOrdinaryItem(Canvas canvas,int code){
   if(lastPointers==null||canvas.Score.Items.Count!=lastPointers.Length)throw new NotSupportedException("Display append pointer history is not available.");
   var item=canvas.Score.Items[canvas.Score.Items.Count-1];
   if(item.Code!=code)throw new NotSupportedException("Display append differs from its measured item type.");
   int[] pointers=new int[lastPointers.Length+1];Array.Copy(lastPointers,pointers,lastPointers.Length);lastPointers=pointers;
   PreviewItem(canvas,canvas.Score.Items.Count,code);
   used+=previewLength;
   lastPointers[lastPointers.Length-1]=used+1;x[0]=checked((short)(used+1));return Snapshot();
  }
  internal NativeFreshDisplayArray PromotePreview(bool allowEmpty=false){
   if(lastPointers==null||previewLength<0||previewLength==0&&!allowEmpty||used+previewLength>31000)throw new NotSupportedException("Display copy preview is not available.");
   var pointers=new int[lastPointers.Length+1];Array.Copy(lastPointers,pointers,lastPointers.Length);
   used+=previewLength;pointers[pointers.Length-1]=used+1;lastPointers=pointers;
   x[0]=checked((short)(used+1));return Snapshot();
  }
  internal NativeFreshDisplayArray DeleteItem(int ordinal){
   if(lastPointers==null||ordinal<1||ordinal>=lastPointers.Length)throw new NotSupportedException("Display delete pointer history is not available.");
   int start=lastPointers[ordinal-1]-1,end=lastPointers[ordinal]-1,length=end-start;
   Array.Copy(x,end+1,x,start+1,used-end);Array.Copy(y,end,y,start,used-end);
   var pointers=new int[lastPointers.Length-1];
   for(int n=0;n<pointers.Length;n++)pointers[n]=n<ordinal?lastPointers[n]:lastPointers[n+1]-length;
   lastPointers=pointers;used-=length;x[0]=checked((short)(used+1));return Snapshot();
  }
  internal NativeFreshDisplayArray CommitItem(Canvas canvas,int ordinal,int code){
   if(lastPointers==null||ordinal<1||ordinal>=lastPointers.Length||canvas.Score.Items.Count!=lastPointers.Length-1)throw new NotSupportedException("Display edit pointer history is not available.");
   int length=PreviewItem(canvas,ordinal,code);int start=lastPointers[ordinal-1]-1,oldEnd=lastPointers[ordinal]-1,delta=length-(oldEnd-start),newEnd=start+length;
   if(used+delta+length+1>31000)throw new NotSupportedException("Display edit compaction capacity exceeded.");var newX=new short[length];var newY=new short[length];Array.Copy(x,used+1,newX,0,length);Array.Copy(y,used,newY,0,length);
   int suffix=used-oldEnd+length+1;Array.Copy(x,oldEnd+1,x,newEnd+1,suffix);Array.Copy(y,oldEnd,y,newEnd,suffix);Array.Copy(newX,0,x,start+1,length);Array.Copy(newY,0,y,start,length);
   used+=delta;for(int n=ordinal;n<lastPointers.Length;n++)lastPointers[n]+=delta;x[0]=checked((short)(used+1));return Snapshot();
  }
 }
}
