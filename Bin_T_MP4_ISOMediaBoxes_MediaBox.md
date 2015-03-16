#summary ISOMediaBoxes.MediaBox Binary Block

# ISOMediaBoxes.MediaBox Binary Block #


Media Atom `'mdia'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<br>
<blockquote><h4>The layout of a media atom</h4>
<img src='https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_030-G.jpg' /></blockquote>


<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_MediaBox_boxList.md'>boxList</a>

<blockquote>Type: <a href='Bin_T_MP4_AtomicInfo.md'>AtomicInfo</a>[] — a series of other atoms<br>The media atom must contain a <a href='Bin_T_MP4_ISOMediaBoxes_MediaHeaderBox.md'>media header atom</a> (<code>'mdhd'</code>), and it can contain a <a href='Bin_T_MP4_ISOMediaBoxes_HandlerBox.md'>handler reference</a> (<code>'hdlr'</code>) atom, <a href='Bin_T_MP4_ISOMediaBoxes_MediaInformationBox.md'>media information</a> (<code>'minf'</code>) atom, and <a href='Bin_T_MP4_ISOMediaBoxes_UserDataBox.md'>user data</a> (<code>'udta'</code>) atom.<br></blockquote>

<h2>Remarks</h2>

Media atoms describe and define a track’s media type and sample data. The media atom contains information that specifies:<br>
The media type, such as sound or videoThe media handler component used to interpret the sample dataThe media timescale and track durationMedia-and-track-specific information, such as sound volume or graphics modeThe media data references, which typically specify the file where the sample data is storedThe sample table atoms, which, for each media sample, specify the sample description, duration, and byte offset from the data reference<br>
The media atom has an atom type of <code>'mdia'</code>.<br>
<br>
<img src='https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif' /> <b>Note</b> Do not confuse the media atom (<code>'mdia'</code>) with the media <i>data</i> atom (<code>'mdat'</code>). The media atom contains only <i>references</i> to media data; the media data atom contains the actual media samples.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <b>MP4.ISOMediaBoxes.MediaBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>