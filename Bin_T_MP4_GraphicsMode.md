#summary GraphicsMode Enumeration

# GraphicsMode Enumeration #


Graphics Modes

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Members</h2>

<table><thead><th> <b>Name</b> </th><th> <b>Value</b> </th><th> <b>Description</b> </th></thead><tbody>
<tr><td> <b>SourceCopy</b> </td><td> 0 </td><td> Copy the source image over the destination. </td></tr>
<tr><td> <b>DitherCopy</b> </td><td> 64 </td><td> Dither the image (if needed), otherwise do a copy. </td></tr>
<tr><td> <b>Blend</b> </td><td> 32 </td><td> Replaces destination pixel with a blend of the source and destination pixel colors, with the proportion for each channel controlled by that channel in the opcolor. </td></tr>
<tr><td> <b>Transparent</b> </td><td> 36 </td><td> Replaces the destination pixel with the source pixel if the source pixel isn't equal to the opcolor. </td></tr>
<tr><td> <b>StraightAlpha</b> </td><td> 256 </td><td> Replaces the destination pixel with a blend of the source and destination pixels, with the proportion controlled by the alpha channel. </td></tr>
<tr><td> <b>PremulWhiteAlpha</b> </td><td> 257 </td><td> Premultiplied with white means that the color components of each pixel have already been blended with a white pixel, based on their alpha channel value. Effectively, this means that the image has already been combined with a white background. First, remove the white from each pixel and then blend the image with the actual background pixels. </td></tr>
<tr><td> <b>PremulBlackAlpha</b> </td><td> 258 </td><td> Premultiplied with black is the same as pre-multiplied with white, except the background color that the image has been blended with is black instead of white. </td></tr>
<tr><td> <b>StraightAlphaBlend</b> </td><td> 260 </td><td> Similar to straight alpha, but the alpha value used for each channel is the combination of the alpha channel and that channel in the opcolor. </td></tr>
<tr><td> <b>Composition</b> </td><td> 259 </td><td> (Tracks only) The track is drawn offscreen, and then composed onto the screen using dither copy. </td></tr></tbody></table>

<h2>Remarks</h2>

QuickTime files use graphics modes to describe how one video or graphics layer should be combined with the layers beneath it. Graphics modes are also known as transfer modes. Some graphics modes require a color to be specified for certain operations, such as blending to determine the blend level. QuickTime uses the graphics modes defined by Apple’s QuickDraw.<br>
<br>
The most common graphics modes are and <code>ditherCopy</code>, which simply indicate that the image should not blend with the image behind it, but overwrite it. QuickTime also defines several additional graphics modes.<br>
<br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>