using System;
namespace ScoreModern.RecoveredFragments {
 public sealed class InputSetupState {
  public int Staff=1,LeftEnd,Vertical,RightEnd=200,Spacing=-1,LeftPosition,RightPosition=200,MeterTop=4,MeterBottom=4,Selected=1;
  public float Size=1;public string Key="C";
  // Resident 1a75:000f: the range test uses the truncated integer, even
  // for the size field, whose accepted value is stored as a float.
  static readonly int[] minimum={0,1,-99,-99,0,1,1,-99,1,1,1};
  static readonly int[] maximum={0,32,199,99,9,216,32,199,216,99,99};
  public void MoveField(int delta){Selected+=delta;if(Selected>11)Selected=1;if(Selected<1)Selected=11;}
  public bool Numeric(float value){if(Selected<1||Selected>10)throw new InvalidOperationException("Numeric setup field required");if(float.IsNaN(value)||float.IsInfinity(value)||value<int.MinValue||value>int.MaxValue)throw new ArgumentOutOfRangeException("value");int n=(int)Math.Truncate(value);if(n<minimum[Selected]||n>maximum[Selected])throw new ArgumentOutOfRangeException("value","Outside the original setup field range");
   switch(Selected){case 1:Staff=n;return true;case 2:if(RightEnd<=n)RightEnd=n+5;if(LeftPosition<n)LeftPosition=n;LeftEnd=n;break;case 3:Vertical=n;break;case 4:Size=value;break;case 5:if(n<=LeftEnd)LeftEnd=n-5;if(n<RightPosition)RightPosition=n;RightEnd=n;break;case 6:Spacing=n;break;case 7:if(RightPosition<=n)RightPosition=n+5;LeftPosition=n;break;case 8:if(n<=LeftPosition)LeftPosition=n-5;RightPosition=n;break;case 9:MeterTop=n;break;case 10:MeterBottom=n;break;}return false;
  }
  public void SpacingLetter(char letter){if(Selected!=6)throw new InvalidOperationException("Select spacing first");if(letter=='R')Spacing=-1;else if(letter=='P')Spacing=0;else throw new ArgumentException("R or P required");}
  public void KeyText(string key){if(Selected!=11)throw new InvalidOperationException("Select key signature first");if(key==null||key.Length<1||key.Length>2||key[0]<'A'||key[0]>'G')throw new ArgumentException("Key signature text");if(key.Length==2){char suffix=key[1];if(suffix=='F'||suffix=='B')suffix='b';if(suffix=='S')suffix='#';if(suffix!='b'&&suffix!='#')throw new ArgumentException("Key signature suffix");if(suffix=='b'&&key[0]=='F'||suffix=='#'&&key[0]!='C'&&key[0]!='F')throw new ArgumentException("Outside the original setup key signatures");key=key[0].ToString()+suffix;}Key=key;}
  // Overlay8 section61's filename selection. Files remain external.
  public static string PresetName(int scan){if(scan<0x3b||scan>0x71||(scan>0x44&&scan<0x54))throw new ArgumentOutOfRangeException("scan");if(scan<0x44)return "F"+(scan-0x3a)+".SET";if(scan<0x5d)return scan==0x44?"F10.SET":"F1"+(char)(scan-0x23)+".SET";if(scan<0x68)scan+=10;return scan==0x71?"F30.SET":"F2"+(char)(scan-0x37)+".SET";}
  public float[] StaffRecord(){return new float[]{8,Staff,LeftEnd,Vertical,Size==1?0:Size,RightEnd==200?0:RightEnd};}
 }
}
