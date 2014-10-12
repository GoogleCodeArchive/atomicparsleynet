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

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		public abstract partial class ISOMSampleEntryFields : ISOMUUIDBox
		{
			protected ISOMSampleEntryFields(string type) : base(type) { }
			internal ISOMSampleEntryFields() { }
		}

		public abstract partial class ISOMVisualSampleEntry : ISOMSampleEntryFields
		{
			protected ISOMVisualSampleEntry(string type) : base(type) { }
			internal ISOMVisualSampleEntry() { }
		}

		public abstract partial class ISOMAudioSampleEntry : ISOMSampleEntryFields
		{
			protected ISOMAudioSampleEntry(string type) : base(type) { }
			internal ISOMAudioSampleEntry() { }
		}
	}
}
