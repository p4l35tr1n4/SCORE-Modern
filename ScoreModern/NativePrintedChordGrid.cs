using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintedChordGrid {
  internal sealed class Grid {
   public float X,Y,Top,Right,Horizontal,Vertical,Size,TextSize,TextY,FingeringSize;
   public int Strings,Frets,Markers,Font,Fingering,Barre;
   public bool Nut,Visible;
   public string Name;
  }
  static readonly float[] CrossX={-55,45,55,-45,-55};
  static readonly float[] CrossY={34,135,135,34,34};
  static float F(double x){return (float)x;}
  internal sealed class State {
   internal Grid Geometry;internal int First,Last,Minimum=99,LastOpen;
   internal void Advance(Grid grid,Item item){
    // Lookup writes only the first endpoint. Geometry/string loops and
    // 0B98:04CB..0506 independently replace the retained endpoint words.
    if(item.P(5)<=-4)First=item.P(5)==-5?1:Math.Max(0,Math.Min(4,(int)item.P(9)))+1;
    if(!grid.Visible)return;
    Geometry=grid;First=grid.Strings*160;
    if(grid.Barre>=100){First=grid.Barre/1000;if(First==0)First=1;Last=grid.Barre/100%10;if(Last==0)Last=6;}
    if(grid.Markers==0)return;
    Minimum=99;LastOpen=0;
    int remaining=grid.Markers,divisor=(int)Math.Pow(10,Math.Min(grid.Strings,6)-1);
    for(int n=1;n<=Math.Min(grid.Strings,6);n++){
     int digit=remaining/divisor;remaining%=divisor;divisor=Math.Max(1,divisor/10);
     if(digit>=8)LastOpen=n;
     else if(digit>0&&(n==First||n==Last))Minimum=Math.Min(Minimum,digit);
    }
   }
  }
  internal static State InitialState(NativePageDocument document,Score score,ScoreText text){
   var state=new State();if(score==null)return state;
   // Natural G redraw precedes the measured F6 scan. Run the same recovered
   // cache writes without drawing; no fixture-specific endpoint seed.
   foreach(var item in score.Items)if(item.Code==12&&item.P(5)<0){var grid=Prepare(item,document.Contexts[item],text);if(grid!=null)state.Advance(grid,item);}
   return state;
  }
  static List<PrimitiveTextPoint> RetainedBarre(Grid grid,State state,NativeNotationContext p,NativeDatGlyphs dat){
   if(grid.Visible||grid.Barre<100||state==null||state.Geometry==null||state.Last-state.First<1)return new List<PrimitiveTextPoint>();
   var frame=state.Geometry;int span=state.Last-state.First;
   float top=frame.Nut?F((double)frame.Top+frame.Vertical*60/160):frame.Top;
   float y=state.Minimum>1?F((double)top-(state.Minimum-1)*frame.Vertical):F((double)top+grid.Size*p.Scale*(state.LastOpen>=state.First&&state.LastOpen<=state.Last?8:4));
   float x=F((double)frame.X+(state.First-1)*frame.Horizontal),aspect=F((double)frame.Horizontal/frame.Vertical);
   return dat.ExternalDrawing(209,F((double)span*grid.Size*aspect*.2f),F((double)grid.Size*(span*.05f+.55f)),p.Scale,x,y);
  }

  // SCORE Code 12 grid, relative 0273:1010–1FF7.
  static Grid Prepare(Item i,NativeNotationContext p,ScoreText text){
   int subtype=(int)i.P(5),root=(int)i.P(7),dims=(int)i.P(14);
   if(subtype<-6||subtype>-1||subtype!=i.P(5)||root!=i.P(7)||root/10<1||root/10>7||root%10>2||
    dims!=i.P(14)||dims<0||dims>99||i.P(13)<0||i.P(13)>999999||i.P(13)!=(int)i.P(13))return null;
   bool lookup=subtype<=-4;
   if(i.P(20)!=0)return null;
   if(!lookup&&(i.P(8)<0||i.P(8)>9||i.P(8)!=(int)i.P(8)||i.P(9)<0||i.P(9)>699||i.P(9)!=(int)i.P(9)||
    i.P(10)<0||i.P(10)>3399||i.P(10)!=(int)i.P(10)||i.P(11)<0||i.P(11)>7399||i.P(11)!=(int)i.P(11)))return null;
   if(i.P(18)<0||i.P(18)>999999||i.P(18)!=(int)i.P(18)||i.P(12)<0||i.P(12)>=6667||Single.IsNaN(i.P(12)))return null;
   float size=i.P(6)==0?1:i.P(6),aspect=i.P(17)==0?1:Math.Abs(i.P(17));
   if(!(size>0&&size<=10&&aspect>0&&aspect<=10&&Math.Abs(i.P(15))<=200&&Math.Abs(i.P(19))<=50))return null;
   int columns=dims/10,frets=dims%10;
   if(columns<2)columns=6;
   if(frets==0)frets=5;
   int font=(int)i.P(16)/10;
   float labelScale=F(i.P(16)%10);
   if(font<0||font>99||labelScale<0)return null;
   if(labelScale==0)labelScale=1;
   float unit=F((double)size*p.Scale*.07500000298023224f);
   float x=F((double)p.Prepared.X+i.P(15)*150),y=p.Prepared.Y;
   var grid=new Grid{X=x,Y=y,Horizontal=F((double)unit*aspect*160),Vertical=F((double)unit*160),
    Size=size,TextSize=F((double)size*labelScale),Strings=columns,Frets=frets,Markers=(int)i.P(13),Font=font,
    Visible=true,Nut=false,Barre=(int)i.P(12),Fingering=(int)i.P(18),Name="CDEFGAB"[root/10-1].ToString()};
   if(root%10!=0)grid.Name+="?"+(root%10);
   if(!lookup)grid.Name+=ManualSuffix(i);
   if(lookup){
    if(i.P(10)!=0||i.P(11)!=0||i.P(12)!=0||i.P(13)!=0)return null;
    int chord=(int)i.P(8),alternative=(int)i.P(9);
    if(chord<0||chord>20||i.P(8)!=chord||i.P(9)!=alternative)return null;
    if(chord==0)chord=1;
    if(alternative<0||alternative>4||subtype==-5)alternative=0;
    ReadChord(grid,text.RootDirectory,root,chord,alternative);
    if(subtype==-6)grid.Name="";
   }
   // Original 0273:211B-2126 / 226C-2279: lookup grids become -1,
   // except name-only -5; a fret number switches that result to -2.
   // 111A-1140 then forces -2 for negative P17 before visibility tests.
   int style=lookup?(subtype==-5?-5:-1):subtype;
   if(lookup&&grid.Barre%100!=0)style=-2;
   if(i.P(17)<0)style=-2;
   grid.FingeringSize=F((double)size*F((1-(double)labelScale)/3.5+labelScale));
   grid.Visible=style!=-3&&style!=-5;
   grid.Nut=style==-1;
   grid.Top=F((double)y+grid.Vertical*frets);
   grid.Right=F((double)x+grid.Horizontal*(columns-1));
   float titleBaseP4=F((double)(frets-1)*size*1.7200000286102295f+i.P(4));
   float titleP4=F(((double)i.P(19)+5.199999809265137f)*size+titleBaseP4);
   grid.TextY=grid.Visible?F((double)titleP4*p.Scale*7+p.RawBase):y;
   return grid;
  }

  static string ManualSuffix(Item item){
   string[] kinds={"","maj","m","?0","?9","+","dim","aug","?-","?8"};
   string suffix=kinds[(int)item.P(8)];
   int interval=(int)item.P(9),addition=(int)item.P(10),bass=(int)item.P(11);
   suffix+=Interval(interval);
   if(addition!=0){
    int mode=addition/1000;
    string prefix=mode==1?"sus":mode==2?"add":mode==3?"(add":"";
    suffix+=prefix+Interval(addition%1000)+(mode==3?")":"");
   }
   if(bass!=0){
    int letter=bass/1000;
    suffix+="/"+(letter==0?"":"CDEFGAB"[letter-1].ToString())+Interval(bass%1000);
   }
   return suffix;
  }
  static string Interval(int value){
   int modifier=value/100,number=value%100;
   string mark=modifier>=1&&modifier<=3?"?"+modifier:modifier==4?"+":modifier==5?"?-":modifier==6?"8":"";
   return mark+(number==0?"":number.ToString(CultureInfo.InvariantCulture));
  }
  // 0273:20F8-2291 indexes twenty entries per chromatic pitch.
  static void ReadChord(Grid grid,string rootPath,int note,int chord,int alternative){
   int pitch=note*2/10-1;
   if(pitch>6)pitch--;
   if(note%10!=0)pitch+=2*(note%10)-3;
   pitch=(pitch+11)%12;
   string file=NativeLibraryLocation.FileFor(rootPath,"GUITAR.DAT");
   int row=pitch*20+chord-1;
   using(var reader=new StreamReader(file,System.Text.Encoding.GetEncoding(437))){
    string line=null;
    for(int n=0;n<=row;n++)line=reader.ReadLine();
    if(line==null)throw new InvalidDataException("GUITAR.DAT is missing the requested chord row.");
    string[] fields=line.Split(';');
    if(fields.Length<3)throw new InvalidDataException("Invalid GUITAR.DAT chord row.");
    string[] values=fields[1].Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
    int index=1+3*alternative;
    if(values.Length<index+3||values[index].Length!=6)throw new InvalidDataException("GUITAR.DAT is missing the requested fingering.");
    grid.Markers=0;
    foreach(char marker in values[index]){
     int digit=marker=='x'||marker=='X'?9:marker=='o'||marker=='O'?8:marker-'0';
     if(digit<0||digit>9)throw new InvalidDataException("Invalid GUITAR.DAT string marker.");
     grid.Markers=grid.Markers*10+digit;
    }
    grid.Barre=ReadNumber(values[index+1]);
    if(grid.Fingering==0)grid.Fingering=ReadNumber(values[index+2]);
    grid.Nut=grid.Barre%100==0;
    string name=fields[0].Trim();
    if(name.Length==0||name.Length>20||name[0]<'A'||name[0]>'G')
     throw new NotSupportedException("GUITAR.DAT chord name outside the verified format.");
    int prefix=name.Length>=3&&name[1]=='?'&&(name[2]=='1'||name[2]=='2')?3:1;
    grid.Name+=name.Substring(prefix);
   }
  }
  static int ReadNumber(string source){
   int value;
   if(!Int32.TryParse(source,NumberStyles.None,CultureInfo.InvariantCulture,out value))
    throw new InvalidDataException("Invalid GUITAR.DAT numeric field.");
   return value;
  }

  static Item Label(Item source,float size,int font){
   return Item.Make(16,source.P(2),source.P(3),source.P(4),1,size,0,1000+font,0,0,0,1,0);
  }
  static NativeNotationContext LabelPosition(NativeNotationContext p,float x,float y){
   var context=new NativeNotationContext{Staff=p.Staff,Projection=p.Projection,RawBase=p.RawBase,Prepared=p.Prepared};
   context.Prepared.X=x;
   context.Prepared.Y=y;
   return context;
  }
  static float LabelX(Grid grid,NativeNotationContext p,ScoreText text,Item label){
   if(!grid.Visible)return grid.X;
   // The text converter returns the width in horizontal SCORE units.
   return F((double)grid.X+((double)grid.Right-grid.X-text.PrepareLayout(label,grid.Name,p.Scale,0).Width*150)/2);
  }
  static void Point(List<PrimitiveTextPoint> points,float x,float y,short op=0){
   points.Add(new PrimitiveTextPoint{X=x,Y=y,Operation=op});
  }
  static void Fingerings(Grid grid,Item item,NativeNotationContext p,ScoreText text,Graphics g,List<PrimitiveTextPoint> points,int column){
   if(grid.Fingering==0||column>=6)return;
   int digit=grid.Fingering.ToString("D6",CultureInfo.InvariantCulture)[column]-'0';
   if(digit>=5&&digit<=7)return;
   string value=digit==9?"T":digit==8?"0":digit.ToString(CultureInfo.InvariantCulture);
   float columnX=F((double)grid.X+grid.Horizontal*column);
   float x=F((double)columnX-grid.FingeringSize*.03200000151991844f*p.Scale*150);
   if(g!=null){
    // 0273:13D5-13E4 stores P3 after division/subtraction; the printer
    // truncates the subsequent single-precision product by 150.
    float offset=F((double)p.Scale*.03200000151991844f*grid.FingeringSize);
    float position=F((double)columnX/150-offset);
    x=(int)F((double)position*150);
   }
   float y=F((double)grid.Y-grid.FingeringSize*2.5999999046325684f*p.Scale*7);
   TextAt(g,item,p,text,points,value,F((double)grid.FingeringSize*.6000000238418579f),0,x,y);
  }
  static void TextAt(Graphics g,Item item,NativeNotationContext p,ScoreText text,List<PrimitiveTextPoint> points,string value,float size,int font,float x,float y,float spacing=0){
   var body=Label(item,size,font);
   body.Set(10,spacing);
   var context=LabelPosition(p,x,y);
   if(g==null)points.AddRange(text.ScanStrokeText(body,value,context,0));
   else{
    text.Set(body,value,p.Scale);
    text.Draw(g,body,body.Text,0,0,p.Scale/5,p.Scale,context);
   }
  }
  static List<PrimitiveTextPoint> Lines(Grid grid,bool printed,Item item,NativeNotationContext p,ScoreText text,Graphics g){
   var points=new List<PrimitiveTextPoint>();
   if(!grid.Visible)return points;
   for(int column=0;column<grid.Strings;column++){
    float x=F((double)grid.X+column*grid.Horizontal);
    Fingerings(grid,item,p,text,g,points,column);
    Point(points,x,grid.Y,1);
    Point(points,x,grid.Top);
   }
   for(int fret=0;fret<=grid.Frets;fret++){
    float y=F((double)grid.Y+fret*grid.Vertical);
    Point(points,grid.X,y,1);
    Point(points,grid.Right,y);
   }
   if(grid.Nut){
    float upper=F((double)grid.Top+grid.Vertical*60/160);
    Point(points,grid.X,upper,2);
    Point(points,grid.X,grid.Top);
    Point(points,grid.Right,grid.Top);
    Point(points,grid.Right,upper);
    Point(points,grid.X,upper);
    if(printed)Point(points,grid.X,grid.Top);
   }
   return points;
  }
  static bool LastFingeringIsZero(Grid grid){
   if(grid.Fingering==0)return false;
   string digits=grid.Fingering.ToString("D6",CultureInfo.InvariantCulture);int last=-1;
   for(int n=0;n<Math.Min(6,grid.Strings);n++){int digit=digits[n]-'0';if(digit<=4||digit>=8)last=digit;}
   return last==0||last==8;
  }
  static void Markers(Grid grid,NativeNotationContext p,NativeDatGlyphs dat,List<PrimitiveTextPoint> points,Action<float,float,float,bool> arc){
   if(!grid.Visible)return;
   int remaining=grid.Markers,divisor=(int)Math.Pow(10,Math.Min(grid.Strings,6)-1);
   float unit=F((double)grid.Vertical/160);
   bool firstCircle=grid.Fingering!=0&&grid.Fingering!=666666,firstPrintedCircle=true;
   // G124/G127 original captures: the final zero fingering clears DS:8D52
   // before the first circle. The circle return changes it to the print flag -1.
   bool printedZero=LastFingeringIsZero(grid);
   float markerSize=F((double)grid.Size*1.2999999523162842f);
   for(int n=0;n<Math.Min(6,grid.Strings);n++){
    int digit=remaining/divisor;remaining%=divisor;divisor=Math.Max(1,divisor/10);
    if(digit==0)continue;
    float x=F((double)grid.X+n*grid.Horizontal);
    float y=F((double)grid.Top+(digit<8?80-digit*160:(grid.Nut?60:0)+(digit==8?95:10))*unit);
    if(digit==9){
     for(int side=0;side<2;side++)
      for(int k=0;k<5;k++)
       Point(points,F((double)x+(side==0?1:-1)*CrossX[k]*unit),F((double)y+CrossY[k]*unit),(short)(k==0?2:0));
     continue;
    }
    float radius=F((double)markerSize*(digit==8?.800000011920929f:1));
    if(arc!=null&&grid.Fingering==0){
     arc(x,y,F((double)p.Scale*radius*3.5),digit!=8);
     // 2453:0271 writes the circle printer's format flag to DS:8281,
     // the second byte of the grid's working marker size at DS:8280.
     var bytes=BitConverter.GetBytes(markerSize);
     bytes[1]=0x20;
     markerSize=BitConverter.ToSingle(bytes,0);
    }
    else points.AddRange(dat.Circle(p.Scale,x,y,radius,radius,digit!=8,arc!=null?(firstPrintedCircle&&printedZero?20:10):(firstCircle?10:20)));
    firstCircle=false;firstPrintedCircle=false;
   }
  }
  static void Paint(Graphics g,NativeNotationContext page,List<PrimitiveTextPoint> points,float width){
   if(points.Count!=0)NativeNoteDetails.Path(g,page,points.ConvertAll(q=>new BarlinePoint(q.X,q.Y,q.Operation)),width);
  }
  static List<PrimitiveTextPoint> Barre(Grid grid,NativeNotationContext p,NativeDatGlyphs dat){
   int first=Math.Max(1,grid.Barre/1000),last=grid.Barre/100%10;
   if(last==0)last=6;
   if(!grid.Visible||grid.Barre<100||last<=first||last>6)return new List<PrimitiveTextPoint>();
   int remaining=grid.Markers,divisor=(int)Math.Pow(10,Math.Min(grid.Strings,6)-1);
   int fret=99,lastOpen=0;
   for(int n=0;n<Math.Min(6,grid.Strings);n++){
    int digit=remaining/divisor;remaining%=divisor;divisor=Math.Max(1,divisor/10);
    if(digit>=8)lastOpen=n+1;
    if((n+1==first||n+1==last)&&digit>0&&digit<8)fret=Math.Min(fret,digit);
   }
   if(fret==99&&!NativeEditCompatibility.ReproduceInvalidStates)throw new NotSupportedException("Guitar barre endpoints have no fretted markers. Enable original invalid-state reproduction to retain SCORE's wrapped barre coordinates.");
   float x=F((double)grid.X+(first-1)*grid.Horizontal);
   float y=fret>1?F((double)grid.Top-(fret-1)*grid.Vertical):
    F((double)grid.Top+grid.Size*p.Scale*(lastOpen>=first&&lastOpen<=last?8:4));
   if(grid.Nut&&fret==1)y=F((double)y+grid.Size*p.Scale*4.5);
   int span=last-first;
   float aspect=F((double)grid.Horizontal/grid.Vertical);
   return dat.ExternalDrawing(209,F((double)span*grid.Size*aspect*.2f),F((double)grid.Size*(span*.05f+.55f)),p.Scale,x,y);
  }
  static void Fret(Graphics g,Grid grid,Item item,NativeNotationContext p,ScoreText text,List<PrimitiveTextPoint> points){
   int fret=grid.Barre%100;
   if(!grid.Visible||fret==0)return;
   float x=F((double)grid.Right+grid.Size*p.Scale*.04800000041723251f*150);
   // 0273:18D0-18E4 / 1957-198C stores each P4 stage before
   // the ordinary text-coordinate preparation; combining them loses a word.
   float topText=F((double)(grid.Frets-1)*grid.Size*1.7200000286102295f+item.P(4));
   float fretP4=F((double)grid.Size*.20000000298023224f+topText);
   fretP4=F((double)fretP4-F(item.P(12)%1)*grid.Size*17.200000762939453f);
   float y=F((double)fretP4*p.Scale*7+p.RawBase);
   TextAt(g,item,p,text,points,fret.ToString(CultureInfo.InvariantCulture)+" fr",F((double)grid.FingeringSize*.6000000238418579f),0,x,y,-.65f);
  }
  internal static List<PrimitiveTextPoint> Scan(Item i,NativeNotationContext p,ScoreText text,NativeDatGlyphs dat,short previousFont,out short finalFont,State state=null){
   var grid=Prepare(i,p,text);
   if(grid==null)throw new NotSupportedException("Guitar-grid controls outside the verified route.");
   if(state!=null)state.Advance(grid,i);
   var points=Lines(grid,false,i,p,text,null);
   Markers(grid,p,dat,points,null);
   Fret(null,grid,i,p,text,points);
   points.AddRange(Barre(grid,p,dat));
   if(NativeEditCompatibility.ReproduceInvalidStates)points.AddRange(RetainedBarre(grid,state,p,dat));
   if(grid.Name.Length==0){finalFont=grid.Fingering!=0&&grid.Fingering!=666666||grid.Barre%100!=0?(short)0:previousFont;return points;}
   var label=Label(i,grid.TextSize,grid.Font);
   points.AddRange(text.ScanStrokeText(label,grid.Name,LabelPosition(p,LabelX(grid,p,text,label),grid.TextY),0,out finalFont));
   return points;
  }
  public static void Validate(Item item,NativeNotationContext page,ScoreText text){if(Prepare(item,page,text)==null)throw new InvalidDataException("Guitar chord parameters are outside the recovered SCORE grid controls.");}
  public static bool Draw(Graphics g,Item item,NativeNotationContext p,ScoreText text,DrawingLibrary library){
   if(p==null||text==null||NativeNotationView.UsesDat)return false;
   var grid=Prepare(item,p,text);
   if(grid==null)return false;
   var state=NativeEditCompatibility.ReproduceInvalidStates?NativePrintContext.GridState(p.SourceScore,text):null;if(state!=null)state.Advance(grid,item);
   var width=NativePrintContext.DefaultLineWidth;
   var points=Lines(grid,true,item,p,text,g);
   var dat=NativeDatDrawing.Get(library);
   Markers(grid,p,dat,points,(x,y,r,filled)=>NativePrintedShapes.DrawArc(g,p,x,y,r,1,-270,90,width,filled,!filled,exactDecimal:true));
   Fret(g,grid,item,p,text,points);
   var barre=Barre(grid,p,dat);if(NativeEditCompatibility.ReproduceInvalidStates)barre.AddRange(RetainedBarre(grid,state,p,dat));
   var titleVectors=new List<PrimitiveTextPoint>();
   if(grid.Name.Length!=0&&NativeEpsExport.For(g)==null&&!GhostscriptTextRenderer.Enabled){
    var label=Label(item,grid.TextSize,grid.Font);
    TextAt(g,item,p,text,points,grid.Name,grid.TextSize,grid.Font,LabelX(grid,p,text,label),grid.TextY);
   }else if(grid.Name.Length!=0){
    var label=Label(item,grid.TextSize,grid.Font);
    text.Set(label,grid.Name,p.Scale);
    var context=LabelPosition(p,LabelX(grid,p,text,label),grid.TextY);
    var plan=text.PreparePostScript(label,context,GhostscriptTextRenderer.DeviceMatrix(g));
    if(plan==null)throw new NotSupportedException("Guitar-grid title contains unsupported SCORE text.");
    foreach(var part in plan.Parts){
     var token=part.Vector;
     if(token==null)continue;
     int face=token.FontWord%100;
     titleVectors.AddRange(face==98?text.StrokePoints(token.Code,plan.Layout,p.Scale,part.X,plan.Y):
      text.DrawingPoints(face>=90?face:90,token.Code>=162?token.Code-68:token.Code,plan.Layout,p.Scale,part.X,plan.Y));
    }
    plan.Parts.RemoveAll(part=>part.Vector!=null);
    if(NativeEpsExport.For(g)==null){
     foreach(var part in plan.Parts)GhostscriptTextRenderer.Prepare(part.Font,GhostscriptTextRenderer.Clip(g));
    }
    text.PaintPostScript(g,plan);
   }
   if(barre.Count==0)points.AddRange(titleVectors);
   Paint(g,p,points,width);
   if(barre.Count!=0){
    Paint(g,p,barre,NativePrintContext.Width(-1));
    Paint(g,p,titleVectors,width);
   }
   return true;
  }
 }
}



