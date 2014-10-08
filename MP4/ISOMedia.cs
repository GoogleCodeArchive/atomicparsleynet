/*
 *			GPAC - Multimedia Framework C SDK - isomedia_dev.h
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using FRAFV.Binary.Serialization;

namespace MP4
{
	/*AC3 config record*/
	public struct AC3Config
	{
		public byte fscod;
		public byte bsid;
		public byte bsmod;
		public byte acmod;
		public byte lfon;
		public byte brcode;
	}

	[BinBlock(BinaryReaderType = "BinStringReader", BinaryWriterType = "BinStringWriter")]
	public sealed partial class ISOMediaBoxes
	{
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public abstract partial class ISOMFullBox: AtomicInfo
		{
			[XmlAttribute, DefaultValue(true)]
			public bool Versioned = true;

			/// <summary>
			/// A 1-byte specification of the version of this atom.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.UInt8, Condition = "Versioned")]
			public int Version;

			/// <summary>
			/// Three bytes of space for future flags.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt24, Condition = "Versioned")]
			public int Flags; //used by versioned atoms and derivatives
		}

		public abstract partial class ISOMUUIDBox: AtomicInfo
		{
			[XmlIgnore]
			public Guid UUID;
		}

		public sealed partial class Box: AtomicInfo
		{
		}

		public sealed partial class FullBox: ISOMFullBox
		{
		}

		public sealed partial class UUIDBox: ISOMUUIDBox
		{
			internal const string DefaultID = "uuid";

			public byte[] Data;
		}

		/// <summary>
		/// Movie sample data — usually this data can be interpreted only by using the movie resource.
		/// </summary>
		/// <remarks>
		/// Note: The data is NEVER loaded to the mdat in this lib.
		/// </remarks>
		public sealed partial class MediaDataBox: AtomicInfo
		{
			internal const string DefaultID = "mdat";

			[XmlIgnore, DefaultValue(0L)]
			public long Offset;
			[XmlIgnore, DefaultValue(0L)]
			public long MediaDataSize;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class UnknownBox: AtomicInfo
		{
			[XmlIgnore]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data = EmptyData;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class UnknownParentBox: AtomicInfo
		{
			[XmlIgnore]
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class UnknownUUIDBox : ISOMUUIDBox
		{
			[XmlIgnore]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		/// <summary>
		/// Movie Header Atom
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion(Duration)")]
		public sealed partial class MovieHeaderBox: ISOMFullBox
		{
			/// <summary>
			/// A 32-bit integer that specifies (in seconds since midnight, January 1, 1904) when the movie atom was created.
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime CreationTime;
			/// <summary>
			/// A 32-bit integer that specifies (in seconds since midnight, January 1, 1904) when the movie atom was changed.
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime ModificationTime;
			/// <summary>
			/// A time value that indicates the time scale for this movie—that is, the number of time units that pass per second in its time 
			/// coordinate system. A time coordinate system that measures time in sixtieths of a second, for example, has a time scale of 60.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int TimeScale;
			/// <summary>
			/// A time value that indicates the duration of the movie in time scale units. Note that this property is derived from the movie’s tracks.
			/// The value of this field corresponds to the duration of the longest track in the movie.
			/// </summary>
			[XmlAttribute, DefaultValue(0L)]
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long Duration;
			/// <summary>
			/// A 32-bit fixed-point number that specifies the rate at which to play this movie. A value of 1.0 indicates normal rate.
			/// </summary>
			[XmlIgnore]//, DefaultValue(1 << 16)]
			[BinData(BinFormat.UInt32)]
			public Fixed<uint, x16> PreferredRate = Fixed<uint, x16>.One;
			/// <summary>
			/// A 16-bit fixed-point number that specifies how loud to play this movie’s sound. A value of 1.0 indicates full volume.
			/// </summary>
			[XmlIgnore]//, DefaultValue(1 << 8)]
			[BinData(BinFormat.UInt16)]
			public Fixed<ushort, x8> PreferredVolume = Fixed<ushort, x8>.One;
			/// <summary>
			/// Ten bytes reserved. Set to 0.
			/// </summary>
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "10")]
			public byte[] Reserved;//[10];
			/// <summary>
			/// The matrix structure associated with this movie. A matrix shows how to map points from one coordinatespace into another.
			/// </summary>
			[XmlElement]
			[BinArray(CountCustomMethod = "9")]
			[BinData(BinFormat.UInt32)]
			public TransformMatrix Matrix = TransformMatrix.DefaultMatrix;//[9]
			/// <summary>
			/// The time value in the movie at which the preview begins.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int PreviewTime;
			/// <summary>
			/// The duration of the movie preview in movie time scale units.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int PreviewDuration;
			/// <summary>
			/// The time value of the time of the movie poster.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int PosterTime;
			/// <summary>
			/// The time value for the start time of the current selection.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int SelectionTime;
			/// <summary>
			/// The duration of the current selection in movie time scale units.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int SelectionDuration;
			/// <summary>
			/// The time value for current time position within the movie.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int CurrentTime;
			/// <summary>
			/// A 32-bit integer that indicates a value to use for the track ID number of the next track added to this movie. Note that 0 is not 
			/// a valid track ID value.
			/// </summary>
			[XmlAttribute, DefaultValue(1)]
			[BinData]
			public int NextTrackID = 1;
		}

		//TODO: ODF implementation
		public sealed partial class ObjectDescriptorBox : ISOMFullBox
		{
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] data;
#line 382 "isomedia_dev.h"
#warning ObjectDescriptorBox isn't implemented
			//	public Descriptor Descriptor;
#line default
		}

		[BinBlock(MethodMode = BinMethodMode.Final)]
		public sealed partial class EdtsEntry : IEntry<EditListBox>
		{
			public EditListBox Owner { get; set; }
			[XmlAttribute]
			[BinData(BinFormat.Int64, Condition = "Owner.Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long SegmentDuration;
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Owner.Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime MediaTime;
			[XmlAttribute]
			[BinData(BinFormat.UInt16)]
			public int MediaRate;
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved;
		}

		[BinBlock(GetDataSizeMethod = "ResolveVersion()")]
		public sealed partial class EditListBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinArray(CountFormat = BinFormat.Int32)]
			Collection<EdtsEntry> entryList;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class EditBox : AtomicInfo
		{
			[XmlIgnore]
			public EditListBox EditList { get { return boxList.Get<EditListBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				EditListBox>(AllowUnknownBox);
		}

		/// <summary>
		/// used to classify boxes in the UserData Box
		/// </summary>
		public sealed partial class UserDataMap
		{
			public AtomicCode BoxType;
			public Guid UUID;
			BoxCollection boxList = new BoxCollection();
		}

		/// <summary>
		/// User Data Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class UserDataBox: AtomicInfo
		{
			[BinCustom]
			MapList boxList = new MapList();
		}

		/// <summary>
		/// The Movie Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MovieBox : AtomicInfo
		{
			/// <summary>
			/// Profile atom
			/// </summary>
#warning Profile atom not found

			/// <summary>
			/// Movie header atom
			/// </summary>
			[XmlIgnore]
			public MovieHeaderBox MovieHeader { get { return boxList.Get<MovieHeaderBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Movie clipping atom
			/// </summary>
#warning Movie clipping atom not found

			/// <summary>
			/// Meta box if any
			/// </summary>
			[XmlIgnore]
			public MetaBox Meta { get { return boxList.Get<MetaBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public ObjectDescriptorBox ObjectDescriptor { get { return boxList.Get<ObjectDescriptorBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public MovieExtendsBox MovieExtends { get { return boxList.Get<MovieExtendsBox>(); } set { boxList.Set(value); } }

			/// <summary>
			/// Track atoms
			/// </summary>
			[XmlIgnore]
			public TrackBox[] TrackList;

			/// <summary>
			/// User data atom
			/// </summary>
			[XmlIgnore]
			public UserDataBox UserData { get { return boxList.Get<UserDataBox>(); } set { boxList.Set(value); } }

			/// <summary>
			/// Color table atom
			/// </summary>
#warning Color table atom not found

			/// <summary>
			/// Compressed movie atom
			/// </summary>
#warning Compressed movie atom not found

			/// <summary>
			/// Other boxes
			/// </summary>
			[XmlIgnore]
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				MovieHeaderBox,
				ObjectDescriptorBox,
				MetaBox,
				MovieExtendsBox,
				TrackBox[],
				UserDataBox>(AllowUnknownBox);

#warning Looking forward to ISOFile
			//public ISOFile Mov;

		}

		/// <summary>
		/// Track Header Atom
		/// </summary>
		public sealed partial class TrackHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			public TrackFlags TrackFlags { get { return (TrackFlags)Flags & TrackFlags.ValidMask; } set { Flags = (int)value; } }

			/// <summary>
			/// Creation time
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime CreationTime;
			/// <summary>
			/// Modification time
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime ModificationTime;
			/// <summary>
			/// Track ID
			/// </summary>
			[XmlAttribute]
			[BinData]
			public int TrackID;
			/// <summary>
			/// Reserved
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved1;
			/// <summary>
			/// Duration
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long Duration;
			/// <summary>
			/// Reserved
			/// </summary>
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "8")]
			public byte[] Reserved2;//[8]
			/// <summary>
			/// Layer
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Layer;
			/// <summary>
			/// Alternate group
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort AlternateGroup;
			/// <summary>
			/// Volume
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt16)]
			public Fixed<ushort, x8> Volume;
			/// <summary>
			/// Reserved
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved3;
			/// <summary>
			/// Matrix structure
			/// </summary>
			[BinArray(CountCustomMethod = "9")]
			[BinData(BinFormat.UInt32)]
			public TransformMatrix Matrix = TransformMatrix.DefaultMatrix;//[9]
			/// <summary>
			/// Track width
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Width;
			/// <summary>
			/// Track height
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Height;
		}

		/// <summary>
		/// Track Reference Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackReferenceBox : AtomicInfo
		{
			/// <summary>
			/// Track reference type atoms
			/// </summary>
			[BinCustom]
			BoxCollection<TrackReferenceTypeBox> boxList = new BoxCollection<TrackReferenceTypeBox>();
		}

		/// <summary>
		/// Track Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackBox : AtomicInfo
		{
			/// <summary>
			/// Track profile atom
			/// </summary>
#warning Track profile atom not found

			/// <summary>
			/// Track header atom
			/// </summary>
			[XmlIgnore]
			public TrackHeaderBox Header { get { return boxList.Get<TrackHeaderBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Track aperture mode dimensions atom
			/// </summary>
#warning Track aperture mode dimensions atom not found

			/// <summary>
			/// Clipping atom 
			/// </summary>
#warning Clipping atom  not found

			/// <summary>
			/// Track matte atom
			/// </summary>
#warning Track matte atom not found

			/// <summary>
			/// Edit atom
			/// </summary>
			[XmlIgnore]
			public EditBox EditBox { get { return boxList.Get<EditBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Track reference atom
			/// </summary>
			[XmlIgnore]
			public TrackReferenceBox References { get { return boxList.Get<TrackReferenceBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Track exclude from autoselection atom
			/// </summary>
#warning Track exclude from autoselection atom not found

			/// <summary>
			/// Track load settings atom
			/// </summary>
#warning Track load settings atom not found

			/// <summary>
			/// Track input map atom
			/// </summary>
#warning Track input map atom not found

			/// <summary>
			/// Media atom
			/// </summary>
			[XmlIgnore]
			public MediaBox Media { get { return boxList.Get<MediaBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// User-defined data atom
			/// </summary>
			[XmlIgnore]
			public UserDataBox UserData { get { return boxList.Get<UserDataBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Meta box if any
			/// </summary>
			[XmlIgnore]
			public MetaBox Meta { get { return boxList.Get<MetaBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Other
			/// </summary>
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				TrackHeaderBox,
				TrackReferenceBox,
				EditBox,
				MediaBox,
				MetaBox,
				UserDataBox>(AllowUnknownBox);
		}

		/// <summary>
		/// Media Header Atom
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion(Duration)")]
		public sealed partial class MediaHeaderBox: ISOMFullBox
		{
			/// <summary>
			/// A 32-bit integer that specifies (in seconds since midnight, January 1, 1904) when the media atom was created.
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime CreationTime;
			/// <summary>
			/// A 32-bit integer that specifies (in seconds since midnight, January 1, 1904) when the media atom was changed.
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime ModificationTime;
			/// <summary>
			/// A time value that indicates the time scale for this media—that is, the number of time units that pass per second in its time coordinate system.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int TimeScale;
			/// <summary>
			/// The duration of this media in units of its time scale.
			/// </summary>
			[XmlAttribute, DefaultValue(0L)]
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long Duration;
			/// <summary>
			/// A 16-bit integer that specifies the language code for this media.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt16)]
			public PackedLanguage Language;
			/// <summary>
			/// A 16-bit integer that specifies the media’s playback quality—that is, its suitability for playback in a given environment.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Quality;
		}

		/// <summary>
		/// Handler Reference Atom
		/// </summary>
		public sealed partial class HandlerBox : ISOMFullBox
		{
			/// <summary>
			/// Component type
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode ComponentType;
			/// <summary>
			/// Component subtype
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode HandlerType;
			/// <summary>
			/// Component manufacturer
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode Manufacturer;
			/// <summary>
			/// Component flags
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int ComponentFlags;
			/// <summary>
			/// Component flags mask
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int ComponentFlagsMask;
			/// <summary>
			/// Component name
			/// </summary>
			[XmlElement, DefaultValue("")]
			[BinData]
			public string ComponentName;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MediaBox : AtomicInfo
		{
			[XmlIgnore]
			public MediaHeaderBox MediaHeader { get { return boxList.Get<MediaHeaderBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public HandlerBox Handler { get { return boxList.Get<HandlerBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public MediaInformationBox Information { get { return boxList.Get<MediaInformationBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Other
			/// </summary>
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				MediaHeaderBox,
				HandlerBox,
				MediaInformationBox>(AllowUnknownBox);
		}

		/// <summary>
		/// Video Media Information Header Atom
		/// </summary>
		public sealed partial class VideoMediaHeaderBox: ISOMFullBox
		{
			/// <summary>
			/// A 16-bit integer that specifies the transfer mode. The transfer mode specifies which Boolean operation QuickDraw should perform when
			/// drawing or transferring an image from one location to another.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData(BinFormat.UInt16)]
			public GraphicsMode GraphicsMode;
			/// <summary>
			/// A 16-bit values that specify the red color for the transfer mode operation indicated in the graphics mode field.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort OpcolorRed;
			/// <summary>
			/// A 16-bit values that specify the green color for the transfer mode operation indicated in the graphics mode field.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort OpcolorGreen;
			/// <summary>
			/// A 16-bit values that specify the blue color for the transfer mode operation indicated in the graphics mode field.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort OpcolorBlue;
		}

		/// <summary>
		/// Sound Media Information Header Atom
		/// </summary>
		public sealed partial class SoundMediaHeaderBox: ISOMFullBox
		{
			/// <summary>
			/// A 16-bit integer that specifies the sound balance of this sound media. Sound balance is the setting that controls the mix of sound
			/// between the two speakers of a computer. This field is normally set to 0.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt16)]
			public Fixed<ushort, x8> Balance;
			/// <summary>
			/// Reserved for use. Set this field to 0.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved;
		}

		public sealed partial class HintMediaHeaderBox: ISOMFullBox
		{
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.UInt16)]
			public int MaxPDUSize;
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.UInt16)]
			public int AvgPDUSize;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int MaxBitrate;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int AvgBitrate;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int SlidingAverageBitrate;
		}

		public sealed partial class MPEGMediaHeaderBox: ISOMFullBox
		{
		}

		public sealed partial class ODMediaHeaderBox : ISOMFullBox
		{
		}

		public sealed partial class OCRMediaHeaderBox : ISOMFullBox
		{
		}

		public sealed partial class SceneMediaHeaderBox : ISOMFullBox
		{
		}

		/// <summary>
		/// Data Reference Atom
		/// </summary>
		public sealed partial class DataReferenceBox : ISOMFullBox
		{
			/// <summary>
			/// Data references
			/// </summary>
			[BinCustom]
			BoxCollection boxArray = new BoxCollection();
		}

		/// <summary>
		/// Data Information Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DataInformationBox : AtomicInfo
		{
			/// <summary>
			/// Data reference atom
			/// </summary>
			[XmlIgnore]
			public DataReferenceBox DataReference { get { return boxList.Get<DataReferenceBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				DataReferenceBox>(AllowUnknownBox);
		}

		public abstract partial class ISOMDataEntryFields : ISOMFullBox
		{
			[XmlElement, DefaultValue("")]
			//the flag set indicates we have a string (WE HAVE TO for URLs)
			[BinData] //(Condition = "(Flags & AtomFlags.Text) == 0")
			public string Location;
		}

		public sealed partial class DataEntryBox : ISOMDataEntryFields
		{
		}

		public sealed partial class DataEntryURLBox : ISOMDataEntryFields
		{
		}

		public sealed partial class DataEntryURNBox : ISOMFullBox
		{
			[XmlElement, DefaultValue("")]
			//the flag set indicates we have a string (WE HAVE TO for URLs)
			[BinData] //(Condition = "(Flags & AtomFlags.Text) == 0")
			public string NameURN;
			[XmlElement, DefaultValue("")]
			//the flag set indicates we have a string (WE HAVE TO for URLs)
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //(Condition = "(Flags & AtomFlags.Text) == 0")
			public string Location;
		}

		public sealed partial class SttsEntry
		{
			[XmlAttribute]
			[BinData]
			public int SampleCount;
			[XmlAttribute]
			[BinData]
			public int SampleDelta;
		}

		public sealed partial class TimeToSampleBox : ISOMFullBox
		{
			[BinArray(CountFormat = BinFormat.Int32)]
			List<SttsEntry> entries = new List<SttsEntry>();
		}

		/*TO CHECK - it could be reasonnable to only use 16bits for both count and offset*/
		public sealed partial class DttsEntry
		{
			[XmlAttribute]
			[BinData]
			public int SampleCount;
			[XmlAttribute]
			[BinData]
			public int DecodingOffset;
		}

		public sealed partial class CompositionOffsetBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinArray(CountFormat = BinFormat.Int32)]
			List<DttsEntry> entries = new List<DttsEntry>();

			/*force one sample per entry*/
			[XmlIgnore]
			public bool UnpackMode;
		}

		public sealed partial class StsfEntry
		{
			[XmlAttribute]
			[BinData]
			public int SampleNumber;
			[XmlElement("FragmentSize")]
			[BinArray(CountFormat = BinFormat.Int32)]
			[BinData]
			public ushort[] FragmentSizes;
		}

		public sealed partial class SampleFragmentBox : ISOMFullBox
		{
			[BinArray(CountFormat = BinFormat.Int32)]
			List<StsfEntry> entries = new List<StsfEntry>();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public abstract partial class ISOMSampleEntryFields : ISOMUUIDBox
		{
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "6")]
			public byte[] Reserved; //char[ 6 ]
			[XmlAttribute]
			[BinData]
			public ushort DataReferenceIndex;
			[XmlIgnore]
			public abstract ProtectionInfoBox ProtectionInfo { get; set; }
		}

		/*base sample entry box (never used but for typecasting)*/
		public sealed partial class SampleEntryBox : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
		}

		public sealed partial class GenericSampleEntryBox : ISOMSampleEntryFields
		{
			/*box type as specified in the file (not this box's type!!)*/
			[XmlAttribute]
			public AtomicCode EntryType;

			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		public sealed partial class ESDBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] data;
#line 698 "isomedia_dev.h"
#warning ESDBox should be implemented with ODF
			//public ESD Desc;
#line default
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MPEG4BitRateBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int BufferSizeDB;
			[XmlAttribute]
			[BinData]
			public int MaxBitrate;
			[XmlAttribute]
			[BinData]
			public int AvgBitrate;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MPEG4ExtensionDescriptorsBox : AtomicInfo
		{
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		/*for most MPEG4 media */
		public sealed partial class MPEGSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ESDBox ESD { get { return boxList.Get<ESDBox>(); } set { boxList.Set(value); } }
			/*used for hinting when extracting the OD stream...*/
			//public SLConfig Slc;
#warning Looking forward to SLConfig implementation from "sync_layer.h"
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				ESDBox,
				ProtectionInfoBox>(AllowUnknownBox);
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class LASeRConfigurationBox : AtomicInfo
		{
			[XmlAttribute, DefaultValue("")]
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")]
			public string Header;
		}

		public sealed partial class LASeRSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public LASeRConfigurationBox LsrConfig { get { return boxList.Get<LASeRConfigurationBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public MPEG4BitRateBox Bitrate { get { return boxList.Get<MPEG4BitRateBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public MPEG4ExtensionDescriptorsBox Descr { get { return boxList.Get<MPEG4ExtensionDescriptorsBox>(); } set { boxList.Set(value); } }

			/*used for hinting when extracting the OD stream...*/
			//public SLConfig Slc;
#warning Looking forward to SLConfig implementation from "sync_layer.h"

			TypedBoxList boxList = TypedBoxList.Create<
				LASeRConfigurationBox,
				MPEG4ExtensionDescriptorsBox,
				MPEG4BitRateBox>(AllowUnknownBox);
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PixelAspectRatioBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int HSpacing;
			[XmlAttribute]
			[BinData]
			public int VSpacing;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RVCConfigurationBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public ushort PredefinedRVCConfig;
			[XmlAttribute]
			[BinData(BinFormat.UInt16, Condition = "PredefinedRVCConfig == 0")]
			public int RVCMetaIdx;
		}

		public abstract partial class ISOMVisualSampleEntry : ISOMSampleEntryFields
		{
			[XmlAttribute]
			[BinData]
			public ushort Version;
			[XmlAttribute]
			[BinData]
			public ushort Revision;
			[XmlAttribute]
			[BinData]
			public int Vendor;
			[XmlAttribute]
			[BinData]
			public int TemporalQuality;
			[XmlAttribute]
			[BinData]
			public int SpacialQuality;
			[XmlAttribute]
			[BinData]
			public ushort Width;
			[XmlAttribute]
			[BinData]
			public ushort Height;
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public Fixed<uint, x16> HorizRes = (Fixed<uint, x16>)0x48;
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public Fixed<uint, x16> VertRes = (Fixed<uint, x16>)0x48;
			[XmlAttribute]
			[BinData]
			public int EntryDataSize;
			[XmlAttribute]
			[BinData]
			public ushort FramesPerSample = 1;
			[XmlIgnore]
			[BinData(BinFormat.Binary, LengthCustomMethod = "32")]
			public UTF8PadString CompressorName;//char compressor_name[33];
			[XmlAttribute]
			[BinData]
			public ushort BitDepth = 0x18;
			[XmlAttribute]
			[BinData]
			public short ColorTableIndex = 1;
			[XmlIgnore]
			public abstract PixelAspectRatioBox PAsp { get; set; }
			[XmlIgnore]
			public abstract RVCConfigurationBox RVCC { get; set; }
		}

		public sealed partial class VisualSampleEntryBox : ISOMVisualSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
			[XmlIgnore]
			public override PixelAspectRatioBox PAsp { get { return null; } set { } }
			[XmlIgnore]
			public override RVCConfigurationBox RVCC { get { return null; } set { } }
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class AVCConfigurationBox : AtomicInfo
		{
			//public AVCConfig Config;
#warning Looking forward to AVCConfig
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] unknownData;
		}

		public sealed partial class MPEGVisualSampleEntryBox : ISOMVisualSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public override PixelAspectRatioBox PAsp { get { return boxList.Get<PixelAspectRatioBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public override RVCConfigurationBox RVCC { get { return boxList.Get<RVCConfigurationBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ESDBox ESD { get { return boxList.Get<ESDBox>(); } set { boxList.Set(value); } }
			/*used for Publishing*/
			//public SLConfig Slc;
#warning Looking forward to SLConfig

			/*avc extensions - we merged with regular 'mp4v' box to handle isma E&A signaling of AVC*/
			[XmlIgnore]
			public AVCConfigurationBox AVCConfig
			{
				get { return boxList.OfType<AVCConfigurationBox>().FirstOrDefault(cfg => cfg.AtomicID == "avcC"); }
				set { if (value.AtomicID == "avcC") boxList.Set(value); }
			}
			[XmlIgnore]
			public AVCConfigurationBox SVCConfig
			{
				get { return boxList.OfType<AVCConfigurationBox>().FirstOrDefault(cfg => cfg.AtomicID == "svcC"); }
				set { if (value.AtomicID == "svcC") boxList.Set(value); }
			}
			[XmlIgnore]
			public MPEG4BitRateBox Bitrate { get { return boxList.Get<MPEG4BitRateBox>(); } set { boxList.Set(value); } }
			/*ext descriptors*/
			[XmlIgnore]
			public MPEG4ExtensionDescriptorsBox Descr { get { return boxList.Get<MPEG4ExtensionDescriptorsBox>(); } set { boxList.Set(value); } }
			/*internally emulated esd*/
			//[XmlIgnore]
			//public ESD EmulEsd;
#warning Looking forward to ESD
			/// <summary>
			/// iPod's hack
			/// </summary>
			[XmlIgnore]
			public UnknownUUIDBox iPodExt { get { return boxList.Get<UnknownUUIDBox>(); } set { boxList.Set(value); } }
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				PixelAspectRatioBox,
				ESDBox,
				AVCConfigurationBox,
				UnknownUUIDBox,
				MPEG4BitRateBox,
				MPEG4ExtensionDescriptorsBox,
				//SVCConfigurationBox,
				ProtectionInfoBox,
				RVCConfigurationBox>(AllowUnknownBox);
		}

		/*this is the default visual sdst (to handle unknown media)*/
		public sealed partial class GenericVisualSampleEntryBox : ISOMVisualSampleEntry
		{
			/*box type as specified in the file (not this box's type!!)*/
			[XmlAttribute]
			public AtomicCode EntryType;
			/*opaque description data (ESDS in MP4, SMI in SVQ3, ...)*/
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		public abstract partial class ISOMAudioSampleEntry : ISOMSampleEntryFields
		{
			[XmlAttribute]
			[BinData]
			public ushort Version;
			[XmlAttribute]
			[BinData]
			public ushort Revision;
			[XmlAttribute]
			[BinData]
			public int Vendor;
			[XmlAttribute]
			[BinData]
			public ushort ChannelCount = 2;
			[XmlAttribute]
			[BinData]
			public ushort BitsPerSample = 16;
			[XmlAttribute]
			[BinData]
			public ushort CompressionID;
			[XmlAttribute]
			[BinData]
			public ushort PacketSize;
			[XmlAttribute]
			[BinData]
			public ushort SamplerateHi;
			[XmlAttribute]
			[BinData]
			public ushort SamplerateLo;
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "16", Condition = "Version == 1")]
			[BinData(LengthCustomMethod = "36", Condition = "Version == 2")]
			public byte[] Reserved2;
		}

		public sealed partial class AudioSampleEntryBox : ISOMAudioSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
		}

		public sealed partial class MPEGAudioSampleEntryBox : ISOMAudioSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ESDBox ESD { get { return boxList.Get<ESDBox>(); } set { boxList.Set(value); } }
			//public SLConfig Slc;
#warning Looking forward to SLConfig
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				ESDBox,
				ProtectionInfoBox>(AllowUnknownBox);
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class GF3GPPConfigBox : AtomicInfo
		{
			//public GF3GPConfig Cfg;
#warning Looking forward to GF3GPConfig
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] unknownData;
		}

		public sealed partial class GF3GPPAudioSampleEntryBox : ISOMAudioSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public GF3GPPConfigBox Info { get { return boxList.Get<GF3GPPConfigBox>(); } set { boxList.Set(value); } }

			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		public sealed partial class GF3GPPVisualSampleEntryBox : ISOMVisualSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }
			[XmlIgnore]
			public override PixelAspectRatioBox PAsp { get { return null; } set { } }
			[XmlIgnore]
			public override RVCConfigurationBox RVCC { get { return null; } set { } }

			[XmlIgnore]
			public GF3GPPConfigBox Info { get { return boxList.Get<GF3GPPConfigBox>(); } set { boxList.Set(value); } }

			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		/*this is the default visual sdst (to handle unknown media)*/
		public sealed partial class GenericAudioSampleEntryBox : ISOMAudioSampleEntry
		{
			/*box type as specified in the file (not this box's type!!)*/
			[XmlAttribute]
			public AtomicCode EntryType;
			/*opaque description data (ESDS in MP4, ...)*/
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class AC3ConfigBox : AtomicInfo
		{
			[BinData(BinFormat.UInt24)]
			int data;

			public AC3Config Config
			{
				get
				{
					return new AC3Config
					{
						fscod = (byte)(data & 0x3),
						bsid = (byte)((data >> 2) & 0x1F),
						bsmod = (byte)((data >> 7) & 0x7),
						acmod = (byte)((data >> 10) & 0x7),
						lfon = (byte)((data >> 13) & 0x1),
						brcode = (byte)((data >> 14) & 0x1F)
					};
				}
				set
				{
					data = (byte)(
						value.fscod & 0x3 |
						(value.bsid  & 0x1F) << 2 |
						(value.bsmod & 0x7) << 7 |
						(value.acmod & 0x7) << 10 |
						(value.lfon & 0x1) << 13 |
						(value.brcode & 0x1F << 14));
				}
			}
		}

		public sealed partial class AC3SampleEntryBox : ISOMAudioSampleEntry
		{
			[XmlIgnore]
			public AC3ConfigBox Info { get { return boxList.Get<AC3ConfigBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<AC3ConfigBox>();
		}

		public sealed partial class DIMSSceneConfigBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public byte Profile;
			[XmlAttribute]
			[BinData]
			public byte Level;
			[XmlAttribute]
			[BinData]
			public byte PathComponents;
			[XmlAttribute]
			[BinData]
			public bool FullRequestHost;
			[XmlAttribute]
			[BinData]
			public bool StreamType;
			[XmlAttribute]
			[BinData]
			public byte ContainsRedundant;
			[XmlAttribute]
			[BinData]
			public string TextEncoding;
			[XmlAttribute]
			[BinData]
			public string ContentEncoding;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DIMSScriptTypesBox : AtomicInfo
		{
			[BinData]
			public string ContentScriptTypes;
		}

		public sealed partial class DIMSSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public DIMSSceneConfigBox Config { get { return boxList.Get<DIMSSceneConfigBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public MPEG4BitRateBox Bitrate { get { return boxList.Get<MPEG4BitRateBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public DIMSScriptTypesBox Scripts { get { return boxList.Get<DIMSScriptTypesBox>(); } set { boxList.Set(value); } }

			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		public sealed partial class MetaDataSampleEntryBox : ISOMSampleEntryFields
		{
			internal const string DefaultID = "metx";

			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }

			[XmlAttribute, DefaultValue("")]
			[BinData]
			public string ContentEncoding; //optional
			[XmlAttribute, DefaultValue("")]
			[BinData]
			public string MIMETypeOrNamespace; //not optional
			[XmlAttribute, DefaultValue("")]
			[BinData(Condition = "AtomicID == DefaultID")]
			public string XMLSchemaLoc; // optional
			[XmlIgnore]
			public MPEG4BitRateBox Bitrate { get { return boxList.Get<MPEG4BitRateBox>(); } set { boxList.Set(value); } }// optional

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				ProtectionInfoBox,
				MPEG4BitRateBox>(AllowUnknownBox);
		}

		public sealed partial class SampleDescriptionBox : ISOMFullBox
		{
			[BinCustom]
			BoxCollection boxArray = new BoxCollection();
		}

		public sealed partial class SampleSizeBox : ISOMFullBox
		{
			internal const string DefaultID = "stsz";

			//24-reserved
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.Int24, Condition = "AtomicID != DefaultID")]
			public int Reserverd;
			/*if this is the compact version, sample size is actually fieldSize*/
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.UInt8, Condition = "AtomicID != DefaultID")]
			[BinData(BinFormat.Int32)]
			public int SampleSize;
			[XmlAttribute, DefaultValue(32)]
			public int SizeLength = 32;
			[XmlElement("Size")]
			[BinCustom]
			public int[] Sizes;
		}

		public sealed partial class ChunkOffsetBox : ISOMFullBox
		{
			[XmlElement("Offset")]
			[BinArray(CountFormat = BinFormat.Int32), BinData]
			public int[] Offsets;
		}

		public sealed partial class ChunkLargeOffsetBox : ISOMFullBox
		{
			[XmlElement("Offset")]
			[BinArray(CountFormat = BinFormat.Int32)]
			[BinData]
			public long[] Offsets;
		}

		public sealed partial class StscEntry
		{
			[XmlAttribute]
			[BinData]
			public int FirstChunk;
			[XmlAttribute]
			[BinData]
			public int SamplesPerChunk;
			[XmlAttribute]
			[BinData]
			public int SampleDescriptionIndex;
		}

		public sealed partial class SampleToChunkBox : ISOMFullBox
		{
			[BinArray(CountFormat = BinFormat.Int32)]
			List<StscEntry> entries = new List<StscEntry>();
		}

		public sealed partial class SyncSampleBox : ISOMFullBox
		{
			[XmlElement("SampleNumber")]
			[BinArray(CountFormat = BinFormat.Int32)]
			[BinData]
			public int[] SampleNumbers;
		}

		public sealed partial class StshEntry
		{
			[XmlAttribute]
			[BinData]
			public int ShadowedSampleNumber;
			[XmlAttribute]
			[BinData]
			public int SyncSampleNumber;//s32
		}

		public sealed partial class ShadowSyncBox : ISOMFullBox
		{
			[BinArray(CountFormat = BinFormat.Int32)]
			List<StshEntry> entries = new List<StshEntry>();
		}

		public sealed partial class DegradationPriorityBox : ISOMFullBox
		{
			//we need to read the DegPriority in a different way...
			//this is called through stbl_read...
			[XmlElement("Priority")]
			[BinArray(CountCustomMethod = "reader.Length()/2")]
			[BinData]
			public short[] Priorities;
		}

		public sealed partial class PaddingBitsBox : ISOMFullBox
		{
			[XmlElement]
			public sbyte[] PadBits;
		}

		public sealed partial class SampleDependencyTypeBox : ISOMFullBox
		{
			/*each dep type is packed on 1 byte*/
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] SampleInfo; /*out-of-order sdtp, assume no padding at the end*/
		}

		public sealed partial class SampleEntry
		{
			[XmlAttribute]
			[BinData]
			public int SampleDelta;
			[BinArray(CountFormat = BinFormat.UInt16)]
			Collection<SubSampleEntry> subSamples;
		}

		[BinBlock(MethodMode = BinMethodMode.Final)]
		public sealed partial class SubSampleEntry : IEntry<SubSampleInformationBox>
		{
			public SubSampleInformationBox Owner { get; set; }
			[XmlAttribute]
			[BinData(BinFormat.Int32, Condition = "Owner.Version == 1")]
			[BinData(BinFormat.UInt16)]
			public int SubSampleSize;
			[XmlAttribute]
			[BinData]
			public byte SubSamplePriority;
			[XmlAttribute]
			[BinData]
			public bool Discardable;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved;
		}

		public sealed partial class SubSampleInformationBox : ISOMFullBox
		{
			[BinArray(CountFormat = BinFormat.Int32)]
			Collection<SampleEntry> samples;
		}

		/// <summary>
		/// Sample Table Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SampleTableBox : AtomicInfo
		{
			/// <summary>
			/// Sample description atom
			/// </summary>
			[XmlIgnore]
			public SampleDescriptionBox SampleDescription { get { return boxList.Get<SampleDescriptionBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Time-to-sample atom
			/// </summary>
			[XmlIgnore]
			public TimeToSampleBox TimeToSample { get { return boxList.Get<TimeToSampleBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Composition offset atom
			/// </summary>
			[XmlIgnore]
			public CompositionOffsetBox CompositionOffset { get { return boxList.Get<CompositionOffsetBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Composition Shift Least Greatest atom
			/// </summary>
#warning Composition Shift Least Greatest atom not found

			/// <summary>
			/// Sync sample atom
			/// </summary>
			[XmlIgnore]
			public SyncSampleBox SyncSample { get { return boxList.Get<SyncSampleBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Partial sync sample atom 
			/// </summary>
#warning Partial sync sample atom not found

			/// <summary>
			/// Sample-to-chunk atom
			/// </summary>
			[XmlIgnore]
			public SampleToChunkBox SampleToChunk { get { return boxList.Get<SampleToChunkBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Sample size atom
			/// </summary>
			[XmlIgnore]
			public SampleSizeBox SampleSize { get { return boxList.Get<SampleSizeBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Chunk offset atom
			/// </summary>
			/// <remarks>
			/// Untyped, to handle 32 bits and 64 bits chunkOffsets
			/// </remarks>
			[XmlIgnore]
			public AtomicInfo ChunkOffset
			{
				get
				{
					return
						boxList[typeof(ChunkOffsetBox), false] ??
						boxList[typeof(ChunkLargeOffsetBox), false];
				}
				set
				{
					if (value is ChunkOffsetBox) boxList.Set((ChunkOffsetBox)value);
					if (value is ChunkLargeOffsetBox) boxList.Set((ChunkLargeOffsetBox)value);
				}
			}
			/// <summary>
			/// Shadow sync atom
			/// </summary>
			[XmlIgnore]
			public ShadowSyncBox ShadowSync { get { return boxList.Get<ShadowSyncBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Sample Group Description atom
			/// </summary>
			[XmlIgnore]
			public SampleGroupDescriptionBox[] SampleGroupsDescription { get { return boxList.ArrayOfType<SampleGroupDescriptionBox>(); } }
			/// <summary>
			/// Sample-to-group atom
			/// </summary>
			[XmlIgnore]
			public SampleGroupBox[] SampleGroups { get { return boxList.ArrayOfType<SampleGroupBox>(); } }
			/// <summary>
			/// Sample Dependency Flags atom
			/// </summary>
			[XmlIgnore]
			public SampleDependencyTypeBox SampleDep { get { return boxList.Get<SampleDependencyTypeBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public DegradationPriorityBox DegradationPriority { get { return boxList.Get<DegradationPriorityBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public PaddingBitsBox PaddingBits { get { return boxList.Get<PaddingBitsBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public SampleFragmentBox Fragments { get { return boxList.Get<SampleFragmentBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public SubSampleInformationBox SubSamples { get { return boxList.Get<SubSampleInformationBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				SampleDescriptionBox,
				TimeToSampleBox,
				CompositionOffsetBox,
				SyncSampleBox,
				ShadowSyncBox,
				SampleToChunkBox,
				SampleSizeBox,
				TypedBoxList.Or<ChunkOffsetBox, ChunkLargeOffsetBox>,
				DegradationPriorityBox,
				SampleDependencyTypeBox,
				PaddingBitsBox,
				SubSampleInformationBox,
				SampleGroupDescriptionBox[],
				SampleGroupBox[],
				SampleFragmentBox>(AllowUnknownBox);
		}

		//__tag_media_info_box
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MediaInformationBox : AtomicInfo
		{
			[XmlIgnore]
			public AtomicInfo InfoHeader
			{
				get
				{
					return
						boxList[typeof(MPEGMediaHeaderBox), false] ??
						boxList[typeof(VideoMediaHeaderBox), false] ??
						boxList[typeof(SoundMediaHeaderBox), false] ??
						boxList[typeof(HintMediaHeaderBox), false];
					//return boxList.Get<gmhd>(false);
				}
				set
				{
					if (value is MPEGMediaHeaderBox) boxList.Set((MPEGMediaHeaderBox)value);
					if (value is VideoMediaHeaderBox) boxList.Set((VideoMediaHeaderBox)value);
					if (value is SoundMediaHeaderBox) boxList.Set((SoundMediaHeaderBox)value);
					if (value is HintMediaHeaderBox) boxList.Set((HintMediaHeaderBox)value);
					//if (value is gmhd) boxList.Set<gmhd>((gmhd)value);
				}
			}
			[XmlIgnore]
			public DataInformationBox DataInformation { get { return boxList.Get<DataInformationBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public SampleTableBox SampleTable { get { return boxList.Get<SampleTableBox>(); } set { boxList.Set(value); } }
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				TypedBoxList.Or<MPEGMediaHeaderBox, VideoMediaHeaderBox, SoundMediaHeaderBox, HintMediaHeaderBox/*, gmhd*/>,
				DataInformationBox,
				SampleTableBox>(AllowUnknownBox);
		}


		/// <summary>
		/// Unused space available in file.
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class FreeSpaceBox: AtomicInfo
		{
			[XmlIgnore]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data = EmptyData;
		}

		/// <summary>
		/// ISO Copyright statement
		/// </summary>
		public sealed partial class CopyrightBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinData(BinFormat.UInt16)]
			public PackedLanguage Language;
			[XmlElement, DefaultValue("")]
			[BinData]
			public string Notice;
		}

		public sealed partial class ChapterEntry
		{
			[XmlAttribute]
			[BinData]
			public long StartTime;
			[XmlElement, DefaultValue("")]
			[BinData(BinFormat.PString, LengthFormat = BinFormat.UInt8, LengthCustomMethod = "encoding.GetByteCount(Name)")]
			public string Name;
		}

		//this is using chpl format according to some NeroRecode samples
		public sealed partial class ChapterListBox : ISOMFullBox
		{
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved; //reserved or ???
			[BinArray]
			List<ChapterEntry> list = new List<ChapterEntry>();
		}

		/// <summary>
		/// Track reference type atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackReferenceTypeBox : AtomicInfo
		{
			/// <summary>
			/// A list of track ID values (32-bit integers) specifying the related tracks. Note that this is one case where track ID values can be
			/// set to 0. Unused entries in the atom may have a track ID value of 0. Setting the track ID to 0 may be more convenient than deleting
			/// the reference.
			/// </summary>
			[XmlElement("TrackID")]
			[BinArray(CountCustomMethod = "reader.Length() / 4")]
			[BinData]
			public int[] TrackIDs;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class JPEG2000Atom: AtomicInfo
		{
			internal const string DefaultID = "jP  ";
			internal const uint Signature = 0x0D0A870Au;

			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data = Signature.GetBytes(false);
		}

		/// <summary>
		/// The File Type Compatibility Atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class FileTypeBox: AtomicInfo
		{
			internal const string DefaultID = "ftyp";

			/// <summary>
			/// A 32-bit unsigned integer that identifies compatible file format.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode Brand;
			/// <summary>
			/// A 32-bit field that indicates the file format specification version.
			/// </summary>
			[XmlIgnore]
			[BinData]
			public int Version;
			/// <summary>
			/// A series of unsigned 32-bit integers listing compatible file formats. The major brand must appear in the list of compatible brands.
			/// One or more “placeholder” entries with value zero are permitted; such entries should be ignored.
			/// </summary>
			[XmlIgnore]
			[BinCustom]
			public AtomicCode[] CompatibleBrand = EmptyBrands;
		}

		public sealed partial class ProgressiveDownloadBox : ISOMFullBox
		{
			[XmlElement("Rate")]
			public int[] Rates;
			[XmlElement("Time")]
			public int[] Times;
		}

		/*
			3GPP streaming text boxes
		*/

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class FontTableBox : AtomicInfo
		{
			public int EntryCount;
			//public FontRecord Fonts;
#warning Looking forward to FontRecord
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] unknownData;
		}

		public sealed partial class Tx3gSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			[XmlAttribute]
			[BinData]
			public int DisplayFlags;
			[XmlAttribute]
			[BinData]
			public sbyte HorizontalJustification;
			[XmlAttribute]
			[BinData]
			public sbyte VerticalJustification;
			/*ARGB*/
			[XmlAttribute]
			[BinData]
			public int BackColor;
			//public BoxRecord DefaultBox;
#warning Looking forward to BoxRecord
			//public StyleRecord	DefaultStyle;
#warning Looking forward to StyleRecord
			[XmlIgnore]
			public FontTableBox FontTable { get { return boxList.Get<FontTableBox>(); } set { boxList.Set(value); } }

			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		/*Apple specific*/
		public sealed partial class TextSampleEntryBox : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			[XmlAttribute]
			[BinData]
			public int DisplayFlags;
			[XmlAttribute]
			[BinData]
			public int TextJustification;
			[XmlAttribute]
			[BinData]
			public string BackgroundColor;//char background_color[6]
			[XmlAttribute]
			[BinData]
			public string ForegroundColor;//char foreground_color[6];
			//[XmlAttribute]
			//[BinData]
			//public BoxRecord DefaultBox;
#warning Looking forward to BoxRecord
			[XmlAttribute]
			[BinData]
			public ushort FontNumber;
			[XmlAttribute]
			[BinData]
			public ushort FontFace;
			[XmlElement(DataType = "hexBinary")]
			[BinData]
			public byte[] Reserved1;//char reserved1[8];
			[XmlAttribute, DefaultValue((byte)0)]
			[BinData]
			public byte Reserved2;
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved3;
			[XmlAttribute]
			[BinData]
			public string TextName; /*font name*/
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextStyleBox : AtomicInfo
		{
			public int EntryCount;
			//public StyleRecord Styles;
#warning Looking forward to StyleRecord
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] unknownData;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextHighlightBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public ushort Startcharoffset;
			[XmlAttribute]
			[BinData]
			public ushort Endcharoffset;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextHighlightColorBox : AtomicInfo
		{
			/*ARGB*/
			[BinData]
			public uint HilColor;
		}

		public sealed partial class KaraokeRecord
		{
			[XmlAttribute]
			[BinData]
			public int HighlightEndTime;
			[XmlAttribute]
			[BinData]
			public ushort StartCharOffset;
			[XmlAttribute]
			[BinData]
			public ushort EndCharOffset;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextKaraokeBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int HighlightStartTime;
			[XmlAttribute]
			[BinData]
			public ushort NumberEntries;
			[XmlAttribute]
			public KaraokeRecord Records;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextScrollDelayBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int ScrollDelay;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextHyperTextBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public ushort Startcharoffset;
			[XmlAttribute]
			[BinData]
			public ushort Endcharoffset;
			[XmlAttribute]
			[BinData]
			public string URL;
			[XmlAttribute]
			[BinData]
			public string URLHint;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextBoxBox : AtomicInfo
		{
			//public BoxRecord Box;
#warning Looking forward to BoxRecord
			[BinData(LengthCustomMethod = "reader.Length()")]
			byte[] unknownData;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextBlinkBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public ushort Startcharoffset;
			[XmlAttribute]
			[BinData]
			public ushort Endcharoffset;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextWrapBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public byte WrapFlag;
		}

		public sealed partial class TrackSelectionBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public int SwitchGroup;
			[XmlElement]
			[BinArray]
			[BinData]
			public int[] AttributeList;
			[XmlAttribute]
			[BinData]
			public int AttributeListCount;
		}

		/*
			MPEG-21 extensions
		*/
		public sealed partial class XMLBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public int XMLLength;
			[XmlAttribute]
			[BinData]
			public string XML;
		}

		public sealed partial class BinaryXMLBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public int DataLength;
			[XmlAttribute]
			[BinData]
			public string Data;
		}

		public sealed partial class ItemExtentEntry
		{
			[XmlAttribute]
			[BinData]
			public long ExtentOffset;
			[XmlAttribute]
			[BinData]
			public long ExtentLength;
			/*for storage only*/
			public long OriginalExtentOffset;
		}

		public sealed partial class ItemLocationEntry
		{
			[XmlAttribute]
			[BinData]
			public ushort ItemID;
			[XmlAttribute]
			[BinData]
			public ushort DataReferenceIndex;
			[XmlAttribute]
			public long BaseOffset;
			/*for storage only*/
			public long OriginalBaseOffset;
			public List<AtomicInfo> ExtentEntries = new List<AtomicInfo>();
		}

		public sealed partial class ItemLocationBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public byte OffsetSize;
			[XmlAttribute]
			[BinData]
			public byte LengthSize;
			[XmlAttribute]
			[BinData]
			public byte BaseOffsetSize;
			public List<AtomicInfo> LocationEntries = new List<AtomicInfo>();
		}

		public sealed partial class PrimaryItemBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public ushort ItemID;
		}

		public sealed partial class ItemProtectionBox : ISOMFullBox
		{
			[XmlIgnore]
			public List<AtomicInfo> ProtectionInformation = new List<AtomicInfo>();
		}

		public sealed partial class ItemInfoEntryBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public ushort ItemID;
			[XmlAttribute]
			[BinData]
			public ushort ItemProtectionIndex;
			/*zero-terminated strings*/
			[XmlAttribute]
			[BinData]
			public string ItemName;
			[XmlAttribute]
			[BinData]
			public string ContentType;
			[XmlAttribute]
			[BinData]
			public string ContentEncoding;
			// needed to actually read the resource file, but not written in the MP21 file.
			[XmlAttribute]
			[BinData]
			public string FullPath;
			// if not 0, full_path is actually the data to write.
			[XmlAttribute]
			[BinData]
			public int DataLen;
		}

		public sealed partial class ItemInfoBox : ISOMFullBox
		{
			public List<AtomicInfo> ItemInfos = new List<AtomicInfo>();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class OriginalFormatBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int DataFormat;
		}

		public sealed partial class SchemeTypeBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public int SchemeType;
			[XmlAttribute]
			[BinData]
			public int SchemeVersion;
			[XmlAttribute]
			[BinData]
			public string URI;
		}

		/*ISMACryp specific*/
		public sealed partial class ISMAKMSBox : ISOMFullBox
		{
			/*zero-terminated string*/
			[XmlAttribute]
			[BinData]
			public string URI;
		}

		/*ISMACryp specific*/
		//__isma_format_box
		public partial class ISMASampleFormatBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public byte SelectiveEncryption;
			[XmlAttribute]
			[BinData]
			public byte KeyIndicatorLength;
			[XmlAttribute]
			[BinData]
			public byte IVLength;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SchemeInformationBox : AtomicInfo
		{
			[XmlIgnore]
			public ISMAKMSBox Ikms { get { return boxList.Get<ISMAKMSBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ISMASampleFormatBox Isfm { get { return boxList.Get<ISMASampleFormatBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public OMADRMKMSBox Okms { get { return boxList.Get<OMADRMKMSBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		//__tag_protect_box
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class ProtectionInfoBox : AtomicInfo
		{
			[XmlIgnore]
			public OriginalFormatBox OriginalFormat { get { return boxList.Get<OriginalFormatBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public SchemeTypeBox SchemeType { get { return boxList.Get<SchemeTypeBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public SchemeInformationBox Info { get { return boxList.Get<SchemeInformationBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		public sealed partial class IPMPInfoBox : ISOMFullBox
		{
			public List<AtomicInfo> Descriptors = new List<AtomicInfo>();
		}

		public sealed partial class IPMPControlBox : ISOMFullBox
		{
			//public IPMPToolList IpmpTools;
#warning Looking forward to IPMPToolList
			public List<AtomicInfo> Descriptors = new List<AtomicInfo>();
		}

		/// <summary>
		/// Metadata Atom
		/// </summary>
		public sealed partial class MetaBox : ISOMFullBox
		{
			[XmlIgnore]
			public HandlerBox Handler { get { return boxList.Get<HandlerBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public PrimaryItemBox PrimaryResource { get { return boxList.Get<PrimaryItemBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public DataInformationBox FileLocations { get { return boxList.Get<DataInformationBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ItemLocationBox ItemLocations { get { return boxList.Get<ItemLocationBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ItemProtectionBox Protections { get { return boxList.Get<ItemProtectionBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public ItemInfoBox ItemInfos { get { return boxList.Get<ItemInfoBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public IPMPControlBox IPMPControl { get { return boxList.Get<IPMPControlBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

/*V2 boxes - Movie Fragments*/

		[BinBlock(GetDataSizeMethod = "ResolveVersion(FragmentDuration)")]
		public sealed partial class MovieExtendsHeaderBox : ISOMFullBox
		{
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long FragmentDuration;
		}

		//__tag_mvex_box
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MovieExtendsBox : AtomicInfo
		{
			[XmlIgnore]
			public TrackExtendsBox[] TrackExList { get { return boxList.ArrayOfType<TrackExtendsBox>(); } }
			[XmlIgnore]
			public MovieExtendsHeaderBox Header { get { return boxList.Get<MovieExtendsHeaderBox>(); } set { boxList.Set(value); } }
			//public ISOFile Mov;
#warning Looking forward to ISOFile
			[BinCustom]
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox); //No order???
		}

/*the TrackExtends contains default values for the track fragments*/
		public sealed partial class TrackExtendsBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public int TrackID;
			[XmlAttribute]
			[BinData]
			public int DefSampleDescIndex;
			[XmlAttribute]
			[BinData]
			public int DefSampleDuration;
			[XmlAttribute]
			[BinData]
			public int DefSampleSize;
			[XmlAttribute]
			[BinData]
			public int DefSampleFlags;
		}

/*indicates the seq num of this fragment*/
		public sealed partial class MovieFragmentHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public int SequenceNumber;
		}

/*MovieFragment is a container IN THE FILE, contains 1 fragment*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MovieFragmentBox : AtomicInfo
		{
			[XmlIgnore]
			public MovieFragmentHeaderBox Header { get { return boxList.Get<MovieFragmentHeaderBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public TrackFragmentBox[] TrackList { get { return boxList.ArrayOfType<TrackFragmentBox>(); } }
			//public ISOFile Mov;
#warning Looking forward to ISOFile
			/*offset in the file of moof or mdat (whichever comes first) for this fragment*/

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				MovieFragmentHeaderBox,
				TrackFragmentBox[]>(AllowUnknownBox);
		}

		public sealed partial class TrackFragmentHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			public TrackFragmentFlags TrackFragmentFlags { get { return (TrackFragmentFlags)Flags & TrackFragmentFlags.ValidMask; } set { Flags = (int)value; } }

			[XmlAttribute]
			[BinData]
			public int TrackID;
			/* all the following are optional fields */
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.BaseOffset) != 0")]
			public long BaseDataOffset;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleDesc) != 0")]
			public int SampleDescIndex;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleDur) != 0")]
			public int DefSampleDuration;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleSize) != 0")]
			public int DefSampleSize;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleFlags) != 0")]
			public int DefSampleFlags;
			//[XmlAttribute]
			//[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.DurEmpty) != 0")]
			//public int EmptyDuration;
		}

		[BinBlock(GetDataSizeMethod = "ResolveVersion(BaseMediaDecodeTime)")]
		public sealed partial class TFBaseMediaDecodeTimeBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long BaseMediaDecodeTime;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackFragmentBox : AtomicInfo
		{
			[XmlIgnore]
			public TrackFragmentHeaderBox Header { get { return boxList.Get<TrackFragmentHeaderBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public TrackFragmentRunBox[] TrackRuns { get { return boxList.ArrayOfType<TrackFragmentRunBox>(); } }
			[XmlIgnore]
			public SampleDependencyTypeBox SDTP { get { return boxList.Get<SampleDependencyTypeBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public SubSampleInformationBox Subs { get { return boxList.Get<SubSampleInformationBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public SampleGroupBox[] SampleGroups { get { return boxList.ArrayOfType<SampleGroupBox>(); } }
			[XmlIgnore]
			public SampleGroupDescriptionBox[] SampleGroupsDescription { get { return boxList.ArrayOfType<SampleGroupDescriptionBox>(); } }

			[XmlIgnore]
			public TFBaseMediaDecodeTimeBox TFDT { get { return boxList.Get<TFBaseMediaDecodeTimeBox>(); } set { boxList.Set(value); } }

			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				TrackFragmentHeaderBox,
				SubSampleInformationBox,
				TFBaseMediaDecodeTimeBox,
				SampleDependencyTypeBox,
				SampleGroupDescriptionBox[],
				SampleGroupBox[],
				TrackFragmentRunBox[]>(AllowUnknownBox);
		}

		public sealed partial class TrackFragmentRunBox : ISOMFullBox
		{
			[XmlAttribute]
			public TrackRunFlags TrackRunFlags { get { return (TrackRunFlags)Flags & TrackRunFlags.ValidMask; } set { Flags = (int)value; } }

			[XmlIgnore]
			[BinCustom(ReadMethod = "int sampleCount = reader.ReadInt32()",
				GetDataSizeMethod = "4", WriteMethod = "writer.Write((int)entries.Count)")]
			public int SampleCount { get { return entries.Count; } }
			/*the following are optional fields */
			[XmlAttribute]
			[BinData(Condition = "(TrackRunFlags & TrackRunFlags.DataOffset) != 0")]
			public int DataOffset;//s32
			[XmlAttribute]
			[BinData(Condition = "(TrackRunFlags & TrackRunFlags.FirstFlag) != 0")]
			public int FirstSampleFlags;
			/*can be empty*/
			[BinArray(CountCustomMethod = "sampleCount")]
			Collection<TrunEntry> entries;

			/*in write mode with data caching*/
			//public BitStream Cache;
#warning Looking forward to BitStream
		}

		[BinBlock(MethodMode = BinMethodMode.Final)]
		public sealed partial class TrunEntry : IEntry<TrackFragmentRunBox>
		{
			public TrackFragmentRunBox Owner { get; set; }
			[XmlAttribute]
			[BinData(Condition = "(Owner.TrackRunFlags & TrackRunFlags.Duration) != 0")]
			public int Duration;
			[XmlAttribute]
			[BinData(Condition = "(Owner.TrackRunFlags & TrackRunFlags.Size) != 0")]
			public int Size;
			[XmlAttribute]
			[BinData(Condition = "(Owner.TrackRunFlags & TrackRunFlags.Flags) != 0")]
			public int Flags;
			[XmlAttribute]
			[BinData(Condition = "(Owner.TrackRunFlags & TrackRunFlags.CTSOffset) != 0")]
			public int CTSOffset;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SegmentTypeBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int MajorBrand;
			[XmlAttribute]
			[BinData]
			public int MinorVersion;
			[XmlAttribute]
			[BinData]
			public int AltCount;
			[XmlElement]
			[BinArray]
			[BinData]
			public int[] AltBrand;
		}

/*RTP Hint Track Sample Entry*/
		public sealed partial class HintSampleEntryBox : ISOMSampleEntryFields
		{
			//this type is used internally for protocols that share the same base entry
			//currently only RTP uses this, but a flexMux could use this entry too...
			[XmlAttribute]
			[BinData]
			public ushort HintTrackVersion = 1;
			[XmlAttribute]
			[BinData]
			public ushort LastCompatibleVersion = 1;
			[XmlAttribute]
			[BinData]
			public int MaxPacketSize;
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RTPBox : AtomicInfo
		{
			internal const string DefaultID = "rtp ";
			[XmlAttribute]
			[BinData(BinFormat.UInt32)]
			public AtomicCode SubType;
			[XmlAttribute, DefaultValue("")]
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //don't write the NULL char
			public string SDPText;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SDPBox : AtomicInfo
		{
			internal const string DefaultID = "sdp ";
			[XmlAttribute, DefaultValue("")]
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //sdp text has no delimiter !!!
			public string SDPText;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RTPOBox : AtomicInfo
		{
			//here we have no pb, just remembed that some entries will have to
			//be 4-bytes aligned ...
			[XmlAttribute]
			[BinData]
			public int TimeOffset;//s32
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class HintTrackInfoBox : AtomicInfo
		{
			[XmlIgnore]
			AtomicInfo SDP
			{
				get
				{
					return
						boxList[typeof(SDPBox), false] ??
						boxList[typeof(RTPBox), false];
				}
				set
				{
					if (value is SDPBox) boxList.Set((SDPBox)value);
					if (value is RTPBox) boxList.Set((RTPBox)value);
				}
			}

			/*contains GF_SDPBox if in track, GF_RTPBox if in movie*/
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				TypedBoxList.Or<RTPBox, SDPBox>>(AllowUnknownBox);
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RelyHintBox : AtomicInfo
		{
			[BinData]
			byte data;
			[XmlAttribute, DefaultValue(0)]
			public int Reserved { get { return data >> 2; } set { data = (byte)(value << 2); } }
			[XmlAttribute]
			public bool Prefered { get { return (data & 0x2u) != 0; } set { data = (byte)(value ? data | 0x2u : data & ~0x2u); } }
			[XmlAttribute]
			public bool Required { get { return (data & 0x1u) != 0; } set { data = (byte)(value ? data | 0x1u : data & ~0x1u); } }
		}

/***********************************************************
			data entry tables for RTP
***********************************************************/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TSHintEntryBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int TimeScale;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TimeOffHintEntryBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int TimeOffset;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SeqOffHintEntryBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int SeqOffset;
		}

/***********************************************************
			hint track information boxes for RTP
***********************************************************/

/*Total number of bytes that will be sent, including 12-byte RTP headers, but not including any network headers*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TRPYBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public long NumberBytes;
		}

		/// <summary>
		/// The total number of bytes that will be sent, including 12-byte RTP headers, but not including any network headers.
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TOTLBox: AtomicInfo
		{
			internal const string DefaultID = "totl";
			[XmlAttribute]
			[BinData]
			public int NumberBytes;
		}

		/*Total number of network packets that will be sent*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NUMPBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public long NumberPackets;
		}

		/*32-bits version of nump used in Darwin*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NPCKBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int NumberPackets;
		}


		/*Total number of bytes that will be sent, not including 12-byte RTP headers*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NTYLBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public long NumberBytes;
		}

		/*32-bits version of tpyl used in Darwin*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TPAYBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int NumberBytes;
		}

		/*Maximum data rate in bits per second.*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MAXRBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int Granularity;
			[XmlAttribute]
			[BinData]
			public int MaxDataRate;
		}


		/*Total number of bytes from the media track to be sent*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DMEDBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public long NumberBytes;
		}

		/*Number of bytes of immediate data to be sent*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DIMMBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public long NumberBytes;
		}


		/*Number of bytes of repeated data to be sent*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DREPBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public long NumberBytes;
		}

		/*Smallest relative transmission time, in milliseconds. signed integer for smoothing*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TMINBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int MinTime;//s32
		}

		/*Largest relative transmission time, in milliseconds.*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TMAXBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int MaxTime;//s32
		}

		/*Largest packet, in bytes, including 12-byte RTP header*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PMAXBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int MaxSize;
		}

		/*Longest packet duration, in milliseconds*/
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DMAXBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int MaxDur;
		}

		/*32-bit payload type number, followed by rtpmap payload string */
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PAYTBox : AtomicInfo
		{
			[XmlAttribute]
			[BinData]
			public int PayloadCode;
			[XmlAttribute, DefaultValue("")]
			[BinData(BinFormat.PString, LengthFormat = BinFormat.UInt8, LengthCustomMethod = "encoding.GetByteCount(PayloadString)")]
			public string PayloadString;
		}


		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NameBox : AtomicInfo
		{
			[XmlAttribute, DefaultValue("")]
			[BinData]
			public string String;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class HintInfoBox : AtomicInfo
		{
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		/*Apple extension*/

		public sealed partial class DataBox : ISOMFullBox
		{
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved;
			[XmlAttribute]
			[BinData]
			public string Data;
			[XmlAttribute]
			[BinData]
			public int Size;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class ListItemBox : AtomicInfo
		{
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class ItemListBox : AtomicInfo
		{
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		/*OMA (P)DCF extensions*/
		public sealed partial class OMADRMCommonHeaderBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public byte EncryptionMethod;
			[XmlAttribute]
			[BinData]
			public byte PaddingScheme;
			[XmlAttribute]
			[BinData]
			public long PlaintextLength;
			[XmlAttribute]
			[BinData]
			public string ContentID;
			[XmlAttribute]
			[BinData]
			public string RightsIssuerURL;
			[XmlAttribute]
			[BinData]
			public string TextualHeaders;
			[XmlAttribute]
			[BinData]
			public int TextualHeadersLen;
			[XmlAttribute]
			public List<AtomicInfo> ExtendedHeaders = new List<AtomicInfo>();
		}

		public sealed partial class OMADRMGroupIDBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public byte GKEncryptionMethod;
			[XmlAttribute]
			[BinData]
			public string GroupID;
			[XmlAttribute]
			[BinData]
			public ushort GKLength;
			[XmlAttribute]
			[BinData]
			public string GroupKey;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class OMADRMMutableInformationBox : AtomicInfo
		{
			[BinCustom]
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		public sealed partial class OMADRMTransactionTrackingBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public string TransactionID;//char TransactionID[16];
		}

		public sealed partial class OMADRMRightsObjectBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData]
			public string OmaRo;
			[XmlAttribute]
			[BinData]
			public int OmaRoSize;
		}

		/*identical*/
		public sealed partial class OMADRMAUFormatBox : ISMASampleFormatBox
		{
		}

		//__oma_kms_box
		public sealed partial class OMADRMKMSBox : ISOMFullBox
		{
			[XmlIgnore]
			public OMADRMCommonHeaderBox Header { get { return boxList.Get<OMADRMCommonHeaderBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public OMADRMAUFormatBox Format { get { return boxList.Get<OMADRMAUFormatBox>(); } set { boxList.Set(value); } }

			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}


		public sealed partial class SIDXReference
		{
			[BinData]
			uint referenceData;
			[XmlAttribute]
			public bool ReferenceType { get { return (referenceData >> 31) != 0u; } set { referenceData |= 1u << 31; } }
			[XmlAttribute]
			public int ReferenceSize { get { return (int)(referenceData & (uint)int.MaxValue); } set { referenceData |= (uint)value & (uint)int.MaxValue; } }
			[XmlAttribute]
			[BinData]
			public int SubsegmentDuration;
			[BinData]
			uint sapData;
			[XmlAttribute]
			public bool StartsWithSAP { get { return (sapData >> 31) != 0u; } set { sapData |= 1u << 31; } }
			[XmlAttribute]
			public int SAPType { get { return (int)(sapData >> 28) & 0x7; } set { sapData |= (uint)(value & 0x7) << 28; } }
			[XmlAttribute]
			public int SAPDeltaTime { get { return (int)(sapData & 0xFFFFFFFu); } set { sapData |= (uint)value & 0xFFFFFFFu; } }
		}

		public sealed partial class SegmentIndexBox : ISOMFullBox
		{

			[XmlAttribute]
			[BinData]
			public int ReferenceID;
			[XmlAttribute]
			[BinData]
			public int Timescale;
			[XmlAttribute]
			[BinData(BinFormat.UInt32, Condition = "Version == 0")]
			[BinData(BinFormat.Int64)]
			public long EarliestPresentationTime;
			[XmlAttribute]
			[BinData(BinFormat.UInt32, Condition = "Version == 0")]
			[BinData(BinFormat.Int64)]
			public long FirstOffset;
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved;
			[XmlAttribute]
			[BinArray(CountFormat = BinFormat.UInt16)]
			public List<SIDXReference> Refs = new List<SIDXReference>();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PcrInfoBox : AtomicInfo
		{
			[XmlElement("Value")]
			public long[] PcrValues
			{
				get { return (pcrValues ?? new ulong[0]).Select(d => (long)(d >> 22)).ToArray(); }
				set { pcrValues = (value ?? new long[0]).Select(d => (ulong)d << 22).ToArray(); }
			}
			[BinArray(CountFormat = BinFormat.Int32)]
			[BinData]
			ulong[] pcrValues;
		}

		/***********************************************************
					Sample Groups
		***********************************************************/
		public sealed partial class SampleGroupEntry
		{
			[XmlAttribute]
			[BinData]
			public int SampleCount;
			[XmlAttribute]
			[BinData]
			public int GroupDescriptionIndex;
		}

		public sealed partial class SampleGroupBox : ISOMFullBox
		{
			[XmlAttribute]
			[BinData(Condition = "Version == 1")]
			public int GroupingType;
			[XmlAttribute]
			[BinData]
			public int GroupingTypeParameter;

			[BinArray(CountFormat = BinFormat.Int32)]
			List<SampleGroupEntry> sampleEntries = new List<SampleGroupEntry>();
		}

		public sealed partial class SampleGroupDescriptionBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode GroupingType;
			[XmlAttribute]
			[BinData(Condition = "Version == 1")]
			public int DefaultLength;

			[BinArray(CountFormat = BinFormat.Int32)]
			[BinArrayItem(CreateCustomMethod = "ResolveGroupDescription()")]
			Collection<SampleGroupDescriptionEntry> groupDescriptions;
		}

		public abstract partial class SampleGroupDescriptionEntry
		{
			[XmlAttribute]
			[BinData(Condition = "Owner.Version == 1 && Owner.DefaultLength == 0")]
			public int Length;
		}

		/*default entry */
		public sealed partial class DefaultSampleGroupDescriptionEntry : SampleGroupDescriptionEntry
		{
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "Length")]
			public byte[] Data;
		}

		/*VisualRandomAccessEntry - 'rap ' type*/
		public sealed partial class VisualRandomAccessEntry : SampleGroupDescriptionEntry
		{
			[BinData]
			byte data;
			[XmlAttribute]
			public bool NumLeadingSamplesKnown { get { return (data & 0x80) != 0; } set { data |= (byte)0x80; } }
			[XmlAttribute]
			public byte NumLeadingSamples { get { return (byte)(data & 0x7F); } set { data |= (byte)(value & 0x7F); } }
		}

		/*RollRecoveryEntry - 'roll' type*/
		public sealed partial class RollRecoveryEntry : SampleGroupDescriptionEntry
		{
			[XmlAttribute]
			[BinData]
			public short RollDistance;
		}

		/*this is the DataHandler structure each data handler has its own bitstream*/
		public abstract partial class ISOMBaseDataHandler
		{
			[XmlAttribute]
			[BinData]
			public byte Type;
			[XmlAttribute]
			[BinData]
			public long CurPos;
			[XmlAttribute]
			[BinData]
			public byte Mode;
			//[XmlAttribute]
			//public BitStream Bs;
#warning Looking forward to BitStream
		}

		//__tag_data_map
		public sealed partial class DataMap : ISOMBaseDataHandler
		{
		}

		public sealed partial class FileDataMap : ISOMBaseDataHandler
		{
			public Stream Stream;
			public bool LastAccesWasRead;
			public string TempFile;
		}

/*file mapping handler. used if supported, only on read mode for complete files  (not in file download)*/
		public sealed partial class FileMappingDataMap : ISOMBaseDataHandler
		{
			[XmlAttribute]
			[BinData]
			public string Name;
			[XmlAttribute]
			[BinData]
			public long FileSize;
			[XmlAttribute]
			[BinData]
			public string ByteMap;
			[XmlAttribute]
			[BinData]
			public long BytePos;
		}

		public abstract partial class ISMOBaseDTEEntry
		{
			[XmlAttribute]
			[BinData]
			public byte Source;
		}

		public sealed partial class GenericDTE : ISMOBaseDTEEntry
		{
		}

		public sealed partial class EmptyDTE : ISMOBaseDTEEntry
		{
		}

		public sealed partial class ImmediateDTE : ISMOBaseDTEEntry
		{
			[XmlAttribute]
			[BinData]
			public byte DataLength;
			[XmlElement(DataType = "hexBinary")]
			[BinData]
			public byte[] Data;//char data[14];
		}

		public sealed partial class SampleDTE : ISMOBaseDTEEntry
		{
			[XmlAttribute]
			[BinData]
			public sbyte TrackRefIndex;
			[XmlAttribute]
			[BinData]
			public int SampleNumber;
			[XmlAttribute]
			[BinData]
			public ushort DataLength;
			[XmlAttribute]
			[BinData]
			public int ByteOffset;
			[XmlAttribute]
			[BinData]
			public ushort BytesPerComp;
			[XmlAttribute]
			[BinData]
			public ushort SamplesPerComp;
		}

		public sealed partial class StreamDescDTE : ISMOBaseDTEEntry
		{
			[XmlAttribute]
			[BinData]
			public sbyte trackRefIndex;
			[XmlAttribute]
			[BinData]
			public int StreamDescIndex;
			[XmlAttribute]
			[BinData]
			public ushort DataLength;
			[XmlAttribute]
			[BinData]
			public int ByteOffset;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved;
		}

		/*****************************************************
				RTP Sample
		*****************************************************/

		/*data cache when reading*/
		//__tag_hint_data_cache
		public sealed partial class HintDataCache
		{
			//public ISOSample Sample;
#warning Looking forward to ISOSample
			[XmlIgnore]
			public TrackBox Track;
			public int SampleNum;
		}

		public sealed partial class HintSample
		{
			/*used internally for future protocol support (write only)*/
			[XmlAttribute]
			[BinData]
			public byte HintType;
			/*QT packets*/
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved;
			[XmlAttribute]
			public List<AtomicInfo> PacketTable = new List<AtomicInfo>();
			[XmlAttribute]
			[BinData]
			public string AdditionalData;
			[XmlAttribute]
			[BinData]
			public int DataLength;
			/*used internally for hinting*/
			[XmlAttribute]
			[BinData]
			public long TransmissionTime;
			/*for read only, used to store samples fetched while building packets*/
			[XmlAttribute]
			public List<AtomicInfo> SampleCache = new List<AtomicInfo>();
		}

/*****************************************************
		Hint Packets (generic packet for future protocol support)
*****************************************************/
		public abstract partial class ISOMBasePacket
		{
			[BinData]
			public int RelativeTransTime;//s32
		}


		public sealed partial class HintPacket : ISOMBasePacket
		{
		}

		/*the RTP packet*/
		public sealed partial class RTPPacket : ISOMBasePacket
		{
			/*RTP Header*/
			[XmlAttribute]
			[BinData]
			public byte PBit;
			[XmlAttribute]
			[BinData]
			public byte XBit;
			[XmlAttribute]
			[BinData]
			public byte MBit;
			/*on 7 bits */
			[XmlAttribute]
			[BinData]
			public byte PayloadType;
			[XmlAttribute]
			[BinData]
			public ushort SquenceNumber;
			/*Hinting flags*/
			[XmlAttribute]
			[BinData]
			public byte BBit;
			[XmlAttribute]
			[BinData]
			public byte Rbit;
			/*ExtraInfos TLVs - not really used */
			[XmlAttribute]
			public List<AtomicInfo> TLV = new List<AtomicInfo>();
			/*DataTable - contains the DTEs...*/
			[XmlAttribute]
			public List<AtomicInfo> DataTable = new List<AtomicInfo>();
		}
	}
}
