using System;

namespace DotChip8.Chip8
{
    /// <summary>
    /// A static helper class to extract specific parts of a 16-bit opcode.
    /// </summary>
    public static class InstructionDecoder
    {
        /// <summary>
        /// Extracts the highest 4 bits (nibble) of the opcode to determine the instruction category.
        /// </summary>
        public static byte GetCategory(ushort opcode)
        {
            return (byte)(opcode >> 4);
        }

        /// <summary>
        /// Extracts the lowest 12 bits of the opcode (NNN). Usually an address.
        /// </summary>
        public static ushort GetNNN(ushort opcode)
        {
            return (ushort)(opcode & 0xFFF);
        }

        /// <summary>
        /// Extracts the lowest 8 bits of the opcode (NN). Usually a constant value.
        /// </summary>
        public static byte GetNN(ushort opcode)
        {
            return (byte)(opcode & 0xFF);
        }

        /// <summary>
        /// Extracts the lowest 4 bits (N). Usually a 4-bit number.
        /// </summary>
        public static byte GetN(ushort opcode)
        {
            return (byte)(opcode & 0xF);
        }

        /// <summary>
        /// Extracts the second nibble of the opcode (X). Usually a register index.
        /// </summary>
        public static byte GetX(ushort opcode)
        {
            return (byte)(opcode >> 8 & 0xF);
        }

        /// <summary>
        /// Extracts the third nibble of the opcode (Y). Usually a register index.
        /// </summary>
        public static byte GetY(ushort opcode)
        {
            return (byte)(opcode >> 4 & 0xF);
        }
    }
}
