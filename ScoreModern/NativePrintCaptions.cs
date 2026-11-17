using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;

namespace ScoreModern {
 internal sealed class NativePrintCaptions {
  sealed class Cell {
   internal float X,Y,Width;
   internal string Text,Font;
  }
  readonly List<Cell> cells=new List<Cell>();
  readonly List<float> tops=new List<float>();
  static readonly float[] dividers={211,327,401,450};

  internal NativePrintCaptions(NativePrintJob job,Score score):this(job,score,null,false){}
  internal NativePrintCaptions(NativePrintJob job,Score score,string displayedName,bool scorlas){
   if(job.Header.Equals("None",StringComparison.OrdinalIgnoreCase))return;
   var source=score.SourcePath;
   string name=displayedName??(String.IsNullOrEmpty(source)?"":Path.GetFileName(source).ToUpperInvariant());
   DateTime? created=String.IsNullOrEmpty(source)||!File.Exists(source)?(DateTime?)null:File.GetLastWriteTime(source);
   var culture=CultureInfo.InvariantCulture;
   string[] values={name,job.Project,created.HasValue?created.Value.ToString("MM-dd-yy",culture):"",
    created.HasValue?created.Value.ToString("HH:mm",culture):"",
    Number(job.Size)+" "+Number(job.Left/4000.0)+" "+Number(job.Bottom/4000.0)+"  "+job.Resolution+" "+job.LineWidth};
   if(scorlas){
    // SCORLAS0AE1:15B4..1659 decodes the DOS stamp, adds80 to
    // its year, then splits each date/time field with signed IDIV10.
    // Format077E prints each digit in one column; an overflowing tens
    // word becomes '*'. Preserve that original post-1999 behavior.
    values[0]=name.PadRight(24);values[1]=job.Project.PadRight(18);
    if(created.HasValue){var stamp=created.Value;if(stamp.Year<1980||stamp.Year>2107)throw new NotSupportedException("SCORLAS caption date is outside the DOS timestamp domain.");values[2]=Digits(stamp.Month)+"-"+Digits(stamp.Day)+"-"+Digits(stamp.Year-1900);values[3]=Digits(stamp.Hour)+":"+Digits(stamp.Minute);}
    // Format07A0: F4.2,2F6.2,I5,I2; each width is independent.
    float left=job.Left/4000f,bottom=job.Bottom/4000f;bool metric=NativeMeasurements.Get(score)==1;
    if(metric){left=(float)((double)left*2.54f);bottom=(float)((double)bottom*2.54f);}
    values[4]=Field(job.Size,4)+Field(left,6,metric?1:2)+Field(bottom,6,metric?1:2)+job.Resolution.ToString(culture).PadLeft(5)+job.LineWidth.ToString(culture).PadLeft(2);
   }
   if(!job.Header.Equals("Footer",StringComparison.OrdinalIgnoreCase))
    Add((float)Math.Round(72*(job.PaperHeight+job.HeaderOffset)),values);
   if(!job.Header.Equals("Header",StringComparison.OrdinalIgnoreCase))
    Add((float)Math.Round(72*job.FooterOffset)+12,values);
  }
  static string Digits(int value){int tens=value/10,ones=value%10;return (tens>9?"*":tens.ToString(CultureInfo.InvariantCulture))+ones.ToString(CultureInfo.InvariantCulture);}
  static string Field(float value,int width,int places=2){string text=RecoveredFragments.SingleFixedDecimal.Format(value,places);if(text.StartsWith("0."))text=text.Substring(1);else if(text.StartsWith("-0."))text="-"+text.Substring(2);return text.Length>width?new string('*',width):text.PadLeft(width);}
  static string Number(double value){return value.ToString(".00",CultureInfo.InvariantCulture);}
  void Add(float top,string[] values){
   tops.Add(top);
   string[] labels={"SCORE File:","Project:","File Date:","Time:","Print data:"};
   float[] lx={22,213,329,403,453},vx={69,241,363,425,490};
   for(int n=0;n<labels.Length;n++){
    cells.Add(new Cell{X=lx[n],Y=top-9,Width=8.5f,Text=labels[n],Font="Times-Roman"});
    cells.Add(new Cell{X=vx[n],Y=top-9,Width=9,Text=values[n],Font="Helvetica-Bold"});
   }
  }
  internal string PostScript(){
   var result=new StringBuilder();
   result.AppendLine("gsave 0 setgray .1 setlinewidth");
   foreach(float top in tops){
    result.AppendLine("newpath 20 "+N(top)+" moveto 590 "+N(top)+" lineto 590 "+N(top-11)+" lineto 20 "+N(top-11)+" lineto closepath");
    foreach(float x in dividers)result.AppendLine(N(x)+" "+N(top)+" moveto "+N(x)+" "+N(top-11)+" lineto");
    result.AppendLine("stroke");
   }
   foreach(var cell in cells){
    string hex=BitConverter.ToString(Encoding.GetEncoding(1252).GetBytes(cell.Text)).Replace("-","");
    result.AppendLine("/"+cell.Font+" findfont ["+N(cell.Width)+" 0 0 9 0 0] makefont setfont "+N(cell.X)+" "+N(cell.Y)+" moveto <"+hex+"> show");
   }
   return result.AppendLine("grestore").ToString();
  }
  internal void Draw(Graphics graphics,float pageHeight){
   if(cells.Count==0)return;
   var fonts=new Dictionary<string,Type1Outline>();
   foreach(var cell in cells){
    if(fonts.ContainsKey(cell.Font))continue;
    var font=NativeTupletFonts.Find(cell.Font);
    if(font==null)throw new FileNotFoundException("Print captions require the external "+cell.Font+" Type 1 font.");
    fonts.Add(cell.Font,font);
   }
   using(var pen=new Pen(Color.Black,.1f)){
    foreach(float top in tops){
     float y=pageHeight-top;
     graphics.DrawRectangle(pen,20,y,570,11);
     foreach(float x in dividers)graphics.DrawLine(pen,x,y,x,y+11);
    }
   }
   foreach(var cell in cells){
    float x=cell.X;
    foreach(char character in cell.Text){
     float advance;
     using(var path=fonts[cell.Font].Character(character,out advance)){
      if(path.PointCount!=0){
       using(var transform=new Matrix(cell.Width/1000,0,0,-9f/1000,x,pageHeight-cell.Y))
        path.Transform(transform);
       graphics.FillPath(Brushes.Black,path);
      }
     }
     x+=advance*cell.Width/1000;
    }
   }
  }
  static string N(float value){return value.ToString("0.####",CultureInfo.InvariantCulture);}
 }
}
