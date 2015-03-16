# Container Box #

An atom that contains other atoms is called a container atom. The parent atom is the container atom exactly one level above a given atom in the hierarchy. An atom that does not contain other atoms is called a leaf atom, and typically contains data as one or more fields or tables. Some leaf atoms act as flags or placeholders, however, and contain no data beyond their size and type fields.

> #### A sample atom ####
![https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_027.gif](https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_027.gif)

Atoms within container atoms do not generally have to be in any particular order, unless such an order is specifically called out in their container. One such example is the handler description atom, which must come before the data being handled. For example, a media handler description atom must come before a media information atom, and a data handler description atom must come before a data information atom.

QT atoms are an enhanced data structure that provide a more general-purpose storage format and remove some of the ambiguities that arise when using simple atoms. A QT atom has an expanded header; the size and type fields are followed by fields for an atom ID and a count of child atoms.

This allows multiple child atoms of the same type to be specified through identification numbers. It also makes it possible to parse the contents of a QT atom of unknown type, by walking the tree of its child atoms.

QT atoms are normally wrapped in an atom container, a data structure with a header containing a lock count. Each atom container contains exactly one root atom, which is the QT atom. Atom containers are not atoms, and are not found in the hierarchy of atoms that makes up a QuickTime movie file. Atom containers may be found as data structures inside some atoms, however. Examples include media input maps and media property atoms.

![https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif](https://atomicparsleynet.googlecode.com/svn/branches/Sandcastle/Presentation/vs2005/icons/alert_note.gif) **Note** An **atom container** is _not_ the same as a **container atom**. An atom container is a _container_, not an atom.

> #### QT atom layout ####
![https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_200.gif](https://atomicparsleynet.googlecode.com/svn/trunk/MP4/Help/qt_l_200.gif)

Each QT atom starts with a QT atom container header, followed by the root atom. The root atom’s type is the QT atom’s type. The root atom contains any other atoms that are part of the structure.

Each container atom starts with a QT atom header followed by the atom’s contents. The contents are either child atoms or data, but never both. If an atom contains children, it also contains all of its children’s data and descendants. The root atom is always present and never has any siblings.