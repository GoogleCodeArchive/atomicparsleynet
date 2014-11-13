//------------------------------------------------------------------------------
// Copyright © FRA & FV 2014
// All rights reserved
//------------------------------------------------------------------------------
// MPEG-4 boxes Framework
//
// SVN revision information:
//   $Revision$
//------------------------------------------------------------------------------
using System;

namespace MP4
{
	/// <summary>
	/// Language Code
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Data2Name(code)}")]
	public struct PackedLanguage
	{
		private const ushort CodeZero = 0;
		private const ushort CodeUndefined = 0x55C4; //und = 75 6E 64
		private const ushort CodeUnspecified = 0x7FFF; //Unspecified Macintosh language code
		private const ushort MinISOCode = 0x400;
		private const int MaskChar1 = 0x1F << 10;
		private const int MaskChar2 = 0x1F << 5;
		private const int MaskChar3 = 0x1F;

		private ushort code;

		public ushort Data { get { return this.code; } }

		public bool IsUndefined { get { return this.code == CodeZero; } }
		public bool IsMac { get { return this.code == CodeUnspecified || this.code < MinISOCode; } }

		public PackedLanguage(ushort data)
		{
			this.code = data;
		}

		public PackedLanguage(string name)
		{
			this.code = Name2Data(name);
		}

		#region Conversions
		private static string Data2Code(ushort data)
		{
			if (data == CodeUnspecified || data < MinISOCode || data == CodeUndefined) return String.Empty;
			//the spec is unclear here, just says "the value 0 is interpreted as undetermined"
			//Macintosh language codes English = 0
			int l1 = (data & MaskChar1) >> 10;
			int l2 = (data & MaskChar2) >> 5;
			int l3 = data & MaskChar3;
			return new String(new char[] { (char)(l1 + 0x60), (char)(l2 + 0x60), (char)(l3 + 0x60) });
		}

		private static ushort Code2Data(string code)
		{
			if (String.IsNullOrEmpty(code) || code == "und")
				return 0;
			return (ushort)(
				(int)code[0] - 0x60 << 10 & MaskChar1 |
				(int)code[1] - 0x60 << 5 & MaskChar2 |
				(int)code[2] - 0x60 & MaskChar3);
		}

		private static string Data2Name(ushort data)
		{
			if (data == CodeUnspecified)// || data == 0)
				return "";
			if (data >= MinISOCode)
				return Data2Code(data);
			Catalog.MacLanguage mac;
			if (Catalog.MacLanguages.TryGetValue(data, out mac))
				return mac.ISO6391Code;
			return "";
		}

		private static ushort Name2Data(string name)
		{
			if (String.IsNullOrEmpty(name))
				return 0;
			Catalog.MacLanguage mac;
			if (name.Length == 3)
				return Code2Data(name);
			if (!Catalog.MacLanguagesISO.TryGetValue(name, out mac))
				throw new NotSupportedException(String.Format("Unknown language {0}", name));
			return mac.Code;
		}

		public static explicit operator string(PackedLanguage lang)
		{
			return Data2Code(lang.code);
		}

		/// <summary>
		/// Converts ISO language code into 16-bit integer.
		/// </summary>
		/// <param name="lang">ISO language code.</param>
		/// <returns>Packed ISO language code in 16-bit integer.</returns>
		public static explicit operator ushort(PackedLanguage lang)
		{
			return lang.code;
		}

		public static explicit operator PackedLanguage(string lang)
		{
			return new PackedLanguage { code = Code2Data(lang) };
		}

		/// <summary>
		/// Converts 16-bit integer into ISO language code.
		/// </summary>
		/// <param name="code">Packed ISO language code in 16-bit integer.</param>
		/// <returns>ISO language code.</returns>
		public static explicit operator PackedLanguage(ushort code)
		{
			return new PackedLanguage { code = code };
		}

		public override string ToString()
		{
			return Data2Name(this.code);
		}
		#endregion

		#region Equals
		public static bool operator ==(PackedLanguage lang, string other)
		{
			return lang.Equals(other);
		}

		public static bool operator ==(PackedLanguage lang, ushort other)
		{
			return lang.code == other;
		}

		public static bool operator ==(PackedLanguage lang, PackedLanguage other)
		{
			return lang.code == other.code;
		}

		public static bool operator !=(PackedLanguage lang, string other)
		{
			return !lang.Equals(other);
		}

		public static bool operator !=(PackedLanguage lang, ushort other)
		{
			return lang.code != other;
		}

		public static bool operator !=(PackedLanguage lang, PackedLanguage other)
		{
			return lang.code != other.code;
		}

		public override bool Equals(object obj)
		{
			if (obj is PackedLanguage)
				return this.code == ((PackedLanguage)obj).code;
			if (obj is ushort)
				return this.code == (ushort)obj;
			if (this.code < MinISOCode) return false;
			return String.Equals(Data2Code(this.code), obj as string);
		}

		public override int GetHashCode()
		{
			return this.code.GetHashCode();
		}
		#endregion
	}
}
