using System;using System.IO;using System.Globalization;using System.Collections.Generic;
namespace ScoreModern {
 public sealed class NativePrintJob:IDisposable {
  [ThreadStatic]public static NativePrintJob Active;NativePrintJob previous;bool automaticResolved;public short? AutomaticMinimum;internal NativePrintExtents ResolvedExtents;public string OffsetWarning{get;private set;}
  public string Source,Output="",Header="None",Project="";public float HeaderOffset=-.18f,FooterOffset=.18f;public float Size=1,PaperWidth=8.5f,PaperHeight=11;public int Left=2000,Bottom=3000;int? configuredBottom;public short Resolution=300,LineWidth=2,Tray,Desc;public bool Freeze,Rotate,Mirror,Stroke,Manual;
  static float F(double n){return (float)n;}static readonly CultureInfo Culture=CultureInfo.InvariantCulture;
  static float Number(string value){int split=value.IndexOfAny(new[]{' ','\t',','});if(split>=0)value=value.Substring(0,split);float n;if(!Single.TryParse(value,NumberStyles.Float,Culture,out n)||Single.IsNaN(n)||Single.IsInfinity(n))throw new InvalidDataException("Invalid finite print parameter: "+value);return n;}
  static bool YesNo(string value){if(value.Equals("Yes",StringComparison.OrdinalIgnoreCase))return true;if(value.Equals("No",StringComparison.OrdinalIgnoreCase))return false;throw new NotSupportedException("Unverified print boolean: "+value);}
  static short Word(string value){float n=Number(value);if(n!=Math.Truncate(n)||n<short.MinValue||n>short.MaxValue)throw new NotSupportedException("Print parameter requires a signed16 integer.");return (short)n;}
  internal void SetOffsets(float left,float bottom){int x=checked((int)Math.Truncate((double)left*4000)),y=checked((int)Math.Truncate((double)bottom*4000));Left=x;Bottom=y;configuredBottom=y;automaticResolved=false;OffsetWarning=null;}
  internal void SetRotation(bool rotate){if(rotate==Rotate)return;Size=RecoveredFragments.PrintRotation.Apply(Size,Rotate,rotate);if(!Freeze){Bottom=RecoveredFragments.PrintRotation.Offset(configuredBottom??Bottom,Rotate,rotate);configuredBottom=Bottom;automaticResolved=false;AutomaticMinimum=null;ResolvedExtents=null;OffsetWarning=null;}Rotate=rotate;}
  public static NativePrintJob Defaults(string root){var data=NativePreferences.Read(root);short resolution=data.Word("RESOLUTION",300);return new NativePrintJob{Resolution=resolution,LineWidth=(short)Math.Round(resolution/150.0,MidpointRounding.AwayFromZero),Stroke=data.Word("SETSTROKE",0)!=0};}
  internal NativePrintJob CopyConfiguration(){var copy=(NativePrintJob)MemberwiseClone();copy.previous=null;copy.automaticResolved=false;copy.AutomaticMinimum=null;copy.ResolvedExtents=null;copy.OffsetWarning=null;return copy;}
  public static NativePrintJob Read(string file){return Read(file,null);}
  public static NativePrintJob Read(string file,NativePrintJob retained){var job=retained==null?new NativePrintJob():(NativePrintJob)retained.MemberwiseClone();job.Source=Path.GetFullPath(file);job.previous=null;job.configuredBottom=null;job.automaticResolved=false;job.AutomaticMinimum=null;job.ResolvedExtents=null;job.OffsetWarning=null;var lines=File.ReadAllLines(file,System.Text.Encoding.GetEncoding(437));for(int index=0;index<lines.Length;index++){string line=lines[index].Trim();if(line.Length==0)continue;int space=line.IndexOfAny(new[]{' ','\t'});string key=space<0?line:line.Substring(0,space),value=space<0?"":line.Substring(space).Trim();switch(key.ToUpperInvariant()){
   case "OUTPUT":job.Output=value.TrimEnd('\0',' ','\t');break;case "SIZE":job.Size=Number(value);break;case "PAPER":case "OFFSETS":
   if(key.Equals("PAPER",StringComparison.OrdinalIgnoreCase)){
    string standard=value.ToUpperInvariant();
    if(standard=="LTR"||standard=="LEG"||standard=="LDG"){
     job.PaperWidth=standard=="LDG"?11:8.5f;
     job.PaperHeight=standard=="LEG"?14:standard=="LDG"?17:11;
     break;
    }
   }
   var parts=value.Split(new[]{' ','\t',','},StringSplitOptions.RemoveEmptyEntries);if(parts.Length!=2)throw new InvalidDataException(key+" requires two values.");if(key.Equals("PAPER",StringComparison.OrdinalIgnoreCase)){job.PaperWidth=Number(parts[0]);job.PaperHeight=Number(parts[1]);}else{job.Left=checked((int)Math.Truncate((double)Number(parts[0])*4000));job.Bottom=checked((int)Math.Truncate((double)Number(parts[1])*4000));}break;
   case "FREEZE":job.Freeze=YesNo(value);break;case "RESOLUTION":job.Resolution=Word(value);break;case "LINE_WIDTH":job.LineWidth=Word(value);break;case "HEADER/FOOTER":
    job.Header=value;
    if(!value.Equals("None",StringComparison.OrdinalIgnoreCase)){
     string mode=value.ToUpperInvariant();
     bool header=mode=="HEADER"||mode=="BOTH",footer=mode=="FOOTER"||mode=="BOTH";
     if(!header&&!footer)throw new NotSupportedException("Unrecognized SCORE header/footer mode: "+value);
     if(index+(header&&footer?3:2)>=lines.Length)throw new InvalidDataException("Incomplete SCORE header/footer settings.");
     job.Project=lines[++index].TrimEnd();
     if(job.Project.Length>18)throw new InvalidDataException("SCORE project names are limited to 18 characters.");
     if(header)job.HeaderOffset=HeaderInput(Number(lines[++index].Trim()),job.HeaderOffset);
     if(footer)job.FooterOffset=FooterInput(Number(lines[++index].Trim()),job.FooterOffset);
    }
    break;case "ROTATE":job.Rotate=YesNo(value);break;case "MIRROR":job.Mirror=YesNo(value);break;case "SETSTROKE":job.Stroke=YesNo(value);break;case "MANUAL_FEED":job.Manual=YesNo(value);break;case "SETPAPERTRAY":job.Tray=Word(value);break;case "DESC":job.Desc=Word(value);break;default:throw new NotSupportedException("Unverified PRINT.SCR directive: "+key);
  }}if(job.Size<=0||job.PaperWidth<=0||job.PaperHeight<=0||job.Resolution<=0)throw new InvalidDataException("Print size, paper dimensions and resolution must be positive.");return job;}
  // Original2E2C:0D14..0D46 /0E11..0E3F: positive zero retains the
  // old offset; nonzero words normalize header negative and footer positive.
  internal static float HeaderInput(float value,float retained){uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(value),0);if(bits==0)return retained;return BitConverter.ToSingle(BitConverter.GetBytes(bits|0x80000000u),0);}
  internal static float FooterInput(float value,float retained){uint bits=BitConverter.ToUInt32(BitConverter.GetBytes(value),0);if(bits==0)return retained;return BitConverter.ToSingle(BitConverter.GetBytes(bits&0x7fffffffu),0);}
  public void Write(string file){Write(file,false);}
  internal void Write(string file,bool scor4){
   var text=new System.Text.StringBuilder();
   Action<string> line=value=>text.Append(value).Append("\r\n");
   string output=Output;
   if(scor4&&output.Equals("Laser",StringComparison.OrdinalIgnoreCase))output="";
   line("OUTPUT  "+output.PadRight(24,scor4&&output.Length==0?'\0':' '));
   line("ROTATE        "+Flag(Rotate).PadRight(3));
   line("SIZE      "+DecimalField(Size,3,5));
   string paper=PaperWidth==8.5f&&PaperHeight==11?"LTR":PaperWidth==8.5f&&PaperHeight==14?"LEG":
    PaperWidth==11&&PaperHeight==17?"LDG":PaperWidth.ToString("0.00",Culture)+"  "+PaperHeight.ToString("0.00",Culture);
   line("PAPER         "+paper);
   line("OFFSETS    "+DecimalField(Left/4000.0,2,4)+"   "+DecimalField((configuredBottom??Bottom)/4000.0,2,4));
   line("FREEZE        "+Flag(Freeze).PadRight(3));
   line("RESOLUTION    "+Resolution.ToString(Culture));
   line("LINE_width     "+LineWidth.ToString(Culture));
   line("HEADER/FOOTER "+Header.PadRight(6));
   if(!Header.Equals("None",StringComparison.OrdinalIgnoreCase)){
    line(Project.PadRight(18));
    if(!Header.Equals("Footer",StringComparison.OrdinalIgnoreCase))line(DecimalField(HeaderOffset,3,7));
    if(!Header.Equals("Header",StringComparison.OrdinalIgnoreCase))line(DecimalField(FooterOffset,3,7));
   }
   line("MIRROR        "+Flag(Mirror).PadRight(3));
   line("SETSTROKE     "+Flag(Stroke).PadRight(3));
   line("MANUAL_feed   "+Flag(Manual).PadRight(3));
   line("SETPAPERTRAY  "+Tray.ToString(Culture));
   line("DESC  "+Desc.ToString(Culture));
   ScoreFileStorage.Write(file,System.Text.Encoding.GetEncoding(437).GetBytes(text.ToString()));
  }
  static string DecimalField(double value,int places,int width){
   return value.ToString("."+new string('0',places),Culture).PadLeft(width);
  }
  static string Flag(bool state){return state?"Yes":"No";}
  // Original2587:0094..014f: retained x87 products, separate truncations.
  public float Scale{get{return F((double)Size*.017999999225139618);}}
  public float GeometryUnit{get{return F(4000.0/Resolution+.009999999776482582);}}
  public float Unit{get{return F((4000.0/Resolution+.009999999776482582)/Size);}}
  public float Width{get{return F((double)Unit*LineWidth);}}
  public int LeftMargin{get{return checked((int)Math.Truncate(4000.0/Size*F(Left/4000.0))+(int)Math.Truncate(100.0/Size));}}
  public int BottomMargin{get{return checked((int)Math.Truncate(4000.0/Size*F(Bottom/4000.0)+24000)+(int)Math.Truncate(250.0/Size));}}
  public void Resolve(Canvas canvas){if(Freeze)return;
   if(!configuredBottom.HasValue||OffsetWarning==null&&!automaticResolved)configuredBottom=Bottom;
   automaticResolved=false;AutomaticMinimum=null;OffsetWarning=null;ResolvedExtents=null;
   try{
    ResolvedExtents=NativePrintMinimum.Extents(canvas);
    short minimum=ResolvedExtents.Bottom;
    AutomaticMinimum=minimum;
    Bottom=RecoveredFragments.AutomaticPrintOffset.SelectBottom(RecoveredFragments.AutomaticPrintOffset.FromMinimumWord(minimum));
   }catch(NotSupportedException error){
    if(!NativeApproximation.Enabled)throw;
    Bottom=configuredBottom.Value;
    OffsetWarning="Automatic print offsets unavailable. Using the configured bottom offset. "+error.Message;
   }
   automaticResolved=true;}
  internal void ResolveScorlas(Canvas canvas){
   // SCORLAS retains the configured offsets; in particular rotation's
   // 3999-unit bottom candidate survives (measured emitted bmar27185).
   // SCORE's standalone automatic minimum selector is a different caller.
   var extent=NativePrintMinimum.Extents(canvas);
   configuredBottom=Bottom;ResolvedExtents=extent;AutomaticMinimum=extent.Bottom;OffsetWarning=null;automaticResolved=true;
  }
  public void ValidateOutput(){if(!Freeze&&!automaticResolved)throw new InvalidOperationException("Resolve the original vector minimum before automatic print output.");if(Tray<0||Tray>9||Desc<0||Desc>1)throw new NotSupportedException("SCORE printer tray must be 0–9 and DESC must be 0 or 1.");}
  internal string DeviceProgram(string drawingBox=null){
   string setup="";
   if(Tray!=0)setup+="statusdict /setpapertray known {statusdict begin "+Tray+" setpapertray end} if\n";
   bool automaticLedger=false;
   // Original2DAB:049B..0508 compares the rounded physical maxima before
   // the DSC +1 padding. It changes this output request, not F6 settings.
   if(PaperWidth==8.5f&&PaperHeight==11&&drawingBox!=null){var edges=drawingBox.Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);if(edges.Length==4)automaticLedger=Int32.Parse(edges[2],Culture)-1>=594||Int32.Parse(edges[3],Culture)-1>=774;}
   if(automaticLedger||PaperWidth!=8.5f||PaperHeight!=11)
    setup+="statusdict /setpage known {statusdict begin "+(automaticLedger?"792":Math.Round(PaperWidth*72).ToString(Culture))+" "+(automaticLedger?"1224":Math.Round(PaperHeight*72).ToString(Culture))+" 1 setpage end} if\n";
   if(Manual)setup+="statusdict /manualfeed true put\n";
   return setup;
  }
  public IDisposable Begin(){ValidateOutput();previous=Active;Active=this;return this;}
  public void Dispose(){Active=previous;}
  public System.Drawing.Drawing2D.Matrix CanvasMatrix(){ValidateOutput();double scale=Double.Parse(Scale.ToString("F5",Culture),Culture);float coefficient=F(30*scale),horizontal=Mirror?-coefficient:coefficient;double rawOriginX=Mirror?32000:-2100;if(Rotate)return new System.Drawing.Drawing2D.Matrix(0,-horizontal,coefficient,0,F(612-scale*(27250+BottomMargin)),F(PaperHeight*72.0+6-scale*(LeftMargin+rawOriginX)));return new System.Drawing.Drawing2D.Matrix(horizontal,0,0,coefficient,F(scale*(LeftMargin+rawOriginX)),F(PaperHeight*72.0-scale*(27250+BottomMargin)));}
  internal string RawProgram(bool mirrorNotation=true){return (Rotate?"612 -6 translate 90 rotate\n":"")+Scale.ToString("F5",Culture)+" dup scale "+LeftMargin+" "+BottomMargin+" translate\n"+(Mirror&&mirrorNotation?"29900 0 translate -1 1 scale\n":"");}
  internal string IncludedProgramPhysicalSpace(float pageWidth,float pageHeight){
   // Undo the actual Windows notation matrix, expressed in PostScript's
   // bottom-left page coordinates. Included programs establish their own CTM.
   using(var matrix=CanvasMatrix()){
    var e=matrix.Elements;
    double x=pageWidth/612.0,y=pageHeight/792.0;
    double a=e[0]*x,b=-e[1]*x,c=-e[2]*y,d=e[3]*y;
    double tx=e[2]*(double)pageHeight+e[4];
    double ty=PaperHeight*72.0-e[3]*(double)pageHeight-e[5];
    double determinant=a*d-b*c;
    var inverse=new[]{d/determinant,-b/determinant,-c/determinant,a/determinant,
     (c*ty-d*tx)/determinant,(b*tx-a*ty)/determinant};
    return "["+String.Join(" ",Array.ConvertAll(inverse,value=>value.ToString("R",Culture)))+"] concat\n";
   }
  }
  public string DeltaProgram(){return RawProgram()+"-2100 -27250 translate 55.55555555555556 dup scale\n";}
 }
}
