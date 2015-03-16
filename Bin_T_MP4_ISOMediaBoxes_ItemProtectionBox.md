#summary ISOMediaBoxes.ItemProtectionBox Binary Block

# ISOMediaBoxes.ItemProtectionBox Binary Block #


Item Protection Box `'ipro'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<i>entryArray items count</i>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer<br><br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemProtectionBox_entryArray.md'>entryArray</a>

<blockquote>Type: <a href='Bin_T_MP4_ISOMediaBoxes_ProtectionInfoBox.md'>ISOMediaBoxes.ProtectionInfoBox</a>[] — a series of protection informations<br>Protection Scheme Informations<br></blockquote>

<h2>Remarks</h2>
The item protection box provides an array of item protection information, for use by the Item Information Box.<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.ItemProtectionBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>