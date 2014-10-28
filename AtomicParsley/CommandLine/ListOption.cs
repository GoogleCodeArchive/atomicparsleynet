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
	[System.Diagnostics.DebuggerDisplay("--{Name,nq} {Template,nq} [...]")]
	public sealed class ListOption : ArgOption
	{
		private List<string> list = new List<string>();

		public ListOption(string name, char shortName, string template, bool required = false)
			: base(name, new string(shortName, 1), template, required) { }

		public ListOption(string name, string template, bool required = false)
			: base(name, "", template, required) { }

		public ListOption(string name, string shortName, string template, bool required = false)
			: base(name, shortName, template, required) { }

		public string[] Value { get { return list.ToArray(); } }

		internal override bool IsUnique { get { return false; } }

		internal override int ParseArgument(IEnumerable<string> args)
		{
			list.Add(args.FirstOrDefault());
			return 2;
		}
	}
}
