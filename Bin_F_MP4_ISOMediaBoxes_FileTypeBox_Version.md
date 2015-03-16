#summary Version Binary Data Field

# Version Binary Data Field #


The file format specification version; minor version of the major brand in general.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>
<h2>Remarks</h2>

The minor version is informative only. It does not appear for compatible-brands, and must not be used to determine the conformance of a file to a standard. It may allow more precise identification of the major specification, for inspection, debugging, or improved decoding.<br>
<br>
For QuickTime movie files, this takes the form of four binary-coded decimal values, indicating the century, year, and month of the <i>QuickTime File Format Specification</i>, followed by a binary coded decimal zero. For example, for the June 2004 minor version, this field is set to the BCD values <code>20 04 06 00</code>.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_FileTypeBox.md'>ISOMediaBoxes.FileTypeBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>