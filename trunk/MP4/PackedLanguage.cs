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
	/// <remarks>
	/// <para>Some elements of a QuickTime file may be associated with a particular spoken language. To indicate
	///   the language associated with a particular object, the QuickTime file format uses either language codes
	///   from the Macintosh Script Manager or ISO language codes (as specified in ISO 639-2/T).</para>
	/// <para>QuickTime stores language codes as unsigned 16-bit fields. All Macintosh language codes have a value
	///   that is less than 0x400 except for the single value 0x7FFF indicating an unspecified language. ISO language
	///   codes are three-character codes, and are stored inside the 16-bit language code field as packed arrays.
	///   If treated as an unsigned 16-bit integer, an ISO language code always has a value of 0x400 or greater unless
	///   the code is equal to the value 0x7FFF indicating an Unspecified Macintosh language code.</para>
	/// <para>If the language is specified using a Macintosh language code, any associated text uses Macintosh
	///   text encoding.</para>
	/// <para>If the language is specified using an ISO language code, any associated text uses Unicode text encoding.
	///   When Unicode is used, the text is in UTF-8 unless it starts with a byte-order-mark (BOM, 0xFEFF. ), whereupon
	///   the text is in UTF-16. Both the BOM and the UTF-16 text should be big-endian.</para>
	///
	/// <para>Note: ISO language codes cannot be used for all elements of a QuickTime file. Currently, ISO language codes
	///   can be used only for user data text. All other elements, including text tracks, must be specified using
	///   Macintosh language codes.</para>
	/// <para>Note: ISO 639-2/T codes do not distinguish between certain language variations. Use an extended language
	///   tag atom ('elng') to make these distinctions. For example, ISO 639-2T does not distinguish between traditional
	///   and simplified Chinese, so also use 'elng' with the value "zh-Hant" or "zh-Hans", respectively.</para>
	/// </remarks>
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
		/// <summary>
		/// Converts 16-bit integer into ISO language code.
		/// </summary>
		/// <param name="data">Packed ISO language code in 16-bit integer.</param>
		/// <returns>ISO language code.</returns>
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

		/// <summary>
		/// Converts ISO language code into 16-bit integer.
		/// </summary>
		/// <param name="code">ISO language code.</param>
		/// <returns>Packed ISO language code in 16-bit integer.</returns>
		/// <remarks>
		/// <para>Because the language codes specified by ISO 639-2/T are three characters long, they must be packed
		///   to fit into a 16-bit field. The packing algorithm must map each of the three characters, which are
		///   always lowercase, into a 5-bit integer and then concatenate these integers into the least significant
		///   15 bits of a 16-bit integer, leaving the 16-bit integer’s most significant bit set to zero.</para>
		/// <para>One algorithm for performing this packing is to treat each ISO character as a 16-bit integer.
		///   Subtract 0x60 from the first character and multiply by 2^10 (0x400), subtract 0x60 from the second
		///   character and multiply by 2^5 (0x20), subtract 0x60 from the third character, and add the three
		///   16-bit values. This will result in a single 16-bit value with the three codes correctly packed into
		///   the 15 least significant bits and the most significant bit set to zero.</para>
		/// </remarks>
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

		public static explicit operator ushort(PackedLanguage lang)
		{
			return lang.code;
		}

		public static explicit operator PackedLanguage(string lang)
		{
			return new PackedLanguage { code = Code2Data(lang) };
		}

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
