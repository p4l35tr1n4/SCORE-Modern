using System;

namespace ScoreModern.RecoveredFragments
{
    // Literal memory-copy fragment only, NOT a complete routine or application feature.
    // Source: overlay 8, 2796:02D6 + 0x22 through +0x43.
    // Contract: caller supplies the original 64 KiB DS contents, signed SI state,
    // and direction flag. Stack/register/call effects outside the copy are excluded.
    public static class SelectorCopy
    {
        public static bool Apply(byte[] dataSegment, short state, bool directionFlag)
        {
            if (dataSegment == null || dataSegment.Length != 65536)
                throw new ArgumentException("A complete 64 KiB data segment is required.");
            if (state <= 1 || state >= 6) return false;

            ushort source = (ushort)(0x3c46 + 6 * state);
            ushort destination = 0x3c6e;
            int increment = directionFlag ? -2 : 2;
            for (int n = 0; n < 3; n++)
            {
                // MOVSW reads the complete word before writing it.
                byte low = dataSegment[source];
                byte high = dataSegment[unchecked((ushort)(source + 1))];
                dataSegment[destination] = low;
                dataSegment[unchecked((ushort)(destination + 1))] = high;
                source = unchecked((ushort)(source + increment));
                destination = unchecked((ushort)(destination + increment));
            }
            return true;
        }
    }
}
