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
	public enum AtomFlags
	{
		/// <summary>
		/// Atom version 1byte/ Atom flags 3 bytes; 0x00 00 00 00
		/// </summary>
		Binary = 0,

		/// <summary>
		/// UTF-8, no termination
		/// </summary>
		Text = 1,

		/// <summary>
		/// \x0D
		/// </summary>
		JPEGBinary = 13,

		/// <summary>
		/// \x0E
		/// </summary>
		PNGBinary = 14,

		/// <summary>
		/// \x15 for cpil, tmpo, rtng; iTMS atoms: cnID, atID, plID, geID, sfID, akID
		/// </summary>
		UInt = 21,

		/// <summary>
		/// 0x58 for uuid atoms that contain files
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

	/// <summary>
	/// The basic data unit
	/// </summary>
	/// <seealso href="https://developer.apple.com/library/mac/documentation/QuickTime/QTFF"/>
	public abstract class AtomicInfo : IBinSerializable
	{
		internal static readonly ILog log = Logger.GetLogger(typeof(AtomicInfo));
		private static readonly byte[] EmptyData = new byte[0];

		/// <summary>
		/// A 32-bit integer that identifies the atom type.
		/// </summary>
		[XmlIgnore]
		public AtomicCode AtomicID { get; set; }

		[XmlAttribute("AtomicID")]
		public string Name
		{
			get { return AtomicID.ToString(); }
			set { this.AtomicID = new AtomicCode(value); }
		}

		//char* ReverseDNSname;
		//char* ReverseDNSdomain;
		public string AncillaryData;         //just contains a simple number for atoms that contains some interesting info (like stsd codec used)
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

		public static AtomicInfo ParseBox(BinaryReader reader, AtomicInfo parent = null, bool required = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			long start = reader.BaseStream.Position;
			uint boxSize = reader.ReadUInt32();
			if (boxSize == 0u && !required)
				return null;
			AtomicCode atomid;
			//fix for some boxes found in some old hinted files
			if (boxSize == 0u)
			{
				uint id = boxSize;
				//Apple has decided to add around 2k of NULL space outside of any atom structure starting with iTunes 7.0.0
				//its possible this is part of gapless playback - but then why would it come after the 'free' at the end of a file like gpac writes?
				while (id == 0u)
				{
					boxSize = id;
					id = reader.ReadUInt32();
				}
				long len = reader.BaseStream.Position - start - 8L;
				if (len > 0)
					log.Warn("Space {0} bytes have been skipped.", len);
				atomid = (AtomicCode)id;
			}
			else
			{
				atomid = reader.ReadAtomicCode();
			}
			return ParseBox(reader, start, boxSize, atomid, parent);
		}

		internal static AtomicInfo ParseBox(BinaryReader reader, long start, long boxSize, AtomicCode atomid, AtomicInfo parent = null)
		{
			var def = MatchToKnownAtom(atomid, parent);
			var box = def.CreateBox(atomid, parent);
			long dataSize;
			var data = reader.BaseStream;
			Stream mem = null;

			if (atomid == ISOMediaBoxes.MediaDataBox.DefaultID)
			{
				if (boxSize == 1L)
					dataSize = reader.ReadInt64() - 16L;
				else if (boxSize > 8L)
					dataSize = boxSize - 8L;
				else
					dataSize = reader.BaseStream.Length - start - 8L;
				if (dataSize < 0L)
					throw new InvalidDataException("Invalid movie sample data size");
				boxSize = dataSize + 8L;
			}
			else
			{
				dataSize = boxSize - 8L;

				//no size means till end of file - EXCEPT FOR some old QuickTime boxes...
				if (boxSize == 0L && atomid == ISOMediaBoxes.TOTLBox.DefaultID)
					dataSize = 4;

				if (dataSize < 0)
					throw new InvalidDataException("Invalid box size");

				mem = new MemoryStream((int)dataSize);
				reader.BaseStream.WriteTo(mem, (int)dataSize);
				mem.Seek(0L, SeekOrigin.Begin);
				reader = new BinReader(mem, Encoding.UTF8, false);
			}
			box.ReadBinary(reader);
			long readSize = data.Position - start;
			if (mem != null)
				readSize -= mem.Length - mem.Position;

			//diagnose damage to 'cprt' by libmp4v2 in 1.4.1 & 1.5.0.1
			//typically, the length of this atom (dataSize) will exceeed it parent (which is reported as 17)
			//true length ot this data will be 9 - impossible for iTunes-style 'data' atom.
			if (atomid == "data" && box is IBoxContainer)
			{
				if (boxSize > readSize)
				{
					boxSize = readSize;
					log.Warn("The 'data' child of the '{0}' atom seems to be corrupted.", box.Name);
				}
			}
			//end diagnosis; APar_Manually_Determine_Parent will still determine it to be a versioned atom (it tests by names), but at file write out,
			//it will write with a length of 9 bytes

			if (boxSize > readSize)
			{
				//throw new InvalidOperationException(String.Format("Unexpected end of box '{0}'", box.Name));
				log.Warn("Unexpected end of box '{0}'", box.Name);
			}
			else if (boxSize < readSize)
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
			reader.ReadCount(((IBoxContainer)this).Boxes, this);
		}

		internal void Read_boxList(BinaryReader reader)
		{
			reader.ReadEnd(((IBoxContainer)this).Boxes, this);
		}

		internal long Size_boxArray()
		{
			return ((IBoxContainer)this).Boxes.SizeCount();
		}

		internal long Size_boxList()
		{
			return ((IBoxContainer)this).Boxes.SizeEnd();
		}

		internal void Write_boxArray(BinaryWriter writer)
		{
			writer.WriteCount(((IBoxContainer)this).Boxes);
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
			private const string TypeUnicodeXMLEnum = "Unicode";
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
				case TypeUnicodeXMLEnum:
					string str = reader.ReadContentAsString();
					this.Data = Encoding.Unicode.GetBytes(str);
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

			private static bool IsEmpty(byte[] data)
			{
				return !data.Any(b => b != 0);
			}
		}

		/// <summary>
		/// A transformation matrix.
		/// </summary>
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
