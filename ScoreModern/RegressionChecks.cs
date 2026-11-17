using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace ScoreModern {
public static class RegressionChecks {
    static void Assert(bool condition,string message){if(!condition)throw new Exception(message);}
    public static int Library(string root,string output) {
        try {
            Directory.CreateDirectory(output);int count=0;var failures=new List<string>();
            foreach(string path in Directory.GetFiles(Path.Combine(root,"LIB"),"*.DRW")) {
                try {var glyphs=DrawingLibrary.Load(path);Assert(glyphs.Length==10,"Expected ten glyph slots.");using(var bitmap=new Bitmap(200,200))using(var graphics=Graphics.FromImage(bitmap))foreach(var glyph in glyphs){graphics.Clear(Color.White);glyph.Draw(graphics,80,100,.1f,.1f,0,false);}count+=glyphs.Length;}
                catch(InvalidDataException e) {failures.Add(Path.GetFileName(path)+": "+e.Message);}
            }
            var lib=new DrawingLibrary(root);Assert(lib.Symbols.ContainsKey(0)&&lib.Symbols.ContainsKey(53)&&lib.Symbols.ContainsKey(900),"Core library mappings missing.");
            Assert(lib.Symbols[0].Name=="treb"&&lib.Symbols[53].Name=="p","Library number-to-file mapping differs.");
            Assert(Math.Abs(lib.Symbols[11].Bounds.Width-68)<.01,"Notehead coordinate decoding differs.");
            using(var bitmap=new Bitmap(150,150))using(var graphics=Graphics.FromImage(bitmap)) {
                graphics.Clear(Color.White);lib.Symbols[11].Draw(graphics,20,80,1,1,0,false);Assert(bitmap.GetPixel(54,80).R<100,"Black notehead should be filled.");
                graphics.Clear(Color.White);lib.Symbols[12].Draw(graphics,20,80,1,1,0,false);Assert(bitmap.GetPixel(54,80).R>240,"Half-note hole should remain white.");
            }
            byte[] broken=File.ReadAllBytes(Path.Combine(root,"LIB","LIBRA.DRW"));broken[74]=0;bool rejected=false;try{FortranRecords.Read(broken);}catch(InvalidDataException){rejected=true;}Assert(rejected,"Damaged Fortran framing was accepted.");
            using(var bitmap=new Bitmap(1200,1000))using(var graphics=Graphics.FromImage(bitmap)) {
                graphics.Clear(Color.White);graphics.SmoothingMode=SmoothingMode.AntiAlias;
                for(int n=0;n<100;n++) {Glyph glyph;if(!lib.Symbols.TryGetValue(n,out glyph)||glyph.Points.Length==0)continue;RectangleF bounds=glyph.Bounds;int col=n%10,row=n/10;float scale=Math.Min(80/Math.Max(1,bounds.Width),60/Math.Max(1,bounds.Height));glyph.Draw(graphics,col*120+12-bounds.X*scale,row*100+8+(bounds.Y+bounds.Height)*scale,scale,scale,0,false);using(var font=new Font("Segoe UI",10))graphics.DrawString(n+" "+glyph.Name,font,Brushes.DimGray,col*120+8,row*100+76);}
                bitmap.Save(Path.Combine(output,"symbol-preview.png"),System.Drawing.Imaging.ImageFormat.Png);
            }
            Console.WriteLine("PASS: "+count+" glyph slots decoded and rendered across all valid .DRW files.");Console.WriteLine("PASS: "+lib.Symbols.Count+" music symbol slots mapped; "+lib.Symbols.Values.Count(g=>g.Points.Length>0)+" contain drawings.");Console.WriteLine("PASS: black-note fill, half-note hole, core mappings, dimensions, and damaged framing checks.");
            foreach(string failure in failures)Console.WriteLine("SOURCE FILE REJECTED: "+failure);
            Assert(failures.All(f=>f.StartsWith("LIBUO.DRW:")),"Unexpected drawing-library failures.");
            return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
    public static int Text(string root) {
        try {
            var layout=new ScoreText(root);var score=Score.Load(Path.Combine(root,"MUSDAT","PRACTICE.MUS"));var original=score.Clone();var item=score.Items.First(i=>i.Code==16);byte[] before=(byte[])item.Bytes.Clone();
            layout.Set(item,"_01Updated title",1);Assert(item.Text=="_01Updated title","Text encoding differs.");Assert(item.P(12)==16,"Text length metadata differs.");Assert(item.P(13)>0,"Text width metadata missing.");
            Assert(before.Skip(4).Take(44).SequenceEqual(item.Bytes.Skip(4).Take(44)),"Text edit altered presentation parameters.");
            var accented=item.Clone();
            layout.Set(accented,"_00Café E\u0301lève façade über niño",1);
            Assert(accented.Text=="_00Caf<<e <<El>>eve fa##cade %%uber ni~no","Unicode accents did not map to SCORE controls.");
            var accents=layout.PrepareLayout(accented,accented.Text,25,0);
            Assert(accents.MetricsSupported&&accents.VectorSupported&&accents.Tokens.Where(t=>t.GlyphName!=null)
                .Select(t=>t.GlyphName).SequenceEqual(new[]{"eacute","Eacute","egrave","ccedilla","udieresis","ntilde"}),
                "Accented text did not select the expected Type 1 glyphs.");
            Assert(accented.P(13)==layout.Measure(accented,accented.Text,1),"Stored accent controls and input measure differently.");
            var accentedScore=score.Clone();
            accentedScore.Items[score.Items.IndexOf(item)]=accented;
            string accentFile=Path.GetTempFileName();
            try {
                File.WriteAllBytes(accentFile,accentedScore.Encode());
                Assert(Score.Load(accentFile).Items[score.Items.IndexOf(item)].Bytes.SequenceEqual(accented.Bytes),
                    "Accented text bytes did not survive save and reopen.");
            } finally { File.Delete(accentFile); }
            string temp=Path.GetTempFileName();try{File.WriteAllBytes(temp,score.Encode());var loaded=Score.Load(temp);Assert(loaded.Items.First(i=>i.Code==16).Text==item.Text,"Edited text did not survive a score round-trip.");for(int n=0;n<score.Items.Count;n++)if(score.Items[n].Code!=16)Assert(original.Items[n].Bytes.SequenceEqual(loaded.Items[n].Bytes),"Text edit changed another object.");}finally{File.Delete(temp);}
            byte[] valid=(byte[])item.Bytes.Clone();bool rejected=false;try{layout.Set(item,"𝄞",1);}catch(System.Text.EncoderFallbackException){rejected=true;}Assert(rejected&&valid.SequenceEqual(item.Bytes),"Unrepresentable text should be rejected before mutation.");
            // Original captures verify160 stored bytes; the old151-byte rejection
            // encoded an incorrect150-byte assumption.
            layout.Set(item,"_00"+new string('x',157),1);Assert(item.P(12)==160&&item.Text.Length==160,"Verified160-byte stored text was rejected or truncated.");valid=(byte[])item.Bytes.Clone();
            rejected=false;try{layout.Set(item,new string('x',161),1);}catch(InvalidDataException){rejected=true;}Assert(rejected&&valid.SequenceEqual(item.Bytes),"Oversize text should be rejected before mutation.");
            var orch=Score.Load(Path.Combine(root,"MUSDAT","ORCH01A.MUS"));Item title=orch.Items.First(i=>i.Code==16&&i.Text.Contains("QUINTET"));float expected=layout.Measure(title,title.Text,.6f);Assert(Math.Abs(expected-title.P(13))<.1,"Recovered font advances differ from the source title metadata.");Assert(Math.Abs(layout.Measure(title,title.Text,.3f)*2-expected)<.001,"Text does not scale with its staff.");
            Console.WriteLine("PASS: editable text metadata, presentation preservation, save/reload, other-object preservation, invalid text rejection, font metrics and staff scaling.");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}
}
