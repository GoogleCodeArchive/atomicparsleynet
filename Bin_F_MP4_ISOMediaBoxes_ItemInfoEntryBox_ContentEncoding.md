#summary ContentEncoding Binary Data Field

# ContentEncoding Binary Data Field #


An optional null-terminated string in UTF-8 characters used to indicate that the binary file is encoded and needs to be decoded before interpreted. The values are as defined for !Content-Encoding for HTTP /1.1. Some possible values are “gzip”, “compress” and “deflate”. An empty string indicates no content encoding.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Encoded Data Serializer</h3>

<i>Binary Reader</i>

<blockquote>Type: <b>[</b><b>BinStringReader]</b><br></blockquote>

<i>Binary Writer</i>

<blockquote>Type: <b>[</b><b>BinStringWriter]</b><br></blockquote>

<h3>Field Value</h3>
Type: <b>CString</b> — an encoded character string <br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ItemInfoEntryBox.md'>ISOMediaBoxes.ItemInfoEntryBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>