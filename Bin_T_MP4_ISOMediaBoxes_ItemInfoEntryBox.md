#summary ISOMediaBoxes.ItemInfoEntryBox Binary Block

# ISOMediaBoxes.ItemInfoEntryBox Binary Block #


Item Information entry Box `'infe'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemInfoEntryBox_ItemID.md'>ItemID</a>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>Contains either 0 for the primary resource (e.g. the XML contained in an <code>'xml '</code> box) or the ID of the item for which the following information is defined.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemInfoEntryBox_ItemProtectionIndex.md'>ItemProtectionIndex</a>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>Contains either 0 for an unprotected item, or the one-based index into the item protection box defining the protection applied to this item (the first box in the item protection box has the index 1).<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemInfoEntryBox_ItemName.md'>ItemName</a>

<blockquote>Type: <b>CString</b> — an encoded character string <br>A null-terminated string in UTF-8 characters containing a symbolic name of the item.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemInfoEntryBox_ContentType.md'>ContentType</a>

<blockquote>Type: <b>CString</b> — an encoded character string <br>The MIME type for the item.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ItemInfoEntryBox_ContentEncoding.md'>ContentEncoding</a>

<blockquote>Type: <b>CString</b> — an encoded character string <br>An optional null-terminated string in UTF-8 characters used to indicate that the binary file is encoded and needs to be decoded before interpreted. The values are as defined for !Content-Encoding for HTTP /1.1. Some possible values are “gzip”, “compress” and “deflate”. An empty string indicates no content encoding.<br></blockquote>

<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.ItemInfoEntryBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>