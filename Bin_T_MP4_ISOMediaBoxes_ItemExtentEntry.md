#summary ISOMediaBoxes.ItemExtentEntry Binary Block

# ISOMediaBoxes.ItemExtentEntry Binary Block #


Item Location Box resource extent

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Offset.md'>Offset</a> (Optional)<br>
<br>
<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer (OffsetSize equal to 4 only)<br>Type: <b>Int64</b> — a 64-bit (8 bytes) integer (OffsetSize equal to 8 only)<br>Provides the absolute offset in bytes from the beginning of the containing file, of this item. If <a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_sizeData.md'>OffsetSize</a> is 0, <a href='Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Offset.md'>Offset</a> takes the value 0.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Length.md'>Length</a> (Optional)<br>
<br>
<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer (LengthSize equal to 4 only)<br>Type: <b>Int64</b> — a 64-bit (8 bytes) integer (LengthSize equal to 8 only)<br>Provides the absolute length in bytes of this metadata item. If <a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_sizeData.md'>LengthSize</a> is 0, <a href='Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Length.md'>Length</a> takes the value 0. If the value is 0, then length of the item is the length of the entire referenced file.<br></blockquote>

<h2>Inheritance Hierarchy</h2>
<b>MP4.ISOMediaBoxes.ItemExtentEntry</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>