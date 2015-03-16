#summary Version Binary Data Field

# Version Binary Data Field #


The version of this atom.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>
<h2>Remarks</h2>

In a number of boxes there are two variant forms: version 0 using 32-bit fields, and version 1 using 64-bit sizes for those same fields. In general, if a version 0 box (32-bit field sizes) can be used, it should be; version 1 boxes should be used only when the 64-bit field sizes they permit, are required.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>ISOMediaBoxes.ISOMFullBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>