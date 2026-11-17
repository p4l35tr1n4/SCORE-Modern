using System;using System.Collections.Generic;using System.Drawing;using System.Drawing.Drawing2D;using System.Globalization;
namespace ScoreModern {
 public static class NativePrintedTextHeads {
  static float F(double value){return (float)value;}
  sealed class RotationScope:IDisposable{
   readonly Graphics graphics;readonly GraphicsState saved;readonly IDisposable eps;
   internal RotationScope(Graphics g,NativeNotationContext page,PointF pivot,float rotation){graphics=g;saved=g.Save();
    using(var basis=page.Projection.Matrix())using(var inverse=page.Projection.Matrix())using(var turn=new Matrix()){inverse.Invert();turn.Translate(-pivot.X,-pivot.Y,MatrixOrder.Append);turn.Rotate(rotation,MatrixOrder.Append);turn.Translate(pivot.X,pivot.Y,MatrixOrder.Append);inverse.Multiply(turn,MatrixOrder.Append);inverse.Multiply(basis,MatrixOrder.Append);g.MultiplyTransform(inverse,MatrixOrder.Prepend);}
    var output=NativeEpsExport.For(g);if(output!=null)eps=output.PushRawRotation(pivot,rotation);
   }
   public void Dispose(){if(eps!=null)eps.Dispose();graphics.Restore(saved);}
  }
  // N149/N155/N160/N161 cover the forced-font mark route and note controls.

  internal static bool VerifiedFontMark(Item i){return NativeLibrarySymbols.IsTextAlias((int)i.P(6))&&i.P(11)>=1000&&i.P(11)<=1056&&i.P(11)==(int)i.P(11)&&i.P(9)<1000;}

  // The alias already projected its body; attachment preparation starts from
  // the staff context so P15/cross-staff offsets are not applied twice.
  internal static NativeNotationContext AttachmentPage(NativeNotationContext page,float x){var result=new NativeNotationContext{SourceScore=page.SourceScore,StaffLedgerWidthControl=page.StaffLedgerWidthControl,Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,StaffScales=page.StaffScales,Prepared=page.Prepared,Projection=page.Projection};result.Prepared.X=x;return result;}
  public static bool IsAlias(Item i){return i.Code==1&&NativePrintedHeads.Custom(i)&&NativeLibrarySymbols.IsTextAlias((int)i.P(6));}
  public static bool Draw(Graphics g,Item i,NativeNotationContext page,DrawingLibrary library,ScoreText text,HashSet<int> cache){
   if(!IsAlias(i))return false;
   if((i.P(11)!=0&&!VerifiedFontMark(i)))return false;
   var note=NativeNoteDetails.Prepare(i,page);if(note==null)return false;
   using(var buffer=NativePrintedPointBuffer.Begin(g)){
    NativeNoteDetails.Ledgers(g,i,page);
    if(!NativePrintedHeads.Draw(g,i,page,library,cache,text))return false;
    var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,StaffScales=page.StaffScales,Projection=page.Projection,Prepared=page.Prepared};
    context.Prepared.Scale=note.Scale;context.Prepared.X=note.NoteX;context.Prepared.Y=note.NoteY;
    int selector=(int)i.P(6);var symbol=Item.Make(9,i.P(2),i.P(3),i.P(4),selector,selector<50?i.P(6):.8330000042915344f,selector<50?i.P(7):0);
    symbol.Set(11,i.P(11));var body=NativeLibrarySymbols.AliasTextItem(symbol,context,text);float cursor=text.LastPrintedGlyphOrigin(body,body.Text,context);
    using(var rotation=new RotationScope(g,page,new PointF(note.NoteX,note.NoteY),-Single.Parse(i.P(9).ToString("F2",CultureInfo.InvariantCulture),CultureInfo.InvariantCulture))){
    int flags=(int)i.P(9)%10,explicitStem=(int)i.P(5)/10%10;bool up=note.StemKind!=2;float direction=up?1:-1,extension=F((double)i.P(8)-100),extra=flags>1?F((flags-1)*2.0-.5):0;
    var stem=new List<BarlinePoint>();
    if(explicitStem!=0&&flags>0){float dy=F((double)page.Prepared.VerticalInput+direction*F((double)extension+extra)*.6000000238418579),dx=F((double)note.Scale*6);
     stem.Add(new BarlinePoint(F((up?.7829999923706055:-1.8830000162124634)*dx+cursor),NativeNoteDetails.SlashVertical(i,page,F((up?2.799999952316284:-5.5)*.6000000238418579+dy)),1));
     stem.Add(new BarlinePoint(F((up?6.5:3.799999952316284)*dx+cursor),NativeNoteDetails.SlashVertical(i,page,F((up?5.400000095367432:-2.799999952316284)*.6000000238418579+dy)),0));
    }
    if(note.Stem){float x=up?F((double)cursor+note.HeadWidth):cursor;
     stem.Add(new BarlinePoint(x,i.P(17)%100!=0?F((double)note.NoteY+(i.P(17)%100)*(double)note.Scale*7*direction):F((double)note.NoteY+2.0999999046325684*note.Scale*direction),1));
     stem.Add(new BarlinePoint(x,F((double)note.NoteY+F(((double)extra+extension+7)*note.Scale*7)*direction),0));
    }
    if(stem.Count>0){short stemWidth=NativeNoteDetails.PrintedStemSelector(i);NativeNoteDetails.Path(g,page,stem,NativePrintContext.Width(stemWidth),inheritedWidth:stemWidth==0);}
    context.Prepared.X=cursor;
    var post=i.Clone();post.Set(4,i.P(4)%100);post.Set(10,0);post.Set(12,0);post.Set(15,0);post.Set(8,i.P(8)%100);
    if(flags>0&&!NativePrintedFlags.Draw(g,post,context,library,cache))return false;
    post.Set(8,0);post.Set(9,0);if(i.P(10)==10)context.Prepared.X=F((double)cursor-note.HeadWidth);
    if((int)i.P(5)%10!=0&&!NativeNoteAttachments.DrawAccidental(g,post,context,library,cache))return false;
    context.Prepared.X=cursor;
    float fraction=F(i.P(9)%1);if(fraction>.8999999761581421)fraction=F(.8999999761581421-fraction);if((int)i.P(9)>=1000)fraction=F(.2840000092983246+fraction);
    float dotX=F((double)cursor+F(((double)fraction*60+18)*note.Scale));
    for(int dot=0;dot<note.DotCount;dot++){if(!NativePrimitiveGlyphs.Draw(g,context,library,cache,new PrimitiveGlyphRequest{Glyph=24,X=dotX,Y=note.DotY,Horizontal=1,Vertical=1}))return false;dotX=F((double)dotX+note.Scale*13);}
    if(i.P(11)!=0){var finger=i.Clone();finger.Set(5,note.StemKind*10);finger.Set(10,0);var attachmentPage=AttachmentPage(page,cursor);NativeNotationContext fingerContext;var fingerBody=NativeNoteAttachments.Fingering(finger,attachmentPage,text,false,out fingerContext,i.P(15)!=0?note.Scale:page.Scale);if(fingerBody==null)return false;fingerContext.Prepared.X=NativeNotationContext.RawWord(fingerContext.Prepared.X);fingerContext.Prepared.Y=NativeNotationContext.RawWord(fingerContext.Prepared.Y);text.Draw(g,fingerBody,fingerBody.Text,0,0,fingerContext.Scale/5,fingerContext.Scale,fingerContext);}
    buffer.Complete();return true;}
   }
  }
 }
}
