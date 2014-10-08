//==================================================================//
/*
    AtomicParsley - id3v2defs.h

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
using System.Linq;
using System.Collections.Generic;
using ID3v2.Frames;

namespace ID3v2
{
	public static class Definitions
	{
		public static readonly ID3FrameDefinition[] KnownFrameList = new ID3FrameDefinition[]
		{
			new ID3FrameDefinition( "",    "",     "",     "Unknown frame",                            "",                FrameIDs.Unknown,             FrameType.Unknown ),
			new ID3FrameDefinition( "TAL", "TALB", "TALB", "Album/Movie/Show title",                   "album",           FrameIDs.Album,               FrameType.Text ),
			new ID3FrameDefinition( "TBP", "TBPM", "TBPM", "BPM (beats per minute)",                   "bpm",             FrameIDs.BPM,                 FrameType.Text ),
			new ID3FrameDefinition( "TCM", "TCOM", "TCOM", "Composer",                                 "composer",        FrameIDs.Composer,            FrameType.Text ),
			new ID3FrameDefinition( "TCO", "TCON", "TCON", "Content Type/Genre",                       "genre",           FrameIDs.ContentType,         FrameType.Text ),
			new ID3FrameDefinition( "TCP", "TCOP", "TCOP", "Copyright message",                        "copyright",       FrameIDs.Copyright,           FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TDEN", "Encoding time",                            "",                FrameIDs.EncodingTime,        FrameType.Text ),
			new ID3FrameDefinition( "TDY", "TDLY", "TDLY", "Playlist delay",                           "",                FrameIDs.PlayListDelay,       FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TDOR", "Original release time",                    "",                FrameIDs.OrigRelTime,         FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TDRC", "Recording time",                           "date",            FrameIDs.RecordingTime,       FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TDRL", "Release time",                             "released",        FrameIDs.ReleaseTime,         FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TDTG", "Tagging time",                             "tagged",          FrameIDs.TaggingTime,         FrameType.Text ),
			new ID3FrameDefinition( "TEN", "TENC", "TENC", "Encoded by",                               "encoder",         FrameIDs.Encoder,             FrameType.Text ),
			new ID3FrameDefinition( "TXT", "TEXT", "TEXT", "Lyricist/Text writer",                     "writer",          FrameIDs.Lyricist,            FrameType.Text ),
			new ID3FrameDefinition( "TFT", "TFLT", "TFLT", "File type",                                "",                FrameIDs.FileType,            FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TIPL", "Involved people list",                     "",                FrameIDs.InvolvedPeople,      FrameType.Text ),
			new ID3FrameDefinition( "TT1", "TIT1", "TIT1", "Content group description",                "grouping",        FrameIDs.GroupDesc,           FrameType.Text ),
			new ID3FrameDefinition( "TT2", "TIT2", "TIT2", "Title/songname/content description",       "title",           FrameIDs.Title,               FrameType.Text ),
			new ID3FrameDefinition( "TT3", "TIT3", "TIT3", "Subtitle/Description refinement",          "subtitle",        FrameIDs.Subtitle,            FrameType.Text ),
			new ID3FrameDefinition( "TKE", "TKEY", "TKEY", "Initial key",                              "",                FrameIDs.InitialKey,          FrameType.Text ),
			new ID3FrameDefinition( "TLA", "TLAN", "TLAN", "Language(s)",                              "",                FrameIDs.Language,            FrameType.Text ),
			new ID3FrameDefinition( "TLE", "TLEN", "TLEN", "Length",                                   "",                FrameIDs.TimeLength,          FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TMCL", "Musician credits list",                    "credits",         FrameIDs.MusicianList,        FrameType.Text ),
			new ID3FrameDefinition( "TMT", "TMED", "TMED", "Media type",                               "media",           FrameIDs.MediaType,           FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TMOO", "Mood",                                     "mood",            FrameIDs.Mood,                FrameType.Text ),
			new ID3FrameDefinition( "TOT", "TOAL", "TOAL", "Original album/movie/show title",          "",                FrameIDs.OrigAlbum,           FrameType.Text ),
			new ID3FrameDefinition( "TOF", "TOFN", "TOFN", "Original filename",                        "",                FrameIDs.OrigFilename,        FrameType.Text ),
			new ID3FrameDefinition( "TOL", "TOLY", "TOLY", "Original lyricist(s)/text writer(s)",      "",                FrameIDs.OrigWriter,          FrameType.Text ),
			new ID3FrameDefinition( "TOA", "TOPE", "TOPE", "Original artist(s)/performer(s)",          "",                FrameIDs.OrigArtist,          FrameType.Text ),
			new ID3FrameDefinition( "",    "TOWN", "TOWN", "File owner/licensee",                      "",                FrameIDs.FileOwner,           FrameType.Text ),
			new ID3FrameDefinition( "TP1", "TPE1", "TPE1", "Artist/Lead performer(s)/Soloist(s)",      "artist",          FrameIDs.Artist,              FrameType.Text ),
			new ID3FrameDefinition( "TP2", "TPE2", "TPE2", "Album artist/Band/orchestra/accompaniment", "album artist",   FrameIDs.AlbumArtist,         FrameType.Text ),
			new ID3FrameDefinition( "TP3", "TPE3", "TPE3", "Conductor/performer refinement",           "conductor",       FrameIDs.Conductor,           FrameType.Text ),
			new ID3FrameDefinition( "TP4", "TPE4", "TPE4", "Interpreted or remixed by",                "remixer",         FrameIDs.Remixer,             FrameType.Text ),
			new ID3FrameDefinition( "TPA", "TPOS", "TPOS", "Part of a set",                            "",                FrameIDs.PartOfSet,           FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TPRO", "Produced notice",                          "",                FrameIDs.ProdNotice,          FrameType.Text ),
			new ID3FrameDefinition( "TPB", "TPUB", "TPUB", "Publisher",                                "publisher",       FrameIDs.Publisher,           FrameType.Text ),
			new ID3FrameDefinition( "TRK", "TRCK", "TRCK", "Track number/Position in set",             "trk#",            FrameIDs.TrackNum,            FrameType.Text ),
			new ID3FrameDefinition( "",    "TRSN", "TRSN", "Internet radio station name",              "",                FrameIDs.IRadioName,          FrameType.Text ),
			new ID3FrameDefinition( "",    "TRSO", "TRSO", "Internet radio station owner",             "",                FrameIDs.IRadioOwner,         FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TSOA", "Album sort order",                         "",                FrameIDs.AlbumSort,           FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TSOP", "Performer sort order",                     "",                FrameIDs.PerformerSort,       FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TSOT", "Title sort order",                         "",                FrameIDs.TitleSort,           FrameType.Text ),
			new ID3FrameDefinition( "TRC", "TSRC", "TSRC", "ISRC",                                     "",                FrameIDs.ISrc,                FrameType.Text ),
			new ID3FrameDefinition( "TSS", "TSSE", "TSSE", "Software/Hardware and settings used for encoding", "",        FrameIDs.EncodingSettings,    FrameType.Text ),
			new ID3FrameDefinition( "",    "",     "TSST", "Set subtitle",                             "",                FrameIDs.SetSubtitle,         FrameType.Text ),
	
			new ID3FrameDefinition( "TDA", "TDAT", "",     "Date",                                     "",                FrameIDs.Date,                FrameType.Text ),
			new ID3FrameDefinition( "TIM", "TIME", "",     "TIME",                                     "",                FrameIDs.Time,                FrameType.Text ),
			new ID3FrameDefinition( "TOR", "TORY", "",     "Original Release Year",                    "",                FrameIDs.OrigRelYear,         FrameType.Text ),
			new ID3FrameDefinition( "TRD", "TRDA", "",     "Recording dates",                          "",                FrameIDs.RecordingDate,       FrameType.Text ),
			new ID3FrameDefinition( "TSI", "TSIZ", "",     "Size",                                     "",                FrameIDs.Size,                FrameType.Text ),
			new ID3FrameDefinition( "TYE", "TYER", "",     "YEAR",                                     "",                FrameIDs.Year,                FrameType.Text ),
	
			new ID3FrameDefinition( "TXX", "TXXX", "TXXX", "User defined text information frame",      "",                FrameIDs.UserDefText,         FrameType.TextUserDef ),
	
			//some of these (like WCOM, WOAF) allow for muliple frames - but (sigh) alas, such is not the case in AP. 
			new ID3FrameDefinition( "WCM", "WCOM", "WCOM", "Commercial information",                   "",                FrameIDs.URLCommInfo,         FrameType.URL ),
			new ID3FrameDefinition( "WCP", "WCOP", "WCOP", "Copyright/Legal information",              "",                FrameIDs.URLCopyright,        FrameType.URL ),
			new ID3FrameDefinition( "WAF", "WOAF", "WOAF", "Official audio file webpage",              "",                FrameIDs.URLAudioFile,        FrameType.URL ),
			new ID3FrameDefinition( "WAR", "WOAR", "WOAR", "Official artist/performer webpage",        "",                FrameIDs.URLArtist,           FrameType.URL ),
			new ID3FrameDefinition( "WAS", "WOAS", "WOAS", "Official audio source webpage",            "",                FrameIDs.URLAudioSource,      FrameType.URL ),
			new ID3FrameDefinition( "",    "WORS", "WORS", "Official Internet radio station homepage", "",                FrameIDs.URLIRadio,           FrameType.URL ),
			new ID3FrameDefinition( "",    "WPAY", "WPAY", "Payment",                                  "",                FrameIDs.URLPayment,          FrameType.URL ),
			new ID3FrameDefinition( "WPB", "WPUB", "WPUB", "Publishers official webpage",              "",                FrameIDs.URLPublisher,        FrameType.URL ),
			new ID3FrameDefinition( "WXX", "WXXX", "WXXX", "User defined URL link frame",              "",                FrameIDs.UserDefURL,          FrameType.URLUserDef ),
	
			new ID3FrameDefinition( "UFI", "UFID", "UFID", "Unique file identifier",                   "",                FrameIDs.UFID,                FrameType.UniqueFileID ),
			new ID3FrameDefinition( "MCI", "MCID", "MCDI", "Music CD Identifier",                      "",                FrameIDs.MusicCDID,           FrameType.CDID ),
	
			new ID3FrameDefinition( "COM", "COMM", "COMM", "Comment",                                  "comment",         FrameIDs.Comment,             FrameType.DescribedText ),
			new ID3FrameDefinition( "ULT", "USLT", "USLT", "Unsynchronised lyrics",                    "lyrics",          FrameIDs.UnsyncLyrics,        FrameType.DescribedText ),
	
			new ID3FrameDefinition( "",    "APIC", "APIC", "Attached picture",                         "",                FrameIDs.EmbeddedPicture,     FrameType.AttachedPicture ),
			new ID3FrameDefinition( "PIC", "",     "",     "Attached picture",                         "",                FrameIDs.EmbeddedPictureV2p2, FrameType.OldV2p2Picture ),
			new ID3FrameDefinition( "GEO", "GEOB", "GEOB", "Attached object",                          "",                FrameIDs.EmbeddedObject,      FrameType.AttachedObject ),
	
			new ID3FrameDefinition( "",    "GRID", "GRID", "Group ID registration",                    "",                FrameIDs.GrID,                FrameType.GroupID ),
			new ID3FrameDefinition( "",    "",     "SIGN", "Signature",                                "",                FrameIDs.Signature,           FrameType.Signature ),
			new ID3FrameDefinition( "",    "PRIV", "PRIV", "Private frame",                            "",                FrameIDs.Private,             FrameType.Private ),
			new ID3FrameDefinition( "CNT", "PCNT", "PCNT", "Play counter",                             "",                FrameIDs.PlayCounter,         FrameType.PlayCounter ),
			new ID3FrameDefinition( "POP", "POPM", "POPM", "Popularimeter",                            "",                FrameIDs.Popularity,          FrameType.Popular )
		};
		public static readonly IDictionary<FrameIDs, ID3FrameDefinition> KnownFrames = KnownFrameList.ToDictionary(def => def.InternalFrameID);
		public static readonly IDictionary<string, ID3FrameDefinition> KnownFramesP2 = KnownFrameList.Where(def => !String.IsNullOrEmpty(def.FrameIDp2)).ToDictionary(def => def.FrameIDp2);
		public static readonly IDictionary<string, ID3FrameDefinition> KnownFramesP3 = KnownFrameList.Where(def => !String.IsNullOrEmpty(def.FrameIDp3)).ToDictionary(def => def.FrameIDp3);
		public static readonly IDictionary<string, ID3FrameDefinition> KnownFramesP4 = KnownFrameList.Where(def => !String.IsNullOrEmpty(def.FrameIDp4)).ToDictionary(def => def.FrameIDp4);

		public static readonly ID3v2FieldDefinition[] FrameTypeConstructionList = new ID3v2FieldDefinition[]
		{
			new ID3v2FieldDefinition<UnknownFrame        >( FrameType.Unknown,          FieldTypes.Unknown ),
			new ID3v2FieldDefinition<TextFrame           >( FrameType.Text,             FieldTypes.TextEncoding, FieldTypes.Text ),
			new ID3v2FieldDefinition<TextFrameUserDef    >( FrameType.TextUserDef,      FieldTypes.TextEncoding, FieldTypes.Description, FieldTypes.Text ),
			new ID3v2FieldDefinition<URLFrame            >( FrameType.URL,              FieldTypes.URL ),
			new ID3v2FieldDefinition<URLFrameUserDef     >( FrameType.URLUserDef,       FieldTypes.TextEncoding, FieldTypes.Description, FieldTypes.URL ),
			new ID3v2FieldDefinition<UniqueFileIDFrame   >( FrameType.UniqueFileID,     FieldTypes.Owner, FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<CDIDFrame           >( FrameType.CDID,             FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<DescribedTextFrame  >( FrameType.DescribedText,    FieldTypes.TextEncoding, FieldTypes.Language, FieldTypes.Description, FieldTypes.Text ),
			new ID3v2FieldDefinition<AttachedPictureFrame>( FrameType.AttachedPicture,  FieldTypes.TextEncoding, FieldTypes.MIMEType, FieldTypes.PicType, FieldTypes.Description, FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<AttachedObjectFrame >( FrameType.AttachedObject,   FieldTypes.TextEncoding, FieldTypes.MIMEType, FieldTypes.Filename, FieldTypes.Description, FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<GroupIDFrame        >( FrameType.GroupID,          FieldTypes.Owner, FieldTypes.GroupSymbol, FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<SignatureFrame      >( FrameType.Signature,        FieldTypes.GroupSymbol, FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<PrivateFrame        >( FrameType.Private,          FieldTypes.Owner, FieldTypes.BinaryData ),
			new ID3v2FieldDefinition<PlayCounterFrame    >( FrameType.PlayCounter,      FieldTypes.Counter ),
			new ID3v2FieldDefinition<PopularFrame        >( FrameType.Popular,          FieldTypes.Owner, FieldTypes.BinaryData, FieldTypes.Counter ),
			new ID3v2FieldDefinition<OldV2P2PictureFrame >( FrameType.OldV2p2Picture,   FieldTypes.TextEncoding, FieldTypes.ImageFormat, FieldTypes.PicType, FieldTypes.Description, FieldTypes.BinaryData )
		};
		public static readonly IDictionary<FrameType, ID3v2FieldDefinition> FrameTypeConstruction = FrameTypeConstructionList.ToDictionary(con => con.FrameType);

		public static readonly ImageFileFormatDefinition[] ImageList = new ImageFileFormatDefinition[]
		{
			new ImageFileFormatDefinition( "image/jpeg",       ".jpg",  0xFF, 0xD8, 0xFF, 0xE0 ),
			new ImageFileFormatDefinition( "image/jpeg",       ".jpg",  0xFF, 0xD8, 0xFF, 0xE1 ),
			new ImageFileFormatDefinition( "image/png",        ".png",  0x89, (byte)'P', (byte)'N', (byte)'G', 0x0D, 0x0A, 0x1A, 0x0A ),
			new ImageFileFormatDefinition( "image/pdf",        ".pdf",  (byte)'%', (byte)'P', (byte)'D', (byte)'F', (byte)'-', (byte)'1', (byte)'.' ), //%PDF-1.
			new ImageFileFormatDefinition( "image/jp2",        ".jp2",  0x00, 0x00, 0x00, 0x0C, (byte)'j', (byte)'P', (byte)' ', (byte)' ', 0x0D, 0x0A, 0x87, 0x0A, 0x00, 0x00, 0x00, 0x14, (byte)'f', (byte)'t', (byte)'y', (byte)'p', (byte)'j', (byte)'p', (byte)'2', (byte)' ' ),
			new ImageFileFormatDefinition( "image/gif",        ".gif",  (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a' ), //GIF89a
			new ImageFileFormatDefinition( "image/tiff",       ".tiff", (byte)'M', (byte)'M', 0x00, (byte)'*' ),
			new ImageFileFormatDefinition( "image/tiff",       ".tiff", (byte)'I', (byte)'I', (byte)'*', 0x00 ),
			new ImageFileFormatDefinition( "image/bmp",        ".bmp",  (byte)'B', (byte)'M' ),
			new ImageFileFormatDefinition( "image/bmp",        ".bmp",  (byte)'B', (byte)'A' ),
			new ImageFileFormatDefinition( "image/photoshop",  ".psd",  (byte)'8', (byte)'B', (byte)'P', (byte)'S' ), //8BPS
			new ImageFileFormatDefinition( "image/other",      ".img",  new byte[] { } )
		};

		public static readonly ID3ImageType[] ImageTypeList = new ID3ImageType[]
		{
			new ID3ImageType( 0x00, "Other" ),
			new ID3ImageType( 0x01, "32x32 pixels 'file icon' (PNG only)" ),
			new ID3ImageType( 0x02, "Other file icon" ),
			new ID3ImageType( 0x03, "Cover (front)" ),
			new ID3ImageType( 0x04, "Cover (back)" ),
			new ID3ImageType( 0x05, "Leaflet page" ),
			new ID3ImageType( 0x06, "Media (e.g. label side of CD)" ),
			new ID3ImageType( 0x07, "Lead artist/lead performer/soloist" ),
			new ID3ImageType( 0x08, "Artist/performer" ),
			new ID3ImageType( 0x09, "Conductor" ),
			new ID3ImageType( 0x0A, "Band/Orchestra" ),
			new ID3ImageType( 0x0B, "Composer" ),
			new ID3ImageType( 0x0C, "Lyricist/text writer" ),
			new ID3ImageType( 0x0D, "Recording Location" ),
			new ID3ImageType( 0x0E, "During recording" ),
			new ID3ImageType( 0x0F, "During performance" ),
			new ID3ImageType( 0x10, "Movie/video screen capture" ),
			new ID3ImageType( 0x11, "A bright coloured fish" ),
			new ID3ImageType( 0x12, "Illustration" ),
			new ID3ImageType( 0x13, "Band/artist logotype" ),
			new ID3ImageType( 0x14, "Publisher/Studio logotype" )
		};

		public static ID3FrameDefinition MatchID3FrameIDstr(TagID foundFrameID, int tagVersion)
		{
			ID3FrameDefinition testFrame;
			switch (tagVersion)
			{
			case 2:
				if (KnownFramesP2.TryGetValue((string)foundFrameID, out testFrame))
					return testFrame;
				break;
			case 3:
				if (KnownFramesP3.TryGetValue((string)foundFrameID, out testFrame))
					return testFrame;
				break;
			case 4:
				if (KnownFramesP4.TryGetValue((string)foundFrameID, out testFrame))
					return testFrame;
				break;
			}

			return KnownFrames[FrameIDs.Unknown]; //return the UnknownFrame if it can't be found
		}

		public static ID3v2FieldDefinition GetFrameCompositionDescription(FrameType frameTypeID)
		{
			ID3v2FieldDefinition testField;
			if (FrameTypeConstruction.TryGetValue(frameTypeID, out testField))
				return testField;
			return FrameTypeConstructionList[0]; //return the UnknownFrame/UnknownField if it can't be found
		}

		public static FrameType FrameStr2FrameType(string frame_str, int tagVersion)
		{
			ID3FrameDefinition testFrame;
			switch (tagVersion)
			{
			case 2:
				if (KnownFramesP2.TryGetValue(frame_str, out testFrame))
					return testFrame.FrameType;
				break;
			case 3:
				if (KnownFramesP3.TryGetValue(frame_str, out testFrame))
					return testFrame.FrameType;
				break;
			case 4:
				if (KnownFramesP4.TryGetValue(frame_str, out testFrame))
					return testFrame.FrameType;
				break;
			}

			return FrameType.Text; //?? FrameType.UnknownFrame
		}
	}
}
