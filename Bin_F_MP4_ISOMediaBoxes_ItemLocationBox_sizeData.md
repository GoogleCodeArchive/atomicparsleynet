#summary sizeData Binary Data Field

# sizeData Binary Data Field #


Specifies the size in bytes of the [Offset](Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Offset.md) field, [Length](Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Length.md) field, and [BaseOffset](Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_BaseOffset.md) field, respectively. These values must be from the set {0, 4, 8}.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>
The box starts with three values, specifying the size in bytes:<br>
<br>
<ul><li><i>OffsetSize</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Taken from the set {0, 4, 8} and indicates the length in bytes of the <a href='F_MP4_ISOMediaBoxes_ItemExtentEntry_Offset.md'>Offset</a> field.<br>
</li><li><i>LengthSize</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Taken from the set {0, 4, 8} and indicates the length in bytes of the <a href='F_MP4_ISOMediaBoxes_ItemExtentEntry_Length.md'>Length</a> field.<br>
</li><li><i>BaseOffsetSize</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Taken from the set {0, 4, 8} and indicates the length in bytes of the <a href='F_MP4_ISOMediaBoxes_ItemLocationEntry_BaseOffset.md'>BaseOffset</a> field.<br>
</li><li><i>Reserved</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Reserved. Set to 0.</li></ul>


<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ItemLocationBox.md'>ISOMediaBoxes.ItemLocationBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>