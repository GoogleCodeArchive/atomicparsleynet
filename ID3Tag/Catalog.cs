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
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace ID3v1
{
	public static class Catalog
	{
		public class ISO639Language
		{
			private CultureInfo ci;
			public string ISO6392Code { get; private set; }
			public string ISO6391Code { get; private set; }
			public string EnglishName { get; private set; }
			public CultureInfo CultureInfo
			{
				get { return this.ci; }
				internal set
				{
					this.ci = value;
					this.EnglishName = ci.EnglishName;
				}
			}
			public ISO639Language(string iso6392Code, string iso6391Code, string engName, CultureInfo ci = null)
			{
				this.ISO6392Code = iso6392Code;
				this.ISO6391Code = iso6391Code;
				this.EnglishName = engName;
				this.ci = ci;
			}
		}

		static Catalog()
		{
			var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			var langs = new List<ISO639Language>(InternalLanguageList);
			KnownLanguages = new Dictionary<string, ISO639Language>(InternalLanguageList.SelectMany(lang => lang.ISO6392Code.Split('/').
				Select(code => new { code, lang })).ToDictionary(lang => lang.code, lang => lang.lang));
			foreach (var ci in cultures)
			{
				ISO639Language lang;
				if (KnownLanguages.TryGetValue(ci.ThreeLetterISOLanguageName, out lang))
				{
					if (lang.CultureInfo == null || ci.IsNeutralCulture && !lang.CultureInfo.IsNeutralCulture)
						lang.CultureInfo = ci;
				}
				else
				{
					lang = new ISO639Language(ci.ThreeLetterISOLanguageName, ci.TwoLetterISOLanguageName, ci.EnglishName, ci);
					langs.Add(lang);
					KnownLanguages.Add(ci.ThreeLetterISOLanguageName, lang);
				}
			}
			KnownLanguageList = langs.OrderBy(lang => lang.ISO6392Code).ToArray();
		}

		public static readonly string[] GenreList = new string[] {
			"Blues", "Classic Rock", "Country", "Dance", "Disco",
			"Funk", "Grunge", "Hip-Hop", "Jazz", "Metal",
			"New Age", "Oldies", "Other", "Pop", "R&B",
			"Rap", "Reggae", "Rock", "Techno", "Industrial",
			"Alternative", "Ska", "Death Metal", "Pranks", "Soundtrack",
			"Euro-Techno", "Ambient", "Trip-Hop", "Vocal", "Jazz+Funk",
			"Fusion", "Trance", "Classical", "Instrumental", "Acid",
			"House", "Game", "Sound Clip", "Gospel", "Noise",
			"AlternRock", "Bass", "Soul", "Punk", "Space", 
			"Meditative", "Instrumental Pop", "Instrumental Rock", "Ethnic", "Gothic", 
			"Darkwave", "Techno-Industrial", "Electronic", "Pop-Folk", "Eurodance", 
			"Dream", "Southern Rock", "Comedy", "Cult", "Gangsta",
			"Top 40", "Christian Rap", "Pop/Funk", "Jungle", "Native American",
			"Cabaret", "New Wave", "Psychadelic", "Rave", "Showtunes",
			"Trailer", "Lo-Fi", "Tribal", "Acid Punk", "Acid Jazz",
			"Polka", "Retro", "Musical", "Rock & Roll", "Hard Rock",
			"Folk", "Folk/Rock", "National Folk", "Swing", "Fast Fusion",
			"Bebob", "Latin", "Revival", "Celtic", "Bluegrass",
			"Avantgarde", "Gothic Rock", "Progressive Rock", "Psychedelic Rock", "Symphonic Rock",
			"Slow Rock", "Big Band", "Chorus", "Easy Listening", "Acoustic", 
			"Humour", "Speech", "Chanson", "Opera", "Chamber Music", "Sonata", 
			"Symphony", "Booty Bass", "Primus", "Porn Groove", 
			"Satire", "Slow Jam", "Club", "Tango", "Samba", 
			"Folklore", "Ballad", "Power Ballad", "Rhythmic Soul", "Freestyle", 
			"Duet", "Punk Rock", "Drum Solo", "A Capella", "Euro-House",
			"Dance Hall"
		};
			/*
			"Goa", "Drum & Bass", "Club House", "Hardcore", 
			"Terror", "Indie", "BritPop", "NegerPunk", "Polsk Punk", 
			"Beat", "Christian Gangsta", "Heavy Metal", "Black Metal", "Crossover", 
			"Contemporary C", "Christian Rock", "Merengue", "Salsa", "Thrash Metal", 
			"Anime", "JPop", "SynthPop",
			}; */
			//apparently the other winamp id3v1 extensions aren't valid
		//the list starts at 0; the embedded genres start at 1
		public static readonly IDictionary<int, string> GenreByNum = GenreList.Select((name, index) => new { name, index }).ToDictionary(genre => genre.index + 1, genre => genre.name);
		public static readonly IDictionary<string, int> GenreByName = GenreList.Select((name, index) => new { name, index }).ToDictionary(genre => genre.name, genre => genre.index + 1);

		private static ISO639Language[] InternalLanguageList = new ISO639Language[]
		{
			new ISO639Language( "aar",     "aa",     "Afar" ),
			new ISO639Language( "abk",     "ab",     "Abkhazian" ),
			new ISO639Language( "ace",     null,     "Achinese" ),
			new ISO639Language( "ach",     null,     "Acoli" ),
			new ISO639Language( "ada",     null,     "Adangme" ),
			new ISO639Language( "ady",     null,     "Adyghe; Adygei" ),
			new ISO639Language( "afa",     null,     "Afro-Asiatic (Other)" ),
			new ISO639Language( "afh",     null,     "Afrihili" ),
			new ISO639Language( "afr",     "af",     "Afrikaans" ),
			new ISO639Language( "ain",     null,     "Ainu" ),
			new ISO639Language( "aka",     "ak",     "Akan" ),
			new ISO639Language( "akk",     null,     "Akkadian" ),
			new ISO639Language( "alb/sqi", "sq",     "Albanian" ), //dual codes
			new ISO639Language( "ale",     null,     "Aleut" ),
			new ISO639Language( "alg",     null,     "Algonquian languages" ),
			new ISO639Language( "alt",     null,     "Southern Altai" ),
			new ISO639Language( "amh",     "am",     "Amharic" ),
			new ISO639Language( "ang",     null,     "English, Old (ca.450-1100)" ),
			new ISO639Language( "anp",     null,     "Angika" ),
			new ISO639Language( "apa",     null,     "Apache languages" ),
			new ISO639Language( "ara",     "ar",     "Arabic" ),
			new ISO639Language( "arc",     null,     "Aramaic" ),
			new ISO639Language( "arg",     "an",     "Aragonese" ),
			new ISO639Language( "arm/hye", "hy",     "Armenian" ), //dual codes
			new ISO639Language( "arn",     null,     "Araucanian" ),
			new ISO639Language( "arp",     null,     "Arapaho" ),
			new ISO639Language( "art",     null,     "Artificial (Other)" ),
			new ISO639Language( "arw",     null,     "Arawak" ),
			new ISO639Language( "asm",     "as",     "Assamese" ),
			new ISO639Language( "ast",     null,     "Asturian; Bable" ),
			new ISO639Language( "ath",     null,     "Athapascan languages" ),
			new ISO639Language( "aus",     null,     "Australian languages" ),
			new ISO639Language( "ava",     "av",     "Avaric" ),
			new ISO639Language( "ave",     "ae",     "Avestan" ),
			new ISO639Language( "awa",     null,     "Awadhi" ),
			new ISO639Language( "aym",     "ay",     "Aymara" ),
			new ISO639Language( "aze",     "az",     "Azerbaijani" ),
			new ISO639Language( "bad",     null,     "Banda" ),
			new ISO639Language( "bai",     null,     "Bamileke languages" ),
			new ISO639Language( "bak",     "ba",     "Bashkir" ),
			new ISO639Language( "bal",     null,     "Baluchi" ),
			new ISO639Language( "bam",     "bm",     "Bambara" ),
			new ISO639Language( "ban",     null,     "Balinese" ),
			new ISO639Language( "baq/eus", "eu",     "Basque" ), //dual codes
			new ISO639Language( "bas",     null,     "Basa" ),
			new ISO639Language( "bat",     null,     "Baltic (Other)" ),
			new ISO639Language( "bej",     null,     "Beja" ),
			new ISO639Language( "bel",     "be",     "Belarusian" ),
			new ISO639Language( "bem",     null,     "Bemba" ),
			new ISO639Language( "ben",     "bn",     "Bengali" ),
			new ISO639Language( "ber",     null,     "Berber (Other)" ),
			new ISO639Language( "bho",     null,     "Bhojpuri" ),
			new ISO639Language( "bih",     "bh",     "Bihari" ),
			new ISO639Language( "bik",     null,     "Bikol" ),
			new ISO639Language( "bin",     null,     "Bini" ),
			new ISO639Language( "bis",     "bi",     "Bislama" ),
			new ISO639Language( "bla",     null,     "Siksika" ),
			new ISO639Language( "bnt",     null,     "Bantu (Other)" ),
			new ISO639Language( "bos",     "bs",     "Bosnian" ),
			new ISO639Language( "bra",     null,     "Braj" ),
			new ISO639Language( "bre",     "br",     "Breton" ),
			new ISO639Language( "btk",     null,     "Batak (Indonesia)" ),
			new ISO639Language( "bua",     null,     "Buriat" ),
			new ISO639Language( "bug",     null,     "Buginese" ),
			new ISO639Language( "bul",     "bg",     "Bulgarian" ),
			new ISO639Language( "bur/mya", "my",     "Burmese" ), //dual codes
			new ISO639Language( "byn",     null,     "Blin; Bilin" ),
			new ISO639Language( "cad",     null,     "Caddo" ),
			new ISO639Language( "cai",     null,     "Central American Indian (Other)" ),
			new ISO639Language( "car",     null,     "Carib" ),
			new ISO639Language( "cat",     "ca",     "Catalan; Valencian" ),
			new ISO639Language( "cau",     null,     "Caucasian (Other)" ),
			new ISO639Language( "ceb",     null,     "Cebuano" ),
			new ISO639Language( "cel",     null,     "Celtic (Other)" ),
			new ISO639Language( "cha",     "ch",     "Chamorro" ),
			new ISO639Language( "chb",     null,     "Chibcha" ),
			new ISO639Language( "che",     "ce",     "Chechen" ),
			new ISO639Language( "chg",     null,     "Chagatai" ),
			new ISO639Language( "chk",     null,     "Chuukese" ),
			new ISO639Language( "chm",     null,     "Mari" ),
			new ISO639Language( "chn",     null,     "Chinook jargon" ),
			new ISO639Language( "cho",     null,     "Choctaw" ),
			new ISO639Language( "chp",     null,     "Chipewyan" ),
			new ISO639Language( "chr",     null,     "Cherokee" ),
			new ISO639Language( "chu",     "cu",     "Church Slavic; Old Slavonic; Church Slavonic; Old Bulgarian; Old Church Slavonic" ),
			new ISO639Language( "chv",     "cv",     "Chuvash" ),
			new ISO639Language( "chy",     null,     "Cheyenne" ),
			new ISO639Language( "cmc",     null,     "Chamic languages" ),
			new ISO639Language( "cop",     null,     "Coptic" ),
			new ISO639Language( "cor",     "kw",     "Cornish" ),
			new ISO639Language( "cos",     "co",     "Corsican" ),
			new ISO639Language( "cpe",     null,     "Creoles and pidgins, English based (Other)" ),
			new ISO639Language( "cpf",     null,     "Creoles and pidgins, French-based (Other)" ),
			new ISO639Language( "cpp",     null,     "Creoles and pidgins, Portuguese-based (Other)" ),
			new ISO639Language( "cre",     "cr",     "Cree" ),
			new ISO639Language( "crh",     null,     "Crimean Tatar; Crimean Turkish" ),
			new ISO639Language( "crp",     null,     "Creoles and pidgins (Other)" ),
			new ISO639Language( "csb",     null,     "Kashubian" ),
			new ISO639Language( "cus",     null,     "Cushitic (Other)" ),
			new ISO639Language( "cze/ces", "cs",     "Czech" ), //dual codes
			new ISO639Language( "dak",     null,     "Dakota" ),
			new ISO639Language( "dan",     "da",     "Danish" ),
			new ISO639Language( "dar",     null,     "Dargwa" ),
			new ISO639Language( "day",     null,     "Dayak" ),
			new ISO639Language( "del",     null,     "Delaware" ),
			new ISO639Language( "den",     null,     "Slave (Athapascan)" ),
			new ISO639Language( "dgr",     null,     "Dogrib" ),
			new ISO639Language( "din",     null,     "Dinka" ),
			new ISO639Language( "div",     "dv",     "Divehi; Dhivehi; Maldivian" ),
			new ISO639Language( "doi",     null,     "Dogri" ),
			new ISO639Language( "dra",     null,     "Dravidian (Other)" ),
			new ISO639Language( "dsb",     null,     "Lower Sorbian" ),
			new ISO639Language( "dua",     null,     "Duala" ),
			new ISO639Language( "dum",     null,     "Dutch, Middle (ca.1050-1350)" ),
			new ISO639Language( "dut/nld", "nl",     "Dutch; Flemish" ), //dual codes
			new ISO639Language( "dyu",     null,     "Dyula" ),
			new ISO639Language( "dzo",     "dz",     "Dzongkha" ),
			new ISO639Language( "efi",     null,     "Efik" ),
			new ISO639Language( "egy",     null,     "Egyptian (Ancient)" ),
			new ISO639Language( "eka",     null,     "Ekajuk" ),
			new ISO639Language( "elx",     null,     "Elamite" ),
			new ISO639Language( "eng",     "en",     "English" ),
			new ISO639Language( "enm",     null,     "English, Middle (1100-1500)" ),
			new ISO639Language( "epo",     "eo",     "Esperanto" ),
			new ISO639Language( "est",     "et",     "Estonian" ),
			new ISO639Language( "ewe",     "ee",     "Ewe" ),
			new ISO639Language( "ewo",     null,     "Ewondo" ),
			new ISO639Language( "fan",     null,     "Fang" ),
			new ISO639Language( "fao",     "fo",     "Faroese" ),
			new ISO639Language( "fat",     null,     "Fanti" ),
			new ISO639Language( "fij",     "fj",     "Fijian" ),
			new ISO639Language( "fil",     null,     "Filipino; Pilipino" ),
			new ISO639Language( "fin",     "fi",     "Finnish" ),
			new ISO639Language( "fiu",     null,     "Finno-Ugrian (Other)" ),
			new ISO639Language( "fon",     null,     "Fon" ),
			new ISO639Language( "fre/fra", "fr",     "French" ), //dual codes
			new ISO639Language( "frm",     null,     "French, Middle (ca.1400-1600)" ),
			new ISO639Language( "fro",     null,     "French, Old (842-ca.1400)" ),
			new ISO639Language( "frr",     null,     "Northern Frisian" ),
			new ISO639Language( "frs",     null,     "Eastern Frisian" ),
			new ISO639Language( "fry",     "fy",     "Western Frisian" ),
			new ISO639Language( "ful",     "ff",     "Fulah" ),
			new ISO639Language( "fur",     null,     "Friulian" ),
			new ISO639Language( "gaa",     null,     "Ga" ),
			new ISO639Language( "gay",     null,     "Gayo" ),
			new ISO639Language( "gba",     null,     "Gbaya" ),
			new ISO639Language( "gem",     null,     "Germanic (Other)" ),
			new ISO639Language( "geo/kat", "ka",     "Georgian" ), //dual codes
			new ISO639Language( "ger/deu", "de",     "German" ),  //dual codes
			new ISO639Language( "gez",     null,     "Geez" ),
			new ISO639Language( "gil",     null,     "Gilbertese" ),
			new ISO639Language( "gla",     "gd",     "Gaelic; Scottish Gaelic" ),
			new ISO639Language( "gle",     "ga",     "Irish" ),
			new ISO639Language( "glg",     "gl",     "Galician" ),
			new ISO639Language( "glv",     "gv",     "Manx" ),
			new ISO639Language( "gmh",     null,     "German, Middle High (ca.1050-1500)" ),
			new ISO639Language( "goh",     null,     "German, Old High (ca.750-1050)" ),
			new ISO639Language( "gon",     null,     "Gondi" ),
			new ISO639Language( "gor",     null,     "Gorontalo" ),
			new ISO639Language( "got",     null,     "Gothic" ),
			new ISO639Language( "grb",     null,     "Grebo" ),
			new ISO639Language( "grc",     null,     "Greek, Ancient (to 1453)" ),
			new ISO639Language( "gre/ell", "el",     "Greek, Modern (1453-)" ), //dual codes
			new ISO639Language( "grn",     "gn",     "Guarani" ),
			new ISO639Language( "gsw",     null,     "Alemanic; Swiss German" ),
			new ISO639Language( "guj",     "gu",     "Gujarati" ),
			new ISO639Language( "gwi",     null,     "Gwich’in" ),
			new ISO639Language( "hai",     null,     "Haida" ),
			new ISO639Language( "hat",     "ht",     "Haitian; Haitian Creole" ),
			new ISO639Language( "hau",     "ha",     "Hausa" ),
			new ISO639Language( "haw",     null,     "Hawaiian" ),
			new ISO639Language( "heb",     "he",     "Hebrew" ),
			new ISO639Language( "her",     "hz",     "Herero" ),
			new ISO639Language( "hil",     null,     "Hiligaynon" ),
			new ISO639Language( "him",     null,     "Himachali" ),
			new ISO639Language( "hin",     "hi",     "Hindi" ),
			new ISO639Language( "hit",     null,     "Hittite" ),
			new ISO639Language( "hmn",     null,     "Hmong" ),
			new ISO639Language( "hmo",     "ho",     "Hiri Motu" ),
			new ISO639Language( "hsb",     null,     "Upper Sorbian" ),
			new ISO639Language( "hun",     "hu",     "Hungarian" ),
			new ISO639Language( "hup",     null,     "Hupa" ),
			new ISO639Language( "iba",     null,     "Iban" ),
			new ISO639Language( "ibo",     "ig",     "Igbo" ),
			new ISO639Language( "ice/isl", "is",     "Icelandic" ), //dual codes
			new ISO639Language( "ido",     "io",     "Ido" ),
			new ISO639Language( "iii",     "ii",     "Sichuan Yi" ),
			new ISO639Language( "ijo",     null,     "Ijo" ),
			new ISO639Language( "iku",     "iu",     "Inuktitut" ),
			new ISO639Language( "ile",     "ie",     "Interlingue" ),
			new ISO639Language( "ilo",     null,     "Iloko" ),
			new ISO639Language( "ina",     "ia",     "Interlingua (International Auxiliary, Language Association)" ),
			new ISO639Language( "inc",     null,     "Indic (Other)" ),
			new ISO639Language( "ind",     "id",     "Indonesian" ),
			new ISO639Language( "ine",     null,     "Indo-European (Other)" ),
			new ISO639Language( "inh",     null,     "Ingush" ),
			new ISO639Language( "ipk",     "ik",     "Inupiaq" ),
			new ISO639Language( "ira",     null,     "Iranian (Other)" ),
			new ISO639Language( "iro",     null,     "Iroquoian languages" ),
			new ISO639Language( "ita",     "it",     "Italian" ),
			new ISO639Language( "jav",     "jv",     "Javanese" ),
			new ISO639Language( "jbo",     null,     "Lojban" ),
			new ISO639Language( "jpn",     "ja",     "Japanese" ),
			new ISO639Language( "jpr",     null,     "Judeo-Persian" ),
			new ISO639Language( "jrb",     null,     "Judeo-Arabic" ),
			new ISO639Language( "kaa",     null,     "Kara-Kalpak" ),
			new ISO639Language( "kab",     null,     "Kabyle" ),
			new ISO639Language( "kac",     null,     "Kachin" ),
			new ISO639Language( "kal",     "kl",     "Kalaallisut; Greenlandic" ),
			new ISO639Language( "kam",     null,     "Kamba" ),
			new ISO639Language( "kan",     "kn",     "Kannada" ),
			new ISO639Language( "kar",     null,     "Karen" ),
			new ISO639Language( "kas",     "ks",     "Kashmiri" ),
			new ISO639Language( "kau",     "kr",     "Kanuri" ),
			new ISO639Language( "kaw",     null,     "Kawi" ),
			new ISO639Language( "kaz",     "kk",     "Kazakh" ),
			new ISO639Language( "kbd",     null,     "Kabardian" ),
			new ISO639Language( "kha",     null,     "Khasi" ),
			new ISO639Language( "khi",     null,     "Khoisan (Other)" ),
			new ISO639Language( "khm",     "km",     "Khmer" ),
			new ISO639Language( "kho",     null,     "Khotanese" ),
			new ISO639Language( "kik",     "ki",     "Kikuyu; Gikuyu" ),
			new ISO639Language( "kin",     "rw",     "Kinyarwanda" ),
			new ISO639Language( "kir",     "ky",     "Kirghiz" ),
			new ISO639Language( "kmb",     null,     "Kimbundu" ),
			new ISO639Language( "kok",     null,     "Konkani" ),
			new ISO639Language( "kom",     "kv",     "Komi" ),
			new ISO639Language( "kon",     "kg",     "Kongo" ),
			new ISO639Language( "kor",     "ko",     "Korean" ),
			new ISO639Language( "kos",     null,     "Kosraean" ),
			new ISO639Language( "kpe",     null,     "Kpelle" ),
			new ISO639Language( "krc",     null,     "Karachay-Balkar" ),
			new ISO639Language( "krl",     null,     "Karelian" ),
			new ISO639Language( "kro",     null,     "Kru" ),
			new ISO639Language( "kru",     null,     "Kurukh" ),
			new ISO639Language( "kua",     "kj",     "Kuanyama; Kwanyama" ),
			new ISO639Language( "kum",     null,     "Kumyk" ),
			new ISO639Language( "kur",     "ku",     "Kurdish" ),
			new ISO639Language( "kut",     null,     "Kutenai" ),
			new ISO639Language( "lad",     null,     "Ladino" ),
			new ISO639Language( "lah",     null,     "Lahnda" ),
			new ISO639Language( "lam",     null,     "Lamba" ),
			new ISO639Language( "lao",     "lo",     "Lao" ),
			new ISO639Language( "lat",     "la",     "Latin" ),
			new ISO639Language( "lav",     "lv",     "Latvian" ),
			new ISO639Language( "lez",     null,     "Lezghian" ),
			new ISO639Language( "lim",     "li",     "Limburgan; Limburger; Limburgish" ),
			new ISO639Language( "lin",     "ln",     "Lingala" ),
			new ISO639Language( "lit",     "lt",     "Lithuanian" ),
			new ISO639Language( "lol",     null,     "Mongo" ),
			new ISO639Language( "loz",     null,     "Lozi" ),
			new ISO639Language( "ltz",     "lb",     "Luxembourgish; Letzeburgesch" ),
			new ISO639Language( "lua",     null,     "Luba-Lulua" ),
			new ISO639Language( "lub",     "lu",     "Luba-Katanga" ),
			new ISO639Language( "lug",     "lg",     "Ganda" ),
			new ISO639Language( "lui",     null,     "Luiseno" ),
			new ISO639Language( "lun",     null,     "Lunda" ),
			new ISO639Language( "luo",     null,     "Luo (Kenya and Tanzania)" ),
			new ISO639Language( "lus",     null,     "Lushai" ),
			new ISO639Language( "mad",     null,     "Madurese" ),
			new ISO639Language( "mag",     null,     "Magahi" ),
			new ISO639Language( "mah",     "mh",     "Marshallese" ),
			new ISO639Language( "mai",     null,     "Maithili" ),
			new ISO639Language( "mak",     null,     "Makasar" ),
			new ISO639Language( "mal",     "ml",     "Malayalam" ),
			new ISO639Language( "man",     null,     "Mandingo" ),
			new ISO639Language( "map",     null,     "Austronesian (Other)" ),
			new ISO639Language( "mar",     "mr",     "Marathi" ),
			new ISO639Language( "mas",     null,     "Masai" ),
			new ISO639Language( "may/msa", "ms",     "Malay" ), //dual codes
			new ISO639Language( "mdf",     null,     "Moksha" ),
			new ISO639Language( "mdr",     null,     "Mandar" ),
			new ISO639Language( "men",     null,     "Mende" ),
			new ISO639Language( "mga",     null,     "Irish, Middle (900-1200)" ),
			new ISO639Language( "mic",     null,     "Mi'kmaq; Micmac" ),
			new ISO639Language( "min",     null,     "Minangkabau" ),
			new ISO639Language( "mis",     null,     "Miscellaneous languages" ),
			new ISO639Language( "mac/mkd", "mk",     "Macedonian" ), //dual codes
			new ISO639Language( "mkh",     null,     "Mon-Khmer (Other)" ),
			new ISO639Language( "mlg",     "mg",     "Malagasy" ),
			new ISO639Language( "mlt",     "mt",     "Maltese" ),
			new ISO639Language( "mnc",     null,     "Manchu" ),
			new ISO639Language( "mni",     null,     "Manipuri" ),
			new ISO639Language( "mno",     null,     "Manobo languages" ),
			new ISO639Language( "moh",     null,     "Mohawk" ),
			new ISO639Language( "mol",     "mo",     "Moldavian" ),
			new ISO639Language( "mon",     "mn",     "Mongolian" ),
			new ISO639Language( "mos",     null,     "Mossi" ),
			new ISO639Language( "mao/mri", "mi",     "Maori" ), //dual codes
			new ISO639Language( "mul",     null,     "Multiple languages" ),
			new ISO639Language( "mun",     null,     "Munda languages" ),
			new ISO639Language( "mus",     null,     "Creek" ),
			new ISO639Language( "mwl",     null,     "Mirandese" ),
			new ISO639Language( "mwr",     null,     "Marwari" ),
			new ISO639Language( "myn",     null,     "Mayan languages" ),
			new ISO639Language( "myv",     null,     "Erzya" ),
			new ISO639Language( "nah",     null,     "Nahuatl" ),
			new ISO639Language( "nai",     null,     "North American Indian" ),
			new ISO639Language( "nap",     null,     "Neapolitan" ),
			new ISO639Language( "nau",     "na",     "Nauru" ),
			new ISO639Language( "nav",     "nv",     "Navajo; Navaho" ),
			new ISO639Language( "nbl",     "nr",     "Ndebele, South; South Ndebele" ),
			new ISO639Language( "nde",     "nd",     "Ndebele, North; North Ndebele" ),
			new ISO639Language( "ndo",     "ng",     "Ndonga" ),
			new ISO639Language( "nds",     null,     "Low German; Low Saxon; German, Low; Saxon, Low" ),
			new ISO639Language( "nep",     "ne",     "Nepali" ),
			new ISO639Language( "new",     null,     "Newari; Nepal Bhasa" ),
			new ISO639Language( "nia",     null,     "Nias" ),
			new ISO639Language( "nic",     null,     "Niger-Kordofanian (Other)" ),
			new ISO639Language( "niu",     null,     "Niuean" ),
			new ISO639Language( "nno",     "nn",     "Norwegian Nynorsk; Nynorsk, Norwegian" ),
			new ISO639Language( "nob",     "nb",     "Norwegian Bokmål; Bokmål, Norwegian" ),
			new ISO639Language( "nog",     null,     "Nogai" ),
			new ISO639Language( "non",     null,     "Norse, Old" ),
			new ISO639Language( "nor",     "no",     "Norwegian" ),
			new ISO639Language( "nqo",     null,     "N'ko" ),
			new ISO639Language( "nso",     null,     "Northern Sotho, Pedi; Sepedi" ),
			new ISO639Language( "nub",     null,     "Nubian languages" ),
			new ISO639Language( "nwc",     null,     "Classical Newari; Old Newari; Classical Nepal Bhasa" ),
			new ISO639Language( "nya",     "ny",     "Chichewa; Chewa; Nyanja" ),
			new ISO639Language( "nym",     null,     "Nyamwezi" ),
			new ISO639Language( "nyn",     null,     "Nyankole" ),
			new ISO639Language( "nyo",     null,     "Nyoro" ),
			new ISO639Language( "nzi",     null,     "Nzima" ),
			new ISO639Language( "oci",     "oc",     "Occitan (post 1500); Provençal" ),
			new ISO639Language( "oji",     "oj",     "Ojibwa" ),
			new ISO639Language( "ori",     "or",     "Oriya" ),
			new ISO639Language( "orm",     "om",     "Oromo" ),
			new ISO639Language( "osa",     null,     "Osage" ),
			new ISO639Language( "oss",     "os",     "Ossetian; Ossetic" ),
			new ISO639Language( "ota",     null,     "Turkish, Ottoman (1500-1928)" ),
			new ISO639Language( "oto",     null,     "Otomian languages" ),
			new ISO639Language( "paa",     null,     "Papuan (Other)" ),
			new ISO639Language( "pag",     null,     "Pangasinan" ),
			new ISO639Language( "pal",     null,     "Pahlavi" ),
			new ISO639Language( "pam",     null,     "Pampanga" ),
			new ISO639Language( "pan",     "pa",     "Panjabi; Punjabi" ),
			new ISO639Language( "pap",     null,     "Papiamento" ),
			new ISO639Language( "pau",     null,     "Palauan" ),
			new ISO639Language( "peo",     null,     "Persian, Old (ca.600-400 B.C.)" ),
			new ISO639Language( "per/fas", "fa",     "Persian" ), //dual codes
			new ISO639Language( "phi",     null,     "Philippine (Other)" ),
			new ISO639Language( "phn",     null,     "Phoenician" ),
			new ISO639Language( "pli",     "pi",     "Pali" ),
			new ISO639Language( "pol",     "pl",     "Polish" ),
			new ISO639Language( "pon",     null,     "Pohnpeian" ),
			new ISO639Language( "por",     "pt",     "Portuguese" ),
			new ISO639Language( "pra",     null,     "Prakrit languages" ),
			new ISO639Language( "pro",     null,     "Provençal, Old (to 1500)" ),
			new ISO639Language( "pus",     "ps",     "Pushto" ),
			                //{ "qaa-qtz", null,     "Reserved for local use" },
			new ISO639Language( "que",     "qu",     "Quechua" ),
			new ISO639Language( "raj",     null,     "Rajasthani" ),
			new ISO639Language( "rap",     null,     "Rapanui" ),
			new ISO639Language( "rar",     null,     "Rarotongan" ),
			new ISO639Language( "roa",     null,     "Romance (Other)" ),
			new ISO639Language( "roh",     "rm",     "Raeto-Romance" ),
			new ISO639Language( "rom",     null,     "Romany" ),
			new ISO639Language( "rum/ron", "ro",     "Romanian" ), //dual codes
			new ISO639Language( "run",     "rn",     "Rundi" ),
			new ISO639Language( "rup",     null,     "Aromanian; Arumanian; Macedo-Romanian" ),
			new ISO639Language( "rus",     "ru",     "Russian" ),
			new ISO639Language( "sad",     null,     "Sandawe" ),
			new ISO639Language( "sag",     "sg",     "Sango" ),
			new ISO639Language( "sah",     null,     "Yakut" ),
			new ISO639Language( "sai",     null,     "South American Indian (Other)" ),
			new ISO639Language( "sal",     null,     "Salishan languages" ),
			new ISO639Language( "sam",     null,     "Samaritan Aramaic" ),
			new ISO639Language( "san",     "sa",     "Sanskrit" ),
			new ISO639Language( "sas",     null,     "Sasak" ),
			new ISO639Language( "sat",     null,     "Santali" ),
			new ISO639Language( "scn",     null,     "Sicilian" ),
			new ISO639Language( "sco",     null,     "Scots" ),
			new ISO639Language( "scr/hrv", "hr",     "Croatian" ), //dual codes
			new ISO639Language( "sel",     null,     "Selkup" ),
			new ISO639Language( "sem",     null,     "Semitic (Other)" ),
			new ISO639Language( "sga",     null,     "Irish, Old (to 900)" ),
			new ISO639Language( "sgn",     null,     "Sign Languages" ),
			new ISO639Language( "shn",     null,     "Shan" ),
			new ISO639Language( "sid",     null,     "Sidamo" ),
			new ISO639Language( "sin",     "si",     "Sinhala; Sinhalese" ),
			new ISO639Language( "sio",     null,     "Siouan languages" ),
			new ISO639Language( "sit",     null,     "Sino-Tibetan (Other)" ),
			new ISO639Language( "sla",     null,     "Slavic (Other)" ),
			new ISO639Language( "slo/slk", "sk",     "Slovak" ), //dual codes
			new ISO639Language( "slv",     "sl",     "Slovenian" ),
			new ISO639Language( "sma",     null,     "Southern Sami" ),
			new ISO639Language( "sme",     "se",     "Northern Sami" ),
			new ISO639Language( "smi",     null,     "Sami languages (Other)" ),
			new ISO639Language( "smj",     null,     "Lule Sami" ),
			new ISO639Language( "smn",     null,     "Inari Sami" ),
			new ISO639Language( "smo",     "sm",     "Samoan" ),
			new ISO639Language( "sms",     null,     "Skolt Sami" ),
			new ISO639Language( "sna",     "sn",     "Shona" ),
			new ISO639Language( "snd",     "sd",     "Sindhi" ),
			new ISO639Language( "snk",     null,     "Soninke" ),
			new ISO639Language( "sog",     null,     "Sogdian" ),
			new ISO639Language( "som",     "so",     "Somali" ),
			new ISO639Language( "son",     null,     "Songhai" ),
			new ISO639Language( "sot",     "st",     "Sotho, Southern" ),
			new ISO639Language( "spa",     "es",     "Spanish; Castilian" ),
			new ISO639Language( "srd",     "sc",     "Sardinian" ),
			new ISO639Language( "srn",     null,     "Sranan Togo" ),
			new ISO639Language( "scc/srp", "sr",     "Serbian" ), //dual codes
			new ISO639Language( "srr",     null,     "Serer" ),
			new ISO639Language( "ssa",     null,     "Nilo-Saharan (Other)" ),
			new ISO639Language( "ssw",     "ss",     "Swati" ),
			new ISO639Language( "suk",     null,     "Sukuma" ),
			new ISO639Language( "sun",     "su",     "Sundanese" ),
			new ISO639Language( "sus",     null,     "Susu" ),
			new ISO639Language( "sux",     null,     "Sumerian" ),
			new ISO639Language( "swa",     "sw",     "Swahili" ),
			new ISO639Language( "swe",     "sv",     "Swedish" ),
			new ISO639Language( "syr",     null,     "Syriac" ),
			new ISO639Language( "tah",     "ty",     "Tahitian" ),
			new ISO639Language( "tai",     null,     "Tai (Other)" ),
			new ISO639Language( "tam",     "ta",     "Tamil" ),
			new ISO639Language( "tat",     "tt",     "Tatar" ),
			new ISO639Language( "tel",     "te",     "Telugu" ),
			new ISO639Language( "tem",     null,     "Timne" ),
			new ISO639Language( "ter",     null,     "Tereno" ),
			new ISO639Language( "tet",     null,     "Tetum" ),
			new ISO639Language( "tgk",     "tg",     "Tajik" ),
			new ISO639Language( "tgl",     "tl",     "Tagalog" ),
			new ISO639Language( "tha",     "th",     "Thai" ),
			new ISO639Language( "tib/bod", "bo",     "Tibetan" ), //dual codes
			new ISO639Language( "tig",     null,     "Tigre" ),
			new ISO639Language( "tir",     "ti",     "Tigrinya" ),
			new ISO639Language( "tiv",     null,     "Tiv" ),
			new ISO639Language( "tkl",     null,     "Tokelau" ),
			new ISO639Language( "tlh",     null,     "Klingon; tlhIngan-Hol" ),
			new ISO639Language( "tli",     null,     "Tlingit" ),
			new ISO639Language( "tmh",     null,     "Tamashek" ),
			new ISO639Language( "tog",     null,     "Tonga (Nyasa)" ),
			new ISO639Language( "ton",     "to",     "Tonga (Tonga Islands)" ),
			new ISO639Language( "tpi",     null,     "Tok Pisin" ),
			new ISO639Language( "tsi",     null,     "Tsimshian" ),
			new ISO639Language( "tsn",     "tn",     "Tswana" ),
			new ISO639Language( "tso",     "ts",     "Tsonga" ),
			new ISO639Language( "tuk",     "tk",     "Turkmen" ),
			new ISO639Language( "tum",     null,     "Tumbuka" ),
			new ISO639Language( "tup",     null,     "Tupi languages" ),
			new ISO639Language( "tur",     "tr",     "Turkish" ),
			new ISO639Language( "tut",     null,     "Altaic (Other)" ),
			new ISO639Language( "tvl",     null,     "Tuvalu" ),
			new ISO639Language( "twi",     "tw",     "Twi" ),
			new ISO639Language( "tyv",     null,     "Tuvinian" ),
			new ISO639Language( "udm",     null,     "Udmurt" ),
			new ISO639Language( "uga",     null,     "Ugaritic" ),
			new ISO639Language( "uig",     "ug",     "Uighur; Uyghur" ),
			new ISO639Language( "ukr",     "uk",     "Ukrainian" ),
			new ISO639Language( "umb",     null,     "Umbundu" ),
			new ISO639Language( "und",     null,     "Undetermined" ),
			new ISO639Language( "urd",     "ur",     "Urdu" ),
			new ISO639Language( "uzb",     "uz",     "Uzbek" ),
			new ISO639Language( "vai",     null,     "Vai" ),
			new ISO639Language( "ven",     "ve",     "Venda" ),
			new ISO639Language( "vie",     "vi",     "Vietnamese" ),
			new ISO639Language( "vol",     "vo",     "Volapük" ),
			new ISO639Language( "vot",     null,     "Votic" ),
			new ISO639Language( "wak",     null,     "Wakashan languages" ),
			new ISO639Language( "wal",     null,     "Walamo" ),
			new ISO639Language( "war",     null,     "Waray" ),
			new ISO639Language( "was",     null,     "Washo" ),
			new ISO639Language( "wel/cym", "cy",     "Welsh" ), // //dual codes
			new ISO639Language( "wen",     null,     "Sorbian languages" ),
			new ISO639Language( "wln",     "wa",     "Walloon" ),
			new ISO639Language( "wol",     "wo",     "Wolof" ),
			new ISO639Language( "xal",     null,     "Kalmyk; Oirat" ),
			new ISO639Language( "xho",     "xh",     "Xhosa" ),
			new ISO639Language( "yao",     null,     "Yao" ),
			new ISO639Language( "yap",     null,     "Yapese" ),
			new ISO639Language( "yid",     "yi",     "Yiddish" ),
			new ISO639Language( "yor",     "yo",     "Yoruba" ),
			new ISO639Language( "ypk",     null,     "Yupik languages" ),
			new ISO639Language( "zap",     null,     "Zapotec" ),
			new ISO639Language( "zen",     null,     "Zenaga" ),
			new ISO639Language( "zha",     "za",     "Zhuang; Chuang" ),
			new ISO639Language( "chi/zho", "zh",     "Chinese" ), //dual codes
			new ISO639Language( "znd",     null,     "Zande" ),
			new ISO639Language( "zul",     "zu",     "Zulu" ),
			new ISO639Language( "zun",     null,     "Zuni" ),
			new ISO639Language( "zxx",     null,     "No linguistic content" )
		};
		public static readonly ISO639Language[] KnownLanguageList;
		public static readonly IDictionary<string, ISO639Language> KnownLanguages;
	}
}
