#summary ISOMediaBoxes.MovieBox Binary Block

# ISOMediaBoxes.MovieBox Binary Block #


The Movie Atom `'moov'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<br>
<blockquote><h4>The layout of a movie atom</h4>
<img src='https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qtff_09.jpg' /></blockquote>


<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieBox_boxList.md'>boxList</a>

<blockquote>Type: <a href='Bin_T_MP4_AtomicInfo.md'>AtomicInfo</a>[] — a series of other atoms<br>The movie atom contains other types of atoms, including at least one of three possible atoms — the <a href='Bin_T_MP4_ISOMediaBoxes_MovieHeaderBox.md'>movie header atom</a> (<code>'mvhd'</code>), the compressed movie atom (<code>'cmov'</code>), or a reference movie atom (<code>'rmra'</code>). An uncompressed movie atom can contain both a movie header atom and a reference movie atom, but it must contain at least one of the two. It can also contain several other atoms, such as a clipping atom (<code>'clip'</code>), one or more <a href='Bin_T_MP4_ISOMediaBoxes_TrackBox.md'>track atoms</a> (<code>'trak'</code>), a color table atom (<code>'ctab'</code>), and a <a href='Bin_T_MP4_ISOMediaBoxes_UserDataBox.md'>user data atom</a> (<code>'udta'</code>).<br></blockquote>

<h2>Remarks</h2>

The metadata for a presentation is stored in the single Movie box which occurs at the top-level of a file. Normally this box is close to the beginning or end of the file, though this is not required.<br>
<br>
You use movie atoms to specify the information that defines a movie—that is, the information that allows your application to interpret the sample data that is stored elsewhere. The movie atom usually contains a movie header atom, which defines the time scale and duration information for the entire movie, as well as its display characteristics. Existing movies may contain a movie profile atom, which summarizes the main features of the movie, such as the necessary codecs and maximum bit rate. In addition, the movie atom contains a track atom for each track in the movie.<br>
<br>
The movie atom has an atom type of <code>'moov'</code>.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <b>MP4.ISOMediaBoxes.MovieBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>