/*
 *			GPAC - Multimedia Framework C SDK - box_dump.c
 *
 *			Authors: Jean Le Feuvre
 *			Copyright (c) Telecom ParisTech 2000-2012
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		public abstract partial class ISOMFullBox: AtomicInfo
		{
			[XmlAttribute, DefaultValue(AtomFlags.Binary)]
			public virtual AtomFlags AtomFlags
			{
				get { return Flags.ValidEnum(AtomFlags.Binary); }
				set { Flags = (int)value; }
			}

			[XmlAttribute, DefaultValue(0)]
			public virtual int ReservedFlags
			{
				get
				{
					return Flags.UnknownEnum<AtomFlags>();
				}
				set { Flags = value; }
			}
		}

		public abstract partial class ISOMUUIDBox: AtomicInfo
		{
			public bool UUIDSpecified
			{
				get { return this.UUID != Guid.Empty; }
			}
		}

		public sealed partial class InvalidBox : AtomicInfo
		{
			public struct ErrorXMLSerializer
			{
				[XmlIgnore]
				public Exception Error;
				[XmlAttribute]
				public string Type { get { return Error.GetType().FullName; } set { } }
				[XmlText]
				public string Message { get { return Error.Message; } set { } }
			}

			public struct InvalidDataXMLSerializer
			{
				[XmlAttribute]
				public string Type { get { return Data == null ? "External" : "Base64"; } set { } }
				[XmlAttribute, DefaultValue(0L)]
				public long Offset { get; set; }
				[XmlAttribute, DefaultValue(0L)]
				public long DataSize { get; set; }
				[XmlText(DataType = "base64Binary")]
				public byte[] Data { get; set; }
			}

			[XmlElement("Error")]
			public ErrorXMLSerializer[] ErrorMessages
			{
				get
				{
					var list = new List<ErrorXMLSerializer>();
					for (var ex = this.Error; ex != null; ex = ex.InnerException)
						list.Add(new ErrorXMLSerializer { Error = ex });
					return list.ToArray();
				}
				set { }
			}

			[XmlElement("Data")]
			public InvalidDataXMLSerializer DataSerializer
			{
				get { return new InvalidDataXMLSerializer { Offset = this.Offset, DataSize = this.RawDataSize, Data = this.Data }; }
				set { this.Offset = value.Offset; this.RawDataSize = value.DataSize; this.Data = value.Data; }
			}
		}

		public sealed partial class UnknownBox : AtomicInfo
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
			[XmlElement(typeof(Box))]
			[XmlElement(typeof(UUIDBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class ChapterEntry
		{
			[XmlAttribute("StartTime")]
			public string StartTimeAsString
			{
				get { return StartTime.ToXML(); }
				set { value.FromXML(out StartTime); }
			}
		}

		public sealed partial class ChapterListBox : ISOMFullBox
		{
			[XmlElement("Chapter")]
			public List<ChapterEntry> List
			{
				get { return this.list; }
			}
		}

		// ISO Copyright statement
		public sealed partial class CopyrightBox : ISOMFullBox
		{
			[XmlAttribute("LanguageCode"), DefaultValue("")]
			public string LanguageAsString { get { return this.Language.ToString(); } set { this.Language = new PackedLanguage(value); } }
		}

		public sealed partial class CompositionOffsetBox : ISOMFullBox
		{
			[XmlElement("CompositionOffsetEntry")]
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

		// Data Information Atom
		public sealed partial class DataInformationBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("DataReferenceBox", typeof(DataReferenceBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		// Data Reference Atom
		public sealed partial class DataReferenceBox : ISOMFullBox, IBoxContainer
		{
#warning Looking forward to 'alis', 'rsrc', 'cios'
			//[XmlElement(typeof(alis))]
			//[XmlElement(typeof(rsrc))]
			//[XmlElement(typeof(cios))]
			[XmlElement("URLDataEntryBox", typeof(DataEntryURLBox))]
			[XmlElement("URNDataEntryBox", typeof(DataEntryURNBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxArray; }
			}
		}

		public sealed partial class EditListBox : ISOMFullBox
		{
			[XmlElement("EditListEntry")]
			public Collection<EdtsEntry> List
			{
				get { return this.entryList; }
			}
		}

		public sealed partial class EditBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("EditListBox", typeof(EditListBox))]
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

		// Unused space available in file.
		public sealed partial class FreeSpaceBox: AtomicInfo
		{
			[XmlElement("Data")]
			public DataXMLSerializer DataSerializer
			{
				get { return new DataXMLSerializer(this.Data); }
				set { this.Data = value.Data; }
			}
		}

		// The File Type Compatibility Atom
		public sealed partial class FileTypeBox: AtomicInfo
		{
			[XmlAttribute("MajorBrand"), DefaultValue("")]
			public string BrandAsString
			{
				get { return this.Brand.ToString(); }
				set { this.Brand = new AtomicCode(value); }
			}

			[XmlAttribute("MinorVersion"), DefaultValue("0")]
			public string VersionAsString
			{
				get { return this.Version.ToString("X"); }
				set { this.Version = int.Parse(value, System.Globalization.NumberStyles.AllowHexSpecifier); }
			}

			public struct BrandEntry
			{
				[XmlAttribute]
				public string AlternateBrand;
			}

			[XmlElement("BrandEntry")]
			public BrandEntry[] AltBrands
			{
				get
				{
					return (CompatibleBrand ?? new AtomicCode[0]).Select(brand => new BrandEntry { AlternateBrand = brand.ToString() }).ToArray();
				}
				set
				{
					CompatibleBrand = value.Select(brand => new AtomicCode(brand.AlternateBrand)).ToArray();
				}
			}
		}

		// Handler Reference Atom
		public sealed partial class HandlerBox : ISOMFullBox
		{
			[XmlAttribute("ComponentType"), DefaultValue("")]
			public string ComponentTypeAsString
			{
				get { return this.ComponentType.ToString(); }
				set { this.ComponentType = new AtomicCode(value); }
			}
			[XmlAttribute("Type"), DefaultValue("")]
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

		// Metadata Atom
		public sealed partial class MetaBox : ISOMFullBox, IBoxContainer
		{
#warning Looking forward to 'ID32', 'data'
			[XmlElement(typeof(HandlerBox))]
			[XmlElement(typeof(DataInformationBox))]
			[XmlElement(typeof(XMLBox))]
			[XmlElement(typeof(BinaryXMLBox))]
			[XmlElement(typeof(ItemLocationBox))]
			[XmlElement(typeof(PrimaryItemBox))]
			[XmlElement(typeof(ItemProtectionBox))]
			[XmlElement(typeof(ItemInfoBox))]
			[XmlElement(typeof(IPMPControlBox))]
			//[XmlElement(typeof(ID32))]
			[XmlElement(typeof(ItemListBox))]
			[XmlElement(typeof(UUIDBox))]
			[XmlElement("MPEG4ESDescriptorBox", typeof(ESDBox))]
			//[XmlElement(typeof(data))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class HintInfoBox : AtomicInfo, IBoxContainer
		{
			[XmlElement("LargeTotalRTPBytesBox", typeof(TRPYBox))]
			[XmlElement("LargeTotalPacketBox", typeof(NUMPBox))]
			[XmlElement("LargeTotalMediaBytesBox", typeof(NTYLBox))]
			[XmlElement("TotalRTPBytesBox", typeof(TOTLBox))]
			[XmlElement("TotalPacketBox", typeof(NPCKBox))]
			[XmlElement("MaxDataRateBox", typeof(MAXRBox))]
			[XmlElement("BytesFromMediaTrackBox", typeof(DMEDBox))]
			[XmlElement("ImmediateDataBytesBox", typeof(DIMMBox))]
			[XmlElement("RepeatedDataBytesBox", typeof(DREPBox))]
			[XmlElement("MinTransmissionTimeBox", typeof(TMINBox))]
			[XmlElement("MaxTransmissionTimeBox", typeof(TMAXBox))]
			[XmlElement("MaxPacketSizeBox", typeof(PMAXBox))]
			[XmlElement("MaxPacketDurationBox", typeof(DMAXBox))]
			[XmlElement("PayloadTypeBox", typeof(PAYTBox))]
			[XmlElement("TotalMediaBytesBox", typeof(TPAYBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class HintTrackInfoBox : AtomicInfo, IBoxContainer
		{
			//this is the value for GF_RTPBox - same as HintSampleEntry for RTP !!!
			[XmlElement("RTPInfoBox", typeof(RTPBox))]
			[XmlElement(typeof(SDPBox))]
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

		public sealed partial class RTPBox : AtomicInfo
		{
			[XmlAttribute("SubType")]
			public string SubTypeAsString
			{
				get { return SubType.ToString(); }
				set { SubType = new AtomicCode(value); }
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

		// Movie sample data — usually this data can be interpreted only by using the movie resource.
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
		}

		// Media Header Atom
		public sealed partial class MediaHeaderBox: ISOMFullBox
		{
			[XmlAttribute("LanguageCode"), DefaultValue("")]
			public string LanguageAsString { get { return this.Language.ToString(); } set { this.Language = new PackedLanguage(value); } }
		}

		public sealed partial class MediaBox : AtomicInfo, IBoxContainer
		{
			[XmlElement(typeof(MediaHeaderBox))]
			[XmlElement(typeof(HandlerBox))]
			[XmlElement(typeof(MediaInformationBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class MediaInformationBox : AtomicInfo, IBoxContainer
		{
			[XmlElement(typeof(MPEGMediaHeaderBox))]
			[XmlElement(typeof(VideoMediaHeaderBox))]
			[XmlElement(typeof(SoundMediaHeaderBox))]
			[XmlElement(typeof(HintMediaHeaderBox))]
			//[XmlElement(typeof(gmhd))]
			[XmlElement(typeof(DataInformationBox))]
			[XmlElement(typeof(SampleTableBox))]
			[XmlElement(typeof(HandlerBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		/*MovieFragment is a container IN THE FILE, contains 1 fragment*/
		public sealed partial class MovieFragmentBox : AtomicInfo, IBoxContainer
		{
			[XmlElement(typeof(MovieFragmentHeaderBox))]
			[XmlElement(typeof(TrackFragmentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		// The Movie Atom
		public sealed partial class MovieBox : AtomicInfo, IBoxContainer
		{
#warning Looking forward to 'drm', 'ipmc'
			[XmlElement(typeof(MovieHeaderBox))]
			[XmlElement(typeof(ObjectDescriptorBox))]
			//[XmlElement(typeof(drm ))]
			[XmlElement(typeof(MetaBox))]
			[XmlElement(typeof(MovieExtendsBox))]
			//[XmlElement(typeof(ipmc))]
			[XmlElement(typeof(TrackBox))]
			[XmlElement(typeof(UserDataBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public abstract partial class ISOMAudioSampleEntry : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public bool Reserved2Specified
			{
				get { return !Reserved2.IsZero(); }
			}
		}

		public sealed partial class MPEGAudioSampleEntryBox : ISOMAudioSampleEntry, IBoxContainer
		{
			[XmlElement("MPEG4ESDescriptorBox", typeof(ESDBox))]
			[XmlElement(typeof(ProtectionInfoBox))]
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
		}

		public abstract partial class ISOMSampleEntryFields : ISOMUUIDBox
		{
			[XmlIgnore]
			public bool ReservedSpecified
			{
				get { return !Reserved.IsZero(); }
			}
		}


		/*for most MPEG4 media */
		public sealed partial class MPEGSampleEntryBox : ISOMSampleEntryFields, IBoxContainer
		{
			[XmlElement("MPEG4ESDescriptorBox", typeof(ESDBox))]
			[XmlElement(typeof(ProtectionInfoBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public abstract partial class ISOMVisualSampleEntry : ISOMSampleEntryFields
		{
			[XmlAttribute("XDPI")]
			public double HorizResAsFloat { get { return (double)HorizRes; } set { HorizRes = (Fixed<uint, x16>)value; } }

			[XmlAttribute("YDPI")]
			public double VertResAsFloat { get { return (double)VertRes; } set { VertRes = (Fixed<uint, x16>)value; } }

			[XmlAttribute("CompressorName"), DefaultValue("")]
			public string CompressorNameAsString { get { return CompressorName; } set { CompressorName = new UTF8PadString(value, 32); } }
		}


		public sealed partial class MPEGVisualSampleEntryBox : ISOMVisualSampleEntry, IBoxContainer
		{
			[XmlElement(typeof(ProtectionInfoBox))]
			[XmlElement(typeof(PixelAspectRatioBox))]
			[XmlElement(typeof(RVCConfigurationBox))]
			[XmlElement("MPEG4ESDescriptorBox", typeof(ESDBox))]
			[XmlElement(typeof(AVCConfigurationBox))]
			[XmlElement(typeof(MPEG4BitRateBox))]
			[XmlElement(typeof(MPEG4ExtensionDescriptorsBox))]
			public Collection<AtomicInfo> Boxes
			{
				get
				{
					/*this is an AVC sample desc*/
					//if (AVCConfig != null || SVCConfig != null) AVC_RewriteESDescriptor(this);
					return this.boxList;
				}
			}
		}

		public sealed partial class MovieExtendsBox : AtomicInfo, IBoxContainer
		{
			[XmlElement(typeof(TrackExtendsBox))]
			[XmlElement(typeof(MovieExtendsHeaderBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		/*the TrackExtends contains default values for the track fragments*/
		public sealed partial class TrackExtendsBox : ISOMFullBox
		{
			const int ReservedDefaultSampleFlags = 7 << 28 | 1 << 15;

			public struct SampleFlagsEntry
			{
				[XmlIgnore]
				public int Flags;
				[XmlAttribute, DefaultValue(0)]
				public int ReservedFlags
				{
					get
					{
						return
							Flags.Bits(2, 24).UnknownEnum<SampleDependsOnFlags>() << 24 |
							Flags.Bits(2, 22).UnknownEnum<SampleIsDependedOnFlags>() << 22 |
							Flags.Bits(2, 20).UnknownEnum<SampleHasRedundancyFlags>() << 20 |
							Flags & ReservedDefaultSampleFlags;
					}
					set { Flags |= (int)value; }
				}
				[XmlAttribute, DefaultValue((sbyte)0)]
				public sbyte IsLeading
				{
					get { return (sbyte)Flags.Bits(2, 26); }
					set { Flags = Flags.Bits(2, 26, value); }
				}
				[XmlAttribute, DefaultValue(SampleDependsOnFlags.Unknown)]
				public SampleDependsOnFlags SampleDependsOn
				{
					get { return Flags.Bits(2, 24).ValidEnum(SampleDependsOnFlags.Unknown); }
					set { Flags = Flags.Bits(2, 24, (int)value); }
				}
				[XmlAttribute, DefaultValue(SampleIsDependedOnFlags.Unknown)]
				public SampleIsDependedOnFlags SampleIsDependedOn
				{
					get { return Flags.Bits(2, 22).ValidEnum(SampleIsDependedOnFlags.Unknown); }
					set { Flags = Flags.Bits(2, 22, (int)value); }
				}
				[XmlAttribute, DefaultValue(SampleHasRedundancyFlags.Unknown)]
				public SampleHasRedundancyFlags SampleHasRedundancy
				{
					get { return Flags.Bits(2, 20).ValidEnum(SampleHasRedundancyFlags.Unknown); }
					set { Flags = Flags.Bits(2, 20, (int)value); }
				}
				[XmlAttribute, DefaultValue((sbyte)0)]
				public sbyte SamplePadding
				{
					get { return (sbyte)Flags.Bits(3, 17); }
					set { Flags = Flags.Bits(3, 17, value); }
				}
				[XmlAttribute, DefaultValue(false)]
				public bool SampleSync
				{
					get { return Flags.Bit(16); }
					set { Flags = Flags.Bit(16, value); }
				}
				[XmlAttribute, DefaultValue((short)0)]
				public short SampleDegradationPriority
				{
					get { return (short)Flags.Bits(15, 0); }
					set { Flags = Flags.Bits(15, 0); }
				}
			}

			[XmlElement("DefaultSampleFlags")]
			public SampleFlagsEntry DefaultSampleFlagsEntry
			{
				get { return new SampleFlagsEntry { Flags = DefaultSampleFlags }; }
				set { DefaultSampleFlags = value.Flags; }
			}
		}

		// Movie Header Atom
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
			public struct PaddingBitsEntry
			{
				[XmlAttribute]
				public sbyte PaddingBits;
			}

			[XmlElement("PaddingBitsEntry")]
			public PaddingBitsEntry[] Entries
			{
				get { return (PadBits ?? new sbyte[0]).Select(bits => new PaddingBitsEntry { PaddingBits = bits }).ToArray(); }
				set { PadBits = value.Select(entry => entry.PaddingBits).ToArray(); }
			}
		}

		// Sound Media Information Header Atom
		public sealed partial class SoundMediaHeaderBox: ISOMFullBox
		{
			[XmlAttribute("Balance"), DefaultValue(0.0)]
			public double BalanceAsFloat { get { return (double)Balance; } set { Balance = value.ToFixed16(); } }
		}

		// Sample Table Atom
		public sealed partial class SampleTableBox : AtomicInfo, IBoxContainer
		{
#warning Looking forward to 'cslg', 'stps'
			[XmlElement(typeof(SampleDescriptionBox))]
			[XmlElement(typeof(TimeToSampleBox))]
			[XmlElement(typeof(CompositionOffsetBox))]
			//[XmlElement(typeof(cslg))]
			[XmlElement(typeof(SyncSampleBox))]
			//[XmlElement(typeof(stps))]
			[XmlElement(typeof(SampleToChunkBox))]
			[XmlElement("SampleSizeBox", typeof(FixedSampleSizeBox))]
			[XmlElement("CompactSampleSizeBox", typeof(CompactSampleSizeBox))]
			[XmlElement(typeof(ChunkOffsetBox))]
			[XmlElement(typeof(ChunkLargeOffsetBox))]//WARNING: AS THIS MAY CHANGE DYNAMICALLY DURING EDIT
			[XmlElement(typeof(ShadowSyncBox))]
			[XmlElement(typeof(SampleGroupDescriptionBox))]
			[XmlElement(typeof(SampleGroupBox))]
			[XmlElement(typeof(SampleDependencyTypeBox))]
			[XmlElement(typeof(DegradationPriorityBox))]
			[XmlElement(typeof(PaddingBitsBox))]
			[XmlElement(typeof(SubSampleInformationBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
		}

		public sealed partial class ChunkOffsetBox : ISOMFullBox
		{
			public struct ChunkEntry
			{
				[XmlAttribute]
				public int ChunkNumber;
				[XmlAttribute]
				public int Offset;
			}

			[XmlElement("ChunkEntry")]
			public ChunkEntry[] Entries
			{
				get { return (Offsets ?? new int[0]).Select((offset, index) => new ChunkEntry { ChunkNumber = index + 1, Offset = offset }).ToArray(); }
				set { Offsets = value.Select(entry => entry.Offset).ToArray(); }
			}
		}

		public sealed partial class ChunkLargeOffsetBox : ISOMFullBox
		{
			public struct ChunkLargeEntry
			{
				[XmlAttribute]
				public int ChunkNumber;
				[XmlAttribute]
				public long Offset;
			}

			[XmlElement("ChunkEntry")]
			public ChunkLargeEntry[] Entries
			{
				get { return (Offsets ?? new long[0]).Select((offset, index) => new ChunkLargeEntry { ChunkNumber = index + 1, Offset = offset }).ToArray(); }
				set { Offsets = value.Select(entry => entry.Offset).ToArray(); }
			}
		}

		public sealed partial class DegradationPriorityBox : ISOMFullBox
		{
			public struct DegradationPriorityEntry
			{
				[XmlAttribute]
				public short DegradationPriority;
			}

			[XmlElement("DegradationPriorityEntry")]
			public DegradationPriorityEntry[] Entries
			{
				get { return (Priorities ?? new short[0]).Select(priority => new DegradationPriorityEntry { DegradationPriority = priority }).ToArray(); }
				set { Priorities = value.Select(entry => entry.DegradationPriority).ToArray(); }
			}
		}

		public sealed partial class SampleToChunkBox : ISOMFullBox
		{
			[XmlElement("SampleToChunkEntry")]
			public List<StscEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class SampleDescriptionBox : ISOMFullBox, IBoxContainer
		{
#warning Looking forward to 'enct', 'drms', 'drmi', 'alac', 'avcp', 'jpeg', 'srtp', 'samr', 'sawb', 'sawp', 's263', 'sevc', 'sqcp', 'ssmv', 'tmcd', 'mjp2'
			[XmlElement("MPEGSystemsSampleDescriptionBox", typeof(MPEGSampleEntryBox))]
			[XmlElement("MPEGAudioSampleDescriptionBox", typeof(MPEGAudioSampleEntryBox))]
			[XmlElement("MPEGVisualSampleDescriptionBox", typeof(MPEGVisualSampleEntryBox))]
			[XmlElement("GenericHintSampleEntryBox", typeof(HintSampleEntryBox))]
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
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxArray; }
			}
		}

		public sealed partial class StsfEntry
		{
			public struct FragmentSizeEntry
			{
				[XmlAttribute]
				public ushort Size;
			}

			[XmlElement("FragmentSizeEntry")]
			public FragmentSizeEntry[] Entries
			{
				get { return (FragmentSizes ?? new ushort[0]).Select(size => new FragmentSizeEntry { Size = size }).ToArray(); }
				set { FragmentSizes = value.Select(entry => entry.Size).ToArray(); }
			}
		}

		public sealed partial class SampleFragmentBox : ISOMFullBox
		{
			[XmlElement("SampleFragmentEntry")]
			public List<StsfEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class ShadowSyncBox : ISOMFullBox
		{
			[XmlElement("SyncShadowEntry")]
			public List<StshEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class SyncSampleBox : ISOMFullBox
		{
			public struct SyncSampleEntry
			{
				[XmlAttribute]
				public int SampleNumber;
			}

			[XmlElement("SyncSampleEntry")]
			public SyncSampleEntry[] Entries
			{
				get { return (SampleNumbers ?? new int[0]).Select(sampleNumber => new SyncSampleEntry { SampleNumber = sampleNumber }).ToArray(); }
				set { SampleNumbers = value.Select(entry => entry.SampleNumber).ToArray(); }
			}
		}

		public abstract partial class SampleSizeBox : ISOMFullBox
		{
			public struct SampleSizeEntry
			{
				[XmlAttribute]
				public int SampleNumber;
				[XmlAttribute]
				public int Size;
			}

			[XmlElement("SampleSizeEntry")]
			public SampleSizeEntry[] Entries
			{
				get { return (Sizes ?? new int[0]).Select((size, index) => new SampleSizeEntry { SampleNumber = index + 1, Size = size }).ToArray(); }
				set { Sizes = value.Select(entry => entry.Size).ToArray(); }
			}
		}

		public sealed partial class TimeToSampleBox : ISOMFullBox
		{
			[XmlElement("TimeToSampleEntry")]
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
			public override int ReservedFlags
			{
				get { return Flags.UnknownFlags<TrackFragmentFlags>(); }
				set { Flags = value; }
			}

			[XmlElement("DefaultSampleFlags")]
			public TrackExtendsBox.SampleFlagsEntry DefaultSampleFlagsEntry
			{
				get { return new TrackExtendsBox.SampleFlagsEntry { Flags = DefaultSampleFlags }; }
				set { DefaultSampleFlags = value.Flags; }
			}
		}

		// Track Header Atom
		public sealed partial class TrackHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			public override AtomFlags AtomFlags { get { return MP4.AtomFlags.Binary; } set { } }

			[XmlAttribute]
			public override int ReservedFlags
			{
				get { return Flags.UnknownFlags<TrackFlags>(); }
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
			[XmlElement(typeof(TrackFragmentHeaderBox))]
			[XmlElement("TrackRunBox", typeof(TrackFragmentRunBox))]
			[XmlElement(typeof(SampleDependencyTypeBox))]
			[XmlElement(typeof(SubSampleInformationBox))]
			[XmlElement(typeof(SampleGroupBox))]
			[XmlElement(typeof(SampleGroupDescriptionBox))]
			[XmlElement("TrackFragmentBaseMediaDecodeTimeBox", typeof(TFBaseMediaDecodeTimeBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		// Track Atom
		public sealed partial class TrackBox : AtomicInfo, IBoxContainer
		{
#warning Looking forward to 'tapt'
			[XmlElement(typeof(TrackHeaderBox))]
			[XmlElement(typeof(EditBox))]
			[XmlElement(typeof(TrackReferenceBox))]
			[XmlElement(typeof(MediaBox))]
			[XmlElement(typeof(UserDataBox))]
			[XmlElement(typeof(MetaBox))]
			//[XmlElement(typeof(tapt))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}
#line 6200 "box_code_base.c"
#warning Lines 6200 - 6302 gf_isom_check_sample_desc method ('vide'=>GenericVisualSampleEntryBox; 'soun'=>GenericAudioSampleEntryBox; GenericSampleEntryBox) isn't implemented
#line default
		}

		// Track Reference Atom
		public sealed partial class TrackReferenceBox : AtomicInfo
		{
			[XmlElement(typeof(TrackReferenceTypeBox))]
			[XmlElement(typeof(HintTrackReferenceBox))]
			[XmlElement(typeof(StreamTrackReferenceBox))]
			[XmlElement(typeof(ODTrackReferenceBox))]
			[XmlElement(typeof(SyncTrackReferenceBox))]
			[XmlElement(typeof(ChapterTrackReferenceBox))]
			public BoxCollection<TrackReferenceTypeBox> Boxes
			{
				get { return this.boxList; }
			}
		}

		// Track reference type atom
		public partial class TrackReferenceTypeBox : AtomicInfo
		{
			[XmlAttribute("Tracks")]
			public string TrackIDsAsString
			{
				get { return TrackIDs.ToXML(); }
				set { value.FromXML(out TrackIDs); }
			}
		}

		public sealed partial class TrackFragmentRunBox : ISOMFullBox
		{
			[XmlAttribute]
			public override AtomFlags AtomFlags { get { return MP4.AtomFlags.Binary; } set { } }

			[XmlAttribute]
			public override int ReservedFlags
			{
				get { return Flags.UnknownFlags<TrackRunFlags>(); }
				set { Flags = value; }
			}

			[XmlElement("FirstSampleFlags")]
			public TrackExtendsBox.SampleFlagsEntry FirstSampleFlagsEntry
			{
				get { return new TrackExtendsBox.SampleFlagsEntry { Flags = FirstSampleFlags }; }
				set { FirstSampleFlags = value.Flags; }
			}

			public bool FirstSampleFlagsEntrySpecified { get { return (TrackRunFlags & TrackRunFlags.FirstFlag) != 0; } }

			[XmlElement("TrackRunEntry")]
			public Collection<TrunEntry> Entries
			{
				get { return this.entries; }
			}
		}

		public sealed partial class TrunEntry
		{
			[XmlElement("SampleFlags")]
			public TrackExtendsBox.SampleFlagsEntry SampleFlagsEntry
			{
				get { return new TrackExtendsBox.SampleFlagsEntry { Flags = Flags }; }
				set { Flags = value.Flags; }
			}

			public bool SampleFlagsEntrySpecified { get { return (Owner.TrackRunFlags & TrackRunFlags.Flags) != 0; } }
		}

		// Used to classify boxes in the UserData Box
		public sealed partial class UserDataMap
		{
#warning Looking forward to 'titl', 'auth', 'perf', 'gnre', 'dscp', 'albm', 'yrrc', 'rtng', 'clsf', 'kywd', 'loci', 'tsel', 'data'
			[XmlElement(typeof(MetaBox))]
			[XmlElement(typeof(HintTrackInfoBox))]
			[XmlElement(typeof(HintInfoBox))]
			[XmlElement(typeof(NameBox))]
			[XmlElement(typeof(CopyrightBox))]
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
			[XmlElement(typeof(UUIDBox))]
			[XmlElement("MPEG4ESDescriptorBox", typeof(ESDBox))]
			//[XmlElement(typeof(data))]
			[XmlElement(typeof(ChapterListBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}

			[XmlAttribute("Type")]
			public string BoxTypeAsString
			{
				get { return BoxType.ToString(); }
				set { BoxType = new AtomicCode(value); }
			}

			public bool UUIDSpecified
			{
				get { return UUID != Guid.Empty; }
			}
		}

		// User Data Atom
		public sealed partial class UserDataBox: AtomicInfo, IBoxContainer
		{
			[XmlElement("UDTARecord")]
			public List<UserDataMap> RecordList
			{
				//warning: here we are not passing the actual "parent" of the list
				//but the UDTA box. The parent itself is not an box, we don't care about it
				get { return this.boxList.maps; }
			}
		}

		public sealed partial class ProgressiveDownloadBox : ISOMFullBox
		{
			[XmlElement("DownloadInfo")]
			public DownloadInfo[] Infos
			{
				get { return this.infos; }
			}
		}

		public sealed partial class SampleDependencyTypeBox : ISOMFullBox
		{
			const int ReservedSampleDependency = 1 << 7;

			public struct SampleDependencyEntry
			{
				[XmlAttribute]
				public int SampleNumber;
				[XmlIgnore]
				// bit 0x80 is reserved; bit combinations 0x30, 0xC0 and 0x03 are reserved
				public byte Flags;
				[XmlAttribute, DefaultValue(0)]
				public int ReservedFlags
				{
					get
					{
						return
							Flags.Bits(2, 4).UnknownEnum<SampleDependsOnFlags>() << 4 |
							Flags.Bits(2, 2).UnknownEnum<SampleIsDependedOnFlags>() << 2 |
							Flags.Bits(2, 0).UnknownEnum<SampleHasRedundancyFlags>() |
							Flags & ReservedSampleDependency;
					}
					set { Flags |= (byte)value; }
				}
				[XmlAttribute, DefaultValue(false)]
				public bool EarlierDisplayTimesAllowed
				{
					get { return Flags.Bit(6); }
					set { Flags = Flags.Bit(6, value); }
				}
				[XmlAttribute, DefaultValue(SampleDependsOnFlags.Unknown)]
				public SampleDependsOnFlags DependsOn
				{
					get { return Flags.Bits(2, 4).ValidEnum(SampleDependsOnFlags.Unknown); }
					set { Flags = Flags.Bits(2, 4, (int)value); }
				}
				[XmlAttribute, DefaultValue(SampleIsDependedOnFlags.Unknown)]
				public SampleIsDependedOnFlags IsDependedOn
				{
					get { return Flags.Bits(2, 2).ValidEnum(SampleIsDependedOnFlags.Unknown); }
					set { Flags = Flags.Bits(2, 2, (int)value); }
				}
				[XmlAttribute, DefaultValue(SampleHasRedundancyFlags.Unknown)]
				public SampleHasRedundancyFlags HasRedundancy
				{
					get { return Flags.Bits(2, 0).ValidEnum(SampleHasRedundancyFlags.Unknown); }
					set { Flags = Flags.Bits(2, 0, (int)value); }
				}
			}

			[XmlElement("SampleDependencyEntry")]
			public SampleDependencyEntry[] Entries
			{
				get { return (SampleInfo ?? new byte[0]).Select((info, index) => new SampleDependencyEntry { SampleNumber = index, Flags = info }).ToArray(); }
				set { SampleInfo = value.Select(entry => entry.Flags).ToArray(); }
			}
		}

		public sealed partial class OriginalFormatBox : AtomicInfo
		{
			[XmlAttribute("DataFormat"), DefaultValue("")]
			public string DataFormatAsString { get { return DataFormat.ToString(); } set { DataFormat = new AtomicCode(value); } }
		}

		public sealed partial class SchemeTypeBox : ISOMFullBox
		{
			[XmlAttribute("SchemeType"), DefaultValue("")]
			public string SchemeTypeAsString { get { return SchemeType.ToString(); } set { SchemeType = new AtomicCode(value); } }
		}

		public sealed partial class SchemeInformationBox : AtomicInfo
		{
			[XmlElement("KMSBox", typeof(ISMAKMSBox))]
			[XmlElement(typeof(ISMASampleFormatBox))]
			[XmlElement("OMADRMAUFormatBox", typeof(OMADRMKMSBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		public sealed partial class OMADRMCommonHeaderBox : ISOMFullBox
		{
			[XmlAttribute("TextualHeaders"), DefaultValue("")]
			public string TextualHeadersAsString
			{
				get { return String.Join(" ", TextualHeaders); }
				set { TextualHeaders = value.Split(' '); }
			}
		}

		public sealed partial class TrackSelectionBox : ISOMFullBox
		{
			[XmlAttribute("Criteria"), DefaultValue("")]
			public string AttributeListAsString
			{
				get { return String.Join(" ", AttributeList); }
				set { AttributeList = value.Split(' ').Select(code => new AtomicCode(code)).ToArray(); }
			}
		}

		public sealed partial class AC3SampleEntryBox : ISOMAudioSampleEntry, IBoxContainer
		{
			[XmlElement("AC3SpecificBox", typeof(AC3ConfigBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		public sealed partial class LASeRSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlElement(typeof(LASeRConfigurationBox))]
			[XmlElement(typeof(MPEG4BitRateBox))]
			[XmlElement(typeof(MPEG4ExtensionDescriptorsBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return boxList; }
			}
		}

		public sealed partial class PcrInfoBox : AtomicInfo
		{
			public struct PCRInfo
			{
				[XmlAttribute]
				public long PCR;
			}

			[XmlElement("PCRInfo")]
			public PCRInfo[] PcrValues
			{
				get { return (pcrValues ?? new ulong[0]).Select(d => new PCRInfo { PCR = (long)(d >> 22) }).ToArray(); }
				set { pcrValues = value.Select(entry => (ulong)entry.PCR << 22).ToArray(); }
			}
		}

		public sealed partial class SampleEntry : IEntry<SubSampleInformationBox>
		{
			[XmlElement("SubSample")]
			public Collection<SubSampleEntry> SubSamples
			{
				get { return this.subSamples; }
			}
		}

		public sealed partial class SubSampleInformationBox : ISOMFullBox
		{
			[XmlElement("SampleEntry")]
			public Collection<SampleEntry> Samples
			{
				get { return this.samples; }
			}
		}

		public sealed partial class SampleGroupBox : ISOMFullBox
		{
			[XmlAttribute("GroupingType"), DefaultValue("")]
			public string GroupingTypeAsString { get { return GroupingType.ToString(); } set { GroupingType = new AtomicCode(value); } }

			public bool GroupingTypeParameterSpecified { get { return Version == 1; } }

			[XmlElement("SampleGroupBoxEntry")]
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

			public bool DefaultLengthSpecified { get { return Version == 1; } }

			[XmlElement(typeof(DefaultSampleGroupDescriptionEntry))]
			[XmlElement(typeof(VisualRandomAccessEntry))]
			[XmlElement(typeof(RollRecoveryEntry))]
			public Collection<SampleGroupDescriptionEntry> GroupDescriptions
			{
				get { return this.groupDescriptions; }
			}
		}

		public sealed partial class HintSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlElement("RTPTimeScaleBox", typeof(TSHintEntryBox))]
			[XmlElement("TimeStampOffsetBox", typeof(TimeOffHintEntryBox))]
			[XmlElement("PacketSequenceOffsetBox", typeof(SeqOffHintEntryBox))]
			public Collection<AtomicInfo> HintDataTable
			{
				get { return this.boxList; }
			}
		}
	}
}
