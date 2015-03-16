#summary boxList Binary Data Field

# boxList Binary Data Field #


The media atom must contain a [media header atom](Bin_T_MP4_ISOMediaBoxes_MediaHeaderBox.md) (`'mdhd'`), and it can contain a [handler reference](Bin_T_MP4_ISOMediaBoxes_HandlerBox.md) (`'hdlr'`) atom, [media information](Bin_T_MP4_ISOMediaBoxes_MediaInformationBox.md) (`'minf'`) atom, and [user data](Bin_T_MP4_ISOMediaBoxes_UserDataBox.md) (`'udta'`) atom.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>

Type: <a href='Bin_T_MP4_AtomicInfo.md'>AtomicInfo</a>[] — a series of other atoms<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_MediaBox.md'>ISOMediaBoxes.MediaBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>