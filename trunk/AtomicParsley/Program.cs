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
using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using ID3v2;
using MP4;
using AtomicParsley.CommandLine;

namespace AtomicParsley
{
	partial class Program
	{
		private static readonly ILog log = Logger.GetLogger<Program>();
		public const int AtomicParsleyID3v2TagMajorVersion = 4;
		public const int AtomicParsleyID3v2TagRevisionVersion = 4;
		public const bool ID3v2TagFlagFooter = false; //bit4; MPEG-4 'ID32' requires this to be false
		public const bool ID3v2TagFlagExperimental = true; //bit5
		public const bool ID3v2TagFlagExtendedHeader = true; //bit6
		public const bool ID3v2TagFlagUnsyncronization = false; //bit7

		static OptionCollection HelpOptions = new OptionCollection
		{
			optHelp, optLongHelp, opt3GPHelp, isoHelp, optFileHelp, optUUIDHelp, optReverseDNSHelp, optID3Help
		};

		static OptionCollection InfoOptions = new OptionCollection
		{
			optGenreList, optStikList, optLanguageList, optMacLanguageList, optRatingsList, optGenreMovieIDList,
			optGenreTVIDList, optID3FramesList, optImageTypeList
		};

		static OptionCollection ImplementedOptions = new OptionCollection
		{
			optVersion,
			optHelp, optLongHelp,
			//opt3GPHelp,
			//isoHelp,
			//optFileHelp,
			//optUUIDHelp,
			//optReverseDNSHelp,
			//optID3Help,
			optGenreList, optStikList, optLanguageList, optMacLanguageList, optRatingsList, optGenreMovieIDList,
			optGenreTVIDList, optImageTypeList, //optID3FramesList,
		};

		static void Main(string[] args)
		{
			var stdout = new WindowTextWriter();
			if (args.Length == 0)
			{
				WriteShortHelp(stdout);
				return;
			}
			if (!ImplementedOptions.ParseArguments(args))
				return;
			if (args.Length == 1) {
				if (optVersion) { ShowVersionInfo(stdout); return; }
				else if (optHelp) { WriteShortHelp(stdout); return; }
				else if (optLongHelp) { WriteLongHelp(stdout); return; }
				else if (opt3GPHelp) { Write3GPHelp(stdout); return; }
				else if (isoHelp) { WriteISOHelp(stdout); return; }
				else if (optFileHelp) { WriteFileLevelHelp(stdout); return; }
				else if (optUUIDHelp) { WriteUUIDHelp(stdout); return; }
				else if (optReverseDNSHelp) { WriteRDNSHelp(stdout); return; }
				else if (optID3Help) { WriteID3Help(stdout); return; }
			}
			if (InfoOptions.Count(opt => opt.IsPresent) == args.Length)
			{
				if (optGenreList) Catalog.ListGenresValues(stdout);
				if (optLanguageList) Catalog.ListLanguageCodes(stdout);
				if (optStikList) Catalog.ListStikValues(stdout);
				if (optRatingsList) Catalog.ListMediaRatings(stdout);
				if (optGenreMovieIDList) Catalog.ListMovieGenreIDValues(stdout);
				if (optGenreTVIDList) Catalog.ListTVGenreIDValues(stdout);
				if (optImageTypeList) Catalog.ListImagTypeStrings(stdout);
				if (optMacLanguageList) Catalog.ListMacLanguageCodes(stdout);
				return;
			}
			if (InfoOptions.Any(opt => opt.IsPresent) || HelpOptions.Any(opt => opt.IsPresent))
			{
				log.Error("too many options");
				return;
			}
			if (ImplementedOptions.UnusedArguments.Length == 0)
				return;

			string fn = ImplementedOptions.UnusedArguments[0];
			if (String.Compare(Path.GetExtension(fn), ".mp3", true) == 0)
			{
				ID3v2Tag tag;
				using (var fs = new FileStream(fn, FileMode.Open))
				{
					tag = ID3v2Tag.Create(fs, new ID3v2Tag.SerializationOptions
						{
							//MajorVersion = AtomicParsleyID3v2TagMajorVersion,
							//RevisionVersion = AtomicParsleyID3v2TagRevisionVersion
						});
				}

				var writer = new XmlSerializer(typeof(ID3v2Tag));
				using (var fs = new FileStream(Path.ChangeExtension(fn, ".xml"), FileMode.Create))
				{
					tag.XMLSerializerOptions = new ID3v2Tag.XMLSerializationOptions
						{
							DisplayHint = true
						};
					writer.Serialize(fs, tag, ID3v2Tag.DefaultXMLNamespaces);
				}

				using (var fs = new FileStream(Path.ChangeExtension(args[0], ".id3"), FileMode.Create))
				{
					int size = tag.GetTagSize();
					Console.Out.WriteLine("Real tag size is {0} byte(s)", size);
					//tag.TotalSize = size;
					tag.Write(fs);
				}
			}
			else
			{
				Container mp4;
				using (var fs = new FileStream(fn, FileMode.Open))
				{
					mp4 = Container.Create(fs);
				}

				try
				{
					var writer = new XmlSerializer(typeof(Container));
					using (var fs = new FileStream(Path.ChangeExtension(fn, ".xml"), FileMode.Create))
					{
						writer.Serialize(fs, mp4, Container.DefaultXMLNamespaces);
					}
				}
				catch (InvalidOperationException ex)
				{
					Exception e = ex;
					Console.ForegroundColor = ConsoleColor.Yellow;
					while (e != null)
					{
						Console.Error.WriteLine(e.Message);
						e = e.InnerException;
					}
					Console.ResetColor();
					Console.ReadKey();
				}
			}
		}
	}
}
