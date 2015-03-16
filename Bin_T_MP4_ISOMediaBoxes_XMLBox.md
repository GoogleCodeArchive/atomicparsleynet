#summary ISOMediaBoxes.XMLBox Binary Block

# ISOMediaBoxes.XMLBox Binary Block #


XML Box `'xml '`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_XMLBox_XML.md'>XML</a>

<blockquote>Type: <b>Char[</b><b>]</b> — an encoded character array with a length <br>XML data<br></blockquote>

<h2>Remarks</h2>

When the primary data is in XML format and it is desired that the XML be stored directly in the meta-box, one of these forms may be used.<br>
<br>
Within an XML box the data is in UTF-8 format unless the data starts with a byte-order-mark (BOM), which indicates that the data is in UTF-16 format.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.XMLBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>