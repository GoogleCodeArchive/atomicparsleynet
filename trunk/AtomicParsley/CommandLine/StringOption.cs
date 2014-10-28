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
using System.Collections.Generic;
using System.Linq;

namespace AtomicParsley.CommandLine
{
	[System.Diagnostics.DebuggerDisplay("--{Name,nq} {Template,nq}")]
	public class StringOption : ArgOption
	{
		bool? hasValue;

		public StringOption(string name, char shortName, string template, bool required = false, bool optionalValue = false)
			: this(name, new string(shortName, 1), template, required, optionalValue) { }

		public StringOption(string name, string template, bool required = false, bool optionalValue = false)
			: this(name, "", template, required, optionalValue) { }

		public StringOption(string name, string shortName, string template, bool required = false, bool optionalValue = false)
			: base(name, shortName, template, required)
		{
			if (!optionalValue) hasValue = true;
		}

		public string Value { get; private set; }

		internal override bool IsUnique { get { return true; } }
		internal override bool? HasValue { get { return hasValue; } }

		internal override int ParseArgument(IEnumerable<string> args)
		{
			Value = args.FirstOrDefault();
			return 2;
		}
	}
}
