//==================================================================//
/*
    AtomicParsley - id3v2types.h

    AtomicParsley is GPL software; you can freely distribute, 
    redistribute, modify & use under the terms of the GNU General
    Public License; either version 2 or its successor.

    AtomicParsley is distributed under the GPL "AS IS", without
    any warranty; without the implied warranty of merchantability
    or fitness for either an expressed or implied particular purpose.

    Please see the included GNU General Public License (GPL) for 
    your rights and further details; see the file COPYING. If you
    cannot, write to the Free Software Foundation, 59 Temple Place
    Suite 330, Boston, MA 02111-1307, USA.  Or www.fsf.org

    Copyright ©2006-2007 puck_lock
    with contributions from others; see the CREDITS file
    ----------------------
    SVN revision information:
      $Revision$
                                                                   */
//==================================================================//
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ID3v2.Frames
{
	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class UnknownFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.Unknown; } }
		private ID3v2Fields.UnknownField Field;
		[XmlIgnore]
		public byte[] Data
		{
			get { return this.Field.Data; }
			set { this.Field.Data = value; }
		}
		[XmlElement("Data")]
		public DataXMLSerializer DataSerializer
		{
			get { return new DataXMLSerializer(Data); }
			set { Data = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.Field }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.Unknown:
				this.Field.Data = data;
				break;
			}
		}
		public UnknownFrame()
		{
			this.Field.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class TextFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.Text; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.TextField TextField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlElement]
		public string[] Text
		{
			get
			{
				return this.TextField.GetText(TextEncoding, binOpt.Latin1, TextSeparator);
			}
			set
			{
				this.TextField.SetText(TextEncoding, binOpt.Latin1, TextSeparator, value);
			}
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.TextField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.Text:
				this.TextField.Data = data;
				break;
			}
		}
		public TextFrame()
		{
			this.TextField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class TextFrameUserDef : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.TextUserDef; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.DescriptionField DescriptionField;
		private ID3v2Fields.TextField TextField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlElement, DefaultValue("")]
		public string Description
		{
			get { return this.DescriptionField.GetDescription(TextEncoding); }
			set { this.DescriptionField.SetDescription(TextEncoding, value); }
		}
		[XmlElement]
		public string[] Text
		{
			get { return this.TextField.GetText(TextEncoding, binOpt.Latin1, TextSeparator); }
			set { this.TextField.SetText(TextEncoding, binOpt.Latin1, TextSeparator, value); }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.DescriptionField, this.TextField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.Description:
				this.DescriptionField.Data = data;
				break;
			case FieldTypes.Text:
				this.TextField.Data = data;
				break;
			}
		}
		public TextFrameUserDef()
		{
			this.DescriptionField.Data = EmptyData;
			this.TextField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class URLFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.URL; } }
		private ID3v2Fields.URLField URLField;
		[XmlElement, DefaultValue("")]
		public string URL
		{
			get { return this.URLField.URL; }
			set { this.URLField.URL = value; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.URLField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.URL:
				this.URLField.Data = data;
				break;
			}
		}
		public URLFrame()
		{
			this.URLField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class URLFrameUserDef : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.URLUserDef; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.DescriptionField DescriptionField;
		private ID3v2Fields.URLField URLField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlElement, DefaultValue("")]
		public string Description
		{
			get { return this.DescriptionField.GetDescription(TextEncoding); }
			set { this.DescriptionField.SetDescription(TextEncoding, value); }
		}
		[XmlElement, DefaultValue("")]
		public string URL
		{
			get { return this.URLField.URL; }
			set { this.URLField.URL = value; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.DescriptionField, this.URLField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.Description:
				this.DescriptionField.Data = data;
				break;
			case FieldTypes.URL:
				this.URLField.Data = data;
				break;
			}
		}
		public URLFrameUserDef()
		{
			this.DescriptionField.Data = EmptyData;
			this.URLField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class UniqueFileIDFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.UniqueFileID; } }
		private ID3v2Fields.OwnerField OwnerField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute, DefaultValue("")]
		public string Owner
		{
			get { return this.OwnerField.Owner; }
			set { this.OwnerField.Owner = value; }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.OwnerField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.Owner:
				this.OwnerField.Data = data;
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public UniqueFileIDFrame()
		{
			this.OwnerField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class CDIDFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.CDID; } }
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public CDIDFrame()
		{
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class DescribedTextFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.DescribedText; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.LanguageField LanguageField;
		private ID3v2Fields.DescriptionField DescriptionField;
		private ID3v2Fields.TextField TextField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlAttribute, DefaultValue("")]
		public string Language
		{
			get { return this.LanguageField.LanguageCode; }
			set { this.LanguageField.LanguageCode = value; }
		}
		[XmlAttribute, DefaultValue("")]
		public string Description
		{
			get { return this.DescriptionField.GetDescription(TextEncoding); }
			set { this.DescriptionField.SetDescription(TextEncoding, value); }
		}
		[XmlElement]
		public string[] Text
		{
			get { return this.TextField.GetText(TextEncoding, binOpt.Latin1, TextSeparator); }
			set { this.TextField.SetText(TextEncoding, binOpt.Latin1, TextSeparator, value); }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.LanguageField, this.DescriptionField, this.TextField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.Language:
				if (data.Length >= 3)
					this.LanguageField.Data = data;
				else
					this.LanguageField.Data = ID3v2Fields.LanguageField.EmptyData;
				break;
			case FieldTypes.Description:
				this.DescriptionField.Data = data;
				break;
			case FieldTypes.Text:
				this.TextField.Data = data;
				break;
			}
		}
		public DescribedTextFrame()
		{
			this.LanguageField.Data = ID3v2Fields.LanguageField.EmptyData;
			this.DescriptionField.Data = EmptyData;
			this.TextField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class AttachedPictureFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.AttachedPicture; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.MIMETypeField MIMETypeField;
		private ID3v2Fields.PicTypeField PicTypeField;
		private ID3v2Fields.DescriptionField DescriptionField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlAttribute, DefaultValue("")]
		public string MIMEType
		{
			get { return this.MIMETypeField.MIMEType; }
			set { this.MIMETypeField.MIMEType = value; }
		}
		[XmlAttribute, DefaultValue((byte)0)]
		public byte PicType
		{
			get { return this.PicTypeField.Data; }
			set { this.PicTypeField.Data = value; }
		}
		[XmlElement, DefaultValue("")]
		public string Description
		{
			get { return this.DescriptionField.GetDescription(TextEncoding); }
			set { this.DescriptionField.SetDescription(TextEncoding, value); }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.MIMETypeField, this.PicTypeField, this.DescriptionField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.MIMEType:
				this.MIMETypeField.Data = data;
				break;
			case FieldTypes.PicType:
				if (data.Length > 0)
					this.PicTypeField.Data = data[0];
				break;
			case FieldTypes.Description:
				this.DescriptionField.Data = data;
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public AttachedPictureFrame()
		{
			this.MIMETypeField.Data = EmptyData;
			this.DescriptionField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class AttachedObjectFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.AttachedObject; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.MIMETypeField MIMETypeField;
		private ID3v2Fields.FileNameField FileNameField;
		private ID3v2Fields.DescriptionField DescriptionField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlAttribute, DefaultValue("")]
		public string MIMEType
		{
			get { return this.MIMETypeField.MIMEType; }
			set { this.MIMETypeField.MIMEType = value; }
		}
		[XmlAttribute, DefaultValue("")]
		public string FileName
		{
			get { return this.FileNameField.GetFileName(TextEncoding); }
			set { this.FileNameField.SetFileName(TextEncoding, value); }
		}
		[XmlElement, DefaultValue("")]
		public string Description
		{
			get { return this.DescriptionField.GetDescription(TextEncoding); }
			set { this.DescriptionField.SetDescription(TextEncoding, value); }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.MIMETypeField, this.FileNameField, this.DescriptionField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.MIMEType:
				this.MIMETypeField.Data = data;
				break;
			case FieldTypes.Filename:
				this.FileNameField.Data = data;
				break;
			case FieldTypes.Description:
				this.DescriptionField.Data = data;
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public AttachedObjectFrame()
		{
			this.MIMETypeField.Data = EmptyData;
			this.FileNameField.Data = EmptyData;
			this.DescriptionField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class GroupIDFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.GroupID; } }
		private ID3v2Fields.OwnerField OwnerField;
		private ID3v2Fields.GroupSymbolField GroupSymbolField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute, DefaultValue("")]
		public string Owner
		{
			get { return this.OwnerField.Owner; }
			set { this.OwnerField.Owner = value; }
		}
		[XmlAttribute, DefaultValue((byte)0)]
		public byte GroupSymbol
		{
			get { return this.GroupSymbolField.Data; }
			set { this.GroupSymbolField.Data = value; }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.OwnerField, this.GroupSymbolField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.Owner:
				this.OwnerField.Data = data;
				break;
			case FieldTypes.GroupSymbol:
				if (data.Length > 0)
					this.GroupSymbolField.Data = data[0];
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public GroupIDFrame()
		{
			this.OwnerField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class SignatureFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.Signature; } }
		private ID3v2Fields.GroupSymbolField GroupSymbolField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute, DefaultValue((byte)0)]
		public byte GroupSymbol
		{
			get { return this.GroupSymbolField.Data; }
			set { this.GroupSymbolField.Data = value; }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.GroupSymbolField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.GroupSymbol:
				if (data.Length > 0)
					this.GroupSymbolField.Data = data[0];
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public SignatureFrame()
		{
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class PrivateFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.Private; } }
		private ID3v2Fields.OwnerField OwnerField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute, DefaultValue("")]
		public string Owner
		{
			get { return this.OwnerField.Owner; }
			set { this.OwnerField.Owner = value; }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.OwnerField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.Owner:
				this.OwnerField.Data = data;
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public PrivateFrame()
		{
			this.OwnerField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class PlayCounterFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.PlayCounter; } }
		private ID3v2Fields.CounterField CounterField;
		[XmlElement]
		public long Counter
		{
			get { return this.CounterField.Counter; }
			set { this.CounterField.Counter = value; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.CounterField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.Counter:
				if (data.Length >= 4)
					this.CounterField.Data = data;
				else
					this.CounterField.Data = ID3v2Fields.CounterField.EmptyData;
				break;
			}
		}
		public PlayCounterFrame()
		{
			this.CounterField.Data = ID3v2Fields.CounterField.EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class PopularFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.Popular; } }
		private ID3v2Fields.OwnerField OwnerField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		private ID3v2Fields.CounterField CounterField;
		[XmlAttribute, DefaultValue("")]
		public string Owner
		{
			get { return this.OwnerField.Owner; }
			set { this.OwnerField.Owner = value; }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		[XmlElement]
		public long Counter
		{
			get { return this.CounterField.Counter; }
			set { this.CounterField.Counter = value; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.OwnerField, this.BinaryDataField, this.CounterField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.Owner:
				this.OwnerField.Data = data;
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			case FieldTypes.Counter:
				if (data.Length >= 4)
					this.CounterField.Data = data;
				else
					this.CounterField.Data = ID3v2Fields.CounterField.EmptyData;
				break;
			}
		}
		public PopularFrame()
		{
			this.OwnerField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
			this.CounterField.Data = ID3v2Fields.CounterField.EmptyData;
		}
	}

	[XmlRoot(Namespace = ID3v2Tag.XMLNS, IsNullable = false)]
	public class OldV2P2PictureFrame : ID3v2Frame
	{
		public override FrameType Type { get { return FrameType.OldV2p2Picture; } }
		private ID3v2Fields.TextEncodingField TextEncodingField;
		private ID3v2Fields.ImageFormatField ImageFormatField;
		private ID3v2Fields.PicTypeField PicTypeField;
		private ID3v2Fields.DescriptionField DescriptionField;
		private ID3v2Fields.BinaryDataField BinaryDataField;
		[XmlAttribute]
		public TextEncodings TextEncoding
		{
			get { return this.TextEncodingField.TextEncoding; }
			set { this.TextEncodingField.TextEncoding = value; }
		}
		[XmlElement]
		public byte[] ImageFormat
		{
			get { return this.ImageFormatField.Data; }
			set { this.ImageFormatField.Data = value; }
		}
		[XmlAttribute, DefaultValue((byte)0)]
		public byte PicType
		{
			get { return this.PicTypeField.Data; }
			set { this.PicTypeField.Data = value; }
		}
		[XmlElement, DefaultValue("")]
		public string Description
		{
			get { return this.DescriptionField.GetDescription(TextEncoding); }
			set { this.DescriptionField.SetDescription(TextEncoding, value); }
		}
		[XmlIgnore]
		public byte[] BinaryData
		{
			get { return this.BinaryDataField.Data; }
			set { this.BinaryDataField.Data = value; }
		}
		[XmlElement("BinaryData")]
		public DataXMLSerializer BinaryDataSerializer
		{
			get { return new DataXMLSerializer(BinaryData); }
			set { BinaryData = value.Data; }
		}
		internal override IID3v2Field[] Fields
		{
			get { return new IID3v2Field[] { this.TextEncodingField, this.ImageFormatField, this.PicTypeField, this.DescriptionField, this.BinaryDataField }; }
		}
		internal override void SetFieldData(FieldTypes field, byte[] data)
		{
			switch (field)
			{
			case FieldTypes.TextEncoding:
				if (data.Length > 0)
					this.TextEncodingField.Data = data[0];
				break;
			case FieldTypes.ImageFormat:
				this.ImageFormatField.Data = data;
				break;
			case FieldTypes.PicType:
				if (data.Length > 0)
					this.PicTypeField.Data = data[0];
				break;
			case FieldTypes.Description:
				this.DescriptionField.Data = data;
				break;
			case FieldTypes.BinaryData:
				this.BinaryDataField.Data = data;
				break;
			}
		}
		public OldV2P2PictureFrame()
		{
			this.ImageFormatField.Data = EmptyData;
			this.DescriptionField.Data = EmptyData;
			this.BinaryDataField.Data = EmptyData;
		}
	}
}
