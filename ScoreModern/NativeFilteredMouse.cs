using System;using System.Drawing;using System.Globalization;
namespace ScoreModern {public static class NativeFilteredMouse {
 public static string Arguments(Canvas canvas,PointF pointer,bool staffOnly){
  short dx=unchecked((short)((short)pointer.X-NativeDosViewport.OriginX));short productX=unchecked((short)(dx*NativeDosViewport.Divisor));short position=(short)Math.Round(productX/150.0,MidpointRounding.AwayFromZero);
  short dy=unchecked((short)(NativeDosViewport.OriginY-(short)pointer.Y));float ordinate=(float)(dy*NativeDosViewport.Divisor);long closest=35000;int selected=1,maximum=canvas.PageDocument.MaximumDefinedStaff;if(maximum>32)throw new NotSupportedException("Mouse staff selection exceeds the original 32-entry staff table.");
  for(int staff=1;staff<=maximum;staff++){double difference=Math.Abs((double)ordinate-((double)canvas.PageDocument.Tables.Scales[staff]*49+canvas.PageDocument.Tables.Bases[staff]));if(Double.IsNaN(difference)||difference>Int32.MaxValue)throw new NotSupportedException("Mouse staff-distance conversion requires original fault-state verification.");long distance=(long)Math.Truncate(difference);if(distance<closest){closest=distance;selected=staff;}}
  return staffOnly?selected.ToString(CultureInfo.InvariantCulture):position.ToString(CultureInfo.InvariantCulture)+" "+selected.ToString(CultureInfo.InvariantCulture);
 }
}}
