//==================================================================//
/*
    AtomicParsley - main.cpp

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

    Copyright ©2005-2007 puck_lock
    with contributions from others; see the CREDITS file

    ----------------------
    Code Contributions by:

    * Mike Brancato - Debian patches & build support
    * Brian Story - porting getopt & native Win32 patches
    ----------------------
    SVN revision information:
      $Revision$
                                                                   */
//==================================================================//
using System.IO;
using System.Linq;
using AtomicParsley.CommandLine;

namespace AtomicParsley
{
	partial class Program
	{
		static readonly string cmdAtomicParsley = Path.GetFileNameWithoutExtension(typeof(Program).Assembly.CodeBase);
		static SwitchOption optHelp = new SwitchOption("help", "h ? help");
		static SwitchOption optVersion = new SwitchOption("version", "v version");
		static SwitchOption optLongHelp = new SwitchOption("longhelp", "Lh longhelp");
		static SwitchOption optReverseDNSHelp = new SwitchOption("reverseDNS-help", "rh rDNS-help");
		static SwitchOption opt3GPHelp = new SwitchOption("3gp-help", "3gp-h 3gp-help");
		static SwitchOption isoHelp = new SwitchOption("ISO-help iso-help", "Ih");
		static SwitchOption optFileHelp = new SwitchOption("file-help", "fh file-help");
		static SwitchOption optUUIDHelp = new SwitchOption("uuid-help", "uh uuid-help");
		static SwitchOption optID3Help = new SwitchOption("ID3-help", "ID3h ID3-help");
		static SwitchOption optGenreList = new SwitchOption("genre-list");
		static SwitchOption optStikList = new SwitchOption("stik-list");
		static SwitchOption optLanguageList = new SwitchOption("language-list languages-list list-language list-languages", "ll");
		static SwitchOption optMacLanguageList = new SwitchOption("mac-language-list mac-languages-list list-mac-language list-mac-languages", "mll");
		static SwitchOption optRatingsList = new SwitchOption("ratings-list");
		static SwitchOption optGenreMovieIDList = new SwitchOption("genre-movie-id-list");
		static SwitchOption optGenreTVIDList = new SwitchOption("genre-tv-id-list");
		static SwitchOption optID3FramesList = new SwitchOption("ID3frames-list");
		static SwitchOption optImageTypeList = new SwitchOption("imagetype-list");
		static SwitchOption optBrands = new SwitchOption("brands", "brands")
		{
			Help = @"Show the major & minor brands for the file & available tagging schemes"
		};
		static SwitchOption optDeepScan = new SwitchOption("DeepScan")
		{
			Help = @"Parse areas of the file that are normally skipped (must be the 3rd arg)"
		};

		static SwitchOption optTest = new SwitchOption("test", 'T')
		{
			Description = @"Test file for mpeg4-ishness & print atom tree",
			Help =
@"Tests file to see if it is a valid MPEG-4 file.
Prints out the hierarchical atom tree."
		};
		static StringOption optTest1 = new StringOption("test", 'T', "1")
		{
			Help = @"Supplemental track level info with ""-T 1"""
		};
		static StringOption optTestDates = new StringOption("test", 'T', "+dates")
		{
			Help =
@"Track level with creation/modified dates
"
		};
		static SwitchOption optShowTextData = new SwitchOption("textdata", 't')
		{
			Description = @"Prints tags embedded within the file",
			Help = @"Print user data text metadata relevant to brand (inc. # of any pics)."
		};
		static StringOption optShowTextDataPlus = new StringOption("textdata", 't', "+")
		{
			Help = @"Show supplemental info like free space, available padding, user data length & media data length"
		};
		static StringOption optShowTextData1 = new StringOption("textdata", 't', "1")
		{
			Help =
@"Show all textual metadata (disregards brands, shows track copyright)
"
		};
		static SwitchOption optExtractPix = new SwitchOption("extractPix", 'E')
		{
			Description = @"Extracts pix to the same folder as the mpeg-4 file",
			Help = @"Extract to same folder (basename derived from file)."
		};
		static StringOption optExtractPixToPath = new StringOption("extractPixToPath", 'e', @"(\path\basename)")
		{
			Help =
@"Extract to specific path (numbers added to basename).
  example:	-e \Desktop\SomeText
  gives:	SomeText_artwork_1.jpg SomeText_artwork_2.png
Note:	extension comes from embedded image file format"
		};
		static StringOption metaArtist = new StringOption("artist", 'a', "(str)")
		{
			Description = @"Set the artist tag",
			Help = @"Set the artist tag: ""moov.udta.meta.ilst.©ART.data"""
		};
		static StringOption metaArtDirector = new StringOption("artDirector", "");
		static StringOption metaArranger = new StringOption("arranger", "");
		static StringOption metaAuthor = new StringOption("author", "");
		static StringOption metaConductor = new StringOption("conductor", "");
		static StringOption metaDirector = new StringOption("director", "");
		static StringOption metaOriginalArtist = new StringOption("originalArtist", "");
		static StringOption metaProducer = new StringOption("producer", "");
		static StringOption metaPerformer = new StringOption("performer", "");
		static StringOption metaSoundEngineer = new StringOption("soundEngineer", "");
		static StringOption metaSoloist = new StringOption("soloist", "");
		static StringOption metaExecutiveProducer = new StringOption("executiveProducer", "");
		static StringOption metaSongTitle = new StringOption("title", 's', "(str)")
		{
			Description = @"Set the title tag",
			Help = @"Set the title tag: ""moov.udta.meta.ilst.©nam.data"""
		};
		static StringOption metaSubtitle = new StringOption("subtitle", "");
		static StringOption metaAlbum = new StringOption("album", 'b', "(str)")
		{
			Description = @"Set the album tag",
			Help = @"Set the album tag: ""moov.udta.meta.ilst.©alb.data"""
		};
		static StringOption metaTracknum = new StringOption("tracknum", 'k', "(num)[/tot]")
		{
			Description = @"Track number (or track number/total tracks)",
			Help = @"Set the track number (or track number & total tracks)."
		};
		static StringOption metaDisknum = new StringOption("disknum", 'd', "(num)[/tot]")
		{
			Description = @"Disk number (or disk number/total disks)",
			Help = @"Set the disk number (or disk number & total disks)."
		};
		static StringOption metaGenre = new StringOption("genre", 'g', "(str)")
		{
			Description = @"Genre tag (see --" + optLongHelp.Name + @" for more info)",
			Help =
@"Set the genre tag: ""©gen"" (custom) or ""gnre"" (standard).
see the standard list with """ + cmdAtomicParsley + @" --" + optGenreList.Name + @""""
		};
		static StringOption metaComment = new StringOption("comment", 'c', "(str)")
		{
			Description = @"Set the comment tag",
			Help = @"Set the comment tag: ""moov.udta.meta.ilst.©cmt.data"""
		};
		static StringOption metaYear = new StringOption("year", 'y', "(num|UTC)")
		{
			Description = @"Year tag (see --" + optLongHelp.Name + @" for ""Release Date"")",
			Help =
@"Set the year tag: ""moov.udta.meta.ilst.©day.data""
set with UTC ""2006-09-11T09:00:00Z"" for Release Date"
		};
		static StringOption metaLyrics = new StringOption("lyrics", 'l', "(str)")
		{
			Description = @"Set lyrics (not subject to 256 byte limit)",
			Help = @"Set the lyrics tag: ""moov.udta.meta.ilst.©lyr.data"""
		};
		static StringOption metaLyricsFile = new StringOption("lyricsFile", @"(\path)")
		{
			Description = @"Set lyrics to the content of a file",
			Help = @"Set the lyrics tag to the content of a file"
		};
		static StringOption metaComposer = new StringOption("composer", 'w', "(str)")
		{
			Description = @"Set the composer tag",
			Help = @"Set the composer tag: ""moov.udta.meta.ilst.©wrt.data"""
		};
		static StringOption metaCopyright = new StringOption("copyright", 'x', "(str)")
		{
			Description = @"Set the copyright tag",
			Help = @"Set the copyright tag: ""moov.udta.meta.ilst.cprt.data"""
		};
		static StringOption metaGrouping = new StringOption("grouping", 'G', "(str)")
		{
			Description = @"Set the grouping tag",
			Help = @"Set the grouping tag: ""moov.udta.meta.ilst.\302©grp.data"""
		};
		static StringOption metaAlbumArtist = new StringOption("albumArtist", 'A', "(str)")
		{
			Description = @"Set the album artist tag",
			Help = @"Set the album artist tag: ""moov.udta.meta.ilst.aART.data"""
		};
		static StringOption metaCompilation = new StringOption("compilation", 'C', "(bool)")
		{
			Description = @"Set the compilation flag (true or false)",
			Help = @"Sets the ""cpil"" atom (true or false to delete the atom)"
		};
		static StringOption metaHDVideo = new StringOption("hdvideo", 'O', "(bool)")
		{
			Description = @"Set the hdvideo flag (true or false)",
			Help = @"Sets the ""hdvd"" atom (true or false to delete the atom)"
		};
		static StringOption metaBPM = new StringOption("bpm", 'B', "(num)")
		{
			Description = @"Set the tempo/bpm",
			Help = @"Set the tempo/bpm tag: ""moov.udta.meta.ilst.tmpo.data"""
		};
		static ListOption metaArtwork = new ListOption("artwork", 'r', @"(\path)")
		{
			Description = @"Set a piece of artwork (jpeg or png only)",
			Help =
@"Set a piece of artwork (jpeg or png) on ""covr.data""
Note:	multiple pieces are allowed with more --artwork args"
		};
		static StringOption metaAdvisory = new StringOption("advisory", 'V', "(1of3)")
		{
			Description = @"Content advisory (values: 'clean', 'explicit')",
			Help = @"Sets the iTunes lyrics advisory ('remove', 'clean', 'explicit')"
		};
		static StringOption metaStik = new StringOption("stik", 'S', "(1of7)")
		{
			Description = @"Sets the iTunes ""stik"" atom (see --" + optLongHelp.Name + @")",
			Help = 
@"Sets the iTunes ""stik"" atom (--stik ""remove"" to delete) ""Movie"", ""Normal"", ""TV Show"" .... others: 
 	see the full list with """ + cmdAtomicParsley + @" --" + optStikList.Name + @"""
 	or set in an integer value with --stik value=(num)
Note:	--stik Audiobook will change file extension to '.m4b'"
		};
		static StringOption metaDescription = new StringOption("description", 'p', "(str)")
		{
			Description = @"Set the description tag",
			Help = @"Sets the description on the ""desc"" atom"
		};
		static StringOption metaRating = new StringOption("Rating", "(str)")
		{
			Help = @"Sets the Rating on the ""rate"" atom"
		};
		static StringOption metaLongDescription = new StringOption("longdesc", 'j', "(str)")
		{
			Description = @"Set the long description tag",
			Help = @"Sets the long description on the ""ldes"" atom"
		};
		static StringOption metaTVNetwork = new StringOption("TVNetwork", 'n', "(str)")
		{
			Description = @"Set the TV Network name",
			Help = @"Sets the TV Network name on the ""tvnn"" atom"
		};
		static StringOption metaTVShowName = new StringOption("TVShowName", 'H', "(str)")
		{
			Description = @"Set the TV Show name",
			Help = @"Sets the TV Show name on the ""tvsh"" atom"
		};
		static StringOption metaTVEpisodeNumber = new StringOption("TVEpisodeNum", 'N', "(num)")
		{
			Description = @"Set the TV Episode number",
			Help = @"Sets the TV Episode number on the ""tves"" atom"
		};
		static StringOption metaTVSeasonNumber = new StringOption("TVSeasonNum", 'U', "(num)")
		{
			Description = @"Set the TV Season number",
			Help = @"Sets the TV Season number on the ""tvsn"" atom"
		};
		static StringOption metaTVEpisode = new StringOption("TVEpisode", 'I', "(str)")
		{
			Description = @"Set the TV episode/production code",
			Help = @"Sets the TV Episode on ""tven"":""209"", but it is a string: ""209 Part 1"""
		};
		static StringOption metaPodcastFlag = new StringOption("podcastFlag", 'f', "(bool)")
		{
			Description = @"Set the podcast flag (true or false)",
			Help = @"Sets the podcast flag (values are ""true"" or ""false"")"
		};
		static StringOption metaCategory = new StringOption("category", 'q', "(str)")
		{
			Description = @"Sets the podcast category",
			Help = @"Sets the podcast category; typically a duplicate of its genre"
		};
		static StringOption metaKeyword = new StringOption("keyword", 'K', "(str)")
		{
			Description = @"Sets the podcast keyword",
			Help = @"Sets the podcast keyword; invisible to Mac OS X Spotlight"
		};
		static StringOption metaPodcastURL = new StringOption("podcastURL", 'L', "(URL)")
		{
			Description = @"Set the podcast feed URL",
			Help = @"Set the podcast feed URL on the ""purl"" atom"
		};
		static StringOption metaPodcastGUID = new StringOption("podcastGUID", 'J', "(URL)")
		{
			Description = @"Set the episode's URL tag",
			Help = @"Set the episode's URL tag on the ""egid"" atom"
		};
		static StringOption metaPurchaseDate = new StringOption("purchaseDate", 'D', "(UTC)")
		{
			Description = @"Set time of purchase",
			Help =
@"Set Universal Coordinated Time of purchase on a ""purd"" atom (use ""timestamp"" to set UTC to now; can be akin to id3v2 TDTG tag)"
		};
		static StringOption metaApID = new StringOption("apID", 'Y', "(str)")
		{
			Description = @"Set the Account Name",
			Help = @"Set the name of the Account Name on the ""apID"" atom"
		};
		static StringOption metaCnID = new StringOption("cnID", "(num)")
		{
			Description = @"Set the iTunes Catalog ID (see --" + optLongHelp.Name + @")",
			Help = @"Set iTunes Catalog ID, used for combining SD and HD encodes in iTunes on the ""cnID"" atom",
			Remarks =@"
To combine you must set ""hdvd"" atom on one file and must have same ""stik"" on both file must not use ""stik"" of value Home Video(0), use Movie(9)

iTunes Catalog numbers can be obtained by finding the item in the iTunes Store.  Once item is found in the iTunes Store right click on picture of " +
@"item and select copy link.  Paste this link into a document or web browser to display the catalog number ID.

An example link for the video Street Kings is:
http://itunes.apple.com/WebObjects/MZStore.woa/wa/viewMovie?id=278743714&s=143441
Here you can see the cnID is 278743714

Alternatively you can use iMDB numbers, however these will not match the iTunes catalog.
"};
		static StringOption metaGeID = new StringOption("geID", "(num)")
		{
			Description = @"Set the iTunes Genre ID (see --" + optLongHelp.Name + @")",
			Help = 
@"Set iTunes Genre ID.  This does not necessarily have to match genre.
See --" + optGenreMovieIDList.Name + @" and --" + optGenreTVIDList.Name + @"
"};
		static StringOption metaXID = new StringOption("xID", "(str)")
		{
			Description = @"Set the vendor-supplied iTunes xID (see --" + optLongHelp.Name + @")",
			Help = @"Set iTunes vendor-supplied xID, used to allow iTunes LPs and iTunes Extras to interact with other content in your iTunes Library"
		};
		static StringOption metaStoreDescription = new StringOption("storedesc", "(str)")
		{
			Description = @"Set the store description tag",
			Help = @"Sets the iTunes store description on the ""sdes"" atom"
		};
		static StringOption metaEncodingTool = new StringOption("encodingTool", "(str)")
		{
			Description = @"Set the name of the encoder",
			Help = @"Set the name of the encoder on the ""©too"" atom"
		};
		static StringOption metaEncodedBy = new StringOption("encodedBy", "(str)")
		{
			Description = @"Set the name of the Person/company who encoded the file",
			Help = @"Set the name of the Person/company who encoded the file on the ""©enc"" atom"
		};
		static StringOption metaPlayGapless = new StringOption("gapless", "(bool)")
		{
			Description = @"Set the gapless playback flag",
			Help = @"Sets the gapless playback flag for a track in a gapless album"
		};
		static StringOption metaSortOrder = new StringOption("sortOrder", "(type) (str)")
		{
			Help =
@"Sets the sort order string for that type of tag. (available types are: ""name"", ""artist"", ""albumartist"", ""album"", ""composer"", ""show"")"
		};

		static StringOption metaReverseDNSForm = new StringOption("rDNSatom", 'M', "(str)  name=(name_str) domain=(reverse_domain)")
		{
			Help =
@"
Manually set a reverseDNS atom."
		};
		static StringOption metaRDNSRating = new StringOption("contentRating", "(rating)")
		{
			Description = @"Set tv/mpaa rating (see --" + optReverseDNSHelp.Name + @")",
			Help =
@"Set the US TV/motion picture media content rating
for available ratings use """ + cmdAtomicParsley + @" --" + optRatingsList.Name + @"""
"
		};

		static StringOption metaStandardDate = new StringOption("tagtime", 'Z', "timestamp", optionalValue: true)
		{
			Help = @"Set the Coordinated Univeral Time of tagging on ""tdtg"""
		};
		static StringOption metaURL = new StringOption("url", 'u', "(URL)")
		{
			Help = @"Set a URL tag on uuid atom name ""©url"""
		};
		static StringOption metaInformation = new StringOption("information", 'i', "(str)")
		{
			Help = @"Set an information tag on uuid atom name""©inf"""
		};
		static StringOption metaUUID = new StringOption("meta-uuid", 'z', "")
		{
			Help = @"There are two forms: 1 for text & 1 for file operations",
			Remarks =
@"setting text form:
--meta-uuid  	(atom) ""text"" (str)  ""atom"" = 4 character atom name of your choice str is whatever text you want to set
file embedding form:
--meta-uuid  	(atom) ""file"" (\path) [description=""foo""] [mime-type=""foo/moof""]
              ""atom"" =	4 character atom name of your choice
              \path =	path to the file that will be embedded
                Note:	a file extension (\path\file.ext) is required
              description =	optional description of the file
               	default is ""[none]""
              mime-type =	optional mime type for the file
               	default is ""none""
                Note:	no auto-disocevery of mime type if you know/want it: supply it."
		};
		static StringOption optExtractUUIDs = new StringOption("extract-uuids", @"[\path]", optionalValue: true)
		{
			Help =
@"Extract all files in uuid atoms under the moov.udta.meta hierarchy. If no \path is given, files will be extracted to the same folder as the " +
@"originating file."
		};
		static StringOption optExtractUUID = new StringOption("extract1uuid", "(atom)")
		{
			Help =
@"Extract file embedded within uuid=atom into same folder
(file will be named with suffix shown in --" + optShowTextData.Name + @")"
		};
		static StringOption optIPodAVCUUID = new StringOption("iPod-uuid", "(num)")
		{
			Help = @"Place the ipod-required uuid for higher resolution avc video files
Currently, the only number used is 1200 - the maximum number of macroblocks allowed by the higher resolution iPod setting.
NOTE:	this requires the ""--" + optDeepScan.Name + @""" option as the 3rd cli argument
NOTE2:	only works on the first avc video track, not all avc tracks"
		};

		static SwitchOption metadataPurge = new SwitchOption("metaEnema", 'P')
		{
			Help = @"Douches away every atom under ""moov.udta.meta.ilst"""
		};
		static SwitchOption userDataPurge = new SwitchOption("udtaEnema", 'X');
		static SwitchOption foobarPurge = new SwitchOption("foobar2000Enema", '2')//. or 2
		{
			Help = @"Eliminates foobar2000's non-compliant so-out-o-spec tagging scheme"
		};
		static SwitchOption metaDump = new SwitchOption("metaDump", 'Q')
		{
			Help = @"Dumps out 'moov.udta' metadata out to a new file next to original
(for diagnostic purposes, please remove artwork before sending)"
		};
		static StringOption manualAtomRemoval = new StringOption("manualAtomRemove", 'R', @"""some.atom.path""")
		{
			Help = @"where some.atom.path can be:",
			Remarks =
@"keys to using manualAtomRemove:
    ilst.ATOM.data or ilst.ATOM	target an iTunes-style metadata tag
    ATOM:lang=foo              	target an atom with this language setting; like 3gp assets
    ATOM.----.name:[foo]       	target a reverseDNS metadata tag; like iTunNORM
                                Note:	these atoms show up with 'AP -t' as: Atom ""----"" [foo]
                                     	'foo' is actually carried on the 'name' atom
    ATOM[x]                    	target an atom with an index other than 1; like trak[2]
    ATOM.uuid=hex-hex-hex-hex  	targt a uuid atom with the uuid of hex string representation
examples:
    moov.udta.meta.ilst.----.name:[iTunNORM]
    moov.trak[3].cprt:lang=urd
    moov.trak[2].uuid=55534d54-21d2-4fce-bb88-695cfac9c740"
		};
		static StringOption optFreeFree = new StringOption("freefree", 'F', "[num]", optionalValue: true)
		{
			Help = @"Remove ""free"" atoms which only act as filler in the file
?(num)? - optional integer argument to delete 'free's to desired level

NOTE 1:	levels begin at level 1 aka file level.
NOTE 2:	Level 0 (which doesn't exist) deletes level 1 atoms that precede 'moov' & don't serve as padding. Typically, such atoms are created by " +
@"libmp4ff or libmp4v2 as a byproduct of tagging.
NOTE 3:	When padding falls below MIN_PAD (typically zero), a default amount of padding (typically 2048 bytes) will be added. To achieve " +
@"absolutely 0 bytes 'free' space with --freefree, set DEFAULT_PAD to 0 via the AP_PADDING mechanism (see below).
"
		};
		static StringOption optOutputFile = new StringOption("output", 'o', @"(\path)")
		{
			Help = @"Specify the filename of tempfile (voids overWrite)"
		};
		static SwitchOption optOverWrite = new SwitchOption("overWrite", 'W')
		{
			Help = @"Writes to temp file; deletes original, renames temp to original
If possible, padding will be used to update without a full rewrite"
		};
		static SwitchOption optNoOptimize = new SwitchOption("preventOptimizing")
		{
			Help = @"Prevents reorganizing the file to have file metadata before media data.
iTunes/Quicktime have so far *always* placed metadata first; many 3rd party utilities do not (preventing streaming to the web, AirTunes, iTV).
Used in conjunction with --" + optOverWrite.Name + @", files with metadata at the end (most ffmpeg produced files) can have their tags rapidly " +
@"updated without requiring a full rewrite.
Note:	this does not un-optimize a file.
Note:	this option will be canceled out if used with the --" + optFreeFree.Name + @" option
"
		};


		static SwitchOption optPreserveTimeStamps = new SwitchOption("preserveTime")
		{
			Help =
@"Will overwrite the original file in place (--" + optOverWrite.Name + @" forced), but will also keep the original file's timestamps intact.
"
		};

		static ListOption isoCopyright = new ListOption("ISO-copyright", "(str)  [movie|track|track=#]  [lang=3str]  [UTF16]")
		{
			Help = @"Set a copyright notice",
			Remarks =
@"# in 'track=#' denotes the target track
3str is the 3 letter ISO-639-2 language.
Brackets [] show optional parameters.
Defaults are: movie level, 'eng' in utf8."
		};

		static ListOption opt3GPTitle = new ListOption("3gp-title", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp media title tag"
		};
		static ListOption opt3GPAuthor = new ListOption("3gp-author", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp author of the media tag"
		};
		static ListOption opt3GPPerformer = new ListOption("3gp-performer", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp performer or artist tag"
		};
		static ListOption opt3GPGenre = new ListOption("3gp-genre", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp genre asset tag"
		};
		static ListOption opt3GPDescription = new ListOption("3gp-description", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp description or caption tag"
		};
		static ListOption opt3GPCopyright = new ListOption("3gp-copyright", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp copyright notice tag",
			Remarks =
@"Note:	The 3gp copyright asset can potentially be altered by using the --" + isoCopyright.Name + @" setting.
"
		};
		static ListOption opt3GPAlbum = new ListOption("3gp-album", "(str)  [lang=3str]  [UTF16]  [trknum=int] [area]")
		{
			Help = @"Set a 3gp album tag (& opt. tracknum)"
		};
		static StringOption opt3GPYear = new StringOption("3gp-year", "(int)  [area]")
		{
			Help =
@"Set a 3gp recording year tag (4 digit only)
"
		};
		static ListOption opt3GPRating = new ListOption("3gp-rating", "(str)  [entity=4str]  [criteria=4str]  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Rating tag"
		};
		static StringOption opt3GPClassification = new StringOption("3gp-classification",
			"(str)  [entity=4str]  [index=int]  [lang=3str]  [UTF16]  [area]")
		{
			Help =
@"Classification
"
		};
		static ListOption opt3GPKeyword = new ListOption("3gp-keyword", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help =
@"Format of str: 'keywords=word1,word2,word3,word4'
"
		};
		static ListOption opt3GPLocation = new ListOption("3gp-location", "(str)  [lang=3str]  [UTF16]  [area]")
		{
			Help = @"Set a 3gp location tag (default: Central Park)",
			Remarks =
@"                   	[longitude=fxd.pt]  [latitude=fxd.pt]  [altitude=fxd.pt] [role=str]  [body=str]  [notes=str]
fxd.pt values are decimal coordinates (55.01209, 179.25W, 63)
'role=' values: 'shooting location', 'real location', 'fictional location'
a negative value in coordinates will be seen as a cli flag
append 'S', 'W' or 'B': lat=55S, long=90.23W, alt=90.25B"
		};

		static ListOption metaID3v2Tag = new ListOption("ID3Tag",
			"(frameID or alias) (str) [desc=(str)] [mimetype=(str)] [imagetype=(str or hex)] [...]")
		{
			Remarks =
@"
... represents other arguments:
[compressed] zlib compress the frame
[UTF16BE, UTF16LE, LATIN1] alternative text encodings for frames that support different encodings
"
		};

		static void ShowVersionInfo(TextWriter writer)
		{
			var assm = typeof(Program).Assembly;
			var ver = (System.Reflection.AssemblyFileVersionAttribute)assm.GetCustomAttributes(
				typeof(System.Reflection.AssemblyFileVersionAttribute), true).FirstOrDefault();
			writer.WriteLine("AtomicParsley version: {0} (.NET)",
				ver != null ? ver.Version : assm.GetName().Version.ToString(3));
		}

		static void WriteOptionSummary(TextWriter writer, string title, params Option[] opts)
		{
			if (!ImplementedOptions.Contains(opts)) return;
			writer.WriteLine(title);
			var list = new OptionCollection();
			foreach(var opt in opts)
				if (ImplementedOptions.Contains(opt))
					list.Add(opt);
			list.WriteOptionSummary(writer);
		}

		static void WriteOptionHelp(TextWriter writer, string title, params Option[] opts)
		{
			if (!ImplementedOptions.Contains(opts)) return;
			writer.WriteLine(title);
			var list = new OptionCollection();
			foreach (var opt in opts)
				if (ImplementedOptions.Contains(opt))
					list.Add(opt);
			list.WriteOptionHelp(writer);
		}

		static void WriteShortHelp(TextWriter writer)
		{
			writer.WriteLine(@"
AtomicParlsey sets metadata into MPEG-4 files & derivatives supporting 3 tag
 schemes: iTunes-style, 3GPP assets & ISO defined copyright notifications.

AtomicParlsey quick help for setting iTunes-style metadata into MPEG-4 files.

General usage examples:");
			if (ImplementedOptions.Contains(optTest1))
				writer.WriteLine(@" 	" + cmdAtomicParsley + @" \path\to.mp4 -" + optTest1.ShortName + @" 1");
			if (ImplementedOptions.Contains(optShowTextDataPlus))
				writer.WriteLine(@" 	" + cmdAtomicParsley + @" \path\to.mp4 -" + optShowTextDataPlus.ShortName + @" +");
			if (ImplementedOptions.Contains(metaArtist, metaArtwork))
				writer.WriteLine(@" 	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaArtist.Name + @" ""Me"" --" + metaArtwork.Name + @" \path\to\art.jpg");
			if (ImplementedOptions.Contains(metaAlbumArtist, metaPodcastFlag))
				writer.WriteLine(@" 	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaAlbumArtist.Name + @" ""You"" --" + metaPodcastFlag.Name + @" true");
			if (ImplementedOptions.Contains(metaStik, metaAdvisory))
				writer.WriteLine(@" 	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaStik.Name + @" ""TV Show"" --" + metaAdvisory.Name + @" explicit");
			if (!ImplementedOptions.Contains(optTest1, optShowTextDataPlus, metaArtist, metaArtwork, metaAlbumArtist, metaPodcastFlag, metaStik, metaAdvisory))
			{
				foreach (var opt in ImplementedOptions.Take(5))
				{
					writer.Write(@" 	" + cmdAtomicParsley);
					opt.WriteDescription(writer);
					writer.WriteLine();
				}
			}
			if (ImplementedOptions.Contains(optTest, optShowTextData, optExtractPix))
			{
				writer.WriteLine(@"
Getting information about the file & tags:");
				if (ImplementedOptions.Contains(optTest))
					writer.WriteLine(@"  -" + optTest.ShortName + @"  --" + optTest.Name + @"       	Test file for mpeg4-ishness & print atom tree");
				if (ImplementedOptions.Contains(optShowTextData))
					writer.WriteLine(@"  -" + optShowTextData.ShortName + @"  --" + optShowTextData.Name + @"   	Prints tags embedded within the file");
				if (ImplementedOptions.Contains(optExtractPix))
					writer.WriteLine(@"  -" + optExtractPix.ShortName + @"  --" + optExtractPix.Name + @" 	Extracts pix to the same folder as the mpeg-4 file");
				writer.WriteLine();
			}

			WriteOptionSummary(writer, @"Setting iTunes-style metadata tags",
				metaArtist, metaSongTitle, metaAlbum, metaGenre, metaTracknum, metaDisknum, metaComment, metaYear,
				metaLyrics, metaLyricsFile, metaComposer, metaCopyright, metaGrouping, metaArtwork, metaBPM,
				metaAlbumArtist, metaCompilation, metaHDVideo, metaAdvisory, metaStik, metaDescription,
				metaLongDescription, metaStoreDescription, metaTVNetwork, metaTVShowName, metaTVEpisode,
				metaTVSeasonNumber, metaTVEpisodeNumber, metaPodcastFlag, metaCategory, metaKeyword, metaPodcastURL,
				metaPodcastGUID, metaPurchaseDate, metaEncodingTool, metaEncodedBy, metaApID, metaCnID, metaGeID,
				metaXID, metaPlayGapless, metaRDNSRating);

			if (ImplementedOptions.Contains(metaArtist, metaStik, metaBPM, metaArtwork, manualAtomRemoval))
			{
				writer.WriteLine(@"
Deleting tags");
				if (ImplementedOptions.Contains(metaArtist, metaStik, metaBPM))
					writer.WriteLine(@"  Set the value to """":       	--" + metaArtist.Name + @" """" --" + metaStik.Name + @" """" --" + metaBPM.Name + @" """"");
				if (ImplementedOptions.Contains(metaArtwork))
					writer.WriteLine(@"  To delete (all) artwork:   	--" + metaArtwork.Name + @" REMOVE_ALL");
				if (ImplementedOptions.Contains(manualAtomRemoval))
					writer.WriteLine(@"  manually removal:          	--" + manualAtomRemoval.Name + @" ""moov.udta.meta.ilst.ATOM""");
				writer.WriteLine();
			}

			writer.WriteLine(@"
More detailed iTunes help is available with " + cmdAtomicParsley + @" --" + optLongHelp.Name + @"");
			if (ImplementedOptions.Contains(optReverseDNSHelp))
				writer.WriteLine(@"Setting reverse DNS forms for iTunes files: see --" + optReverseDNSHelp.Name + @"");
			if (ImplementedOptions.Contains(opt3GPHelp))
				writer.WriteLine(@"Setting 3gp assets into 3GPP & derivative files: see --" + opt3GPHelp.Name + @"");
			if (ImplementedOptions.Contains(isoHelp))
				writer.WriteLine(@"Setting copyright notices for all files: see --" + isoHelp.Name + @"");
			if (ImplementedOptions.Contains(optFileHelp))
				writer.WriteLine(@"For file-level options & padding info: see --" + optFileHelp.Name + @"");
			if (ImplementedOptions.Contains(optUUIDHelp))
				writer.WriteLine(@"Setting custom private tag extensions: see --" + optUUIDHelp.Name + @"");
			if (ImplementedOptions.Contains(optID3Help))
				writer.WriteLine(@"Setting ID3 tags onto mpeg-4 files: see --" + optID3Help.Name + @"");
			writer.WriteLine();
			writer.WriteHorizontalLine();

			ShowVersionInfo(writer);
			writer.WriteLine();
			var log = Logger.GetLogger<Program>();
			log.Debug("Report issues at http://code.google.com/p/atomicparsleynet/");
		}

		static void WriteLongHelp(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParlsey help page for setting iTunes-style metadata into MPEG-4 files.");
			if (ImplementedOptions.Contains(opt3GPHelp))
				writer.WriteCenterLine(@"(3gp help available with " + cmdAtomicParsley + @" --" + opt3GPHelp.Name + @")");
			if (ImplementedOptions.Contains(isoHelp))
				writer.WriteCenterLine(@"(ISO copyright help available with " + cmdAtomicParsley + @" --" + isoHelp.Name + @")");
			if (ImplementedOptions.Contains(optReverseDNSHelp))
				writer.WriteCenterLine(@"(reverse DNS form help available with " + cmdAtomicParsley + @" --" + optReverseDNSHelp.Name + @")");
			writer.WriteLine(@"Usage:	" + cmdAtomicParsley + @" [mp4FILE]... [OPTION]... [ARGUMENT]... [ [OPTION2]...[ARGUMENT2]...]
");
			if (ImplementedOptions.Contains(optExtractPixToPath))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.mp4 -" + optExtractPixToPath.ShortName + @" \Desktop\pix");
			if (ImplementedOptions.Contains(metaPodcastURL, metaTracknum))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaPodcastURL.Name + @" ""http://www.url.net"" --" + metaTracknum.Name + @" 45/356");
			if (ImplementedOptions.Contains(metaCopyright))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaCopyright.Name + @" ""℗ © 2006""");
			if (ImplementedOptions.Contains(metaYear, metaPurchaseDate))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaYear.Name + @" ""2006-07-27T14:00:43Z"" --" + metaPurchaseDate.Name + @" timestamp");
			if (ImplementedOptions.Contains(metaSortOrder))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.mp4 --" + metaSortOrder.Name + @" artist ""Mighty Dub Cats, The""");
			writer.WriteHorizontalLine();

			WriteOptionHelp(writer, @"Extract any pictures in user data ""covr"" atoms to separate files. ",
				optExtractPix, optExtractPixToPath);

			writer.WriteHorizontalLine();

			WriteOptionHelp(writer, @"Tag setting options:",
				metaArtist, metaSongTitle, metaAlbum, metaGenre, metaTracknum, metaDisknum, metaComment, metaYear,
				metaLyrics, metaLyricsFile, metaComposer, metaCopyright, metaGrouping, metaArtwork, metaBPM,
				metaAlbumArtist, metaCompilation, metaHDVideo, metaAdvisory, metaStik, metaDescription, metaRating,
				metaLongDescription, metaStoreDescription, metaTVNetwork, metaTVShowName, metaTVEpisode,
				metaTVSeasonNumber, metaTVEpisodeNumber, metaPodcastFlag, metaCategory, metaKeyword, metaPodcastURL,
				metaPodcastGUID, metaPurchaseDate, metaEncodingTool, metaEncodedBy, metaApID, metaCnID, metaGeID,
				metaXID, metaPlayGapless, metaSortOrder);

			writer.WriteLine(@"
NOTE:	Except for artwork, only 1 of each tag is allowed; artwork allows multiple pieces.
NOTE:	Tags that carry text(str) have a limit of 255 utf8 characters; however lyrics and long descriptions have no limit.");
			writer.WriteHorizontalLine();
			if (ImplementedOptions.ContainsAll(metaArtist, metaLyrics, metaArtwork))
			{
				writer.WriteLine(@" To delete a single atom, set the tag to null (except artwork):
 	--" + metaArtist.Name + @" """" --" + metaLyrics.Name + @" """"
 	--" + metaArtwork.Name + @" REMOVE_ALL ");
			}

			WriteOptionHelp(writer, "", metadataPurge, foobarPurge, manualAtomRemoval);

			writer.WriteHorizontalLine();
		}

		static void WriteFileLevelHelp(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParsley help page for general & file level options.
");
			writer.WriteHorizontalLine();

			WriteOptionHelp(writer, @"Atom reading services:",
				optTest, optTest1, optTestDates, optShowTextData, optShowTextDataPlus, optShowTextData1, optBrands);

			writer.WriteHorizontalLine();

			WriteOptionHelp(writer, @"File services:",
				optFreeFree, optNoOptimize, metaDump, optOutputFile, optOverWrite, optPreserveTimeStamps, optDeepScan,
				optIPodAVCUUID);

			if (ImplementedOptions.Contains(optFreeFree, optOutputFile, optDeepScan, optIPodAVCUUID))
			{
				writer.WriteLine(@"
Examples: ");
				if (ImplementedOptions.Contains(optFreeFree))
					writer.WriteLine(@"  --" + optFreeFree.Name + @" 0        	(deletes all top-level non-padding atoms preceding 'mooov')");
				if (ImplementedOptions.Contains(optFreeFree))
					writer.WriteLine(@"  --" + optFreeFree.Name + @" 1        	(deletes all non-padding atoms at the top most level)");
				if (ImplementedOptions.Contains(optOutputFile))
					writer.WriteLine(@"  --" + optOutputFile.Name + @" \Desktop\newfile.mp4");
				if (ImplementedOptions.Contains(optDeepScan, optIPodAVCUUID))
					writer.WriteLine(@"  " + cmdAtomicParsley + @" \path\to\file.m4v --" + optDeepScan.Name + @" --" + optIPodAVCUUID.Name + @" 1200
");
			}
			writer.WriteHorizontalLine();
			writer.WriteLine(@" Padding & 'free' atoms:

 	A special type of atom called a 'free' atom is used for padding (all 'free' atoms contain NULL space). When changes need to occur, these " +
@"'free' atom are used. They grows or shink, but the relative locations of certain other atoms (stco/mdat) remain the same. If there is no 'free' " +
@"space, a full rewrite will occur. The locations of 'free' atom(s) that AP can use as padding must be follow 'moov.udta' & come before 'mdat'. A " +
@"'free' preceding 'moov' or following 'mdat' won't be used as padding for example. 

 	Set the shell variable AP_PADDING with these values, separated by colons to alter padding behavior:

  DEFAULT_PADDING=  - 	the amount of padding added if the minimum padding is non-existant in the file
                      	default = 2048
  MIN_PAD=          - 	the minimum padding present before more padding will be added
                      	default = 0
  MAX_PAD=          - 	the maximum allowable padding; excess padding will be eliminated
                      	default = 5000

 	If you use --" + optFreeFree.Name + @" to eliminate 'free' atoms from the file, the DEFAULT_PADDING amount will still be added to any newly " +
@"written files. Set DEFAULT_PADDING=0 to prevent any 'free' padding added at rewrite. You can set MIN_PAD to be assured that at least that " +
@"amount of padding will be present - similarly, MAX_PAD limits any excessive amount of padding. All 3 options will in all likelyhood produce a " +
@"full rewrite of the original file. Another case where a full rewrite will occur is when the original file is not optimized and has 'mdat' " +
@"preceding 'moov'.

Examples:
   c:>	SET AP_PADDING=""DEFAULT_PAD=0""
   c:>	SET AP_PADDING=""DEFAULT_PAD=3128""
   c:>	SET AP_PADDING=""DEFAULT_PAD=5128:MIN_PAD=200:MAX_PAD=6049""

Note:	while AtomicParsley is still in the beta stage, the original file will always remain untouched - unless given the --" + optOverWrite.Name +
@" flag when if possible, utilizing available padding to update tags will be tried (falling back to a full rewrite if changes are greater than " +
@"the found padding).");
			writer.WriteHorizontalLine();
			writer.WriteLine(@" iTunes 7 & Gapless playback:

 	iTunes 7 adds NULL space at the ends of files (filled with zeroes). It is possble this is how iTunes implements gapless playback - perhaps " +
@"not. In any event, with AtomicParsley you can choose to preserve that NULL space, or you can eliminate its presence (typically around 2,000 " +
@"bytes). The default behavior is to preserve it - if it is present at all. You can choose to eliminate it by setting the environmental " +
@"preference for AP_PADDING to have DEFAULT_PAD=0

Example:
   c:> SET AP_PADDING=""DEFAULT_PAD=0""");
			writer.WriteHorizontalLine();
		}

		static void Write3GPHelp(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParsley 3gp help page for setting 3GPP-style metadata.");
			writer.WriteHorizontalLine();
			writer.WriteLine(@" 	3GPP text tags can be encoded in either UTF-8 (default input encoding) or UTF-16 (converted from UTF-8). Many " +
@"3GPP text tags can be set for a desired language by a 3-letter-lowercase code (default is ""eng""). For tags that support the language " +
@"attribute (all except year), more than one tag of the same name (3 titles for example) differing in the language code is supported.

 	iTunes-style metadata is not supported by the 3GPP TS 26.244 version 6.4.0 Release 6 specification. 3GPP asset tags can be set at movie level " +
@"or track level & are set in a different hierarchy: moov.udta if at movie level (versus iTunes moov.udta.meta.ilst). Other 3rd party utilities " +
@"may allow setting iTunes-style metadata in 3gp files. When a 3gp file is detected (file extension doesn't matter), only 3gp spec-compliant " +
@"metadata will be read & written.

  Note1:	there are a number of different 'brands' that 3GPP files come marked as. Some will not be supported by AtomicParsley due simply to " +
@"them being unknown and untested.

  Note2:	There are slight accuracy discrepancies in location's fixed point decimals set and retrieved.

  Note3:	QuickTime Player can see a limited subset of these tags, but only in 1 language & there seems to be an issue with not all unicode " +
@"text displaying properly. This is an issue withing QuickTime - the exact same text (in utf8) displays properly in an MPEG-4 file. Some " +
@"languages can also display more glyphs than others.
");
			writer.WriteHorizontalLine();

			WriteOptionHelp(writer, @" 	Tag setting options (default user data area is movie level; default lang is 'eng'; default encoding is UTF8):
 	required arguments are in (parentheses); optional arguments are in [brackets]
",
				opt3GPTitle, opt3GPAuthor, opt3GPPerformer, opt3GPGenre, opt3GPDescription, opt3GPCopyright,
				opt3GPAlbum, opt3GPYear, opt3GPRating, opt3GPClassification, opt3GPKeyword, opt3GPLocation);

			writer.WriteLine(@"
 	[area] can be ""movie"", ""track"" or ""track=num"" where 'num' is the number of the track. If not specificied, assets will be placed at " +
@"movie level. The ""track"" option sets the asset across all available tracks

Note1:	'4str' = a 4 letter string like ""PG13""; 3str is a 3 letter string like ""eng""; int is an integer");
			if (ImplementedOptions.Contains(optLanguageList, optMacLanguageList))
				writer.WriteLine(@"Note2:	List all languages for '3str' with """ + cmdAtomicParsley + @" --" + optLanguageList.Name + @" or " +
@"""" + cmdAtomicParsley + @" --" + optMacLanguageList.Name + @" (unknown languages become ""und"")");
			writer.WriteHorizontalLine();
			writer.WriteLine(@"Usage: AtomicParsley [3gpFILE] --option [argument] [optional_arguments]  [ --option2 [argument2]...] 
");
			if (ImplementedOptions.Contains(optShowTextData))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp -" + optShowTextData.ShortName + @" ");
			if (ImplementedOptions.Contains(optTest1))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp -" + optTest1.ShortName + @" 1 ");
			if (ImplementedOptions.Contains(opt3GPPerformer))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPPerformer.Name + @" ""Enjoy Yourself"" lang=pol UTF16");
			if (ImplementedOptions.Contains(opt3GPYear, opt3GPAlbum))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPYear.Name + @" 2006 --" + opt3GPAlbum.Name +
					@" ""White Label"" track=8 lang=fra");
			if (ImplementedOptions.Contains(opt3GPAlbum))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPAlbum.Name + @" ""Cow Cod Soup For Everyone"" track=10 lang=car");
			writer.WriteLine();
			if (ImplementedOptions.Contains(opt3GPClassification))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPClassification.Name + @" ""Poor Sport"" entity=""PTA "" index=12 UTF16");
			if (ImplementedOptions.Contains(opt3GPKeyword))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPKeyword.Name + @" keywords=""foo1,foo2,foo 3"" UTF16 --" +
					opt3GPKeyword.Name + @" """"");
			if (ImplementedOptions.Contains(opt3GPLocation))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPLocation.Name + @" 'Bethesda Terrace' latitude=40.77 " +
					@"longitude=73.98W altitude=4.3B role='real' body=Earth notes='Underground'");
			writer.WriteLine();
			if (ImplementedOptions.Contains(opt3GPTitle))
				writer.WriteLine(@"example:	" + cmdAtomicParsley + @" \path\to.3gp --" + opt3GPTitle.Name + @" ""I see London."" --" + opt3GPTitle.Name +
					@" ""Veo Madrid."" lang=spa --" + opt3GPTitle.Name + @" ""Widze Warsawa."" lang=pol
");
		}

		static void WriteISOHelp(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParsley help page for setting ISO copyright notices at movie & track level.");
			writer.WriteHorizontalLine();
			if (ImplementedOptions.Contains(metaCopyright, isoCopyright))
				writer.WriteLine(@" 	The ISO specification allows for setting copyright in a number of places. This copyright atom is independant " +
@"of the iTunes-style --" + metaCopyright.Name + @" tag that can be set. This ISO tag is identical to the 3GP-style copyright. In fact, using --" +
isoCopyright.Name + @" can potentially overwrite the 3gp copyright asset if set at movie level & given the same language to set the copyright on. " +
@"This copyright notice is the only metadata tag defined by the reference ISO 14496-12 specification.
");
			if (ImplementedOptions.Contains(isoCopyright))
			{
				if (ImplementedOptions.Contains(optLanguageList, optMacLanguageList))
					writer.WriteLine(@" 	ISO copyright notices can be set at movie level, track level for a single track, or for all tracks. " +
	@"Multiple copyright notices are allowed, but they must differ in the language setting. To see available languages use """ + cmdAtomicParsley +
	@" --" + optLanguageList.Name + @""" or """ + cmdAtomicParsley + @" --" + optMacLanguageList.Name + @""". Notices can be set in utf8 or utf16.
");
				WriteOptionHelp(writer, "", isoCopyright);

				writer.WriteLine(@"
example:	" + cmdAtomicParsley + @" \path\file.mp4 -" + optShowTextData1.ShortName + @" 1
    Note:	the only way to see all contents is with -" + optShowTextData1.ShortName + @" 1 
example:	" + cmdAtomicParsley + @" \path\file.mp4 --" + isoCopyright.Name + @" ""Sample""
example:	" + cmdAtomicParsley + @" \path\file.mp4 --" + isoCopyright.Name + @" ""Sample"" movie
example:	" + cmdAtomicParsley + @" \path\file.mp4 --" + isoCopyright.Name + @" ""Sample"" track=2 lang=urd
example:	" + cmdAtomicParsley + @" \path\file.mp4 --" + isoCopyright.Name + @" ""Sample"" track UTF16
example:	" + cmdAtomicParsley + @" --" + isoCopyright.Name + @" ""Example"" track --" + isoCopyright.Name + @" ""Por Exemplo"" track=2 lang=spa UTF16

Note:	to remove the copyright, set the string to """" - the track and language must match the target.
example:	--" + isoCopyright.Name + @" """" track --" + isoCopyright.Name + @" """" track=2 lang=spa

Note:	(foo) denotes required arguments; [foo] denotes optional parameters & may have defaults.
");
			}
		}

		static void WriteUUIDHelp(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParsley help page for setting uuid user extension metadata tags.");
			writer.WriteHorizontalLine();
			writer.WriteLine(@" 	Setting a user-defined 'uuid' private extention tags will appear in ""moov.udta.meta"". These will only be " +
@"read by AtomicParsley & can be set irrespective of file branding. The form of uuid that AP is a v5 uuid generated from a sha1 hash of an atom " +
@"name in an 'AtomicParsley.sf.net' namespace.

 	The uuid form is in some Sony & Compressor files, but of version 4 (random/pseudo-random). An example uuid of 'cprt' in the " +
@"'AtomicParsley.sf.net' namespace is: ""4bd39a57-e2c8-5655-a4fb-7a19620ef151"". 'cprt' in the same namespace will always create that uuid; uuid " +
@"atoms will only print out if the uuid generated is the same as discovered. ");
			if (ImplementedOptions.Contains(optShowTextData))
				writer.WriteLine(@"Sony uuids don't for example show up with " + cmdAtomicParsley + @" -" + optShowTextData.ShortName + @".");
			writer.WriteLine();

			WriteOptionHelp(writer, "", metaInformation, metaURL, metaStandardDate);

			writer.WriteLine();

			WriteOptionHelp(writer, @"Define & set an arbitrary atom with a text data or embed a file:",
				metaUUID);

			writer.WriteLine(@"
Note:	(foo) denotes required arguments; [foo] denotes optional arguments & may have defaults.

Examples: ");
			if (ImplementedOptions.Contains(metaStandardDate, metaInformation, metaURL))
				writer.WriteLine(@" 	--" + metaStandardDate.Name + @" timestamp --" + metaInformation.Name + @" ""[psst]I see metadata"" --" + metaURL.Name + @" http://www.bumperdumper.com");
			if (ImplementedOptions.Contains(metaUUID))
			{
				writer.WriteLine(
@" 	--" + metaUUID.Name + @" tagr text ""Johnny Appleseed"" --" + metaUUID.Name + @" ©sft text ""OpenShiiva encoded.""
 	--" + metaUUID.Name + @" scan file \usr\pix\scans.zip
 	--" + metaUUID.Name + @" 1040 file ..\..\2006_taxes.pdf description=""Fooled 'The Man' yet again.""");
			}
			if (ImplementedOptions.Contains(metaStandardDate, metaInformation, metaURL, metaUUID, manualAtomRemoval))
			{
				writer.WriteLine(@"can be removed with:");
				if (ImplementedOptions.Contains(metaStandardDate, metaInformation, metaURL, metaUUID))
					writer.WriteLine(@" 	--" + metaStandardDate.Name + @" """" --" + metaInformation.Name + @" """"  --" + metaURL.Name + @" "" ""  --" + metaUUID.Name + @" scan file """"");
				if (ImplementedOptions.Contains(manualAtomRemoval))
					writer.WriteLine(
@" 	--" + manualAtomRemoval.Name + @" moov.udta.meta.uuid=672c98cd-f11f-51fd-adec-b0ee7b4d215f \
 	--" + manualAtomRemoval.Name + @" moov.udta.meta.uuid=1fed6656-d911-5385-9cb2-cb2c100f06e7");
			}
			if (ImplementedOptions.Contains(manualAtomRemoval))
			{
				writer.WriteLine(@"Remove the Sony uuid atoms with:
 	--" + manualAtomRemoval.Name + @" moov.trak[1].uuid=55534d54-21d2-4fce-bb88-695cfac9c740 \
 	--" + manualAtomRemoval.Name + @" moov.trak[2].uuid=55534d54-21d2-4fce-bb88-695cfac9c740 \
 	--" + manualAtomRemoval.Name + @" uuid=50524f46-21d2-4fce-bb88-695cfac9c740");
			}
			if (ImplementedOptions.Contains(optShowTextData))
				writer.WriteLine(@"
Viewing the contents of uuid atoms:
 	-" + optShowTextData.ShortName + @" or --" + optShowTextData.Name + @"textdata
 	Shows the uuid atoms (both text & file) that AP sets; Example output:
   	Atom uuid=ec0f...d7 (AP uuid for ""scan"") contains: FILE.zip; description=[none]
   	Atom uuid=672c...5f (AP uuid for ""tagr"") contains: Johnny Appleseed
");

			if (ImplementedOptions.Contains(optExtractUUID, optExtractUUIDs))
			{
				WriteOptionHelp(writer, @"Extracting an embedded file in a uuid atom:",
					optExtractUUID, optExtractUUIDs);

				writer.WriteLine(@"
Examples:");
				if (ImplementedOptions.Contains(optExtractUUID))
					writer.WriteLine(@" 	--" + optExtractUUID.Name + @" scan
  ... 	Extracted uuid=scan attachment to file: \some\path\FILE_scan_uuid.zip");
				if (ImplementedOptions.Contains(optExtractUUIDs))
					writer.WriteLine(@" 	--" + optExtractUUIDs.Name + @" \Desktop\plops
  ... 	Extracted uuid=pass attachment to file: \Users\me\Desktop\plops_pass_uuid.pdf
  ... 	Extracted uuid=site attachment to file: \Users\me\Desktop\plops_site_uuid.html");
				writer.WriteLine();
			}
			writer.WriteHorizontalLine();
		}

		static void WriteRDNSHelp(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParsley help page for setting reverse domain '----' metadata atoms.");
			writer.WriteHorizontalLine();
			writer.WriteCenterLine(@"Please note that the reverse DNS format supported here is not feature complete.");
			writer.WriteLine(@"
 	Another style of metadata that iTunes uses is called the reverse DNS format. For all known tags, iTunes offers no user-accessible exposure to " +
@"these tags or their contents. This reverse DNS form has a differnt form than other iTunes tags that have a atom name that describes the content " +
@"of 'data' atom it contains. In the reverseDNS format, the parent to the structure called the '----' atom, with children atoms that describe & " +
@"contain the metadata carried. The 'mean' child contains the reverse domain itself ('com.apple.iTunes') & the 'name' child contains the " +
@"descriptor ('iTunNORM'). A 'data' atom follows that actually contains the contents of the tag.
");

			WriteOptionHelp(writer, "", metaRDNSRating, metaReverseDNSForm);

			writer.WriteLine(@"
 	To set the form manually, 3 things are required: a domain, a name, and the desired text.
  Note:	multiple 'data' atoms are supported, but not in the com.apple.iTunes domain
Examples:");
			if (ImplementedOptions.Contains(metaRDNSRating))
				writer.WriteLine(@" 	--" + metaRDNSRating.Name + @" ""NC-17"" --" + metaRDNSRating.Name + @" ""TV-Y7""");
			if (ImplementedOptions.Contains(metaReverseDNSForm))
				writer.WriteLine(@" 	--" + metaReverseDNSForm.Name + @" ""mpaa|PG-13|300|"" name=iTunEXTC domain=com.apple.iTunes");
			if (ImplementedOptions.Contains(metaRDNSRating))
				writer.WriteLine(@" 	--" + metaRDNSRating.Name + @" """"");
			if (ImplementedOptions.Contains(metaReverseDNSForm))
				writer.WriteLine(
@" 	--" + metaReverseDNSForm.Name + @" """" name=iTunEXTC domain=com.apple.iTunes
 	--" + metaReverseDNSForm.Name + @" ""try1"" name=EVAL domain=org.fsf --" + metaReverseDNSForm.Name + @" ""try 2"" name=EVAL domain=org.fsf
 	--" + metaReverseDNSForm.Name + @" """" name=EVAL domain=org.fsf");
			writer.WriteHorizontalLine();
		}

		static void WriteID3Help(TextWriter writer)
		{
			writer.WriteLine(@"AtomicParsley help page for ID32 atoms with ID3 tags.");
			writer.WriteHorizontalLine();
			writer.WriteCenterLine(@"Please note that ID3 tag support is not feature complete & is in an alpha state.");
			writer.WriteHorizontalLine();
			writer.WriteLine(@" 	ID3 tags are the tagging scheme used by mp3 files (where they are found typically at the start of the file). " +
@"In mpeg-4 files, ID3 version 2 tags are located in specific hierarchies at certain levels, at file/movie/track level. The way that ID3 tags are " +
@"carried on mpeg-4 files (carried by 'ID32' atoms) was added in early 2006, but the ID3 tagging 'informal standard' was last updated (to v2.4) in 2000.
 	With few exceptions, ID3 tags in mpeg-4 files exist identically to their mp3 counterparts.

 	The ID3 parlance, a frame contains an piece of metadata. A frame (like COMM for comment, or TIT1 for title) contains the information, while " +
@"the tag contains all the frames collectively. The 'informal standard' for ID3 allows multiple langauges for frames like COMM (comment) & " +
@"USLT (lyrics). In mpeg-4 this language setting is removed from the ID3 domain and exists in the mpeg-4 domain. That means that when an english " +
@"and a spanish comment are set, 2 separate ID32 atoms are created, each with a tag & 1 frame as in this example:
 	--" + metaID3v2Tag.Name + @" COMM ""Primary"" --desc=AAA --" + metaID3v2Tag.Name + @" COMM ""El Segundo"" UTF16LE lang=spa --desc=AAA");
			if (ImplementedOptions.Contains(optID3FramesList))
				writer.WriteLine(@" 	See available frames with """ + cmdAtomicParsley + @" --" + optID3FramesList.Name + @"""");
			if (ImplementedOptions.Contains(optImageTypeList))
				writer.WriteLine(@" 	See avilable imagetypes with """ + cmdAtomicParsley + @" --" + optImageTypeList.Name + @"""");
			writer.WriteLine(@"
 	AtomicParsley writes ID3 version 2.4.0 tags *only*. There is no up-converting from older versions.
Defaults are:
  -	default to movie level (moov.meta.ID32); other options are [ ""root"", ""track=(num)"" ] (see WARNING)
  -	UTF-8 text encoding when optional; other options are [ ""LATIN1"", ""UTF16BE"", ""UTF16LE"" ]
  -	frames that require descriptions have a default of """"
  -	for frames requiring a language setting, the ID32 language is used (currently defaulting to 'eng')
  -	frames that require descriptions have a default of """"
  -	image type defaults to 0x00 or Other; for image type 0x01, 32x32 png is enforced (switching to 0x02)
  -	setting the image mimetype is generally not required as the file is tested, but can be overridden
  -	zlib compression off

WARNING:
 	Quicktime Player (up to v7.1.3 at least) will freeze opeing a file with ID32 tags at movie level. Specifically, the parent atom, 'meta' is " +
@"the source of the issue. You can set the tags at file or track level which avoids the problem, but the default is movie level. iTunes is unaffected.");
			writer.WriteHorizontalLine();
			writer.WriteLine(@"Current limitations:
  -	syncsafe integers are used as indicated by the id3 ""informal standard"". usage/reading of nonstandard ordinary unsigned integers (uint32_t) " +
@"is not/will not be implemented.
  -	externally referenced images (using mimetype '-->') are prohibited by the ID32 specification.
  -	the ID32 atom is only supported in a non-referenced context
  -	probably a raft of other limitations that my brain lost along the way...");
			writer.WriteHorizontalLine();

			WriteOptionHelp(writer, @"Usage:",
				metaID3v2Tag);

			writer.WriteLine(@"Note:	(foo) denotes required arguments; [foo] denotes optional parameters

Examples:
 	--" + metaID3v2Tag.Name + @" APIC \path\to\img.ext
 	--" + metaID3v2Tag.Name + @" APIC \path\to\img.ext desc=""something to say"" imagetype=0x08 UTF16LE compressed
 	--" + metaID3v2Tag.Name + @" composer ""I, Claudius"" --" + metaID3v2Tag.Name + @" TPUB ""Seneca the Roman"" --" + metaID3v2Tag.Name + @" TMOO Imperial
 	--" + metaID3v2Tag.Name + @" UFID look@me.org uniqueID=randomUUIDstamp

Extracting embedded images in APIC frames:
 	--" + metaID3v2Tag.Name + @" APIC extract
      	images are extracted into the same directory as the source mpeg-4 file

Setting MCDI (Music CD Identifier):
 	--" + metaID3v2Tag.Name + @" MCDI D
      	Information to create this frame is taken directly off an Audio CD's TOC. The letter after ""MCDI"" is the letter of the drive where the CD is present.
");
		}
	}
}
