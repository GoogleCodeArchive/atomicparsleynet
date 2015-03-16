#summary ISOMediaBoxes.HandlerBox Binary Block

# ISOMediaBoxes.HandlerBox Binary Block #


Handler Atom `'hdlr'` — Handler Reference Atom or Metadata Handler Atom.

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<br>
<blockquote><h4>The layout of a handler reference atom</h4>
<img src='https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_031.gif' /></blockquote>


<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a>

<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_HandlerBox_ComponentType.md'>ComponentType</a>

<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>
Within a <a href='Bin_T_MP4_ISOMediaBoxes_MediaBox.md'>media atom</a> this is a <a href='T_MP4_AtomicCode.md'>four-character code</a> that identifies the type of the handler. Only two values are valid for this field: <code>'mhlr'</code> for media handlers and <code>'dhlr'</code> for data handlers.</blockquote>

<blockquote>Within a <a href='Bin_T_MP4_ISOMediaBoxes_MetaBox.md'>meta box</a> this is a predefined integer that is set to 0.</blockquote>


<a href='Bin_F_MP4_ISOMediaBoxes_HandlerBox_HandlerType.md'>HandlerType</a>

<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>
Within a <a href='Bin_T_MP4_ISOMediaBoxes_MediaBox.md'>media atom</a> this is a <a href='T_MP4_AtomicCode.md'>four-character code</a> that identifies the type of the media handler or data handler.</blockquote>

<blockquote>Within a <a href='Bin_T_MP4_ISOMediaBoxes_MetaBox.md'>meta box</a> this is a <a href='T_MP4_AtomicCode.md'>four-character code</a> that identifies the structure used in the metadata atom, set to <code>‘mdta’</code>.</blockquote>


<a href='Bin_F_MP4_ISOMediaBoxes_HandlerBox_Manufacturer.md'>Manufacturer</a>

<blockquote>Type: <b>UInt32</b> — a 32-bit (4 bytes) unsigned integer <br>Component manufacturer. Reserved. Set to 0.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_HandlerBox_ComponentFlags.md'>ComponentFlags</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>Component flags. Reserved. Set to 0.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_HandlerBox_ComponentFlagsMask.md'>ComponentFlagsMask</a>

<blockquote>Type: <b>Int32</b> — a 32-bit (4 bytes) integer <br>Component flags mask. Reserved. Set to 0.<br></blockquote>

<i>ComponentName length</i>

<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) integer (Optional)<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_HandlerBox_ComponentName.md'>ComponentName</a>

<blockquote>Type: <b>CString</b> — a null-terminated UTF-8 encoded character string<br>A (counted) string that specifies the name of the component — that is, the media/metadata handler used when this media was created. This field may contain a zero-length (empty) string.<br></blockquote>

<h2>Remarks</h2>

The handler atom within a <a href='Bin_T_MP4_ISOMediaBoxes_MediaBox.md'>media atom</a> declares the process by which the media data in the stream may be presented, and thus, the nature of the media in a stream. For example, a video handler would handle a video track.<br>
<br>
The handler reference atom specifies the media handler component that is to be used to interpret the media’s data. The handler reference atom has an atom type value of <code>'hdlr'</code>.<br>
<br>
Historically, the handler reference atom was also used for data references. However, this use is no longer current and may now be safely ignored.<br>
<br>
This box when present within a <a href='Bin_T_MP4_ISOMediaBoxes_MetaBox.md'>meta box</a>, declares the structure or format of the <code>'meta'</code> box contents. It defines the structure used for all types of metadata stored within the metadata atom.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.HandlerBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>