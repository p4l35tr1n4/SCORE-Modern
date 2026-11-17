using System;using System.Linq;using System.Runtime.InteropServices;using System.Windows.Forms;
namespace ScoreModern {public sealed class NativeMouseVisibility {
 [DllImport("user32.dll",SetLastError=true)]static extern IntPtr CreateCursor(IntPtr instance,int hotX,int hotY,int width,int height,byte[] andMask,byte[] xorMask);
 [DllImport("user32.dll")]static extern bool DestroyCursor(IntPtr handle);
 static Cursor blank;static IntPtr handle;int depth;Cursor saved;
 static Cursor Blank(){if(blank==null){handle=CreateCursor(IntPtr.Zero,0,0,32,32,Enumerable.Repeat((byte)255,128).ToArray(),new byte[128]);if(handle==IntPtr.Zero)throw new System.ComponentModel.Win32Exception();blank=new Cursor(handle);Application.ApplicationExit+=(s,e)=>{blank.Dispose();DestroyCursor(handle);handle=IntPtr.Zero;};}return blank;}
 public bool Hidden{get{return depth>0;}}
 public void Apply(Control target,bool show){if(show){if(depth>0)depth--;}else{if(depth==Int32.MaxValue)throw new NotSupportedException("Mouse-hide nesting exceeds the native counter range.");if(depth==0)saved=target.Cursor;depth++;}target.Cursor=Hidden?Blank():(saved??Cursors.Default);}
 }}
