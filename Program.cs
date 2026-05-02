using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DotChip8.Chip8;

namespace DotChip8
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("No path for the rom file provided");
            else if (args.Length > 1)
                throw new ArgumentException($"Cannot give more than 1 rom file: {args.Length - 1} given");

            Console.WriteLine("Starting DotChip8 Emulator...");

            Console.WriteLine("\nSelect your keyboard layout:");
            Console.WriteLine("[1] QWERTY");
            Console.WriteLine("[2] AZERTY");

            bool isAzerty;
            while (true)
            {
                var choice = Console.ReadKey(true).Key;
                if (choice == ConsoleKey.D1 || choice == ConsoleKey.NumPad1)
                {
                    isAzerty = false;
                    Console.WriteLine("QWERTY selected.");
                    break;
                }
                else if (choice == ConsoleKey.D2 || choice == ConsoleKey.NumPad2)
                {
                    isAzerty = true;
                    Console.WriteLine("AZERTY selected.");
                    break;
                }
            }
            Thread.Sleep(500);

            Memory memory = new Memory();
            Display display = new Display();
            Keypad keypad = new Keypad();
            Random random = new Random();

            Cpu cpu = new Cpu(memory, display, keypad, random);

            string romPath = args[0];
            byte[] romData = File.ReadAllBytes(romPath);
            memory.LoadRom(romData);

            Console.Clear();

            Stopwatch timerClock = Stopwatch.StartNew();

            bool isRunning = true;
            while (isRunning)
            {
                HandleInput(keypad, isAzerty);

                cpu.Cycle();

                if (cpu.DrawFlag)
                {
                    Console.SetCursorPosition(0, 0);
                    display.Render();
                    cpu.DrawFlag = false;
                }

                if (timerClock.ElapsedMilliseconds >= 16)
                {
                    cpu.UpdateTimers();
                    timerClock.Restart();

                    for (byte i = 0; i < 16; i++) { keypad.SetKey(i, false); }
                }

                Thread.Sleep(2);
            }
        }

        static void HandleInput(Keypad keypad, bool isAzerty)
        {
            if (!Console.KeyAvailable) return;

            var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);

            if (isAzerty)
            {
                // AZERTY Layout Mapping
                // & é " '
                // A Z E R
                // Q S D F
                // W X C V
                switch (keyChar)
                {
                    case '&': keypad.SetKey(0x1, true); break;
                    case 'é': keypad.SetKey(0x2, true); break;
                    case '"': keypad.SetKey(0x3, true); break;
                    case '\'': keypad.SetKey(0xC, true); break;

                    case 'a': keypad.SetKey(0x4, true); break;
                    case 'z': keypad.SetKey(0x5, true); break;
                    case 'e': keypad.SetKey(0x6, true); break;
                    case 'r': keypad.SetKey(0xD, true); break;

                    case 'q': keypad.SetKey(0x7, true); break;
                    case 's': keypad.SetKey(0x8, true); break;
                    case 'd': keypad.SetKey(0x9, true); break;
                    case 'f': keypad.SetKey(0xE, true); break;

                    case 'w': keypad.SetKey(0xA, true); break;
                    case 'x': keypad.SetKey(0x0, true); break;
                    case 'c': keypad.SetKey(0xB, true); break;
                    case 'v': keypad.SetKey(0xF, true); break;
                }
            }
            else
            {
                // QWERTY Layout Mapping
                // 1 2 3 4
                // Q W E R
                // A S D F
                // Z X C V
                switch (keyChar)
                {
                    case '1': keypad.SetKey(0x1, true); break;
                    case '2': keypad.SetKey(0x2, true); break;
                    case '3': keypad.SetKey(0x3, true); break;
                    case '4': keypad.SetKey(0xC, true); break;

                    case 'q': keypad.SetKey(0x4, true); break;
                    case 'w': keypad.SetKey(0x5, true); break;
                    case 'e': keypad.SetKey(0x6, true); break;
                    case 'r': keypad.SetKey(0xD, true); break;

                    case 'a': keypad.SetKey(0x7, true); break;
                    case 's': keypad.SetKey(0x8, true); break;
                    case 'd': keypad.SetKey(0x9, true); break;
                    case 'f': keypad.SetKey(0xE, true); break;

                    case 'z': keypad.SetKey(0xA, true); break;
                    case 'x': keypad.SetKey(0x0, true); break;
                    case 'c': keypad.SetKey(0xB, true); break;
                    case 'v': keypad.SetKey(0xF, true); break;
                }
            }
        }
    }
}
