﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;
using Mosa.Kernel.x86.Smbios;
using Mosa.Runtime.x86;
using System;

namespace Mosa.HelloWorld.x86
{
	/// <summary>
	/// Boot
	/// </summary>
	public static class Boot
	{
		public static ConsoleSession Console;

		/// <summary>
		/// Main
		/// </summary>
		public static void Main()
		{
			Kernel.x86.Kernel.Setup();

			Console = ConsoleManager.Controller.Boot;

			Console.Clear();

			Console.ScrollRow = 25;

			IDT.SetInterruptHandler(ProcessInterrupt);

			Console.Color = Color.White;

			Console.Goto(0, 0);

			Console.Color = Color.Yellow;
			Console.BackgroundColor = Color.Black;

			Console.Write("MOSA OS Version 1.4 '");
			Console.Color = Color.Red;
			Console.Write("Neptune");
			Console.Color = Color.Yellow;
			Console.Write("'                                Copyright 2008-2017");

			Console.Color = 0x0F;
			Console.Write(new String((char)205, 60));
			Console.Write((char)203);
			Console.Write(new String((char)205, 19));
			Console.WriteLine();

			Console.Goto(2, 0);
			Console.Color = Color.Green;
			Console.Write("Multibootaddress: ");
			Console.Color = Color.Gray;
			Console.Write(Multiboot.MultibootAddress, 16, 8);

			Console.WriteLine();
			Console.Color = Color.Green;
			Console.Write("Multiboot-Flags:  ");
			Console.Color = Color.Gray;
			Console.Write(Multiboot.Flags, 2, 32);
			Console.WriteLine();

			Console.Color = Color.Green;
			Console.Write("Size of Memory:   ");
			Console.Color = Color.Gray;
			Console.Write((Multiboot.MemoryLower + Multiboot.MemoryUpper) / 1024, 10, -1);
			Console.Write(" MB (");
			Console.Write(Multiboot.MemoryLower + Multiboot.MemoryUpper, 10, -1);
			Console.Write(" KB)");
			Console.WriteLine();
			Console.WriteLine();

			Console.WriteLine();
			Console.Color = Color.Green;
			Console.Write("Smbios Info: ");

			if (SmbiosManager.IsAvailable)
			{
				Console.Color = Color.White;
				Console.Write("[");
				Console.Color = Color.Gray;
				Console.Write("Version ");
				Console.Write(SmbiosManager.MajorVersion, 10, -1);
				Console.Write(".");
				Console.Write(SmbiosManager.MinorVersion, 10, -1);
				Console.Color = Color.White;
				Console.Write("]");
				Console.WriteLine();

				Console.Color = Color.Yellow;
				Console.Write("[Bios]");
				Console.Color = Color.White;
				Console.WriteLine();

				var biosInformation = new BiosInformationStructure();
				Console.Color = Color.White;
				Console.Write("Vendor: ");
				Console.Color = Color.Gray;
				Console.Write(biosInformation.BiosVendor);
				Console.WriteLine();
				Console.Color = Color.White;
				Console.Write("Version: ");
				Console.Color = Color.Gray;
				Console.Write(biosInformation.BiosVersion);
				Console.WriteLine();
				Console.Color = Color.White;
				Console.Write("Date: ");
				Console.Color = Color.Gray;
				Console.Write(biosInformation.BiosDate);

				Console.Color = Color.Yellow;
				Console.Row = 8;
				Console.Column = 35;
				Console.Write("[Cpu]");
				Console.Color = Color.White;
				Console.WriteLine();
				Console.Column = 35;

				var cpuStructure = new CpuStructure();
				Console.Color = Color.White;
				Console.Write("Vendor: ");
				Console.Color = Color.Gray;
				Console.Write(cpuStructure.Vendor);
				Console.WriteLine();
				Console.Column = 35;
				Console.Color = Color.White;
				Console.Write("Version: ");
				Console.Color = Color.Gray;
				Console.Write(cpuStructure.Version);
				Console.WriteLine();
				Console.Column = 35;
				Console.Color = Color.White;
				Console.Write("Socket: ");
				Console.Color = Color.Gray;
				Console.Write(cpuStructure.Socket);
				Console.Write(" MHz");
				Console.WriteLine();
				Console.Column = 35;
				Console.Color = Color.White;
				Console.Write("Cur. Speed: ");
				Console.Color = Color.Gray;
				Console.Write(cpuStructure.MaxSpeed, 10, -1);
				Console.Write(" MHz");
				Console.WriteLine();
				Console.Column = 35;
			}
			else
			{
				Console.Color = Color.Red;
				Console.Write("No SMBIOS available on this system!");
			}

			Console.WriteLine();
			Console.WriteLine();

			Console.Color = Color.Green;
			Console.Write("Memory-Map:");
			Console.WriteLine();

			for (uint index = 0; index < Multiboot.MemoryMapCount; index++)
			{
				Console.Color = Color.White;
				Console.Write(Multiboot.GetMemoryMapBase(index), 16, 8);
				Console.Write(" - ");
				Console.Write(Multiboot.GetMemoryMapBase(index) + Multiboot.GetMemoryMapLength(index) - 1, 16, 8);
				Console.Write(" (");
				Console.Color = Color.Gray;
				Console.Write(Multiboot.GetMemoryMapLength(index), 16, 8);
				Console.Color = Color.White;
				Console.Write(") ");
				Console.Color = Color.Gray;
				Console.Write("Type: ");
				Console.Write(Multiboot.GetMemoryMapType(index), 16, 1);
				Console.WriteLine();
			}

			Console.Color = Color.Yellow;
			Console.Goto(24, 29);
			Console.Write("www.mosa-project.org");

			// Borders

			Console.Color = Color.White;

			for (uint index = 0; index < 60; index++)
			{
				Console.Goto(14, index);
				Console.Write((char)205);
			}

			for (uint index = 0; index < 60; index++)
			{
				Console.Goto(6, index);
				Console.Write((char)205);
			}

			for (uint index = 60; index < 80; index++)
			{
				Console.Goto(19, index);
				Console.Write((char)205);
			}

			for (uint index = 0; index < 80; index++)
			{
				Console.Goto(23, index);
				Console.Write((char)205);
			}

			for (uint index = 2; index < 20; index++)
			{
				Console.Goto(index, 60);
				if (index == 6)
					Console.Write((char)185);
				else if (index == 14)
					Console.Write((char)185);
				else if (index == 19)
					Console.Write((char)200);
				else
					Console.Write((char)186);
			}

			Console.Goto(12, 0);

			while (true)
			{
				DisplayCMOS();
				DisplayTime();

				Native.Hlt();
			}
		}

		/// <summary>
		/// Displays the seconds.
		/// </summary>
		private static void DisplayCMOS()
		{
			Console.Row = 2;
			Console.Column = 65;
			Console.Color = 0x0A;
			Console.Write("CMOS:");
			Console.WriteLine();
			Console.Color = 0x0F;

			byte i = 0;
			for (byte x = 0; x < 5; x++)
			{
				Console.Column = 65;
				for (byte y = 0; y < 4; y++)
				{
					Console.Write(CMOS.Get(i), 16, 2);
					Console.Write(' ');
					i++;
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Displays the seconds.
		/// </summary>
		private static void DisplayTime()
		{
			Console.Goto(24, 50);
			Console.Color = Color.Green;
			Console.Write("Time: ");

			byte bcd = 10;

			if (CMOS.BCD)
				bcd = 16;

			Console.Color = Color.White;
			Console.Write(CMOS.Hour, bcd, 2);
			Console.Color = Color.Gray;
			Console.Write(':');
			Console.Color = Color.White;
			Console.Write(CMOS.Minute, bcd, 2);
			Console.Color = Color.Gray;
			Console.Write(':');
			Console.Color = Color.White;
			Console.Write(CMOS.Second, bcd, 2);
			Console.Write(' ');
			Console.Color = Color.Gray;
			Console.Write('(');
			Console.Color = Color.White;
			Console.Write(CMOS.Month, bcd, 2);
			Console.Color = Color.Gray;
			Console.Write('/');
			Console.Color = Color.White;
			Console.Write(CMOS.Day, bcd, 2);
			Console.Color = Color.Gray;
			Console.Write('/');
			Console.Color = Color.White;
			Console.Write('2');
			Console.Write('0');
			Console.Write(CMOS.Year, bcd, 2);
			Console.Color = Color.Gray;
			Console.Write(')');
		}

		private static uint counter = 0;

		public static void ProcessInterrupt(uint interrupt, uint errorCode)
		{
			counter++;

			uint c = Console.Column;
			uint r = Console.Row;
			byte col = Console.Color;
			byte back = Console.BackgroundColor;

			Console.Column = 31;
			Console.Row = 0;
			Console.Color = Color.Cyan;
			Console.BackgroundColor = Color.Black;

			Console.Write(counter, 10, 7);
			Console.Write(':');
			Console.Write(interrupt, 16, 2);
			Console.Write(':');
			Console.Write(errorCode, 16, 2);

			if (interrupt == 0x20)
			{
				// Timer Interrupt! Switch Tasks!
			}
			else
			{
				Console.Write('-');
				Console.Write(counter, 10, 7);
				Console.Write(':');
				Console.Write(interrupt, 16, 2);

				if (interrupt == 0x21)
				{
					byte scancode = Keyboard.ReadScanCode();
					Console.Write('-');
					Console.Write(scancode, 16, 2);
				}
			}

			Console.Column = c;
			Console.Row = r;
			Console.Color = col;
			Console.BackgroundColor = back;
		}
	}
}
