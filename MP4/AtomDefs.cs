//==================================================================//
/*
    AtomicParsley - AtomDefs.h

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

namespace MP4
{
	public static class Definitions
	{
		public static readonly AtomDefinition[] KnownAtoms = new AtomDefinition[]
		{
			                                                        //name     parent atom    container                  number                                box_type
			new AtomDefinition<ISOMediaBoxes.FileTypeBox            >("ftyp",  "",      AtomState.ChildAtom,       AtomRequirements.RequiredOnce,        BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.JPEG2000Atom           >("jP  ",  "",      AtomState.ChildAtom,       AtomRequirements.RequiredOnce,        BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.MovieBox               >("moov",  "",      AtomState.ParentAtom,      AtomRequirements.RequiredOnce,        BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.MediaDataBox           >("mdat",  "",      AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.ProgressiveDownloadBox >("pdin",  "",      AtomState.ChildAtom,       AtomRequirements.OptionalOnce,        BoxType.VersionedAtom ),
			
			new AtomDefinition<ISOMediaBoxes.MovieFragmentBox       >("moof",  "",      AtomState.ParentAtom,      AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.MovieFragmentHeaderBox >("mfhd",  "moof",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackFragmentBox       >("traf",  "moof",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackFragmentHeaderBox >("tfhd",  "traf",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackFragmentRunBox    >("trun",  "traf",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			new AtomDefinition                                       ("mfra",  "",      AtomState.ParentAtom,      AtomRequirements.OptionalOnce,        BoxType.SimpleAtom ),
			new AtomDefinition                                       ("tfra",  "mfra",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("mfro",  "mfra",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.FreeSpaceBox           >("free",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.FreeSpaceBox           >("skip",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.UUIDBox                >("uuid",           AtomState.ChildAtom,       AtomRequirements.RequiredOnce,        BoxType.ExtendedAtom ),

			new AtomDefinition<ISOMediaBoxes.MovieHeaderBox>         ("mvhd",  "moov",  AtomState.ChildAtom,       AtomRequirements.RequiredOnce,        BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.ObjectDescriptorBox    >("iods",  "moov",  AtomState.ChildAtom,       AtomRequirements.OptionalOnce,        BoxType.VersionedAtom ),
			new AtomDefinition                                       ("drm ",  "moov",  AtomState.ChildAtom,       AtomRequirements.OptionalOnce,        BoxType.VersionedAtom ),     // 3gp/MobileMP4
			new AtomDefinition<ISOMediaBoxes.TrackBox               >("trak",  "moov",  AtomState.ParentAtom,      AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.TrackHeaderBox         >("tkhd",  "trak",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceBox      >("tref",  "trak",  AtomState.ParentAtom,      AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.MediaBox               >("mdia",  "trak",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),

			new AtomDefinition                                       ("tapt",  "trak",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("clef",  "tapt",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("prof",  "tapt",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("enof",  "tapt",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.MediaHeaderBox         >("mdhd",  "mdia",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MediaInformationBox    >("minf",  "mdia",  AtomState.ParentAtom,      AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.HandlerBox             >("hdlr",  "mdia",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),     //minf parent present in chapterized
			new AtomDefinition<ISOMediaBoxes.HandlerBox             >("hdlr",  "meta",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),     //minf parent present in chapterized
			new AtomDefinition<ISOMediaBoxes.HandlerBox             >("hdlr",  "minf",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),     //minf parent present in chapterized

			new AtomDefinition<ISOMediaBoxes.VideoMediaHeaderBox    >("vmhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SoundMediaHeaderBox    >("smhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.HintMediaHeaderBox     >("hmhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGMediaHeaderBox     >("odhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGMediaHeaderBox     >("crhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGMediaHeaderBox     >("sdhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGMediaHeaderBox     >("nmhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition                                       ("gmhd",  "minf",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),     //present in chapterized

			new AtomDefinition<ISOMediaBoxes.DataInformationBox     >("dinf",  "minf",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //required in minf
			new AtomDefinition<ISOMediaBoxes.DataInformationBox     >("dinf",  "meta",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //required in minf

			new AtomDefinition<ISOMediaBoxes.DataReferenceBox       >("dref",  "dinf",  AtomState.DualStateAtom,   AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.DataEntryURLBox        >("url ",  "dref",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.DataEntryURNBox        >("urn ",  "dref",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition                                       ("alis",  "dref",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition                                       ("cios",  "dref",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.SampleTableBox         >("stbl",  "minf",  AtomState.ParentAtom,      AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TimeToSampleBox        >("stts",  "stbl",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.CompositionOffsetBox   >("ctts",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SampleDescriptionBox   >("stsd",  "stbl",  AtomState.DualStateAtom,   AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.SampleSizeBox          >("stsz",  "stbl",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SampleSizeBox          >("stz2",  "stbl",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.SampleToChunkBox       >("stsc",  "stbl",  AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.ChunkOffsetBox         >("stco",  "stbl",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.ChunkLargeOffsetBox    >("co64",  "stbl",  AtomState.ChildAtom,       AtomRequirements.ReqFamilialOne,      BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.SyncSampleBox          >("stss",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.ShadowSyncBox          >("stsh",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.DegradationPriorityBox >("stdp",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.PaddingBitsBox         >("padb",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SampleDependencyTypeBox>("sdtp",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("sdtp",  "traf",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SampleGroupBox         >("sbgp",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SampleGroupBox         >("sbgp",  "traf",  AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition                                       ("stps",  "stbl",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.EditBox                >("edts",  "trak",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.EditListBox            >("elst",  "edts",  AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.UserDataBox            >("udta",  "moov",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.UserDataBox            >("udta",  "trak",  AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.MetaBox                >("meta",  "",      AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),   //optionally contains info
			new AtomDefinition<ISOMediaBoxes.MetaBox                >("meta",  "moov",  AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),   //optionally contains info
			new AtomDefinition<ISOMediaBoxes.MetaBox                >("meta",  "trak",  AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),   //optionally contains info
			new AtomDefinition<ISOMediaBoxes.MetaBox                >("meta",  "udta",  AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),   //optionally contains info

			new AtomDefinition<ISOMediaBoxes.MovieExtendsBox        >("mvex",  "moov",           AtomState.ParentAtom,      AtomRequirements.OptionalOnce,        BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.MovieExtendsHeaderBox  >("mehd",  "mvex",           AtomState.ChildAtom,       AtomRequirements.OptionalOnce,        BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackExtendsBox        >("trex",  "mvex",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			                                                        //"stsl",  "????",           CHILD_ATOM,                OPTIONAL_ONE,                         VERSIONED_ATOM ,             //contained by a sample entry box
			new AtomDefinition<ISOMediaBoxes.SubSampleInformationBox>("subs",  "stbl",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.SubSampleInformationBox>("subs",  "traf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition                                       ("xml ",  "meta",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("bxml",  "meta",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("iloc",  "meta",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("pitm",  "meta",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("ipro",  "meta",           AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("iinf",  "meta",           AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("infe",  "iinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition                                       ("sinf",  "ipro",           AtomState.ParentAtom,      AtomRequirements.RequiredOne,        BoxType.SimpleAtom ),        //parent atom is also "Protected Sample Entry"
			new AtomDefinition                                       ("sinf",  "drms",           AtomState.ParentAtom,      AtomRequirements.RequiredOne,        BoxType.SimpleAtom ),        //parent atom is also "Protected Sample Entry"
			new AtomDefinition                                       ("sinf",  "drmi",           AtomState.ParentAtom,      AtomRequirements.RequiredOne,        BoxType.SimpleAtom ),        //parent atom is also "Protected Sample Entry"
			new AtomDefinition                                       ("frma",  "sinf",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("imif",  "sinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("schm",  "sinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("schm",  "srpp",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("schi",  "sinf",           AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("schi",  "srpp",           AtomState.DualStateAtom,   AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("skcr",  "sinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition                                       ("user",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("key ",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),     //could be required in 'drms'/'drmi'
			new AtomDefinition                                       ("iviv",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("righ",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.NameBox                >("name",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("priv",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),

			new AtomDefinition                                       ("iKMS",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),     // 'iAEC', '264b', 'iOMA', 'ICSD'
			new AtomDefinition                                       ("iSFM",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("iSLT",  "schi",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //boxes with 'k***' are also here; reserved
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("IKEY",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("hint",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("dpnd",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("ipir",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("mpod",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("sync",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TrackReferenceTypeBox  >("chap",  "tref",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //?possible versioned?

			new AtomDefinition                                       ("ipmc",  "moov",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("ipmc",  "meta",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.TSHintEntryBox         >("tims",  "rtp ",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TimeOffHintEntryBox    >("tsro",  "rtp ",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.SeqOffHintEntryBox     >("snro",  "rtp ",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),

			new AtomDefinition                                       ("srpp",  "srtp",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),

			new AtomDefinition<ISOMediaBoxes.HintTrackInfoBox       >("hnti",  "udta",           AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.RTPBox                 >("rtp ",  "hnti",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //'rtp ' is defined twice in different containers
			new AtomDefinition<ISOMediaBoxes.SDPBox                 >("sdp ",  "hnti",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),

			new AtomDefinition<ISOMediaBoxes.HintInfoBox            >("hinf",  "udta",           AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.NameBox                >("name",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TRPYBox                >("trpy",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.NUMPBox                >("nump",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.NTYLBox                >("tpyl",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TOTLBox                >("totl",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.NPCKBox                >("npck",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.MAXRBox                >("maxr",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.DMEDBox                >("dmed",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.DIMMBox                >("dimm",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.DREPBox                >("drep",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TMINBox                >("tmin",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TMAXBox                >("tmax",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.PMAXBox                >("pmax",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.DMAXBox                >("dmax",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.PAYTBox                >("payt",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.TPAYBox                >("tpay",  "hinf",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),

			new AtomDefinition                                       ("drms",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("drmi",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("alac",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGAudioSampleEntryBox>("mp4a",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGSampleEntryBox     >("mp4s",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGVisualSampleEntryBox>("mp4v", "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGVisualSampleEntryBox>("avc1", "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGVisualSampleEntryBox>("avc2", "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGVisualSampleEntryBox>("svc1", "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("avcp",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("text",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("jpeg",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("tx3g",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.HintSampleEntryBox     >("rtp ",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),     //"rtp " occurs twice; disparate meanings
			new AtomDefinition                                       ("srtp",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.SimpleAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGAudioSampleEntryBox>("enca",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGVisualSampleEntryBox>("encv", "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("enct",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition<ISOMediaBoxes.MPEGSampleEntryBox     >("encs",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("samr",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("sawb",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("sawp",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("s263",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("sevc",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("sqcp",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("ssmv",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("tmcd",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),
			new AtomDefinition                                       ("mjp2",  "stsd",           AtomState.DualStateAtom,  AtomRequirements.ReqFamilialOne,       BoxType.VersionedAtom ),     //mjpeg2000

			new AtomDefinition                                       ("alac",  "alac",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("avcC",  "avc1",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("avcC",  "drmi",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("damr",  "samr",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("damr",  "sawb",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("d263",  "s263",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("dawp",  "sawp",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("devc",  "sevc",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("dqcp",  "sqcp",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("dsmv",  "ssmv",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("bitr",  "d263",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("btrt",  "avc1",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //found in NeroAVC
			new AtomDefinition                                       ("m4ds",  "avc1",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //?possible versioned?
			new AtomDefinition                                       ("ftab",  "tx3g",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),
			new AtomDefinition                                       ("jp2h",  "mjp2",           AtomState.ParentAtom,      AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //mjpeg2000

			new AtomDefinition                                       ("ihdr",  "jp2h",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //mjpeg2000
			new AtomDefinition                                       ("colr",  "jp2h",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),     //mjpeg2000
			new AtomDefinition                                       ("fiel",  "mjp2",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //mjpeg2000
			new AtomDefinition                                       ("jp2p",  "mjp2",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.VersionedAtom ),     //mjpeg2000
			new AtomDefinition                                       ("jsub",  "mjp2",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //mjpeg2000
			new AtomDefinition                                       ("orfo",  "mjp2",           AtomState.ChildAtom,       AtomRequirements.OptionalOne,         BoxType.SimpleAtom ),        //mjpeg2000

			new AtomDefinition<ISOMediaBoxes.CopyrightBox           >("cprt",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),    //the only ISO defined metadata tag; also a 3gp asset
			new AtomDefinition                                       ("titl",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),    //3gp assets
			new AtomDefinition                                       ("auth",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("perf",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("gnre",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("dscp",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("albm",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("yrrc",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.VersionedAtom ),
			new AtomDefinition                                       ("rtng",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("clsf",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("kywd",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),
			new AtomDefinition                                       ("loci",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),

			new AtomDefinition                                       ("ID32",  "meta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.PackedLangAtom ),    //id3v2 tag
			new AtomDefinition                                       ("tsel",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),        //but only at track level in a 3gp file

			new AtomDefinition<ISOMediaBoxes.ChapterListBox         >("chpl",  "udta",           AtomState.ChildAtom,       AtomRequirements.OptionalOnce,        BoxType.VersionedAtom ),     //Nero - seems to be versioned
			                                                        //"ndrm",  "udta",           CHILD_ATOM,                OPTIONAL_ONCE,                        VERSIONED_ATOM ,             //Nero - seems to be versioned
			                                                        //"tags",  "udta",           CHILD_ATOM,                OPTIONAL_ONCE,                        SIMPLE_ATOM ,                //Another Nero-CreationЄ
			                                                                                                                                            // ...so if they claim that "tags doesn't have any children",
			                                                                                                                                            // why does nerotags.exe say "tshd atom"? If 'tags' doesn't
			                                                                                                                                            // have any children, then tshd can't be an atom....
			                                                                                                                                            // Clearly, they are EternallyRightЄ and everyone else is
			                                                                                                                                            // always wrong.

			                                                                                                                                            //Pish! Seems that Nero is simply unable to register any atoms.

			new AtomDefinition                                       ("ilst",  "meta",           AtomState.ParentAtom,      AtomRequirements.OptionalOnce,        BoxType.SimpleAtom ),        //iTunes metadata container
			new AtomDefinition                                       ("----",  "ilst",           AtomState.ParentAtom,      AtomRequirements.OptionalMany,        BoxType.SimpleAtom ),        //reverse dns metadata
			new AtomDefinition                                       ("mean",  "----",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom ),
			new AtomDefinition                                       ("name",  "----",           AtomState.ChildAtom,       AtomRequirements.RequiredOne,         BoxType.VersionedAtom )
			//gnrm - GenericSampleEntryBox
			//gnrv - GenericVisualSampleEntryBox
			//gnra - GenericAudioSampleEntryBox
			//rely - RelyHintBox
			//rtpo - RTPOBox
			//sgpd - SampleGroupDescriptionBox
			//stsf - SampleFragmentBox
			//void - VoidBox
			//pasp - PixelAspectRatioBox
			//metx, mett - MetaDataSampleEntryBox
			//dac3 - AC3ConfigBox
			//ac-3 - AC3SampleEntryBox
			//lsrc - LASERConfigurationBox
			//lsr1 - LASeRSampleEntryBox
			//sidx - SegmentIndexBox
			//pcrb - PcrInfoBox
			//tfdt - TFBaseMediaDecodeTimeBox
			//rvcc - RVCConfigurationBox
		};
		public static readonly AtomDefinition UnknownAtom = new AtomDefinition<ISOMediaBoxes.UnknownBox>(null, AtomState.ChildAtom, AtomRequirements.OptionalMany, BoxType.SimpleAtom); //multiple parents; keep 2nd from end; manual return
		public static readonly AtomDefinition DataReferenceAtom = new AtomDefinition(null, "dref", AtomState.ChildAtom, AtomRequirements.OptionalMany, BoxType.VersionedAtom); //support any future named child to dref; keep 4th from end; manual return
		public static readonly AtomDefinition ElementaryStreamDescriptionAtom = new AtomDefinition<ISOMediaBoxes.ESDBox>("esds", AtomState.ChildAtom, AtomRequirements.RequiredOne, BoxType.SimpleAtom); //multiple parents; keep 3rd from end; manual return
		public static readonly AtomDefinition ListAtom = new AtomDefinition(null, "ilst", AtomState.ParentAtom, AtomRequirements.OptionalOne, BoxType.SimpleAtom); //multiple parents; keep 2nd from end; manual return
		public static readonly AtomDefinition iTunesAtom = new AtomDefinition("data", AtomState.ChildAtom, AtomRequirements.ParentSpecific, BoxType.VersionedAtom); //multiple parents
	}
}
