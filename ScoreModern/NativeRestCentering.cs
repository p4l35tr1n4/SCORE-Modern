using System;using System.Collections.Generic;
namespace ScoreModern {
 public sealed class NativeRestCenterChange {public Item Item;public float Position,Spacing;public bool ChangesSpacing;}
 public static class NativeRestCentering {
  static float F(double v){return (float)v;}
  static bool Finite(float v){return !Single.IsNaN(v)&&!Single.IsInfinity(v);}
  public static int StaffArgument(string text){float value;long word;if(!Single.TryParse(text,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out value)||!ScoreModern.RecoveredFragments.FiniteBinary32Integer64.TryConvert(BitConverter.ToUInt32(BitConverter.GetBytes(value),0),out word)||word<Int32.MinValue||word>Int32.MaxValue)throw new NotSupportedException("CW requires a finite signed32-bit staff argument.");return unchecked((short)word);}
  public static List<NativeRestCenterChange> Plan(Score score,int staff){return PlanCore(score,staff,null);}
  public static List<NativeRestCenterChange> PlanItem(Score score,Item item){return PlanCore(score,33,item);}
  internal static List<NativeRestCenterChange> PlanWholeRests(Score score,int staff=33){return PlanCore(score,staff,null,true);}
  static List<NativeRestCenterChange> PlanCore(Score score,int staff,Item only,bool onlyCenterable=false){
   if(score==null)throw new InvalidOperationException("Load a score before using CW.");
   if(staff<=0)return new List<NativeRestCenterChange>();
   if(staff>32767)throw new NotSupportedException("CW staff argument outside the measured command domain.");
   var page=new NativePageDocument(score);var changes=new List<NativeRestCenterChange>();
   foreach(var item in score.Items){if(only!=null&&!Object.ReferenceEquals(only,item))continue;if(item.Code!=2)continue;if(!Finite(item.P(2))||Math.Abs(item.P(2))>32767)throw new NotSupportedException("CW staff outside the finite signed-word domain.");if(staff<=32&&item.Staff!=staff)continue;
    if(!Finite(item.P(5))||Math.Abs(item.P(5))>32767||!Finite(item.P(4))||Math.Abs(item.P(4))>32767)throw new NotSupportedException("CW type/pitch outside the finite signed-word domain.");
    int kind=(int)item.P(5);
    bool centerable=kind<=-2&&kind>-20&&(kind<-10||kind>-8);
    if(onlyCenterable&&!centerable)continue;
    for(int n=6;n<=20;n++)if(!Finite(item.P(n)))throw new NotSupportedException("CW non-finite parameters require fault-path handling.");
    NativeNotationContext context;if(!page.Contexts.TryGetValue(item,out context)||context.Scale<=0||!Finite(context.Scale)||!Finite(item.P(3)))throw new NotSupportedException("CW needs a verified finite staff context.");
    var change=new NativeRestCenterChange{Item=item,Position=-1};if(!centerable){changes.Add(change);continue;}
    bool explicitSize=BitConverter.ToInt32(BitConverter.GetBytes(item.P(15)),0)!=0;
    float scale=explicitSize?F((double)context.Scale*item.P(15)):context.Scale;if(!explicitSize&&Math.Abs((int)item.P(4))>=100)scale=F((double)scale*.699999988079071);
    if(!Finite(scale)||scale==0)throw new NotSupportedException("CW zero/non-finite effective size requires fault-path handling.");
    float position=F((double)item.P(3)+F((double)item.P(10)*scale/25));
    float left=-9999,right=9999;Item leftBar=null,rightBar=null;
    foreach(var bar in score.Items){if(bar.P(1)!=14||bar.P(2)>item.P(2))continue;if(!Finite(bar.P(2))||!Finite(bar.P(3))||!Finite(bar.P(4)))throw new NotSupportedException("CW bar controls outside the finite domain.");
     float span=BitConverter.ToInt32(BitConverter.GetBytes(bar.P(4)),0)==0?1:Math.Abs(bar.P(4));if((double)bar.P(2)+span<=item.P(2))continue;
     if(bar.P(3)>position&&bar.P(3)<right){right=bar.P(3);rightBar=bar;}if(bar.P(3)<position&&bar.P(3)>left){left=bar.P(3);leftBar=bar;}
    }
    if(rightBar==null){changes.Add(change);continue;}
    float unit=F((double)scale/25);bool trimLeft=(double)position-left>((double)scale/25)*7;if(trimLeft)left=F((double)position-unit*4.5);
    if(kind==-5){
     int rightStyle=BarStyle(rightBar),leftStyle=trimLeft||leftBar==null?0:BarStyle(leftBar);
     if(rightStyle!=0)right=F((double)right-RightClearance(rightStyle)*unit);
     if(leftStyle!=0)left=F((double)left+LeftClearance(leftStyle)*unit);
     float endpoint=F((double)right-position+left),gap=F((double)unit*3.5);if((double)right-endpoint>gap)endpoint=F((double)right-gap);change.Position=endpoint;}
    // The original FST rounds the saved interval but retains its extended value
    // for centering. European spacing subsequently reloads the rounded interval.
    else {double width=(double)right-left;change.Position=F(width*.5-(double)scale*(kind==-4?.13699999451637268:.0560000017285347)+left);if(kind<=-13&&kind!=-14){change.ChangesSpacing=true;change.Spacing=F((double)F(width)*1.0416699647903442/scale);}}
    changes.Add(change);
   }return changes;
  }
  static int BarStyle(Item bar){if(!Finite(bar.P(5))||bar.P(5)<-1||bar.P(5)>32767)throw new NotSupportedException("CW bar style outside the signed-word domain.");int style=(int)bar.P(5);return style>6?0:style;}
  static float RightClearance(int style){if(style==-1)return 6;return new float[]{1.2000000476837158f,2.200000047683716f,4.400000095367432f,1,4.400000095367432f,3}[style-1];}
  static float LeftClearance(int style){if(style==-1)return 4.400000095367432f;return new float[]{0,0,0,3.5999999046325684f,3.4000000953674316f,4}[style-1];}
  public static void Apply(IEnumerable<NativeRestCenterChange> changes){foreach(var change in changes){change.Item.Set(9,change.Position);if(change.ChangesSpacing)change.Item.Set(6,change.Spacing);}}
 }
 public sealed partial class Editor {
  void CenterScor4GroupWholeRests(){
   var plan=NativeRestCentering.PlanWholeRests(score);
   if(plan.Count==0)return;
   bool trackDisplay=CanTrackDisplayRedraw();
   using(var storage=BeginParameterMutation()){
    Checkpoint();
    NativeRestCentering.Apply(plan);
    dirty=true;
    RefreshAll(canvas.Selected);
    if(trackDisplay)RebuildTrackedDisplay(centeredRests:plan.ConvertAll(change=>score.Items.IndexOf(change.Item)));
    storage.Complete();
   }
  }

  void ExecuteRestCentering(string[] fields){
   if(mode==ScreenMode.Edit){
    if(fields.Length>2)throw new NotSupportedException("CW accepts at most one staff argument.");
    if(fields.Length==2)NativeRestCentering.StaffArgument(fields[1]);
    if(canvas.Selected==null||canvas.Selected.Code!=2)return;
    var selectedPlan=NativeRestCentering.PlanItem(score,canvas.Selected);
    if(selectedPlan.Count==0)return;
    Checkpoint();
    NativeRestCentering.Apply(selectedPlan);
    dirty=true;
    RefreshAll(canvas.Selected);
    return;
   }
   if(fields.Length==1)return;
   if(fields.Length!=2)throw new NotSupportedException("CW usage: CW staff.");
   int staff=NativeRestCentering.StaffArgument(fields[1]);
   var plan=NativeRestCentering.Plan(score,staff);
   if(plan.Count!=0){
    bool trackDisplay=staff>=1&&staff<=32&&CanTrackDisplayRedraw();
    Checkpoint();
    NativeRestCentering.Apply(plan);
    dirty=true;
    RefreshAll(canvas.Selected);
    if(trackDisplay)RebuildTrackedDisplay(centeredRests:plan.ConvertAll(change=>score.Items.IndexOf(change.Item)));
   }
   Message("Centred "+plan.Count+" rests.");
  }
 }

}
