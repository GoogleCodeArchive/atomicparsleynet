/*
 *			GPAC - Multimedia Framework C SDK - sample_descs.c
 *
 *			Copyright (c) Jean Le Feuvre 2000-2005 
 *					All rights reserved
 *
 *  This file is part of GPAC / ISO Media File Format sub-project
 *
 *  GPAC is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  GPAC is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *   
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  ----------------------
 *  SVN revision information:
 *    $Revision$
 *
 */
using System.ComponentModel;
using System.Xml.Serialization;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		public abstract partial class ISOMSampleEntryFields : ISOMUUIDBox
		{
			[XmlIgnore]
			public bool ReservedSpecified
			{
				get { return !Reserved.IsZero(); }
			}
		}

		public abstract partial class ISOMVisualSampleEntry : ISOMSampleEntryFields
		{
			[XmlAttribute("HorizRes")]
			public double HorizResAsFloat { get { return (double)HorizRes; } set { HorizRes = (Fixed<uint, x16>)value; } }

			[XmlAttribute("VertRes")]
			public double VertResAsFloat { get { return (double)VertRes; } set { VertRes = (Fixed<uint, x16>)value; } }

			[XmlAttribute("CompressorName"), DefaultValue("")]
			public string CompressorNameAsString { get { return CompressorName; } set { CompressorName = new UTF8PadString(value, 32); } }
		}

		public abstract partial class ISOMAudioSampleEntry : ISOMSampleEntryFields
		{
			[XmlIgnore]
			public bool Reserved2Specified
			{
				get { return !Reserved2.IsZero(); }
			}
		}
	}
}
