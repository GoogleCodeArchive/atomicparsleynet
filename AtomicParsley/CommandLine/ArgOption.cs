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
using System.IO;

namespace AtomicParsley.CommandLine
{
	public abstract class ArgOption : Option
	{
		public ArgOption(string name, string shortName, string template, bool required = false)
			: base(name, shortName, required)
		{
			this.Template = template;
		}

		public string Template { get; private set; }

		internal override bool? HasValue { get { return true; } }

		internal override void WriteDescription(TextWriter writer)
		{
			if (Name.Length > 12)
				writer.Write("  --{0,-24}  ", Name + " " + Template);
			else
				writer.Write("  --{0,-12} {1,-11}  ", Name, Template);
		}

		internal override void WriteHelp(TextWriter writer)
		{
			writer.Write("  --{0,-16} ", Name);
			if (ShortNames.Length > 0)
				writer.Write(",  -{0,-2}  ", ShortNames[0]);
			else if (Template.Length > 7)
				writer.Write(new string(' ', Math.Max(1, 15 - Template.Length)));
			else
				writer.Write(new string(' ', 8));
			writer.Write("{0,-7}  ", Template);
		}
	}
}
