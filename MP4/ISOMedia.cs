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
		[XmlAttribute]
		public byte fscod;
		[XmlAttribute]
		public byte bsid;
		[XmlAttribute]
		public byte bsmod;
		[XmlAttribute]
		public byte acmod;
		[XmlAttribute]
		public bool lfon;
		[XmlAttribute("bit_rate_code")]
		public byte brcode;
		[XmlAttribute, DefaultValue((byte)0)]
		public byte Reserved;
	}

	[BinBlock(BinaryReaderType = "BinStringReader", BinaryWriterType = "BinStringWriter")]
	public sealed partial class ISOMediaBoxes
	{
		/// <summary>
		/// Full atom
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public abstract partial class ISOMFullBox: AtomicInfo
		{
			/// <summary>
			/// This atom has a version field and flags field
			/// </summary>
			[XmlAttribute, DefaultValue(true)]
			public bool Versioned = true;

			/// <summary>
			/// The version of this atom.
			/// </summary>
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.UInt8, Condition = "Versioned")]
			public int Version;

			/// <summary>
			/// Future flags.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt24, Condition = "Versioned")]
			public int Flags; //used by versioned atoms and derivatives
		}

		public abstract partial class ISOMUUIDBox: AtomicInfo
		{
			[XmlAttribute]
			public Guid UUID;
		}

		public sealed partial class Box: AtomicInfo
		{
		}

		/// <summary>
		/// <c>'VOID'</c>
		/// </summary>
		public sealed partial class VoidBox : AtomicInfo
		{
			internal const string DefaultID = "VOID";
			public VoidBox() : base(DefaultID) { }
		}

		public sealed partial class FullBox: ISOMFullBox
		{
		}

		/// <summary>
		/// <c>'uuid'</c>
		/// </summary>
		public sealed partial class UUIDBox: ISOMUUIDBox
		{
			internal const string DefaultID = "uuid";

			public byte[] Data;
		}

		/// <summary>
		/// Movie sample data <c>'mdat'</c> — usually this data can be interpreted only by using the movie resource.
		/// </summary>
		public sealed partial class MediaDataBox: AtomicInfo
		{
			internal const string DefaultID = "mdat";
			/// <summary>
			/// Initializes a new instance of the Movie sample data atom <c>'mdat'</c>.
			/// </summary>
			public MediaDataBox() : base(DefaultID) { }

			/// <summary>
			/// Movie sample data external resource offset
			/// </summary>
			[XmlIgnore, DefaultValue(0L)]
			public long Offset;
			/// <summary>
			/// Movie sample data size
			/// </summary>
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
			[BinCustom]
			BoxCollection boxList = new BoxCollection();
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class UnknownUUIDBox : ISOMUUIDBox
		{
			internal const string DefaultID = "uuid";
			public UnknownUUIDBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		/// <summary>
		/// Movie Header Atom <c>'mvhd'</c>
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion(Duration)")]
		public sealed partial class MovieHeaderBox: ISOMFullBox
		{
			internal const string DefaultID = "mvhd";
			/// <summary>
			/// Initializes a new instance of the Movie Header Atom <c>'mvhd'</c>.
			/// </summary>
			public MovieHeaderBox() : base(DefaultID) { }

			/// <summary>
			/// Specifies when the movie atom was created.
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime CreationTime;
			/// <summary>
			/// Specifies when the movie atom was changed.
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
			/// A fixed-point 16.16 number that specifies the rate at which to play this movie. A value of 1.0 indicates normal rate.
			/// </summary>
			[XmlIgnore]//, DefaultValue(1 << 16)]
			[BinData(BinFormat.UInt32)]
			public Fixed<uint, x16> PreferredRate = Fixed<uint, x16>.One;
			/// <summary>
			/// A fixed-point 8.8 number that specifies how loud to play this movie’s sound. A value of 1.0 indicates full volume.
			/// </summary>
			[XmlIgnore]//, DefaultValue(1 << 8)]
			[BinData(BinFormat.UInt16)]
			public Fixed<ushort, x8> PreferredVolume = Fixed<ushort, x8>.One;
			/// <summary>
			/// Reserved. Set to 0.
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
			/// The track ID number of the next track added to this movie. Note that 0 is not 
			/// a valid track ID value.
			/// </summary>
			[XmlAttribute]
			[BinData]
			public int NextTrackID = 1;
		}

		//TODO: ODF implementation
		/// <summary>
		/// <c>'iods'</c>
		/// </summary>
		public sealed partial class ObjectDescriptorBox : ISOMFullBox
		{
			internal const string DefaultID = "iods";
			public ObjectDescriptorBox() : base(DefaultID) { }

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
			[XmlIgnore]
			public EditListBox Owner { get; set; }
			[XmlAttribute("Duration")]
			[BinData(BinFormat.Int64, Condition = "Owner.Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long SegmentDuration;
			[XmlAttribute]
			[BinData(BinFormat.Int64, Condition = "Owner.Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long MediaTime;
			[XmlAttribute]
			[BinData(BinFormat.UInt16)]
			public int MediaRate;
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Reserved;
		}

		/// <summary>
		/// <c>'elst'</c>
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion()")]
		public sealed partial class EditListBox : ISOMFullBox
		{
			internal const string DefaultID = "elst";
			public EditListBox() : base(DefaultID) { CreateEntryCollection(out entryList, this); }

			[BinArray(CountFormat = BinFormat.Int32)]
			Collection<EdtsEntry> entryList;
		}

		/// <summary>
		/// <c>'edts'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class EditBox : AtomicInfo
		{
			internal const string DefaultID = "edts";
			public EditBox() : base(DefaultID) { }

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
			[XmlIgnore]
			public AtomicCode BoxType;
			[XmlAttribute]
			public Guid UUID;
			BoxCollection boxList = new BoxCollection();
		}

		/// <summary>
		/// User Data Atom <c>'udta'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class UserDataBox: AtomicInfo
		{
			internal const string DefaultID = "udta";
			/// <summary>
			/// Initializes a new instance of the User Data Atom <c>'udta'</c>.
			/// </summary>
			public UserDataBox() : base(DefaultID) { }

			[BinCustom]
			MapList boxList = new MapList();
		}

		/// <summary>
		/// The Movie Atom <c>'moov'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MovieBox : AtomicInfo
		{
			internal const string DefaultID = "moov";
			/// <summary>
			/// Initializes a new instance of the Movie atom <c>'moov'</c>.
			/// </summary>
			public MovieBox() : base(DefaultID) { }

			/// <summary>
			/// Profile atom
			/// </summary>
#warning Profile atom not found
			[XmlIgnore]
			public AtomicInfo Profile;
			/// <summary>
			/// Movie header atom
			/// </summary>
			[XmlIgnore]
			public MovieHeaderBox MovieHeader { get { return boxList.Get<MovieHeaderBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Movie clipping atom
			/// </summary>
#warning Movie clipping atom not found
			[XmlIgnore]
			public AtomicInfo MovieClipping;
			/// <summary>
			/// The Meta box
			/// </summary>
			[XmlIgnore]
			public MetaBox Meta { get { return boxList.Get<MetaBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Object descriptor box
			/// </summary>
			[XmlIgnore]
			public ObjectDescriptorBox ObjectDescriptor { get { return boxList.Get<ObjectDescriptorBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Movie extends box
			/// </summary>
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
			[XmlIgnore]
			public AtomicInfo ColorTable;
			/// <summary>
			/// Compressed movie atom
			/// </summary>
#warning Compressed movie atom not found
			[XmlIgnore]
			public AtomicInfo CompressedMovie;
			/// <summary>
			/// Reference movie atom
			/// </summary>
#warning Reference movie atom not found
			[XmlIgnore]
			public AtomicInfo ReferenceMovie;
			/// <summary>
			/// The movie atom contains other types of atoms, including at least one of three possible atoms — the
			/// <see cref="T:MP4.ISOMediaBoxes.MovieHeaderBox">movie header atom</see> (<c>'mvhd'</c>), the
			/// compressed movie atom (<c>'cmov'</c>), or a reference movie atom (<c>'rmra'</c>). An uncompressed movie
			/// atom can contain both a movie header atom and a reference movie atom, but it must contain at least one
			/// of the two. It can also contain several other atoms, such as a clipping atom (<c>'clip'</c>), one or more
			/// <see cref="T:MP4.ISOMediaBoxes.TrackBox">track atoms</see> (<c>'trak'</c>), a color table atom (<c>'ctab'</c>),
			/// and a <see cref="T:MP4.ISOMediaBoxes.UserDataBox">user data atom</see> (<c>'udta'</c>).
			/// </summary>
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
		/// Track Header Atom <c>'tkhd'</c>
		/// </summary>
		public sealed partial class TrackHeaderBox : ISOMFullBox
		{
			internal const string DefaultID = "tkhd";
			/// <summary>
			/// Initializes a new instance of the Track Header Atom <c>'tkhd'</c>.
			/// </summary>
			public TrackHeaderBox() : base(DefaultID) { }

			[XmlAttribute]
			public TrackFlags TrackFlags { get { return Flags.ValidFlags<TrackFlags>(); } set { Flags = (int)value; } }

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
			[XmlAttribute("AlternateGroupID"), DefaultValue((ushort)0)]
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
		/// Track Reference Atom <c>'tref'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackReferenceBox : AtomicInfo
		{
			internal const string DefaultID = "tref";
			/// <summary>
			/// Initializes a new instance of the Track Reference Atom <c>'tref'</c>.
			/// </summary>
			public TrackReferenceBox() : base(DefaultID) { }

			/// <summary>
			/// Track reference type atoms
			/// </summary>
			[BinCustom]
			BoxCollection<TrackReferenceTypeBox> boxList = new BoxCollection<TrackReferenceTypeBox>();
		}

		/// <summary>
		/// Track Atom <c>'trak'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackBox : AtomicInfo
		{
			internal const string DefaultID = "trak";
			/// <summary>
			/// Initializes a new instance of the Track Reference Atom <c>'tref'</c>.
			/// </summary>
			public TrackBox() : base(DefaultID) { }

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
		/// Media Header Atom <c>'mdhd'</c>
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion(Duration)")]
		public sealed partial class MediaHeaderBox: ISOMFullBox
		{
			internal const string DefaultID = "mdhd";
			/// <summary>
			/// Initializes a new instance of the Media Header Atom <c>'mdhd'</c>.
			/// </summary>
			public MediaHeaderBox() : base(DefaultID) { }

			/// <summary>
			/// Specifies when the media atom was created.
			/// </summary>
			[XmlAttribute]
			[BinData(BinFormat.MacDate64, Condition = "Version == 1")]
			[BinData(BinFormat.MacDate32)]
			public DateTime CreationTime;
			/// <summary>
			/// Specifies when the media atom was changed.
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
			/// The language code for this media.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt16)]
			public PackedLanguage Language;
			/// <summary>
			/// The media’s playback quality—that is, its suitability for playback in a given environment.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort Quality;
		}

		/// <summary>
		/// Handler Reference Atom <c>'hdlr'</c>
		/// </summary>
		public sealed partial class HandlerBox : ISOMFullBox
		{
			internal const string DefaultID = "hdlr";
			/// <summary>
			/// Initializes a new instance of the Handler Reference Atom <c>'hdlr'</c>.
			/// </summary>
			public HandlerBox() : base(DefaultID) { }

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
			[XmlAttribute("Name"), DefaultValue("")]
			[BinData]
			public string ComponentName;
		}

		/// <summary>
		/// Media Atom <c>'mdia'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MediaBox : AtomicInfo
		{
			internal const string DefaultID = "mdia";
			/// <summary>
			/// Initializes a new instance of the Media atom <c>'mdia'</c>.
			/// </summary>
			public MediaBox() : base(DefaultID) { }

			/// <summary>
			/// 
			/// </summary>
			[XmlIgnore]
			public MediaHeaderBox MediaHeader { get { return boxList.Get<MediaHeaderBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public HandlerBox Handler { get { return boxList.Get<HandlerBox>(); } set { boxList.Set(value); } }
			[XmlIgnore]
			public MediaInformationBox Information { get { return boxList.Get<MediaInformationBox>(); } set { boxList.Set(value); } }
			/// <summary>
			/// Other atoms
			/// </summary>
			[BinCustom]
			TypedBoxList boxList = TypedBoxList.Create<
				MediaHeaderBox,
				HandlerBox,
				MediaInformationBox>(AllowUnknownBox);
		}

		/// <summary>
		/// Video Media Information Header Atom <c>'vmhd'</c>
		/// </summary>
		public sealed partial class VideoMediaHeaderBox: ISOMFullBox
		{
			internal const string DefaultID = "vmhd";
			/// <summary>
			/// Initializes a new instance of the Video Media Information Header Atom <c>'vmhd'</c>.
			/// </summary>
			public VideoMediaHeaderBox() : base(DefaultID) { }

			/// <summary>
			/// The transfer mode. The transfer mode specifies which Boolean operation QuickDraw should perform when
			/// drawing or transferring an image from one location to another.
			/// </summary>
			[XmlAttribute, DefaultValue(GraphicsMode.SourceCopy)]
			[BinData(BinFormat.UInt16)]
			public GraphicsMode GraphicsMode;
			/// <summary>
			/// The red color for the transfer mode operation indicated in the graphics mode field.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort OpcolorRed;
			/// <summary>
			/// The green color for the transfer mode operation indicated in the graphics mode field.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort OpcolorGreen;
			/// <summary>
			/// The blue color for the transfer mode operation indicated in the graphics mode field.
			/// </summary>
			[XmlAttribute, DefaultValue((ushort)0)]
			[BinData]
			public ushort OpcolorBlue;
		}

		/// <summary>
		/// Sound Media Information Header Atom <c>'smhd'</c>
		/// </summary>
		public sealed partial class SoundMediaHeaderBox: ISOMFullBox
		{
			internal const string DefaultID = "smhd";
			/// <summary>
			/// Initializes a new instance of the Sound Media Information Header Atom <c>'smhd'</c>.
			/// </summary>
			public SoundMediaHeaderBox() : base(DefaultID) { }

			/// <summary>
			/// A fixed-point 8.8 number that specifies the sound balance of this sound media. Sound balance is the setting that controls the mix of sound
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

		/// <summary>
		/// <c>'hmhd'</c>
		/// </summary>
		public sealed partial class HintMediaHeaderBox: ISOMFullBox
		{
			internal const string DefaultID = "hmhd";
			public HintMediaHeaderBox() : base(DefaultID) { }

			[XmlAttribute("MaximumPDUSize"), DefaultValue(0)]
			[BinData(BinFormat.UInt16)]
			public int MaxPDUSize;
			[XmlAttribute("AveragePDUSize"), DefaultValue(0)]
			[BinData(BinFormat.UInt16)]
			public int AvgPDUSize;
			[XmlAttribute("MaxBitRate"), DefaultValue(0)]
			[BinData]
			public int MaxBitrate;
			[XmlAttribute("AverageBitRate"), DefaultValue(0)]
			[BinData]
			public int AvgBitrate;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int SlidingAverageBitrate;
		}

		/// <summary>
		/// <c>'nmhd'</c>
		/// </summary>
		public sealed partial class MPEGMediaHeaderBox: ISOMFullBox
		{
			internal const string DefaultID = "nmhd";
			public MPEGMediaHeaderBox() : base(DefaultID) { }
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
		/// Data Reference Atom <c>'dref'</c>
		/// </summary>
		public sealed partial class DataReferenceBox : ISOMFullBox
		{
			internal const string DefaultID = "dref";
			/// <summary>
			/// Initializes a new instance of the Data Reference Atom <c>'dref'</c>.
			/// </summary>
			public DataReferenceBox() : base(DefaultID) { }

			/// <summary>
			/// Data references
			/// </summary>
			[BinCustom]
			BoxCollection boxArray = new BoxCollection();
		}

		/// <summary>
		/// Data Information Atom <c>'dinf'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DataInformationBox : AtomicInfo
		{
			internal const string DefaultID = "dinf";
			/// <summary>
			/// Initializes a new instance of the Data Information Atom <c>'dinf'</c>.
			/// </summary>
			public DataInformationBox() : base(DefaultID) { }

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
			[XmlAttribute("URL"), DefaultValue("")]
			public abstract string Location { get; set; }
		}

		public sealed partial class DataEntryBox : ISOMDataEntryFields
		{
			[XmlAttribute("URL"), DefaultValue("")]
			public override string Location { get; set; }
		}

		/// <summary>
		/// <c>'url '</c>
		/// </summary>
		public sealed partial class DataEntryURLBox : ISOMDataEntryFields
		{
			internal const string DefaultID = "url ";
			public DataEntryURLBox() : base(DefaultID) { }

			[XmlAttribute("URL"), DefaultValue("")]
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //(Condition = "(Flags & AtomFlags.Text) == 0")
			public override string Location { get; set; }
		}

		/// <summary>
		/// <c>'urn '</c>
		/// </summary>
		public sealed partial class DataEntryURNBox : ISOMDataEntryFields
		{
			internal const string DefaultID = "urn ";
			public DataEntryURNBox() : base(DefaultID) { }

			[XmlAttribute("URN"), DefaultValue("")]
			//the flag set indicates we have a string (WE HAVE TO for URLs)
			[BinData] //(Condition = "(Flags & AtomFlags.Text) == 0")
			public string NameURN;
			[XmlAttribute("URL"), DefaultValue("")]
			//the flag set indicates we have a string (WE HAVE TO for URLs)
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //(Condition = "(Flags & AtomFlags.Text) == 0")
			public override string Location { get; set; }
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

		/// <summary>
		/// <c>'stts'</c>
		/// </summary>
		public sealed partial class TimeToSampleBox : ISOMFullBox
		{
			internal const string DefaultID = "stts";
			public TimeToSampleBox() : base(DefaultID) { }

			[BinArray(CountFormat = BinFormat.Int32)]
			List<SttsEntry> entries = new List<SttsEntry>();
		}

		/*TO CHECK - it could be reasonnable to only use 16bits for both count and offset*/
		public sealed partial class DttsEntry
		{
			[XmlAttribute]
			[BinData]
			public int SampleCount;
			[XmlAttribute("CompositionOffset")]
			[BinData]
			public int DecodingOffset;
		}

		/// <summary>
		/// <c>'ctts'</c>
		/// </summary>
		public sealed partial class CompositionOffsetBox : ISOMFullBox
		{
			internal const string DefaultID = "ctts";
			public CompositionOffsetBox() : base(DefaultID) { }

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
			[XmlIgnore]
			[BinArray(CountFormat = BinFormat.Int32)]
			[BinData]
			public ushort[] FragmentSizes;
		}

		/// <summary>
		/// <c>'STSF'</c>
		/// </summary>
		public sealed partial class SampleFragmentBox : ISOMFullBox
		{
			internal const string DefaultID = "STSF";
			public SampleFragmentBox() : base(DefaultID) { }

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
			internal const string DefaultID = "gnrm";
			public GenericSampleEntryBox() : base(DefaultID) { }

			/*box type as specified in the file (not this box's type!!)*/
			[XmlIgnore]
			public AtomicCode EntryType;

			[XmlElement("ExtensionData", DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		/// <summary>
		/// <c>'esds'</c>
		/// </summary>
		public sealed partial class ESDBox : ISOMFullBox
		{
			internal const string DefaultID = "esds";
			public ESDBox() : base(DefaultID) { }

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
			public int MaxBitRate;
			[XmlAttribute]
			[BinData]
			public int AvgBitRate;
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

		/// <summary>
		/// <c>'lsrC'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class LASeRConfigurationBox : AtomicInfo
		{
			internal const string DefaultID = "lsrC";
			public LASeRConfigurationBox() : base(DefaultID) { }

			[XmlElement("LASeRHeader", DataType = "hexBinary"), DefaultValue("")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Header;
		}

		/// <summary>
		/// <c>'lsr1'</c>
		/// </summary>
		public sealed partial class LASeRSampleEntryBox : ISOMSampleEntryFields
		{
			internal const string DefaultID = "lsr1";
			public LASeRSampleEntryBox() : base(DefaultID) { }

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

		/// <summary>
		/// <c>'pasp'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PixelAspectRatioBox : AtomicInfo
		{
			internal const string DefaultID = "pasp";
			public PixelAspectRatioBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int HSpacing;
			[XmlAttribute]
			[BinData]
			public int VSpacing;
		}

		/// <summary>
		/// <c>'rvcc'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RVCConfigurationBox : AtomicInfo
		{
			internal const string DefaultID = "rvcc";
			public RVCConfigurationBox() : base(DefaultID) { }

			[XmlAttribute("Predefined")]
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
			internal const string DefaultID = "gnrv";
			public GenericVisualSampleEntryBox() : base(DefaultID) { }

			/*box type as specified in the file (not this box's type!!)*/
			[XmlIgnore]
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
			[XmlAttribute("Channels")]
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
			[XmlAttribute("SampleRate")]
			[BinData]
			public ushort SampleRateHi;
			[XmlAttribute]
			[BinData]
			public ushort SampleRateLo;
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

		public abstract partial class GF3GPPAudioSampleEntryBox : ISOMAudioSampleEntry
		{
			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }

			[XmlIgnore]
			public GF3GPPConfigBox Info { get { return boxList.Get<GF3GPPConfigBox>(); } set { boxList.Set(value); } }

			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		public sealed partial class AMRSampleDescriptionBox : GF3GPPAudioSampleEntryBox
		{
		}

		public sealed partial class AMRWBSampleDescriptionBox : GF3GPPAudioSampleEntryBox
		{
		}

		public sealed partial class EVRCSampleDescriptionBox : GF3GPPAudioSampleEntryBox
		{
		}

		public sealed partial class QCELPSampleDescriptionBox : GF3GPPAudioSampleEntryBox
		{
		}

		public sealed partial class SMVSampleDescriptionBox : GF3GPPAudioSampleEntryBox
		{
		}

		public sealed partial class GF3GPAudioSampleDescriptionBox : GF3GPPAudioSampleEntryBox
		{
		}

		public abstract partial class GF3GPPVisualSampleEntryBox : ISOMVisualSampleEntry
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

		public sealed partial class H263SampleDescriptionBox : GF3GPPVisualSampleEntryBox
		{
		}

		public sealed partial class GF3GPVisualSampleDescriptionBox : GF3GPPVisualSampleEntryBox
		{
		}

		/*this is the default visual sdst (to handle unknown media)*/
		public sealed partial class GenericAudioSampleEntryBox : ISOMAudioSampleEntry
		{
			internal const string DefaultID = "gnra";
			public GenericAudioSampleEntryBox() : base(DefaultID) { }

			/*box type as specified in the file (not this box's type!!)*/
			[XmlIgnore]
			public AtomicCode EntryType;
			/*opaque description data (ESDS in MP4, ...)*/
			[XmlElement(DataType = "hexBinary")]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data;
		}

		/// <summary>
		/// <c>'dac3'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class AC3ConfigBox : AtomicInfo
		{
			internal const string DefaultID = "dac3";
			public AC3ConfigBox() : base(DefaultID) { }

			[BinData(BinFormat.UInt24)]
			int data;

			public AC3Config Config
			{
				get
				{
					return new AC3Config
					{
						Reserved = (byte)data.Bits(5, 19),
						brcode = (byte)data.Bits(5, 14),
						lfon = data.Bit(13),
						acmod = (byte)data.Bits(3, 10),
						bsmod = (byte)data.Bits(3, 7),
						bsid = (byte)data.Bits(5, 2),
						fscod = (byte)data.Bits(2, 0)
					};
				}
				set
				{
					data = (byte)data
						.Bits(5, 19, value.Reserved)
						.Bits(5, 14, value.brcode)
						.Bit(13, value.lfon)
						.Bits(3, 10, value.acmod)
						.Bits(3, 7, value.bsmod)
						.Bits(5, 2, value.bsid)
						.Bits(2, 0, value.fscod);
				}
			}
		}

		/// <summary>
		/// <c>'ac-3'</c>
		/// </summary>
		public sealed partial class AC3SampleEntryBox : ISOMAudioSampleEntry
		{
			internal const string DefaultID = "ac-3";
			public AC3SampleEntryBox() : base(DefaultID) { }

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
			[XmlAttribute("UseFullRequestHosts")]
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
			[XmlAttribute("Types")]
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

		/// <summary>
		/// <c>'metx'</c>
		/// </summary>
		public sealed partial class MetaDataSampleEntryBox : ISOMSampleEntryFields
		{
			internal const string DefaultID = "metx";

			[XmlIgnore]
			public override ProtectionInfoBox ProtectionInfo { get { return boxList.Get<ProtectionInfoBox>(); } set { boxList.Set(value); } }

			[XmlAttribute, DefaultValue("")]
			[BinData]
			public string ContentEncoding; //optional
			[XmlAttribute, DefaultValue("")] //<namespace> or <mime_type>
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

		/// <summary>
		/// <c>'stsd'</c>
		/// </summary>
		public sealed partial class SampleDescriptionBox : ISOMFullBox
		{
			internal const string DefaultID = "stsd";
			public SampleDescriptionBox() : base(DefaultID) { }

			/// <summary>
			/// An array of sample descriptions.
			/// </summary>
			[BinCustom]
			BoxCollection boxArray = new BoxCollection();
		}

		public abstract partial class SampleSizeBox : ISOMFullBox
		{
			[XmlAttribute, DefaultValue(0)]
			public virtual int ConstantSampleSize { get { return 0; } set { } }
			/*if this is the compact version*/
			[XmlAttribute, DefaultValue(32)]
			public virtual byte SampleSizeBits { get { return 32; } set { } }

			protected abstract int[] Sizes { get; set; }
		}

		/// <summary>
		/// <c>'stsz'</c>
		/// </summary>
		public sealed partial class FixedSampleSizeBox : SampleSizeBox
		{
			internal const string DefaultID = "stsz";
			public FixedSampleSizeBox() : base(DefaultID) { }

			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public override int ConstantSampleSize { get; set; }
			[XmlIgnore]
			[BinCustom(ReadMethod = "int sampleCount = reader.ReadInt32(); if (ConstantSampleSize != 0) sampleCount = 0",
				GetDataSizeMethod = "4", WriteMethod = "writer.Write((int)Sizes.Length)")]
			public int SampleCount { get { return Sizes.Length; } }
			[BinArray(CountCustomMethod = "sampleCount")]
			[BinData]
			protected override int[] Sizes { get; set; }
		}

		/// <summary>
		/// <c>'stz2'</c>
		/// </summary>
		public sealed partial class CompactSampleSizeBox : SampleSizeBox
		{
			internal const string DefaultID = "stz2";
			public CompactSampleSizeBox() : base(DefaultID) { }

			//24-reserved
			[XmlAttribute, DefaultValue(0)]
			[BinData(BinFormat.Int24)]
			public int Reserverd;
			/*if this is the compact version*/
			[XmlAttribute, DefaultValue(32)]
			[BinData]
			public override byte SampleSizeBits { get; set; }

			[BinCustom]
			protected override int[] Sizes { get; set; }
		}

		/// <summary>
		/// <c>'stco'</c>
		/// </summary>
		public sealed partial class ChunkOffsetBox : ISOMFullBox
		{
			internal const string DefaultID = "stco";
			public ChunkOffsetBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinArray(CountFormat = BinFormat.Int32), BinData]
			public int[] Offsets;
		}

		/// <summary>
		/// <c>'co64'</c>
		/// </summary>
		public sealed partial class ChunkLargeOffsetBox : ISOMFullBox
		{
			internal const string DefaultID = "co64";
			public ChunkLargeOffsetBox() : base(DefaultID) { }

			[XmlIgnore]
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

		/// <summary>
		/// <c>'stsc'</c>
		/// </summary>
		public sealed partial class SampleToChunkBox : ISOMFullBox
		{
			internal const string DefaultID = "stsc";
			public SampleToChunkBox() : base(DefaultID) { }

			[BinArray(CountFormat = BinFormat.Int32)]
			List<StscEntry> entries = new List<StscEntry>();
		}

		/// <summary>
		/// <c>'stss'</c>
		/// </summary>
		public sealed partial class SyncSampleBox : ISOMFullBox
		{
			internal const string DefaultID = "stss";
			public SyncSampleBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinArray(CountFormat = BinFormat.Int32)]
			[BinData]
			public int[] SampleNumbers;
		}

		public sealed partial class StshEntry
		{
			[XmlAttribute("ShadowedSample")]
			[BinData]
			public int ShadowedSampleNumber;
			[XmlAttribute("SyncSample")]
			[BinData]
			public int SyncSampleNumber;//s32
		}

		/// <summary>
		/// <c>'stsh'</c>
		/// </summary>
		public sealed partial class ShadowSyncBox : ISOMFullBox
		{
			internal const string DefaultID = "stsh";
			public ShadowSyncBox() : base(DefaultID) { }

			[BinArray(CountFormat = BinFormat.Int32)]
			List<StshEntry> entries = new List<StshEntry>();
		}

		/// <summary>
		/// <c>'stdp'</c>
		/// </summary>
		public sealed partial class DegradationPriorityBox : ISOMFullBox
		{
			internal const string DefaultID = "stdp";
			public DegradationPriorityBox() : base(DefaultID) { }

			//we need to read the DegPriority in a different way...
			//this is called through stbl_read...
			[XmlIgnore]
			[BinArray(CountCustomMethod = "reader.Length()/2")]
			[BinData]
			public short[] Priorities;
		}

		/// <summary>
		/// <c>'padb'</c>
		/// </summary>
		public sealed partial class PaddingBitsBox : ISOMFullBox
		{
			internal const string DefaultID = "padb";
			public PaddingBitsBox() : base(DefaultID) { }

			[XmlIgnore]
			public sbyte[] PadBits;
		}

		/// <summary>
		/// <c>'sdtp'</c>
		/// </summary>
		public sealed partial class SampleDependencyTypeBox : ISOMFullBox
		{
			internal const string DefaultID = "sdtp";
			public SampleDependencyTypeBox() : base(DefaultID) { }

			/*each dep type is packed on 1 byte*/
			[XmlIgnore]
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
			[XmlIgnore]
			public SubSampleInformationBox Owner { get; set; }
			[XmlAttribute("Size")]
			[BinData(BinFormat.Int32, Condition = "Owner.Version == 1")]
			[BinData(BinFormat.UInt16)]
			public int SubSampleSize;
			[XmlAttribute("Priority")]
			[BinData]
			public byte SubSamplePriority;
			[XmlAttribute]
			[BinData]
			public bool Discardable;
			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved;
		}

		/// <summary>
		/// <c>'subs'</c>
		/// </summary>
		public sealed partial class SubSampleInformationBox : ISOMFullBox
		{
			internal const string DefaultID = "subs";
			public SubSampleInformationBox() : base(DefaultID) { CreateEntryCollection(out samples, this); }

			[BinArray(CountFormat = BinFormat.Int32)]
			Collection<SampleEntry> samples;
		}

		/// <summary>
		/// Sample Table Atom <c>'stbl'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SampleTableBox : AtomicInfo
		{
			internal const string DefaultID = "stbl";
			/// <summary>
			/// Initializes a new instance of the Sample Table Atom <c>'stbl'</c>.
			/// </summary>
			public SampleTableBox() : base(DefaultID) { }

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
			public SampleSizeBox SampleSize { get { return (SampleSizeBox)boxList[typeof(SampleSizeBox), false]; } set { boxList.Set(value); } }
			/// <summary>
			/// Chunk offset atom
			/// </summary>
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

		/// <summary>
		/// <c>'minf'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MediaInformationBox : AtomicInfo
		{
			internal const string DefaultID = "minf";
			public MediaInformationBox() : base(DefaultID) { }

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
		/// Unused space available in file <c>'free'</c>.
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class FreeSpaceBox: AtomicInfo
		{
			internal const string DefaultID = "free";
			/// <summary>
			/// Initializes a new instance of the Unused space atom <c>'free'</c>.
			/// </summary>
			public FreeSpaceBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinData(LengthCustomMethod = "reader.Length()")]
			public byte[] Data = EmptyData;
		}

		/// <summary>
		/// ISO Copyright statement <c>'cprt'</c>
		/// </summary>
		public sealed partial class CopyrightBox : ISOMFullBox
		{
			internal const string DefaultID = "cprt";
			/// <summary>
			/// Initializes a new instance of the ISO Copyright statement atom <c>'cprt'</c>.
			/// </summary>
			public CopyrightBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinData(BinFormat.UInt16)]
			public PackedLanguage Language;
			[XmlAttribute("CopyrightNotice"), DefaultValue("")]
			[BinData]
			public string Notice;
		}

		public sealed partial class ChapterEntry
		{
			[XmlIgnore]
			[BinData]
			public long StartTime;
			[XmlAttribute, DefaultValue("")]
			[BinData(BinFormat.PString, LengthFormat = BinFormat.UInt8, LengthCustomMethod = "encoding.GetByteCount(Name)")]
			public string Name;
		}

		//this is using chpl format according to some NeroRecode samples
		/// <summary>
		/// <c>'chpl'</c>
		/// </summary>
		public sealed partial class ChapterListBox : ISOMFullBox
		{
			internal const string DefaultID = "chpl";
			public ChapterListBox() : base(DefaultID, version: 1) { }

			[XmlAttribute, DefaultValue(0)]
			[BinData]
			public int Reserved; //reserved or ???
			[BinArray]
			List<ChapterEntry> list = new List<ChapterEntry>();
		}

		/// <summary>
		/// Track reference type atom <c>'REFT'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public partial class TrackReferenceTypeBox : AtomicInfo
		{
			internal const string DefaultID = "REFT";
			/// <summary>
			/// Initializes a new instance of the Track reference type atom <c>'REFT'</c>.
			/// </summary>
			public TrackReferenceTypeBox() : base(DefaultID) { }

			/// <summary>
			/// A list of track ID values specifying the related tracks. Note that this is one case where track ID values can be
			/// set to 0. Unused entries in the atom may have a track ID value of 0. Setting the track ID to 0 may be more convenient than deleting
			/// the reference.
			/// </summary>
			[XmlIgnore]
			[BinArray(CountCustomMethod = "reader.Length() / 4")]
			[BinData]
			public int[] TrackIDs;
		}

		public sealed class HintTrackReferenceBox : TrackReferenceTypeBox { }

		public sealed class StreamTrackReferenceBox : TrackReferenceTypeBox { }

		public sealed class ODTrackReferenceBox : TrackReferenceTypeBox { }

		public sealed class SyncTrackReferenceBox : TrackReferenceTypeBox { }

		public sealed class ChapterTrackReferenceBox : TrackReferenceTypeBox { }

		/// <summary>
		/// <c>'jP  '</c>
		/// </summary>
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
		/// The File Type Compatibility Atom <c>'ftyp'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class FileTypeBox: AtomicInfo
		{
			internal const string DefaultID = "ftyp";
			/// <summary>
			/// Initializes a new instance of the File Type Compatibility atom <c>'ftyp'</c>.
			/// </summary>
			public FileTypeBox() : base(DefaultID) { }

			/// <summary>
			/// Identifies compatible file format.
			/// </summary>
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode Brand;
			/// <summary>
			/// The file format specification version; minor version of the major brand in general.
			/// </summary>
			[XmlIgnore]
			[BinData]
			public int Version;
			/// <summary>
			/// Listing compatible file formats. The major brand must appear in
			/// the list of compatible brands. One or more “placeholder” entries with value zero are permitted; such
			/// entries should be ignored.
			/// </summary>
			[XmlIgnore]
			[BinCustom]
			public AtomicCode[] CompatibleBrand = EmptyBrands;
		}

		/// <summary>
		/// <c>'pdin'</c>
		/// </summary>
		public sealed partial class ProgressiveDownloadBox : ISOMFullBox
		{
			internal const string DefaultID = "pdin";
			public ProgressiveDownloadBox() : base(DefaultID) { AtomFlags = AtomFlags.Text; /* 1 */ }

			public struct DownloadInfo
			{
				[XmlAttribute]
				public int Rate;
				[XmlAttribute]
				public int EstimatedTime;
			}

			DownloadInfo[] infos;

			[XmlElement("Rate")]
			public int[] Rates { get { return infos.Select(info => info.Rate).ToArray(); } }
			[XmlElement("Time")]
			public int[] Times { get { return infos.Select(info => info.EstimatedTime).ToArray(); } }
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
			[XmlAttribute("BackgroundColor")]
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
			public ushort StartCharOffset;
			[XmlAttribute]
			[BinData]
			public ushort EndCharOffset;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TextHighlightColorBox : AtomicInfo
		{
			/*ARGB*/
			[BinData]
			public uint HighlightColor;
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
			public ushort StartCharOffset;
			[XmlAttribute]
			[BinData]
			public ushort EndCharOffset;
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
			public ushort StartCharOffset;
			[XmlAttribute]
			[BinData]
			public ushort EndCharOffset;
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
			[XmlIgnore]
			[BinArray]
			[BinData(BinFormat.UInt32)]
			public AtomicCode[] AttributeList;
		}

		/*
			MPEG-21 extensions
		*/
		public sealed partial class XMLBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinData]
			public string XML;

			public XmlCDataSection XMLAsCData
			{
				get
				{
					XmlDocument doc = new XmlDocument();
					return doc.CreateCDataSection(XML);
				}
				set
				{
					XML = value.Value;
				}
			}
		}

		public sealed partial class BinaryXMLBox : ISOMFullBox
		{
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
			[XmlElement("ItemExtentEntry")]
			public List<ItemExtentEntry> ExtentEntries = new List<ItemExtentEntry>();
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
			[XmlElement("ItemLocationEntry")]
			public List<ItemLocationEntry> LocationEntries = new List<ItemLocationEntry>();
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
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode DataFormat;
		}

		public sealed partial class SchemeTypeBox : ISOMFullBox
		{
			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode SchemeType;
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
			[XmlAttribute("KMSURI")]
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
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

/*V2 boxes - Movie Fragments*/

		/// <summary>
		/// <c>'mehd'</c>
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion(FragmentDuration)")]
		public sealed partial class MovieExtendsHeaderBox : ISOMFullBox
		{
			internal const string DefaultID = "mehd";
			public MovieExtendsHeaderBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long FragmentDuration;
		}

		/// <summary>
		/// <c>'mvex'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MovieExtendsBox : AtomicInfo
		{
			internal const string DefaultID = "mvex";
			public MovieExtendsBox() : base(DefaultID) { }

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
		/// <summary>
		/// <c>'trex'</c>
		/// </summary>
		public sealed partial class TrackExtendsBox : ISOMFullBox
		{
			internal const string DefaultID = "trex";
			public TrackExtendsBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int TrackID;
			[XmlAttribute]
			[BinData]
			public int SampleDescriptionIndex;
			[XmlAttribute]
			[BinData]
			public int SampleDuration;
			[XmlAttribute]
			[BinData]
			public int SampleSize;
			[XmlIgnore]
			[BinData]
			public int DefaultSampleFlags;
		}

		/*indicates the seq num of this fragment*/
		/// <summary>
		/// <c>'mfhd'</c>
		/// </summary>
		public sealed partial class MovieFragmentHeaderBox : ISOMFullBox
		{
			internal const string DefaultID = "mfhd";
			public MovieFragmentHeaderBox() : base(DefaultID) { }

			[XmlAttribute("FragmentSequenceNumber")]
			[BinData]
			public int SequenceNumber;
		}

		/*MovieFragment is a container IN THE FILE, contains 1 fragment*/
		/// <summary>
		/// <c>'moof'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MovieFragmentBox : AtomicInfo
		{
			internal const string DefaultID = "moof";
			public MovieFragmentBox() : base(DefaultID) { }

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

		/// <summary>
		/// <c>'tfhd'</c>
		/// </summary>
		public sealed partial class TrackFragmentHeaderBox : ISOMFullBox
		{
			internal const string DefaultID = "tfhd";
			public TrackFragmentHeaderBox() : base(DefaultID) { }

			[XmlAttribute]
			public TrackFragmentFlags TrackFragmentFlags { get { return Flags.ValidFlags<TrackFragmentFlags>(); } set { Flags = (int)value; } }

			[XmlAttribute]
			[BinData]
			public int TrackID;
			/* all the following are optional fields */
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.BaseOffset) != 0")]
			public long BaseDataOffset;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleDesc) != 0")]
			public int SampleDescriptionIndex;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleDur) != 0")]
			public int SampleDuration;
			[XmlAttribute]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleSize) != 0")]
			public int SampleSize;
			[XmlIgnore]
			[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.SampleFlags) != 0")]
			public int DefaultSampleFlags;
			//[XmlAttribute]
			//[BinData(Condition = "(TrackFragmentFlags & TrackFragmentFlags.DurEmpty) != 0")]
			//public int EmptyDuration;
		}

		/// <summary>
		/// <c>'tfdt'</c>
		/// </summary>
		[BinBlock(GetDataSizeMethod = "ResolveVersion(BaseMediaDecodeTime)")]
		public sealed partial class TFBaseMediaDecodeTimeBox : ISOMFullBox
		{
			internal const string DefaultID = "tfdt";
			public TFBaseMediaDecodeTimeBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData(BinFormat.Int64, Condition = "Version == 1")]
			[BinData(BinFormat.UInt32)]
			public long BaseMediaDecodeTime;
		}

		/// <summary>
		/// <c>'traf'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TrackFragmentBox : AtomicInfo
		{
			internal const string DefaultID = "traf";
			public TrackFragmentBox() : base(DefaultID) { }

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

		/// <summary>
		/// <c>'trun'</c>
		/// </summary>
		public sealed partial class TrackFragmentRunBox : ISOMFullBox
		{
			internal const string DefaultID = "trun";
			public TrackFragmentRunBox() : base(DefaultID) { CreateEntryCollection(out entries, this); }

			[XmlAttribute]
			public TrackRunFlags TrackRunFlags { get { return Flags.ValidFlags<TrackRunFlags>(); } set { Flags = (int)value; } }

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
			[XmlIgnore]
			public TrackFragmentRunBox Owner { get; set; }
			[XmlAttribute]
			[BinData(Condition = "(Owner.TrackRunFlags & TrackRunFlags.Duration) != 0")]
			public int Duration;
			[XmlAttribute]
			[BinData(Condition = "(Owner.TrackRunFlags & TrackRunFlags.Size) != 0")]
			public int Size;
			[XmlIgnore]
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

		/// <summary>
		/// <c>'rtp '</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RTPBox : AtomicInfo
		{
			internal const string DefaultID = "rtp ";
			public RTPBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode SubType;
			[XmlText, DefaultValue("")]
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //don't write the NULL char
			public string SDPText;
		}

		/// <summary>
		/// <c>'sdp '</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SDPBox : AtomicInfo
		{
			internal const string DefaultID = "sdp ";
			public SDPBox() : base(DefaultID) { }

			[XmlText, DefaultValue("")]
			[BinData(BinFormat.PString, LengthCustomMethod = "reader.Length()")] //sdp text has no delimiter !!!
			public string SDPText;
		}

		/// <summary>
		/// <c>'rtpo'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RTPOBox : AtomicInfo
		{
			internal const string DefaultID = "rtpo";
			public RTPOBox() : base(DefaultID) { }

			//here we have no pb, just remembed that some entries will have to
			//be 4-bytes aligned ...
			[XmlAttribute("PacketTimeOffset")]
			[BinData]
			public int TimeOffset;//s32
		}

		/// <summary>
		/// <c>'hnti'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class HintTrackInfoBox : AtomicInfo
		{
			internal const string DefaultID = "hnti";
			public HintTrackInfoBox() : base(DefaultID) { }

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

		/// <summary>
		/// <c>'rely'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class RelyHintBox : AtomicInfo
		{
			internal const string DefaultID = "rely";
			public RelyHintBox() : base(DefaultID) { }

			[BinData]
			byte data;
			[XmlAttribute, DefaultValue(0)]
			public int Reserved { get { return data.Bits(6, 2); } set { data = data.Bits(6, 2, value); } }
			[XmlAttribute]
			public bool Prefered { get { return data.Bit(1); } set { data = (byte)(value ? data | 0x2u : data & ~0x2u); } }
			[XmlAttribute]
			public bool Required { get { return (data & 0x1u) != 0; } set { data = (byte)(value ? data | 0x1u : data & ~0x1u); } }
		}

/***********************************************************
			data entry tables for RTP
***********************************************************/
		/// <summary>
		/// <c>'tims'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TSHintEntryBox : AtomicInfo
		{
			internal const string DefaultID = "tims";
			public TSHintEntryBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int TimeScale;
		}

		/// <summary>
		/// <c>'tsro'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TimeOffHintEntryBox : AtomicInfo
		{
			internal const string DefaultID = "tsro";
			public TimeOffHintEntryBox() : base(DefaultID) { }

			[XmlAttribute("TimeStampOffset")]
			[BinData]
			public int TimeOffset;
		}

		/// <summary>
		/// <c>'snro'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class SeqOffHintEntryBox : AtomicInfo
		{
			internal const string DefaultID = "snro";
			public SeqOffHintEntryBox() : base(DefaultID) { }

			[XmlAttribute("SeqNumOffset")]
			[BinData]
			public int SeqOffset;
		}

/***********************************************************
			hint track information boxes for RTP
***********************************************************/

		/*Total number of bytes that will be sent, including 12-byte RTP headers, but not including any network headers*/
		/// <summary>
		/// <c>'trpy'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TRPYBox : AtomicInfo
		{
			internal const string DefaultID = "trpy";
			public TRPYBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public long RTPBytesSent;
		}

		/*The total number of bytes that will be sent, including 12-byte RTP headers, but not including any network headers*/
		/// <summary>
		/// <c>'totl'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TOTLBox: AtomicInfo
		{
			internal const string DefaultID = "totl";
			public TOTLBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int RTPBytesSent;
		}

		/*Total number of network packets that will be sent*/
		/// <summary>
		/// <c>'nump'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NUMPBox : AtomicInfo
		{
			internal const string DefaultID = "nump";
			public NUMPBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public long PacketsSent;
		}

		/*32-bits version of nump used in Darwin*/
		/// <summary>
		/// <c>'npck'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NPCKBox : AtomicInfo
		{
			internal const string DefaultID = "npck";
			public NPCKBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int PacketsSent;
		}


		/*Total number of bytes that will be sent, not including 12-byte RTP headers*/
		/// <summary>
		/// <c>'tpyl'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NTYLBox : AtomicInfo
		{
			internal const string DefaultID = "tpyl";
			public NTYLBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public long BytesSent;
		}

		/*32-bits version of tpyl used in Darwin*/
		/// <summary>
		/// <c>'tpay'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TPAYBox : AtomicInfo
		{
			internal const string DefaultID = "tpay";
			public TPAYBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int BytesSent;
		}

		/*Maximum data rate in bits per second.*/
		/// <summary>
		/// <c>'maxr'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class MAXRBox : AtomicInfo
		{
			internal const string DefaultID = "maxr";
			public MAXRBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int Granularity;
			[XmlAttribute]
			[BinData]
			public int MaxDataRate;
		}


		/*Total number of bytes from the media track to be sent*/
		/// <summary>
		/// <c>'dmed'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DMEDBox : AtomicInfo
		{
			internal const string DefaultID = "dmed";
			public DMEDBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public long BytesSent;
		}

		/*Number of bytes of immediate data to be sent*/
		/// <summary>
		/// <c>'dimm'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DIMMBox : AtomicInfo
		{
			internal const string DefaultID = "dimm";
			public DIMMBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public long BytesSent;
		}


		/*Number of bytes of repeated data to be sent*/
		/// <summary>
		/// <c>'drep'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DREPBox : AtomicInfo
		{
			internal const string DefaultID = "drep";
			public DREPBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public long RepeatedBytes;
		}

		/*Smallest relative transmission time, in milliseconds. signed integer for smoothing*/
		/// <summary>
		/// <c>'tmin'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TMINBox : AtomicInfo
		{
			internal const string DefaultID = "tmin";
			public TMINBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int MinimumTransmitTime;//s32
		}

		/*Largest relative transmission time, in milliseconds.*/
		/// <summary>
		/// <c>'tmax'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class TMAXBox : AtomicInfo
		{
			internal const string DefaultID = "tmax";
			public TMAXBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int MaximumTransmitTime;//s32
		}

		/*Largest packet, in bytes, including 12-byte RTP header*/
		/// <summary>
		/// <c>'pmax'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PMAXBox : AtomicInfo
		{
			internal const string DefaultID = "pmax";
			public PMAXBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int MaximumSize;
		}

		/*Longest packet duration, in milliseconds*/
		/// <summary>
		/// <c>'dmax'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class DMAXBox : AtomicInfo
		{
			internal const string DefaultID = "dmax";
			public DMAXBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int MaximumDuration;
		}

		/*32-bit payload type number, followed by rtpmap payload string */
		/// <summary>
		/// <c>'payt'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PAYTBox : AtomicInfo
		{
			internal const string DefaultID = "payt";
			public PAYTBox() : base(DefaultID) { }

			[XmlAttribute("PayloadID")]
			[BinData]
			public int PayloadCode;
			[XmlAttribute, DefaultValue("")]
			[BinData(BinFormat.PString, LengthFormat = BinFormat.UInt8, LengthCustomMethod = "encoding.GetByteCount(PayloadString)")]
			public string PayloadString;
		}


		/// <summary>
		/// <c>'name'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class NameBox : AtomicInfo
		{
			internal const string DefaultID = "name";
			public NameBox() : base(DefaultID) { }

			[XmlAttribute("Name"), DefaultValue("")]
			[BinData]
			public string String;
		}

		/// <summary>
		/// <c>'hinf'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class HintInfoBox : AtomicInfo
		{
			internal const string DefaultID = "hinf";
			public HintInfoBox() : base(DefaultID) { }

			[XmlIgnore]
			public IEnumerable<MAXRBox> DataRates { get { return this.boxList.OfType<MAXRBox>(); } } //Unique by Granularity
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
			public long PlainTextLength;
			[XmlAttribute]
			[BinData]
			public string ContentID;
			[XmlAttribute]
			[BinData]
			public string RightsIssuerURL;
			[XmlIgnore]
			[BinArray]
			[BinData]
			public string[] TextualHeaders;
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
			[XmlElement(DataType = "hexBinary")]
			[BinData]
			public byte[] GroupKey;
		}

		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class OMADRMMutableInformationBox : AtomicInfo
		{
			[BinCustom]
			TypedBoxList boxList = new TypedBoxList(AllowUnknownBox);
		}

		public sealed partial class OMADRMTransactionTrackingBox : ISOMFullBox
		{
			[XmlElement(DataType = "hexBinary")]
			[BinData]
			public byte[] TransactionID;//char TransactionID[16];
		}

		public sealed partial class OMADRMRightsObjectBox : ISOMFullBox
		{
			[XmlElement(DataType = "hexBinary")]
			[BinData]
			public byte[] OMARightsObject;
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
			[XmlAttribute("Type")]
			public bool ReferenceType { get { return referenceData.Bit(31); } set { referenceData = referenceData.Bit(31, value); } }
			[XmlAttribute("Size")]
			public int ReferenceSize { get { return (int)referenceData.Bits(31, 0); } set { referenceData = referenceData.Bits(31, 0, (uint)value); } }
			[XmlAttribute("Duration")]
			[BinData]
			public int SubsegmentDuration;
			[BinData]
			uint sapData;
			[XmlAttribute]
			public bool StartsWithSAP { get { return sapData.Bit(31); } set { sapData = sapData.Bit(31, value); } }
			[XmlAttribute]
			public int SAPType { get { return (int)sapData.Bits(3, 28); } set { sapData = sapData.Bits(3, 28, (uint)value); } }
			[XmlAttribute]
			public int SAPDeltaTime { get { return (int)sapData.Bits(28, 0); } set { sapData = sapData.Bits(28, 0, (uint)value); } }
		}

		/// <summary>
		/// <c>'sidx'</c>
		/// </summary>
		public sealed partial class SegmentIndexBox : ISOMFullBox
		{
			internal const string DefaultID = "sidx";
			public SegmentIndexBox() : base(DefaultID) { }

			[XmlAttribute]
			[BinData]
			public int ReferenceID;
			[XmlAttribute]
			[BinData]
			public int TimeScale;
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
			[XmlElement("Reference")]
			[BinArray(CountFormat = BinFormat.UInt16)]
			public List<SIDXReference> Refs = new List<SIDXReference>();
		}

		/// <summary>
		/// <c>'pcrb'</c>
		/// </summary>
		[BinBlock(MethodMode = BinMethodMode.Abstract)]
		public sealed partial class PcrInfoBox : AtomicInfo
		{
			internal const string DefaultID = "pcrb";
			public PcrInfoBox() : base(DefaultID) { }

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

		/// <summary>
		/// <c>'sbgp'</c>
		/// </summary>
		public sealed partial class SampleGroupBox : ISOMFullBox
		{
			internal const string DefaultID = "sbgp";
			public SampleGroupBox() : base(DefaultID) { }

			[XmlIgnore]
			[BinData(BinFormat.UInt32)]
			public AtomicCode GroupingType;
			[XmlAttribute]
			[BinData(Condition = "Version == 1")]
			public int GroupingTypeParameter;

			[BinArray(CountFormat = BinFormat.Int32)]
			List<SampleGroupEntry> sampleEntries = new List<SampleGroupEntry>();
		}

		/// <summary>
		/// <c>'sgpd'</c>
		/// </summary>
		public sealed partial class SampleGroupDescriptionBox : ISOMFullBox
		{
			internal const string DefaultID = "sgpd";
			/*version 0 is deprecated, use v1 by default*/
			public SampleGroupDescriptionBox() : base(DefaultID, version: 1) { CreateEntryCollection(out groupDescriptions, this); }

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

		[BinBlock(MethodMode = BinMethodMode.Virtual)]
		public abstract partial class SampleGroupDescriptionEntry : IEntry<SampleGroupDescriptionBox>
		{
			[XmlIgnore]
			public SampleGroupDescriptionBox Owner { get; set; }

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
			public bool NumLeadingSamplesKnown { get { return data.Bit(7); } set { data = data.Bit(7, value); } }
			[XmlAttribute]
			public byte NumLeadingSamples { get { return (byte)data.Bits(7, 0); } set { data = data.Bits(7, 0, value); } }
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
