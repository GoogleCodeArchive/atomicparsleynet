#summary ComponentType Binary Data Field

# ComponentType Binary Data Field #


Within a [media atom](Bin_T_MP4_ISOMediaBoxes_MediaBox.md) this is a [four-character code](T_MP4_AtomicCode.md) that identifies the type of the handler. Only two values are valid for this field: `'mhlr'` for media handlers and `'dhlr'` for data handlers.

Within a [meta box](Bin_T_MP4_ISOMediaBoxes_MetaBox.md) this is a predefined integer that is set to 0.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_HandlerBox.md'>ISOMediaBoxes.HandlerBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>