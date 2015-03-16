# Introduction #

AtomicParsley .NET porting. Unofficial alpha version
  * Console application
  * GPAC boxes Framework
  * ID3 tags Framework
  * Documentation and Specification

## Usage ##

AtomicParlsey sets metadata into MPEG-4 files & derivatives supporting 3 tag schemes: iTunes-style, 3GPP assets & ISO defined copyright notifications.

AtomicParlsey quick help for setting iTunes-style metadata into MPEG-4 files.

<pre>
Usage:	AtomicParsley.NET [mp4FILE**]... [OPTION**]... [ARGUMENT**]... [ [OPTION2**]...[ARGUMENT2**]...]<br>
--version<br>
--help<br>
--longhelp<br>
--genre-list<br>
--stik-list<br>
--language-list<br>
--mac-language-list<br>
--ratings-list<br>
--genre-movie-id-list<br>
--genre-tv-id-list<br>
--imagetype-list<br>
--brands<br>
</pre>

# Changelog #

## GPAC Boxes Library v0.5.0.36 alpha ##
  * Implementation and specification of:
    * MovieHeaderBox
    * MovieBox
    * FileTypeBox
    * HandlerBox
    * MediaBox
    * MetaBox
    * !XMLBox
    * !BinaryXMLBox
    * ItemLocationBox
    * PrimaryItemBox
    * ItemProtectionBox
    * ItemInfoBox
    * ItemInfoEntryBox

## AtomicParsley.NET v0.9.6.36 alpha ##
  * Information options:
    * --version
    * --help
    * --longhelp
    * --genre-list
    * --stik-list
    * --language-list
    * --mac-language-list
    * --ratings-list
    * --genre-movie-id-list
    * --genre-tv-id-list
    * --imagetype-list
    * --brands

## ID3 Tags Library v0.9.6.29 alpha ##
  * ID3 frames version 2.x