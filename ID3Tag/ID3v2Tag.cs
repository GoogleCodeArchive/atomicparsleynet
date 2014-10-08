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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using FRAFV.Binary.Serialization;

namespace ID3v2
{
	/// <summary>
	/// ID3v2 flags
	/// </summary>
	/// <remarks>
	/// All the other flags MUST be cleared. If one of these undefined flags
	/// are set, the tag might not be readable for a parser that does not
	/// know the flags function.
	/// </remarks>
	[Flags]
	public enum TagFlags
	{
		None = 0,
		BIT0 = 0x01,
		BIT1 = 0x02,
		BIT2 = 0x04,
		BIT3 = 0x08,
		/// <summary>
		/// Footer present
		/// </summary>
		/// <remarks>
		/// Bit 4 indicates that a footer (section 3.4) is present at the very
		/// end of the tag. A set bit indicates the presence of a footer.
		/// </remarks>
		Footer = 0x10,
		/// <summary>
		/// Experimental indicator
		/// </summary>
		/// <remarks>
		/// The third bit (bit 5) is used as an 'experimental indicator'. This
		/// flag SHALL always be set when the tag is in an experimental stage.
		/// </remarks>
		Experimental = 0x20,
		/// <summary>
		/// Extended header
		/// </summary>
		/// <remarks>
		/// The second bit (bit 6) indicates whether or not the header is
		/// followed by an extended header. A set bit indicates the presence of 
		/// an extended header.
		/// </remarks>
		ExtendedHeader = 0x40,
		/// <summary>
		/// Unsynchronisation
		/// </summary>
		/// <remarks>
		/// Bit 7 in the 'ID3v2 flags' indicates whether or not
		/// unsynchronisation is applied on all frames; a set bit indicates usage.
		/// </remarks>
		Unsyncronization = 0x80
	}

	/// <summary>
	/// ID3v2 header
	/// </summary>
	/// <remarks>
	/// The first part of the ID3v2 tag is the 10 byte tag header, laid out
	/// as follows:
	/// 
	///   ID3v2/file identifier      "ID3"
	///   ID3v2 version              $04 00
	///   ID3v2 flags                %abcd0000
	///   ID3v2 size             4 * %0xxxxxxx
	/// 
	///  An ID3v2 tag can be detected with the following pattern:
	///    $49 44 33 yy yy xx zz zz zz zz
	///  Where yy is less than $FF, xx is the 'flags' byte and zz is less than
	///  $80.
	/// </remarks>
	[XmlRoot("Tag", Namespace = XMLNS, IsNullable = false)]
	public class ID3v2Tag
	{
		private static readonly ILog log = Logger.GetLogger(typeof(ID3v2Tag));
		internal const string XMLNS = "urn:schemas-id3-org:id3v2";
		public const string SignatureStr = "ID3";

		private static readonly byte[] Signature = Encoding.ASCII.GetBytes(SignatureStr);

		[XmlAttribute("Signature")]
		public string SignatureSerializer
		{
			get { return SignatureStr; }
			set { }
		}
		/// <summary>
		/// ID3v2 version
		/// </summary>
		/// <remarks>
		/// The first three bytes of the tag are always "ID3", to indicate that
		/// this is an ID3v2 tag, directly followed by the two version bytes. The
		/// first byte of ID3v2 version is its major version, while the second
		/// byte is its revision number. In this case this is ID3v2.4.0. All
		/// revisions are backwards compatible while major versions are not. If
		/// software with ID3v2.4.0 and below support should encounter version
		/// five or higher it should simply ignore the whole tag. Version or
		/// revision will never be $FF.
		/// </remarks>
		[XmlAttribute("Version")]
		public string VersionAsString
		{
			get { return this.binOpt.Version.ToString(); }
			set { this.binOpt.Version = System.Version.Parse(value); }
		}
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
		/// <summary>
		/// ID3v2 flags
		/// </summary>
		/// <remarks>
		/// The version is followed by the ID3v2 flags field, of which currently
		/// four flags are used.
		/// </remarks>
		[XmlAttribute, DefaultValue(TagFlags.None)]
		public TagFlags Flags { get; set; }
		/// <summary>
		/// ID3v2 tag size
		/// </summary>
		/// <remarks>
		/// The ID3v2 tag size is stored as a 32 bit synchsafe integer, 
		/// making a total of 28 effective bits (representing up to 256MB).
		/// 
		/// The ID3v2 tag size is the sum of the byte length of the extended
		/// header, the padding and the frames after unsynchronisation. If a
		/// footer is present this equals to ('total size' - 20) bytes, otherwise
		/// ('total size' - 10) bytes.
		/// </remarks>
		[XmlAttribute, DefaultValue(0)]
		public int Length { get; set; } //this is a bonafide uint_32_t length, not a syncsafe int

		[XmlIgnore]
		public int TotalSize
		{
			get { return (this.Flags & TagFlags.Footer) == TagFlags.Footer ? this.Length + 20 : this.Length + 10; }
			set { this.Length = (this.Flags & TagFlags.Footer) == TagFlags.Footer ? value - 20 : value - 10; }
		}

		/// <summary>
		/// Extended header
		/// </summary>
		/// <remarks>
		/// this extended header section depends on a bitsetting in Flags
		/// </remarks>
		[XmlIgnore]
		public int ExtendedHeaderLength { get; set; } //the entire extended header section is unimplemented flags & flag frames

		[XmlIgnore]
		public ICollection<ID3v2Frame> FrameList { get; private set; }

		[XmlElement(typeof(ID3v2.Frames.UnknownFrame))]
		[XmlElement(typeof(ID3v2.Frames.TextFrame))]
		[XmlElement(typeof(ID3v2.Frames.TextFrameUserDef))]
		[XmlElement(typeof(ID3v2.Frames.URLFrame))]
		[XmlElement(typeof(ID3v2.Frames.URLFrameUserDef))]
		[XmlElement(typeof(ID3v2.Frames.UniqueFileIDFrame))]
		[XmlElement(typeof(ID3v2.Frames.CDIDFrame))]
		[XmlElement(typeof(ID3v2.Frames.DescribedTextFrame))]
		[XmlElement(typeof(ID3v2.Frames.AttachedPictureFrame))]
		[XmlElement(typeof(ID3v2.Frames.AttachedObjectFrame))]
		[XmlElement(typeof(ID3v2.Frames.GroupIDFrame))]
		[XmlElement(typeof(ID3v2.Frames.SignatureFrame))]
		[XmlElement(typeof(ID3v2.Frames.PrivateFrame))]
		[XmlElement(typeof(ID3v2.Frames.PlayCounterFrame))]
		[XmlElement(typeof(ID3v2.Frames.PopularFrame))]
		[XmlElement(typeof(ID3v2.Frames.OldV2P2PictureFrame))]
		public ID3v2Frame[] FrameListSerializer
		{
			get {
				int k = 0;
				var res = new ID3v2Frame[this.FrameList.Count];
				foreach (var frame in this.FrameList)
				{
					frame.xmlOpt = this.xmlOpt;
					res[k++] = frame;
				}
				return res.ToArray();
			}
			set { this.FrameList = new List<ID3v2Frame>(value); }
		}

		[XmlIgnore]
		public bool ModifiedTag { get; set; }

		public static readonly XmlSerializerNamespaces DefaultXMLNamespaces = new XmlSerializerNamespaces(
			new XmlQualifiedName[] { new XmlQualifiedName(String.Empty, XMLNS) });

		public ID3v2Tag()
		{
			this.FrameList = new List<ID3v2Frame>();
		}

		private SerializationOptions binOpt;
		private XMLSerializationOptions xmlOpt;

		[XmlIgnore]
		public SerializationOptions SerializerOptions { get { return this.binOpt.Clone(); } }
		[XmlIgnore]
		public XMLSerializationOptions XMLSerializerOptions
		{
			get { return this.xmlOpt.Clone(); }
			set { this.xmlOpt = value.Clone(); }
		}

		public sealed class SerializationOptions: ICloneable
		{
			public int? MajorVersion { get; set; }
			public int? RevisionVersion { get; set; }
			public Version Version
			{
				get
				{
					return (MajorVersion.HasValue && RevisionVersion.HasValue) ?
						new Version(2, MajorVersion.Value, RevisionVersion.Value) : new Version();
				}
				set
				{
					MajorVersion = value.Build >= 0 ? value.Minor : (int?)null;
					RevisionVersion = value.Build >= 0 ? value.Build : (int?)null;
				}
			}
			public Encoding Latin1 { get; set; }

			object ICloneable.Clone()
			{
				return Clone();
			}

			public SerializationOptions Clone()
			{
				return new SerializationOptions
				{
					MajorVersion = MajorVersion,
					RevisionVersion = RevisionVersion,
					Latin1 = Latin1
				};
			}
		}

		public class XMLSerializationOptions: ICloneable
		{
			public bool DisplayHint { get; set; }

			object ICloneable.Clone()
			{
				return Clone();
			}

			public XMLSerializationOptions Clone()
			{
				return new XMLSerializationOptions
				{
					DisplayHint = DisplayHint
				};
			}
		}

		public static ID3v2Tag Create(Stream stream, SerializationOptions options)
		{
			if (stream.Length - stream.Position < 20L)
				return null;
			var buf = new byte[4];
			if (stream.Read(buf, 0, 3) != 3 ||
				buf[0] != Signature[0] || buf[1] != Signature[1] || buf[2] != Signature[2])
				return null;
			int majorVersion = stream.ReadByte();
			int revisionVersion = stream.ReadByte();
			if (majorVersion < 0 || revisionVersion < 0) return null;
			var flags = (TagFlags)stream.ReadByte();

			if (options.MajorVersion.HasValue && majorVersion != options.MajorVersion.Value)
			{
				log.Warn("An ID32 atom was encountered using an unsupported ID3v2 tag version: {0}. Skipping", majorVersion);
				return null;
			}
			if (options.RevisionVersion.HasValue && revisionVersion != options.RevisionVersion.Value)
			{
				log.Warn("An ID32 atom was encountered using an unsupported ID3v2.4 tag revision: {0}. Skipping", revisionVersion);
				return null;
			}

			if ((flags & (TagFlags.BIT0 | TagFlags.BIT1 | TagFlags.BIT2 | TagFlags.BIT3)) != 0)
				return null;

			if ((flags & TagFlags.Footer) == TagFlags.Footer)
			{
				log.Error("An ID32 atom was encountered with a forbidden footer flag. Skipping.");
				return null;
			}

			if ((flags & TagFlags.Experimental) == TagFlags.Experimental)
			{
				log.Debug("An ID32 atom was encountered with an experimental flag set.");
			}

			if ((flags & TagFlags.Unsyncronization) == TagFlags.Unsyncronization)
			{
				log.Error("An ID3 tag with the unsynchronized flag set which is not supported. Skipping.");
				return null;
			}

			if (stream.Read(buf, 0, 4) != 4) return null;
			int length = buf.ToSynchSafeInt32(0);

			var binOpt = options.Clone();
			binOpt.MajorVersion = majorVersion;
			binOpt.RevisionVersion = revisionVersion;
			if (binOpt.Latin1 == null)
				binOpt.Latin1 = Encoding.ASCII;

			var tag = new ID3v2Tag()
			{
				binOpt = binOpt,
				Flags = flags,
				Length = length
			};
			
			var mem = new MemoryStream(tag.Length);
			stream.WriteTo(mem, tag.Length);
			var reader = new BinReader(mem, binOpt.Latin1, false);
			mem.Seek(0L, SeekOrigin.Begin);
			tag.Read(reader);
			return tag;
		}

		private void Read(BinaryReader reader)
		{
			if ((this.Flags & TagFlags.ExtendedHeader) == TagFlags.ExtendedHeader && this.MajorVersion >= 3)
			{
				long pos = reader.BaseStream.Position;
				this.ExtendedHeaderLength = reader.ReadSize(this.MajorVersion); //2.2 doesn't have it
				reader.BaseStream.Seek(pos + this.ExtendedHeaderLength, SeekOrigin.Begin);
			}
			else
				this.ExtendedHeaderLength = 0;

			ScanID3Tag(reader);
		}

		public void Write(Stream stream)
		{
			FrameFilter();

			int tag_size = this.Length + 10;
			var mem = new MemoryStream(tag_size);
			mem.Write(Signature, 0, 3);
			mem.WriteByte((byte)this.MajorVersion);
			mem.WriteByte((byte)this.RevisionVersion);
			mem.WriteByte((byte)this.Flags);

			mem.Write(this.Length.GetSynchSafeBytes(), 0, 4);
			var writer = new BinWriter(mem, false);
			if (!RenderID3Tag(writer))
				return;
			if (mem.Position > tag_size)
				throw new InvalidDataException(String.Format("Expected tag length is {0} but writen length is {0}", tag_size, mem.Position));
			int padding = tag_size - (int)mem.Position;
			mem.Seek(0L, SeekOrigin.Begin);
			mem.WriteTo(stream);
			if (padding == 0)
				return;
			var buf = new byte[padding];
			Array.Clear(buf, 0, buf.Length);
			stream.Write(buf, 0, buf.Length);
		}

		#region id3 parsing functions

		private void ScanID3Tag(BinaryReader reader)
		{
			while (reader.BaseStream.Length - reader.BaseStream.Position > 14L)
			{
				TagID frameID;
				switch (this.MajorVersion)
				{
				case 2:
					frameID = reader.ReadTagID(3);
					break;
				default:
					frameID = reader.ReadTagID(4);
					break;
				}
				if (frameID.IsEmpty || frameID.NonConformance)
					break;
				var def = Definitions.MatchID3FrameIDstr(frameID, this.MajorVersion);
				var frame = ID3v2Frame.Create(def, frameID, this.binOpt);
				FrameList.Add(frame);
				frame.Read(reader);
			}
		}

		#endregion

		#region id3 rendering functions

		private bool LocateFrameSymbol(ID3v2Frame targetFrame, byte groupsymbol)
		{
			if (targetFrame.ID == FrameIDs.GrID)
			{
				return FrameList.Any(testFrame => testFrame.ID != FrameIDs.GrID && testFrame.GroupingSymbol == groupsymbol);
			}
			else
			{
				return FrameList.Any(testFrame => testFrame.ID == FrameIDs.GrID && groupsymbol == ((ID3v2.Frames.GroupIDFrame)testFrame).GroupSymbol);
			}
		}

		private void FrameFilter()
		{
			ID3v2Frame MCDI_frame = null;
			ID3v2Frame TRCK_frame = null;
			foreach (ID3v2Frame thisFrame in FrameList)//FirstFrame;
			{
				var thisCDID = thisFrame as ID3v2.Frames.CDIDFrame;
				var thisGrid = thisFrame as ID3v2.Frames.GroupIDFrame;
				var thisSig = thisFrame as ID3v2.Frames.SignatureFrame;
				if (!thisFrame.EliminateFrame)
				{
					if (thisCDID != null)
					{
						MCDI_frame = thisFrame;
					}
					if (thisFrame.ID == FrameIDs.TrackNum)
					{
						TRCK_frame = thisFrame;
					}
					if (thisGrid != null && thisFrame.ID == FrameIDs.GrID)
					{ //find any frames containing this symbol; if none are present this frame will be discarded
						thisFrame.EliminateFrame = !LocateFrameSymbol(thisFrame, thisGrid.GroupSymbol);
						if (!thisFrame.EliminateFrame)
						{
							thisFrame.Flags |= FrameFlags.Grouping;
						}

					}
					else if (thisSig != null && thisFrame.ID == FrameIDs.Signature)
					{ //find a GRID frame that contains this symbol (@ field_string, not ID3v2_Frame_GroupingSymbol)
						thisFrame.EliminateFrame = !LocateFrameSymbol(thisFrame, thisSig.GroupSymbol);
						//since the group symbol is carried as a field for SIGN, no need to set the frame's grouping bit in the frame flags

					}
					else if (thisFrame.GroupingSymbol > 0)
					{ //find a GRID frame that contains this symbol, otherwise discard it
						thisFrame.EliminateFrame = !LocateFrameSymbol(thisFrame, thisFrame.GroupingSymbol);
						if (!thisFrame.EliminateFrame)
						{
							thisFrame.Flags |= FrameFlags.Grouping;
						}

					}
				}
			}

			if (MCDI_frame != null && TRCK_frame == null)
			{
				log.Warn("The MCDI frame was skipped due to a missing TRCK frame");
				MCDI_frame.EliminateFrame = true;
			}
		}

		public int GetTagSize()
		{ // a rough approximation of how much to malloc; this will be larger than will be ultimately required
			int tag_size = 10 + this.ExtendedHeaderLength; //3bytes signature 'ID3', 2bytes version, 1byte flags, 4bytes tag length (syncsafe int), extended header
			int surviving_frame_count = 0;
			//if (ModifiedTag == false) return tag_len;
			if (FrameList.Count == 0) return 0; //but a frame isn't removed by AP; its just marked for elimination
			//if (MajorVersion != 4) return tag_len; //only id3 version 2.4 tags are written

			FrameFilter();

			foreach (ID3v2Frame eval_frame in FrameList)
			{
				if (eval_frame.EliminateFrame == true)
				{
					continue;
				}
				tag_size += eval_frame.GetFrameSize();
				surviving_frame_count++;
			}
			if (surviving_frame_count == 0) return 0; //the 'ID3' header alone isn't going to be written with 0 existing frames
			return tag_size;
		}

		private bool RenderID3Tag(BinaryWriter writer)
		{
			bool contains_rendered_frames = false;
			if ((this.Flags & TagFlags.ExtendedHeader) == TagFlags.ExtendedHeader)
				throw new NotImplementedException("Extended header currently unimplemented");

			foreach (var thisFrame in FrameList)
			{
				if (thisFrame.EliminateFrame == true)
					continue;
				contains_rendered_frames = true;
				thisFrame.Write(writer);
			}
			return contains_rendered_frames;
		}
		#endregion
	}
}
