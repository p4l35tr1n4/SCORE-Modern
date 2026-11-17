using System;

namespace ScoreModern.RecoveredFragments
{
    // Host interfaces expose the unresolved machine context and callees.
    // They do not supply inferred formatting behavior or segment defaults.
    public interface ISelectorContext
    {
        ushort DataSegment { get; }
        bool DirectionFlag { get; }
        ushort ReadWord(ushort segment, ushort offset);
        void WriteWord(ushort segment, ushort offset, ushort value);
        ushort CallFar(ushort relativeSegment, ushort offset, ushort[] stackWords);
    }

    public static class SelectorRoutine
    {
        // Original overlay 8, relative 2796:02D6. This ports observable data
        // accesses/control flow/call arguments; it is NOT a 16-bit ABI adapter.
        // Physical stack effects, registers, interrupts and callee internals
        // remain outside this interface and require separate integration proof.
        public static void Execute(ISelectorContext context, ushort parameterSegment, ushort parameterOffset)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (context.ReadWord(parameterSegment, parameterOffset) == 1)
            {
                context.CallFar(0x1f08, 0x0003, new ushort[] { 0x3c7a });
                return;
            }

            // The original reloads the parameter pointer and reads the word
            // again. Do not collapse the two reads into one cached state.
            short state = unchecked((short)context.ReadWord(parameterSegment, parameterOffset));
            if (state <= 1 || state >= 6) return;

            ushort source = (ushort)(0x3c46 + 6 * state);
            ushort destination = 0x3c6e;
            ushort destinationSegment = context.DataSegment; // PUSH DS; POP ES
            for (int n = 0; n < 3; n++)
            {
                ushort value = context.ReadWord(context.DataSegment, source);
                context.WriteWord(destinationSegment, destination, value);
                int delta = context.DirectionFlag ? -2 : 2;
                source = unchecked((ushort)(source + delta));
                destination = unchecked((ushort)(destination + delta));
            }
            ushort returnedAx = context.CallFar(0x1f08, 0x2ac6,
                new ushort[] { 0x0010, 0x3c6a, context.DataSegment });
            context.CallFar(0x1f08, 0x0003, new ushort[] { 0x3c82, returnedAx });
        }
    }
}
