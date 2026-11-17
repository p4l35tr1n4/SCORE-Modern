using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ScoreModern {
 sealed class WindowsMidiInput : IDisposable {
  internal sealed class Device {
   internal uint Id;
   internal string Name, Identity;
   public override string ToString(){return Name;}
  }

  [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
  struct Capabilities {
   public ushort Manufacturer,Product;
   public uint Version;
   [MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)] public string Name;
   public uint Support;
  }
  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  delegate void Callback(IntPtr device,uint message,UIntPtr context,UIntPtr data,UIntPtr timestamp);
  [DllImport("winmm.dll")] static extern uint midiInGetNumDevs();
  [DllImport("winmm.dll",ExactSpelling=true)] static extern uint midiInGetDevCapsW(UIntPtr id,out Capabilities caps,uint size);
  [DllImport("winmm.dll")] static extern uint midiInOpen(out IntPtr handle,uint id,Callback callback,UIntPtr context,uint flags);
  [DllImport("winmm.dll")] static extern uint midiInStart(IntPtr handle);
  [DllImport("winmm.dll")] static extern uint midiInStop(IntPtr handle);
  [DllImport("winmm.dll")] static extern uint midiInClose(IntPtr handle);

  internal static List<Device> Devices(){
   var result=new List<Device>();
   uint count=midiInGetNumDevs();
   for(uint id=0;id<count;id++){
    Capabilities caps;
    if(midiInGetDevCapsW(new UIntPtr(id),out caps,(uint)Marshal.SizeOf(typeof(Capabilities)))==0)
     result.Add(new Device{Id=id,Name=caps.Name,Identity=caps.Manufacturer+"/"+caps.Product+"/"+caps.Name});
   }
   return result;
  }

  IntPtr handle;
  readonly Callback callback;

  internal WindowsMidiInput(Device device,Action<uint> receive){
   callback=(port,message,context,data,time)=>{
    if(message==0x3c3)receive(data.ToUInt32());
   };
   Check(midiInOpen(out handle,device.Id,callback,UIntPtr.Zero,0x30000),"open");
   uint error=midiInStart(handle);
   if(error==0)return;
   Dispose();
   Check(error,"start");
  }

  static void Check(uint result,string operation){
   if(result!=0)throw new InvalidOperationException("Could not "+operation+" MIDI input. Windows MIDI error "+result+".");
  }

  public void Dispose(){
   if(handle==IntPtr.Zero)return;
   midiInStop(handle);
   midiInClose(handle);
   handle=IntPtr.Zero;
   GC.KeepAlive(callback);
  }
 }
}
