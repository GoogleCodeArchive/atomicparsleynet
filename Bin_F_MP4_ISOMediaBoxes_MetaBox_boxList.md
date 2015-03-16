#summary boxList Binary Data Field

# boxList Binary Data Field #


The container for metadata is an atom of type `‘meta’`. The metadata atom must contain the following subatoms:

[metadata handler atom](Bin_T_MP4_ISOMediaBoxes_HandlerBox.md) (`‘hdlr’`), metadata item keys atom (`‘keys’`), and metadata [item list atom](Bin_T_MP4_ISOMediaBoxes_ItemListBox.md) (`‘ilst’`). Other optional atoms that may be contained in a metadata atom include the country list atom (`‘ctry’`), language list atom (`‘lang’`) and [free space atom](Bin_T_MP4_ISOMediaBoxes_FreeSpaceBox.md) (`‘free’`).

**Serialization Namespace:**  [MP4](Bin_N_MP4.md)<br><b>Assembly:</b>  MP4 (in MP4.dll)<br>
<h2>Data Serialization</h2>

<h3>Field Value</h3>

Type: <a href='Bin_T_MP4_AtomicInfo.md'>AtomicInfo</a>[] — a series of other atoms<br>
<br>
<h2>Remarks</h2>

The country list and language list atoms can be used to store localized data in an efficient manner. The free space atom may be used to reserve space in a metadata atom for later additions to it, or to zero out bytes within a metadata atom after editing and removing elements from it. The free space atom may not occur within any other subatom contained in the metadata atom.<br>
<br>
The metadata is required to contain a <code>‘hdlr’</code> box indicating the structure or format of the metadata contents. That metadata is located either within a box within this box (e.g. an XML box), or is located by the item identified by a primary item box.<br>
<br>
All other contained boxes are specific to the format specified by the handler box.<br>
<br>
The other boxes defined here may be defined as optional or mandatory for a given format. If they are used, then they must take the form specified here. These optional boxes include a data-information box, which documents other files in which metadata values (e.g. pictures) are placed, and a item location box, which documents where in those files each item is located (e.g. in the common case of multiple pictures stored in the same file). At most one meta box may occur at each of the file level, movie level, or track level.<br>
<br>
If an <a href='Bin_T_MP4_ISOMediaBoxes_ItemProtectionBox.md'>ItemProtectionBox</a> occurs, then some or all of the meta-data, including possibly the primary resource, may have been protected and be un-readable unless the protection system is taken into account.<br>
<br>
<h2>See Also</h2>

<a href='Bin_T_MP4_ISOMediaBoxes_MetaBox.md'>ISOMediaBoxes.MetaBox Block</a>

<a href='Bin_N_MP4.md'>MP4 Serialization Namespace</a>