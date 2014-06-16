// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Column.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Kuick.Data
{
	public class Column : ICloneable<Column>
	{
		#region constructor
		public Column() 
		{
		}
		#endregion

		#region ICloneable<T>
		public Column Clone()
		{
			Column clone = new Column();
			clone.TableName = TableName;
			clone.EntityName = EntityName;
			clone.Category = Category;
			clone.Description = Description;
			clone.Property = Property;
			clone.Spec = Spec.Clone();
			clone.Refer = null == Refer ? null : Refer.Clone();
			clone.Initiate = null == Initiate ? null : Initiate.Clone();
			clone.Identity = null == Identity ? null : Identity.Clone();
			clone.Visual = null == Visual ? null : Visual.Clone();
			clone.Encryption = null == Encryption ? null : Encryption.Clone();
			clone.Validations = Validations;
			clone.SkipDiff = SkipDiff;
			return clone;
		}
		#endregion

		#region propety
		public string TableName { get; internal set; }
		public string EntityName { get; internal set; }

		public CategoryAttribute Category { get; internal set; }
		public DescriptionAttribute Description { get; internal set; }
		public PropertyInfo Property { get; internal set; }

		public ColumnSpec Spec { get; internal set; }
		public ColumnRefer Refer { get; internal set; }
		public ColumnInitiate Initiate { get; internal set; }
		public ColumnIdentity Identity { get; internal set; }
		public ColumnVisual Visual { get; internal set; }

		public ColumnEncrypt Encryption { get; internal set; }
		public List<IValidation> Validations { get; internal set; }

		public string Title { get { return Description.Description; } }
		public bool SkipDiff { get; internal set; }
		#endregion

		#region Validate
		public Result Validate(object value)
		{
			Result result = new Result();
			foreach(IValidation x in Validations) {
				Result innerResult = new Result(false);
				try {
					innerResult = x.Validate(this, value);
				} catch(Exception ex) {
					Logger.Error(
						"Kuick.Data.Column.Validate",
						ex.ToAny(
							new Any("validation", x.GetType().Name)
						)
					);
					innerResult.Success = false;
				}
				result.InnerResults.Add(innerResult);
			}

			return result;
		}
		#endregion

		#region assistance
		/// <summary>
		/// [T_USER].[NAME]
		/// </summary>
		public string BuildFullName(Func<string, string> tag)
		{
			return tag(FullName);
		}

		/// <summary>
		/// [T_USER].[NAME] AS T_USER_K_NAME
		/// </summary>
		public string BuildFullNameAsName(
			Func<string, string> tag, 
			Func<string, string> unTag)
		{
			return DataUtility.FullNameToFullNameAsName(FullName, tag, unTag);
		}

		/// <summary>
		/// T_USER.NAME AS T_USER_K_NAME
		/// </summary>
		public string FullNameAsName
		{
			get
			{
				return DataUtility.FullNameToFullNameAsName(FullName);
			}
		}

		/// <summary>
		/// T_USER.NAME
		/// </summary>
		public string FullName
		{
			get
			{
				return string.Format("{0}.{1}", TableName, Spec.ColumnName);
			}
		}

		/// <summary>
		/// UserEntity.Name
		/// </summary>
		public string PropertyFullName
		{
			get
			{
				return string.Format("{0}.{1}", EntityName, Property.Name);
			}
		}

		/// <summary>
		/// T_USER_K_NAME
		/// </summary>
		public string AsName
		{
			get
			{
				return DataUtility.FullNameToAsName(FullName);
			}
		}

		public string AliasName 
		{
			get
			{
				return EntityCache.GetAlias(EntityName) + Spec.ColumnName;
			}
		}

		public bool AllowDBCS
		{
			get
			{
				return Spec.DbType.EnumIn(
					SqlDataType.MaxVarChar,
					SqlDataType.MaxVarWChar,
					SqlDataType.VarChar,
					SqlDataType.VarWChar,
					SqlDataType.WChar,
					SqlDataType.Char
				);
			}
		}

		public bool IsString
		{
			get
			{
				return Spec.DbType.EnumIn(
					SqlDataType.MaxVarChar,
					SqlDataType.MaxVarWChar,
					SqlDataType.VarChar,
					SqlDataType.VarWChar,
					SqlDataType.WChar,
					SqlDataType.Char
				);
			}
		}

		public bool IsBigString
		{
			get
			{
				return IsString
					? Spec.Length > 256
					: false;
			}
		}

		public bool IsBoolean
		{
			get
			{
				return Spec.DbType.EnumIn(
					SqlDataType.Boolean,
					SqlDataType.Bit
				);
			}
		}

		public bool IsNumber
		{
			get
			{
				return Spec.DbType.EnumIn(
					SqlDataType.Integer,
					SqlDataType.Double,
					SqlDataType.Decimal
				);
			}
		}

		public bool IsDateTime
		{
			get
			{
				return Spec.DbType.EnumIn(
					SqlDataType.TimeStamp
				);
			}
		}

		public bool IsEnum
		{
			get
			{
				return Spec.DbType.EnumIn(
					SqlDataType.Enum
				);
			}
		}

		public bool WideCharacter
		{
			get
			{
				return Spec.DbType.EnumIn<SqlDataType>(
					SqlDataType.MaxVarWChar,
					SqlDataType.VarWChar,
					SqlDataType.WChar
				);
			}
		}

		public int ByteCount
		{
			get
			{
				return WideCharacter ? Spec.Length * 2 : Spec.Length;
			}
		}

		private ColumnDataFormat _Format = ColumnDataFormat.Undefined;
		public ColumnDataFormat Format
		{
			get 
			{
				if (_Format != ColumnDataFormat.Undefined) { return _Format; }

				Type type = Property.PropertyType;
				if(type.IsString()) {
					_Format = ColumnDataFormat.String;
				} else if(type.IsInteger()) {
					_Format = ColumnDataFormat.Integer;
				} else if (type.IsInt64()) {
					_Format = ColumnDataFormat.Int64;
				} else if(type.IsDecimal()) {
					_Format = ColumnDataFormat.Decimal;
				} else if(type.IsLong()) {
					_Format = ColumnDataFormat.Long;
				} else if(type.IsShort()) {
					_Format = ColumnDataFormat.Short;
				} else if(type.IsDouble()) {
					_Format = ColumnDataFormat.Double;
				} else if(type.IsSingle()) {
					_Format = ColumnDataFormat.Float;
				} else if(type.IsBoolean()) {
					_Format = ColumnDataFormat.Boolean;
				} else if(type.IsChar()) {
					_Format = ColumnDataFormat.Char;
				} else if(type.IsEnum) {
					_Format = ColumnDataFormat.Enum;
				} else if(type.IsByte()) {
					_Format = ColumnDataFormat.Byte;
				} else if(type.IsByteArray()) {
					return ColumnDataFormat.ByteArray;
				} else if(type.IsDateTime()) {
					return ColumnDataFormat.DateTime;
				} else if(type.IsColor()) {
					return ColumnDataFormat.Color;
				} else if (type.IsGuid()) {
					return ColumnDataFormat.Guid;
				} else {
					Logger.Error(
						"Column.Format",
						"Unhandled Property Type",
						new Any("Property Type", type.Name)
					);
					throw new NotImplementedException();
				}

				return _Format;
			}
		}
		#endregion

		#region Internal
		internal void Foolproof()
		{
			Type type = Property.PropertyType;

			#region by PropertyType
			if(type.IsString()) {
				// String
				Spec.DbType = Spec.DbType == SqlDataType.Unknown 
					? SqlDataType.VarWChar : Spec.DbType;
				Spec.Length = Spec.Length == 0 ? 128 : Spec.Length;
				if(!Visual.Input.EnumIn(
						VisualInput.TextBox,
						VisualInput.TextArea,
						VisualInput.HtmlEditor,
						VisualInput.Time)) {
					Visual.Input = VisualInput.TextBox;
				}
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.String; 
				}
			} else if(type.IsEnum) {
				// Enum
				Spec.DbType = SqlDataType.Enum;
				Spec.Length = 128;
				Refer.Type = type;
				Visual.Input = Checker.IsFlagEnum(type)
					? VisualInput.CheckBoxList
					: VisualInput.RadioButtons; //VisualInput.DropDownList;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Enum;
				}
			} else if(type.IsBoolean()) {
				// Boolean
				Spec.DbType = SqlDataType.Bit;
				Visual.Input = VisualInput.CheckBox;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Boolean;
				}
			} else if(type.IsByte()) {
				// Byte
				Spec.DbType = SqlDataType.Bit;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Byte;
				}
			} else if(type.IsLong()) {
				// Long
				Spec.DbType = SqlDataType.Long;
				Spec.Length = 19;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Long;
				}
			} else if(type.IsFloat()) {
				// Float
				Spec.DbType = SqlDataType.Double;
				Spec.Length = 19;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Float;
				}
			} else if(type.IsDouble()) {
				// Double
				Spec.DbType = SqlDataType.Double;
				Spec.Length = 19;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Double;
				}
			} else if(type.IsInteger()) {
				// Integer
				Spec.DbType = SqlDataType.Integer;
				Spec.Length = 9;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Integer;
				}
			} else if (type.IsInt64()) {
				// Integer
				Spec.DbType = SqlDataType.Int64;
				Spec.Length = 18;
				if (Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Int64;
				}
			} else if(type.IsDateTime()) {
				// DateTime
				Spec.DbType = SqlDataType.TimeStamp;
				Spec.Length = 23;
				if(
					Visual.Input != VisualInput.Date
					&&
					Visual.Input != VisualInput.Time
					&&
					Visual.Input != VisualInput.TimeStamp) {
					Visual.Input = VisualInput.TimeStamp;
				}
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.DateTime;
				}
			} else if(type.IsDecimal()) {
				// Decimal
				Spec.DbType = SqlDataType.Decimal;
				Spec.Length = 12;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Decimal;
				}
			} else if(type.IsByteArray()) {
				// Byte[]
				Spec.DbType = SqlDataType.MaxVarBinary;
				Spec.Length = 102400;
				Visual.Input = VisualInput.FileUpload;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.ByteArray;
				}
			} else if(type.IsColor()) {
				// Color
				Spec.DbType = SqlDataType.Integer;
				Visual.Input = VisualInput.Color;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Color;
				}
			} else if(type.IsGuid()) {
				// Guid
				Spec.DbType = SqlDataType.Uuid;
				Visual.Input = VisualInput.TextBox;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Guid;
				}
			} else if(type.IsShort()) {
				// Short
				Spec.DbType = SqlDataType.Integer;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Short;
				}
			} else if(type.IsSingle()) {
				// Single
				Spec.DbType = SqlDataType.Integer;
				if(Initiate.Style == InitiateValue.None) {
					Initiate.Format = ColumnDataFormat.Integer;
				}
			} else {
				throw new KException(string.Format(
					"{0}.{1} property type doesnot match the spec type of {2}.",
					EntityName,
					Property.Name,
					Spec.DbType
				));
			}
			#endregion

			#region by Column Logic
			if(!type.IsEnum && Refer.HasType && !type.IsBoolean()) {
				Visual.Input = Checker.IsFlagEnum(Refer.Type)
					? VisualInput.CheckBoxList
					: VisualInput.DropDownList;
			}
			#endregion

			#region by InitiateValue
			string STORED_TYPE_ERROR = string.Format(
				"ColumnInitiate InitiateValue.{0} format value must be stored as {1} type.",
				Initiate.Style,
				"{0}"
			);
			try {
				switch(Initiate.Style) {
					case InitiateValue.None:
					case InitiateValue.Empty:
						break;
					case InitiateValue.Uuid:
					case InitiateValue.UuidAndAutoUpdate:
						if(!type.IsString()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "string")
							);
						}
						break;
					case InitiateValue.UniqueTicks:
					case InitiateValue.UniqueTicksAndAutoUpdate:
						break;
					case InitiateValue.NullDate:
					case InitiateValue.MaxDate:
						if(!type.IsDateTime()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "DateTime")
							);
						}
						break;
					case InitiateValue.Date4:
					case InitiateValue.Date8:
					case InitiateValue.Date8s:
					case InitiateValue.Date14:
					case InitiateValue.Date14s:
					case InitiateValue.Date17:
					case InitiateValue.Date17s:
					case InitiateValue.Date14AutoUpdate:
					case InitiateValue.Date17AutoUpdate:
						if(!type.IsString() && !type.IsDateTime()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "string/DateTime")
							);
						}
						break;
					case InitiateValue.IntegerAndAutoIncremental:
						if(!type.IsInteger()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "Integer")
							);
						}
						break;
					case InitiateValue.ToUpper:
					case InitiateValue.ToLower:
						if(!type.IsString()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "string")
							);
						}
						break;
					case InitiateValue.CurrentUser:
					case InitiateValue.CurrentUserAndAutoUpdate:
						Visual.Flag = Visual.Flag | VisualFlag.HideInModify;
						if(!type.IsString()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "string")
							);
						}
						break;
					case InitiateValue.TimsSpanHex:
						Visual.Flag = Visual.Flag | VisualFlag.HideInModify;
						if(!type.IsString()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "string")
							);
						}
						break;
					case InitiateValue.DaysOffset:
						if(!type.IsDateTime()) {
							throw new Exception(
								string.Format(STORED_TYPE_ERROR, "DateTime")
							);
						}
						break;
					default:
						throw new NotImplementedException(Initiate.Style.ToString());
				}
			} catch(Exception ex) {
				Logger.Error(
					"ColumnSpec.Foolproof: by SQLDefaultValue section",
					ex.ToAny(
					new Any("TableName", TableName),
					new Any("ColumnName", Spec.ColumnName)
				));
				throw;
			}
			#endregion

			#region by Identity
			if(null != Identity) {
				if(!type.IsInteger() && !type.IsString()) {
					throw new Exception(
						"ColumnIdentity value must be stored as integer or string type."
					);
				}
			}
			#endregion

			#region Correct
			// primary key
			if(Spec.PrimaryKey) {
				Spec.NotAllowNull = true;
				Spec.ReadOnly = true;
			}
			// Correct
			switch(Spec.DbType) {
				case SqlDataType.Unknown:
					Spec.Length = 256;
					Spec.DbType = SqlDataType.VarWChar;
					break;
				case SqlDataType.Long:
					Spec.Length = 19;
					break;
				case SqlDataType.Integer:
					Spec.Length = 9;
					break;
				case SqlDataType.Int64:
					Spec.Length = 18;
					break;
				case SqlDataType.Bit:
					Spec.Length = 5;
					break;
				case SqlDataType.Enum:
					Spec.Length = 128;
					break;
				case SqlDataType.Boolean:
					Spec.Length = 5;
					break;
				case SqlDataType.VarChar:
				case SqlDataType.VarWChar:
				case SqlDataType.Char:
				case SqlDataType.WChar:
					// string type default length
					if(Spec.Length <= 0) { Spec.Length = 256; }
					break;
				case SqlDataType.Decimal:
					break;
				case SqlDataType.Double:
					break;
				case SqlDataType.TimeStamp:
					Spec.Length = 23;
					break;
				case SqlDataType.MaxVarBinary:
					Spec.Length = 102400;
					break;
				case SqlDataType.MaxVarChar:
					Spec.Length = 102400;
					break;
				case SqlDataType.MaxVarWChar:
					Spec.Length = 102400;
					break;
				case SqlDataType.Uuid:
					Spec.DbType = SqlDataType.Uuid;
					Spec.Length = Math.Max(Spec.Length, 32);
					break;
				case SqlDataType.Identity:
					Spec.DbType = SqlDataType.Integer;
					Spec.Length = 9;
					Spec.NotAllowNull = true;
					Spec.ReadOnly = true;
					Spec.Identity = true;
					break;
				default:
					throw new NotImplementedException(string.Format(
						"ColumnSpec.Constructor: {0}",
						Spec.DbType.ToString()
					));
			}
			#endregion

			#region SqlDataType
			bool propErr = false;
			string errType = string.Empty;
			switch(Spec.DbType) {
				case SqlDataType.Bit:
				case SqlDataType.Boolean:
					if(
						!type.IsByte() 
						&& 
						!type.IsBoolean()){
							propErr = true;
							errType = typeof(Boolean).Name;
					};
					break;
				case SqlDataType.Decimal:
					if(!type.IsDecimal()) {
						propErr = true;
						errType = typeof(Decimal).Name;
					};
					break;
				case SqlDataType.Double:
					if(!type.IsDouble() && !type.IsFloat()) {
						propErr = true;
						errType = typeof(Double).Name;
					};
					break;
				case SqlDataType.Long:
					if(!type.IsLong()) {
						propErr = true;
						errType = typeof(long).Name;
					};
					break;
				case SqlDataType.Integer:
					if(
						!type.IsInteger()
						&&
						!type.IsByte()
						&&
						!type.IsShort()
						&&
						!type.IsSingle()
						&&
						!type.IsBoolean()
						&&
						!type.IsColor()) {
						propErr = true;
						errType = typeof(int).Name;
					};

					break;
				case SqlDataType.Int64:
					if (
						!type.IsInteger()
						&&
						!type.IsByte()
						&&
						!type.IsShort()
						&&
						!type.IsSingle()
						&&
						!type.IsBoolean()
						&&
						!type.IsColor()) {
						propErr = true;
						errType = typeof(Int64).Name;
					};

					break;
				case SqlDataType.TimeStamp:
					if(!type.IsDateTime()) {
						propErr = true;
						errType = typeof(DateTime).Name;
					};
					break;
				case SqlDataType.MaxVarBinary:
					if(!type.IsByteArray()) {
						propErr = true;
						errType = typeof(byte[]).Name;
					};
					break;
				case SqlDataType.Char:
				case SqlDataType.MaxVarChar:
				case SqlDataType.MaxVarWChar:
				case SqlDataType.VarChar:
				case SqlDataType.VarWChar:
				case SqlDataType.WChar:
					if(!type.IsString()) {
						propErr = true;
						errType = typeof(string).Name;
					};
					break;
				case SqlDataType.Uuid:
					if (!type.IsGuid()) {
						propErr = true;
						errType = "Guid";
					};
					break;
				case SqlDataType.Enum:
					if(!type.IsEnum) {
						propErr = true;
						errType = "Enum";
					};
					break;
				default:
					throw new NotImplementedException(string.Format(
						"Kuick.Data.Column.CorrectSettings: {0}",
						Spec.DbType.ToString()
					));
			}
			#endregion

			#region ColumnInitiate
			#endregion

			#region VisualSize
			switch(Visual.Input) {
				case VisualInput.TextBox:
					break;
				case VisualInput.TextArea:
					Visual.Size = Visual.Size == VisualSize.Medium 
						? VisualSize.XLarge 
						: Visual.Size;
					break;
				case VisualInput.Password:
					break;
				case VisualInput.Date:
					break;
				case VisualInput.Time:
					break;
				case VisualInput.TimeStamp:
					break;
				case VisualInput.FileUpload:
					break;
				case VisualInput.DropDownList:
					break;
				case VisualInput.CheckBoxList:
					break;
				case VisualInput.CheckBox:
					break;
				case VisualInput.RadioButtons:
					break;
				case VisualInput.HtmlEditor:
					break;
				case VisualInput.Color:
					break;
				default:
					break;
			}
			#endregion

			#region Nullable
			if(!DataCurrent.Data.DBNull) {
				switch(Spec.DbType) {
					case SqlDataType.Bit:
					case SqlDataType.Boolean:
					case SqlDataType.Long:
					case SqlDataType.Decimal:
					case SqlDataType.Double:
					case SqlDataType.Integer:
					case SqlDataType.Int64:
					case SqlDataType.TimeStamp:
					case SqlDataType.Identity:
						Spec.NotAllowNull = true;
						break;
					case SqlDataType.Unknown:
					case SqlDataType.Char:
					case SqlDataType.Enum:
					case SqlDataType.MaxVarBinary:
					case SqlDataType.MaxVarChar:
					case SqlDataType.MaxVarWChar:
					case SqlDataType.VarChar:
					case SqlDataType.VarWChar:
					case SqlDataType.WChar:
					case SqlDataType.Uuid:
					default:
						break;
				}
			}
			#endregion

			#region throw
			if(propErr) {
				throw new KException(string.Format(
					"{0}.{1} property type doesn't match the spec type of {2}.",
					EntityName,
					Property.Name,
					Spec.DbType
				));
			}
			#endregion
		}
		#endregion
	}
}
