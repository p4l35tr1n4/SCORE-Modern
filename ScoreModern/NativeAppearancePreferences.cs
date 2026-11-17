using System;using System.Drawing;using System.Drawing.Imaging;using System.Windows.Forms;using System.Linq;
namespace ScoreModern {
 public sealed class NativeScreenPalette {
  public Color Graphics,Background,Text,Edit;
  static readonly int[] Rgb={0x000000,0x0000aa,0x00aa00,0x00aaaa,0xaa0000,0xaa00aa,0xaa5500,0xaaaaaa,0x555555,0x5555ff,0x55ff55,0x55ffff,0xff5555,0xff55ff,0xffff55,0xffffff};
  public static NativeScreenPalette Read(string root){
   var preferences=NativePreferences.Read(root);
   string[] words=(preferences.Text("COLOR")??"0 15 0 9").Replace(',',' ').Split(new[]{' ','\t'},StringSplitOptions.RemoveEmptyEntries);
   if(words.Length!=3&&words.Length!=4)
    throw new NotSupportedException("COLO in "+preferences.Path+" requires foreground, background and text colours, with an optional edit colour.");
   var colors=new Color[4];
   colors[3]=Color.FromArgb(unchecked((int)0xff000000)|Rgb[9]);
   for(int n=0;n<words.Length;n++){
    int index;
    if(!Int32.TryParse(words[n],out index)||index<0||index>15)
     throw new NotSupportedException("COLO in "+preferences.Path+" requires integer colour numbers from 0 to 15.");
    colors[n]=Color.FromArgb(unchecked((int)0xff000000)|Rgb[index]);
   }
   var palette=new NativeScreenPalette{Graphics=colors[0],Background=colors[1],Text=colors[2],Edit=colors[3]};NativeWindowsColors.Apply(palette,preferences.Text("COLOR")!=null);return palette;
  }
  public void Remap(Graphics target,Bitmap bitmap,Rectangle destination){using(var attributes=new ImageAttributes()){float r=Graphics.R/255f,g=Graphics.G/255f,b=Graphics.B/255f;var m=new ColorMatrix(new[]{new[]{Background.R/255f-r,0f,0f,0f,0f},new[]{0f,Background.G/255f-g,0f,0f,0f},new[]{0f,0f,Background.B/255f-b,0f,0f},new[]{0f,0f,0f,1f,0f},new[]{r,g,b,0f,1f}});attributes.SetColorMatrix(m);target.DrawImage(bitmap,destination,0,0,bitmap.Width,bitmap.Height,GraphicsUnit.Pixel,attributes);}}
 }
 public sealed partial class Editor {
  void ApplyPreferenceColors(){var palette=NativeScreenPalette.Read(source);Color text=mode==ScreenMode.Edit?palette.Edit:palette.Text;ApplyTextColors(top,palette.Background,text);ApplyTextColors(functionStrip,palette.Background,palette.Text);BackColor=canvas.BackColor=palette.Background;viewport.BackColor=NativeScoreBehaviour.Enabled?palette.Background:workspaceColor??palette.Background;canvas.Invalidate();}
  static void ApplyTextColors(Control control,Color back,Color fore){if(control.BackColor!=back)control.BackColor=back;if(control.ForeColor!=fore)control.ForeColor=fore;foreach(Control child in control.Controls)ApplyTextColors(child,back,fore);}
 }
}
