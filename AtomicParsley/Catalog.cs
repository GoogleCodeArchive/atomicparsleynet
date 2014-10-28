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

namespace AtomicParsley
{
	public static class Catalog
	{
		public static void ListStikValues(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available stik settings - case sensitive");
			writer.Write('\t');
			writer.WriteLine("(number in parens shows the stik value).");

			foreach (var stik in MP4.Catalog.StikList)
				writer.WriteLine("({0}) \t{1}", stik.Number, stik.Name);
		}

		public static void ListMediaRatings(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available ratings:");
			foreach (var rating in MP4.Catalog.KnownRatings)
				writer.WriteLine(" {0}", rating.Label);
		}

		public static void ListTVGenreIDValues(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available iTunes TV Genre IDs:");
			foreach (var genre in MP4.Catalog.GenreIDTV)
				writer.WriteLine("({0})\t{1}", genre.ID, genre.Name);
		}

		public static void ListMovieGenreIDValues(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available iTunes Movie Genre IDs:");
			foreach (var genre in MP4.Catalog.GenreIDMovie)
				writer.WriteLine("({0})\t{1}", genre.ID, genre.Name);
		}

		public static void ListLanguageCodes(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available language codes");
			writer.WriteLine("ISO639-2 code  ...\tEnglish name:");
			foreach (var lang in ID3v1.Catalog.KnownLanguageList)
#if NOMUI
				writer.WriteLine(" {0,-7} ...\t{1}{2}", lang.ISO6392Code, lang.EnglishName, lang.CultureInfo == null ? " (Int)" : "");
#else
				if (lang.CultureInfo == null)
					writer.WriteLine(" {0,-7} ...\t{1} (Int)", lang.ISO6392Code, lang.EnglishName);
				else
					writer.WriteLine(" {0,-7} ...\t{1}", lang.ISO6392Code, lang.CultureInfo.DisplayName);
#endif
		}

		public static void ListMacLanguageCodes(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available Macintosh language codes");
			writer.WriteLine("Macintosh code  ...\tEnglish name:");
			foreach (var lang in MP4.Catalog.MacLanguageList)
#if NOMUI
				writer.WriteLine(" {0,-3} ...\t{1}{2}", lang.Code, lang.EnglishName, lang.CultureInfo == null ? " (Int)" : "");
#else
				if (lang.CultureInfo == null)
					writer.WriteLine(" {0,-3} ...\t{1} (Int)", lang.Code, lang.EnglishName);
				else
					writer.WriteLine(" {0,-3} ...\t{1}", lang.Code, lang.CultureInfo.DisplayName);
#endif
		}

		public static void ListGenresValues(TextWriter writer)
		{
			writer.Write('\t');
			writer.WriteLine("Available standard genres - case sensitive.");
			foreach (var genre in ID3v1.Catalog.GenreList)
				writer.WriteLine("({0}.) \t{1}", ID3v1.Catalog.GenreByName[genre], genre);
		}

		public static void ListImagTypeStrings(TextWriter writer)
		{
			writer.WriteLine("These 'image types' are used to identify pictures embedded in 'APIC' ID3 tags:");
			writer.WriteLine("      usage is \"AP --ID3Tag APIC /path.jpg --imagetype=\"str\"");
			writer.WriteLine("      str can be either the hex listing *or* the full string");
			writer.WriteLine("      default is 0x00 - meaning 'Other'");
			writer.WriteLine("   Hex      \tFull String");
			writer.WriteLine("  ----------------------------");
			foreach (var image in ID3v2.Definitions.ImageTypeList)
				writer.WriteLine("  0x{0:X2}     \t\"{1}\"", image.HexCode, image.ImageType);
		}
	}
}
