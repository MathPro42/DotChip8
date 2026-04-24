using System;

namespace DotChip8.Chip8
{
    public class Keypad
    {
        // Tracks the state of the 16 keys (0x0 to 0xF). True = pressed.
        private bool[] _keys;

        public Keypad()
        {
            _keys = new bool[16];
        }

        /// <summary>
        /// Updates the state of a specific key.
        /// </summary>
        public void SetKey(byte key, bool isPressed)
        {
            _keys[key] = isPressed;
        }

        /// <summary>
        /// Checks if a specific key is currently pressed.
        /// </summary>
        public bool IsKeyPressed(byte key)
        {
            return _keys[key];
        }
    }
}
