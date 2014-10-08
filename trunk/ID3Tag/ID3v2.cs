//==================================================================//
/*
    AtomicParsley - id3v2.cpp

    AtomicParsley is GPL software; you can freely distribute, 
    redistribute, modify & use under the terms of the GNU General
    Public License; either version 2 or its successor.

    AtomicParsley is distributed under the GPL "AS IS", without
    any warranty; without the implied warranty of merchantability
    or fitness for either an expressed or implied particular purpose.

    Please see the included GNU General Public License (GPL) for 
    your rights and further details; see the file COPYING. If you
    cannot, write to the Free Software Foundation, 59 Temple Place
    Suite 330, Boston, MA 02111-1307, USA.  Or www.fsf.org

    Copyright ©2006-2007 puck_lock
    with contributions from others; see the CREDITS file
    ----------------------
    SVN revision information:
      $Revision$
                                                                   */
//==================================================================//
using System;
using System.IO;
using System.Text;
using FRAFV.Binary.Serialization;

namespace ID3v2
{
	public static class BigEndianInt
	{
		#region Size
		internal static int ReadSize(this BinaryReader reader, int majorVersion)
		{
			switch (majorVersion)
			{
				case 4:
					return reader.ReadSynchSafeInt32();
				case 3:
					return reader.ReadInt32();
				case 2:
					return reader.ReadUInt24();
				default:
					throw new ArgumentOutOfRangeException("majorVersion");
			}
		}

		internal static void WriteSize(this BinaryWriter writer, int majorVersion, int size)
		{
			switch (majorVersion)
			{
				case 4:
					writer.WriteSynchSafe(size);
					break;
				case 3:
					writer.Write(size);
					break;
				case 2:
					writer.WriteInt24(size);
					break;
				default:
					throw new ArgumentOutOfRangeException("majorVersion");
			}
		}
		#endregion
	}

	public static class TextEncodingString
	{
		private static readonly ushort BOMLE = BitConverter.ToUInt16(Encoding.Unicode.GetPreamble(), 0);
		private static readonly ushort BOMBE = BitConverter.ToUInt16(Encoding.BigEndianUnicode.GetPreamble(), 0);

		#region Text with text encoding
		public static string[] Data2Text(this byte[] data, TextEncodings encoding, Encoding latin1, string separator)
		{
			string str = data.Data2String(encoding, latin1) ?? String.Empty;
			if (separator == null) return new string[] { str };
			return str.Split(new string[] { separator }, StringSplitOptions.None);
		}

		public static string[] ReadID3Text(this BinaryReader reader, int len, TextEncodings encoding, Encoding latin1, string separator)
		{
			return reader.ReadBytes(len).Data2Text(encoding, latin1, separator);
		}

		public static byte[] Text2Data(this string[] text, TextEncodings encoding, Encoding latin1, string separator)
		{
			string str;
			if (separator != null)
				str = String.Join(separator, text ?? new string[0]);
			else if (text != null && text.Length > 0)
				str = text[0];
			else
				str = String.Empty;
			return str.String2Data(encoding, latin1);
		}

		public static void WriteID3Text(this BinaryWriter writer, TextEncodings encoding, Encoding latin1, string separator, params string[] text)
		{
			writer.Write(text.Text2Data(encoding, latin1, separator));
		}
		#endregion

		#region String with text encoding
		public static string Data2String(this byte[] data, TextEncodings encoding = TextEncodings.Latin1, Encoding latin1 = null)
		{
			string str;
			switch (encoding)
			{
			case TextEncodings.Latin1:
				str = (latin1 ?? Encoding.ASCII).GetString(data);
				break;
			case TextEncodings.UTF16LEWithBOM:
				ushort bom = data.Length >= 2 ? BitConverter.ToUInt16(data, 0) : (ushort)0;
				if (bom == BOMLE)
					str = Encoding.Unicode.GetString(data, 2, data.Length - 2);
				else if (bom == BOMBE)
					str = Encoding.BigEndianUnicode.GetString(data, 2, data.Length - 2);
				else
					throw new NotSupportedException(String.Format("Unknown BOM {0}", bom));
				break;
			case TextEncodings.UTF16BENoBOM:
				str = Encoding.BigEndianUnicode.GetString(data);
				break;
			case TextEncodings.UTF8:
				str = Encoding.UTF8.GetString(data);
				break;
			default:
				throw new ArgumentException(String.Format("Unknown encoding {0}", encoding));
			}
			return str.TrimEnd('\0');
		}

		public static string ReadID3String(this BinaryReader reader, int len, TextEncodings encoding = TextEncodings.Latin1, Encoding latin1 = null)
		{
			return reader.ReadBytes(len).Data2String(encoding, latin1);
		}

		public static byte[] String2Data(this string str, TextEncodings encoding = TextEncodings.Latin1, Encoding latin1 = null)
		{
			switch (encoding)
			{
			case TextEncodings.Latin1:
				return (latin1 ?? Encoding.ASCII).GetBytes(str);
			case TextEncodings.UTF16LEWithBOM:
				var data = new byte[2 + Encoding.Unicode.GetByteCount(str)];
				Array.Copy(Encoding.Unicode.GetPreamble(), data, 2);
				Encoding.Unicode.GetBytes(str, 0, str.Length, data, 2);
				return data;
			case TextEncodings.UTF16BENoBOM:
				return Encoding.BigEndianUnicode.GetBytes(str);
			case TextEncodings.UTF8:
				return Encoding.UTF8.GetBytes(str);
			default:
				throw new ArgumentException(String.Format("Unknown encoding {0}", encoding));
			}
		}
		#endregion

		#region Code string

		public static TagID ReadTagID(this BinaryReader reader, int len)
		{
			return new TagID(reader.ReadBytes(len));
		}

		public static void Write(this BinaryWriter writer, TagID id)
		{
			writer.Write(id.Data);
		}
		#endregion
	}

	public static class TagStream
	{
		public static void WriteTo(this Stream src, Stream dst, int len)
		{
			var buffer = new byte[8192];
			while (len > 0)
			{
				int bytesRead = src.Read(buffer, 0, Math.Min(len, 8192));
				if (bytesRead == 0)
					break;
				dst.Write(buffer, 0, bytesRead);
				len -= bytesRead;
			}
		}
	}
}
