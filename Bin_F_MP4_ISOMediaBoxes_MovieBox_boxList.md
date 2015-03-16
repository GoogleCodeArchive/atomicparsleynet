#summary boxList Binary Data Field

# boxList Binary Data Field #


The movie atom contains other types of atoms, including at least one of three possible atoms — the [movie header atom](Bin_T_MP4_ISOMediaBoxes_MovieHeaderBox.md) (`'mvhd'`), the compressed movie atom (`'cmov'`), or a reference movie atom (`'rmra'`). An uncompressed movie atom can contain both a movie header atom and a reference movie atom, but it must contain at least one of the two. It can also contain several other atoms, such as a clipping atom (`'clip'`), one or more [track atoms](Bin_T_MP4_ISOMediaBoxes_TrackBox.md) (`'trak'`), a color table atom (`'ctab'`), and a [user data atom](Bin_T_MP4_ISOMediaBoxes_UserDataBox.md) (`'udta'`).

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>

Type: <a href='Bin_T_MP4_AtomicInfo.md'>AtomicInfo</a>[] — a series of other atoms<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_MovieBox.md'>ISOMediaBoxes.MovieBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>