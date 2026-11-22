using System;
using System.Collections.Generic;
using System.Linq;
using ScoreModern.RecoveredFragments;

namespace ScoreModern {
 internal sealed class NativePageTranspose {
  readonly short semitones;
  readonly string normalizationRoot;
  readonly bool addKey;
  readonly short? clef;
  readonly bool page4,preservePitch;
  internal Action<string> ReportWarning;
  internal NativePageTranspose():this(2){}
  internal NativePageTranspose(short interval){
   if(interval < -12 || interval > 23)
    throw new NotSupportedException("PAGE transposition supports intervals from down one octave to up a major fourteenth.");
   semitones=interval;
  }
  internal NativePageTranspose(short interval,string root):this(interval){normalizationRoot=root;}
  internal NativePageTranspose(short interval,string root,bool key):this(interval,root){
   addKey=key;
  }
  internal NativePageTranspose(short interval,string root,bool key,short? targetClef,bool page4=false,bool preservePitch=false):this(interval,root,key){
   if(targetClef.HasValue&&(targetClef.Value<0||targetClef.Value>3))throw new NotSupportedException("PAGE clef conversion requires a standard clef.");
   clef=targetClef;
   this.page4=page4;this.preservePitch=preservePitch;
  }
  readonly TranspositionWorkspace workspace=new TranspositionWorkspace(
   new TranspositionCollectorState(0),new TranspositionClefState(0x2020,0x2020),
   new short[200],new short[200],0,new TranspositionRelationshipWindow(),true);

  // 0CC8:03E3 copies the unsorted parameter bank to the scratch
  // segment. PAGE 1176:0F43 later tests the preceding word of the
  // accidental flags. The flag-clear routine starts two bytes later.
  internal void RetainSortScratch(IList<Item> records){
   const int kindZeroOffset=0x09d2;
   int offset=kindZeroOffset;
   foreach(var record in records){
    var bytes=record.Bytes;
    if(offset+2<=bytes.Length){
     workspace.KindZeroGuard=BitConverter.ToInt16(bytes,offset);
     return;
    }
    offset-=bytes.Length;
   }
  }

  // PAGE 0DFF:0235 reuses one workspace across the staff passes.
  // PAGE calls the tonal engine before LI/J and
  // adjusts attachments itself (05C7..06B7), without Main TR's slur rounding.
  internal List<Item> Apply(IList<Item> input){
   if(preservePitch&&input.Any(i=>i.Code==1&&Math.Abs((int)i.P(5))%10>5))throw new NotSupportedException("Quarter-tone transposition is outside the recovered conventional accidental table. The source has not been changed.");
   int staffCount=input.Where(item=>item.Code==8).Select(item=>item.Staff).DefaultIfEmpty(1).Max();
   if(staffCount<1||staffCount>32)throw new NotSupportedException("PAGE transposition requires 1–32 staves.");
   if(input.Any(i=>i.Staff<1||i.Staff>staffCount||((i.Code==5||i.Code==6)&&
    (i.Count<6||
     Math.Abs(i.P(4))>=100&&!(i.Code==6&&i.Count>=7&&i.P(7)>=10&&i.P(7)<=19&&i.P(7)==Math.Truncate(i.P(7))&&Math.Abs(i.P(4))<200)||
     Math.Abs(i.P(5))>=100||i.P(3)<0||i.P(3)>201||i.P(6)<0))))
    throw new NotSupportedException("PAGE transposition requires ordinary beam and slur controls.");
   var source=input.Select(i=>Enumerable.Range(1,i.Count).Select(i.P).ToArray()).ToList();
   short shift=0;var staffShifts=new Dictionary<int,int>();
   for(int staff=1;staff<=staffCount;staff++){
    var before=preservePitch?source.Select(row=>(float[])row.Clone()).ToList():null;
    var session=InputTranspositionOperation.Begin(source,new TranspositionRequest{Staff=staff,Left=0,Right=200,Semitones=semitones},workspace);
    if(clef.HasValue)InputTranspositionOperation.ChangeClef(session,clef.Value);
    bool insertKey=addKey&&!(preservePitch&&source.Any(row=>row.Length>=8&&row[0]==17&&row[1]==staff&&row[7]!=0));
    var transposed=InputTranspositionOperation.FinishCore(session,insertKey,false);
    if(transposed.Rows.Count!=source.Count+(transposed.InsertedKey?1:0))
     throw new NotSupportedException("PAGE transposition reaches an unverified key or fingering path.");
    if(transposed.WarningRequired&&ReportWarning!=null)ReportWarning(InputTranspositionEmbeddedControl.FingeringWarning);
    if(preservePitch)InputChromaticPitchCorrection.Apply(before,transposed.Rows,(short)staff,semitones);
    staffShifts[staff]=session.Result.Interval.Diatonic;
    source=transposed.Rows;
    shift=session.Result.Interval.Diatonic;
   }
   var result=new List<Item>();
   for(int n=0;n<input.Count;n++){
    var row=source[n];
    bool resized=row.Length!=input[n].Count;
    if(resized&&!((input[n].Code==3&&input[n].Count<5&&row.Length==5)||(input[n].Code==17&&input[n].Count<6&&row.Length==6)))throw new NotSupportedException("PAGE transposition changes an unsupported record extent.");
    var item=input[n].Clone();if(resized)item.Bytes=Item.Make(row).Bytes;
    for(int p=1;p<=row.Length;p++)if(BitConverter.ToInt32(BitConverter.GetBytes(row[p-1]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(item.P(p)),0))item.Set(p,row[p-1]);
    if(item.Code==5||item.Code==6){int localShift=preservePitch?staffShifts[item.Staff]:shift;for(int endpoint=0;endpoint<2;endpoint++){int parameter=endpoint==0?4:5;float x=item.P(endpoint==0?3:6);int correction=0;if(preservePitch){var notes=Enumerable.Range(0,input.Count).Where(k=>input[k].Code==1&&input[k].Staff==item.Staff&&Math.Abs(input[k].P(3)-x)<.0001).OrderBy(k=>Math.Abs(input[k].P(4)%100-input[n].P(parameter)%100)).ToArray();if(notes.Length>0){int note=notes[0];float delta=source[note][3]%100-input[note].P(4)%100;float value=input[n].P(parameter)+delta;if(item.Code==6&&parameter==4&&Math.Abs(input[n].P(4))>=100){float pitch=input[n].P(4)%100+delta;value=pitch+(pitch<0?-100:100);}item.Set(parameter,value);continue;}}item.Set(parameter,(float)((double)item.P(parameter)+localShift+correction));}}
    result.Add(item);
   }
   for(int n=input.Count;n<source.Count;n++){
    var key=source[n];
    if(key.Length!=6||key[0]!=17||key[1]<1||key[1]>staffCount)throw new InvalidOperationException("Unexpected PAGE key insertion record.");
    result.Add(Item.Make(key));
   }
   return normalizationRoot==null?result:Editor.NormalizePageStems(result,normalizationRoot,page4);
  }
 }
}
