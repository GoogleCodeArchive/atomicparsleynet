//==================================================================//
/*
    AtomicParsley - arrays.cpp

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

    * Mellow_Flow - fix genre matching/verify genre limits
    ----------------------
    SVN revision information:
      $Revision$
                                                                   */
//==================================================================//
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace MP4
{
	public static class Catalog
	{
		public class MacLanguage
		{
			private CultureInfo ci;
			public string ISO6391Code { get; private set; }
			public string EnglishName { get; private set; }
			public ushort Code { get; private set; }
			public CultureInfo CultureInfo
			{
				get { return this.ci; }
				internal set
				{
					this.ci = value;
					this.EnglishName = ci.EnglishName;
				}
			}
			public MacLanguage(string iso6391Code, string engName, ushort code)
			{
				this.ISO6391Code = iso6391Code;
				this.EnglishName = engName;
				this.Code = code;
			}
		}

		static Catalog()
		{
			var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			foreach (var ci in cultures)
			{
				MacLanguage lang;
				if (MacLanguagesISO.TryGetValue(ci.Name, out lang))
				{
					if (lang.CultureInfo == null || ci.IsNeutralCulture && !lang.CultureInfo.IsNeutralCulture)
						lang.CultureInfo = ci;
				}
			}
		}

		public static readonly Stik[] StikList = new Stik[]
		{
			new Stik( "Movie", 0 ),
			new Stik( "Normal", 1 ),
			new Stik( "Audiobook", 2 ),
			new Stik( "Whacked Bookmark", 5 ),
			new Stik( "Music Video", 6 ),
			new Stik( "Short Film", 9 ),
			new Stik( "TV Show", 10 ),
			new Stik( "Booklet", 11 )
		};

		public static readonly GenreID[] GenreIDMovie = new GenreID[]
		{
			new GenreID( "Action & Adventure", 4401 ),
			new GenreID( "Anime", 4402 ),
			new GenreID( "Classics", 4403 ),
			new GenreID( "Comedy", 4404 ),
			new GenreID( "Documentary", 4405 ),
			new GenreID( "Drama", 4406 ),
			new GenreID( "Foreign", 4407 ),
			new GenreID( "Horror", 4408 ),
			new GenreID( "Independent", 4409 ),
			new GenreID( "Kids & Family", 4410 ),
			new GenreID( "Musicals", 4411 ),
			new GenreID( "Romance", 4412 ),
			new GenreID( "Sci-Fi & Fantasy", 4413 ),
			new GenreID( "Short Films", 4414 ),
			new GenreID( "Special Interest", 4415 ),
			new GenreID( "Thriller", 4416 ),
			new GenreID( "Sports", 4417 ),
			new GenreID( "Western", 4418 ),
			new GenreID( "Urban", 4419 ),
			new GenreID( "Holiday", 4420 ),
			new GenreID( "Made for TV", 4421 ),
			new GenreID( "Concert Films", 4422 ),
			new GenreID( "Music Documentaries", 4423 ),
			new GenreID( "Music Feature Films", 4424 ),
			new GenreID( "Japanese Cinema", 4425 ),
			new GenreID( "Jidaigeki", 4426 ),
			new GenreID( "Tokusatsu", 4427 ),
			new GenreID( "Korean Cinema", 4428 )
		};

		public static readonly GenreID[] GenreIDTV = new GenreID[]
		{
			new GenreID( "Comedy", 4000 ),
			new GenreID( "Drama", 4001 ),
			new GenreID( "Animation", 4002 ),
			new GenreID( "Action & Adventure", 4003 ),
			new GenreID( "Classic", 4004 ),
			new GenreID( "Kids", 4005 ),
			new GenreID( "Nonfiction", 4005 ),
			new GenreID( "Reality TV", 4007 ),
			new GenreID( "Sci-Fi & Fantasy", 4008 ),
			new GenreID( "Sports", 4009 ),
			new GenreID( "Teens", 4010 ),
			new GenreID( "Latino TV", 4011 )
		};

		// from William Herrera: http://search.cpan.org/src/BILLH/LWP-UserAgent-iTMS_Client-0.16/lib/LWP/UserAgent/iTMS_Client.pm
		public static readonly StoreFrontID[] StoreFronts = new StoreFrontID[]
		{
			new StoreFrontID( "United States",  143441 ),
			new StoreFrontID( "France",         143442 ),
			new StoreFrontID( "Germany",        143443 ),
			new StoreFrontID( "United Kingdom", 143444 ),
			new StoreFrontID( "Austria",        143445 ),
			new StoreFrontID( "Belgium",        143446 ),
			new StoreFrontID( "Finland",        143447 ),
			new StoreFrontID( "Greece",         143448 ),
			new StoreFrontID( "Ireland",        143449 ),
			new StoreFrontID( "Italy",          143450 ),
			new StoreFrontID( "Luxembourg",     143451 ),
			new StoreFrontID( "Netherlands",    143452 ),
			new StoreFrontID( "Portugal",       143453 ),
			new StoreFrontID( "Spain",          143454 ),
			new StoreFrontID( "Canada",         143455 ),
			new StoreFrontID( "Sweden",         143456 ),
			new StoreFrontID( "Norway",         143457 ),
			new StoreFrontID( "Denmark",        143458 ),
			new StoreFrontID( "Switzerland",    143459 ),
			new StoreFrontID( "Australia",      143460 ),
			new StoreFrontID( "New Zealand",    143461 ),
			new StoreFrontID( "Japan",          143462 )
		};

		public static readonly MediaRating[] KnownRatings = new MediaRating[]
		{
			new MediaRating( "us-tv|TV-MA|600|",       "TV-MA" ),
			new MediaRating( "us-tv|TV-14|500|",       "TV-14" ),
			new MediaRating( "us-tv|TV-PG|400|",       "TV-PG" ),
			new MediaRating( "us-tv|TV-G|300|",        "TV-G" ),
			new MediaRating( "us-tv|TV-Y|200|",        "TV-Y" ),
			new MediaRating( "us-tv|TV-Y7|100|",       "TV-Y7" ),
			              // "us-tv||0|",              "not-applicable" }, //though its a valid flag & some files have this, AP won't be setting it.
			new MediaRating( "mpaa|UNRATED|600|",      "Unrated" ),
			new MediaRating( "mpaa|NC-17|500|",        "NC-17" ),
			new MediaRating( "mpaa|R|400|",            "R" ),
			new MediaRating( "mpaa|PG-13|300|",        "PG-13" ),
			new MediaRating( "mpaa|PG|200|",           "PG" ),
			new MediaRating( "mpaa|G|100|",            "G" ),
			              // "mpaa||0|",               "not-applicable" } //see above
			//new MediaRating( "ru-movies|UNRATED|600|", "Unrated" ),
			new MediaRating( "ru-movies|18+|400|",     "18+" ),
			new MediaRating( "ru-movies|16+|375|",     "16+" ),
			new MediaRating( "ru-movies|12+|200|",     "12+" ),
			new MediaRating( "ru-movies|6+|150|",      "6+" ),
			new MediaRating( "ru-movies|0+|100|",      "0+" )
		};

		/// <summary>
		/// Macintosh Language Codes
		/// </summary>
		/// <remarks>
		/// The Macintosh language codes supported by QuickTime.
		/// </remarks>
		public static readonly MacLanguage[] MacLanguageList = new MacLanguage[]
		{
			new MacLanguage( "en",      "English",              0 ),
			new MacLanguage( "fr",      "French",               1 ),
			new MacLanguage( "de",      "German",               2 ),
			new MacLanguage( "it",      "Italian",              3 ),
			new MacLanguage( "nl",      "Dutch",                4 ),
			new MacLanguage( "sv",      "Swedish",              5 ),
			new MacLanguage( "es",      "Spanish",              6 ),
			new MacLanguage( "da",      "Danish",               7 ),
			new MacLanguage( "pt",      "Portuguese",           8 ),
			new MacLanguage( "no",      "Norwegian",            9 ),
			new MacLanguage( "he",      "Hebrew",               10 ),
			new MacLanguage( "ja",      "Japanese",             11 ),
			new MacLanguage( "ar",      "Arabic",               12 ),
			new MacLanguage( "fi",      "Finnish",              13 ),
			new MacLanguage( "el",      "Greek",                14 ),
			new MacLanguage( "is",      "Icelandic",            15 ),
			new MacLanguage( "mt",      "Maltese",              16 ),
			new MacLanguage( "tr",      "Turkish",              17 ),
			new MacLanguage( "hr",      "Croatian",             18 ),
			new MacLanguage( "zh-Hant", "Traditional Chinese",  19 ),
			new MacLanguage( "ur",      "Urdu",                 20 ),
			new MacLanguage( "hi",      "Hindi",                21 ),
			new MacLanguage( "th",      "Thai",                 22 ),
			new MacLanguage( "ko",      "Korean",               23 ),
			new MacLanguage( "lt",      "Lithuanian",           24 ),
			new MacLanguage( "pl",      "Polish",               25 ),
			new MacLanguage( "hu",      "Hungarian",            26 ),
			new MacLanguage( "et",      "Estonian",             27 ),
			new MacLanguage( "lv",      "Latvian",              28 ),
			new MacLanguage( "se",      "Sami",                 29 ),
			new MacLanguage( "fo",      "Faroese",              30 ),
			new MacLanguage( "fa",      "Farsi",                31 ),
			new MacLanguage( "ru",      "Russian",              32 ),
			new MacLanguage( "zh-Hans", "Simplified Chinese",   33 ),
			new MacLanguage( "nl-BE",   "Flemish",              34 ),
			new MacLanguage( "ga",      "Irish",                35 ),
			new MacLanguage( "sq",      "Albanian",             36 ),
			new MacLanguage( "ro",      "Romanian",             37 ),
			new MacLanguage( "cs",      "Czech",                38 ),
			new MacLanguage( "sk",      "Slovak",               39 ),
			new MacLanguage( "sl",      "Slovenian",            40 ),
			new MacLanguage( "yi",      "Yiddish",              41 ),
			new MacLanguage( "sr",      "Serbian",              42 ),
			new MacLanguage( "mk",      "Macedonian",           43 ),
			new MacLanguage( "bg",      "Bulgarian",            44 ),
			new MacLanguage( "uk",      "Ukrainian",            45 ),
			new MacLanguage( "be",      "Belarusian",           46 ),
			new MacLanguage( "uz",      "Uzbek",                47 ),
			new MacLanguage( "kk",      "Kazakh",               48 ),
			new MacLanguage( "az",      "Azerbaijani",          49 ),
			new MacLanguage( "az-Arab", "Azerbaijani (Arabic)", 50 ),
			new MacLanguage( "hy",      "Armenian",             51 ),
			new MacLanguage( "ka",      "Georgian",             52 ),
			new MacLanguage( "mo",      "Moldavian",            53 ),
			new MacLanguage( "ky",      "Kirghiz",              54 ),
			new MacLanguage( "tg",      "Tajiki",               55 ),
			new MacLanguage( "tk",      "Turkmen",              56 ),
			new MacLanguage( "mn",      "Mongolian",            57 ),
			new MacLanguage( "mn-Cyrl", "Mongolian (Cyrillic)", 58 ),
			new MacLanguage( "ps",      "Pashto",               59 ),
			new MacLanguage( "ku",      "Kurdish",              60 ),
			new MacLanguage( "ks",      "Kashmiri",             61 ),
			new MacLanguage( "sd",      "Sindhi",               62 ),
			new MacLanguage( "bo",      "Tibetan",              63 ),
			new MacLanguage( "ne",      "Nepali",               64 ),
			new MacLanguage( "sa",      "Sanskrit",             65 ),
			new MacLanguage( "mr",      "Marathi",              66 ),
			new MacLanguage( "bn",      "Bengali",              67 ),
			new MacLanguage( "as",      "Assamese",             68 ),
			new MacLanguage( "gu",      "Gujarati",             69 ),
			new MacLanguage( "pa",      "Punjabi",              70 ),
			new MacLanguage( "or",      "Oriya",                71 ),
			new MacLanguage( "ml",      "Malayalam",            72 ),
			new MacLanguage( "kn",      "Kannada",              73 ),
			new MacLanguage( "ta",      "Tamil",                74 ),
			new MacLanguage( "te",      "Telugu",               75 ),
			new MacLanguage( "si",      "Sinhala",              76 ),
			new MacLanguage( "my",      "Burmese",              77 ),
			new MacLanguage( "km",      "Khmer",                78 ),
			new MacLanguage( "lo",      "Lao",                  79 ),
			new MacLanguage( "vi",      "Vietnamese",           80 ),
			new MacLanguage( "id",      "Indonesian",           81 ),
			new MacLanguage( "tl",      "Tagalog",              82 ),
			new MacLanguage( "ms-Latn", "Malay (Latin)",        83 ),
			new MacLanguage( "ms-Arab", "Malay (Arabic)",       84 ),
			new MacLanguage( "am",      "Amharic",              85 ),
			new MacLanguage( "om",      "Oromo",                87 ),
			new MacLanguage( "so",      "Somali",               88 ),
			new MacLanguage( "sw",      "Swahili",              89 ),
			new MacLanguage( "rw",      "Kinyarwanda",          90 ),
			new MacLanguage( "rn",      "Rundi",                91 ),
			new MacLanguage( "ny",      "Nyanja",               92 ),
			new MacLanguage( "mg",      "Malagasy",             93 ),
			new MacLanguage( "eo",      "Esperanto",            94 ),
			new MacLanguage( "cy",      "Welsh",                128 ),
			new MacLanguage( "eu",      "Basque",               129 ),
			new MacLanguage( "ca",      "Catalan",              130 ),
			new MacLanguage( "la",      "Latin",                131 ),
			new MacLanguage( "qu",      "Quechua",              132 ),
			new MacLanguage( "gn",      "Guarani",              133 ),
			new MacLanguage( "ay",      "Aymara",               134 ),
			new MacLanguage( "tt",      "Tatar",                135 ),
			new MacLanguage( "ug",      "Uighur",               136 ),
			new MacLanguage( "dz",      "Dzongkha",             137 ),
			new MacLanguage( "ja-Latn", "Javanese (Latin)",     138 )
		};
		public static readonly IDictionary<ushort, MacLanguage> MacLanguages = MacLanguageList.ToDictionary(lang => lang.Code);
		public static readonly IDictionary<string, MacLanguage> MacLanguagesISO = MacLanguageList.ToDictionary(lang => lang.ISO6391Code);
		public static readonly MacLanguage UnspecifiedMacLanguage = new MacLanguage(null, "Unspecified", 0x7FFF);
	}
}
