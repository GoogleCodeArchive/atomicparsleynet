#summary BaseOffset Binary Data Field

# BaseOffset Binary Data Field #


Provides a base value for offset calculations within the referenced data. If [BaseOffsetSize](Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_sizeData.md) is 0, **BaseOffset** takes the value 0, i.e. it is unused.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer (BaseOffsetSize equal to 4 only)<br>Type: <b>Int64</b> — a 64-bit (8 bytes) integer (BaseOffsetSize equal to 8 only)<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ItemLocationEntry.md'>ISOMediaBoxes.ItemLocationEntry Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>