using System;

namespace ScoreModern.RecoveredFragments
{
    public interface IBiosKeyContext
    {
        // Explicit dependency for original INT 16h/AH=00h. No Windows-to-BIOS
        // mapping or original hardware/timing behavior is supplied here.
        ushort ReadBiosKey();
        void WriteByte(ushort segment, ushort offset, byte value);
    }

    public static class BiosKeyReader
    {
        // Original root 16FA:0841, 23 bytes. Ports the BIOS result and ordered
        // output-byte effects, not the complete register/flag/stack ABI.
        public static void Execute(IBiosKeyContext context, ushort asciiSegment,
            ushort asciiOffset, ushort scanSegment, ushort scanOffset)
        {
            if (context == null) throw new ArgumentNullException("context");
            ushort ax = context.ReadBiosKey();
            context.WriteByte(asciiSegment, asciiOffset, (byte)(ax & 255));
            context.WriteByte(scanSegment, scanOffset, (byte)(ax >> 8));
        }
    }
}
