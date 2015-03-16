#summary ISOMediaBoxes.MovieHeaderBox Binary Block

# ISOMediaBoxes.MovieHeaderBox Binary Block #


Movie Header Atom `'mvhd'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<br>
<blockquote><h4>The layout of a movie header atom</h4>
<img src='https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_095.gif' /></blockquote>


<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_CreationTime.md'>CreationTime</a>

<blockquote>Type: <b>MacDate64</b> — a Macintosh date as a 64-bit (8 bytes) number of seconds since January 1, 1904 (Version 1 only)<br>Type: <b>MacDate32</b> — a Macintosh date as a 32-bit (4 bytes) number of seconds since January 1, 1904 <br>Specifies when the movie atom was created.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_ModificationTime.md'>ModificationTime</a>

<blockquote>Type: <b>MacDate32</b> — a Macintosh date as a 32-bit (4 bytes) number of seconds since January 1, 1904 <br>Type: <b>MacDate64</b> — a Macintosh date as a 64-bit (8 bytes) number of seconds since January 1, 1904 (Version 1 only)<br>Specifies when the movie atom was changed.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_TimeScale.md'>TimeScale</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>A time value that indicates the time scale for this movie — that is, the number of time units that pass per second in its time coordinate system. A time coordinate system that measures time in sixtieths of a second, for example, has a time scale of 60.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_Duration.md'>Duration</a>

<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>Type: <b>Int64</b> — a 64-bit (8 bytes) integer (Version 1 only)<br>A time value that indicates the duration of the movie in time scale units. Note that this property is derived from the movie’s tracks. The value of this field corresponds to the duration of the longest track in the movie.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_PreferredRate.md'>PreferredRate</a>

<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>A <a href='T_MP4_Fixed_2.md'>fixed-point 16.16 number</a> that specifies the rate at which to play this movie. A value of 1.0 indicates normal rate.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_PreferredVolume.md'>PreferredVolume</a>

<blockquote>Type: <b>UInt16</b> — a 16-bit (2 bytes) unsigned integer <br>A <a href='T_MP4_Fixed_2.md'>fixed-point 8.8 number</a> that specifies how loud to play this movie’s sound. A value of 1.0 indicates full volume.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_Reserved.md'>Reserved</a>

<blockquote>Type: <b>Byte[</b><b>10]</b> — a stream of binary data with a fixed length equal to 10 <br>Reserved. Set to 0.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_Matrix.md'>Matrix</a>

<blockquote>Type: <b>UInt32[</b><b>9]</b> — an array of 32-bit unsigned integers with a fixed array items count equal to 9 <br>The <a href='T_MP4_AtomicInfo_TransformMatrix.md'>matrix structure</a> associated with this movie. A matrix shows how to map points from one coordinatespace into another.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_PreviewTime.md'>PreviewTime</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The time value in the movie at which the preview begins.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_PreviewDuration.md'>PreviewDuration</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The duration of the movie preview in movie time scale units.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_PosterTime.md'>PosterTime</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The time value of the time of the movie poster.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_SelectionTime.md'>SelectionTime</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The time value for the start time of the current selection.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_SelectionDuration.md'>SelectionDuration</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The duration of the current selection in movie time scale units.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_CurrentTime.md'>CurrentTime</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The time value for current time position within the movie.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MovieHeaderBox_NextTrackID.md'>NextTrackID</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>The track ID number of the next track added to this movie. Note that 0 is not a valid track ID value.<br></blockquote>

<h2>Remarks</h2>

You use the movie header atom to specify the characteristics of an entire movie. The data contained in this atom defines characteristics of the entire movie, such as time scale and duration.<br>
<br>
The movie header atom is a leaf atom. It has an atom type value of <code>'mvhd'</code>.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.MovieHeaderBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>