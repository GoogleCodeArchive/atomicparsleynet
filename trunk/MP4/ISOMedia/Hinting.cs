/*
 *			GPAC - Multimedia Framework C SDK - hinting.c
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
using System.Xml.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		public sealed partial class HintSampleEntryBox : ISOMSampleEntryFields, IBoxContainer
		{
			public override ProtectionInfoBox ProtectionInfo { get { return null; } set { } }

			[XmlElement("TSHint", typeof(TSHintEntryBox))]
			[XmlElement("TimeOffHint", typeof(TimeOffHintEntryBox))]
			[XmlElement("SeqOffHint", typeof(SeqOffHintEntryBox))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> HintDataTable
			{
				get { return this.boxList; }
			}

			Collection<AtomicInfo> IBoxContainer.Boxes { get { return this.boxList; } }
		}

#line 117 "hinting.c"
#warning Lines 117 - 984 aren't implemented
#line default
	}
}
