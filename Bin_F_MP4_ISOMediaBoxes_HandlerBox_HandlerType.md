#summary HandlerType Binary Data Field

# HandlerType Binary Data Field #


Within a [media atom](Bin_T_MP4_ISOMediaBoxes_MediaBox.md) this is a [four-character code](T_MP4_AtomicCode.md) that identifies the type of the media handler or data handler.

Within a [meta box](Bin_T_MP4_ISOMediaBoxes_MetaBox.md) this is a [four-character code](T_MP4_AtomicCode.md) that identifies the structure used in the metadata atom, set to `‘mdta’`.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>
<h2>Remarks</h2>

For media handlers, this field defines the type of data — for example, <code>'vide'</code> for video data, <code>'soun'</code> for sound data, <code>'hint'</code> for hint track or <code>'subt'</code> for subtitles.<br>
<br>
For data handlers, this field defines the data reference type; for example, a component subtype value of <code>'alis'</code> identifies a file alias.<br>
<br>
A reader parsing a metadata atom should confirm the handler type in the metadata handler atom is <code>‘mdta’</code> before interpreting any other structures in the metadata atom. If the handler type is not <code>‘mdta’</code>, the interpretation is defined by another specification.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_HandlerBox.md'>ISOMediaBoxes.HandlerBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>