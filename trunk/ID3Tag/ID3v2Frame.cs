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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using FRAFV.Binary.Serialization;

namespace ID3v2
{
	/// <summary>
	/// the order of these frame types must exactly match the order listed in the FrameTypeConstructionList[] array!!!
	/// </summary>
	public enum FrameType
	{
		Unknown = -1,
		Text,
		TextUserDef,
		URL,
		URLUserDef,
		UniqueFileID,
		CDID,
		DescribedText, //oy... these frames (COMM, USLT) can differ by description
		AttachedPicture,
		AttachedObject,
		GroupID,
		Signature,
		Private,
		PlayCounter,
		Popular,
		OldV2p2Picture
	}

	/// <summary>
	/// Frame header flags
	/// </summary>
	/// <remarks>
	/// In the frame header the size descriptor is followed by two flag
	/// bytes. All unused flags MUST be cleared. The first byte is for
	/// 'status messages' and the second byte is a format description. If an
	/// unknown flag is set in the first byte the frame MUST NOT be changed
	/// without that bit cleared. If an unknown flag is set in the second
	/// byte the frame is likely to not be readable. Some flags in the second
	/// byte indicates that extra information is added to the header. These
	/// fields of extra information is ordered as the flags that indicates
	/// them. The flags field is defined as follows (l and o left out because
	/// ther resemblence to one and zero):
	/// 
	///   %0abc0000 %0h00kmnp
	/// 
	/// Some frame format flags indicate that additional information fields
	/// are added to the frame. This information is added after the frame
	/// header and before the frame data in the same order as the flags that
	/// indicates them. I.e. the four bytes of decompressed size will precede
	/// the encryption method byte. These additions affects the 'frame size'
	/// field, but are not subject to encryption or compression.
	/// 
	/// The default status flags setting for a frame is, unless stated
	/// otherwise, 'preserved if tag is altered' and 'preserved if file is
	/// altered', i.e. %00000000.
	/// </remarks>
	[Flags]
	public enum FrameFlags
	{
		None = 0,
		/// <summary>
		/// Tag alter preservation
		/// </summary>
		/// <remarks>
		/// This flag tells the tag parser what to do with this frame if it is
		/// unknown and the tag is altered in any way. This applies to all
		/// kinds of alterations, including adding more padding and reordering
		/// the frames.
		/// 
		/// 0     Frame should be preserved.
		/// 1     Frame should be discarded.
		/// </remarks>
		Status = 0x4000,
		/// <summary>
		/// File alter preservation
		/// </summary>
		/// <remarks>
		/// This flag tells the tag parser what to do with this frame if it is
		/// unknown and the file, excluding the tag, is altered. This does not
		/// apply when the audio is completely replaced with other audio data.
		/// 
		/// 0     Frame should be preserved.
		/// 1     Frame should be discarded.
		/// </remarks>
		Preserve = 0x2000,
		/// <summary>
		/// Read only
		/// </summary>
		/// <remarks>
		/// This flag, if set, tells the software that the contents of this
		/// frame are intended to be read only. Changing the contents might
		/// break something, e.g. a signature. If the contents are changed,
		/// without knowledge of why the frame was flagged read only and
		/// without taking the proper means to compensate, e.g. recalculating
		/// the signature, the bit MUST be cleared.
		/// </remarks>
		ReadOnly = 0x1000,
		/// <summary>
		/// Grouping identity
		/// </summary>
		/// <remarks>
		/// This flag indicates whether or not this frame belongs in a group
		/// with other frames. If set, a group identifier byte is added to the
		/// frame. Every frame with the same group identifier belongs to the
		/// same group.
		/// 
		/// 0     Frame does not contain group information
		/// 1     Frame contains group information
		/// </remarks>
		Grouping = 0x0040,
		/// <summary>
		/// Compression
		/// </summary>
		/// <remarks>
		/// This flag indicates whether or not the frame is compressed.
		/// A 'Data Length Indicator' byte MUST be included in the frame.
		/// 0     Frame is not compressed.
		/// 1     Frame is compressed using zlib [zlib] deflate method.
		///       If set, this requires the 'Data Length Indicator' bit
		///       to be set as well.
		/// </remarks>
		Compressed = 0x0008,
		/// <summary>
		/// Encryption
		/// </summary>
		/// <remarks>
		/// This flag indicates whether or not the frame is encrypted. If set,
		/// one byte indicating with which method it was encrypted will be
		/// added to the frame. See description of the ENCR frame for more
		/// information about encryption method registration. Encryption
		/// should be done after compression. Whether or not setting this flag
		/// requires the presence of a 'Data Length Indicator' depends on the
		/// specific algorithm used.
		///
		/// 0     Frame is not encrypted.
		/// 1     Frame is encrypted.
		/// </remarks>
		Encrypted = 0x0004,
		/// <summary>
		/// Unsynchronisation
		/// </summary>
		/// <remarks>
		/// This flag indicates whether or not unsynchronisation was applied
		/// to this frame. See section 6 for details on unsynchronisation.
		/// If this flag is set all data from the end of this header to the
		/// end of this frame has been unsynchronised. Although desirable, the
		/// presence of a 'Data Length Indicator' is not made mandatory by
		/// unsynchronisation.
		/// 
		/// 0     Frame has not been unsynchronised.
		/// 1     Frame has been unsyrchronised.
		/// </remarks>
		Unsynced = 0x0002,
		/// <summary>
		/// Data length indicator
		/// </summary>
		/// <remarks>
		/// This flag indicates that a data length indicator has been added to
		/// the frame. The data length indicator is the value one would write
		/// as the 'Frame length' if all of the frame format flags were
		/// zeroed, represented as a 32 bit synchsafe integer.
		/// 
		/// 0      There is no Data Length Indicator.
		/// 1      A data length Indicator has been added to the frame.
		/// </remarks>
		LenIndicated = 0x0001
	}

	/// <summary>
	/// ID3v2 frame
	/// </summary>
	/// <remarks>
	/// All ID3v2 frames consists of one frame header followed by one or more
	/// fields containing the actual information. The header is always 10
	/// bytes and laid out as follows:
	/// 
	///   Frame ID      $xx xx xx xx  (four characters)
	///   Size      4 * %0xxxxxxx
	///   Flags         $xx xx
	/// </remarks>
	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public abstract class ID3v2Frame
	{
		private static readonly ILog log = Logger.GetLogger(typeof(ID3v2Frame));
		protected static byte[] EmptyData = new byte[0];

		private TagID TagID;

		/// <summary>
		/// The frame ID is made out of the characters capital A-Z and 0-9.
		/// Identifiers beginning with "X", "Y" and "Z" are for experimental
		/// frames and free for everyone to use, without the need to set the
		/// experimental bit in the tag header. Bear in mind that someone else
		/// might have used the same identifier as you. All other identifiers are
		/// either used or reserved for future use.
		/// </summary>
		[XmlAttribute("ID")]
		public string Namestr { get { return (string)this.TagID; } set { this.TagID = (TagID)value; } }
		/// <summary>
		/// The frame ID is followed by a size descriptor containing the size of
		/// the data in the final frame, after encryption, compression and
		/// unsynchronisation. The size is excluding the frame header ('total
		/// frame size' - 10 bytes) and stored as a 32 bit synchsafe integer.
		/// </summary>
		//[XmlIgnore]
		//public int Length { get; set; } //this is the real length, not a syncsafe int; note: does not include frame ID (like 'TIT2', 'TCO' - 3or4 bytes) or frame flags (2bytes)
		/// <summary>
		/// In the frame header the size descriptor is followed by two flag
		/// bytes.
		/// </summary>
		[XmlAttribute, DefaultValue(FrameFlags.None)]
		public FrameFlags Flags { get; set; }

		//these next 2 values can be potentially be stored based on bitsetting in frame flags;
		[XmlAttribute, DefaultValue((byte)0)]
		public byte GroupingSymbol { get; set; }
		//[XmlIgnore]
		//public int ExpandedLength { get; set; }

		/// <summary>
		/// Major version
		/// </summary>
		[XmlIgnore]
		public int MajorVersion
		{
			get { return this.binOpt.MajorVersion.Value; }
			set { this.binOpt.MajorVersion = value; }
		}
		/// <summary>
		/// Revision number
		/// </summary>
		[XmlIgnore]
		public int RevisionVersion
		{
			get { return this.binOpt.RevisionVersion.Value; }
			set { this.binOpt.RevisionVersion = value; }
		}

		[XmlIgnore]
		public FrameIDs ID { get; set; }
		[XmlIgnore]
		public bool EliminateFrame { get; set; }

		[XmlAttribute, DefaultValue("")]
		public string Hint
		{
			get { return this.xmlOpt.DisplayHint ? Definitions.KnownFrames[ID].FrameDescription : String.Empty; }
			set { }
		}

		protected ID3v2Tag.SerializationOptions binOpt;
		internal ID3v2Tag.XMLSerializationOptions xmlOpt;

		public abstract FrameType Type { get; }
		internal abstract IID3v2Field[] Fields { get; }
		internal abstract void SetFieldData(FieldTypes field, byte[] data);

		#region Binary serialization
		public void Read(BinaryReader reader)
		{
			//byte[] id = reader.ReadBytes(4);
			//this.Namestr = Encoding.ASCII.GetString(id);
			int length = reader.ReadSize(this.MajorVersion);
			int expandedLength = 0;

			if (this.MajorVersion >= 3)
			{
				this.Flags = (FrameFlags)reader.ReadUInt16(); //v2.2 doesn't have frame level flags (but it does have field level flags)

				if ((this.Flags & FrameFlags.Unsynced) == FrameFlags.Unsynced)
				{
					//DE-UNSYNC frame
					int fullframesize = length;
					var mem = new MemoryStream(fullframesize);
					Desynchronize(reader.BaseStream, mem, fullframesize);
					length = (int)mem.Position;
					mem.Seek(0L, SeekOrigin.Begin);
					reader = new BinReader(mem, false);
					this.Flags -= FrameFlags.Unsynced;
				}

				//info based on frame flags (order based on the order of flags defined by the frame flags
				if ((this.Flags & FrameFlags.Grouping) == FrameFlags.Grouping) {
					log.Debug("Frame {0} has a grouping flag set", this.Namestr);
					this.GroupingSymbol = reader.ReadByte(); //er, uh... wouldn't this also require ID32_FRAMEFLAG_LENINDICATED to be set???
				}

				if ((this.Flags & FrameFlags.Compressed) == FrameFlags.Compressed) { // technically ID32_FRAMEFLAG_LENINDICATED should also be tested
					log.Debug("Frame {0} has a compressed flag set", this.Namestr);
					expandedLength = reader.ReadSize(this.MajorVersion);
				}
			}

			int frameLen;
			if (expandedLength != 0)
			{
				using (var decompress = new System.IO.Compression.DeflateStream(reader.BaseStream, System.IO.Compression.CompressionMode.Decompress, true))
				{
					var mem = new MemoryStream(length);
					decompress.WriteTo(mem, expandedLength);
					mem.Seek(0L, SeekOrigin.Begin);
					frameLen = expandedLength;
					reader = new BinReader(mem, false);
				}
			}
			else
			{
				frameLen = length;
			}

			ScanID3Frame(reader, frameLen);
		}

		public void Write(BinaryWriter writer)
		{
			if (this.TagID.IsEmpty || this.TagID.Name.Length != 4)
				throw new NotSupportedException(String.Format("Unknown frame ID [{0}]", this.Namestr));

			//this won't be able to convert from 1 tag version to another because it doesn't look up the frame id strings for the change
			writer.Write(this.TagID);
			int length = this.GetFrameLength();
			int expandedLength = 0;
			MemoryStream compressed = null;
			if (this.MajorVersion >= 3 && (this.Flags & FrameFlags.Compressed) == FrameFlags.Compressed)
			{
				var mem = new MemoryStream(length);
				var writer2 = new BinaryWriter(mem, binOpt.Latin1);
				RenderID3Frame(writer2, length);
				compressed = new MemoryStream(length);
				using (var compress = new System.IO.Compression.DeflateStream(compressed, System.IO.Compression.CompressionMode.Compress, true))
				{
					mem.Seek(0L, SeekOrigin.Begin);
					mem.WriteTo(compress);
				}
				expandedLength = length;
				length = (int)compressed.Position;
			}
			writer.WriteSize(this.MajorVersion, length);
			switch (this.MajorVersion)
			{
			case 3:
			case 4:
				//render frame flags //TODO: compression & group symbol are the only ones that can possibly be set here
				writer.Write((ushort)this.Flags);
				//grouping flag? 1 byte; technically, its outside the header and before the fields begin
				if ((this.Flags & FrameFlags.Grouping) == FrameFlags.Grouping)
					writer.Write(this.GroupingSymbol);
				break;
			}
			//compression flag? 4bytes; technically, its outside the header and before the fields begin
			if (compressed != null)
			{
				writer.WriteSize(this.MajorVersion, expandedLength);
				compressed.Seek(0L, SeekOrigin.Begin);
				compressed.WriteTo(writer.BaseStream);
			}
			else
			{
				RenderID3Frame(writer, length);
			}
		}

		private void Desynchronize(Stream src, Stream dst, int bufferlen)
		{
			for (; bufferlen > 0; bufferlen--)
			{
				int b = src.ReadByte();
				if (b < 0) return;
				dst.WriteByte((byte)b);
				if (b == 0xFF && bufferlen > 1)
				{
					b = src.ReadByte();
					if (b < 0) return;
					if (b != 0x00)
						dst.WriteByte((byte)b);
				}
			}
		}
		#endregion

		#region XML serialization
		public sealed class DataXMLSerializer: IXmlSerializable
		{
			public byte[] Data { get; private set; }
			private const string TypeXMLAttr = "Type";
			private const string TypeBase64XMLEnum = "Base64";
			private const string TypeHexXMLEnum = "Hex";
			private const string TypeGuidXMLEnum = "GUID";
			private const string TypeUnicodeXMLEnum = "Unicode";

			public DataXMLSerializer(byte[] data)
			{
				this.Data = data;
			}

			public DataXMLSerializer() { }

			System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
			{
				return null;
			}

			void IXmlSerializable.ReadXml(XmlReader reader)
			{
				reader.MoveToContent();
				if (reader.NodeType != XmlNodeType.Element)
					return;
				string type = reader.GetAttribute(TypeXMLAttr);
				switch (type)
				{
					case TypeGuidXMLEnum:
						string guid = reader.ReadContentAsString();
						this.Data = XmlConvert.ToGuid(guid).ToByteArray();
						return;
					case TypeUnicodeXMLEnum:
						string str = reader.ReadContentAsString();
						this.Data = Encoding.Unicode.GetBytes(str);
						return;
				}
				var mem = new MemoryStream();
				var buffer = new byte[8192];
				int read;
				switch (type)
				{
					case TypeBase64XMLEnum:
						while ((read = reader.ReadContentAsBase64(buffer, 0, 8192)) != 0)
							mem.Write(buffer, 0, read);
						break;
					case TypeHexXMLEnum:
						while ((read = reader.ReadContentAsBinHex(buffer, 0, 8192)) != 0)
							mem.Write(buffer, 0, read);
						break;
				}
				this.Data = mem.ToArray();
			}

			void IXmlSerializable.WriteXml(XmlWriter writer)
			{
				if (this.Data.Length == 16)
				{
					var guid = new Guid(this.Data);
					writer.WriteAttributeString(TypeXMLAttr, TypeGuidXMLEnum);
					writer.WriteString(XmlConvert.ToString(guid));
				}
				else if (IsUnicode(this.Data))
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeUnicodeXMLEnum);
					writer.WriteString(Encoding.Unicode.GetString(this.Data, 0, this.Data.Length - 2));
				}
				else if (this.Data.Length > 256)
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeBase64XMLEnum);
					writer.WriteBase64(this.Data, 0, this.Data.Length);
				}
				else
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeHexXMLEnum);
					writer.WriteBinHex(this.Data, 0, this.Data.Length);
				}
			}

			private static bool IsUnicode(byte[] data)
			{
				if (data.Length < 2 || (data.Length % 2) != 0 || data[data.Length - 1] != 0 || data[data.Length - 2] != 0)
					return false;
				for (int k = 0; k < data.Length - 2; k += 2)
				{
					if (data[k] < 32 && data[k + 1] == 0)
						return false;
				}
				return true;
			}
		}
		#endregion

		internal byte[] this[FieldTypes field]
		{
			get
			{
				return this.Fields.First(f => f.Type == field).Data;
			}
			set
			{
				this.SetFieldData(field, value);
			}
		}

		protected string TextSeparator
		{
			get
			{
				switch (this.MajorVersion)
				{
				case 4:
					return "\0";
				case 3:
					switch (this.ID)
					{
					case FrameIDs.Lyricist:
					case FrameIDs.Composer:
					case FrameIDs.OrigWriter:
					case FrameIDs.OrigArtist:
					case FrameIDs.Artist:
						return "/";
					default:
						return null;
					}
				default:
					return null;
				}
			}
		}

		#region id3 parsing functions

		private byte[] ExtractField(BinaryReader reader, int maxFieldLen, FieldTypes fieldType, TextEncodings encoding)
		{
			switch (fieldType)
			{
			case FieldTypes.Unknown:
				//the difference between this unknown field & say a binary data field is the unknown field is always the first (and only) field
			case FieldTypes.Text:
			case FieldTypes.URL:
			case FieldTypes.Counter:
			case FieldTypes.BinaryData:
				//this class of fields may contains NULLs but is *NOT* NULL terminated in any form
				return reader.ReadBytes(maxFieldLen);
			case FieldTypes.PicType:
			case FieldTypes.GroupSymbol:
			case FieldTypes.TextEncoding:
				return new byte[] { reader.ReadByte() };
			case FieldTypes.Language:
			case FieldTypes.ImageFormat:
				return reader.ReadBytes(3);
			case FieldTypes.MIMEType:
			case FieldTypes.Owner:
			//difference between ID3_OWNER_FIELD & ID3_DESCRIPTION_FIELD field classes is the owner field is always 8859-1 encoded (single NULL term)
			case FieldTypes.Filename:
			case FieldTypes.Description:
				var buf = reader.ReadBytes(maxFieldLen);
				int len, pos;
				switch (fieldType)
				{
				case FieldTypes.MIMEType:
				case FieldTypes.Owner:
					encoding = TextEncodings.Latin1;
					break;
				}
				switch (encoding)
				{
				case TextEncodings.Latin1:
				case TextEncodings.UTF8:
					len = Array.IndexOf(buf, (byte)0);
					pos = len + 1;
					break;
				case TextEncodings.UTF16BENoBOM:
				case TextEncodings.UTF16LEWithBOM:
					len = -1;
					for(int k = 0; k < buf.Length-1; k+=2)
						if (buf[k] == 0 && buf[k + 1] == 0)
						{
							len = k;
							break;
						}
					pos = len + 2;
					break;
				default:
					throw new ArgumentException(String.Format("Unknown encoding {0}", encoding));
				}
				if (len < 0)
				{
					return buf;
				}
				else
				{
					var data = new byte[len];
					Array.Copy(buf, data, len);
					reader.BaseStream.Seek(pos - buf.Length, SeekOrigin.Current);
					return data;
				}
			}
			throw new ArgumentException(String.Format("Unknown field type {0}", fieldType));
			//fprintf(stdout, "%" PRIu32 ", %s, %s\n", bytes_used, buffer, (thisFrame->ID3v2_Frame_Fields+fieldNum)->field_string);
		}

		private void ScanID3Frame(BinaryReader reader, int frameLen)
		{
			long stop = reader.BaseStream.Position + frameLen;
			TextEncodings encoding = TextEncodings.Latin1;
			foreach (var field in this.Fields)
			{
				int maxFieldLen = (int)(stop - reader.BaseStream.Position);
				if (maxFieldLen > 0)
				{
					var data = ExtractField(reader, maxFieldLen, field.Type, encoding);
					this.SetFieldData(field.Type, data);
					if (field.Type == FieldTypes.TextEncoding)
						encoding = ((ID3v2Fields.TextEncodingField)field).TextEncoding;
				}
				else
				{
					log.Warn("Unexpected end of the frame {0}", this.Namestr);
					this.SetFieldData(field.Type, new byte[0]);
				}
			}
		}

		public static ID3v2Frame Create(ID3FrameDefinition frame, TagID tag, ID3v2Tag.SerializationOptions options)
		{
			var res = Definitions.FrameTypeConstruction[frame.FrameType].CreateFrame();
			res.ID = frame.InternalFrameID;
			res.TagID = tag;
			res.binOpt = options;
			return res;
		}
		#endregion

		#region id3 rendering functions
		private void RenderField(BinaryWriter writer, byte[] data, FieldTypes fieldType, TextEncodings encoding)
		{
			//fprintf(stdout, "Total Fields for %s: %u (this is %u, %u)\n", id3v2_frame->ID3v2_Frame_Namestr, id3v2_frame->ID3v2_FieldCount, fld_idx, this_field->ID3v2_Field_Type);
			switch (fieldType)
			{
			//these are raw data fields of variable/fixed length and are not NULL terminated
			case FieldTypes.Unknown:
			case FieldTypes.PicType:
			case FieldTypes.GroupSymbol:
			case FieldTypes.TextEncoding:
			case FieldTypes.Language:
			case FieldTypes.Counter:
			case FieldTypes.ImageFormat:
			case FieldTypes.Text:
			case FieldTypes.URL:
			case FieldTypes.BinaryData:
				writer.Write(data);
				//fprintf(stdout, "Field idx %u(%d) is now %" PRIu32 " bytes long (+%" PRIu32 ")\n", fld_idx, this_field->ID3v2_Field_Type, *frame_length, this_field->field_length);
				break;
			//these are iso 8859-1 encoded with a single NULL terminator
			//a 'LINK' url would also come here and be seperately enumerated (because it has a terminating NULL); but in 3gp assets, external references aren't allowed
			//an 'OWNE'/'COMR' price field would also be here because of single byte NULL termination
			case FieldTypes.MIMEType:
			case FieldTypes.Owner:
				writer.Write(data);
				writer.Write((byte)0);
				//fprintf(stdout, "Field idx %u(%d) is now %" PRIu32 " bytes long\n", fld_idx, this_field->ID3v2_Field_Type, *frame_length);
				break;
			//these fields are subject to NULL byte termination - based on what the text encoding field says the encoding of this string is
			case FieldTypes.Filename:
			case FieldTypes.Description:
				writer.Write(data);
				switch (encoding)
				{
				case TextEncodings.Latin1:
				case TextEncodings.UTF8:
					writer.Write((byte)0);
					break;
				case TextEncodings.UTF16BENoBOM:
				case TextEncodings.UTF16LEWithBOM:
					writer.Write((ushort)0);
					break;
				default:
					throw new ArgumentException(String.Format("Unknown encoding {0}", encoding));
				}
				//fprintf(stdout, "Field idx %u(%d) is now %" PRIu32 " bytes long\n", fld_idx, this_field->ID3v2_Field_Type, *frame_length);
				break;
			default:
				throw new ArgumentException(String.Format("Unknown field type {0}", fieldType));
			}
		}

		private void RenderID3Frame(BinaryWriter writer, int length)
		{
			long pos = writer.BaseStream.Position;
			TextEncodings encoding = TextEncodings.Latin1;
			foreach (var field in this.Fields)
			{
				if (field.Type == FieldTypes.TextEncoding)
					encoding = ((ID3v2Fields.TextEncodingField)field).TextEncoding;
				var data = field.Data;
				RenderField(writer, data, field.Type, encoding);
			}
			if (writer.BaseStream.Position - pos != length)
				throw new InvalidDataException(String.Format("Expected frame length is {0} but writen length is {0}", length, writer.BaseStream.Position - pos));
		}

		internal int GetFrameLength()
		{
			int size, len;
			GetFrameDim(out size, out len);
			return len;
		}

		internal int GetFrameSize()
		{
			int size, len;
			GetFrameDim(out size, out len);
			return size;
		}

		private void GetFrameDim(out int frame_size, out int frame_len)
		{
			frame_len = 0;
			switch (this.MajorVersion)
			{
			case 2:
				frame_size = 6; //3bytes frameID, 3bytes frame length (24bit int)
				break;
			default:
				frame_size = 10; //4bytes frameID, 4bytes frame length (int or syncsafe int), 2bytes flags
				if ((this.Flags & FrameFlags.Grouping) == FrameFlags.Grouping)
					frame_size++; //optional group symbol
				if ((this.Flags & FrameFlags.Compressed) == FrameFlags.Compressed)
					frame_size += 4; //decompressed length
				break;
			}

			TextEncodings encoding = TextEncodings.Latin1;
			foreach (var field in this.Fields)
			{
				if (field.Type == FieldTypes.TextEncoding)
					encoding = ((ID3v2Fields.TextEncodingField)field).TextEncoding;
				frame_len += field.Data.Length;
				switch (field.Type)
				{
				//these are raw data fields of variable/fixed length and are not NULL terminated
				case FieldTypes.Unknown:
				case FieldTypes.PicType:
				case FieldTypes.GroupSymbol:
				case FieldTypes.TextEncoding:
				case FieldTypes.Language:
				case FieldTypes.Counter:
				case FieldTypes.ImageFormat:
				case FieldTypes.Text:
				case FieldTypes.URL:
				case FieldTypes.BinaryData:
					break;
				//these are iso 8859-1 encoded with a single NULL terminator
				//a 'LINK' url would also come here and be seperately enumerated (because it has a terminating NULL); but in 3gp assets, external references aren't allowed
				//an 'OWNE'/'COMR' price field would also be here because of single byte NULL termination
				case FieldTypes.MIMEType:
				case FieldTypes.Owner:
					frame_len++;
					break;
				//these fields are subject to NULL byte termination - based on what the text encoding field says the encoding of this string is
				case FieldTypes.Filename:
				case FieldTypes.Description:
					switch (encoding)
					{
					case TextEncodings.Latin1:
					case TextEncodings.UTF8:
						frame_len++;
						break;
					case TextEncodings.UTF16BENoBOM:
					case TextEncodings.UTF16LEWithBOM:
						frame_len += 2;
						break;
					default:
						throw new InvalidOperationException(String.Format("Unknown encoding {0}", encoding));
					}
					break;
				default:
					throw new InvalidOperationException(String.Format("Unknown field type {0}", field.Type));
				}
			}
			frame_size += frame_len;
		}
		#endregion
	}
}
