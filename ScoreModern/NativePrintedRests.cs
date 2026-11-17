using System;using System.Collections.Generic;using System.Drawing;
namespace ScoreModern {
 public static class NativePrintedRests {
  static float F(double value){return (float)value;}
  // Placement transcribed by NativeScanMusicVectors from the original shared
  // rest routine, now checked independently against all68 Wandh01 EPS rests.
  // Keep unverified printed auxiliary/cue domains out of this route.
  public static bool Draw(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,ScoreText text=null){
   if(item.Code!=2||page==null||library==null)return false;
   if(item.P(13)!=0||item.P(17)!=0){item=item.Clone();if(item.Count>=13)item.Set(13,0);if(item.Count>=17)item.Set(17,0);}
   int kind=(int)item.P(5);if(item.P(5)!=kind)return false;
   if(NativeDatRestAttachments.Invisible(item,kind)){
    RenderResourceAudit.Record("printed invisible rest",kind.ToString(),"original Code2 visibility branch","no body or attachments requested");return true;
   }
   if((kind==-4&&item.P(6)>=-32768&&item.P(6)<=32767&&item.P(6)!=0)||
      ((kind==-6||kind==-7)&&item.P(6)>0&&item.P(6)<=32767)){
    item=item.Clone();item.Set(6,0);
   }
   if(kind<=-6&&item.P(10)!=0){
    if(Single.IsNaN(item.P(10))||Single.IsInfinity(item.P(10))||Math.Abs(item.P(10))>32767)return false;
    float shift=F((double)item.P(10)*NativeDatRestAttachments.EffectiveScale(item,page)/25);
    var moved=new NativeNotationContext{SourceScore=page.SourceScore,StaffLedgerWidthControl=page.StaffLedgerWidthControl,Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};moved.Prepared.X=F((double)shift*150+page.Prepared.X);
    item=item.Clone();item.Set(3,F((double)item.P(3)+shift));item.Set(10,0);page=moved;
   }
   if(item.P(8)>0&&(kind==-5||kind==-6||kind==-7||kind>=-19&&kind<=-11))return DrawMultiNumber(g,item,page,library,cache,text,kind);
   if(kind<=-6)return DrawComposite(g,item,page,library,cache,text,kind);
   float longRestOrigin;
   if(NativeDatRestAttachments.TryUnnumberedLongOrigin(item,page,out longRestOrigin)){
    float scale=F((double)NativeDatRestAttachments.EffectiveScale(item,page)/page.Scale);
    return NativePrimitiveGlyphs.Draw(g,page,library,null,new PrimitiveGlyphRequest{
     Glyph=38,X=longRestOrigin,Y=page.Vertical(NativeDatRestAttachments.Pitch(item)),Horizontal=scale,Vertical=scale});
   }
   if(kind< -4||kind>10)return false;
   bool invertedFermata=item.P(8)==-2;
   bool fermata=item.P(8)==-1||invertedFermata;
   bool numeral=item.P(8)>0&&item.P(8)<=999&&item.P(8)==(int)item.P(8);
   if(item.P(8)!=0&&!fermata&&!numeral||fermata&&!library.Symbols.ContainsKey(31))return false;
   if(Single.IsNaN(item.P(10))||Single.IsInfinity(item.P(10))||Math.Abs(item.P(10))>32767)return false;
   float size=item.P(12)==0?1:item.P(12);if(fermata&&(Single.IsNaN(size)||Single.IsInfinity(size)||Single.IsNaN(item.P(11))||Single.IsInfinity(item.P(11))))return false;
   if((item.P(15)!=0||Math.Abs(item.P(4))>=100)&&!fermata&&!numeral&&
    (item.P(8)!=0))return false;
   var body=item.Clone();body.Set(8,0);body.Set(10,0);body.Set(11,0);body.Set(12,0);body.Set(13,0);body.Set(14,0);body.Set(16,0);
   if(item.P(9)==-1&&kind<=-2){if(page.SourceScore==null)return false;var plan=NativeRestCentering.PlanItem(page.SourceScore,item);if(plan.Count>0)body.Set(9,plan[0].Position);}
   var context=new NativeNotationContext{SourceScore=page.SourceScore,StaffLedgerWidthControl=page.StaffLedgerWidthControl,Staff=page.Staff,RawBase=page.RawBase,StaffBases=page.StaffBases,Prepared=page.Prepared,Projection=page.Projection};
   if(item.P(10)!=0){float shift=F((double)item.P(10)*NativeDatRestAttachments.EffectiveScale(item,page)/25);context.Prepared.X=F((double)shift*150+context.Prepared.X);}
   if(numeral){
    if(text==null||Single.IsNaN(item.P(6))||item.P(6)<0||item.P(6)>32767||(item.P(9)!=0&&!(kind<=-2&&(item.P(9)>0||item.P(9)==-1))))return false;
    short font=NativeDatRestAttachments.NumberFont(item,library);bool deferred=font>=90&&font<1000;
    // Numeral request precedes dots/body. Cached glyphs and Type1 text
    // print immediately; buffered numeral paths precede other deferred paths.
    var numbered=item.Clone();numbered.Set(9,body.P(9));Action number=()=>DrawNumeral(g,numbered,context,library,text);if(!deferred)number();
    return DrawBody(g,body,context,library,cache,null,false,deferred?number:null);
   }
   if(!fermata)return DrawBody(g,body,context,library,cache,null);
   float bodyScale=NativeDatRestAttachments.EffectiveScale(item,page);
   float factor=F((double)bodyScale*6),offset=kind<0?1.4199999570846558f:kind==0?.949999988079071f:kind==1?1.350000023841858f:1.149999976158142f;
   float origin=body.P(9)>0?F((double)body.P(9)*150):context.Prepared.X,shiftX=F((double)factor*offset),width=F((double)size*factor*1.690000057220459);
   float x=F((double)item.P(16)*150+origin+shiftX-width),pitch=NativeDatRestAttachments.Pitch(item),height=F((invertedFermata?1.0:13.0)+(pitch>0?pitch:0)+item.P(11)),y=context.Vertical(height);
   float attachmentSize=F((double)size*bodyScale/page.Scale);
   var attachment=new PrimitiveGlyphRequest{Glyph=31,X=x,Y=y,Horizontal=attachmentSize,Vertical=invertedFermata?-attachmentSize:attachmentSize,WidthSelector=0};
   return DrawBody(g,body,context,library,cache,attachment,invertedFermata);
  }
  static bool DrawMultiNumber(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,ScoreText text,int kind){
   if(text==null||(kind>=-7&&kind!=-5&&item.P(6)!=0)||Single.IsNaN(item.P(6))||item.P(6)<0||item.P(8)!=Math.Truncate(item.P(8))||item.P(8)>999||item.P(9)<0)return false;
   for(int field=10;field<=20;field++)if(field!=11&&field!=12&&field!=14&&field!=15&&field!=16&&!(field==10&&kind==-5)&&item.P(field)!=0)return false;
   if(NativeDatRestAttachments.CueEuropeanNumber(item))return DrawComposite(g,item,page,library,cache,text,kind);
   NativeNotationContext context;Item label;
   try{label=NativeDatRestAttachments.NumeralText(item,page,library,text,out context);}catch(NotSupportedException){return false;}
   Action number=()=>text.Draw(g,label,label.Text,0,0,page.Scale/5,page.Scale,context);
   if(kind==-5){
    var raw=NativeDatRestAttachments.LongBody(item,page);var points=new List<BarlinePoint>();
    foreach(var q in raw)points.Add(new BarlinePoint(q.X,q.Y,q.Operation));
    NativeNoteDetails.Path(g,page,points,NativePrintContext.DefaultLineWidth);number();return true;
   }
   var body=item.Clone();body.Set(8,0);body.Set(11,0);body.Set(12,0);body.Set(14,0);body.Set(16,0);
   number();return DrawComposite(g,body,page,library,cache,text,kind);
  }
  static bool DrawComposite(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,ScoreText text,int kind){
   bool cueNumber=NativeDatRestAttachments.CueEuropeanNumber(item);
   if((kind>=-7&&item.P(6)!=0)||Single.IsNaN(item.P(6))||item.P(6)<0||item.P(8)!=0&&!cueNumber||
      Single.IsNaN(item.P(9))||Single.IsInfinity(item.P(9))||item.P(9)<0)return false;
   for(int field=10;field<=20;field++)if(field!=15&&!(cueNumber&&(field==11||field==12||field==14||field==16))&&item.P(field)!=0)return false;
   if(Single.IsNaN(item.P(15))||Single.IsInfinity(item.P(15))||Math.Abs(item.P(15))>32767)return false;
   if(kind!=-6&&kind!=-7&&(kind< -19||kind> -11))return false;
   int count=kind==-6?2:kind==-7?4:-kind-10;
   float scale=NativeDatRestAttachments.EffectiveScale(item,page),size=F((double)scale/page.Scale),spacing=F((double)(item.P(6)==0?1:item.P(6))*(item.P(15)!=0?scale:page.Scale));
   var context=new NativeNotationContext{Staff=page.Staff,RawBase=page.RawBase,Projection=page.Projection,Prepared=page.Prepared};
   float origin=item.P(9)>0?F((double)item.P(9)*150):page.Prepared.X;
   var components=new List<Tuple<int,int>>();
   NativeDatRestAttachments.EuropeanComponents(count,(bars,offset)=>components.Add(Tuple.Create(bars,offset)));
   foreach(var part in components)if(!library.Symbols.ContainsKey(part.Item1==1?41:part.Item1==2?116:117))return false;
   foreach(var part in components){
    float x=F((double)origin+part.Item2*spacing);
    if(part.Item1==1){
     context.Prepared.X=x;context.Prepared.Y=page.Prepared.Y;var body=Item.Make(2,item.P(2),item.P(3),item.P(4),-2,0,4);body.Set(15,item.P(15));if(cueNumber){body.Set(8,item.P(8));body.Set(11,item.P(11));body.Set(12,item.P(12));body.Set(14,item.P(14));body.Set(16,item.P(16));DrawNumeral(g,body,context,library,text);foreach(int field in new[]{8,11,12,14,16})body.Set(field,0);}if(!DrawBody(g,body,context,library,cache,null))return false;
    }else{
     context.Prepared.X=x;
     context.Prepared.Y=page.Vertical(F(8.0+(kind>=-7?NativeDatRestAttachments.Pitch(item):0)));
     var symbol=Item.Make(9,item.P(2),item.P(3),0,part.Item1==2?116:117,F((double).8299999833106995f*size));
     if(!NativeLibrarySymbols.Draw(g,symbol,context,library,text,cache))return false;
    }
   }
   return true;
  }
  static void DrawNumeral(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,ScoreText text){
   NativeNotationContext context;var word=NativeDatRestAttachments.NumeralText(item,page,library,text,out context);
   text.Draw(g,word,word.Text,0,0,page.Scale/5,page.Scale,context);
  }

  static bool DrawFermata(Graphics g,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,PrimitiveGlyphRequest attachment){
   RenderResourceAudit.Record("printed rest fermata","31",library.Root,"recovered rest attachment request; external LIB glyph");
   return NativePrimitiveGlyphs.Draw(g,page,library,cache,attachment);
  }
  static bool DrawDots(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,int rest,float origin){
   // The screen guide has no printed counterpart. F6 still draws the
   // ordinary augmentation sequence for the positive P6 word.
   if(item.P(6)<=0)return true;
   float[] offsets={Single.NaN,19,19,19,13,15.5f,18.399999618530273f,22.299999237060547f,26.200000762939453f};
   float scale=NativeDatRestAttachments.EffectiveScale(item,page),glyphSize=F((double)scale/page.Scale),dotX=F((double)origin+offsets[Math.Min(8,rest+4)]*scale);
   float pitch=item.P(15)!=0?item.P(4):item.P(4)%100;
   float dotY=page.Vertical(F((double)pitch+(rest>2?10:8)));
   for(int n=0;n<(int)item.P(6);n++){
    if(!NativePrimitiveGlyphs.Draw(g,page,library,cache,new PrimitiveGlyphRequest{Glyph=24,X=dotX,Y=dotY,Horizontal=glyphSize,Vertical=glyphSize,WidthSelector=0}))return false;
    dotX=F((double)dotX+13*scale);
   }
   return true;
  }
  static bool DrawBody(Graphics g,Item item,NativeNotationContext page,DrawingLibrary library,HashSet<int> cache,PrimitiveGlyphRequest? attachment,bool invertedFermata=false,Action beforeDeferred=null){
   if(item.Code!=2||page==null||library==null||item.P(8)!=0)return false;
   int rest=(int)item.P(5);if(item.P(5)!=rest||rest< -4||rest>10)return false;
   if(Single.IsNaN(item.P(4))||Single.IsInfinity(item.P(4))||Math.Abs(item.P(4))>32767)return false;
   if(Single.IsNaN(item.P(15))||Single.IsInfinity(item.P(15))||Math.Abs(item.P(15))>32767)return false;
   if(item.P(9)!=0&&item.P(9)!=-1&&item.P(9)<0)return false;
   for(int n=10;n<=20;n++)if(n!=15&&item.P(n)!=0)return false;
   if(item.P(6)<=-1&&item.P(6)>=-32768){RenderResourceAudit.Record("printed invisible rest",rest.ToString(),"original Code2 visibility branch","no body or attachments requested");return true;}if(Single.IsNaN(item.P(6))||item.P(6)<-1||item.P(6)>32767)return false;
   int count=Math.Max(1,rest),glyph=rest>0?44:43+rest;float scale=NativeDatRestAttachments.EffectiveScale(item,page);
   float position=item.P(9);if(position==-1&&rest<=-2){if(page.SourceScore==null)return false;var centered=NativeRestCentering.PlanItem(page.SourceScore,item);if(centered.Count>0)position=centered[0].Position;}float origin=rest<=-2&&position>0?F((double)position*150):page.Prepared.X;
   float x=count>1?F((double)origin-3*scale):origin;
   float offset=rest==-4?0:rest==-2?63:rest==-1?49:56;
   float units=F(offset/7.0-Math.Min(2,count/2)*2);
   bool cue=item.P(15)==0&&Math.Abs((int)item.P(4))>=100;
   if(count>1&&cue){units=F((double)units+.699999988079071);units=F((double)units+.699999988079071);}
   RenderResourceAudit.Record("printed rest",glyph.ToString(),"original shared rest placement and external LIB","68-item supplied EPS audit domain: centring, default/relative pitch, repeated flags and fill-plus-stroke");
   float glyphSize=F((double)scale/page.Scale),pitch=item.P(15)==0?item.P(4)%100:item.P(4);
   int ledgerPitch=(int)item.P(4)%100;
   bool procedures=!NativeNotationView.UsesDat&&cache!=null;
   bool attachmentImmediate=attachment.HasValue&&procedures&&cache.Contains(31)&&attachment.Value.Horizontal==attachment.Value.Vertical;
   bool dotsImmediate=procedures&&cache.Contains(24),bodyImmediate=procedures&&cache.Contains(glyph);
   // The original prepares fermata, dots, body, then ledger. Procedure calls
   // paint immediately; expanded requests drain in that same preparation order.
   for(int phase=0;phase<2;phase++){
    if(phase==1&&beforeDeferred!=null)beforeDeferred();
    bool immediate=phase==0;
    if(attachment.HasValue&&attachmentImmediate==immediate&&!DrawFermata(g,page,library,cache,attachment.Value))return false;
    if(dotsImmediate==immediate&&!DrawDots(g,item,page,library,cache,rest,origin))return false;
    if(bodyImmediate==immediate){
     for(int n=0;n<count;n++){
      float y=F((double)page.Vertical(F((double)pitch+units))+n*14.0*scale);
      if(!NativePrimitiveGlyphs.Draw(g,page,library,cache,new PrimitiveGlyphRequest{Glyph=glyph,X=x,Y=y,Horizontal=glyphSize,Vertical=glyphSize,WidthSelector=0}))return false;
      x=F((double)x+4.5*scale);
     }
    }
    if(!immediate&&((rest==-2&&(ledgerPitch>=4||ledgerPitch<=-7))||(rest==-1&&(ledgerPitch>=5||ledgerPitch<=-6)))){
     float y=page.Vertical(F((double)pitch+offset/7.0));
     NativeNoteDetails.Path(g,page,new[]{new BarlinePoint(F((double)origin-8*scale),y,1),new BarlinePoint(F((double)origin+25*scale),y,0)},NativePrintContext.DefaultLineWidth);
    }
   }
   return true;
  }
 }
}
