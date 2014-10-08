/*
 *			GPAC - Multimedia Framework C SDK - box_code_meta.c
 *
 *			Copyright (c) Cyril Concolato / Jean Le Feuvre 2005
 *					All rights reserved
 *
 *  This file is part of GPAC / ISO Media File Format sub-project
 *
 *  GPAC is free software you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation either version 2, or (at your option)
 *  any later version.
 *   
 *  GPAC is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *   
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  ----------------------
 *  SVN revision information:
 *    $Revision$
 *
 */
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		/// <summary>
		/// Metadata Atom
		/// </summary>
		public sealed partial class MetaBox: ISOMFullBox, IBoxContainer
		{
#warning Looking forward to 'dinf', 'xml ', 'bxml', 'iloc', 'pitm', 'ipro', 'iinf', 'ipmc', 'ID32', 'ilst', 'esds', 'data'
			[XmlElement("Handler", typeof(HandlerBox))]
			//[XmlElement(typeof(dinf))]
			//[XmlElement(typeof(xml ))]
			//[XmlElement(typeof(bxml))]
			//[XmlElement(typeof(iloc))]
			//[XmlElement(typeof(pitm))]
			//[XmlElement(typeof(ipro))]
			//[XmlElement(typeof(iinf))]
			//[XmlElement(typeof(ipmc))]
			//[XmlElement(typeof(ID32))]
			//[XmlElement(typeof(ilst))]
			[XmlElement("FreeSpace", typeof(ISOMediaBoxes.FreeSpaceBox))]
			[XmlElement("UUID", typeof(ISOMediaBoxes.UUIDBox))]
			//[XmlElement(typeof(esds))]
			//[XmlElement(typeof(data))]
			[XmlElement(typeof(UnknownBox))]
			[XmlElement(typeof(UnknownParentBox))]
			public Collection<AtomicInfo> Boxes
			{
				get { return this.boxList; }
			}

			public override void ReadBinary(BinaryReader reader)
			{
				long pos = reader.BaseStream.Position;
				var next = reader.ReadUInt32();
				reader.BaseStream.Seek(pos, SeekOrigin.Begin);
				/*try to hack around QT files which don't use a full box for meta*/
				if ((long)next < reader.Length()) //TODO: like error
				{
					base.ReadBinary(reader);
				}
				else
				{
					this.Versioned = false;
				}
				reader.ReadEnd(boxList, this);
			}

			public override long DataSize
			{
				get
				{
					return boxList.SizeEnd() + base.DataSize;
				}
			}

			public override void WriteBinary(BinaryWriter writer)
			{
				base.WriteBinary(writer);
				writer.WriteEnd(boxList);
			}
		}
	}
}
