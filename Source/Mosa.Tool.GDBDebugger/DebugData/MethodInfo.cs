﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Tool.GDBDebugger.DebugData
{
	public class MethodInfo
	{
		public ulong Address { get; set; }
		public uint Size { get; set; }
		public ulong DefAddress { get; set; }
		public string FullName { get; set; }
		public string Type { get; set; }
		public string ReturnType { get; set; }
		public uint StackSize { get; set; }
		public uint ParameterStackSize { get; set; }
		public uint Attributes { get; set; }
	}
}
