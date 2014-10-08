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
	public abstract class FixedScale
	{
		internal FixedScale() { }
	}
	public sealed class x8 : FixedScale
	{
		private x8() { }
	}
	public sealed class x16 : FixedScale
	{
		private x16() { }
	}
	public sealed class x24 : FixedScale
	{
		private x24() { }
	}
	public sealed class x30 : FixedScale
	{
		private x30() { }
	}
	public sealed class x32 : FixedScale
	{
		private x32() { }
	}

	public struct Fixed<TStorage, TScale>
		where TStorage: struct
		where TScale : FixedScale
	{
		private static int scale;
		private static ulong one;
		private ulong value;

		static Fixed()
		{
			scale = int.Parse(typeof(TScale).Name.Substring(1));
			one = 1Lu << scale;
			One = new Fixed<TStorage, TScale> { value = one };
			Zero = new Fixed<TStorage, TScale> { value = 0 };
		}

		public Fixed(TStorage data)
		{
			this.value = Convert.ToUInt64(data);
		}

		public TStorage Data { get { return (TStorage)Convert.ChangeType(value, typeof(TStorage)); } }
		public static int Scale { get { return scale; } }

		public static readonly Fixed<TStorage, TScale> One;
		public static readonly Fixed<TStorage, TScale> Zero;

		#region Explicit cast
		public static explicit operator TStorage(Fixed<TStorage, TScale> value)
		{
			return value.Data;
		}

		public static explicit operator Fixed<TStorage, TScale>(TStorage value)
		{
			return new Fixed<TStorage, TScale>(value);
		}

		public static explicit operator int(Fixed<TStorage, TScale> value)
		{
			return (int)value.ToInt64();
		}

		public static explicit operator long(Fixed<TStorage, TScale> value)
		{
			return value.ToInt64();
		}

		public static explicit operator float(Fixed<TStorage, TScale> value)
		{
			return (float)value.ToDouble();
		}

		public static explicit operator double(Fixed<TStorage, TScale> value)
		{
			return value.ToDouble();
		}

		public static implicit operator Fixed<TStorage, TScale>(int value)
		{
			return ToFixed(value);
		}

		public static implicit operator Fixed<TStorage, TScale>(long value)
		{
			return ToFixed(value);
		}

		public static explicit operator Fixed<TStorage, TScale>(float value)
		{
			return ToFixed(value);
		}

		public static explicit operator Fixed<TStorage, TScale>(double value)
		{
			return ToFixed(value);
		}
		#endregion

		#region Convert value
		public long ToInt64()
		{
			return (long)(value + (one >> 1)) >> scale;
		}

		public double ToDouble()
		{
			return (double)value / one;
		}

		public static Fixed<TStorage, TScale> ToFixed(long value)
		{
			return new Fixed<TStorage, TScale> { value = (ulong)value << scale };
		}

		public static Fixed<TStorage, TScale> ToFixed(double value)
		{
			return new Fixed<TStorage, TScale>((TStorage)Convert.ChangeType(value * one, typeof(TStorage)));
		}
		#endregion

		#region Base implementation
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is Fixed<TStorage, TScale>)
				return this.value == ((Fixed<TStorage, TScale>)obj).value;
			return false;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override string ToString()
		{
			return ToDouble().ToString();
		}

		public static bool operator ==(Fixed<TStorage, TScale> value1, Fixed<TStorage, TScale> value2)
		{
			return value1.value == value2.value;
		}

		public static bool operator !=(Fixed<TStorage, TScale> value1, Fixed<TStorage, TScale> value2)
		{
			return value1.value != value2.value;
		}
		#endregion
	}
}
