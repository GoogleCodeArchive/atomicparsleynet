//------------------------------------------------------------------------------
// Copyright © FRA & FV 2013
// All rights reserved
//------------------------------------------------------------------------------
// ID3 tags Framework
//
// SVN revision information:
//   $Revision$
//------------------------------------------------------------------------------
using System;
using System.Linq;

namespace ID3v2
{
	public struct TagID
	{
		private string id;

		public string Name { get { return this.id; } }
		public byte[] Data { get { return Code2Data(this.id); } }

		public bool IsEmpty { get { return String.IsNullOrEmpty(this.id); } }
		public bool NonConformance
		{
			get
			{
				if (this.id == null) return false;
				return this.id.Any(c => !(c >= '0' && c <= '9' || c >= 'A' && c <= 'Z'));
			}
		}

		public TagID(byte[] data)
		{
			this.id = Data2Code(data);
		}

		#region Conversions
		private static string Data2Code(byte[] data)
		{
			switch (data.Length)
			{
			case 0:
				return String.Empty;
			case 3:
				return BitConverter.ToUInt16(data, 0) == 0 && data[2] == 0 ? String.Empty :
					new String(new char[] { (char)data[0], (char)data[1], (char)data[2] });
			case 4:
				return BitConverter.ToUInt32(data, 0) == 0 ? String.Empty :
					new String(new char[] { (char)data[0], (char)data[1], (char)data[2], (char)data[3] });
			default:
				throw new ArgumentOutOfRangeException("data");
			}
		}

		public static byte[] Code2Data(string code)
		{
			if (String.IsNullOrEmpty(code)) return new byte[0];
			switch (code.Length)
			{
			case 3:
				return new byte[] { (byte)code[0], (byte)code[1], (byte)code[2] };
			case 4:
				return new byte[] { (byte)code[0], (byte)code[1], (byte)code[2], (byte)code[3] };
			default:
				throw new ArgumentOutOfRangeException("code");
			}
		}

		public static explicit operator string(TagID id)
		{
			return id.id;
		}

		public static explicit operator TagID(string other)
		{
			return new TagID { id = other };
		}

		public override string ToString()
		{
			return this.id;
		}
		#endregion

		#region Equals
		public static bool operator ==(TagID id, string other)
		{
			return id.Equals(other);
		}

		public static bool operator ==(TagID id, TagID other)
		{
			return id.id == other.id;
		}

		public static bool operator !=(TagID id, string other)
		{
			return !id.Equals(other);
		}

		public static bool operator !=(TagID id, TagID other)
		{
			return id.id != other.id;
		}

		public override bool Equals(object obj)
		{
			if (obj is TagID)
				return this.id == ((TagID)obj).id;
			return String.Equals(this.id, obj as string);
		}

		public override int GetHashCode()
		{
			return (this.id ?? "").GetHashCode();
		}
		#endregion
	}
}
