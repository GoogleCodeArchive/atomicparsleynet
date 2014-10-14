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
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		internal static readonly byte[] EmptyData = new byte[0];

		public abstract partial class ISOMFullBox: AtomicInfo
		{
			public ISOMFullBox() { }

			protected ISOMFullBox(int version)
			{
				this.Version = version;
			}

			protected ISOMFullBox(string type) : base(type) { }

			protected ISOMFullBox(string type, int version)
				: base(type)
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
			protected ISOMUUIDBox() { }
			protected ISOMUUIDBox(string type) : base(type) { }
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

		public abstract partial class ISOMDataEntryFields : ISOMFullBox
		{
			protected ISOMDataEntryFields(string type) : base(type) { }
			internal ISOMDataEntryFields() { }
		}

		public sealed partial class EditListBox : ISOMFullBox
		{
			private void ResolveVersion()
			{
				if (entryList.Any(
					entry => entry.SegmentDuration > uint.MaxValue ||
					entry.MediaTime > uint.MaxValue))
					Version = 1;
				else
					Version = 0;
			}
		}

		public sealed partial class ESDBox : ISOMFullBox
		{
#line 1053 "box_code_base.c"
#warning ESDBox should be implemented with ODF
#line default
		}

		// Unused space available in file.
		public sealed partial class FreeSpaceBox: AtomicInfo
		{
			public FreeSpaceBox(int size)
			{
				this.Data = new byte[size];
			}
		}

		// The File Type Compatibility Atom
		public sealed partial class FileTypeBox: AtomicInfo
		{
			internal static readonly AtomicCode[] EmptyBrands = new AtomicCode[0];

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

		//TODO: ODF implementation
		public sealed partial class ObjectDescriptorBox : ISOMFullBox
		{
#line 2635 "box_code_base.c"
#warning ObjectDescriptorBox isn't implemented
#line default
		}

		// Movie sample data — usually this data can be interpreted only by using the movie resource.
		public sealed partial class MediaDataBox: AtomicInfo
		{
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

		public sealed partial class MPEGAudioSampleEntryBox : ISOMAudioSampleEntry, IBoxContainer
		{
			private MPEGAudioSampleEntryBox(string type) : base(type) { }
			public MPEGAudioSampleEntryBox() { }

			public static MPEGAudioSampleEntryBox CreateMP4A()
			{
				return new MPEGAudioSampleEntryBox("mp4a");
			}

			public static MPEGAudioSampleEntryBox CreateENCA()
			{
				return new MPEGAudioSampleEntryBox("enca");
			}
		}

		/*for most MPEG4 media */
		public sealed partial class MPEGSampleEntryBox : ISOMSampleEntryFields, IBoxContainer
		{
			private MPEGSampleEntryBox(string type) : base(type) { }
			public MPEGSampleEntryBox() { }

			public static MPEGSampleEntryBox CreateMP4S()
			{
				return new MPEGSampleEntryBox("mp4s");
			}

			public static MPEGSampleEntryBox CreateENCS()
			{
				return new MPEGSampleEntryBox("encs");
			}
		}

		public sealed partial class MPEGVisualSampleEntryBox : ISOMVisualSampleEntry, IBoxContainer
		{
			private MPEGVisualSampleEntryBox(string type) : base(type) { }
			public MPEGVisualSampleEntryBox() { }

			public static MPEGVisualSampleEntryBox CreateMP4V()
			{
				return new MPEGVisualSampleEntryBox("mp4v");
			}

			public static MPEGVisualSampleEntryBox CreateAVC1()
			{
				return new MPEGVisualSampleEntryBox("avc1");
			}

			public static MPEGVisualSampleEntryBox CreateAVC2()
			{
				return new MPEGVisualSampleEntryBox("avc2");
			}

			public static MPEGVisualSampleEntryBox CreateSVC1()
			{
				return new MPEGVisualSampleEntryBox("svc1");
			}

			public static MPEGVisualSampleEntryBox CreateENCV()
			{
				return new MPEGVisualSampleEntryBox("encv");
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

		public abstract partial class SampleSizeBox : ISOMFullBox
		{
			protected SampleSizeBox(string type) : base(type) { }

			public static SampleSizeBox Create(int[] sizes)
			{
				if (sizes == null) return new FixedSampleSizeBox();
				byte fieldSize = 4;
				int constantSampleSize = sizes[0];
				foreach (int size in sizes)
				{
					if (constantSampleSize != size) constantSampleSize = 0;
					if (size < 0xF) continue;
					//switch to 8-bit table
					else if (size <= 0xFF && fieldSize <= 8)
						fieldSize = 8;
					//switch to 16-bit table
					else if (size <= 0xFFFF && fieldSize <= 16)
						fieldSize = 16;
					//switch to 32-bit table
					else
						fieldSize = 32;
				}
				//if all samples are of the same size, switch to regular (more compact)
				if (constantSampleSize != 0)
				{
					return new FixedSampleSizeBox
					{
						ConstantSampleSize = constantSampleSize
					};
				}
				if (fieldSize == 32)
				{
					//oops, doesn't fit in a compact table
					return new FixedSampleSizeBox
					{
						Sizes = sizes
					};
				}
				//make sure we are a compact table (no need to change the mem representation)
				return new CompactSampleSizeBox
				{
					SampleSizeBits = fieldSize,
					Sizes = sizes
				};
			}
		}

		public sealed partial class CompactSampleSizeBox : SampleSizeBox
		{
			private void Read_Sizes(System.IO.BinaryReader reader)
			{
				int sampleCount = reader.ReadInt32();
				Sizes = new int[sampleCount];
				//no samples, no parsing pb
				if (sampleCount == 0) return;
				switch (SampleSizeBits)
				{
					case 4:
					case 8:
					case 16:
						break;
					default:
						//try to fix the file
						//no samples, no parsing pb
						if (sampleCount == 0)
						{
							SampleSizeBits = 16;
							return;
						}
						int estSize = (int)(reader.Length() / sampleCount);
						if (estSize == 0 && ((sampleCount + 1) / 2 == reader.Length()))
							SampleSizeBits = 4;
						else if (estSize == 1 || estSize == 2)
							SampleSizeBits = (byte)(8 * estSize);
						else
							throw new InvalidDataException("Invalid sample size in " + AtomicID);
						break;
				}
				switch (SampleSizeBits)
				{
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
				return SampleSizeBits == 4 ?
					4 + (Sizes.Length + 1) / 2 :
					4 + Sizes.Length * (SampleSizeBits / 8);
			}

			private void Write_Sizes(System.IO.BinaryWriter writer)
			{
				int sampleCount = Sizes.Length;
				writer.Write((int)sampleCount);
				switch (SampleSizeBits)
				{
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

		// Track Atom
		public sealed partial class TrackBox : AtomicInfo, IBoxContainer
		{
#line 6200 "box_code_base.c"
#warning Lines 6200 - 6302 gf_isom_check_sample_desc method ('vide'=>GenericVisualSampleEntryBox; 'soun'=>GenericAudioSampleEntryBox; GenericSampleEntryBox) isn't implemented
#line default
		}

		// Track Reference Atom
		public sealed partial class TrackReferenceBox : AtomicInfo, IBoxContainer
		{
			Collection<AtomicInfo> IBoxContainer.Boxes { get { return this.boxList; } }
		}

		// Track reference type atom
		public partial class TrackReferenceTypeBox : AtomicInfo
		{
			public int AddRefTrack(int trackID)
			{
				int i = Array.IndexOf(this.TrackIDs, trackID);
				if (i >= 0) return i;
				this.TrackIDs = TrackIDs.Concat(new int[] { trackID }).ToArray();
				return this.TrackIDs.Length - 1;
			}
		}

		// Used to classify boxes in the UserData Box
		public sealed partial class UserDataMap
		{
			public UserDataMap(AtomicCode boxType)
			{
				this.BoxType = boxType;
			}

			public UserDataMap(Guid uuid)
			{
				this.BoxType = (AtomicCode)UUIDBox.DefaultID;
				this.UUID = uuid;
			}

			public UserDataMap() { }
		}

		// User Data Atom
		public sealed partial class UserDataBox: AtomicInfo, IBoxContainer
		{
			#region UserDataMap collection
			private class MapList : Collection<AtomicInfo>
			{
				internal List<UserDataMap> maps;

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
			public override void ReadBinary(BinaryReader reader)
			{
				base.ReadBinary(reader);
				int count = (int)(reader.Length() / 8);
				infos = new DownloadInfo[count];
				for (int index = 0; index < count; index++)
				{
					DownloadInfo item;
					item.Rate = reader.ReadInt32();
					item.EstimatedTime = reader.ReadInt32();
					infos[index] = item;
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
				int count = infos.Length;
				for(int index = 0; index < count; index++)
				{
					var item = infos[index];
					writer.Write((int)item.Rate);
					writer.Write((int)item.EstimatedTime);
				}
			}
		}

		public sealed partial class AC3SampleEntryBox : ISOMAudioSampleEntry, IBoxContainer
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
		}

		public sealed partial class LASeRSampleEntryBox : ISOMSampleEntryFields
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
		}

		public sealed partial class SampleEntry : IEntry<SubSampleInformationBox>
		{
			SubSampleInformationBox IEntry<SubSampleInformationBox>.Owner
			{
				get { return null; }
				set { AtomicInfo.CreateEntryCollection(out subSamples, value); }
			}
		}

		public sealed partial class SampleGroupDescriptionBox : ISOMFullBox
		{
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
		}
	}
}
