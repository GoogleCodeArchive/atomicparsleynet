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

namespace ID3v2
{
	/// <summary>
	/// the order of these frames must exactly match the order listed in the KnownFrames[] array!!!
	/// </summary>
	public enum FrameIDs
	{
		Unknown = -1,
		Album,
		BPM,
		Composer,
		ContentType,
		Copyright,
		EncodingTime,
		PlayListDelay,
		OrigRelTime,
		RecordingTime,
		ReleaseTime,
		TaggingTime,
		Encoder,
		Lyricist,
		FileType,
		InvolvedPeople,
		GroupDesc,
		Title,
		Subtitle,
		InitialKey,
		Language,
		TimeLength,
		MusicianList,
		MediaType,
		Mood,
		OrigAlbum,
		OrigFilename,
		OrigWriter,
		OrigArtist,
		FileOwner,
		Artist,
		AlbumArtist,
		Conductor,
		Remixer,
		PartOfSet,
		ProdNotice,
		Publisher,
		TrackNum,
		IRadioName,
		IRadioOwner,
		AlbumSort,
		PerformerSort,
		TitleSort,
		ISrc,
		EncodingSettings,
		SetSubtitle,
		Date,
		Time,
		OrigRelYear,
		RecordingDate,
		Size,
		Year,
		UserDefText,
		URLCommInfo,
		URLCopyright,
		URLAudioFile,
		URLArtist,
		URLAudioSource,
		URLIRadio,
		URLPayment,
		URLPublisher,
		UserDefURL,
		UFID,
		MusicCDID,
		Comment,
		UnsyncLyrics,
		EmbeddedPicture,
		EmbeddedPictureV2p2,
		EmbeddedObject,
		GrID,
		Signature,
		Private,
		PlayCounter,
		Popularity
	}

	/// <summary>
	/// Structure that defines the (subset) known ID3 frames defined by id3 informal specification.
	/// </summary>
	public class ID3FrameDefinition
	{
		public string FrameIDp2 { get; private set; }
		public string FrameIDp3 { get; private set; }
		public string FrameIDp4 { get; private set; }
		public string FrameDescription { get; private set; }
		public string FrameIDpreset { get; private set; }
		public FrameIDs InternalFrameID { get; private set; }
		public FrameType FrameType { get; private set; }

		public ID3FrameDefinition(string idp2, string idp3, string idp4, string desc, string preset, FrameIDs id, FrameType type)
		{
			this.FrameIDp2 = idp2;
			this.FrameIDp3 = idp3;
			this.FrameIDp4 = idp4;
			this.FrameDescription = desc;
			this.FrameIDpreset = preset;
			this.InternalFrameID = id;
			this.FrameType = type;
		}
	}

	public class ImageFileFormatDefinition
	{
		public string ImageMIMEType { get; private set; }
		public string ImageFileExtn { get; private set; }
		public byte[] ImageBinaryHeader { get; private set; }

		public ImageFileFormatDefinition(string type, string ext, params byte[] header)
		{
			this.ImageMIMEType = type;
			this.ImageFileExtn = ext;
			this.ImageBinaryHeader = header;
		}
	}

	public class ID3ImageType
	{
		public byte HexCode { get; private set; }
		public string ImageType { get; private set; }

		public ID3ImageType(byte code, string type)
		{
			this.HexCode = code;
			this.ImageType = type;
		}
	}

	/// <summary>
	/// Structure that defines how any ID3v2FrameType is constructed, listing an array of its constituent FieldTypes
	/// </summary>
	public abstract class ID3v2FieldDefinition
	{
		public FrameType FrameType { get; private set; }
		public Type FrameClass { get; private set; }
		public FieldTypes[] FieldComponents { get; private set; }

		protected ID3v2FieldDefinition(FrameType type, Type obj, FieldTypes[] components)
		{
			this.FrameType = type;
			this.FrameClass = obj;
			this.FieldComponents = components;
		}

		public abstract ID3v2Frame CreateFrame();
	}

	public class ID3v2FieldDefinition<TFrame> : ID3v2FieldDefinition
		where TFrame : ID3v2Frame, new()
	{
		public ID3v2FieldDefinition(FrameType type, params FieldTypes[] components)
			: base(type, typeof(TFrame), components) { }

		public override ID3v2Frame CreateFrame()
		{
			return new TFrame();
		}
	}
}
