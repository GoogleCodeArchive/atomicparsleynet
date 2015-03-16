#summary CompatibleBrand Binary Data Field

# CompatibleBrand Binary Data Field #


Listing compatible file formats. The major brand must appear in the list of compatible brands. One or more “placeholder” entries with value zero are permitted; such entries should be ignored.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>

Type: <b>Int32[</b><b>]</b> — a series of 32-bit unsigned integers<br>
<br>
<h2>Remarks</h2>

If none of the compatible brands fields is set to <code>'qt '</code>, then the file is not a QuickTime movie file. QuickTime currently returns an error when attempting to open a file whose file type, file extension, or MIME type identifies it as a QuickTime movie, but whose file type atom does not include the <code>'qt '</code> brand.<br>
<br>
<img src='https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif' /> <b>Note</b> A common source of this error is an MPEG-4 file incorrectly named with the <code>.mov</code> file extension or with the MIME type incorrectly set to “video/quicktime”. MPEG-4 files are automatically imported by QuickTime only when they are correctly identified as MPEG-4 files using the Mac OS file type, file extension, or MIME type.<br>
<br>
<img src='https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_caution.gif' /> <b>Note</b> Use of the QuickTime file format in this manner is subject to license from Apple, Inc.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_FileTypeBox.md'>ISOMediaBoxes.FileTypeBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>