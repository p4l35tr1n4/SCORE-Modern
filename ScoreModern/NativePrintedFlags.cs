using System;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using ScoreModern.RecoveredFragments;
namespace ScoreModern {
 public static class NativePrintedFlags {
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache=null){
   if(NativeDatDrawing.Flags(g,item,page,library))return true;
   if(page==null||library==null||item.Code!=1||(int)item.P(9)%10<1||item.P(9)<0||item.P(9)>=2000)return false;
   var note=NativeNoteDetails.Prepare(item,page);if(note==null||(note.StemKind!=1&&note.StemKind!=2))return false;
   int head=(int)item.P(6);if(NativePrintedHeads.Parenthesized(item))head-=10;
   if(head < -1 || head>9&&head<20 || head>=3000)return false;
   Glyph glyph;if(!library.Symbols.TryGetValue(20,out glyph))return false;
   bool up=note.StemKind==1;int count=(int)item.P(9)%10;float scale=note.Scale;float headWidth=note.HeadWidth,increment=(float)((up?7.0:-7.0)*scale),x=up?(float)((double)note.NoteX+headWidth):note.NoteX,y=(float)((double)(item.P(8)%100)*increment+note.NoteY+increment);if(count>1)y=(float)((double)y-increment/2.0);int vertical=up?-1:1;var eps=NativeEpsExport.For(g);
   RenderResourceAudit.Record("printed flag","20:"+glyph.Name,glyph.Source,"measured black up/down flags, cue scale and corpus stem-extension controls; other note components verified separately");
   int firstFlag=0;
   // Original N127 P00 requests: a cached down-flag uses the shared
   // primitive procedure for the first flag, with remaining flags expanded.
   if(!up&&cache!=null&&cache.Contains(20)){
    float relative=(float)((double)scale/page.Scale);
    if(!NativePrimitiveGlyphs.Draw(g,page,library,cache,new PrimitiveGlyphRequest{Glyph=20,X=x,Y=y,Horizontal=relative,Vertical=relative,WidthSelector=-1}))return false;
    firstFlag=1;y=(float)((double)y+increment+increment);
   }
   var points=new List<BarlinePoint>();
   for(int flag=firstFlag;flag<count;flag++){
    for(int n=0;n<glyph.Points.Length;n++){
     int op=glyph.Operations[n];if(flag>0&&n>0&&op>0&&op!=3)break;
     points.Add(new BarlinePoint(NativeNotationContext.Word((float)((double)glyph.Points[n].X*scale/4+x),true),NativeNotationContext.Word((float)((double)glyph.Points[n].Y*scale/4*vertical+y),false),op));
    }
    y=(float)((double)y+increment+increment);
   }
   if(points.Count>0)NativeNoteDetails.Path(g,page,points,NativePrintContext.Width(-1),true);
   return true;
  }
 }
}

