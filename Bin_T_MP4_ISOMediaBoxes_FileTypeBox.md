#summary ISOMediaBoxes.FileTypeBox Binary Block

# ISOMediaBoxes.FileTypeBox Binary Block #


The File Type Compatibility Atom `'ftyp'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_FileTypeBox_Brand.md'>Brand</a>

<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>Identifies compatible file format.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_FileTypeBox_Version.md'>Version</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The file format specification version; minor version of the major brand in general.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_FileTypeBox_CompatibleBrand.md'>CompatibleBrand</a>

<blockquote>Type: <b>Int32[</b><b>]</b> — a series of 32-bit unsigned integers<br>Listing compatible file formats. The major brand must appear in the list of compatible brands. One or more “placeholder” entries with value zero are permitted; such entries should be ignored.<br></blockquote>

<h2>Remarks</h2>

The file type compatibility atom, also called the file type atom, allows the reader to determine whether this is a type of file that the reader understands. Specifically, the file type atom identifies the file type specifications with which the file is compatible. This allows the reader to distinguish among closely related file types, such as QuickTime movie files, MPEG-4, and JPEG-2000 files (all of which may contain file type atoms, movie atoms, and movie data atoms).<br>
<br>
The file type atom is optional, but strongly recommended. If present, it must be the first significant atom in the file, preceding the movie atom (and any free space atoms, preview atom, or movie data atoms).<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <b>MP4.ISOMediaBoxes.FileTypeBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>