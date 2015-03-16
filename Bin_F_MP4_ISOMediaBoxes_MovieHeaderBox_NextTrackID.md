#summary NextTrackID Binary Data Field

# NextTrackID Binary Data Field #


The track ID number of the next track added to this movie. Note that 0 is not a valid track ID value.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>
Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>
<h2>Remarks</h2>

The value of NextTrackID shall be larger than the largest track-ID in use. If this value is equal to or larger than all 1s (32-bit maxint), and a new media track is to be added, then a search must be made in the file for a unused track identifier.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_MovieHeaderBox.md'>ISOMediaBoxes.MovieHeaderBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>