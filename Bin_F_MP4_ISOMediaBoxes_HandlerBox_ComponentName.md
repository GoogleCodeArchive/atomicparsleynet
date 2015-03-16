#summary ComponentName Binary Data Field

# ComponentName Binary Data Field #


A (counted) string that specifies the name of the component — that is, the media/metadata handler used when this media was created. This field may contain a zero-length (empty) string.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Length</h3>

Type: <b>Byte</b> — an 8-bit (1 byte) integer (Optional)<br>
<br>
<h3>Field Value</h3>

Type: <b>CString</b> — a null-terminated UTF-8 encoded character string<br>
<br>
<h2>Remarks</h2>

A null-terminated string in UTF-8 characters which gives a human-readable name for the track type or the metadata type (for debugging and inspection purposes).<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_HandlerBox.md'>ISOMediaBoxes.HandlerBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>