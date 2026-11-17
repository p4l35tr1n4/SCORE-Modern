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
  readonly bool page4;
  internal NativePageTranspose():this(2){}
  internal NativePageTranspose(short interval){
   if(interval!=1&&interval!=-1&&interval!=-7&&
    !Enumerable.Range(1,16).Any(choice=>InputTranspositionInterval.Menu((short)choice)==interval))
    throw new NotSupportedException("Choose a PAGE transposition interval from the menu.");
   semitones=interval;
  }
  internal NativePageTranspose(short interval,string root):this(interval){normalizationRoot=root;}
  internal NativePageTranspose(short interval,string root,bool key):this(interval,root){
   addKey=key;
  }
  internal NativePageTranspose(short interval,string root,bool key,short? targetClef,bool page4=false):this(interval,root,key){
   if(targetClef.HasValue&&(targetClef.Value<0||targetClef.Value>3))throw new NotSupportedException("PAGE clef conversion requires a standard clef.");
   clef=targetClef;
   this.page4=page4;
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
   int staffCount=input.Where(item=>item.Code==8).Select(item=>item.Staff).DefaultIfEmpty(1).Max();
   if(staffCount<1||staffCount>32)throw new NotSupportedException("PAGE transposition requires 1–32 staves.");
   if(input.Any(i=>i.Staff<1||i.Staff>staffCount||((i.Code==5||i.Code==6)&&
    (i.Count<6||
     Math.Abs(i.P(4))>=100&&!(page4&&i.Code==6&&i.Count==7&&i.P(7)==12&&i.P(4)>=100&&i.P(4)<200)||
     Math.Abs(i.P(5))>=100||i.P(3)<0||i.P(3)>201||i.P(6)<0))))
    throw new NotSupportedException("PAGE transposition requires ordinary beam and slur controls.");
   var source=input.Select(i=>Enumerable.Range(1,i.Count).Select(i.P).ToArray()).ToList();
   short shift=0;
   for(int staff=1;staff<=staffCount;staff++){
    var session=InputTranspositionOperation.Begin(source,new TranspositionRequest{Staff=staff,Left=0,Right=200,Semitones=semitones},workspace);
    if(clef.HasValue)InputTranspositionOperation.ChangeClef(session,clef.Value);
    var transposed=InputTranspositionOperation.FinishCore(session,addKey,false);
    if(transposed.Rows.Count!=source.Count+(transposed.InsertedKey?1:0)||transposed.WarningRequired)
     throw new NotSupportedException("PAGE transposition reaches an unverified key or fingering path.");
    source=transposed.Rows;
    shift=session.Result.Interval.Diatonic;
   }
   var result=new List<Item>();
   for(int n=0;n<input.Count;n++){
    var row=source[n];
    if(row.Length!=input[n].Count)throw new NotSupportedException("PAGE transposition changes the record extent.");
    var item=input[n].Clone();
    for(int p=1;p<=row.Length;p++)if(BitConverter.ToInt32(BitConverter.GetBytes(row[p-1]),0)!=BitConverter.ToInt32(BitConverter.GetBytes(item.P(p)),0))item.Set(p,row[p-1]);
    if(item.Code==5||item.Code==6){item.Set(4,(float)((double)item.P(4)+shift));item.Set(5,(float)((double)item.P(5)+shift));}
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
