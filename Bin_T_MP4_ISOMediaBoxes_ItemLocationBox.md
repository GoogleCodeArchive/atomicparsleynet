#summary ISOMediaBoxes.ItemLocationBox Binary Block

# ISOMediaBoxes.ItemLocationBox Binary Block #


Item Location Box `'iloc'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_sizeData.md'>sizeData</a>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>
The box starts with three values, specifying the size in bytes:</blockquote>

<ul><li><i>OffsetSize</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Taken from the set {0, 4, 8} and indicates the length in bytes of the <a href='F_MP4_ISOMediaBoxes_ItemExtentEntry_Offset.md'>Offset</a> field.<br>
</li><li><i>LengthSize</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Taken from the set {0, 4, 8} and indicates the length in bytes of the <a href='F_MP4_ISOMediaBoxes_ItemExtentEntry_Length.md'>Length</a> field.<br>
</li><li><i>BaseOffsetSize</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Taken from the set {0, 4, 8} and indicates the length in bytes of the <a href='F_MP4_ISOMediaBoxes_ItemLocationEntry_BaseOffset.md'>BaseOffset</a> field.<br>
</li><li><i>Reserved</i><br> Type: <b>UInt4</b> — an 4-bit unsigned integer<br> Reserved. Set to 0.</li></ul>

<blockquote>Specifies the size in bytes of the <a href='Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Offset.md'>Offset</a> field, <a href='Bin_F_MP4_ISOMediaBoxes_ItemExtentEntry_Length.md'>Length</a> field, and <a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationEntry_BaseOffset.md'>BaseOffset</a> field, respectively. These values must be from the set {0, 4, 8}.<br></blockquote>

<i>locationEntries items count</i>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemLocationBox_locationEntries.md'>locationEntries</a>

<blockquote>Type: <a href='Bin_T_MP4_ISOMediaBoxes_ItemLocationEntry.md'>ISOMediaBoxes.ItemLocationEntry</a>[] — a series of items <br>Resources in the following array.<br></blockquote>

<h2>Remarks</h2>

The item location box provides a directory of resources in this or other files, by locating their containing file, their offset within that file, and their length. Placing this in binary format enables common handling of this data, even by systems which do not understand the particular metadata system (handler) used. For example, a system might integrate all the externally referenced metadata resources into one file, re-adjusting file offsets and file references accordingly.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.ItemLocationBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>