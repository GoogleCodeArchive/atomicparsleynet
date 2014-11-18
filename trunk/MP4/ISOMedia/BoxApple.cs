//------------------------------------------------------------------------------
// Copyright © FRA & FV 2014
// All rights reserved
//------------------------------------------------------------------------------
// MPEG-4 boxes Framework
//
// SVN revision information:
//   $Revision: 30 $
//------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using FRAFV.Binary.Serialization;

namespace MP4
{
	public sealed partial class ISOMediaBoxes
	{
		public sealed partial class DataBox : ISOMFullBox
		{
			public Encoding TextEncoding
			{
				get
				{
					switch (DataType)
					{
						case DataFlags.UTF8:
						case DataFlags.UTF8Sort:
						case DataFlags.ISRC:
						case DataFlags.MI3P:
						case DataFlags.URL:
						case DataFlags.UPC:
							return Encoding.UTF8;
						case DataFlags.SJIS:
							return Encoding.GetEncoding("shift_jis");
						case DataFlags.UTF16:
						case DataFlags.UTF16Sort:
							return Encoding.BigEndianUnicode;
						default:
							return Encoding.Default;
					}
				}
			}

			private void Read_data(System.IO.BinaryReader reader)
			{
				int len = (int)reader.Length();
				switch (DataType)
				{
					case DataFlags.UUID:
						if (len != 16)
							throw new FormatException("Invalid UUID size: " + len);
						data = new Guid(reader.ReadBytes(16));
						break;
					case DataFlags.Duration:
						if (len != 4)
							throw new FormatException("Invalid Duration size: " + len);
						data = new TimeSpan(reader.ReadInt32() * TimeSpan.TicksPerMillisecond);
						break;
					case DataFlags.DateTime:
						switch (len)
						{
							case 4:
								data = reader.ReadMacDate32();
								break;
							case 8:
								data = reader.ReadMacDate64();
								break;
							default:
								throw new FormatException("Invalid DataTime size: " + len);
						}
						break;
					case DataFlags.Int:
						switch (len)
						{
							case 1:
								data = reader.ReadSByte();
								break;
							case 2:
								data = reader.ReadInt16();
								break;
							case 4:
								data = reader.ReadInt32();
								break;
							case 8:
								data = reader.ReadInt64();
								break;
							default:
								throw new FormatException("Invalid Integer size: " + len);
						}
						break;
					case DataFlags.UInt:
						switch (len)
						{
							case 1:
								data = reader.ReadByte();
								break;
							case 2:
								data = reader.ReadUInt16();
								break;
							case 4:
								data = reader.ReadUInt32();
								break;
							case 8:
								data = reader.ReadUInt64();
								break;
							default:
								throw new FormatException("Invalid Unsigned Integer size: " + len);
						}
						break;
					case DataFlags.Single:
						if (len != 4)
							throw new FormatException("Invalid Single size: " + len);
						data = reader.ReadSingle();
						break;
					case DataFlags.Double:
						//case DataFlags.RIAAPA:
						if (len == 1)
							data = reader.ReadByte();
						else if (len == 8)
							data = reader.ReadDouble();
						else
							throw new FormatException("Invalid Double size: " + len);
						break;
					default:
						if (ValueIsText)
						{
							var encoder = new BOMReader(reader.BaseStream, TextEncoding);
							data = new string(encoder.ReadChars(len));
						}
						else
						{
							data = reader.ReadBytes(len);
						}
						break;
				}
			}

			private long Size_data()
			{
				ResolveData();
				switch (DataType)
				{
					case DataFlags.UUID:
						return 16;
					case DataFlags.Duration:
						return 4;
					case DataFlags.DateTime:
						return (DateTime)data > BinConverter.MaxMacDate32 ? 8 : 4;
					case DataFlags.Int:
						if (data is sbyte)
							return 1;
						else if (data is short)
							return 2;
						else if (data is int)
							return 4;
						else if (data is long)
							return 8;
						else
							throw new InvalidCastException("Invalid Integer value type: " + data.GetType());
					case DataFlags.UInt:
						if (data is byte)
							return 1;
						else if (data is ushort)
							return 2;
						else if (data is uint)
							return 4;
						else if (data is ulong)
							return 8;
						else
							throw new InvalidCastException("Invalid Unsigned Integer value type: " + data.GetType());
					case DataFlags.Single:
						return 4;
					case DataFlags.Double:
						//case DataFlags.RIAAPA:
						return data is byte ? 1 : 8;
					default:
						if (ValueIsText)
							return TextEncoding.GetByteCount((string)data ?? String.Empty);
						return ((byte[])data ?? new byte[0]).Length;
				}
			}

			private void Write_data(System.IO.BinaryWriter writer)
			{
				ResolveData();
				switch (DataType)
				{
					case DataFlags.UUID:
						writer.Write(((Guid)data).ToByteArray());
						break;
					case DataFlags.Duration:
						writer.Write((int)(((TimeSpan)data).Ticks / TimeSpan.TicksPerMillisecond));
						break;
					case DataFlags.DateTime:
						if ((DateTime)data > BinConverter.MaxMacDate32)
							writer.WriteMacDate64((DateTime)data);
						else
							writer.WriteMacDate32((DateTime)data);
						break;
					case DataFlags.Int:
						if (data is sbyte)
							writer.Write((sbyte)data);
						else if (data is short)
							writer.Write((short)data);
						else if (data is int)
							writer.Write((int)data);
						else if (data is long)
							writer.Write((long)data);
						else
							throw new InvalidCastException("Invalid Integer value type: " + data.GetType());
						break;
					case DataFlags.UInt:
						if (data is byte)
							writer.Write((byte)data);
						else if (data is ushort)
							writer.Write((ushort)data);
						else if (data is uint)
							writer.Write((uint)data);
						else if (data is ulong)
							writer.Write((ulong)data);
						else
							throw new InvalidCastException("Invalid Unsigned Integer value type: " + data.GetType());
						break;
					case DataFlags.Single:
						writer.Write((float)data);
						break;
					case DataFlags.Double:
						//case DataFlags.RIAAPA:
						if (data is byte)
							writer.Write((byte)data);
						else
							writer.Write((double)data);
						break;
					default:
						if (ValueIsText)
						{
							var encoder = new BOMWriter(writer.BaseStream, TextEncoding);
							encoder.Write(((string)data ?? String.Empty).ToCharArray());
						}
						else
						{
							writer.Write((byte[])data ?? new byte[0]);
						}
						break;
				}
			}

			public bool ValueIsText
			{
				get
				{
					switch (DataType)
					{
						case DataFlags.UTF8:
						case DataFlags.UTF16:
						case DataFlags.SJIS:
						case DataFlags.UTF8Sort:
						case DataFlags.UTF16Sort:
						case DataFlags.ISRC:
						case DataFlags.MI3P:
						case DataFlags.URL:
						case DataFlags.UPC:
							return true;
						default:
							return false;
					}
				}
			}

			public bool ValueIsBinary
			{
				get
				{
					switch (DataType)
					{
						case DataFlags.UUID:
						case DataFlags.Duration:
						case DataFlags.DateTime:
						case DataFlags.Int:
						case DataFlags.UInt:
						case DataFlags.Single:
						case DataFlags.Double:
							return false;
						default:
							return !ValueIsText;
					}
				}
			}

			[XmlIgnore]
			public object Value
			{
				get
				{
					ResolveData();
					return data;
				}
				set
				{
					if (value == null)
						DataType = DataFlags.Binary;
					else if (value is Guid)
						DataType = DataFlags.UUID;
					else if (value is TimeSpan)
						DataType = DataFlags.Duration;
					else if (value is DateTime)
						DataType = DataFlags.DateTime;
					else if (value is sbyte || value is short || value is int || value is long)
						DataType = DataFlags.Int;
					else if (value is byte || value is ushort || value is uint || value is ulong)
						DataType = DataFlags.UInt;
					else if (value is float)
						DataType = DataFlags.Single;
					else if (value is double)
						DataType = DataFlags.Double;
					else
						DataType = DataFlags.Binary;
					data = value;
					valueSize = 0;
				}
			}
		}
	}
}
