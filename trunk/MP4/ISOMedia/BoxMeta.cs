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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		// Metadata Atom
		public sealed partial class MetaBox: ISOMFullBox, IBoxContainer
		{
			private void ReadFullBox(BinaryReader reader)
			{
				long size = reader.Length();
				//try to hack around QT files which don't use a full box for meta
				if (size < 4L) return; //no full box and no atoms
				uint boxSize = reader.ReadUInt32();
				var atomid = size >= 8L ? reader.ReadAtomicCode() : (AtomicCode)0u;
				if ((uint)atomid <= size)
				{
					base.Version = (byte)(boxSize >> 24);
					base.Flags = (int)(boxSize & 0xFFFFFFu);
					if (size < 8L) return; //full box without atoms
					boxSize = (uint)atomid;
					atomid = reader.ReadAtomicCode();
				}
				var box = AtomicInfo.ParseBox(reader, boxSize, atomid, parent: this);
				boxList.Add(box);
			}
		}

		public sealed partial class XMLBox : ISOMFullBox
		{
			[XmlIgnore]
			public XmlDocument XMLDocument
			{
				get
				{
					var doc = new XmlDocument();
					doc.LoadXml(XML);
					return doc;
				}
				set
				{
					XML = value.OuterXml;
				}
			}
		}

		public sealed partial class BinaryXMLBox : ISOMFullBox
		{
			[XmlIgnore]
			public XmlDocument XMLDocument
			{
				get
				{
					var doc = new XmlDocument();
					doc.Load(new MemoryStream(Data));
					return doc;
				}
				set
				{
					Data = System.Text.Encoding.UTF8.GetBytes(value.OuterXml);
				}
			}
		}

		public sealed partial class ItemLocationEntry : IEntry<ItemLocationBox>
		{
			private ItemLocationBox owner;

			/// <summary>
			/// Item Location Box
			/// </summary>
			public ItemLocationBox Owner
			{
				get { return owner; }
				set { AtomicInfo.CreateEntryCollection(out extentEntries, owner = value); }
			}
		}
	}
}
