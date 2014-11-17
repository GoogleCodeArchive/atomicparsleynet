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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;

namespace MP4
{
	/// <summary>
	/// Metadata style
	/// </summary>
	[Flags]
	public enum MetadataStyle
	{
		/// <summary>
		/// Undefined style
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// iTunes style
		/// </summary>
		iTunes = 0x100,
		/// <summary>
		/// 3gpp files prior to 3gp6
		/// </summary>
		ThirdGenPartner = 0x300,
		/// <summary>
		/// 3GPP Release6 the first spec to contain the complement of assets
		/// </summary>
		ThirdGenPartnerVer1Rel6 = 0x316,
		/// <summary>
		/// 3GPP Release7 introduces ID32 atoms
		/// </summary>
		ThirdGenPartnerVer1Rel7 = 0x317,
		/// <summary>
		/// 3gp2 files
		/// </summary>
		ThirdGenPartnerVer2 = 0x320,
		/// <summary>
		/// 3gp2 files, 3GPP2 C.S0050-A introduces 'gadi'
		/// </summary>
		ThirdGenPartnerVer2RelA = 0x321,
		/// <summary>
		/// Motion JPEG 2000
		/// </summary>
		MotionJPEG2000 = 0x400,
		/// <summary>
		/// PSP
		/// </summary>
		PSP = 0x800,
		/// <summary>
		/// Brand style mask
		/// </summary>
		Brand = 0x700
	}

	public static class ApTypes
	{
		/// <summary>
		/// no NULL termination
		/// </summary>
		public const int UTF8_iTunesStyle_256glyphLimited = 0;
		/// <summary>
		/// no NULL termination
		/// </summary>
		public const int UTF8_iTunesStyle_Unlimited = 1;
		/// <summary>
		/// no NULL termination, used in purl &amp; egid
		/// </summary>
		public const int UTF8_iTunesStyle_Binary = 3;
		/// <summary>
		/// terminated with a NULL uint8_t
		/// </summary>
		public const int UTF8_3GP_Style = 8;
		/// <summary>
		/// terminated with a NULL uint16_t
		/// </summary>
		public const int UTF16_3GP_Style = 16;

		private const uint MaxAtomID = 0xFFFFU;

		public static AtomicCode ReadAtomicCode(this BinaryReader reader)
		{
			return (AtomicCode)reader.ReadUInt32();
		}

		public static void Write(this BinaryWriter writer, AtomicCode code)
		{
			writer.Write((uint)code);
		}

		public static void ReadCount32(this BinaryReader reader, ICollection<AtomicInfo> list, AtomicInfo parent, bool required = true)
		{
			int count = reader.ReadInt32();
			for (int index = 0; index < count; index++)
			{
				var box = AtomicInfo.ParseBox(reader, parent, required);
				list.Add(box);
			}
		}

		public static long SizeCount32(this ICollection<AtomicInfo> list)
		{
			return 4L + list.Sum(box => box.GetBoxSize());
		}

		public static void WriteCount32(this BinaryWriter writer, ICollection<AtomicInfo> list)
		{
			writer.Write((int)list.Count);
			foreach (var box in list)
			{
				box.WriteBox(writer);
			}
		}

		public static void ReadCount16(this BinaryReader reader, ICollection<AtomicInfo> list, AtomicInfo parent, bool required = true)
		{
			int count = reader.ReadInt16();
			for (int index = 0; index < count; index++)
			{
				var box = AtomicInfo.ParseBox(reader, parent, required);
				list.Add(box);
			}
		}

		public static long SizeCount16(this ICollection<AtomicInfo> list)
		{
			return 2L + list.Sum(box => box.GetBoxSize());
		}

		public static void WriteCount16(this BinaryWriter writer, ICollection<AtomicInfo> list)
		{
			writer.Write((ushort)list.Count);
			foreach (var box in list)
			{
				box.WriteBox(writer);
			}
		}

		public static void ReadEnd(this BinaryReader reader, ICollection<AtomicInfo> list, AtomicInfo parent, bool required = false)
		{
			while (reader.BaseStream.Position < reader.BaseStream.Length)
			{
				var box = AtomicInfo.ParseBox(reader, parent, required);
				list.Add(box);
			}
		}

		public static long SizeEnd(this ICollection<AtomicInfo> list)
		{
			return list.Sum(box => box.GetBoxSize());
		}

		public static void WriteEnd(this BinaryWriter writer, ICollection<AtomicInfo> list)
		{
			foreach (var box in list)
			{
				box.WriteBox(writer);
			}
		}

		public static bool IsZero(this byte[] data)
		{
			if (data == null) return true;
			return !data.Any(b => b != 0);
		}

		public static Fixed<uint, x16> ToFixed32(this double value)
		{
			return (Fixed<uint, x16>)value;
		}

		public static Fixed<ushort, x8> ToFixed16(this double value)
		{
			return (Fixed<ushort, x8>)value;
		}

		public static string ToXML(this int[] value)
		{
			return String.Join(" ", value.Select(item => XmlConvert.ToString(item)));
		}

		public static void FromXML(this string value, out int[] data)
		{
			data = (value ?? "").Split(' ').Select(item => XmlConvert.ToInt32(item)).ToArray();
		}

		public static string ToXML(this long /*TimeSpan*/ ticks)
		{
			var value = new TimeSpan(ticks);
			var sb = new StringBuilder(20);
			if (value.Ticks < 0)
			{
				sb.Append('-');
				value = new TimeSpan(-value.Ticks);
			}
			sb.Append((value.Days * 24 + value.Hours).ToString("D2", NumberFormatInfo.InvariantInfo));
			sb.Append(':');
			sb.Append(value.Minutes.ToString("D2", NumberFormatInfo.InvariantInfo));
			sb.Append(':');
			sb.Append(value.Seconds.ToString("D2", NumberFormatInfo.InvariantInfo));
			sb.Append('.');
			sb.Append(value.Milliseconds.ToString("D3", NumberFormatInfo.InvariantInfo));
			return sb.ToString();
		}

		public static void FromXML(this string value, out long /*TimeSpan*/ ticks)
		{
			var parts = value.Split(':');
			if (parts.Length != 3)
				throw new FormatException("Unknown duration format " + value);
			var sec = parts[2].Split('.');
			if (sec.Length != 2)
				throw new FormatException("Unknown duration format " + value);
			int h = int.Parse(parts[0], NumberFormatInfo.InvariantInfo);
			int m = int.Parse(parts[1], NumberFormatInfo.InvariantInfo);
			int s = int.Parse(sec[0], NumberFormatInfo.InvariantInfo);
			int ms = int.Parse(sec[1], NumberFormatInfo.InvariantInfo);
			var data = new TimeSpan(0, h, m, s, ms);
			ticks = data.Ticks;
		}

		public static int UnknownEnum<TEnum>(this int value)
			where TEnum : struct
		{
			if (Enum.IsDefined(typeof(TEnum), value))
				return 0;
			else
				return value;
		}

		public static TEnum ValidEnum<TEnum>(this int value, TEnum defValue)
			where TEnum : struct
		{
			if (Enum.IsDefined(typeof(TEnum), value))
				return (TEnum)Enum.ToObject(typeof(TEnum), value);
			else
				return defValue;
		}

		public static int UnknownFlags<TEnum>(this int value)
			where TEnum : struct
		{
			int mask = 0;
			foreach (object eflag in Enum.GetValues(typeof(TEnum)))
			{
				mask |= Convert.ToInt32(eflag);
			}
			return value & ~mask;
		}

		public static TEnum ValidFlags<TEnum>(this int value)
			where TEnum : struct
		{
			return (TEnum)Enum.ToObject(typeof(TEnum), value ^ UnknownFlags<TEnum>(value));
		}

		public static uint Bits(this uint value, int length, int shift)
		{
			if (length < 0 || length > 32)
				throw new ArgumentOutOfRangeException("length");
			if (shift < 0 || shift > 32)
				throw new ArgumentOutOfRangeException("shift");
			uint mask = uint.MaxValue >> (32 - length);
			return (value >> shift) & mask;
		}

		public static int Bits(this int value, int length, int shift)
		{
			return (int)Bits((uint)value, length, shift);
		}

		public static int Bits(this ushort value, int length, int shift)
		{
			if (length < 0 || length > 16)
				throw new ArgumentOutOfRangeException("length");
			if (shift < 0 || shift > 16)
				throw new ArgumentOutOfRangeException("shift");
			int mask = ushort.MaxValue >> (16 - length);
			return (value >> shift) & mask;
		}

		public static int Bits(this byte value, int length, int shift)
		{
			if (length < 0 || length > 8)
				throw new ArgumentOutOfRangeException("length");
			if (shift < 0 || shift > 8)
				throw new ArgumentOutOfRangeException("shift");
			int mask = byte.MaxValue >> (8 - length);
			return (value >> shift) & mask;
		}

		public static bool Bit(this uint value, int shift)
		{
			if (shift < 0 || shift > 32)
				throw new ArgumentOutOfRangeException("shift");
			uint mask = 1u << shift;
			return (value & mask) == mask;
		}

		public static bool Bit(this int value, int shift)
		{
			return Bit((uint)value, shift);
		}

		public static bool Bit(this ushort value, int shift)
		{
			if (shift < 0 || shift > 16)
				throw new ArgumentOutOfRangeException("shift");
			int mask = 1 << shift;
			return (value & mask) == mask;
		}

		public static bool Bit(this byte value, int shift)
		{
			if (shift < 0 || shift > 8)
				throw new ArgumentOutOfRangeException("shift");
			int mask = 1 << shift;
			return (value & mask) == mask;
		}

		public static uint Bits(this uint value, int length, int shift, uint bits)
		{
			if (length < 0 || length > 32)
				throw new ArgumentOutOfRangeException("length");
			if (shift < 0 || shift > 32)
				throw new ArgumentOutOfRangeException("shift");
			uint mask = uint.MaxValue >> (32 - length) << shift;
			return value & ~mask | (bits << shift) & mask;
		}

		public static int Bits(this int value, int length, int shift, int bits)
		{
			return (int)Bits((uint)value, length, shift, (uint)bits);
		}

		public static ushort Bits(this ushort value, int length, int shift, int bits)
		{
			if (length < 0 || length > 16)
				throw new ArgumentOutOfRangeException("length");
			if (shift < 0 || shift > 16)
				throw new ArgumentOutOfRangeException("shift");
			int mask = ushort.MaxValue >> (16 - length) << shift;
			return (ushort)(value & ~mask | (bits << shift) & mask);
		}

		public static byte Bits(this byte value, int length, int shift, int bits)
		{
			if (length < 0 || length > 8)
				throw new ArgumentOutOfRangeException("length");
			if (shift < 0 || shift > 8)
				throw new ArgumentOutOfRangeException("shift");
			int mask = byte.MaxValue >> (8 - length) << shift;
			return (byte)(value & ~mask | (bits << shift) & mask);
		}

		public static uint Bit(this uint value, int shift, bool bit)
		{
			if (shift < 0 || shift > 32)
				throw new ArgumentOutOfRangeException("shift");
			uint mask = 1u << shift;
			return bit ? value | mask : value & ~mask;
		}

		public static int Bit(this int value, int shift, bool bit)
		{
			return (int)Bit((uint)value, shift, bit);
		}

		public static ushort Bit(this ushort value, int shift, bool bit)
		{
			if (shift < 0 || shift > 16)
				throw new ArgumentOutOfRangeException("shift");
			int mask = 1 << shift;
			return (ushort)(bit ? value | mask : value & ~mask);
		}

		public static byte Bit(this byte value, int shift, bool bit)
		{
			if (shift < 0 || shift > 8)
				throw new ArgumentOutOfRangeException("shift");
			int mask = 1 << shift;
			return (byte)(bit ? value | mask : value & ~mask);
		}
	}

	[System.Diagnostics.DebuggerDisplay("{KnownParentAtom ?? \"*\",nq}.{KnownAtomName,nq}<{BoxClass==null ? \"unknown box\" : BoxClass.Name,nq}>")]
	public class AtomDefinition
	{
		public string KnownAtomName { get; private set; }
		public string KnownParentAtom { get; private set; }
		public AtomState ContainerState { get; private set; }
		public AtomRequirements PresenceRequirements { get; private set; }
		public BoxType BoxType { get; private set; }
		public Type BoxClass { get; private set; }

		public bool AnyLevel { get { return this.KnownParentAtom == null; } }
		public bool FileLevel { get { return this.KnownParentAtom == ""; } }

		public virtual AtomicInfo CreateBox(AtomicCode atom, AtomicInfo parent)
		{
			AtomicInfo box;
			//if (BoxClass == typeof(ISOMediaBoxes.FileTypeBox)) box = new ISOMediaBoxes.FileTypeBox(); else
			if (this.BoxClass == null)
			{
				if (this.BoxType == BoxType.SimpleAtom && this.ContainerState == AtomState.ParentAtom)
					box = new ISOMediaBoxes.UnknownParentBox();
				else
					box = new ISOMediaBoxes.UnknownBox();
			}
			else
			{
				box = (AtomicInfo)Activator.CreateInstance(this.BoxClass);
			}
			box.Name = KnownAtomName ?? (string)atom;
			box.Parent = parent;
			return box;
		}

		protected AtomDefinition(string name, Type type, string parent, AtomState state, AtomRequirements requirements, BoxType boxType)
		{
			this.KnownAtomName = name;
			this.KnownParentAtom = parent;
			this.ContainerState = state;
			this.PresenceRequirements = requirements;
			this.BoxType = boxType;
			this.BoxClass = type;
		}

		public AtomDefinition(string name, AtomState state, AtomRequirements requirements, BoxType boxType)
			: this(name, null, null, state, requirements, boxType) { }

		public AtomDefinition(string name, string parent, AtomState state, AtomRequirements requirements, BoxType boxType)
			: this(name, null, parent, state, requirements, boxType) { }
	}

	public class AtomDefinition<TBox> : AtomDefinition
		where TBox : AtomicInfo, new()
	{
		public override AtomicInfo CreateBox(AtomicCode atom, AtomicInfo parent)
		{
			if (KnownAtomName != null)
				return new TBox
				{
					Name = KnownAtomName,
					Parent = parent
				};
			else
				return new TBox
				{
					AtomicID = atom,
					Parent = parent
				};
		}

		public AtomDefinition(string name, AtomState state, AtomRequirements requirements, BoxType boxType)
			: base(name, typeof(TBox), null, state, requirements, boxType) { }

		public AtomDefinition(string name, string parent, AtomState state, AtomRequirements requirements, BoxType boxType)
			: base(name, typeof(TBox), parent, state, requirements, boxType) { }
	}

	public class UUIDVitals
	{
		public UUIDForm UUIDForm;
		public Guid BinaryUUID;
		public string UUIDAPAtomName;
	}


	public class Stik
	{
		public string Name { get; private set; }
		public int Number { get; private set; }
		public Stik(string name, int number)
		{
			this.Name = name;
			this.Number = number;
		}
	}

	public class GenreID
	{
		public string Name { get; private set; }
		public int ID { get; private set; }
		public GenreID(string name, int id)
		{
			this.Name = name;
			this.ID = id;
		}
	}

	public class StoreFrontID
	{
		public string Name { get; private set; }
		public int ID { get; private set; }
		public StoreFrontID(string name, int id)
		{
			this.Name = name;
			this.ID = id;
		}
	}

	public class MediaRating
	{
		public string Code { get; private set; }
		public string Label { get; private set; }
		public MediaRating(string code, string label)
		{
			this.Code = code;
			this.Label = label;
		}
	}

	public enum UUIDForm {
		Deprecated,
		SHA1Namespace,
		APSHA1Namespace,
		Other
	}

	/// <summary>
	/// Graphics Modes
	/// </summary>
	[Flags]
	public enum GraphicsMode
	{
		/// <summary>
		/// Copy the source image over the destination.
		/// </summary>
		SourceCopy = 0,
		/// <summary>
		/// Dither the image (if needed), otherwise do a copy.
		/// </summary>
		DitherCopy = 64,
		/// <summary>
		/// Replaces destination pixel with a blend of the source and destination pixel colors, with the proportion
		///   for each channel controlled by that channel in the opcolor.
		/// </summary>
		Blend = 32,
		/// <summary>
		/// Replaces the destination pixel with the source pixel if the source pixel isn't equal to the opcolor.
		/// </summary>
		Transparent = 36,
		/// <summary>
		/// Replaces the destination pixel with a blend of the source and destination pixels, with the proportion
		///   controlled by the alpha channel.
		/// </summary>
		StraightAlpha = 0x100,
		/// <summary>
		/// Premultiplied with white means that the color components of each pixel have already been blended with
		///   a white pixel, based on their alpha channel value. Effectively, this means that the image has already
		///   been combined with a white background. First, remove the white from each pixel and then blend the image
		///   with the actual background pixels.
		/// </summary>
		PremulWhiteAlpha = 0x101,
		/// <summary>
		/// Premultiplied with black is the same as pre-multiplied with white, except the background color that
		///   the image has been blended with is black instead of white.
		/// </summary>
		PremulBlackAlpha = 0x102,
		/// <summary>
		/// Similar to straight alpha, but the alpha value used for each channel is the combination of the alpha channel
		///   and that channel in the opcolor.
		/// </summary>
		StraightAlphaBlend = 0x104,
		/// <summary>
		/// (Tracks only) The track is drawn offscreen, and then composed onto the screen using dither copy.
		/// </summary>
		Composition = 0x103 //dither copy
	}
}
