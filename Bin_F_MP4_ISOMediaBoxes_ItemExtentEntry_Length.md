#summary Length Binary Data Field

# Length Binary Data Field #


Provides the absolute length in bytes of this metadata item. If [LengthSize](Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_sizeData.md) is 0, **Length** takes the value 0. If the value is 0, then length of the item is the length of the entire referenced file.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>Int64</b> — a 64-bit (8 bytes) integer (LengthSize equal to 8 only)<br>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer (LengthSize equal to 4 only)<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ItemExtentEntry.md'>ISOMediaBoxes.ItemExtentEntry Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>