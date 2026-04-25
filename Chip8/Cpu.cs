using System;
using System.Collections.Generic;

namespace DotChip8.Chip8
{
    public class Cpu(Memory memory, Display display, Keypad keypad, Random random)
    {
        public bool DrawFlag { get; set; } = false;
        // Hardware References
        private Memory _memory = memory;
        private Display _display = display;
        private Keypad _keypad = keypad;
        private Random _random = random;

        // CPU State
        private byte[] _v = new byte[16];
        private ushort _i = 0;
        private ushort _pc = 0x200;
        private Stack<ushort> _stack = new Stack<ushort>();

        // Timers
        private byte _delayTimer = 0;
        private byte _soundTimer = 0;

        /// <summary>
        /// Represents one cycle of the CPU: Fetch, Decode, Execute.
        /// Should also handle PC incrementation.
        /// </summary>
        public void Cycle()
        {
            ushort opcode = _memory.ReadWord(_pc);
            _pc += 2;
            switch (InstructionDecoder.GetCategory(opcode))
            {
                case 0:
                    Op0(opcode);
                    break;
                case 1:
                    _pc = InstructionDecoder.GetNNN(opcode);
                    break;
                case 2:
                    var nnn = InstructionDecoder.GetNNN(opcode);
                    _stack.Push(_pc);
                    _pc = nnn;
                    break;
                case 3:
                    if (_v[InstructionDecoder.GetX(opcode)] == InstructionDecoder.GetNN(opcode))
                    {
                        _pc += 2;
                    }
                    break;
                case 4:
                    if (_v[InstructionDecoder.GetX(opcode)] != InstructionDecoder.GetNN(opcode))
                    {
                        _pc += 2;
                    }
                    break;
                case 5:
                    if (_v[InstructionDecoder.GetX(opcode)] == _v[InstructionDecoder.GetY(opcode)])
                    {
                        _pc += 2;
                    }
                    break;
                case 6:
                    _v[InstructionDecoder.GetX(opcode)] = InstructionDecoder.GetNN(opcode);
                    break;
                case 7:
                    _v[InstructionDecoder.GetX(opcode)] += InstructionDecoder.GetNN(opcode);
                    break;
                case 8:
                    Op8(opcode);
                    break;
                case 9:
                    if (_v[InstructionDecoder.GetX(opcode)] != _v[InstructionDecoder.GetY(opcode)])
                    {
                        _pc += 2;
                    }
                    break;
                case 0xA:
                    _i = InstructionDecoder.GetNNN(opcode);
                    break;
                case 0xB:
                    _pc = (ushort)(_v[0] + InstructionDecoder.GetNNN(opcode));
                    break;
                case 0xC:
                    _v[InstructionDecoder.GetX(opcode)] = (byte)(_random.Next(256) & InstructionDecoder.GetNN(opcode));
                    break;
                case 0xD:
                    OpD(opcode);
                    break;
                case 0xE:
                    OpE(opcode);
                    break;
                case 0xF:
                    OpF(opcode);
                    break;
                default:
                    throw new InvalidOperationException("Unknown operation category:" + opcode.ToString());

            }
        }

        /// <summary>
        /// Updates the Delay and Sound timers.
        /// This should be called at 60Hz from your main loop, independent of the CPU Cycle speed.
        /// </summary>
        public void UpdateTimers()
        {
            if (_delayTimer > 0)
            {
                _delayTimer -= 1;
            }
            if (_soundTimer > 0)
            {
                _soundTimer -= 1;
            }
        }

        private void Op0(ushort opcode)
        {
            var nn = InstructionDecoder.GetNN(opcode);
            if (nn == 0x00E0)
            {
                _display.Clear();
                DrawFlag = true;
            }
            else if (nn == 0x00EE)
            {
                if (_stack.Count == 0)
                {
                    throw new InvalidOperationException("No subroutine to return from");
                }
                _pc = _stack.Pop();
            }
            else
            {
                throw new InvalidOperationException("Unknown operation code in 0:" + opcode.ToString());
            }
        }

        private void Op8(ushort opcode)
        {
            var x = InstructionDecoder.GetX(opcode);
            var y = InstructionDecoder.GetY(opcode);
            switch (InstructionDecoder.GetN(opcode))
            {
                case 0:
                    _v[x] = _v[y];
                    break;
                case 1:
                    _v[x] |= _v[y];
                    break;
                case 2:
                    _v[x] &= _v[y];
                    break;
                case 3:
                    _v[x] ^= _v[y];
                    break;
                case 4:
                    if (_v[x] + _v[y] > 255)
                        _v[0xF] = 1;
                    else
                        _v[0xF] = 0;
                    _v[x] += _v[y];
                    break;
                case 5:
                    if (_v[x] >= _v[y])
                        _v[0xF] = 1;
                    else
                        _v[0xF] = 0;
                    _v[x] = (byte)(_v[x] - _v[y]);
                    break;
                case 6:
                    _v[0xF] = (byte)(_v[x] & 0x1);
                    _v[x] >>= 1;
                    break;
                case 7:
                    if (_v[x] <= _v[y])
                        _v[0xF] = 1;
                    else
                        _v[0xF] = 0;
                    _v[x] = (byte)(_v[y] - _v[x]);
                    break;
                case 0xE:
                    _v[0xF] = (byte)(_v[x] >> 7 & 0x1);
                    _v[x] <<= 1;
                    break;
                default:
                    throw new InvalidOperationException("Unknown operation code:" + opcode.ToString());
            }
        }

        private void OpD(ushort opcode)
        {
            var x = InstructionDecoder.GetX(opcode);
            var y = InstructionDecoder.GetY(opcode);
            var n = InstructionDecoder.GetN(opcode);

            byte[] spriteData = new byte[n];
            for (int i = 0; i < n; i += 1)
            {
                spriteData[i] = _memory.ReadByte((ushort)(_i + i));
            }

            if (_display.DrawSprite(_v[x], _v[y], spriteData))
                _v[0xF] = 1;
            else
                _v[0xF] = 0;
            DrawFlag = true;
        }

        private void OpE(ushort opcode)
        {
            var x = InstructionDecoder.GetX(opcode);
            var nn = InstructionDecoder.GetNN(opcode);
            if (nn == 0x9E)
            {
                if (_keypad.IsKeyPressed(_v[x]))
                {
                    _pc += 2;
                }
            }
            else if (nn == 0xA1)
            {
                if (!_keypad.IsKeyPressed(_v[x]))
                {
                    _pc += 2;
                }
            }
            else
            {
                throw new InvalidOperationException($"Unknown operation code in E: {opcode:X}");
            }
        }

        private void OpF(ushort opcode)
        {
            var x = InstructionDecoder.GetX(opcode);
            var nn = InstructionDecoder.GetNN(opcode);
            switch (nn)
            {
                case 0x07:
                    _v[x] = _delayTimer;
                    break;
                case 0x0A:
                    bool keyPressed = false;
                    for (byte i = 0; i < 16; i += 1)
                    {
                        if (_keypad.IsKeyPressed(i))
                        {
                            _v[x] = i;
                            keyPressed = true;
                            break;
                        }
                    }
                    if (!keyPressed)
                    {
                        _pc -= 2;
                    }
                    break;
                case 0x15:
                    _delayTimer = _v[x];
                    break;
                case 0x18:
                    _soundTimer = _v[x];
                    break;
                case 0x1E:
                    _i += _v[x];
                    break;
                case 0x29:
                    _i = (ushort)(_v[x] * 5);
                    break;
                case 0x33:
                    _memory.WriteByte(_i, (byte)(_v[x] / 100));
                    _memory.WriteByte((ushort)(_i + 1), (byte)(_v[x] / 10 % 10));
                    _memory.WriteByte((ushort)(_i + 2), (byte)(_v[x] % 10));
                    break;
                case 0x55:
                    for (int i = 0; i <= x; i += 1)
                    {
                        _memory.WriteByte((ushort)(_i + i), _v[i]);
                    }
                    break;
                case 0x65:
                    for (int i = 0; i <= x; i += 1)
                    {
                        _v[i] = _memory.ReadByte((ushort)(_i + i));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown operation code in category F:" + opcode.ToString());
            }
        }
    }
}
