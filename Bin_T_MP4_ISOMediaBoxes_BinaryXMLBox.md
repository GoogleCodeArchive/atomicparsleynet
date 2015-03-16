#summary ISOMediaBoxes.BinaryXMLBox Binary Block

# ISOMediaBoxes.BinaryXMLBox Binary Block #


Binary XML Box `'bxml'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_BinaryXMLBox_Data.md'>Data</a>

<blockquote>Type: <b>Byte[</b><b>]</b> — a stream of binary data with a length <br>Binary XML data<br></blockquote>

<h2>Remarks</h2>

When the primary data is in XML format and it is desired that the XML be stored directly in the meta-box, one of these forms may be used. The Binary XML Box may only be used when there is a single well-defined binarization of the XML for that defined format as identified by the handler.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.BinaryXMLBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>