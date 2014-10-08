/*
 *			GPAC - Multimedia Framework C SDK - box_code_base.c
 *
 *			Copyright (c) Jean Le Feuvre 2000-2005 
 *					All rights reserved
 *
 *  This file is part of GPAC / ISO Media File Format sub-project
 *
 *  GPAC is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  GPAC is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *   
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  ----------------------
 *  SVN revision information:
 *    $Revision$
 *
 */
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		internal static readonly byte[] EmptyData = new byte[0];

		public abstract partial class ISOMFullBox: AtomicInfo
		{
			[XmlAttribute, DefaultValue(AtomFlags.Binary)]
			public virtual AtomFlags AtomFlags
			{
				get { return UnknownFlags == 0 ? (AtomFlags)Flags : MP4.AtomFlags.Binary; }
				set { Flags = (int)value; }
			}

			[XmlAttribute, DefaultValue(0)]
			public virtual int UnknownFlags
			{
				get
				{
					return ((AtomFlags[])Enum.GetValues(typeof(AtomFlags)))
						.Contains((AtomFlags)Flags) ? 0 : Flags;
				}
				set { Flags = value; }
			}

			public ISOMFullBox() { }

			protected ISOMFullBox(int version)
			{
				this.Version = version;
			}

			protected void ResolveVersion(long data)
			{
				this.Version = data > uint.MaxValue ? 1 : 0;
			}
		}

		public abstract partial class ISOMUUIDBox: AtomicInfo
		{
			[XmlAttribute("UUID"), DefaultValue("")]
			public string UUIDAsString
			{
				get { return this.UUID == Guid.Empty ? "" : XmlConvert.ToString(this.UUID); }
				set { this.UUID = String.IsNullOrEmpty(value) ? Guid.Empty : XmlConvert.ToGuid(value); }
			}
		}

		public sealed partial class Box: AtomicInfo
		{
			public override void ReadBinary(BinaryReader reader) { }

			public override long DataSize
			{
				get { return 0L; }
			}

			public override void WriteBinary(BinaryWriter writer) { }
		}

		public sealed partial class UUIDBox: ISOMUUIDBox
		{
			public UUIDBox()
			{
				this.Data = EmptyData;
			}

			public override void ReadBinary(BinaryReader reader)
			{
				long length = reader.Length();
				if (length >= 16L)
				{
					this.UUID = new Guid(reader.ReadBytes(16));
					if (this.UUID == Guid.Empty)
					{
						this.Data = new byte[(int)reader.Length()];
						Array.Copy(this.UUID.ToByteArray(), 0, this.Data, 0, 16);
						reader.Read(this.Data, 16, (int)length - 16);
					}
					else
					{
						this.Data = reader.ReadBytes((int)length - 16);
					}
				}
				else
				{
					this.UUID = Guid.Empty;
					this.Data = reader.ReadBytes((int)length);
				}
			}

			public override long DataSize
			{
				get
				{
					return (this.UUID == Guid.Empty ? 0 : 16) + this.Data.Length;
				}
			}

			public override void WriteBinary(BinaryWriter writer)
			{
				if (this.UUID != Guid.Empty)
					writer.Write(this.UUID.ToByteArray());
				writer.Write(this.Data);
			}
		}

		public sealed partial class UnknownBox: AtomicInfo
		{
			[XmlElement("Data")]
			public DataXMLSerializer DataSerializer
			{
				get { return new DataXMLSerializer(this.Data); }
				set { this.Data = value.Data; }
			}
		}

		public sealed partial class UnknownParentBox: AtomicInfo, IBoxContainer
		{
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			[XmlElement(typeof(FreeSpaceBox))]
			[XmlElement(typeof(UUIDBox))]
			[XmlElement(typeof(MovieHeaderBox))]
			[XmlElement(typeof(MediaHeaderBox))]
			[XmlElement(typeof(VideoMediaHeaderBox))]
			[XmlElement(typeof(SoundMediaHeaderBox))]
			[XmlElement(typeof(HintMediaHeaderBox))]
			[XmlElement(typeof(MPEGMediaHeaderBox))]
			[XmlElement(typeof(UserDataBox))]
			[XmlElement(typeof(TrackReferenceTypeBox))]
			[XmlElement(typeof(TOTLBox))]
			[XmlElement(typeof(SampleTableBox))]
			[XmlElement(typeof(DataInformationBox))]
			[XmlElement(typeof(HandlerBox))]
			[XmlElement(typeof(NameBox))]
			[XmlElement(typeof(MediaBox))]
			[XmlElement(typeof(TrackHeaderBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class ChapterListBox : ISOMFullBox
		{
			[XmlElement("Chapter")]
			public List<ChapterEntry> List
			{
				get { return this.list; }
			}

			public ChapterListBox() : base(version: 1) { }
		}

		/// <summary>
		/// ISO Copyright statement
		/// </summary>
		public sealed partial class CopyrightBox : ISOMFullBox
		{
			[XmlAttribute("Language"), DefaultValue("")]
			public string LanguageAsString { get { return this.Language.ToString(); } set { this.Language = new PackedLanguage(value); } }
		}

		public sealed partial class CompositionOffsetBox : ISOMFullBox
		{
			public List<DttsEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class UnknownUUIDBox: ISOMUUIDBox
		{
			[XmlElement("Data")]
			public DataXMLSerializer DataSerializer
			{
				get { return new DataXMLSerializer(this.Data); }
				set { this.Data = value.Data; }
			}
		}

		/// <summary>
		/// Data Information Atom
		/// </summary>
		public sealed partial class DataInformationBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("DataReference", typeof(DataReferenceBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		/// <summary>
		/// Data Reference Atom
		/// </summary>
		public sealed partial class DataReferenceBox : ISOMFullBox, IBoxContainer
		{
#warning Looking forward to 'alis', 'rsrc', 'cios'
			//[XmlElement(typeof(alis))]
			//[XmlElement(typeof(rsrc))]
			//[XmlElement(typeof(cios))]
			[XmlElement("DataEntryURL", typeof(DataEntryURLBox))]
			[XmlElement("DataEntryURN", typeof(DataEntryURNBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxArray; }
			}
		}

		public sealed partial class EditListBox : ISOMFullBox
		{
			[XmlElement("Edit")]
			public Collection<EdtsEntry> List
			{
				get { return this.entryList; }
			}

			private void ResolveVersion()
			{
				if (entryList.Any(
					entry => entry.SegmentDuration > uint.MaxValue ||
					entry.MediaTime > BinConverter.MaxMacDate32))
					Version = 1;
				else
					Version = 0;
			}

			public EditListBox()
			{
				CreateEntryCollection(out entryList, this);
			}
		}

		public sealed partial class EditBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("EditList", typeof(EditListBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		public sealed partial class ESDBox : ISOMFullBox
		{
#line 1053 "box_code_base.c"
#warning ESDBox should be implemented with ODF
			[XmlElement("Data")]
			public DataXMLSerializer DataSerializer
			{
				get { return new DataXMLSerializer(this.data); }
				set { this.data = value.Data; }
			}
#line default
		}

		/// <summary>
		/// Unused space available in file.
		/// </summary>
		public sealed partial class FreeSpaceBox: AtomicInfo
		{
			[XmlElement("Data")]
			public DataXMLSerializer DataSerializer
			{
				get { return new DataXMLSerializer(this.Data); }
				set { this.Data = value.Data; }
			}

			public FreeSpaceBox() { }

			public FreeSpaceBox(int size)
			{
				this.Data = new byte[size];
			}
		}

		/// <summary>
		/// The File Type Compatibility Atom
		/// </summary>
		public sealed partial class FileTypeBox: AtomicInfo
		{
			internal static readonly AtomicCode[] EmptyBrands = new AtomicCode[0];

			[XmlAttribute("Brand"), DefaultValue("")]
			public string BrandAsString
			{
				get { return this.Brand.ToString(); }
				set { this.Brand = new AtomicCode(value); }
			}

			[XmlAttribute("Version"), DefaultValue("0")]
			public string VersionAsString
			{
				get { return this.Version.ToString("X"); }
				set { this.Version = int.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier); }
			}

			[XmlElement("CompatibleBrand")]
			public string[] AltBrandSerializer
			{
				get
				{
					return this.CompatibleBrand.Select(brand => brand.ToString()).ToArray();
				}
				set
				{
					this.CompatibleBrand = value.Select(brand => new AtomicCode(brand)).ToArray();
				}
			}

			private void TestCompatibleBrand()
			{
				foreach (var brand in this.CompatibleBrand)
				{
					if (brand == "mp42"/*0x6D703432*/ || brand == "iso2"/*0x69736F32*/)
					{
						base.AncillaryData = (string)brand;
					}
				}
			}

			private void Read_CompatibleBrand(BinaryReader reader)
			{
				var brands = new List<AtomicCode>();
				while (reader.Length() >= 4L)
				{
					var brand = reader.ReadAtomicCode();
					if (!brand.IsEmpty)
						brands.Add(brand);
				}
				this.CompatibleBrand = brands.ToArray();
			}

			private long Size_CompatibleBrand()
			{
				return this.CompatibleBrand.Length * 4;
			}

			private void Write_CompatibleBrand(BinaryWriter writer)
			{
				foreach (var brand in this.CompatibleBrand)
				{
					writer.Write(brand);
				}
			}
		}

		public sealed partial class GenericSampleEntryBox : ISOMSampleEntryFields
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			//carefull we are not writing the box type but the entry type so switch for write
			public override void WriteBox(BinaryWriter writer)
			{
				long length = GetBoxSize();
				writer.Write((int)length);
				writer.Write(this.EntryType);
				WriteBinary(writer);
			}
		}

		public sealed partial class GenericVisualSampleEntryBox : ISOMVisualSampleEntry
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
			public override PixelAspectRatioBox PAsp { get { return null; } set { } }
			public override RVCConfigurationBox RVCC { get { return null; } set { } }

			//carefull we are not writing the box type but the entry type so switch for write
			public override void WriteBox(BinaryWriter writer)
			{
				long length = GetBoxSize();
				writer.Write((int)length);
				writer.Write(this.EntryType);
				WriteBinary(writer);
			}
		}

		public sealed partial class GenericAudioSampleEntryBox : ISOMAudioSampleEntry
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			//carefull we are not writing the box type but the entry type so switch for write
			public override void WriteBox(BinaryWriter writer)
			{
				long length = GetBoxSize();
				writer.Write((int)length);
				writer.Write(this.EntryType);
				WriteBinary(writer);
			}
		}

		/// <summary>
		/// Handler Reference Atom
		/// </summary>
		public sealed partial class HandlerBox : ISOMFullBox
		{
			[XmlAttribute("ComponentType"), DefaultValue("")]
			public string ComponentTypeAsString
			{
				get { return this.ComponentType.ToString(); }
				set { this.ComponentType = new AtomicCode(value); }
			}
			[XmlAttribute("HandlerType"), DefaultValue("")]
			public string HandlerTypeAsString
			{
				get { return this.HandlerType.ToString(); }
				set { this.HandlerType = new AtomicCode(value); }
			}
			[XmlAttribute("Manufacturer"), DefaultValue("")]
			public string ManufacturerAsString
			{
				get { return this.Manufacturer.ToString(); }
				set { this.Manufacturer = new AtomicCode(value); }
			}
		}

		public sealed partial class HintInfoBox : AtomicInfo, IBoxContainer
		{
			[XmlIgnore]
			public IEnumerable<MAXRBox> DataRates
			{
				get { return this.boxList.OfType<MAXRBox>(); } //Unique by Granularity
			}

			[XmlElement("TRPY", typeof(TRPYBox))]
			[XmlElement("NUMP", typeof(NUMPBox))]
			[XmlElement("NTYL", typeof(NTYLBox))]
			[XmlElement("TOTL", typeof(TOTLBox))]
			[XmlElement("NPCK", typeof(NPCKBox))]
			[XmlElement("MAXR", typeof(MAXRBox))]
			[XmlElement("DMED", typeof(DMEDBox))]
			[XmlElement("DIMM", typeof(DIMMBox))]
			[XmlElement("DREP", typeof(DREPBox))]
			[XmlElement("TMIN", typeof(TMINBox))]
			[XmlElement("TMAX", typeof(TMAXBox))]
			[XmlElement("PMAX", typeof(PMAXBox))]
			[XmlElement("DMAX", typeof(DMAXBox))]
			[XmlElement("PAYT", typeof(PAYTBox))]
			[XmlElement("TPAY", typeof(TPAYBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class HintTrackInfoBox : AtomicInfo, IBoxContainer
		{
			//this is the value for GF_RTPBox - same as HintSampleEntry for RTP !!!
			[XmlElement("RTP", typeof(RTPBox))]
			[XmlElement("SDP", typeof(SDPBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				//WARNING: because of the HNTI at movie level, we cannot use the generic parsing scheme!
				//this because the child SDP box at the movie level has a type of RTP, used for
				//the HintSampleEntry !
				get
				{
					//"ITS LENGTH IS CALCULATED BY SUBSTRACTING 8 (or 12) from the box size" - QT specs
					//this means that we don't have any NULL char as a delimiter in QT ...

					//if (rtp.SubType != "sdp ")
					//	throw new NotSupportedException();
					return this.boxList; //Unique RTPBox or SDPBox
				}
			}
		}

		//TODO: ODF implementation
		public sealed partial class ObjectDescriptorBox : ISOMFullBox
		{
#line 2635 "box_code_base.c"
#warning ObjectDescriptorBox isn't implemented
			[XmlElement("Data")]
			public DataXMLSerializer DataSerializer
			{
				get { return new DataXMLSerializer(this.data); }
				set { this.data = value.Data; }
			}
#line default
		}

		/// <summary>
		/// Movie sample data — usually this data can be interpreted only by using the movie resource.
		/// </summary>
		public sealed partial class MediaDataBox: AtomicInfo
		{
			public struct ExternalDataXMLSerializer
			{
				[XmlAttribute]
				public string Type { get { return "External"; } set { } }
				[XmlAttribute]
				public long Offset { get; set; }
				[XmlAttribute]
				public long DataSize { get; set; }
			}

			[XmlElement("Data")]
			public ExternalDataXMLSerializer DataSerializer
			{
				get { return new ExternalDataXMLSerializer { Offset = this.Offset, DataSize = this.MediaDataSize }; }
				set { this.Offset = value.Offset; this.MediaDataSize = value.DataSize; }
			}

			public override void ReadBinary(BinaryReader reader)
			{
				long length = reader.Length();
				this.Offset = length == 0L ? 0L : reader.BaseStream.Position;
				this.MediaDataSize = length;
				//then skip these bytes
				reader.BaseStream.Seek(length, SeekOrigin.Current);
			}

			public override long DataSize
			{
				get { return this.DataSize; }
			}

			public override void WriteBinary(BinaryWriter writer)
			{
				//make sure we have some data ...
				//if not, we handle that independantly (edit files)
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Media Header Atom
		/// </summary>
		public sealed partial class MediaHeaderBox: ISOMFullBox
		{
			[XmlAttribute("Language"), DefaultValue("")]
			public string LanguageAsString { get { return this.Language.ToString(); } set { this.Language = new PackedLanguage(value); } }
		}

		public sealed partial class MediaBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("MediaHeader", typeof(MediaHeaderBox))]
			[XmlElement("Handler", typeof(HandlerBox))]
			[XmlElement("Information", typeof(MediaInformationBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class MediaInformationBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("MPEGMediaHeader", typeof(MPEGMediaHeaderBox))]
			[XmlElement("VideoMediaHeader", typeof(VideoMediaHeaderBox))]
			[XmlElement("SoundMediaHeader", typeof(SoundMediaHeaderBox))]
			[XmlElement("HintMediaHeader", typeof(HintMediaHeaderBox))]
			//[XmlElement(typeof(gmhd))]
			[XmlElement("DataInformation", typeof(DataInformationBox))]
			[XmlElement("SampleTable", typeof(SampleTableBox))]
			[XmlElement("Handler", typeof(HandlerBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		/*MovieFragment is a container IN THE FILE, contains 1 fragment*/
		public sealed partial class MovieFragmentBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("Header", typeof(MovieFragmentHeaderBox))]
			[XmlElement("Track", typeof(TrackFragmentBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		/// <summary>
		/// The Movie Atom
		/// </summary>
		public sealed partial class MovieBox : AtomicInfo, IBoxContainer
		{
#warning Looking forward to 'drm', 'ipmc'
			[XmlElement("MovieHeader", typeof(MovieHeaderBox))]
			[XmlElement("ObjectDescriptor", typeof(ObjectDescriptorBox))]
			//[XmlElement(typeof(drm ))]
			[XmlElement("Meta", typeof(MetaBox))]
			[XmlElement("MovieExtends", typeof(MovieExtendsBox))]
			//[XmlElement(typeof(ipmc))]
			[XmlElement("Track", typeof(TrackBox))]
			[XmlElement("UserData", typeof(UserDataBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class MPEGAudioSampleEntryBox : ISOMAudioSampleEntry, IBoxContainer
		{
			[XmlElement("ESD", typeof(ESDBox))]
			[XmlElement("ProtectionInfo", typeof(ProtectionInfoBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get
				{
					/*hack for some weird files (possibly recorded with live.com tools, needs further investigations)*/
					/*HACK for QT files: get the esds box from the track*/
					//ESD = AtomicInfo.ParseBox(reader, this);
					return this.boxList;
				}
			}

			public static MPEGAudioSampleEntryBox CreateMP4A()
			{
				return new MPEGAudioSampleEntryBox { AtomicID = (AtomicCode)"mp4a" };
			}

			public static MPEGAudioSampleEntryBox CreateENCA()
			{
				return new MPEGAudioSampleEntryBox { AtomicID = (AtomicCode)"enca" };
			}
		}

		/*for most MPEG4 media */
		public sealed partial class MPEGSampleEntryBox : ISOMSampleEntryFields, IBoxContainer
		{
			[XmlElement("ESD", typeof(ESDBox))]
			[XmlElement("ProtectionInfo", typeof(ProtectionInfoBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}

			public static MPEGSampleEntryBox CreateMP4S()
			{
				return new MPEGSampleEntryBox { AtomicID = (AtomicCode)"mp4s" };
			}

			public static MPEGSampleEntryBox CreateENCS()
			{
				return new MPEGSampleEntryBox { AtomicID = (AtomicCode)"encs" };
			}
		}

		public sealed partial class MPEGVisualSampleEntryBox : ISOMVisualSampleEntry, IBoxContainer
		{
			[XmlElement("ProtectionInfo", typeof(ProtectionInfoBox))]
			[XmlElement("PAsp", typeof(PixelAspectRatioBox))]
			[XmlElement("RVCC", typeof(RVCConfigurationBox))]
			[XmlElement("ESD", typeof(ESDBox))]
			[XmlElement("AVCConfig", typeof(AVCConfigurationBox))]
			[XmlElement("Bitrate", typeof(MPEG4BitRateBox))]
			[XmlElement("Descr", typeof(MPEG4ExtensionDescriptorsBox))]
			[XmlElement("iPodExt", typeof(UnknownUUIDBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get
				{
					/*this is an AVC sample desc*/
					//if (AVCConfig != null || SVCConfig != null) AVC_RewriteESDescriptor(this);
					return this.boxList;
				}
			}

			public static MPEGVisualSampleEntryBox CreateMP4V()
			{
				return new MPEGVisualSampleEntryBox { AtomicID = (AtomicCode)"mp4v" };
			}

			public static MPEGVisualSampleEntryBox CreateAVC1()
			{
				return new MPEGVisualSampleEntryBox { AtomicID = (AtomicCode)"avc1" };
			}

			public static MPEGVisualSampleEntryBox CreateAVC2()
			{
				return new MPEGVisualSampleEntryBox { AtomicID = (AtomicCode)"avc2" };
			}

			public static MPEGVisualSampleEntryBox CreateSVC1()
			{
				return new MPEGVisualSampleEntryBox { AtomicID = (AtomicCode)"svc1" };
			}

			public static MPEGVisualSampleEntryBox CreateENCV()
			{
				return new MPEGVisualSampleEntryBox { AtomicID = (AtomicCode)"encv" };
			}
		}

		public sealed partial class MovieExtendsBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("Track", typeof(TrackExtendsBox))]
			[XmlElement("Header", typeof(MovieExtendsHeaderBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		/// <summary>
		/// Movie Header Atom
		/// </summary>
		public sealed partial class MovieHeaderBox: ISOMFullBox
		{
			[XmlIgnore]
			public bool ReservedSpecified
			{
				get { return !Reserved.IsZero(); }
			}

			[XmlAttribute("PreferredRate"), DefaultValue(1.0)]
			public double PreferredRateAsFloat { get { return (double)PreferredRate; } set { PreferredRate = value.ToFixed32(); } }
			[XmlAttribute("PreferredVolume"), DefaultValue(1.0)]
			public double PreferredVolumeAsFloat { get { return (double)PreferredVolume; } set { PreferredVolume = value.ToFixed16(); } }

			[XmlIgnore]
			public bool MatrixSpecified
			{
				get { return !Matrix.IsDefaultMatrix; }
			}
		}

		public sealed partial class PaddingBitsBox : ISOMFullBox
		{
			public override void ReadBinary(BinaryReader reader)
			{
				base.ReadBinary(reader);
				int sampleCount = reader.ReadInt32();
				PadBits = new sbyte[sampleCount];
				for (int i = 0; i < sampleCount; i += 2)
				{
					int data = reader.ReadByte();
					if (i + 1 < sampleCount)
					{
						PadBits[i + 1] = (sbyte)((data >> 4) & 0x7);
					}
					PadBits[i] = (sbyte)(data & 0x7);
				}
			}

			public override long DataSize
			{
				get
				{
					return base.DataSize + (PadBits.Length + 1) / 2;
				}
			}

			public override void WriteBinary(BinaryWriter writer)
			{
				base.WriteBinary(writer);
				int sampleCount = PadBits.Length;
				writer.Write((int)sampleCount);
				for (int i = 0; i < sampleCount; i += 2)
				{
					int data = 0;
					if (i + 1 < sampleCount)
					{
						data |= (PadBits[i + 1] & 0x7) << 4;
					}
					data |= PadBits[i] & 0x7;
					writer.Write((byte)data);
				}
			}
		}

		/// <summary>
		/// Sound Media Information Header Atom
		/// </summary>
		public sealed partial class SoundMediaHeaderBox: ISOMFullBox
		{
			[XmlAttribute("Balance"), DefaultValue(0.0)]
			public double BalanceAsFloat { get { return (double)Balance; } set { Balance = value.ToFixed16(); } }
		}

		/// <summary>
		/// Sample Table Atom
		/// </summary>
		public sealed partial class SampleTableBox : AtomicInfo, IBoxContainer
		{
#warning Looking forward to 'cslg', 'stps'
			[XmlElement("SampleDescription", typeof(SampleDescriptionBox))]
			[XmlElement("TimeToSample", typeof(TimeToSampleBox))]
			[XmlElement("CompositionOffset", typeof(CompositionOffsetBox))]
			//[XmlElement(typeof(cslg))]
			[XmlElement("SyncSample", typeof(SyncSampleBox))]
			//[XmlElement(typeof(stps))]
			[XmlElement("SampleToChunk", typeof(SampleToChunkBox))]
			[XmlElement("SampleSize", typeof(SampleSizeBox))]
			[XmlElement("ChunkOffset", typeof(ChunkOffsetBox))]
			[XmlElement("ChunkLargeOffset", typeof(ChunkLargeOffsetBox))]//WARNING: AS THIS MAY CHANGE DYNAMICALLY DURING EDIT
			[XmlElement("ShadowSync", typeof(ShadowSyncBox))]
			[XmlElement("SampleGroupDescription", typeof(SampleGroupDescriptionBox))]
			[XmlElement("SampleGroup", typeof(SampleGroupBox))]
			[XmlElement("SampleDep", typeof(SampleDependencyTypeBox))]
			[XmlElement("DegradationPriority", typeof(DegradationPriorityBox))]
			[XmlElement("PaddingBits", typeof(PaddingBitsBox))]
			[XmlElement("SubSamples", typeof(SubSampleInformationBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class SampleToChunkBox : ISOMFullBox
		{
			[XmlElement("Entry")]
			public List<StscEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class SampleDescriptionBox : ISOMFullBox, IBoxContainer
		{
#warning Looking forward to 'enct', 'drms', 'drmi', 'alac', 'avcp', 'jpeg', 'srtp', 'samr', 'sawb', 'sawp', 's263', 'sevc', 'sqcp', 'ssmv', 'tmcd', 'mjp2'
			[XmlElement(typeof(MPEGSampleEntryBox))]
			[XmlElement(typeof(MPEGAudioSampleEntryBox))]
			[XmlElement(typeof(MPEGVisualSampleEntryBox))]
			[XmlElement(typeof(HintSampleEntryBox))]
			[XmlElement(typeof(Tx3gSampleEntryBox))]
			[XmlElement(typeof(TextSampleEntryBox))]
			//[XmlElement(typeof(ENCT))]
			[XmlElement(typeof(MetaDataSampleEntryBox))]
			[XmlElement(typeof(DIMSSampleEntryBox))]
			[XmlElement(typeof(AC3SampleEntryBox))]
			[XmlElement(typeof(LASeRSampleEntryBox))]
			//[XmlElement(typeof(SUBTYPE_3GP_AMR))]
			//[XmlElement(typeof(SUBTYPE_3GP_AMR_WB))]
			//[XmlElement(typeof(SUBTYPE_3GP_EVRC))]
			//[XmlElement(typeof(SUBTYPE_3GP_QCELP))]
			//[XmlElement(typeof(SUBTYPE_3GP_SMV))]
			//[XmlElement(typeof(SUBTYPE_3GP_H263))]
			//[XmlElement(typeof(drms))]
			//[XmlElement(typeof(drmi))]
			//[XmlElement(typeof(alac))]
			//[XmlElement(typeof(avcp))]
			//[XmlElement(typeof(jpeg))]
			//[XmlElement(typeof(samr))]
			//[XmlElement(typeof(sawb))]
			//[XmlElement(typeof(sawp))]
			//[XmlElement(typeof(s263))]
			//[XmlElement(typeof(sevc))]
			//[XmlElement(typeof(sqcp))]
			//[XmlElement(typeof(ssmv))]
			//[XmlElement(typeof(tmcd))]
			//[XmlElement(typeof(mjp2))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxArray; }
			}
		}

		public sealed partial class SampleFragmentBox : ISOMFullBox
		{
			[XmlElement("Entry")]
			public List<StsfEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class ShadowSyncBox : ISOMFullBox
		{
			[XmlElement("Entry")]
			public List<StshEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class SampleSizeBox : ISOMFullBox
		{
			private void Read_Sizes(System.IO.BinaryReader reader)
			{
				int sampleCount = reader.ReadInt32();
				Sizes = new int[sampleCount];
				//no samples, no parsing pb
				if (sampleCount == 0) return;
				switch (SampleSize)
				{
					case 4:
					case 8:
					case 16:
						SizeLength = SampleSize;
						break;
					default:
						if (AtomicID == DefaultID)
						{
							//if (SampleSize != 0) return;
							SizeLength = 32;
							break;
						}
						//try to fix the file
						int estSize = (int)(reader.Length() / sampleCount);
						if (estSize == 0 && ((sampleCount + 1) / 2 == reader.Length()))
							SizeLength = 4;
						else if (estSize == 1 || estSize == 2)
							SizeLength = 8 * estSize;
						else
							throw new InvalidDataException("Invalid sample size in " + AtomicID);
						break;
				}
				switch (SizeLength)
				{
					case 32:
						for (int index = 0; index < sampleCount; index++)
							Sizes[index] = reader.ReadInt32();
						break;
					case 16:
						for (int index = 0; index < sampleCount; index++)
							Sizes[index] = reader.ReadUInt16();
						break;
					case 8:
						for (int index = 0; index < sampleCount; index++)
							Sizes[index] = reader.ReadByte();
						break;
					case 4:
						//note we could optimize the mem usage by keeping the table compact
						//in memory. But that would complicate both caching and editing
						//we therefore keep all sizes as u32 and uncompress the table
						for (int i = 0; i < sampleCount; i += 2)
						{
							int data = reader.ReadByte();
							if (i + 1 < sampleCount)
							{
								Sizes[i + 1] = (data >> 4);
							}
							Sizes[i] = data & 0xF;
						}
						break;
				}
			}

			private long Size_Sizes()
			{
				if (Sizes == null) return 4;
				if (AtomicID == DefaultID)
				{
					return 4 + Sizes.Length * 4;
				}
				SizeLength = 4;
				SampleSize = Sizes[0];
				foreach (int size in Sizes)
				{
					if (SampleSize != size) SampleSize = 0;
					if (size < 0xF) continue;
					//switch to 8-bit table
					else if (size <= 0xFF && SizeLength <= 8)
						SizeLength = 8;
					//switch to 16-bit table
					else if (size <= 0xFFFF && SizeLength <= 16)
						SizeLength = 16;
					//switch to 32-bit table
					else
						SizeLength = 32;
				}
				//if all samples are of the same size, switch to regular (more compact)
				if (SampleSize != 0)
				{
					AtomicID = (AtomicCode)DefaultID;
					Sizes = null;
					return 4;
				}
				if (SizeLength == 32)
				{
					//oops, doesn't fit in a compact table
					AtomicID = (AtomicCode)DefaultID;
					return 4 + Sizes.Length * 4;
				}
				//make sure we are a compact table (no need to change the mem representation)
				AtomicID = (AtomicCode)"stz2";
				SampleSize = 0; //SampleSize = SizeLength
				return SizeLength == 4 ?
					4 + (Sizes.Length + 1) / 2 :
					4 + Sizes.Length * (SizeLength / 8);
			}

			private void Write_Sizes(System.IO.BinaryWriter writer)
			{
				int sampleCount = Sizes.Length;
				writer.Write((int)sampleCount);
				switch (SizeLength)
				{
					case 32:
						for (int index = 0; index < sampleCount; index++)
							writer.Write((int)Sizes[index]);
						break;
					case 16:
						for (int index = 0; index < sampleCount; index++)
							writer.Write((ushort)Sizes[index]);
						break;
					case 8:
						for (int index = 0; index < sampleCount; index++)
							writer.Write((byte)Sizes[index]);
						break;
					case 4:
						for (int i = 0; i < sampleCount; i += 2)
						{
							int data = 0;
							if (i + 1 < sampleCount)
							{
								data = Sizes[i + 1] << 4;
							}
							data |= Sizes[i] & 0xF;
							writer.Write((byte)data);
						}
						break;
					default:
						throw new InvalidDataException("Invalid sample size in " + AtomicID);
				}
			}
		}


		public sealed partial class TimeToSampleBox : ISOMFullBox
		{
			[XmlElement("Entry")]
			public List<SttsEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class TrackFragmentHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			public override AtomFlags AtomFlags { get { return MP4.AtomFlags.Binary; } set { } }

			[XmlAttribute]
			public override int UnknownFlags
			{
				get { return Flags & ~(int)TrackFragmentFlags.ValidMask; }
				set { Flags = value; }
			}
		}

		/// <summary>
		/// Track Header Atom
		/// </summary>
		public sealed partial class TrackHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			public override AtomFlags AtomFlags { get { return MP4.AtomFlags.Binary; } set { } }

			[XmlAttribute]
			public override int UnknownFlags
			{
				get { return Flags & ~(int)TrackFlags.ValidMask; }
				set { Flags = value; }
			}

			[XmlAttribute("Volume")]
			public double VolumeAsFloat { get { return (double)Volume; } set { Volume = value.ToFixed16(); } }

			[XmlIgnore]
			public bool Reserved2Specified
			{
				get { return !Reserved2.IsZero(); }
			}

			[XmlIgnore]
			public bool MatrixSpecified
			{
				get { return !Matrix.IsDefaultMatrix; }
			}
		}

		public sealed partial class TrackFragmentBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("Header", typeof(TrackFragmentHeaderBox))]
			[XmlElement("TrackRun", typeof(TrackFragmentRunBox))]
			[XmlElement("SDTP", typeof(SampleDependencyTypeBox))]
			[XmlElement("Subs", typeof(SubSampleInformationBox))]
			[XmlElement("SampleGroup", typeof(SampleGroupBox))]
			[XmlElement("SampleGroupDescription", typeof(SampleGroupDescriptionBox))]
			[XmlElement("TFDT", typeof(TFBaseMediaDecodeTimeBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		/// <summary>
		/// Track Atom
		/// </summary>
		public sealed partial class TrackBox : AtomicInfo, IBoxContainer
		{
#warning Looking forward to 'tapt'
			[XmlElement("Header", typeof(TrackHeaderBox))]
			[XmlElement("EditBox", typeof(EditBox))]
			[XmlElement("References", typeof(TrackReferenceBox))]
			[XmlElement("Media", typeof(MediaBox))]
			[XmlElement("UserData", typeof(UserDataBox))]
			[XmlElement("Meta", typeof(MetaBox))]
			//[XmlElement(typeof(tapt))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
#line 6200 "box_code_base.c"
#warning Lines 6200 - 6302 gf_isom_check_sample_desc method ('vide'=>GenericVisualSampleEntryBox; 'soun'=>GenericAudioSampleEntryBox; GenericSampleEntryBox) isn't implemented
#line default
		}

		/// <summary>
		/// Track Reference Atom
		/// </summary>
		public sealed partial class TrackReferenceBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("TrackReferenceType", typeof(TrackReferenceTypeBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public BoxCollection<TrackReferenceTypeBox> Boxes
			{
				get { return this.boxList; }
			}

			Collection<AtomicInfo> IBoxContainer.Boxes { get { return this.boxList; } }
		}

		/// <summary>
		/// Track reference type atom
		/// </summary>
		public sealed partial class TrackReferenceTypeBox: AtomicInfo
		{
			public int AddRefTrack(int trackID)
			{
				int i = Array.IndexOf(this.TrackIDs, trackID);
				if (i >= 0) return i;
				this.TrackIDs = TrackIDs.Concat(new int[] { trackID }).ToArray();
				return this.TrackIDs.Length - 1;
			}
		}

		public sealed partial class TrackFragmentRunBox : ISOMFullBox
		{
			[XmlAttribute]
			public override AtomFlags AtomFlags { get { return MP4.AtomFlags.Binary; } set { } }

			[XmlAttribute]
			public override int UnknownFlags
			{
				get { return Flags & ~(int)TrackRunFlags.ValidMask; }
				set { Flags = value; }
			}

			[XmlElement("Entry")]
			public Collection<TrunEntry> Entries
			{
				get { return this.entries; }
			}

			public TrackFragmentRunBox()
			{
				CreateEntryCollection(out entries, this);
			}
		}

		/// <summary>
		/// used to classify boxes in the UserData Box
		/// </summary>
		public sealed partial class UserDataMap
		{
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}

			public UserDataMap(AtomicCode boxType)
			{
				this.BoxType = boxType;
			}

			public UserDataMap(Guid uuid)
			{
				this.BoxType = (AtomicCode)UUIDBox.DefaultID;
				this.UUID = uuid;
			}
		}

		/// <summary>
		/// User Data Atom
		/// </summary>
		public sealed partial class UserDataBox: AtomicInfo, IBoxContainer
		{
			#region UserDataMap collection
			private class MapList : Collection<AtomicInfo>
			{
				private List<UserDataMap> maps;

				protected override IEnumerable<AtomicInfo> Elements
				{
					get { return maps.SelectMany(map => map.Boxes); }
				}

				public MapList()
				{
					this.maps = new List<UserDataMap>();
				}

				public UserDataMap Get(AtomicCode boxType)
				{
					return maps.FirstOrDefault(map => map.BoxType == boxType);
				}

				public UserDataMap Get(Guid uuid)
				{
					return maps.FirstOrDefault(map => map.BoxType == UUIDBox.DefaultID && map.UUID == uuid);
				}

				public override void Add(AtomicInfo box)
				{
					var uuidBox = box as ISOMUUIDBox;
					UserDataMap map;
					if (uuidBox != null)
					{
						map = Get(uuidBox.UUID);
						if (map == null)
						{
							map = new UserDataMap(uuidBox.UUID);
							maps.Add(map);
						}
					}
					else
					{
						map = Get(box.AtomicID);
						if (map == null)
						{
							map = new UserDataMap(box.AtomicID);
							maps.Add(map);
						}
					}
					map.Boxes.Add(box);
				}

				public override void Clear()
				{
					maps.Clear();
				}

				public override bool Remove(AtomicInfo box)
				{
					var uuidBox = box as ISOMUUIDBox;
					UserDataMap map;
					if (uuidBox != null)
					{
						map = Get(uuidBox.UUID);
						if (map == null) return false;
					}
					else
					{
						map = Get(box.AtomicID);
						if (map == null) return false;
					}
					return map.Boxes.Remove(box);
				}
			}
			#endregion

#warning Looking forward to 'hnti', 'hinf', 'name', 'titl', 'auth', 'perf', 'gnre', 'dscp', 'albm', 'yrrc', 'rtng', 'clsf', 'kywd', 'loci', 'tsel', 'data'
			[XmlElement("Meta", typeof(MetaBox))]
			//[XmlElement(typeof(hnti))]
			//[XmlElement(typeof(hinf))]
			//[XmlElement(typeof(name))]
			[XmlElement("Copyright", typeof(CopyrightBox))]
			//[XmlElement(typeof(titl))]
			//[XmlElement(typeof(auth))]
			//[XmlElement(typeof(perf))]
			//[XmlElement(typeof(gnre))]
			//[XmlElement(typeof(dscp))]
			//[XmlElement(typeof(albm))]
			//[XmlElement(typeof(yrrc))]
			//[XmlElement(typeof(rtng))]
			//[XmlElement(typeof(clsf))]
			//[XmlElement(typeof(kywd))]
			//[XmlElement(typeof(loci))]
			//[XmlElement(typeof(tsel))]
			[XmlElement("FreeSpace", typeof(FreeSpaceBox))]
			[XmlElement("UUID", typeof(UUIDBox))]
			[XmlElement(typeof(ESDBox))]
			//[XmlElement(typeof(data))]
			[XmlElement("ChapterList", typeof(ChapterListBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> RecordList
			{
				//warning: here we are not passing the actual "parent" of the list
				//but the UDTA box. The parent itself is not an box, we don't care about it
				get { return this.boxList; }
			}

			Collection<AtomicInfo> IBoxContainer.Boxes { get { return boxList; } }

			public UserDataMap this[AtomicCode boxType]
			{
				get { return boxList.Get(boxType); }
			}

			public UserDataMap this[Guid uuid]
			{
				get { return boxList.Get(uuid); }
			}
		}

		public sealed partial class VoidBox : AtomicInfo
		{
			public override void ReadBinary(BinaryReader reader)
			{
				if (reader.Length() > 0)
					throw new InvalidDataException("Non empty VOID box");
			}

			public override long DataSize
			{
				get { return 0; }
			}

			public override void WriteBinary(BinaryWriter writer)
			{
			}
		}

		public sealed partial class ProgressiveDownloadBox : ISOMFullBox
		{
			public ProgressiveDownloadBox()
			{
				AtomFlags = AtomFlags.Text; //1
			}
		}

		public sealed partial class ProgressiveDownloadBox : ISOMFullBox
		{
			public override void ReadBinary(BinaryReader reader)
			{
				base.ReadBinary(reader);
				int count = (int)(reader.Length() / 8);
				Rates = new int[count];
				Times = new int[count];
				for (int index = 0; index < count; index++)
				{
					Rates[index] = reader.ReadInt32();
					Times[index] = reader.ReadInt32();
				}
			}

			public override long DataSize
			{
				get
				{
					return base.DataSize + Rates.Length * 8;
				}
			}

			public override void WriteBinary(BinaryWriter writer)
			{
				base.WriteBinary(writer);
				int count = Rates.Length;
				for(int index = 0; index < count; index++)
				{
					writer.Write((int)Rates[index]);
					writer.Write((int)Times[index]);
				}
			}
		}

		public sealed partial class AC3SampleEntryBox : ISOMAudioSampleEntry, IBoxContainer
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			[XmlElement("Info", typeof(AC3ConfigBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		public sealed partial class LASeRSampleEntryBox : ISOMSampleEntryFields
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			[XmlElement("LsrConfig", typeof(LASeRConfigurationBox))]
			[XmlElement("Bitrate", typeof(MPEG4BitRateBox))]
			[XmlElement("Descr", typeof(MPEG4ExtensionDescriptorsBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		public sealed partial class SampleEntry : IEntry<SubSampleInformationBox>
		{
			[XmlElement("SubSample")]
			public Collection<SubSampleEntry> SubSamples
			{
				get { return this.subSamples; }
			}

			SubSampleInformationBox IEntry<SubSampleInformationBox>.Owner
			{
				get { return null; }
				set { AtomicInfo.CreateEntryCollection(out subSamples, value); }
			}
		}

		public sealed partial class SubSampleInformationBox : ISOMFullBox
		{
			[XmlElement("Sample")]
			public Collection<SampleEntry> Samples
			{
				get { return this.samples; }
			}

			public SubSampleInformationBox()
			{
				CreateEntryCollection(out samples, this);
			}
		}

		public sealed partial class SampleGroupBox : ISOMFullBox
		{
			[XmlElement("Sample")]
			public List<SampleGroupEntry> SampleEntries
			{
				get { return this.sampleEntries; }
			}
		}

		public sealed partial class SampleGroupDescriptionBox : ISOMFullBox
		{
			[XmlAttribute("GroupingType")]
			public string GroupingTypeAsString
			{
				get { return GroupingType.ToString(); }
				set { GroupingType = new AtomicCode(value); }
			}

			[XmlElement("DefaultSampleGroupDescription", typeof(DefaultSampleGroupDescriptionEntry))]
			[XmlElement("VisualRandomAccess", typeof(VisualRandomAccessEntry))]
			[XmlElement("RollRecovery", typeof(RollRecoveryEntry))]
			public Collection<SampleGroupDescriptionEntry> GroupDescriptions
			{
				get { return this.groupDescriptions; }
			}

			private SampleGroupDescriptionEntry ResolveGroupDescription()
			{
				switch ((string)GroupingType)
				{
					case "roll":
						return new RollRecoveryEntry();
					case "rap ":
						return new VisualRandomAccessEntry();
					default:
						return new DefaultSampleGroupDescriptionEntry();
				}
			}

			/*version 0 is deprecated, use v1 by default*/
			public SampleGroupDescriptionBox()
				: base(version: 1)
			{
				CreateEntryCollection(out groupDescriptions, this);
			}
		}

		public abstract partial class SampleGroupDescriptionEntry : IEntry<SampleGroupDescriptionBox>
		{
			public SampleGroupDescriptionBox Owner { get; set; }
		}
	}
}
