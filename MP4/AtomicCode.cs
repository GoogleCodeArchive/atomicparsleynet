//------------------------------------------------------------------------------
// Copyright © FRA & FV 2014
// All rights reserved
//------------------------------------------------------------------------------
// MPEG-4 boxes Framework
//
// SVN revision information:
//   $Revision$
//------------------------------------------------------------------------------
using System;

namespace MP4
{
	public struct AtomicCode
	{
		private uint code;

		public bool IsEmpty { get { return this.code == 0u; } }

		public AtomicCode(string name)
		{
			this.code = AtomName2ID(name);
		}

		#region Conversions
		private static string UIntBE2Code(uint data)
		{
			if (data <= 0xFFFFu) return String.Empty;
			return new String(new char[] {
				(char)((data & 0xFF000000u) >> 24),
				(char)((data & 0xFF0000u) >> 16),
				(char)((data & 0xFF00u) >> 8),
				(char)(data & 0xFFu)});
		}

		private static uint Code2UIntBE(string code)
		{
			if (String.IsNullOrEmpty(code)) return 0U;
			if (code.Length != 4)
				throw new ArgumentOutOfRangeException("code.Length");
			return
				((uint)code[0] << 24) |
				((uint)code[1] << 16) |
				((uint)code[2] << 8) |
				(uint)code[3];
		}

		private static string AtomID2Name(uint id)
		{
			if (id == 0u)
				return String.Empty;
			if (id <= 0xFFFFu)
				return id.ToString();
			return UIntBE2Code(id).Trim();
		}

		private static uint AtomName2ID(string name)
		{
			if (String.IsNullOrEmpty(name))
				return 0u;
			ushort id;
			if (ushort.TryParse(name, out id))
				return id;
			return Code2UIntBE((name).PadRight(4, ' '));
		}

		public static explicit operator string(AtomicCode code)
		{
			return UIntBE2Code(code.code);
		}

		public static explicit operator uint(AtomicCode code)
		{
			return code.code;
		}

		public static explicit operator AtomicCode(string code)
		{
			return new AtomicCode { code = Code2UIntBE(code) };
		}

		public static explicit operator AtomicCode(uint code)
		{
			return new AtomicCode { code = code };
		}

		public override string ToString()
		{
			return AtomID2Name(this.code);
		}
		#endregion

		#region Equals
		public static bool operator ==(AtomicCode code, string other)
		{
			return code.Equals(other);
		}

		public static bool operator ==(AtomicCode code, uint other)
		{
			return code.code == other;
		}

		public static bool operator ==(AtomicCode code, AtomicCode other)
		{
			return code.code == other.code;
		}

		public static bool operator !=(AtomicCode code, string other)
		{
			return !code.Equals(other);
		}

		public static bool operator !=(AtomicCode code, uint other)
		{
			return code.code != other;
		}

		public static bool operator !=(AtomicCode code, AtomicCode other)
		{
			return code.code != other.code;
		}

		public override bool Equals(object obj)
		{
			if (obj is AtomicCode)
				return this.code == ((AtomicCode)obj).code;
			if (obj is uint)
				return this.code == (uint)obj;
			return String.Equals(UIntBE2Code(code), obj as string);
		}

		public override int GetHashCode()
		{
			return this.code.GetHashCode();
		}
		#endregion
	}
}
