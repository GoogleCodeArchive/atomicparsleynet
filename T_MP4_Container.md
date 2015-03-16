# Container Specification #

The International Standard specifies the ISO base media file format, which is a general format forming the basis for a number of other more specific file formats. This format contains the timing, structure, and media information for timed sequences of media data, such as audio/visual presentations.

For the purposes of this International Standard, the following terms and definitions apply.

| [Box or Atom](Bin_T_MP4_AtomicInfo.md) | An object-oriented building block defined by a unique type identifier and length. |
|:---------------------------------------|:----------------------------------------------------------------------------------|
| Chunk |  A contiguous set of samples for one track. |
| [Container Box](T_MP4_IBoxContainer.md) | A box whose sole purpose is to contain and group a set of related boxes. |
| Hint Track | A special track which does not contain media data. Instead it contains instructions for packaging one or more tracks into a streaming channel. |
| Hinter | A tool that is run on a file containing only media, to add one or more hint tracks to the file and so facilitate streaming. |
| ISO Base Media File | The name of the file format. This format was specified as ISO/IEC 14496-12 (MPEG-4 Part 12). The identical text is published as ISO/IEC 15444-12 (JPEG 2000, Part 12). |
| QuickTime File | The name of the QuickTime File Format (QTFF). This format has been used as the basis of the MPEG-4 standard and the JPEG-2000 standard, developed by the International Organization for Standardization (ISO Base Media File). Although these file types have similar structures and contain many functionally identical elements, they are distinct file types. |
| Sample | In non-hint tracks, a sample is an individual frame of video, a time-contiguous series of video frames, or a time-contiguous compressed section of audio. In hint tracks, a sample defines the formation of one or more streaming packets. No two samples within a track may share the same time-stamp. |
| Sample Description | A structure which defines and describes the format of some number of samples in a track. |
| Sample Table | A packed directory for the timing and physical layout of the samples in a track. |
| Track | A collection of related samples in a media file. For media data, a track corresponds to a sequence of images or sampled audio. For hint tracks, a track corresponds to a streaming channel. |

QuickTime movies are stored on disk, using two basic structures for storing information: atoms (also known as simple atoms, classic atoms or boxes) and QT atoms. Most atoms that you encounter in the QuickTime File Format are simple or classic atoms. Both simple atoms and QT atoms, however, allow you to construct arbitrarily complex hierarchical data structures. Both also allow your application to ignore data that they don’t understand.

A QuickTime file stores the description of its media separately from the media data.

The description is called the movie resource, movie atom, or simply the movie, and contains information such as the number of tracks, the video compression format, and timing information. The movie resource also contains an index describing where all the media data is stored.

The media data is the actual sample data, such as video frames and audio samples, used in the movie. The media data may be stored in the same file as the QuickTime movie, in a separate file, in multiple files, in alternate sources such as databases or real-time streams, or in some combination of these.