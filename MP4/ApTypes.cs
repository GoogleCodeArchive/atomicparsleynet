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

namespace MP4
{
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
		/// no NULL termination, used in purl & egid
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

		public const int UNDEFINED_STYLE = 0;
		public const int ITUNES_STYLE = 100;
		/// <summary>
		/// 3gpp files prior to 3gp6
		/// </summary>
		public const int THIRD_GEN_PARTNER = 300;
		/// <summary>
		/// 3GPP Release6 the first spec to contain the complement of assets
		/// </summary>
		public const int THIRD_GEN_PARTNER_VER1_REL6 = 306;
		/// <summary>
		/// 3GPP Release7 introduces ID32 atoms
		/// </summary>
		public const int THIRD_GEN_PARTNER_VER1_REL7 = 307;
		/// <summary>
		/// 3gp2 files
		/// </summary>
		public const int THIRD_GEN_PARTNER_VER2 = 320;
		/// <summary>
		/// 3gp2 files, 3GPP2 C.S0050-A introduces 'gadi'
		/// </summary>
		public const int THIRD_GEN_PARTNER_VER2_REL_A = 321;
		public const int MOTIONJPEG2000 = 400;

		private const uint MaxAtomID = 0xFFFFU;

		public static AtomicCode ReadAtomicCode(this BinaryReader reader)
		{
			return (AtomicCode)reader.ReadUInt32();
		}

		public static void Write(this BinaryWriter writer, AtomicCode code)
		{
			writer.Write((uint)code);
		}

		public static void ReadCount(this BinaryReader reader, ICollection<AtomicInfo> list, AtomicInfo parent, bool required = true)
		{
			list.Clear();
			int count = reader.ReadInt32();
			for (int index = 0; index < count; index++)
			{
				var box = AtomicInfo.ParseBox(reader, parent, required);
				list.Add(box);
			}
		}

		public static long SizeCount(this ICollection<AtomicInfo> list)
		{
			return 4L + list.Sum(box => box.GetBoxSize());
		}

		public static void WriteCount(this BinaryWriter writer, ICollection<AtomicInfo> list)
		{
			writer.Write((int)list.Count);
			foreach (var box in list)
			{
				box.WriteBox(writer);
			}
		}

		public static void ReadEnd(this BinaryReader reader, ICollection<AtomicInfo> list, AtomicInfo parent, bool required = false)
		{
			list.Clear();
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
	}

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
	/// <remarks>
	/// <para>QuickTime files use graphics modes to describe how one video or graphics layer should be combined with
	///   the layers beneath it. Graphics modes are also known as transfer modes. Some graphics modes require a color
	///   to be specified for certain operations, such as blending to determine the blend level. QuickTime uses
	///   the graphics modes defined by Apple’s QuickDraw.</para>
	/// <para>The most common graphics modes are and <c>ditherCopy</c>, which simply indicate that the image should
	///   not blend with the image behind it, but overwrite it. QuickTime also defines several additional graphics modes.</para>
	/// </remarks>
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
