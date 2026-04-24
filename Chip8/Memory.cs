using System;

namespace DotChip8.Chip8
{
    public class Memory
    {
        private readonly byte[] _ram;
        private readonly byte[] _fontset;

        public Memory()
        {
            _ram = new byte[4096];
            _fontset =
                    [
                        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                        0x20, 0x60, 0x20, 0x20, 0x70, // 1
                        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
                    ];
            for (int i = 0; i < _fontset.Length; i += 1)
            {
                _ram[i] = _fontset[i];
            }
        }

        /// <summary>
        /// Reads a single byte from the specified memory address.
        /// </summary>
        public byte ReadByte(ushort address)
        {
            return _ram[address];
        }

        /// <summary>
        /// Writes a single byte to the specified memory address.
        /// </summary>
        public void WriteByte(ushort address, byte value)
        {
            _ram[address] = value;
        }

        /// <summary>
        /// Reads two consecutive bytes from memory and combines them into a 16-bit word (opcode).
        /// </summary>
        public ushort ReadWord(ushort address)
        {
            return (ushort)(ReadByte(address) << 8 + ReadByte((ushort)(address + 1)));
        }

        /// <summary>
        /// Loads an array of bytes (a ROM) into memory starting at address 0x200.
        /// </summary>
        public void LoadRom(byte[] romData)
        {
            ushort address = 0x200;
            if (address + _ram.Length > romData.Length)
            {
                throw new InvalidOperationException("The given ROM file is too large to fit in memory.");
            }
            for (int i = 0; i < romData.Length; i++)
            {
                _ram[address + i] = romData[i];
            }
        }
    }
}
