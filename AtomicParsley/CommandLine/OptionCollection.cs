//------------------------------------------------------------------------------
// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
//------------------------------------------------------------------------------
// AtomicParsley
//
// SVN revision information:
//   $Revision$
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtomicParsley.CommandLine
{
	public sealed class OptionCollection : ICollection<Option>
	{
		private static readonly ILog log = Logger.GetLogger<OptionCollection>();
		private Lazy<Dictionary<string, Option[]>> nameMap;
		private Lazy<Dictionary<string, Option[]>> shortMap;
		private List<Option> options = new List<Option>();
		private List<string> nonoptions = new List<string>();

		private void InitMap()
		{
			if (nameMap == null || nameMap.IsValueCreated)
				nameMap = new Lazy<Dictionary<string, Option[]>>(() =>
					options.SelectMany(opt => opt.Names
						.Select(name => new KeyValuePair<string, Option>(name, opt)))
					.GroupBy(key => key.Key, key => key.Value)
					.ToDictionary(group => group.Key, group => group.ToArray()));
			if (shortMap == null || shortMap.IsValueCreated)
				shortMap = new Lazy<Dictionary<string, Option[]>>(() =>
					options.SelectMany(opt => opt.ShortNames
						.Select(name => new KeyValuePair<string, Option>(name, opt)))
					.GroupBy(key => key.Key, key => key.Value)
					.ToDictionary(group => group.Key, group => group.ToArray()));
		}

		public OptionCollection()
		{
			InitMap();
		}

		public int Count { get { return (options.Count); } }

		public string[] UnusedArguments
		{
			get { return nonoptions.ToArray(); }
		}

		#region ICollection implementation

		bool ICollection<Option>.IsReadOnly { get { return false; } }

		public Option this[string name]
		{
			get
			{
				Option[] options;
				if (nameMap.Value.TryGetValue(name, out options) && options.Length == 1)
					return options[0];
				else
					return null;
			}
		}

		public void Add(Option option)
		{
			if (option == null) return;//throw new ArgumentNullException("option");
			options.Add(option);
			InitMap();
		}

		public void Clear()
		{
			options.Clear();
			InitMap();
		}

		public bool Contains(Option option)
		{
			return options.Contains(option);
		}

		public bool Contains(params Option[] option)
		{
			return option.Any(opt => options.Contains(opt));
		}

		public bool ContainsAll(params Option[] option)
		{
			return !option.Any(opt => !options.Contains(opt));
		}

		void ICollection<Option>.CopyTo(Option[] array, int startIndex)
		{
			options.CopyTo(array, startIndex);
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator<Option> IEnumerable<Option>.GetEnumerator()
		{
			return options.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return options.GetEnumerator();
		}

		#endregion

		public bool Remove(Option option)
		{
			int index = options.IndexOf(option);
			if (index < 0) return (false);
			options.RemoveAt(index);
			InitMap();
			return true;
		}

		private void WriteLineIndent(TextWriter writer, int indent, string line)
		{
			if (writer != Console.Out)
			{
				writer.WriteLine(line);
				return;
			}

			int bufWidth = Console.BufferWidth;
			int scrWidth = Console.WindowWidth;

		}

		/// <summary>
		/// Print help
		/// </summary>
		/// <param name="writer"></param>
		public void WriteOptionSummary(TextWriter writer)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			foreach (Option option in options)
			{
				writer.SetIndent(30);
				option.WriteDescription(writer);
				writer.WriteLine(option.Description);
			}
		}

		/// <summary>
		/// Print help
		/// </summary>
		/// <param name="writer"></param>
		public void WriteOptionHelp(TextWriter writer)
		{
			var scrWriter = writer as WindowTextWriter;
			if (writer == null) throw new ArgumentNullException("writer");
			foreach (Option option in options)
			{
				writer.SetIndent(38);
				option.WriteHelp(writer);
				writer.WriteLine(option.Help);
				if (String.IsNullOrEmpty(option.Remarks)) continue;
				writer.SetIndent(4);
				writer.Write("    ");
				writer.WriteLine(option.Remarks);
			}
		}

		/// <summary>
		/// Parse arguments -- the main show
		/// </summary>
		/// <param name="args"></param>
		/// <param name="results"></param>
		public bool ParseArguments(string[] args)
		{
			bool hasErrors = false;

			for (int optind = 0; optind < args.Length; optind++)
			{
				string arg = args[optind];
				if (arg.Length == 0) continue;
				if (arg.Length > 1 &&
					(arg.StartsWith("-", StringComparison.Ordinal) || arg.StartsWith("+", StringComparison.Ordinal)))
				{
					string value = "", opt;
					Option[] options;
					bool found;
					if (arg.Length > 2 && arg.StartsWith("--", StringComparison.Ordinal)) //long option
					{
						// option processing
						// find the named option
						int index = 1;
						while (index < arg.Length)
						{
							if (arg[index] == '=')
							{
								value = arg.Substring(index + 1);
								break;
							}
							index++;
						}
						opt = arg.Substring(0, index);
						found = nameMap.Value.TryGetValue(arg.Substring(2, index - 2), out options);
					}
					else if (shortMap.Value.TryGetValue(arg.Substring(1), out options))
					{
						opt = arg;
						found = true;
					}
					else //short option
					{
						// Look at and handle the next short option-character.
						if (arg.Length > 2)
						{
							//If we end this element by taking the rest as an arg,
							//we must advance to the next element now. 
							value = arg.Substring(2);
						}
						opt = arg.Substring(0, 2);
						found = shortMap.Value.TryGetValue(arg.Substring(1, 1), out options);
					}
					// invoke the appropriate logic
					if (found)
					{
						found = false;
						foreach (var option in options)
						{
							if (option.IsPresent && option.IsUnique)
							{
								log.Error("option `{0}' is ambiguous", opt);
								hasErrors = true;
								found = true;
								break;
							}
							var values = args.Skip(optind + 1).TakeWhile(a => a.Length < 2 ||
								!(a.StartsWith("-", StringComparison.Ordinal) || a.StartsWith("+", StringComparison.Ordinal)));
							if (value.Length > 0)
							{
								values = new string[] { value }.Concat(values);
							}
							if (values.Any())
							{
								if (value.Length > 0 && option.HasValue == false)
								{
									log.Error("option `{0}' doesn't allow an argument", opt);
									hasErrors = true;
									found = true;
									break;
								}
							}
							else
							{
								if (option.HasValue == true)
								{
									log.Error("option `{0}' requires an argument", opt);
									hasErrors = true;
									found = true;
									break;
								}
							}
							int count = option.ParseArgument(values);
							if (count > 0)
							{
								option.IsPresent = true;
								found = true;
								optind += count - 1;
								break;
							}
						}
						if (!found)
						{
							log.Error("invalid option `{0}'", opt);
							hasErrors = true;
						}
					}
					else
					{
						log.Error("unrecognized option `{0}'", opt);
						hasErrors = true;
					}
				}
				else if (arg[0] == '@')
				{
					string responseFile = arg.Substring(1);
					List<string> responses = new List<string>();
					using (TextReader reader = File.OpenText(responseFile))
					{
						while (true)
						{
							string response = reader.ReadLine();
							if (response == null) break;
							responses.Add(response);
						}
					}
					ParseArguments(responses.ToArray());
				}
				else
				{
					// non-option processing
					nonoptions.Add(arg);
				}
			}

			// make sure the required arguments were present
			if (this.options.Any(option => option.IsRequired && !option.IsPresent))
				return false;

			return !hasErrors;
		}

	}
}
