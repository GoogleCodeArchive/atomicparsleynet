#summary DataReferenceIndex Binary Data Field

# DataReferenceIndex Binary Data Field #


Either zero (‘this file’) or a 1-based index into the data references in the data information box.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>
<h2>Remarks</h2>

The data-reference index may take the value 0, indicating a reference into the same file as this metadata, or an index into the data-reference table.<br>
<br>
Some referenced data may itself use offset/length techniques to address resources within it (e.g. an MP4 file might be ‘included’ in this way). Normally such offsets are relative to the beginning of the containing file. The field ‘base offset’ provides an additional offset for offset calculations within that contained data. For example, if an MP4 file is included within a file formatted to this specification, then normally data-offsets within that MP4 section are relative to the beginning of file; base_offset adds to those offsets.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_ItemLocationEntry.md'>ISOMediaBoxes.ItemLocationEntry Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>