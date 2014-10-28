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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtomicParsley.CommandLine
{
	public abstract class Option
	{
		protected Option(string name, string shortName, bool required)
		{
			if (name.Any(character => !(Char.IsLetter(character) || Char.IsDigit(character) ||
				character == '?' || character == '-' || character == ' ')) || String.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Names must consist of letters.", "name");
			if (shortName.Any(character => !(Char.IsLetter(character) || Char.IsDigit(character) ||
				character == '?' || character == '-' || character == ' ')))
				throw new ArgumentException("Names must consist of letters.", "shorts");
			this.Names = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			this.ShortNames = shortName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			this.IsRequired = required;
		}

		public string Description { get; set; }
		public string Help { get; set; }
		public string Remarks { get; set; }
		public virtual bool IsPresent { get; internal set; }
		public bool IsRequired { get; private set; }
		public string[] Names { get; private set; }
		public string[] ShortNames { get; private set; }
		public string Name { get { return Names[0]; } }
		public string ShortName { get { return ShortNames.FirstOrDefault(); } }

		public static implicit operator bool(Option option)
		{
			return option.IsPresent;
		}

		internal abstract bool IsUnique { get; }
		internal abstract bool? HasValue { get; }

		internal abstract int ParseArgument(IEnumerable<string> args);

		internal abstract void WriteDescription(TextWriter writer);

		internal abstract void WriteHelp(TextWriter writer);
	}
}
