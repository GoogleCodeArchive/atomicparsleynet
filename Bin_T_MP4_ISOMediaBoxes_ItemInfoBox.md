#summary ISOMediaBoxes.ItemInfoBox Binary Block

# ISOMediaBoxes.ItemInfoBox Binary Block #


Item Information Box `'iinf'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<i>entryArray items count</i>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer<br><br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemInfoBox_entryArray.md'>entryArray</a>

<blockquote>Type: <a href='Bin_T_MP4_ISOMediaBoxes_ItemInfoEntryBox.md'>ISOMediaBoxes.ItemInfoEntryBox</a>[] — a series of entries<br>An array of entries<br></blockquote>

<h2>Remarks</h2>
The Item information box provides extra information about selected items, including symbolic (‘file’) names. It may optionally occur, but if it does, it must be interpreted, as item protection or content encoding may have changed the format of the data in the item. If both content encoding and protection are indicated for an item, a reader should first un-protect the item, and then decode the item’s content encoding. If more control is needed, an IPMP sequence code may be used.<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.ItemInfoBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>