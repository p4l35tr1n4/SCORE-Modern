using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScoreModern {
// DOS Fortran sequential records: 4B file marker, framed byte records,
// 81 for a continuing 128-byte segment, 82 end-of-file.
public static class FortranRecords {
    public static byte[] Write(params byte[][] records) {
        using(var output=new MemoryStream()) {
            output.WriteByte(0x4b);
            foreach(byte[] record in records) {
                for(int offset=0;;offset+=128) {
                    int remaining=record.Length-offset;
                    byte marker=(byte)(remaining>128?0x81:remaining);
                    int length=Math.Min(128,remaining);
                    output.WriteByte(marker);
                    output.Write(record,offset,length);
                    output.WriteByte(marker);
                    if(remaining<=128)break;
                }
            }
            output.WriteByte(0x82);
            return output.ToArray();
        }
    }
    public static List<byte[]> Read(byte[] bytes) {return Read(bytes,false);}
    public static List<byte[]> Read(byte[] bytes,bool allowDrawingEndAtRecordBoundary) {
        if(bytes.Length<3||bytes[0]!=0x4b)throw new InvalidDataException("Missing drawing-library signature.");
        var records=new List<byte[]>(); int p=1;
        using(var record=new MemoryStream()) {
            while(p<bytes.Length) {
                int marker=bytes[p++];
                if(marker==0x82) {if(record.Length!=0||p!=bytes.Length)throw new InvalidDataException("Invalid drawing-library end marker.");return records;}
                if(marker>0x81)throw new InvalidDataException("Invalid Fortran segment marker.");
                int length=Math.Min(marker,128);
                if(p+length>=bytes.Length||bytes[p+length]!=marker)throw new InvalidDataException("Truncated or mismatched Fortran segment.");
                record.Write(bytes,p,length);p+=length+1;
                if(marker!=0x81) {records.Add(record.ToArray());record.SetLength(0);}
            }
        // The supplied LIBUO file ends after its second complete framed record.
        // Original SCORE's six actual glyphs from that file were verified in EPS.
        // This exception applies only to the drawing loader's two-record format.
        if(allowDrawingEndAtRecordBoundary&&record.Length==0&&records.Count==2)return records;
        }
        throw new InvalidDataException("Drawing library has no end marker.");
    }
}
public sealed class Glyph {
    public string Source; public string Name; public PointF[] Points; public short[] Operations;
    public RectangleF Bounds {get {if(Points.Length==0)return RectangleF.Empty;float x=Points.Min(p=>p.X),y=Points.Min(p=>p.Y);return new RectangleF(x,y,Points.Max(p=>p.X)-x,Points.Max(p=>p.Y)-y);}}
    public void Draw(Graphics g,float x,float y,float scaleX,float scaleY,float rotation,bool outline) { Draw(g,x,y,scaleX,scaleY,rotation,outline,Color.FromArgb(30,32,37)); }
    internal void Draw(Graphics g,float x,float y,float scaleX,float scaleY,float rotation,bool outline,Color ink) {
        if(Points.Length==0)return;
        var state=g.Save();
        try {
            g.TranslateTransform(x,y);g.RotateTransform(rotation);g.ScaleTransform(scaleX,-scaleY);
            using(var pen=new Pen(ink,Math.Max(.4f,.75f/Math.Max(.01f,Math.Abs(scaleX)))))
            using(var fill=new GraphicsPath(FillMode.Alternate)) {
                var contour=new List<PointF>(); int kind=0;
                Action flushContour=()=>{if(contour.Count>1) {if(kind==1)g.VectorDrawLines(pen,contour.ToArray());else if(contour.Count>2)fill.AddPolygon(contour.ToArray());}contour.Clear();};
                Action flushFill=()=>{if(fill.PointCount>0){if(outline)g.VectorDrawPath(pen,fill);else g.VectorFillPath(Brushes.Black,fill);fill.Reset();}};
                for(int n=0;n<Points.Length;n++) {
                    int op=Operations[n];
                    // Negative opcodes in the supplied drawings mark points within
                    // a contour. Their smoothing hints are not yet reconstructed.
                    if(op>0) {flushContour();if(op!=3)flushFill();kind=op==1?1:2;}
                    contour.Add(Points[n]);
                }
                flushContour();flushFill();
            }
        } finally {g.Restore(state);}
    }
}
public sealed class DrawingLibrary {
    public readonly string Root;
    public readonly string Directory;
    public readonly Dictionary<int,Glyph> Symbols=new Dictionary<int,Glyph>();
    public readonly List<string> Warnings=new List<string>();
    public DrawingLibrary(string root) {
        Root=Path.GetFullPath(root);
        string dir=NativeLibraryLocation.DirectoryFor(root,true);Directory=Path.GetFullPath(dir);
        var files=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
        string original=Path.Combine(root,"LIB");
        foreach(string folder in new[]{original,dir}.Distinct(StringComparer.OrdinalIgnoreCase)){
            if(!System.IO.Directory.Exists(folder)){Warnings.Add("Drawing folder not found: "+folder);continue;}
            foreach(string file in System.IO.Directory.GetFiles(folder,"LIB??.DRW"))files[Path.GetFileName(file)]=file;
        }
        foreach(string path in files.Values.OrderBy(p=>Path.GetFileName(p))) {
            string name=Path.GetFileNameWithoutExtension(path);
            int first;if(!TryFirstSymbol(path,out first))continue;
            try {var glyphs=Load(path);for(int i=0;i<10;i++)Symbols[first+i]=glyphs[i];}catch(InvalidDataException e){Warnings.Add(name+": "+e.Message);}
        }
    }
    internal static bool TryFirstSymbol(string path,out int first){
        string name=Path.GetFileNameWithoutExtension(path).ToUpperInvariant();
        first=-1;
        if(name.Length!=5||!name.StartsWith("LIB",StringComparison.Ordinal)||name[3]<'R'||name[3]>'Z'||name[4]<'A'||name[4]>'Z')return false;
        first=((name[3]-'R')*26+name[4]-'A')*10;return true;
    }
    public static Glyph[] Load(string path) {
        var records=FortranRecords.Read(File.ReadAllBytes(path),true);if(records.Count!=2||records[0].Length!=72||records[1].Length%6!=0)throw new InvalidDataException("Unsupported drawing records.");
        byte[] header=records[0],data=records[1];var glyphs=new Glyph[10];
        if(BitConverter.ToUInt16(header,0)!=1||BitConverter.ToUInt16(header,20)!=data.Length/2+1)throw new InvalidDataException("Invalid glyph directory.");
        for(int i=0;i<10;i++) {
            string name=Encoding.GetEncoding(437).GetString(header,22+i*5,5).Trim();
            // Unused directory slots have blank names and may contain stale
            // pointers. They are not drawings, even when those pointers look valid.
            if(name.Length==0){glyphs[i]=new Glyph{Source=Path.GetFullPath(path),Name="",Points=new PointF[0],Operations=new short[0]};continue;}
            int a=BitConverter.ToUInt16(header,i*2)-1,b=BitConverter.ToUInt16(header,i*2+2)-1;
            if(a<0||b<a||b*2>data.Length||(b-a)%3!=0)throw new InvalidDataException("Invalid glyph offsets.");
            int count=(b-a)/3;var glyph=new Glyph{Source=Path.GetFullPath(path),Name=name,Points=new PointF[count],Operations=new short[count]};
            for(int j=0;j<count;j++) {int p=(a+j*3)*2;glyph.Points[j]=new PointF(BitConverter.ToInt16(data,p),BitConverter.ToInt16(data,p+2));glyph.Operations[j]=BitConverter.ToInt16(data,p+4);if(glyph.Operations[j]<-3||glyph.Operations[j]>3)throw new InvalidDataException("Unknown drawing opcode.");}
            glyphs[i]=glyph;
        }
        return glyphs;
    }
}
public sealed class SymbolPicker : ScoreDialog {
    public int SymbolNumber=-1;
    public SymbolPicker(DrawingLibrary library) {
        Text="SCORE symbol library";Width=800;Height=650;StartPosition=FormStartPosition.CenterParent;Font=new Font("Segoe UI",10);
        var search=new TextBox{Dock=DockStyle.Top};var list=new ListBox{Dock=DockStyle.Fill,DrawMode=DrawMode.OwnerDrawFixed,ItemHeight=66,IntegralHeight=false,MultiColumn=true,ColumnWidth=180,BorderStyle=BorderStyle.None};
        var insert=new Button{Text="Insert selected symbol",Dock=DockStyle.Bottom,Height=40};
        Action filter=()=>{list.BeginUpdate();list.Items.Clear();foreach(var pair in library.Symbols.OrderBy(p=>p.Key))if(pair.Value.Points.Length>0&&(pair.Key+" "+pair.Value.Name).IndexOf(search.Text,StringComparison.OrdinalIgnoreCase)>=0)list.Items.Add(pair.Key);list.EndUpdate();};
        Load+=(a,b)=>{using(var graphics=CreateGraphics()){float dpi=graphics.DpiX/96f;list.ItemHeight=(int)Math.Ceiling(66*dpi);list.ColumnWidth=(int)Math.Ceiling(180*dpi);}};
        search.TextChanged+=(a,b)=>filter();
        list.DrawItem+=(a,e)=>{if(e.Index<0)return;e.DrawBackground();int number=(int)list.Items[e.Index];Glyph glyph=library.Symbols[number];RectangleF b=glyph.Bounds;float dpi=e.Graphics.DpiX/96f;float scale=Math.Min(65*dpi/Math.Max(1,b.Width),43*dpi/Math.Max(1,b.Height));var state=e.Graphics.Save();e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;glyph.Draw(e.Graphics,e.Bounds.X+10*dpi-b.X*scale,e.Bounds.Y+8*dpi+(b.Y+b.Height)*scale,scale,scale,0,false);e.Graphics.Restore(state);using(var f=new Font("Segoe UI",10))e.Graphics.DrawString(number+"\n"+glyph.Name,f,Brushes.DimGray,e.Bounds.X+95*dpi,e.Bounds.Y+10*dpi);e.DrawFocusRectangle();};
        var warnings=new Label{Dock=DockStyle.Bottom,Height=library.Warnings.Count>0?55:25,ForeColor=Color.DimGray,Text=library.Symbols.Values.Count(g=>g.Points.Length>0)+" drawings available"+(library.Warnings.Count>0?"\nIgnored source files: "+String.Join("; ",library.Warnings):"")};
        Action choose=()=>{if(list.SelectedItem==null)return;SymbolNumber=(int)list.SelectedItem;DialogResult=DialogResult.OK;Close();};insert.Click+=(a,b)=>choose();list.DoubleClick+=(a,b)=>choose();Controls.Add(list);Controls.Add(search);Controls.Add(insert);Controls.Add(warnings);filter();
    }
}
}


