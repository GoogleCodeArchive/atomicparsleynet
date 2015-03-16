#summary Brand Binary Data Field

# Brand Binary Data Field #


Identifies compatible file format.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>
<h2>Remarks</h2>

Any incompatible change in a specification should therefore register a new ‘brand’ identifier to identify files conformant to the new specification.<br>
<br>
The type <code>'isom'</code> (ISO Base Media file) is defined in this section of this specification, as identifying files that conform to the first version of ISO Base Media File Format.<br>
<br>
More specific identifiers can be used to identify precise versions of specifications providing more detail. This brand should not be used as the major brand; this base file format should be derived into another specification to be used. There is therefore no defined normal file extension, or mime type assigned to this brand, nor definition of the minor version when <code>'isom'</code> is the major brand.<br>
<br>
Files would normally be externally identified (e.g. with a file extension or mime type) that identifies the ‘best use’ (major brand), or the brand that the author believes will provide the greatest compatibility.<br>
<br>
The brand <code>'iso2'</code> shall be used to indicate compatibility with this amended version of the ISO Base Media File Format; it may be used in addition to or instead of the <code>'isom'</code> brand and the same usage rules apply. If used without the brand <code>'isom'</code> identifying the first version of the specification, it indicates that support for some or all of the technology introduced by this amendment is required.<br>
<br>
The brand <code>'avc1'</code> shall be used to indicate that the file is conformant with the ‘AVC Extensions’ in sub-clause. If used without other brands, this implies that support for those extensions is required. The use of <code>'avc1'</code> as a major-brand may be permitted by specifications; in that case, that specification defines the file extension and required behavior.<br>
<br>
If a Meta-box with an MPEG-7 handler type is used at the file level, then the brand <code>'mp71'</code> should be a member of the compatible-brands list in the file-type box.<br>
<br>
The brand <code>'qt '</code> (note the two trailing ASCII space characters) for QuickTime movie files. If a file is compatible with multiple brands, all such brands are listed in the <a href='Bin_F_MP4_ISOMediaBoxes_FileTypeBox_CompatibleBrand.md'>CompatibleBrand</a> fields, and the ‘major brand’ identifies the preferred brand or best use.<br>
<br>
<img src='https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif' /> <b>Note</b> In current version QuickTime movie files are not supported.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_FileTypeBox.md'>ISOMediaBoxes.FileTypeBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>