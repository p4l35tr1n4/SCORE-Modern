using System;
namespace ScoreModern.RecoveredFragments {
 // Compiler numeric constants recovered from original initialized data.
 // Four following descriptor words reproduce the measured DOS load for the
 // opt-in quarter-tone compatibility behavior, not portable font widths.
 // No resource, font or installation/serial data is embedded.
 public static class LayoutSpacingWorkspace {
  public static byte[] Create() {
   var data=new byte[65536];
   Buffer.BlockCopy(BitConverter.GetBytes(0x40900000u),0,data,0x383E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40A00000u),0,data,0x3842,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40354FDFu),0,data,0x3846,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x384A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x4019999Au),0,data,0x384E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40066666u),0,data,0x3852,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40800000u),0,data,0x3856,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40200000u),0,data,0x385A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x385E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x4059999Au),0,data,0x3862,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x3866,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40333333u),0,data,0x386A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40833333u),0,data,0x386E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40933333u),0,data,0x3872,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40833333u),0,data,0x3876,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40C66666u),0,data,0x387A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40900000u),0,data,0x387E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x00002910u),0,data,0x3882,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x00003729u),0,data,0x3886,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x002E2910u),0,data,0x388A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x362D3729u),0,data,0x388E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3A83126Fu),0,data,0x7748,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F800000u),0,data,0x774C,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40000000u),0,data,0x7750,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40A00000u),0,data,0x7754,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40E00000u),0,data,0x7758,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0xBF800000u),0,data,0x775E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3D23D70Au),0,data,0x7762,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x42C80000u),0,data,0x7766,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41200000u),0,data,0x776A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FC00000u),0,data,0x776E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F000000u),0,data,0x7772,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3DCCCCCDu),0,data,0x7776,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F333333u),0,data,0x777A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41A00000u),0,data,0x777E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40133333u),0,data,0x7782,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F666666u),0,data,0x7786,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FD9999Au),0,data,0x778A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40800000u),0,data,0x778E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41400000u),0,data,0x7792,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FA66666u),0,data,0x7796,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3FE66666u),0,data,0x779A,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40F00000u),0,data,0x779E,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40D00000u),0,data,0x77A2,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40400000u),0,data,0x77A6,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3E4CCCCDu),0,data,0x77AA,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x41700000u),0,data,0x77AE,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40200000u),0,data,0x77B2,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3CF5C28Fu),0,data,0x77B6,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x40333333u),0,data,0x77BC,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x406CCCCDu),0,data,0x77C0,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0x3F800000u),0,data,0x77E0,4);
   Buffer.BlockCopy(BitConverter.GetBytes(0xC0800000u),0,data,0x77F6,4);
   return data;
  }
 }
}
