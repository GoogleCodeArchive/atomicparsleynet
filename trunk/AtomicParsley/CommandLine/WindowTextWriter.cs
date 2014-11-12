//------------------------------------------------------------------------------
// Copyright © FRA & FV 2014
// All rights reserved
//------------------------------------------------------------------------------
// AtomicParsley
//
// SVN revision information:
//   $Revision$
//------------------------------------------------------------------------------
using System;
using System.IO;

namespace AtomicParsley.CommandLine
{
	public sealed class WindowTextWriter : TextWriter
	{
		private TextWriter Out;
		private int line;
		private int firstLine;
		private Lazy<bool> ShowA9 = new Lazy<bool>(() => CheckSym('©', 'c', 'C'));
		private Lazy<bool> ShowAE = new Lazy<bool>(() => CheckSym('®', 'r', 'R'));
		private Lazy<bool> Show2117 = new Lazy<bool>(() => CheckSym('℗', 'p', 'P'));

		private static bool CheckSym(char sym, char like1, char like2)
		{
			try
			{
				var b = Console.OutputEncoding.GetBytes(new string(sym, 1));
				return b[0] != (byte)like1 && b[0] != (byte)like2 && b[0] != (byte)'?';
			}
			catch
			{
				return false;
			}
		}

		public WindowTextWriter()
		{
			this.Out = Console.Out;
		}

		public int Indent { get; set; }

		public void WriteHorizontalLine(char line = '-')
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			int width = Console.WindowWidth;
			Out.Write(new string(line, width));
			if (width < Console.BufferWidth)
				Out.WriteLine();
			Console.ResetColor();
		}

		private void WriteString(string value)
		{
			value = value ?? String.Empty;
			value = value.Replace((char)160, ' ');
			value = value.Replace('\t', ' ');
			Out.Write(value);
		}

		private string FixEncoding(string value)
		{
			if (!ShowA9.Value) value = value.Replace("©", "(c)");
			if (!ShowAE.Value) value = value.Replace("®", "(R)");
			if (!Show2117.Value) value = value.Replace("℗", "(P)");
			return value;
		}

		public override void Write(char value)
		{
			switch (value)
			{
				case '\r':
				case '\n':
					line = 0;
					break;
				default:
					if (value == (char)160) value = ' ';
					if (line >= Console.WindowWidth)
					{
						if (line < Console.BufferWidth)
							Out.WriteLine();
						Out.Write(new string(' ', Indent));
						line = Indent;
					}
					else
						line++;
					break;
			}
			Out.Write(value);
		}

		public override void WriteLine()
		{
			Indent = 0;
			firstLine = 0;
			line = 0;
			Out.WriteLine();
		}

		public override void Write(string value)
		{
			value = FixEncoding(value ?? String.Empty);
			int scrWidth = Console.WindowWidth;
			int bufWidth = Console.BufferWidth;
			var lines = value.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			for (int k = 0; true; k++)
			{
				string l = lines[k];
				int tab = l.IndexOf('\t');
				if (tab > 0) firstLine = line + tab + 1 - Indent;
				while (line + l.Length > scrWidth)
				{
					int len = Math.Max(0, scrWidth - line);
					int sep = l.LastIndexOf(' ', len);
					if (sep < 0) sep = len;
					line += sep;
					WriteString(l.Remove(sep));
					l = sep < l.Length ? l.Substring(sep + 1) : "";
					if (line < scrWidth || line < bufWidth)
						Out.WriteLine();
					line = Indent + firstLine;
					Out.Write(new string(' ', line));
				}
				line += l.Length;
				WriteString(l);
				if (k == lines.Length - 1) break;
				if (line < scrWidth || line < bufWidth)
					Out.WriteLine();
				line = Indent;
				firstLine = 0;
				Out.Write(new string(' ', line));
			}
		}

		public override void Write(char[] buffer)
		{
			Write(new string(buffer));
		}

		public override void Write(char[] buffer, int index, int count)
		{
			Write(new string(buffer, index, count));
		}

		public override void WriteLine(string value)
		{
			Write(value);
			WriteLine();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Out.Dispose();
		}

		public override void Flush()
		{
			Out.Flush();
		}

		public override System.Text.Encoding Encoding
		{
			get { return Out.Encoding; }
		}

#if NET45
		public static bool IsOutputRedirected
		{
			get { return Console.IsOutputRedirected; }
		}

		public static bool IsInputRedirected
		{
			get { return Console.IsInputRedirected; }
		}
#else
		[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern int GetFileType(Microsoft.Win32.SafeHandles.SafeFileHandle handle);
		[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

		private static bool IsHandleRedirected(int nStdHandle)
		{
			var ioHandle = GetStdHandle(nStdHandle);
			var handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(ioHandle, false);
			int fileType = GetFileType(handle);
			if ((fileType & 2) != 2)
			{
				return true;
			}
			int num;
			bool consoleMode = GetConsoleMode(ioHandle, out num);
			return !consoleMode;
		}

		public static bool IsOutputRedirected
		{
			[System.Security.SecuritySafeCritical]
			get { return IsHandleRedirected(-11); }
		}

		public static bool IsInputRedirected
		{
			[System.Security.SecuritySafeCritical]
			get { return IsHandleRedirected(-10); }
		}
#endif
	}

	public static class TextWriterExtensions
	{
		public static void WriteHorizontalLine(this TextWriter writer)
		{
			var scrWriter = writer as WindowTextWriter;
			if (scrWriter != null)
				scrWriter.WriteHorizontalLine('-');
			else
				writer.WriteLine(new string('-', 80));
		}

		public static void WriteCenterLine(this TextWriter writer, string line)
		{
			if (writer is WindowTextWriter)
				writer.Write(new string(' ', Math.Max(0, Console.WindowWidth - line.Length) / 2));
			writer.WriteLine(line);
		}

		public static void SetIndent(this TextWriter writer, int indent)
		{
			var scrWriter = writer as WindowTextWriter;
			if (scrWriter != null) scrWriter.Indent = indent;
		}
	}
}
