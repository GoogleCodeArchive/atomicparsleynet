//==================================================================//
/*
    AtomicParsley - id3v2types.h

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
using System.Text;
using FRAFV.Binary.Serialization;

namespace ID3v2
{
	public enum FieldTypes
	{
		Unknown = -1,
		Text,
		TextEncoding,
		Owner, // UFID,PRIV
		Description, // TXXX, WXXX
		URL,
		Language, // USLT
		MIMEType, // APIC
		PicType, // APIC
		BinaryData, // APIC,GEOB
		Filename, // GEOB
		GroupSymbol,
		Counter,
		ImageFormat // PIC in v2.2
	}

	/// <summary>
	/// the wording of the ID3 (v2.4 in this case) 'informal standard' is not always
	/// replete with clarity.  text encodings are worded as having a NULL terminator
	/// (8or16bit), even for the body of text frames with that in hand, then a
	/// description field from COMM should look much like a utf8 text field and yet
	/// for TXXX, description is expressely worded as:
	///
	/// "The frame body consists of a description of the string, represented as a
	/// terminated string, followed by the actual string."
	///
	///     Description       <text string according to encoding> $00 (00)
	///     Value             <text string according to encoding>
	///
	/// Note how description is expressly *worded* as having a NULL terminator, but
	/// the text field is not.  GEOB text clarifies things better: "The first two
	/// strings [mime & filename] may be omitted, leaving only their terminations.
	///
	///     MIME type              <text string> $00
	///     Filename               <text string according to encoding> $00 (00)
	///
	/// so these trailing $00 (00) are the terminators for the strings - not
	/// separators between n-length string fields.  If the string is devoid of
	/// content (not NULLed out, but *devoid* of info), then the only thing that
	/// should exist is for a utf16 BOM to exist on text encoding 0x01. The
	/// (required) terminator for mime & filename are specifically enumerated in the
	/// frame format, which matches the wording of the frame description.  ...and so
	/// AP does not terminate text fields
	///
	/// Further sealing the case is the reference implementation for id3v2.3
	/// (id3lib) doesn't terminate text fields:
	///
	/// http://sourceforge.net/project/showfiles.php?group_id=979&package_id=4679
	/// </summary>
	public enum TextEncodings
	{
		/// <summary>
		/// ISO-8859-1 (LATIN-1, Identical to ASCII for values smaller than 0x80)
		/// </summary>
		Latin1 = 0,
		/// <summary>
		/// UCS-2 (UTF-16 encoded Unicode with BOM), in ID3v2.2 and ID3v2.3
		/// </summary>
		UTF16LEWithBOM = 1,
		/// <summary>
		/// UTF-16BE encoded Unicode without BOM, in ID3v2.4
		/// </summary>
		UTF16BENoBOM = 2,
		/// <summary>
		/// UTF-8 encoded Unicode, in ID3v2.4
		/// </summary>
		UTF8 = 3
	}

	public interface IID3v2Field
	{
		FieldTypes Type { get; }
		byte[] Data { get; }
	}

	public static class ID3v2Fields
	{
		#region Field structures
		internal struct UnknownField : IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.Unknown; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
		}

		internal struct TextField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.Text; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public void SetText(TextEncodings encoding, Encoding latin1, string separator, params string[] value)
			{
				this.Data = value.Text2Data(encoding, latin1, separator);
			}
			public string[] GetText(TextEncodings encoding, Encoding latin1, string separator)
			{
				return this.Data.Data2Text(encoding, latin1, separator);
			}
		}

		internal struct TextEncodingField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.TextEncoding; } }
			public byte Data;
			byte[] IID3v2Field.Data { get { return new byte[] { this.Data }; } }
			public TextEncodings TextEncoding
			{
				get { return (TextEncodings)this.Data; }
				set { this.Data = (byte)value; }
			}
		}

		internal struct OwnerField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.Owner; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public string Owner
			{
				get { return this.Data.Data2String(); }
				set { this.Data = value.String2Data(); }
			}
		}

		internal struct DescriptionField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.Description; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public void SetDescription(TextEncodings encoding, string value)
			{
				this.Data = value.String2Data(encoding);
			}
			public string GetDescription(TextEncodings encoding)
			{
				return this.Data.Data2String(encoding);
			}
		}

		internal struct URLField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.URL; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public string URL
			{
				get { return this.Data.Data2String(); }
				set { this.Data = value.String2Data(); }
			}
		}

		internal struct LanguageField: IID3v2Field
		{
			public static readonly byte[] EmptyData = new byte[] { 0, 0, 0 };
			public FieldTypes Type { get { return FieldTypes.Language; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public string LanguageCode
			{
				get
				{
					if (Data.Length == 0 ||
						Data.Length == 3 && Data[0] == 0 && Data[1] == 0 && Data[2] == 0)
						return String.Empty;
					return this.Data.Data2String();
				}
				set
				{
					if (String.IsNullOrEmpty(value))
						this.Data = EmptyData;
					this.Data = value.String2Data();
				}
			}
		}

		internal struct MIMETypeField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.MIMEType; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public string MIMEType
			{
				get { return this.Data.Data2String(); }
				set { this.Data = value.String2Data(); }
			}
		}

		internal struct PicTypeField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.PicType; } }
			public byte Data;
			byte[] IID3v2Field.Data { get { return new byte[] { this.Data }; } }
		}

		internal struct BinaryDataField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.BinaryData; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
		}

		internal struct FileNameField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.Filename; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public void SetFileName(TextEncodings encoding, string value)
			{
				this.Data = value.String2Data(encoding);
			}
			public string GetFileName(TextEncodings encoding)
			{
				return this.Data.Data2String(encoding);
			}
		}

		internal struct GroupSymbolField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.GroupSymbol; } }
			public byte Data;
			byte[] IID3v2Field.Data { get { return new byte[] { this.Data }; } }
		}

		internal struct CounterField: IID3v2Field
		{
			public static readonly byte[] EmptyData = new byte[] { 0, 0, 0, 0 };
			public FieldTypes Type { get { return FieldTypes.Counter; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
			public long Counter
			{
				get { return this.Data.ToInt64(0, false); }
				set { this.Data = value.GetBytes(false); }
			}
		}

		internal struct ImageFormatField: IID3v2Field
		{
			public FieldTypes Type { get { return FieldTypes.ImageFormat; } }
			public byte[] Data;
			byte[] IID3v2Field.Data { get { return this.Data; } }
		}
		#endregion
	}
}
