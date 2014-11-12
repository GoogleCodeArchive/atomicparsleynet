//==================================================================//
/*
    AtomicParsley - parsley.cpp

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
    * Lowell Stewart - null-termination bugfix for Apple compliance
    * Brian Story - native Win32 patches; memset/framing/leaks fixes
    ----------------------
    SVN revision information:
      $Revision$
                                                                   */
//==================================================================//
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using FRAFV.Binary.Serialization;

namespace MP4
{
	/// <summary>
	/// A movie container
	/// </summary>
	[XmlRoot("IsoMediaFile", Namespace = XMLNS)]
	public sealed class Container
	{
		private static readonly ILog log = Logger.GetLogger(typeof(Container));
		internal const string XMLNS = "urn:schemas-mp4ra-org:container";

		private MetadataStyle metadataStyle;
		private bool editable;

		[XmlIgnore]
		public ICollection<AtomicInfo> Atoms { get; private set; }

		[XmlElement(typeof(ISOMediaBoxes.FileTypeBox))]
		[XmlElement(typeof(ISOMediaBoxes.JPEG2000Atom))]
		[XmlElement(typeof(ISOMediaBoxes.MovieBox))]
		[XmlElement(typeof(ISOMediaBoxes.MediaDataBox))]
		[XmlElement(typeof(ISOMediaBoxes.ProgressiveDownloadBox))]
		[XmlElement(typeof(ISOMediaBoxes.MovieFragmentBox))]
		//[XmlElement(typeof(mfra))]
		[XmlElement(typeof(ISOMediaBoxes.MetaBox))]
		[XmlElement(typeof(ISOMediaBoxes.FreeSpaceBox))]
		[XmlElement(typeof(ISOMediaBoxes.UUIDBox))]
		[XmlElement(typeof(ISOMediaBoxes.ESDBox))]
		//[XmlElement(typeof(data))]
		[XmlElement(typeof(ISOMediaBoxes.UnknownBox))]
		[XmlElement(typeof(ISOMediaBoxes.UnknownParentBox))]
		public AtomicInfo[] AtomsSerializer
		{
			get
			{
				int k = 0;
				var res = new AtomicInfo[this.Atoms.Count];
				foreach (var atom in this.Atoms)
				{
					//atom.xmlOpt = this.xmlOpt;
					res[k++] = atom;
				}
				return res.ToArray();
			}
			set { this.Atoms = new List<AtomicInfo>(value); }
		}

		internal static TBox[] BoxListSerialize<TBox>(ICollection<TBox> list)
			where TBox: class
		{
			int k = 0;
			var res = new TBox[list.Count];
			foreach (TBox atom in list)
			{
				//atom.xmlOpt = this.xmlOpt;
				res[k++] = atom;
			}
			return res.ToArray();
		}

		internal static List<TBox> BoxListDeserialize<TBox>(TBox[] list)
			where TBox: class
		{
			return new List<TBox>(list);
		}

		public static readonly XmlSerializerNamespaces DefaultXMLNamespaces = new XmlSerializerNamespaces(
			new XmlQualifiedName[] { new XmlQualifiedName(String.Empty, XMLNS) });

		public Container()
		{
			this.editable = false;
			this.Atoms = new List<AtomicInfo>();
		}

		public Container(bool editable)
			: this()
		{
			this.editable = editable;
		}

		private BinaryReader CreateReader(Stream file)
		{
			var bound = new BoundStream(file, 0L, -1L);
			return new BinReader(bound, littleEndian: false);
		}

		#region Locating/Finding Atoms
		private int GetTrackCount()
		{
			return this.Atoms.Where(file => file is IBoxContainer).SelectMany(file => ((IBoxContainer)file).Boxes).
				Count(atom =>  atom.Name == "trak");
		}

		private AtomicInfo FindAtomInTrack(int track_num, AtomicCode search_atom)
		{
			int track_tally = 0;

			foreach (var atom in this.Atoms.Where(file => file is IBoxContainer).SelectMany(file => ((IBoxContainer)file).Boxes))
			{
				if (atom.Name == "trak")
				{
					track_tally += 1;
					if (track_num == track_tally)
					{
						//drill down into stsd
						return atom.FindAtom(search_atom);
					}
				}
			}
			return null;
		}

		private static void FindUnknownBoxes(IDictionary<Type, Type[]> unknowns, IBoxContainer container)
		{
			var childs = container.Boxes.Where(box =>
				box is ISOMediaBoxes.UnknownBox ||
				box is ISOMediaBoxes.UnknownParentBox ||
				box is ISOMediaBoxes.UnknownUUIDBox ||
				box is ISOMediaBoxes.InvalidBox ||
				box is ISOMediaBoxes.VoidBox ||
				box is ISOMediaBoxes.FreeSpaceBox).Select(box => box.GetType());
			if (childs.Any())
			{
				Type[] list;
				var type = container.GetType();
				if (type == typeof(ISOMediaBoxes.UserDataBox))
					type = typeof(ISOMediaBoxes.UserDataMap);
				if (unknowns.TryGetValue(type, out list))
					unknowns[type] = list.Concat(childs).Distinct().ToArray();
				else
					unknowns.Add(type, childs.Distinct().ToArray());
			}
			foreach (var child in container.Boxes.Select(box => box as IBoxContainer).Where(box => box != null))
				FindUnknownBoxes(unknowns, child);
		}

		public IDictionary<Type, Type[]> FindUnknownBoxes()
		{
			var unknowns = new Dictionary<Type, Type[]>();
			foreach (var container in Atoms.Select(box => box as IBoxContainer).Where(box => box != null))
				FindUnknownBoxes(unknowns, container);
			return unknowns;
		}
		#endregion

		#region File scanning & atom parsing

		private MetadataStyle IdentifyBrand(string brand)
		{
			switch (brand)
			{
			//what ISN'T supported
			case "qt  " : //this is listed at mp4ra, but there are features of the file that aren't supported (like the 4 NULL bytes after the last udta child atom
				throw new NotSupportedException("Quicktime movie files are not supported.");

			//
			//3GPP2 specification documents brands
			//

			case "3g2b" : //3GPP2 release A
				return MetadataStyle.ThirdGenPartnerVer2RelA; //3GPP2 C.S0050-A_v1.0_060403, Annex A.2 lists differences between 3GPP & 3GPP2 - assets are not listed

			case "3g2a" : //                                //3GPP2 release 0
				return MetadataStyle.ThirdGenPartnerVer2;

			//
			//3GPP specification documents brands, not all are listed at mp4ra
			//

			case "3gp7" : //                                //Release 7 introduces ID32; though it doesn't list a iso bmffv2 compatible brand. Technically, ID32
			//could be used on older 3gp brands, but iso2 would have to be added to the compatible brand list.
			case "3gs7" : //                                //I don't feel the need to do that, since other things might have to be done. And I'm not looking into it.
			case "3gr7" :
			case "3ge7" :
			case "3gg7" :
				return MetadataStyle.ThirdGenPartnerVer1Rel7;

			case "3gp6" : //                                //3gp assets which were introducted by NTT DoCoMo to the Rel6 workgroup on January 16, 2003
			//with S4-030005.zip from http://www.3gpp.org/ftp/tsg_sa/WG4_CODEC/TSGS4_25/Docs/ (! albm, loci)
			case "3gr6" : //progressive
			case "3gs6" : //streaming
			case "3ge6" : //extended presentations (jpeg images)
			case "3gg6" : //general (not yet suitable; superset)
				return MetadataStyle.ThirdGenPartnerVer1Rel6;
				break;

			case "3gp4" : //                                //3gp assets (the full complement) are available: source clause is S5.5 of TS26.244 (Rel6.4 & later):
			case "3gp5" : //                                //"that the file conforms to the specification; it includes everything required by,
				//                                          //and nothing contrary to the specification (though there may be other material)"
				//                                          //it stands to reason that 3gp assets aren't contrary since 'udta' is defined by iso bmffv1
				return MetadataStyle.ThirdGenPartner;

			//
			//other brands that are have compatible brands relating to 3GPP/3GPP2
			//

			case "kddi" : //                                //3GPP2 EZmovie (optionally restricted) media; these have a 3GPP2 compatible brand
				return MetadataStyle.ThirdGenPartnerVer2;
			case "mmp4" :
				return MetadataStyle.ThirdGenPartner;

			//
			//what IS supported for iTunes-style metadata
			//

			case "MSNV" : //(PSP) - this isn't actually listed at mp4ra, but since they are popular...
				return MetadataStyle.iTunes | MetadataStyle.PSP;
			case "M4A " : //these are all listed at http://www.mp4ra.org/filetype.html as registered brands
			case "M4B " :
			case "M4P " :
			case "M4V " :
			case "M4VH" :
			case "M4VP" :
			case "mp42" :
			case "mp41" :
			case "isom" :
			case "iso2" :
			case "avc1" :
				return MetadataStyle.iTunes;

			//
			//other brands that are derivatives of the ISO Base Media File Format
			//
			case "mjp2" :
			case "mj2s" :
				return MetadataStyle.MotionJPEG2000;

			//other lesser unsupported brands; http://www.mp4ra.org/filetype.html like dv, mp21 & ... whatever mpeg7 brand is
			default:
				throw new NotSupportedException(String.Format("Unsupported MPEG-4 file brand found '{0}'", brand));
			}
		}

		private ISOMediaBoxes.FileTypeBox ReadFileType(BinaryReader reader)
		{
			this.Atoms.Clear();
			long boxStart = reader.BaseStream.Position;
			long boxSize = reader.ReadUInt32();
			var atomid = reader.ReadAtomicCode();
			ISOMediaBoxes.FileTypeBox ftyp = null;
			if (boxSize == 12L && atomid == ISOMediaBoxes.JPEG2000Atom.DefaultID)
			{
				var jP = AtomicInfo.ParseBox(reader, boxSize, atomid, editable) as ISOMediaBoxes.JPEG2000Atom;
				if (jP == null || jP.Data != ISOMediaBoxes.JPEG2000Atom.Signature)
				{
					throw new InvalidDataException("Bad jpeg2000 file (invalid header).");
				}
				this.Atoms.Add(jP);
				ftyp = AtomicInfo.ParseBox(reader) as ISOMediaBoxes.FileTypeBox;
				if (ftyp == null)
				{
					throw new InvalidDataException("Expected ftyp atom missing."); //the atom right after the jpeg2000/mjpeg2000 signature is *supposed* to be 'ftyp'
				}
			}
			else
			{
				if (atomid == ISOMediaBoxes.FileTypeBox.DefaultID)
					ftyp = AtomicInfo.ParseBox(reader, boxSize, atomid, editable) as ISOMediaBoxes.FileTypeBox;
				if (ftyp == null)
				{
					throw new InvalidDataException("Bad mpeg4 file (ftyp atom missing or alignment error).");
				}
			}
			this.Atoms.Add(ftyp);

			return ftyp;
		}

		/// <summary>
		/// ScanAtoms
		/// </summary>
		/// <param name="path">the complete path to the originating file to be tested</param>
		/// <param name="deepscan_REQ">controls whether we go into 'stsd' or just a superficial scan</param>
		private void ScanAtoms(Stream file)
		{
			var reader = CreateReader(file);

			metadataStyle = IdentifyBrand((string)ReadFileType(reader).Brand);

			while (true)
			{
				var atom = AtomicInfo.ParseBox(reader, required: false, editable: editable);
				if (atom == null) break;
				this.Atoms.Add(atom);
			}

			//if (brand == 0x69736F6D) { //'isom' test for amc files & its (?always present?) uuid 0x63706764A88C11D48197009027087703
			//    char EZ_movie_uuid[100];
			//    memset(EZ_movie_uuid, 0, sizeof(EZ_movie_uuid));
			//    memcpy(EZ_movie_uuid, "uuid=\x63\x70\x67\x64\xA8\x8C\x11\xD4\x81\x97\x00\x90\x27\x08\x77\x03", 21); //this is in an endian form, so it needs to be converted
			//    APar_endian_uuid_bin_str_conversion(EZ_movie_uuid+5);
			//    if ( APar_FindAtom(EZ_movie_uuid, false, EXTENDED_ATOM, 0, true) != NULL) {
			//        metadata_style = UNDEFINED_STYLE;
			//    }
			//}

			if (/*!deep_atom_scan &&*/ !this.Atoms.Any(box => box.Name == "moov"))
				throw new InvalidDataException("Bad mpeg4 file (no 'moov' atom).");
		}

		public static Container Create(Stream file, bool editable = false)
		{
			var mp4 = new Container(editable);
			mp4.ScanAtoms(file);
			return mp4;
		}
		#endregion

		#region Extracts
		public void ExtractBrands(Stream fs, TextWriter stdout)
		{
			var reader = CreateReader(fs);

			var ftyp = ReadFileType(reader);
			var metadata_style = IdentifyBrand((string)ftyp.Brand);

			bool cb_V2ISOBMFF = ftyp.TestCompatibleBrand();

			stdout.Write(" Major Brand: {0}", ftyp.Brand);

			//if (ftyp.Brand == "isom")
			//{
			//	fs.Seek(save, SeekOrigin.Begin);
			//	ScanAtoms(fs);
			//}

			stdout.WriteLine("  -  version {0:X}", ftyp.Version);

			stdout.WriteLine(" Compatible Brands: {0}", String.Join(" ", ftyp.CompatibleBrand));

			stdout.WriteLine(" Tagging schemes available:");
			switch (metadata_style)
			{
				case MetadataStyle.iTunes:
				case MetadataStyle.iTunes | MetadataStyle.PSP:
					stdout.WriteLine("   iTunes-style metadata allowed.");
					break;
				case MetadataStyle.ThirdGenPartner:
				case MetadataStyle.ThirdGenPartnerVer1Rel6:
				case MetadataStyle.ThirdGenPartnerVer1Rel7:
				case MetadataStyle.ThirdGenPartnerVer2:
					stdout.WriteLine("   3GP-style asset metadata allowed.");
					break;
				case MetadataStyle.ThirdGenPartnerVer2RelA:
					stdout.WriteLine("   3GP-style asset metadata allowed [& unimplemented GAD (Geographical Area Description) asset].");
					break;
			}
			if (cb_V2ISOBMFF || metadata_style == MetadataStyle.ThirdGenPartnerVer1Rel7)
			{
				stdout.WriteLine("   ID3 tags on ID32 atoms @ file/movie/track level allowed.");
			}
			stdout.WriteLine("   ISO-copyright notices @ movie and/or track level allowed.");
			stdout.WriteLine("   uuid private user extension tags allowed.");
		}
		#endregion
	}
}
