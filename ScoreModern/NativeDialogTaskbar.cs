using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ScoreModern {
 internal static class NativeDialogTaskbar {
  [StructLayout(LayoutKind.Sequential)]
  struct PropertyKey {public Guid Format;public uint Id;}
  [StructLayout(LayoutKind.Sequential)]
  struct PropertyValue {public ushort Type,Reserved1,Reserved2,Reserved3;public IntPtr Text,Reserved4;}
  [ComImport,Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  interface PropertyStore {
   [PreserveSig]int GetCount(out uint count);
   [PreserveSig]int GetAt(uint index,out PropertyKey key);
   [PreserveSig]int GetValue(ref PropertyKey key,out PropertyValue value);
   [PreserveSig]int SetValue(ref PropertyKey key,ref PropertyValue value);
   [PreserveSig]int Commit();
  }
  [DllImport("shell32.dll",ExactSpelling=true)]
  static extern int SHGetPropertyStoreForWindow(IntPtr window,ref Guid id,out PropertyStore store);

  internal static void Set(IntPtr window,string name){
   var id=typeof(PropertyStore).GUID;
   PropertyStore store;
   if(SHGetPropertyStoreForWindow(window,ref id,out store)<0||store==null)return;
   var key=new PropertyKey{Format=new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),Id=5};
   string suffix=Regex.Replace(name,"[^a-zA-Z0-9.-]","-");
   var value=new PropertyValue{Type=31,Text=Marshal.StringToCoTaskMemUni("ScoreModern.Dialog."+suffix)};
   try{store.SetValue(ref key,ref value);}
   finally{Marshal.FreeCoTaskMem(value.Text);Marshal.ReleaseComObject(store);}
  }
 }
}
