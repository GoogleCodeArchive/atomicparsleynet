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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ID3v2;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public enum DataFlags
	{
		/// <summary>
		/// Reserved for use where no type needs to be indicated
		/// </summary>
		Binary = 0,
		/// <summary>
		/// UTF-8 — without any count or NULL terminator
		/// </summary>
		UTF8 = 1,
		/// <summary>
		/// UTF-16 — also known as UTF-16BE
		/// </summary>
		UTF16 = 2,
		/// <summary>
		/// S/JIS — deprecated unless it is needed for special Japanese characters
		/// </summary>
		SJIS = 3,
		/// <summary>
		/// UTF-8 sort — variant storage of a string for sorting only
		/// </summary>
		UTF8Sort = 4,
		/// <summary>
		/// UTF-16 sort — variant storage of a string for sorting only
		/// </summary>
		UTF16Sort = 5,
		/// <summary>
		/// HTML — the HTML file header specifies which HTML version
		/// </summary>
		HTML = 6,
		/// <summary>
		/// XML — the XML header must identify the DTD or schemas
		/// </summary>
		XML = 7,
		/// <summary>
		/// UUID — also known as GUID; stored as 16 bytes in binary (valid as an ID)
		/// </summary>
		UUID = 8,
		/// <summary>
		/// ISRC — stored as UTF-8 text (valid as an ID)
		/// </summary>
		ISRC = 9,
		/// <summary>
		/// MI3P — stored as UTF-8 text (valid as an ID)
		/// </summary>
		MI3P = 10,
		/// <summary>
		/// GIF — (deprecated) a GIF image
		/// </summary>
		GIF = 12,
		/// <summary>
		/// JPEG — in a JFIF wrapper
		/// </summary>
		JPEG = 13,
		/// <summary>
		/// PNG — in a PNG wrapper
		/// </summary>
		PNG = 14,
		/// <summary>
		/// URL — absolute, in UTF-8 characters
		/// </summary>
		URL = 15,
		/// <summary>
		/// Duration — in milliseconds, 32-bit integer
		/// </summary>
		Duration = 16,
		/// <summary>
		/// Date/Time — in UTC, counting seconds since midnight, January 1, 1904; 32 or 64-bits
		/// </summary>
		DateTime = 17,
		/// <summary>
		/// Genres — a list of enumerated values
		/// </summary>
		Genres = 18,
		//for cpil, tmpo, rtng; iTMS atoms: cnID, atID, plID, geID, sfID, akID
		/// <summary>
		/// BE Signed Integer — a big-endian signed integer in 1,2,3 or 4 bytes 
		/// </summary>
		Int = 21,
		/// <summary>
		/// BE Unsigned Integer — a big-endian unsigned integer in 1,2,3 or 4 bytes; size of value determines integer size
		/// </summary>
		UInt = 22,
		/// <summary>
		/// BE Float32 — a big-endian 32-bit floating point value (IEEE754)
		/// </summary>
		Single = 23,
		/// <summary>
		/// BE Float64 — a big-endian 64-bit floating point value (IEEE754)
		/// </summary>
		Double = 24,
		/// <summary>
		/// RIAA parental advisory — { -1=no, 1=yes, 0=unspecified }, 8-bit ingteger
		/// </summary>
		RIAAPA = 24,
		/// <summary>
		/// Universal Product Code — in text UTF-8 format (valid as an ID)
		/// </summary>
		UPC = 25,
		/// <summary>
		/// BMP — windows bitmap format graphics
		/// </summary>
		BMP = 27,
		/// <summary>
		/// QuickTimeMetadata atom — a block of data having the structure of the Metadata atom defined in this specification
		/// </summary>
		QTMeta = 28,
		/// <summary>
		/// For uuid atoms that contain files
		/// </summary>
		UUIDBinary = 88
	}

	/// <summary>
	/// These flags indicate how the track is used in the movie.
	/// </summary>
	[Flags]
	public enum TrackFlags
	{
		/// <summary>
		/// Indicates that the track is enabled.
		/// </summary>
		Enabled = 0x0001,
		/// <summary>
		/// Indicates that the track is used in the movie.
		/// </summary>
		Movie = 0x0002,
		/// <summary>
		/// Indicates that the track is used in the movie’s preview.
		/// </summary>
		Preview = 0x0004,
		/// <summary>
		/// Indicates that the track is used in the movie’s poster.
		/// </summary>
		Poster = 0x0008
	}

	[Flags]
	public enum TrackFragmentFlags
	{
		BaseOffset = 0x01,
		SampleDesc = 0x02,
		SampleDur = 0x08,
		SampleSize = 0x10,
		SampleFlags = 0x20,
		DurEmpty = 0x10000,
		MovieFragmentBaseOffset = 0x20000
	}

	[Flags]
	public enum TrackRunFlags
	{
		DataOffset = 0x01,
		FirstFlag = 0x04,
		Duration = 0x100,
		Size = 0x200,
		Flags = 0x400,
		CTSOffset = 0x800
	}

	public enum SampleDependsOnFlags
	{
		Unknown = 0,
		SampleDoesNotDependOnOthers = 2, // ie: an I picture
		SampleDependsOnOthers = 1 // ie: not an I picture
	}

	public enum SampleIsDependedOnFlags
	{
		Unknown = 0,
		NoOtherSampleDependsOnThisSample = 2,  // mediaSampleDroppable
		OtherSamplesDependOnThisSample = 1
	}

	public enum SampleHasRedundancyFlags
	{
		Unknown = 0,
		ThereIsNoRedundantCodingInThisSample = 2,
		ThereIsRedundantCodingInThisSample = 1
	}

	/// <summary>
	/// Box (atom) container
	/// </summary>
	public interface IBoxContainer
	{
		/// <summary>
		/// Children
		/// </summary>
		Collection<AtomicInfo> Boxes { get; }
	}

	public interface IEntry<TOwner>
	{
		TOwner Owner { get; set; }
	}

	public sealed class BOMReader: BinStringReader
	{
		public BOMReader(Stream input, Encoding encoding)
			: base(input, encoding, littleEndian: false)
		{
			AllowDetectBOM = true;
			AllowUnterminatedString = true;
		}
	}

	public sealed class BOMWriter : BinStringWriter
	{
		public BOMWriter(Stream output, Encoding encoding)
			: base(output, encoding, littleEndian: false) { }
	}

	/// <summary>
	/// The basic data unit
	/// </summary>
	/// <seealso href="https://developer.apple.com/library/mac/documentation/QuickTime/QTFF">QuickTime File Format Specification</seealso>
	public abstract class AtomicInfo : IBinSerializable
	{
		internal static readonly ILog log = Logger.GetLogger(typeof(AtomicInfo));
		private static readonly byte[] EmptyData = new byte[0];

		/// <summary>
		/// Identifies the atom type, typically represented as a four-character code.
		/// </summary>
		[XmlIgnore]
		public AtomicCode AtomicID { get; set; }

		[XmlAttribute("AtomicID"), DefaultValue("")]
		public string Name
		{
			get { return AtomicID.ToString(); }
			set { this.AtomicID = new AtomicCode(value); }
		}

		public sealed class RawDataManager
		{
			public byte[] Data { get; private set; }
			public int Offset { get; private set; }
			public long Position { get; private set; }
			public int Length { get; private set; }

			public BinaryReader CreateReader()
			{
				var mem = new MemoryStream(Data);
				var bound = new BoundStream(mem, Position, Position + Length);
				return new BinReader(bound, Encoding.UTF8, littleEndian: false);
			}

			public RawDataManager(BinaryReader reader)
			{
				var mem = new MemoryStream();
				reader.BaseStream.WriteTo(mem);
				Data = mem.ToArray();
				Offset = 0;
				Position = reader.BaseStream.Position;
				Length = Data.Length;
			}

			public RawDataManager(BinaryReader reader, int dataSize)
			{
				Data = new byte[dataSize];
				Offset = 0;
				Position = reader.BaseStream.Position;
				Length = Data.Length;
				reader.Read(Data, 0, Length);
			}

			public RawDataManager(RawDataManager parent, long position, int dataSize)
			{
				Data = parent.Data;
				Offset = (int)(position - parent.Position) + parent.Offset;
				Position = position;
				Length = dataSize;
			}

			public byte[] CurrentData
			{
				get
				{
					var data = new byte[Length];
					Array.Copy(Data, Offset, data, 0, Length);
					return data;
				}
			}
		}

		[XmlIgnore]
		public RawDataManager RawData { get; internal set; }

		[XmlIgnore]
		public AtomicInfo Parent { get; set; }

		protected AtomicInfo() { }

		protected AtomicInfo(string type)
		{
			this.AtomicID = new AtomicCode(type);
		}

		public abstract void ReadBinary(BinaryReader reader);

		/// <summary>
		/// An integer that specifies the number of bytes in this box data, including all its fields and contained boxes.
		/// </summary>
		public abstract long DataSize { get; }
		public abstract void WriteBinary(BinaryWriter writer);

		protected Encoding Language2Encoding { get { return Encoding.UTF8; } }

		public static bool AllowUnknownBox { get { return true; } }

		public static AtomicInfo ParseBox(BinaryReader reader, AtomicInfo parent = null, bool required = true, bool editable = false)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			if (!(reader.BaseStream is BoundStream))
			{
				var bound = reader.BaseStream.CanSeek ?
					new BoundStream(reader.BaseStream) :
					new BoundStream(reader.BaseStream, 0L, -1L);

				reader = new BinReader(bound, Encoding.UTF8, littleEndian: false);
			}

			byte[] buf;
			uint boxSize;
			if (required)
			{
				boxSize = reader.ReadUInt32();
			}
			else
			{
				buf = new byte[4];
				if (reader.Read(buf, 0, 4) < 4 || buf.IsZero())
					return null;
				boxSize = buf.ToUInt32(0, littleEndian: false);
			}
			AtomicCode atomid;
			//fix for some boxes found in some old hinted files
			if (boxSize >= 2 && boxSize <= 4)
			{
				return new ISOMediaBoxes.VoidBox(4);
			}
			else if (boxSize == 0u)
			{
				//now here's a bad thing: some files use size 0 for void atoms, some for "till end of file" indictaion..
				buf = boxSize.GetBytes(littleEndian: false);
				long start = reader.BaseStream.Position;
				//Apple has decided to add around 2k of NULL space outside of any atom structure starting with iTunes 7.0.0
				//its possible this is part of gapless playback - but then why would it come after the 'free' at the end of a file like gpac writes?
				while (buf.IsZero())
				{
					if (reader.Read(buf, 0, 4) < 4)
						return new ISOMediaBoxes.VoidBox((int)(reader.BaseStream.Position - start));
				}
				long len = reader.BaseStream.Position - start;
				if (len > 0)
					log.Warn("Space {0} bytes have been skipped.", len);
				atomid = (AtomicCode)buf.ToUInt32(0, littleEndian: false);
			}
			else
			{
				atomid = reader.ReadAtomicCode();
			}
			return ParseBox(reader, boxSize, atomid, editable, parent);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader">Binary reader with bounded stream.</param>
		/// <param name="boxSize"></param>
		/// <param name="atomid"></param>
		/// <param name="editable"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		internal static AtomicInfo ParseBox(BinaryReader reader, long boxSize, AtomicCode atomid, bool editable = false, AtomicInfo parent = null)
		{
			AtomicInfo box;
			long hdrSize = 8L;
			//handle uuid
			if (atomid == ISOMediaBoxes.UUIDBox.DefaultID)
			{
				var uuid = new Guid(reader.ReadBytes(16));
				hdrSize += 16L;
				//TODO: KnownUUIDAtoms
				box = new ISOMediaBoxes.UnknownUUIDBox
				{
					UUID = uuid
				};
			}
			else
			{
				var def = MatchToKnownAtom(atomid, parent);
				box = def.CreateBox(atomid, parent);
			}
			long dataSize, parentSize = 0L;
			bool hasSize = true;

			//handle large box
			if (boxSize == 1L)
			{
				hdrSize += 8L;
				dataSize = reader.ReadInt64() - hdrSize;
				if (dataSize < 0L)
					throw new InvalidDataException("Invalid box large size for " + atomid);
			}
			else if (boxSize > 0L)
			{
				dataSize = boxSize - hdrSize;
			}
			else
			{
				dataSize = 0L;
				hasSize = false;
			}

			#region Exceptional box size
			if (dataSize == 0L && atomid == ISOMediaBoxes.MediaDataBox.DefaultID && parent == null)
			{
				hasSize = false;
			}
			//no size means till end of file - EXCEPT FOR some old QuickTime boxes...
			else if (boxSize == 0 && atomid == ISOMediaBoxes.TOTLBox.DefaultID)
			{
				dataSize = 4L;
			}
			#endregion

			long start = reader.BaseStream.Position;
			//try to resolve data size
			if (!hasSize)
			{
				if (atomid != ISOMediaBoxes.MediaDataBox.DefaultID)
					log.Debug("Warning Read Box type {0} size 0 reading till the end of file", atomid);
				if (reader.BaseStream.CanSeek)
				{
					dataSize = reader.BaseStream.Length - start;
				}
			}

			if (dataSize < 0L)
				throw new InvalidDataException(String.Format("Box size {1} less than box header size {2} for {0}", atomid, boxSize, hdrSize));

			//media data box - set bound only
			if (atomid == ISOMediaBoxes.MediaDataBox.DefaultID && parent == null)
			{
				if (!hasSize)
					reader.BaseStream.SetLength(-1L);
				else
					reader.BaseStream.SetLength(start + dataSize);
			}
			//box without length - read rest part to buffer
			else if (!hasSize)
			{
				box.RawData = new RawDataManager(reader);
				dataSize = box.RawData.Length;
				reader = box.RawData.CreateReader();
			}
			//editable - read to buffer
			else if (editable)
			{
				box.RawData = new RawDataManager(reader, (int)dataSize);
				reader = box.RawData.CreateReader();
			}
			//editable parent - link to buffer
			else if (parent != null && parent.RawData != null)
			{
				box.RawData = new RawDataManager(parent.RawData, start, (int)dataSize);
				parentSize = reader.BaseStream.Length;
				reader.BaseStream.SetLength(start + dataSize);
			}
			//readonly - set bound only
			else
			{
				if (parent != null)
					parentSize = reader.BaseStream.Length;
				reader.BaseStream.SetLength(start + dataSize);
			}

			start = reader.BaseStream.Position;
			try
			{
				box.ReadBinary(reader);
			}
			catch (EndOfStreamException ex)
			{
				if (atomid == ISOMediaBoxes.FileTypeBox.DefaultID ||
					atomid == ISOMediaBoxes.JPEG2000Atom.DefaultID ||
					atomid == ISOMediaBoxes.MediaDataBox.DefaultID)
					throw;

				log.Error(ex.Message);
				box = new ISOMediaBoxes.InvalidBox(ex, box, reader, start);
			}
			catch (IOException) { throw; }
			catch (UnauthorizedAccessException) { throw; }
			catch (OutOfMemoryException) { throw; }
			catch (StackOverflowException) { throw; }
			catch (InvalidDataException) { throw; }
			catch (Exception ex)
			{
				if (atomid == ISOMediaBoxes.FileTypeBox.DefaultID ||
					atomid == ISOMediaBoxes.JPEG2000Atom.DefaultID ||
					atomid == ISOMediaBoxes.MediaDataBox.DefaultID)
					throw;

				log.Error(ex.Message);
				box = new ISOMediaBoxes.InvalidBox(ex, box, reader, start);
			}
			finally
			{
				if (parentSize > 0L)
					reader.BaseStream.SetLength(parentSize);
				else if (parent == null)
					reader.BaseStream.SetLength(-1L);
			}
			long readSize = reader.BaseStream.Position - start;

			//diagnose damage to 'cprt' by libmp4v2 in 1.4.1 & 1.5.0.1
			//typically, the length of this atom (dataSize) will exceeed it parent (which is reported as 17)
			//true length ot this data will be 9 - impossible for iTunes-style 'data' atom.
			if (atomid == "data" && parent is IBoxContainer)
			{
				if (dataSize > readSize + 8L)
				{
					log.Warn("The 'data' child of the '{0}' atom seems to be corrupted.", box.Name);
					reader.BaseStream.Skip(dataSize - readSize);
					dataSize = readSize;
				}
			}
			//end diagnosis; APar_Manually_Determine_Parent will still determine it to be a versioned atom (it tests by names), but at file write out,
			//it will write with a length of 9 bytes

			if (dataSize > readSize)
			{
				//throw new InvalidOperationException(String.Format("Unexpected end of box '{0}'", box.Name));
				log.Warn("Unexpected end of box '{0}'", box.Name);
				reader.BaseStream.Skip(dataSize - readSize);
			}
			else if (hasSize && dataSize < readSize)
			{
				throw new InvalidDataException(String.Format("Invalid atom '{0}' size", box.Name));
			}

			return box;
		}

		public virtual void WriteBox(BinaryWriter writer)
		{
			long length = GetBoxSize();
			writer.Write((int)length);
			writer.Write(this.AtomicID);
			WriteBinary(writer);
		}

		public long GetBoxSize()
		{
			return DataSize + 8L;
		}

		internal void Read_boxArray(BinaryReader reader)
		{
			reader.ReadCount32(((IBoxContainer)this).Boxes, this);
		}

		internal void Read_entryArray(BinaryReader reader)
		{
			reader.ReadCount16(((IBoxContainer)this).Boxes, this);
		}

		internal void Read_boxList(BinaryReader reader)
		{
			reader.ReadEnd(((IBoxContainer)this).Boxes, this);
		}

		internal long Size_boxArray()
		{
			return ((IBoxContainer)this).Boxes.SizeCount32();
		}

		internal long Size_entryArray()
		{
			return ((IBoxContainer)this).Boxes.SizeCount16();
		}

		internal long Size_boxList()
		{
			return ((IBoxContainer)this).Boxes.SizeEnd();
		}

		internal void Write_boxArray(BinaryWriter writer)
		{
			writer.WriteCount32(((IBoxContainer)this).Boxes);
		}

		internal void Write_entryArray(BinaryWriter writer)
		{
			writer.WriteCount16(((IBoxContainer)this).Boxes);
		}

		internal void Write_boxList(BinaryWriter writer)
		{
			writer.WriteEnd(((IBoxContainer)this).Boxes);
		}

		public AtomicInfo FindAtom(AtomicCode atom)
		{
			var container = this as IBoxContainer;
			if (container == null)
				return null;
			foreach (var child in container.Boxes)
			{
				if (child.AtomicID == atom)
					return child;
				child.FindAtom(atom);
			}
			return null;
		}

		/// <summary>
		/// MatchToKnownAtom
		/// </summary>
		/// <param name="atom">the name of our newly found atom</param>
		/// <param name="parent">the parent container atom</param>
		/// <returns></returns>
		private static AtomDefinition MatchToKnownAtom(AtomicCode atom, AtomicInfo parent)
		{
			string atom_container = parent == null ? "" : (string)parent.AtomicID;
			string atom_name = (string)atom;
			//if this atom is contained by 'ilst', then it is *highly* likely an iTunes-style metadata parent atom
			if (atom_container == "ilst" && atom_name != ISOMediaBoxes.UUIDBox.DefaultID)
			{
				return Definitions.ListAtom; //2nd to last KnowAtoms is a generic placeholder iTunes-parent atom
				//fprintf(stdout, "found iTunes parent %s = atom %s\n", KnownAtoms[return_known_atom].known_atom_name, atom_name);

				//if this atom is "data" get the full path to it; we will take any atom under 'ilst' and consider it an iTunes metadata parent atom
			}
			else if (atom_name == "data")
			{
				//find_atom_path only is NULL in APar_ScanAtoms (where fromFile is true) and in APar_CreateSparseAtom, where atom_number was just filled
				string fullpath = parent.ProvideAtomPath();

				//fprintf(stdout, "APar_ProvideAtomPath gives %s (%s-%s)\n", fullpath, atom_name, atom_container);
				if (fullpath.StartsWith("moov.udta.meta.ilst"))
				{
					return Definitions.iTunesAtom; //last KnowAtoms is a generic placeholder iTunes-data atom
					//fprintf(stdout, "found iTunes data child\n");
				}


			} //if this atom is "esds" get the full path to it; take any atom under 'stsd' as a parent to esds (that parent would be a 4CC codec; not all do have 'esds'...)
			else if (atom_name == "esds")
			{
				string fullpath = parent.ProvideAtomPath();

				if (fullpath.StartsWith("moov.trak.mdia.minf.stbl.stsd"))
				{
					return Definitions.ElementaryStreamDescriptionAtom; //manually return the esds atom
				}

			}
			else
			{
				//try matching the name of the atom
				foreach (var knownAtom in Definitions.KnownAtoms.Where(knownAtom => atom_name == knownAtom.KnownAtomName))
				{
					//name matches, now see if the container atom matches any known container for that atom
					if (knownAtom.AnyLevel)
					{
						return knownAtom; //the list starts at 0; the unknown atom is at 0; first known atom (ftyp) is at 1

					}
					else
					{
						if (knownAtom.KnownParentAtom == atom_container)
							return knownAtom; //the list starts at 0; the unknown atom is at 0; first known atom (ftyp) is at 1
					}
				}
			}
			//accommodate any future child to dref; force to being versioned
			if (atom_container == "dref")
			{
				return Definitions.DataReferenceAtom; //return a generic *VERSIONED* child atom; otherwise an atom without flags will be present & chunk offsets will not update
			}
			return Definitions.UnknownAtom;
		}

		/// <summary>
		/// ProvideAtomPath
		/// </summary>
		/// <param name="atom">the wanted path of an atom</param>
		/// <returns>string into which the path will be placed (working backwards)</returns>
		internal string ProvideAtomPath()
		{
			var atom_path = new List<string>();
			if (this.AtomicID == ISOMediaBoxes.UUIDBox.DefaultID)
			{
				atom_path.Add("uuid=" + this.Name);
			}
			else
			{
				atom_path.Add(this.Name);
			}

			var atom = this.Parent;
			while (atom != null)
			{
				atom_path.Add(atom.Name);
				atom = atom.Parent;
			}
			return String.Join(".", atom_path.Reverse<string>());
		}

		protected TBox[] BoxListSerialize<TBox>(ICollection<TBox> list)
			where TBox: AtomicInfo
		{
			return Container.BoxListSerialize(list);
		}

		protected List<TBox> BoxListDeserialize<TBox>(TBox[] list)
			where TBox: AtomicInfo
		{
			return Container.BoxListDeserialize(list);
		}

		protected internal static void CreateEntryCollection<TEntry, TOwner>(out Collection<TEntry> collection, TOwner owner)
			where TEntry : IEntry<TOwner>
		{
			collection = new EntryCollection<TEntry, TOwner>(owner);
		}

		#region XML serialization
		public sealed class DataXMLSerializer : IXmlSerializable
		{
			public byte[] Data { get; private set; }
			private const string TypeXMLAttr = "Type";
			private const string TypeBase64XMLEnum = "Base64";
			private const string TypeHexXMLEnum = "Hex";
			private const string TypeGuidXMLEnum = "GUID";
			private const string TypeUTF16LEXMLEnum = "UTF16LE";
			private const string TypeUTF16BEXMLEnum = "UTF16BE";
			private const string TypeUTF8XMLEnum = "UTF8";
			private const string TypeEmptyEnum = "Empty";
			private const string DataSizeXMLAttr = "DataSize";

			public DataXMLSerializer(byte[] data)
			{
				this.Data = data;
			}

			public static DataXMLSerializer NonEmpty(byte[] data)
			{
				if (!data.Any(b => b != 0)) return null;
				return new DataXMLSerializer(data);
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
				case TypeUTF16BEXMLEnum:
					string strbe = reader.ReadContentAsString();
					this.Data = Encoding.BigEndianUnicode.GetBytes(strbe);
					return;
				case TypeUTF16LEXMLEnum:
					string strle = reader.ReadContentAsString();
					this.Data = Encoding.Unicode.GetBytes(strle);
					return;
				case TypeUTF8XMLEnum:
					string str = reader.ReadContentAsString();
					this.Data = Encoding.UTF8.GetBytes(str);
					return;
				case TypeEmptyEnum:
					int size = XmlConvert.ToInt32(reader.GetAttribute(DataSizeXMLAttr));
					this.Data = new byte[size];
					Array.Clear(this.Data, 0, size);
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
				if (IsEmpty(this.Data))
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeEmptyEnum);
					writer.WriteAttributeString(DataSizeXMLAttr, XmlConvert.ToString(this.Data.Length));
				}
				else if (this.Data.Length == 16)
				{
					var guid = new Guid(this.Data);
					writer.WriteAttributeString(TypeXMLAttr, TypeGuidXMLEnum);
					writer.WriteString(XmlConvert.ToString(guid));
				}
				else if (IsUTF16BE(this.Data))
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeUTF16BEXMLEnum);
					writer.WriteString(Encoding.BigEndianUnicode.GetString(this.Data, 2, this.Data.Length - 2));
				}
				else if (IsUTF16LE(this.Data))
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeUTF16LEXMLEnum);
					writer.WriteString(Encoding.Unicode.GetString(this.Data, 2, this.Data.Length - 2));
				}
				else if (IsUTF8(this.Data))
				{
					writer.WriteAttributeString(TypeXMLAttr, TypeUTF8XMLEnum);
					writer.WriteString(Encoding.UTF8.GetString(this.Data, 0, this.Data.Length));
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

			private static bool IsUTF16BE(byte[] data)
			{
				if (data.Length <= 2 || data.Length % 2 != 0) return false;
				var bom = Encoding.BigEndianUnicode.GetPreamble();
				string str;
				if (data[0] != bom[0] || data[1] != bom[1]) return false;
				{
					try
					{
						str = Encoding.BigEndianUnicode.GetString(data, 2, data.Length - 2);
					}
					catch
					{
						return false;
					}
				}
				return !str.Any(c => (int)c < 32 && c != '\t' && c != '\r' && c != '\n');
			}

			private static bool IsUTF8(byte[] data)
			{
				if (data.Length <= 2) return false;
				var bom = Encoding.BigEndianUnicode.GetPreamble();
				string str;
				if (data[0] == bom[0] && data[1] == bom[1])
					return false;
				bom = Encoding.Unicode.GetPreamble();
				if (data[0] == bom[0] && data[1] == bom[1])
					return false;
				try
				{
					str = Encoding.UTF8.GetString(data);
				}
				catch
				{
					return false;
				}
				return !str.Any(c => (int)c < 32 && c != '\t' && c != '\r' && c != '\n');
			}

			private static bool IsUTF16LE(byte[] data)
			{
				if (data.Length <= 2 || data.Length % 2 != 0) return false;
				var bom = Encoding.Unicode.GetPreamble();
				string str;
				if (data[0] != bom[0] || data[1] != bom[1]) return false;
				{
					try
					{
						str = Encoding.Unicode.GetString(data, 2, data.Length - 2);
					}
					catch
					{
						return false;
					}
				}
				return !str.Any(c => (int)c < 32 && c != '\t' && c != '\r' && c != '\n');
			}

			private static bool IsEmpty(byte[] data)
			{
				return !data.Any(b => b != 0);
			}
		}

		/// <summary>
		/// A transformation matrix.
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("<[{A},{B},{U}],[{C},{D},{V}],[{Tx},{Ty},{W}]>")]
		public sealed class TransformMatrix
		{
			public enum MatrixValues { A, B, U, C, D, V, Tx, Ty, W }
			private static readonly uint[] defaultMatrix = new uint[]
			{
				(uint)Fixed<uint, x16>.One,  (uint)Fixed<uint, x16>.Zero, (uint)Fixed<uint, x30>.Zero,
				(uint)Fixed<uint, x16>.Zero, (uint)Fixed<uint, x16>.One,  (uint)Fixed<uint, x30>.Zero,
				(uint)Fixed<uint, x16>.Zero, (uint)Fixed<uint, x16>.Zero, (uint)Fixed<uint, x30>.One
			};

			private uint[] matrix = new uint[9];

			public static readonly TransformMatrix DefaultMatrix = new TransformMatrix { matrix = defaultMatrix };

			public bool IsDefaultMatrix { get { return IsDefault(matrix); } }

			//public static 

			private double m2d(int index)
			{
				if (IsDefault(matrix)) return Double.Epsilon;
				switch ((MatrixValues)index)
				{
					case MatrixValues.U:
					case MatrixValues.V:
					case MatrixValues.W:
						return (double)(Fixed<uint, x30>)matrix[index];
					default:
						return (double)(Fixed<uint, x16>)matrix[index];
				}
			}

			private void d2m(int index, double value)
			{
				if (value == Double.Epsilon)
				{
					matrix[index] = defaultMatrix[index];
					return;
				}
				switch ((MatrixValues)index)
				{
					case MatrixValues.U:
					case MatrixValues.V:
					case MatrixValues.W:
						matrix[index] = (uint)(Fixed<uint, x30>)value;
						break;
					default:
						matrix[index] = (uint)(Fixed<uint, x16>)value;
						break;
				}
			}

			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double A { get { return m2d(0); } set { d2m(0, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double B { get { return m2d(1); } set { d2m(1, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double U { get { return m2d(2); } set { d2m(2, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double C { get { return m2d(3); } set { d2m(3, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double D { get { return m2d(4); } set { d2m(4, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double V { get { return m2d(5); } set { d2m(5, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double Tx { get { return m2d(6); } set { d2m(6, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double Ty { get { return m2d(7); } set { d2m(7, value); } }
			[XmlAttribute, DefaultValue(Double.Epsilon)]
			public double W { get { return m2d(8); } set { d2m(8, value); } }

			[XmlIgnore]
			public object this[MatrixValues index]
			{
				get
				{
					switch (index)
					{
						case MatrixValues.U:
						case MatrixValues.V:
						case MatrixValues.W:
							return (Fixed<uint, x30>)matrix[(int)index];
						default:
							return (Fixed<uint, x16>)matrix[(int)index];
					}
				}
			}

			internal TransformMatrix(uint[] matrix)
			{
				if (IsDefault(matrix))
					this.matrix = defaultMatrix;
				else
					this.matrix = (uint[])matrix.Clone();
			}

			public TransformMatrix() { }

			private static bool IsDefault(uint[] matrix)
			{
				for (int k = 0; k < defaultMatrix.Length; k++)
					if (matrix[k] != defaultMatrix[k])
						return false;
				return true;
			}

			public static explicit operator TransformMatrix(uint[] data)
			{
				return new TransformMatrix(data);
			}

			public static explicit operator uint[](TransformMatrix matrix)
			{
				return matrix.matrix;
			}
		}
		#endregion
	}

	public enum AtomState
	{
		/// <summary>
		/// Container atom
		/// </summary>
		ParentAtom = 0,
		SimpleParentAtom = 1,
		/// <summary>
		/// Acts as both parent (contains other atoms) &amp; child (carries data)
		/// </summary>
		DualStateAtom = 2,
		/// <summary>
		/// Atom that does NOT contain any children
		/// </summary>
		ChildAtom = 3,
		UnknownAtom = 4
	}

	public enum AtomRequirements
	{
		/// <summary>
		/// Means total of 1 atom per file  (or total of 1 if parent atom is required to be present)
		/// </summary>
		RequiredOnce = 30,
		/// <summary>
		/// Means 1 atom per container atom; totalling many per file  (or required present if optional parent atom
		/// is present)
		/// </summary>
		RequiredOne = 31,
		/// <summary>
		/// Means 1 or more atoms per container atom are required to be present
		/// </summary>
		RequiredVariable = 32,
		/// <summary>
		/// Means (iTunes-style metadata) the atom defines how many are present;
		/// most are MAX 1 'data' atoms; 'covr' is ?unlimited?
		/// </summary>
		ParentSpecific = 33, //
		/// <summary>
		/// Means total of 1 atom per file, but not required
		/// </summary>
		OptionalOnce = 34,
		/// <summary>
		/// Means 1 atom per container atom but not required; many may be present in a file
		/// </summary>
		OptionalOne = 35,
		/// <summary>
		/// Means more than 1 occurrence per container atom
		/// </summary>
		OptionalMany = 36,
		/// <summary>
		/// Means that one of the family of atoms defined by the spec is required by the parent atom
		/// </summary>
		ReqFamilialOne = OptionalOne,
		Uknown = 38
	}

	public enum BoxType
	{
		SimpleAtom = 50,
		VersionedAtom = 51,
		ExtendedAtom = 52,
		PackedLangAtom = 53,
		UnknownAtom = 59
	}
}
