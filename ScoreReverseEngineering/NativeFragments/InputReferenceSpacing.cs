using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ScoreModern.RecoveredFragments {
 // Reference spacing from overlay 7 index 57. The DOS reference table
 // stores the horizontal anchors and subtracts .001 from its upper search
 // boundaries. Grace and barline entries can supply a right drawing
 // anchor without consuming beats.
 public sealed class InputReferenceSpacing {
  readonly float clefX,end,total;
  readonly float[] anchors,queryGrid,upperAnchors;
  struct Marker {
   public readonly int Code,Slot;
   public readonly float X;
   public Marker(int code,int slot,float x){Code=code;Slot=slot;X=x;}
  }
  readonly List<Marker> markers=new List<Marker>();
  public bool HasClefAtZero {get{return OpeningClefX==0;}}
  internal float? OpeningClefX {get;private set;}
  internal float Start {get{return anchors[0];}}
  internal float QueryEnd {get{return end;}}

  public InputReferenceSpacing(IList<float[]> staff):this(staff,null){}
  public InputReferenceSpacing(IList<float[]> staff,Action<int,float> normalizedPosition){
   var notation=CollectReferenceRows(staff,normalizedPosition).ToArray();
   // Source SP collector skips opening bar drawings before its clef/timed table.
   // Interior brackets still supply right drawing anchors (measured offset probe).
   int start=0;while(start<notation.Length&&Bar(notation[start]))start++;
   bool hasClef=notation.Length>start&&OpeningClef(notation[start]);
   int clef=hasClef?start:start-1;
   if(notation.Length<=clef+1)throw new NotSupportedException("Reference spacing needs timed events.");
   // An absent source clef supplies zero; source appearance does not change timing.
   var events=SpacingEvents(notation,clef+1,false);
   var notes=events.Where(row=>!Grace(row)&&!Structural(row)).ToList();
   if(notes.Count==0)
    throw new NotSupportedException("Reference spacing needs at least one timed event.");
   clefX=hasClef?notation[clef][2]:0;
   if(hasClef)OpeningClefX=clefX;
   // Without an explicit closing bar, overlay 7 appends its literal 200 marker.
   bool hasClosingBar=Bar(events[events.Count-1]);
   end=hasClosingBar?events[events.Count-1][2]:200;
   anchors=new float[notes.Count+1];
   for(int i=0;i<notes.Count;i++){
    total=(float)((double)total+notes[i][6]);
    anchors[i]=notes[i][2];
   }
   anchors[notes.Count]=end;
   upperAnchors=anchors.Skip(1).ToArray();
   int spanIndex=-1;
   bool hasIntermediateAnchor=false;
   for(int i=0;i<events.Count;i++){
    var row=events[i];
    if(Structural(row))markers.Add(new Marker((int)row[0],i+2+clef,row[2]));
    if(!Grace(row)&&!Structural(row)){spanIndex++;hasIntermediateAnchor=false;}
    else if(row[0]!=3&&spanIndex>=0&&!hasIntermediateAnchor){upperAnchors[spanIndex]=row[2];hasIntermediateAnchor=true;}
   }
   if(!hasClosingBar){
    markers.Add(new Marker(14,events.Count+2+clef,end));
   }
   // Overlay 7 index 56 maps reference durations onto a separate query grid.
   // Index 57 interpolates from that grid to the stored horizontal positions.
   queryGrid=new float[anchors.Length];
   queryGrid[0]=anchors[0];
   queryGrid[notes.Count]=end;
   float span=(float)((double)end-anchors[0]);
   float unitsPerBeat=(float)((double)span/total);
   for(int i=1;i<notes.Count;i++)
    queryGrid[i]=(float)((double)notes[i-1][6]*unitsPerBeat+queryGrid[i-1]);
   for(int i=1;i<anchors.Length;i++){
    bool flatTerminal=i==anchors.Length-1&&anchors[i]==anchors[i-1]&&upperAnchors[i-1]==anchors[i];
    if((!flatTerminal&&(!(anchors[i]>anchors[i-1])||
        !(upperAnchors[i-1]>anchors[i-1]&&upperAnchors[i-1]<=anchors[i])))||
       !(queryGrid[i]-queryGrid[i-1]>.001f))
     throw new NotSupportedException("Reference spacing needs increasing horizontal anchors and distinct query positions.");
   }
  }

  // Reference setup consumes the common rhythmic collector's positions and
  // reduced durations. Coincident notes can leave duration in the scratch
  // group for later onsets, even when both notes have independent stems.
  static List<float[]> CollectReferenceRows(IList<float[]> staff,Action<int,float> normalizedPosition){
   var result=new List<float[]>();if(staff.Count==0)return result;
   var words=new List<float>();var starts=new int[staff.Count];
   for(int i=0;i<staff.Count;i++){
    starts[i]=words.Count+1;words.Add(staff[i].Length);words.AddRange(staff[i]);
   }
   var rhythm=new float[staff.Count+4];var positions=new float[rhythm.Length];var scratch=new float[rhythm.Length];
   float number=staff[0][1];var bank=words.ToArray();var collected=LayoutRhythmicCollection.Collect(bank,starts,number,0,1,rhythm,positions,scratch);
   // These are private timing-table rows, never emitted as notation items.
   for(int i=0;i<collected.Count;i++){
    float duration=rhythm[i],x=positions[i];
    if(duration>0)result.Add(new[]{1f,number,x,0,10,0,duration});
    else if(duration==-64)result.Add(new[]{1f,number,x,100,10,0,65,100});
    else if(duration==-14||duration==-140)result.Add(new[]{14f,number,x,1});
    else if(duration==-3||duration==-17||duration==-18)result.Add(new[]{-duration,number,x});
    else throw new NotSupportedException("Reference timing marker is outside the verified projection.");
   }
   if(normalizedPosition!=null)for(int i=0;i<staff.Count;i++)
    if(staff[i].Length>=3&&bank[starts[i]+2]!=staff[i][2])normalizedPosition(i,bank[starts[i]+2]);
   return result;
  }

  static float P(float[] row,int parameter){return row.Length>=parameter?row[parameter-1]:0;}
  static bool OpeningClef(float[] row){
   float kind=P(row,5);
   return row[0]==3&&kind>=0&&kind<=4&&kind==Math.Truncate(kind);
  }
  static bool Bar(float[] row){
   if(row[0]!=14)return false;
   float style=P(row,5);
   if(P(row,4)<0)return style>=-1&&style<=7&&style==Math.Truncate(style);
   float span=P(row,4);
   return span>=1&&span<=32&&span==Math.Truncate(span)&&style>=-1&&style<=11&&style==Math.Truncate(style);
  }
  static bool Structural(float[] row){
   return Bar(row)||row[0]==3||row[0]==17||row[0]==18;
  }
  static bool Grace(float[] row){return row[0]==1&&P(row,7)==65;}
  static bool GraceMember(float[] row){return row[0]==1&&P(row,7)==64&&P(row,5)%100<10;}
  static bool SupportedEvent(float[] row,bool working){
   if(Structural(row))return true;
   float duration=P(row,7);
   if(Grace(row))return Math.Abs(P(row,4))>=100&&Math.Abs(P(row,4))<200&&P(row,8)>=100&&P(row,15)==0;
   if(GraceMember(row))return false;
   if(row.Length<7||!(duration<65&&(duration>0||working&&duration==0)))return false;
   if(row[0]==1)return true;
   // Reference timing consumes rest onsets/durations, independently of drawing controls.
   return row[0]==2;
  }
  static List<float[]> SpacingEvents(IList<float[]> records,int first,bool working){
   var events=new List<float[]>();
   float[] leader=null;
   for(int i=first;i<records.Count;i++){
    var row=records[i];
    // Working notes carry a member marker. Compact SCORE items retain the
    // stemless P5 and share the preceding chord leader's onset and duration.
    if(leader!=null&&leader[0]==1&&row[0]==1&&row.Length>=7&&
       row[2].Equals(leader[2])&&(row[6]==leader[6]||Grace(leader)&&GraceMember(row))&&Math.Abs(row[3])<200&&
       leader[4]%100>=10&&leader[4]%100<30&&row[4]>=0&&row[4]%100<10&&P(row,15)==(working?-1:0))
     continue;
    if(!SupportedEvent(row,working))throw new NotSupportedException("This item falls outside the verified reference spacing route.");
    leader=row;
    events.Add(row);
   }
   return events;
  }
  internal string Apply(List<float[]> records,float scale){
   int leadingBar=records.Count>0&&Bar(records[0])?1:0;
   bool hasClef=records.Count>leadingBar&&OpeningClef(records[leadingBar]);
   int first=leadingBar+(hasClef?1:0);
   if(records.Count<=first)throw new NotSupportedException("Reference spacing needs a linked timed event.");
   float actual=0;
   foreach(var row in SpacingEvents(records,first,true))
    if(!Grace(row)&&!Structural(row))actual=row[6]==0?float.NaN:(float)((double)actual+row[6]);
   if(leadingBar==1)records[0][2]=0;
   if(hasClef)records[leadingBar][2]=clefX;
   int interval=0,markerIndex=0;
   float left=queryGrid[0],upper=(float)((double)queryGrid[1]-.0010000000474974513);
   for(int i=first;i<records.Count;i++){
    if(Grace(records[i])||GraceMember(records[i])){records[i][2]=-1;continue;}
    if(Structural(records[i])){
     var row=records[i];
     int previous=i-1;
     while(previous>0&&(records[previous][2]<0||float.IsNaN(records[previous][2])))previous--;
     float prior=records[previous][2],position=prior;
     int match=markerIndex;
     while(match<markers.Count&&markers[match].Code!=row[0])match++;
     if(match<markers.Count){
      markerIndex=match+1;
      if(markers[match].X>=prior)position=markers[match].X;
      else if(Bar(row))for(int candidate=markerIndex;candidate<markers.Count&&markers[candidate].Slot<=100;candidate++){
       if(markers[candidate].Code!=14||markers[candidate].X<prior)continue;
       position=markers[candidate].X;
       break;
      }
     }
     records[i][2]=position;
     continue;
    }
    float query=records[i][2];
    while(upper<=query){
     if(interval+2>=anchors.Length)throw new NotSupportedException("Reference onset falls beyond the verified anchor table.");
     interval++;
     left=upper;
     upper=(float)((double)queryGrid[interval+1]-.0010000000474974513);
    }
    records[i][2]=(float)(((double)upperAnchors[interval]-anchors[interval])*((double)query-left)/
     ((double)upper-left)+anchors[interval]);
   }
   for(int i=records.Count-1;i>=first;i--){
    bool grace=Grace(records[i]),indefinite=float.IsNaN(records[i][2]);
    if(!grace&&(!indefinite||records[i][0]==1&&P(records[i],15)==-1))continue;
    int following=i+1;
    while(following<records.Count&&(grace&&GraceMember(records[following])||
      indefinite&&records[following][0]==1&&P(records[following],15)==-1))following++;
    float position=(float)((double)end-4);
    if(following<records.Count){
     int previous=i-1;
     while(previous>0&&!(records[previous][2]>0))previous--;
     float next=records[following][2],gap=(float)((double)next-records[previous][2]);
     float offset=gap<=4?(float)((double)gap/2):3;
     position=(float)((double)next-offset);
    }
    for(int member=i;member<following;member++)records[member][2]=position;
   }
   for(int i=first;i<records.Count;i++){
    var row=records[i];
    if(!Structural(row))continue;
    bool trailing=i==records.Count-1;
    float next=trailing?end:records[i+1][2];
    float position=row[2],prior=records[i-1][2];
    if(next>=position&&position!=prior)continue;
    if(Bar(row))row[2]=trailing?200:(float)((double)next-(double)scale*2.5);
    else if(row[0]==18){
     int previousCode=(int)records[i-1][0];
     float origin=trailing&&previousCode!=14?end:prior;
     row[2]=(float)((double)origin+(double)scale*(previousCode==3?5.5:1.5));
    }
    else if(row[0]==17&&trailing)row[2]=(float)((double)prior+(double)scale*2);
    else{
     float width=row[0]==3?6.5f:(float)(((double)Math.Abs(P(row,5))-1)*2.5+4.5);
     double gap=(double)next-position;
     if(gap<width)width=(float)(gap/2);
     row[2]=(float)((double)next-(double)scale*width);
    }
   }
   if(float.IsNaN(actual)||Math.Abs((float)((double)total-actual))<=.007000000216066837f)return null;
   return String.Format(CultureInfo.InvariantCulture,
    "*** Mismatch - Spacing Staff has {0,7:F4} Quarter Notes.\r\n{1,8} Quarter notes on this staff.",
    total,actual==0?".0000":actual.ToString("F4",CultureInfo.InvariantCulture));
  }
 }
}
