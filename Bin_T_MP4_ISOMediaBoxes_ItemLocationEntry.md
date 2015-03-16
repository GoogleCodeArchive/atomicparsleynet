#summary ISOMediaBoxes.ItemLocationEntry Binary Block

# ISOMediaBoxes.ItemLocationEntry Binary Block #


Item Location Box resource

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_ItemID.md'>ItemID</a>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>An arbitrary integer ‘name’ for this resource which can be used to refer to it (e.g. in a URL).<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_DataReferenceIndex.md'>DataReferenceIndex</a>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>Either zero (‘this file’) or a 1-based index into the data references in the data information box.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_BaseOffset.md'>BaseOffset</a> (Optional)<br>
<br>
<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer (BaseOffsetSize equal to 4 only)<br>Type: <b>Int64</b> — a 64-bit (8 bytes) integer (BaseOffsetSize equal to 8 only)<br>Provides a base value for offset calculations within the referenced data. If <a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_sizeData.md'>BaseOffsetSize</a> is 0, <a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_BaseOffset.md'>BaseOffset</a> takes the value 0, i.e. it is unused.<br></blockquote>

<i>extentEntries items count</i>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_extentEntries.md'>extentEntries</a>

<blockquote>Type: <a href='Bin_T_MP4_ISOMediaBoxes_ItemExtentEntry.md'>ISOMediaBoxes.ItemExtentEntry</a>[] — a series of items <br>Extents into which the resource is fragmented.<br></blockquote>

<h2>Inheritance Hierarchy</h2>
<b>MP4.ISOMediaBoxes.ItemLocationEntry</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>