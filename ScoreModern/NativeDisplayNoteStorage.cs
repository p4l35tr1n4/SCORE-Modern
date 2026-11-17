using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScoreModern {
 public sealed partial class Editor {
  bool physicalDisplayNotePending;
  int physicalDisplayNoteIndex=-1;
  byte[] physicalDisplayWorkingMusic;
  Score physicalDisplayWorkingScore;

  static bool TrackedStaff(Item item){
   return item!=null&&item.Code==8&&item.Count==6&&item.Staff==1&&item.P(3)==0&&
    item.P(5)==1&&item.P(6)==200&&!Single.IsNaN(item.P(4))&&Math.Abs(item.P(4))<100;
  }
  bool TrackedStaffHeader(Item item){
   return item!=null&&item.Code==8&&item.Count==6&&item.Staff>=1&&item.Staff<=32&&
    item.P(3)==0&&item.P(5)>0&&item.P(5)<=2&&Math.Abs(item.P(4))<100&&item.P(6)==200&&
    score.Items.Count(row=>row.Code==8&&row.Staff==item.Staff)<=1;
  }
  static bool TrackedRest(Item item){
   return item!=null&&item.Code==2&&item.Count==7&&item.Staff==1&&item.P(5)==0&&item.P(6)==0&&
    (item.P(7)==.25f||item.P(7)==.5f||item.P(7)==1||item.P(7)==2||item.P(7)==4)&&
    !Single.IsNaN(item.P(4))&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedLine(Item item){
   return item!=null&&item.Code==4&&item.Count>=6&&item.Count<=7&&item.Staff==1&&item.P(5)==16&&item.P(7)==0&&
    !Single.IsNaN(item.P(4))&&Math.Abs(item.P(4))<100&&!Single.IsNaN(item.P(6))&&item.P(6)>0&&item.P(6)<=200;
  }
  static bool TrackedTrill(Item item){
   return item!=null&&item.Code==7&&item.Count==6&&item.Staff==1&&item.P(5)==0&&
    !Single.IsNaN(item.P(4))&&Math.Abs(item.P(4))<100&&!Single.IsNaN(item.P(6))&&item.P(6)>item.P(3)&&item.P(6)<=200;
  }
  static bool TrackedClef(Item item){
   return item!=null&&item.Code==3&&item.Staff>=1&&item.Staff<=32&&item.Count>=3&&item.Count<=5&&item.P(5)==0&&
    !Single.IsNaN(item.P(4))&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedMeter(Item item){
   if(item==null||item.Code!=18||item.Staff<1||item.Staff>32||!(Math.Abs(item.P(4))<100))return false;
   float top=item.P(5),bottom=item.P(6);
   if(top==98||top==99)return bottom==0&&item.Count==5||
    item.Count==6&&(bottom==0||bottom==1);
   return item.Count==6&&((top==2||top==3||top==4)&&bottom==4||(top==6||top==9)&&bottom==8||top==2&&bottom==2||top==12&&bottom==16);
  }
  static bool TrackedBox(Item item){
   return item!=null&&item.Code==15&&item.Staff==1&&item.Count==6&&
    item.P(5)==1&&item.P(6)==1&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedInvisibleItem(Item item){
   return item!=null&&item.Code==13&&item.Staff==1&&
    (item.Count==4||item.Count==5)&&item.P(5)==0&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedSimpleShape(Item item){
   if(item==null||item.Code!=12||item.Staff!=1||!(Math.Abs(item.P(4))<100))return false;
   return item.Count==6&&(item.P(5)==0||item.P(5)==1)&&item.P(6)==1||
    item.Count==7&&item.P(5)==1&&((item.P(6)==2&&item.P(7)==1)||(item.P(6)==1&&item.P(7)==2));
  }
  static bool TrackedNumber(Item item){
   if(item==null||item.Code!=10||item.Staff!=1||item.Count<6||item.Count>8||
    item.P(6)!=1||item.P(7)!=1||!(Math.Abs(item.P(4))<100))return false;
   float value=item.P(5);
   if(item.Count==8)return value==7&&item.P(8)==1||value==42&&item.P(8)==2||value==999&&item.P(8)==3;
   return value==0||value==7||value==42||value==999||value==1001||value==1012;
  }
  static bool TrackedBracket(Item item){
   return item!=null&&item.Code==5&&item.Staff==1&&(item.Count==8||item.Count==9)&&
    item.P(5)==12&&item.P(6)==48&&item.P(7)==1&&
    (item.P(8)==3&&item.P(9)==4||item.P(8)==4&&item.P(9)==0)&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedSlur(Item item){
   return item!=null&&item.Code==5&&item.Staff==1&&item.Count==7&&
    item.P(5)==6&&item.P(6)==48&&item.P(7)==2&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedBeam(Item item){
   return item!=null&&item.Code==6&&item.Staff==1&&item.Count==7&&
    Math.Abs(item.P(5))<100&&item.P(6)>item.P(3)&&item.P(6)<=200&&
    item.P(7)==12&&Math.Abs(item.P(4))<100;
  }
  static bool TrackedDynamic(Item item){
   if(item==null||item.Code!=9||item.Staff<1||item.Staff>32||(item.Count!=5&&item.Count!=6)||
    !(Math.Abs(item.P(4))<100))return false;
   float symbol=item.P(5);
   if(symbol==52)return item.P(6)==0||item.P(6)==1;
   return item.Count==6&&item.P(6)==1&&(symbol==50||symbol==51||symbol==53||symbol==54);
  }
  static bool TrackedKey(Item item){
   if(item==null||item.Code!=17||item.Staff!=1||(item.Count!=5&&item.Count!=6)||
    item.P(6)!=0||!(Math.Abs(item.P(4))<100))return false;
   float signature=item.P(5);
   return signature!=0&&Math.Abs(signature)<=7&&signature==Math.Truncate(signature)||
    signature==101||signature==-104||Math.Abs(signature)==107;
  }
  static bool TrackedBarline(Item item){
   if(item==null||item.Code!=14||item.Staff!=1)return false;
   float style=item.P(5);
   if(item.Count==13)return (item.P(4)==1||item.P(4)==2)&&(style==1||style==2)&&
    Enumerable.Range(6,7).All(p=>item.P(p)==0)&&(item.P(13)==1||item.P(13)==-.5f);
   if(item.Count!=4&&item.Count!=5)return false;
   // SCORE validates the item before it reaches the DAT dispatcher.
   return NativeDatBarlines.DefaultCenteredDash(item)||item.P(4)==1&&style>=-1&&style<999;
  }
  static bool TrackedPlainText(Item item){
   return item!=null&&item.Code==16&&item.Staff==1&&item.Count>=14&&item.P(5)==1&&item.P(6)==1&&
    item.P(7)==0&&item.P(8)==0&&item.P(9)==0&&item.P(10)==0&&item.P(11)==0&&
    Math.Abs(item.P(4))<100&&IsPlainPrimitiveText(item.Text);
  }
  static bool IsPlainPrimitiveText(string text){
   text=text.TrimEnd(' ');
   return text.Length>3&&text.StartsWith("_00",StringComparison.Ordinal)&&text.Skip(3).All(c=>c>='A'&&c<='Z');
  }
  static bool TrackedTextPosition(Item item){
   if(item==null||item.Code!=16||item.Staff<1||item.Staff>32||item.Count<14||
    item.P(5)!=1||!(item.P(6)>0&&item.P(6)<=2)||!(Math.Abs(item.P(4))<100))return false;
   for(int p=7;p<=11;p++)if(item.P(p)!=0)return false;
   string value=item.Text.TrimEnd(' ');
   return value.Length>3&&value[0]=='_'&&value[1]>='0'&&value[1]<='9'&&
    value[2]>='0'&&value[2]<='9'&&value.Skip(3).All(c=>c>=' '&&c<='~'&&c!='_'&&c!='\\');
  }
  static bool TrackedDisplayPosition(Item item,int parameter){
   if(TrackedBarline(item))return parameter==3||item.Count==13&&parameter==4;
   return parameter==4||NativeRetainedDisplayArray.SupportsNote(item)&&parameter==9||
    (TrackedLine(item)||TrackedTrill(item))&&parameter==6;
  }
  static bool TrackedNonNoteLifecycle(Item item){
   return TrackedRest(item)||TrackedLine(item)||TrackedTrill(item)||TrackedClef(item)||
    TrackedMeter(item)||TrackedKey(item)||TrackedBeam(item)||TrackedSlur(item)||TrackedBracket(item)||
    TrackedNumber(item)||TrackedSimpleShape(item)||TrackedBox(item)||TrackedInvisibleItem(item)||
    TrackedDynamic(item)||TrackedBarline(item)||TrackedPlainText(item);
  }
  static bool TrackedDisplayItem(Item item){
   return NativeRetainedDisplayArray.SupportsNotePreview(item)||TrackedStaff(item)||TrackedNonNoteLifecycle(item)||TrackedTextPosition(item);
  }
  bool DisplayItemCacheMatches(Item item){
   return item.Code==1?physicalDisplayStorage.CanTrackNote(canvas,item):!canvas.DisappearingStaffs.Any();
  }
  bool DisplayResourcesMatch(){
   return !physicalDisplayActualFonts&&canvas.Library!=null&&canvas.TextLayout!=null&&physicalDisplayLibrary==canvas.Library&&
    physicalDisplayLibraryRevision==NativeLibraryLocation.RevisionFor(canvas.Library.Root)&&
    physicalDisplayActualFonts==canvas.TextLayout.ShowActualDatFonts&&
    physicalDisplayHidden==String.Join(",",canvas.DisappearingStaffs.OrderBy(n=>n));
  }
  static bool DisplayScanFailure(Exception error){
   return error is NotSupportedException||error is IOException||error is InvalidDataException||
    error is ArgumentException||error is OverflowException;
  }
  void ObserveDisplaySelection(Item item){
   if(!physicalDisplayKnown)return;
   // An unmodelled selection can change the original head cache even when
   // active records are unchanged. Do not retain a later-load claim.
   try{
    bool measuredStaffHeader=TrackedStaffHeader(item)&&!canvas.DisappearingStaffs.Any();
    bool measuredBassClef=item!=null&&item.Code==3&&item.Count==5&&item.Staff>=1&&item.Staff<=32&&
     item.P(4)==0&&item.P(5)==1&&!canvas.DisappearingStaffs.Any();
    if(physicalDisplayNotePending||!DisplayResourcesMatch()||
     !measuredStaffHeader&&!measuredBassClef&&(!TrackedDisplayItem(item)||!DisplayItemCacheMatches(item)))InvalidateDisplayStorage();
   }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
  }
  DisplayNoteMutation BeginDisplayNoteParameters(List<OriginalParameterOperation> operations,bool restMove=false){
   bool valid=false;int ordinal=canvas.Selected==null?-1:score.Items.IndexOf(canvas.Selected);
   if(physicalDisplayKnown&&ordinal>=0&&TrackedDisplayItem(canvas.Selected)&&DisplayResourcesMatch()&&
    (restMove&&TrackedRest(canvas.Selected)||operations!=null&&operations.Count==1&&!operations[0].Add&&!operations[0].Copy&&
    TrackedDisplayPosition(canvas.Selected,operations[0].Parameter))){
    byte[] expected=physicalDisplayNotePending?physicalDisplayWorkingMusic:physicalDisplayMusic;
    valid=expected!=null&&expected.SequenceEqual(score.Encode())&&
     (!physicalDisplayNotePending||ordinal==physicalDisplayNoteIndex);
    if(valid)try{valid=DisplayItemCacheMatches(canvas.Selected);}
    catch(Exception error){if(!DisplayScanFailure(error))throw;valid=false;}
   }
   return new DisplayNoteMutation(this,valid,ordinal,restMove);
  }
  DisplayNoteMutation BeginDisplayTextChange(Item original,Item replacement){
   int ordinal=original==null?-1:score.Items.IndexOf(original);
   bool accepted=physicalDisplayKnown&&mode==ScreenMode.Edit&&ordinal>=0&&
    TrackedPlainText(original)&&TrackedPlainText(replacement)&&DisplayResourcesMatch()&&
    !canvas.DisappearingStaffs.Any()&&physicalDisplayMusic!=null&&
    physicalDisplayMusic.SequenceEqual(score.Encode())&&!physicalDisplayNotePending;
   return new DisplayNoteMutation(this,accepted,ordinal,false,true);
  }
  sealed class DisplayNoteMutation:IDisposable {
   Editor owner;readonly bool valid,restMove,textChange;readonly int ordinal;readonly byte[][] before;
   internal DisplayNoteMutation(Editor editor,bool accepted,int index,bool movedRest,bool changedText=false){
    owner=editor;valid=accepted;ordinal=index;restMove=movedRest;textChange=changedText;
    if(valid)before=editor.score.Items.Select(item=>(byte[])item.Bytes.Clone()).ToArray();
   }
   internal void Complete(){
    var editor=owner;owner=null;if(!valid)return;
    try{
     bool same=editor.DisplayResourcesMatch()&&editor.score.Items.Count==before.Length&&
      editor.canvas.Selected==editor.score.Items[ordinal]&&TrackedDisplayItem(editor.canvas.Selected);
     for(int n=0;same&&n<before.Length;n++){
      if(n!=ordinal)same=before[n].SequenceEqual(editor.score.Items[n].Bytes);
      else{
       var old=new Item(before[n]);var current=editor.score.Items[n];
       for(int p=1;same&&p<=Math.Max(old.Count,current.Count);p++)
        if(!(textChange&&p>=12)&&!TrackedDisplayPosition(current,p)&&!(restMove&&p==3))same=BitConverter.ToInt32(BitConverter.GetBytes(old.P(p)),0)==
         BitConverter.ToInt32(BitConverter.GetBytes(current.P(p)),0);
      }
     }
     if(!same){editor.InvalidateDisplayStorage();return;}
     using(var probe=new Canvas{Score=editor.score.Clone(),Library=editor.canvas.Library,TextLayout=editor.canvas.TextLayout}){
      editor.physicalDisplayStorage.PreviewItem(probe,ordinal+1,editor.canvas.Selected.Code);
     }
     editor.physicalDisplayKnown=true;editor.physicalDisplayNotePending=true;
     editor.physicalDisplayNoteIndex=ordinal;editor.physicalDisplayWorkingMusic=editor.score.Encode();
     editor.physicalDisplayWorkingScore=editor.score.Clone();
     editor.physicalDisplayLast=editor.physicalDisplayStorage.ReadActive();
    }catch(Exception error){if(!DisplayScanFailure(error))throw;editor.InvalidateDisplayStorage();}
   }
   public void Dispose(){if(owner==null)return;var editor=owner;owner=null;if(valid)editor.InvalidateDisplayStorage();}
  }
  bool CanTrackNonNoteLifecycle(Item item,bool preview){
   // Removing a single ordinary staff compacts its vector block. The
   // child's existing display vectors stay in place until redraw.
   bool ordinaryStaff=!preview&&TrackedStaffHeader(item);
   if(!physicalDisplayKnown||canvas.DisappearingStaffs.Any()||!DisplayResourcesMatch()||
    !ordinaryStaff&&!TrackedNonNoteLifecycle(item)||physicalDisplayNotePending!=preview)return false;
   var expected=preview?physicalDisplayWorkingMusic:physicalDisplayMusic;
   return expected!=null&&expected.SequenceEqual(score.Encode());
  }
  // Staff 1 also has measured down-stem and flagged-note copy histories.
  static bool TrackedNoteLifecycle(Item item){
   return NativeRetainedDisplayArray.SupportsNote(item)&&item.Staff>=1&&
    (item.Staff==1||item.Count==7&&item.Staff<=2||item.Count==9&&item.P(9)==1&&item.Staff<=3);
  }
  bool CanPromoteDisplayCopy(Item item,int index){
   if(physicalDisplayNoteIndex!=index)return false;
   if(CanTrackNonNoteLifecycle(item,true))return true;
   // Original copy promotes its pitch-edit preview without rewriting vectors.
   if(!physicalDisplayKnown||!physicalDisplayNotePending||canvas.DisappearingStaffs.Any()||
    !DisplayResourcesMatch()||!TrackedNoteLifecycle(item)||
    editOriginal==null||physicalDisplayWorkingMusic==null||
    !physicalDisplayWorkingMusic.SequenceEqual(score.Encode()))return false;
   for(int p=1;p<=Math.Max(item.Count,editOriginal.Count);p++)if(p!=4&&item.P(p)!=editOriginal.P(p))return false;
   return true;
  }
  bool CanTrackDisplayRemoval(Item item,int index,bool restoring){
   if(CanTrackNonNoteLifecycle(item,false))return true;
   return physicalDisplayKnown&&!physicalDisplayNotePending&&!canvas.DisappearingStaffs.Any()&&
    DisplayResourcesMatch()&&TrackedNoteLifecycle(item)&&
    (restoring||index>=0&&index<score.Items.Count)&&
    physicalDisplayMusic!=null&&physicalDisplayMusic.SequenceEqual(score.Encode());
  }
  void TrackDisplayLifecycle(bool accepted,int deleteOrdinal=0,bool promotePreview=false,int appendCode=0){
   if(!accepted){InvalidateDisplayStorage();return;}
   try{
    if(deleteOrdinal>0)physicalDisplayLast=physicalDisplayStorage.DeleteItem(deleteOrdinal);
    else if(promotePreview)physicalDisplayLast=physicalDisplayStorage.PromotePreview(TrackedInvisibleItem(canvas.Selected));
    else using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout})physicalDisplayLast=physicalDisplayStorage.AppendOrdinaryItem(probe,appendCode);
    physicalDisplayKnown=true;physicalDisplayMusic=score.Encode();physicalDisplayNotePending=false;physicalDisplayWorkingMusic=null;physicalDisplayWorkingScore=null;
   }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
  }
  void ObserveDisplayExit(bool abort){
   if(!physicalDisplayKnown)return;
   if(!physicalDisplayNotePending){if(canvas.EditModified||abort)InvalidateDisplayStorage();return;}
   bool same=!abort&&canvas.EditModified&&canvas.Selected!=null&&
    score.Items.IndexOf(canvas.Selected)==physicalDisplayNoteIndex&&DisplayResourcesMatch()&&
    physicalDisplayWorkingScore!=null&&score.Items.Count==physicalDisplayWorkingScore.Items.Count&&
    score.Trailer.SequenceEqual(physicalDisplayWorkingScore.Trailer);
   for(int n=0;same&&n<score.Items.Count;n++){
    var old=physicalDisplayWorkingScore.Items[n];var item=score.Items[n];
    if(n!=physicalDisplayNoteIndex)same=item.Bytes.SequenceEqual(old.Bytes);
    else for(int p=1;same&&p<=Math.Max(old.Count,item.Count);p++)
     same=BitConverter.ToInt32(BitConverter.GetBytes(old.P(p)),0)==BitConverter.ToInt32(BitConverter.GetBytes(item.P(p)),0);
   }
   if(!same){InvalidateDisplayStorage();return;}
   try{
    if(!TrackedDisplayItem(canvas.Selected)){InvalidateDisplayStorage();return;}
    using(var probe=new Canvas{Score=score.Clone(),Library=canvas.Library,TextLayout=canvas.TextLayout})
     physicalDisplayLast=physicalDisplayStorage.CommitItem(probe,physicalDisplayNoteIndex+1,canvas.Selected.Code);
    physicalDisplayMusic=score.Encode();physicalDisplayNotePending=false;physicalDisplayWorkingMusic=null;physicalDisplayWorkingScore=null;
   }catch(Exception error){if(!DisplayScanFailure(error))throw;InvalidateDisplayStorage();}
  }
  bool BeginTrackedDisplayAbort(){
   return physicalDisplayKnown&&physicalDisplayNotePending&&canvas.Selected!=null&&
    score.Items.IndexOf(canvas.Selected)==physicalDisplayNoteIndex&&DisplayResourcesMatch()&&
    physicalDisplayWorkingMusic!=null&&physicalDisplayWorkingMusic.SequenceEqual(score.Encode());
  }
  void CompleteTrackedDisplayAbort(bool accepted){
   if(!accepted)return;
   if(!DisplayResourcesMatch()||physicalDisplayMusic==null||!physicalDisplayMusic.SequenceEqual(score.Encode())){
    InvalidateDisplayStorage();return;
   }
   // Measured F8 restores records without erasing the last preview scratch.
   physicalDisplayKnown=true;physicalDisplayNotePending=false;physicalDisplayWorkingMusic=null;physicalDisplayWorkingScore=null;
   physicalDisplayLast=physicalDisplayStorage.ReadActive();
  }
 }
}
