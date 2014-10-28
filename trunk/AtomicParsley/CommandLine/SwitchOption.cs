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
using System.IO;

namespace AtomicParsley.CommandLine
{
	[System.Diagnostics.DebuggerDisplay("--{Name,nq}")]
	public sealed class SwitchOption : Option
	{
		public SwitchOption(string name, char shortName, bool required = false)
			: this(name, new string(shortName, 1), required) { }

		public SwitchOption(string name, bool required = false)
			: this(name, "", required) { }

		public SwitchOption(string name, string shortName, bool required = false)
			: base(name, shortName, required) { }

		internal override bool IsUnique { get { return true; } }
		internal override bool? HasValue { get { return false; } }

		internal override int ParseArgument(IEnumerable<string> args)
		{
			return 1;
		}

		internal override void WriteDescription(TextWriter writer)
		{
			writer.Write("  --{0,-24}  ", Name);
		}

		internal override void WriteHelp(TextWriter writer)
		{
			writer.Write("  --{0,-16} ,", Name);
			if (ShortNames.Length == 0)
				writer.Write(new string(' ', 16));
			else
				writer.Write("  -{0,-11}  ", ShortName);
		}
	}
}
