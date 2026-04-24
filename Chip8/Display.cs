using System;

namespace DotChip8.Chip8
{
    public class Display
    {
        private bool[] _gfx;
        private int _length;
        private int _height;


        public Display()
        {
            _gfx = new bool[64 * 32];
            _length = 64;
            _height = 32;
        }

        public Display(int length, int height)
        {
            _gfx = new bool[length * height];
            _length = length;
            _height = height;
        }

        /// <summary>
        /// Clears the entire display (sets all pixels to false).
        /// Corresponds to opcode 00E0.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _gfx.Length; i += 1)
            {
                _gfx[i] = false;
            }
        }

        /// <summary>
        /// Draws a sprite to the screen given an X, Y coordinate and the sprite data bytes.
        /// Sprites XOR their pixels against the current screen.
        /// If a pixel is erased (goes from true to false), return true (collision). Otherwise, false.
        /// Corresponds to opcode DXYN.
        /// </summary>
        public bool DrawSprite(byte x, byte y, byte[] spriteData)
        {
            bool collision = false;
            for (int i = 0; i < spriteData.Length; i += 1)
            {
                for (int j = 7; j >= 0; j -= 1)
                {
                    int drawX = (x + (7 - j)) % _length;
                    int drawY = (y + i) % _height;

                    int index = drawX + (drawY * _length);

                    bool pixelState = _gfx[index];
                    bool spriteBit = (spriteData[i] & (1 << j)) != 0;

                    _gfx[index] ^= spriteBit;

                    if (pixelState && !_gfx[index])
                    {
                        collision = true;
                    }
                }
            }
            return collision;
        }

        /// <summary>
        /// Renders the current state of _gfx to the Console.
        /// </summary>
        public void Render()
        {
            Console.WriteLine("----------------------------------------------------------------");
            for (int j = 0; j < _height; j += 1)
            {
                string line = "";
                for (int i = 0; i < _length; i += 1)
                {
                    if (_gfx[i + _height * j])
                    {
                        line += '█';
                    }
                    else
                    {
                        line += ' ';
                    }
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------------------------------------------");
        }
    }
}
