#summary ISOMediaBoxes.MetaBox Binary Block

# ISOMediaBoxes.MetaBox Binary Block #


Metadata Atom `'meta'`

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<br>
<blockquote><h4>Sample of a metadata atom and subatoms</h4>
<img src='https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/metadata_atom.jpg' /></blockquote>


<h2>Data Fields</h2>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Version.md'>Version</a> (Optional)<br>
<br>
<blockquote>Type: <b>Byte</b> — an 8-bit (1 byte) unsigned integer <br>The version of this atom.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_ISOMFullBox_Flags.md'>Flags</a> (Optional)<br>
<br>
<blockquote>Type: <b>UInt24</b> — a 24-bit (3 bytes) unsigned integer <br>Future flags.<br></blockquote>

<a href='Bin_F_MP4_ISOMediaBoxes_MetaBox_boxList.md'>boxList</a>

<blockquote>Type: <a href='Bin_T_MP4_AtomicInfo.md'>AtomicInfo</a>[] — a series of other atoms<br>
The container for metadata is an atom of type <code>‘meta’</code>. The metadata atom must contain the following subatoms:</blockquote>

<blockquote><a href='Bin_T_MP4_ISOMediaBoxes_HandlerBox.md'>metadata handler atom</a> (<code>‘hdlr’</code>), metadata item keys atom (<code>‘keys’</code>), and metadata <a href='Bin_T_MP4_ISOMediaBoxes_ItemListBox.md'>item list atom</a> (<code>‘ilst’</code>). Other optional atoms that may be contained in a metadata atom include the country list atom (<code>‘ctry’</code>), language list atom (<code>‘lang’</code>) and <a href='Bin_T_MP4_ISOMediaBoxes_FreeSpaceBox.md'>free space atom</a> (<code>‘free’</code>).</blockquote>


<h2>Remarks</h2>

Metadata can be defined as useful information related to media. This metadata format uses a key–value pair for each type of metadata being stored.<br>
<br>
<img src='https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif' /> <b>Note</b> The QuickTime file format also defines user data which, in some limited cases, can be used to store metadata.<br>
<br>
The meaning of a metadata item identifies what it represents: a copyright notice, the performer’s name, and so on. It uses an extensible namespace allowing for meanings or keys to be added, and then referenced, from metadata items. These keys may be four-character codes, names in reverse-address format (such as “com.apple.quicktime.windowlocation”) or any other key format including native formats from external metadata standards. A key is tagged with its namespace allowing for extension in the future. It is recommended that reverse-address format be used in the general case: this provides an extensible syntax for vendor data or for other organizations or standards bodies.<br>
<br>
Metadata is stored immediately in the corresponding atom structures, by value.<br>
<br>
A metadata item can be identified as specific to a country or set of countries, to a language or set of languages, or to some combination of languages and countries. This identification allows for a default value (suitable for any language or country not explicitly called out), a single value, or a list of values.<br>
<br>
The metadata can be stored within a movie atom (<code>‘moov’</code>), a track atom (<code>‘trak’</code>) or a media atom (<code>‘mdia’</code>). Only one metadata atom is allowed for each location. If there is user data and metadata stored in the same location, and both declare the same information, for example, declare a copyright notice, the metadata takes precedence.<br>
<br>
The metadata is required to contain a <code>‘hdlr’</code> box indicating the structure or format of the metadata contents. That metadata is located either within a box within this box (e.g. an XML box), or is located by the item identified by a primary item box.<br>
<br>
<h2>Inheritance Hierarchy</h2>
<a href='Bin_T_MP4_AtomicInfo.md'>MP4.AtomicInfo</a><br>  <a href='Bin_T_MP4_ISOMediaBoxes_ISOMFullBox.md'>MP4.ISOMediaBoxes.ISOMFullBox</a><br>    <b>MP4.ISOMediaBoxes.MetaBox</b><br>
<h2>See Also</h2>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>