#summary extentEntries Binary Data Field

# extentEntries Binary Data Field #


Extents into which the resource is fragmented.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Items Count</h3>
Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>
<h3>Field Value</h3>
Type: <a href='Bin_T_MP4_ISOMediaBoxes_ItemExtentEntry.md'>ISOMediaBoxes.ItemExtentEntry</a>[] — a series of items <br>
<h2>Remarks</h2>

Items may be stored fragmented into extents, e.g. to enable interleaving. An extent is a contiguous subset of the bytes of the resource; the resource is formed by concatenating the extents. If only one extent is used then either or both of the offset and length may be implied:<br>
<br>
<ul><li>If the offset is not identified (the field has a length of zero), then the beginning of the file (offset 0) is implied.<br>
</li><li>If the length is not specified, or specified as zero, then the entire file length is implied. References into the same file as this metadata, or items divided into more than one extent, should have an explicit offset and length, or use a MIME type requiring a different interpretation of the file, to avoid infinite recursion.</li></ul>


The size of the item is the sum of the extent lengths.<br>
<br>
<img src='https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif' /> <b>Note</b> extents may be interleaved with the chunks defined by the sample tables of tracks.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ItemLocationEntry.md'>ISOMediaBoxes.ItemLocationEntry Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>