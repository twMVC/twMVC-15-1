// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Entity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Xml.Serialization;
using System.Drawing;
using System.Web;

namespace Kuick.Data
{
	public class Entity : DynamicData, IEntity
	{
		#region Event
		public event EntityEventHandler BeforeAdd;
		public event EntityEventHandler BeforeModify;
		public event EntityEventHandler BeforeRemove;

		public event InstanceEventHandler AfterSelect;
		public event EntityEventHandler AfterAdd;
		public event EntityEventHandler AfterModify;
		public event EntityEventHandler AfterRemove;

		public event InstancesEventHandler AfterQuery;
		#endregion

		#region const
		public const string VERSION_NUMBER = "VERSION_NUMBER";
		#endregion

		#region fields
		protected static object _Lock = new object();
		#endregion

		#region constructor
		public Entity()
		{
			// Initiate
			InitiateColumnValue();

			// Difference
			if(null != Difference.Handler) {
				BeforeModify += new EntityEventHandler(DifferenceBefore);
				BeforeRemove += new EntityEventHandler(DifferenceBefore);
				AfterAdd += new EntityEventHandler(DifferenceAfterAdd);
				AfterModify += new EntityEventHandler(DifferenceAfterModify);
				AfterRemove += new EntityEventHandler(DifferenceAfterRemove);
			}
		}
		#endregion

		#region IValidate
		public Result Validate()
		{
			return Validate(Columns.ToArray());
		}
		#endregion

		#region IEntity
		[XmlIgnore]
		private string _TableName;
		public virtual string TableName
		{
			get
			{
				if(null == _TableName) {
					_TableName = SqlNamingConvention.ToTableName(this.GetType());
				}
				return _TableName;
			}
		}

		[XmlIgnore]
		public virtual List<EntityIndex> Indexes
		{
			get
			{
				return new List<EntityIndex>();
			}
		}

		[XmlIgnore]
		public virtual bool IsView
		{
			get
			{
				return false;
			}
		}

		[XmlIgnore]
		public bool IsCompositeKey
		{
			get
			{
				return KeyColumns.Count > 1;
			}
		}

		[XmlIgnore]
		private string _KeyName;
		public string KeyName
		{
			get
			{
				if(null == _KeyName) {
					switch(KeyColumns.Count) {
						case 0: // no
							Logger.Error(string.Format(
								"Table {0} must have a primary key!", TableName
							));
							_KeyName = string.Empty;
							break;
						case 1: // justone
							_KeyName = KeyColumns[0].Spec.ColumnName;
							break;
						default: // multiple
							StringBuilder sb = new StringBuilder();
							foreach(Column x in KeyColumns) {
								if(sb.Length > 0) {
									sb.Append(DataConstants.Seperator);
								}
								sb.Append(x.Spec.ColumnName);
							}
							_KeyName = sb.ToString();
							break;
					}
				}
				return _KeyName;
			}
		}

		[XmlIgnore]
		public string KeyValue
		{
			get
			{
				switch(KeyColumns.Count) {
					case 0: // no
						Logger.Error(string.Format(
							"Table {0} must have a primary key!", TableName
						));
						return string.Empty;
					case 1: // justone
						object keyValue = GetValue(KeyColumn);
						return null == keyValue
							? string.Empty
							: keyValue.ToString();
					default: // multiple
						StringBuilder sb = new StringBuilder();
						foreach(Column x in KeyColumns) {
							if(sb.Length > 0) {
								sb.Append(DataConstants.Seperator);
							}
							sb.Append(GetValue(x).ToString());
						}
						return sb.ToString();
				}
			}
			set
			{
				switch(KeyColumns.Count) {
					case 0: // no
						Logger.Error(string.Format(
							"Table {0} must have a primary key!", TableName
						));
						return;
					case 1: // justone
						SetValue(KeyColumn, value);
						return;
					default: // multiple
						string[] values = value.Split(DataConstants.Seperator);
						if(values.Length != KeyColumns.Count) {
							Logger.Error(
								"Entity.KeyValue.set_KeyValue",
								"Primary keys and values does not match",
								new Any(
									string.Format(
										"Primary Key Values (seperated with {0})",
										DataConstants.Seperator
									),
									value
								)
							);
							return;
						}

						int count = KeyColumns.Count;
						for(int i = 0; i < count; i++) {
							SetValue(KeyColumns[i], values[i]);
						}
						break;
				}
			}
		}

		private string _Alias;
		public string Alias
		{
			get
			{
				if(null == _Alias) {
					_Alias = EntityCache.GetAlias(EntityName);
				}
				return _Alias;
			}
		}

		public string _Abbr;
		public string Abbr
		{
			get
			{
				if(null == _Abbr) {
					_Abbr = EntityName
						.TrimStart(Prefix)
						.TrimEnd(DataConstants.Entity.Suffix)
						.ToLower();
				}
				return _Abbr;
			}
		}

		public string _Prefix;
		public virtual string Prefix
		{
			get
			{
				if(null == _Prefix) {
					_Prefix = string.Empty;
					string[] parts = this.
						GetType().
						Assembly.
						FullName.
						SplitWith(",")[0].
						SplitWith(".");
					for(int i = parts.Length - 1; i > -1; i--) {
						string part = parts[i];
						if(!part.In("Kuick", "Data", "Module", "Web")) {
							_Prefix = part;
							break;
						}
					}
				}
				return _Prefix;
			}
		}

		public static string GetAlias(string entityName)
		{
			return EntityCache.GetAlias(entityName);
		}

		[XmlIgnore]
		public virtual bool Alterable
		{
			get
			{
				return true;
			}
		}

		[XmlIgnore]
		public virtual bool Addable
		{
			get
			{
				return true;
			}
		}

		[XmlIgnore]
		public virtual bool Modifyable
		{
			get
			{
				return true;
			}
		}

		[XmlIgnore]
		public virtual bool Removable
		{
			get
			{
				return true;
			}
		}

		[XmlIgnore]
		private bool _AllowBatchModify = false;
		public virtual bool AllowBatchModify
		{
			get
			{
				return _AllowBatchModify
					? true
					: null == BeforeModify && null == AfterModify;
			}
			set
			{
				_AllowBatchModify = value;
			}
		}

		[XmlIgnore]
		private bool _AllowBatchRemove = false;
		public virtual bool AllowBatchRemove
		{
			get
			{
				return _AllowBatchRemove
					? true
					: null == BeforeRemove && null == AfterRemove;
			}
			set
			{
				_AllowBatchRemove = value;
			}
		}

		public DataResult Add()
		{
			return Api.Get(EntityName).Add(this);
		}
		public DataResult Modify()
		{
			return Api.Get(EntityName).Modify(this);
		}
		public DataResult Remove()
		{
			return Api.Get(EntityName).Remove(this);
		}
		public virtual DataResult ForceRemove()
		{
			return new DataResult(
				false, 
				string.Format(
					"Not support ForceRemove",
					Table.Description
				)
			);
		}

		[XmlIgnore]
		private string _EntityName;
		public string EntityName
		{
			get
			{
				if(null == _EntityName) {
					_EntityName = this.GetType().Name;
				}
				return _EntityName;
			}
		}

		[XmlIgnore]
		public virtual string TitleValue
		{
			get
			{
				return KeyValue;
			}
		}

		private Table _Table;
		[XmlIgnore]
		public Table Table
		{
			get
			{
				if(null == _Table) {
					lock(_Lock) {
						if(null == _Table) {
							if(Heartbeat.Singleton.PreDatabaseStartFinished) {
								IEntity instance = EntityCache.Get(EntityName);
								if(null == instance) {
									throw new NullReferenceException();
								}
								_Table = instance.Table;
							} else {
								_Table = new Table();
								object[] objs = this
									.GetType()
									.GetCustomAttributes(true);
								CategoryAttribute category = null;
								DescriptionAttribute description = null;
								EntitySpec spec = null;
								List<EntityMapping> mappings = new List<EntityMapping>();
								List<EntityIndex> indexes = new List<EntityIndex>();
								EntityVisual visual = null;
								FollowDiff followDiff = null;
								// List<IEntity> refers = null; // ?
								foreach(object x in objs) {
									if(x is CategoryAttribute) {
										category = (CategoryAttribute)x;
										continue;
									}
									if(x is DescriptionAttribute) {
										description = (DescriptionAttribute)x;
										continue;
									}
									if(x is EntitySpec) {
										spec = (EntitySpec)x;
										spec.Table = _Table;
										continue;
									}
									if(x is EntityIndex) {
										EntityIndex index = (EntityIndex)x;
										index.Table = _Table;
										indexes.Add(index);
										continue;
									}
									if(x is EntityMapping) {
										EntityMapping mapping = (EntityMapping)x;
										mapping.Table = _Table;
										mappings.Add(mapping);
										continue;
									}
									if(x is EntityVisual) {
										visual = (EntityVisual)x;
										visual.Table = _Table;
										continue;
									}
									if(x is FollowDiff) {
										followDiff = (FollowDiff)x;
										continue;
									}
								}

								// CategoryAttribute
								if(null == category) {
									category = new CategoryAttribute(
										this.GetType().Namespace
									);
								}

								// DescriptionAttribute
								if(null == description) {
									description = new DescriptionAttribute(
										GetType()
											.Name
											.TrimEnd(DataConstants.Entity.Suffix)
									);
								}

								_Table.EntityName = EntityName;
								_Table.TableName = TableName;
								_Table.Class = this.GetType();

								_Table.Category = category;
								_Table.Description = description;
								_Table.Spec = spec;
								_Table.Indexes = indexes;
								_Table.Mappings = mappings;
								_Table.Visual = visual;
								_Table.FollowDiff = followDiff;
							}
						}
					}
				}
				return _Table;
			}
		}

		private List<Column> _Columns;
		[XmlIgnore]
		public List<Column> Columns
		{
			get
			{
				if(null == _Columns) {
					lock(_Lock) {
						if(null == _Columns) {
							if(Heartbeat.Singleton.PreDatabaseStartFinished) {
								IEntity instance = EntityCache.Get(EntityName);
								if(null == instance) {
									throw new NullReferenceException();
								}
								_Columns = instance.Columns;
							} else {
								_Columns = new List<Column>();
								List<Column> list = new List<Column>();
								PropertyInfo[] infos = this.GetType().GetProperties();
								foreach(PropertyInfo info in infos) {
									Column column = new Column();
									object[] objs = info.GetCustomAttributes(true);
									CategoryAttribute category = null;
									DescriptionAttribute description = null;
									DefaultValueAttribute defaultValue = null;
									ColumnSpec spec = null;
									ColumnEncrypt encryptor = null;
									ColumnRefer refer = null;
									ColumnInitiate initiate = null;
									ColumnIdentity identity = null;
									ColumnVisual visual = null;
									IgnoreDiff ignoreDiff = null;
									List<IValidation> validations = new List<IValidation>();
									foreach(object x in objs) {
										if(x is CategoryAttribute) {
											category = (CategoryAttribute)x;
											continue;
										}
										if(x is DescriptionAttribute) {
											description = (DescriptionAttribute)x;
											continue;
										}
										if(x is DefaultValueAttribute) {
											defaultValue = (DefaultValueAttribute)x;
											continue;
										}
										if(x is ColumnSpec) {
											spec = (ColumnSpec)x;
											spec.Column = column;
											continue;
										}
										if(x is ColumnEncrypt) {
											encryptor = (ColumnEncrypt)x;
											encryptor.Column = column;
											continue;
										}
										if(x is ColumnRefer) {
											refer = (ColumnRefer)x;
											refer.Column = column;
											if(refer.Style == ReferValue.Self) {
												refer.Type = this.GetType();
											}
											continue;
										}
										if(x is ColumnInitiate) {
											initiate = (ColumnInitiate)x;
											initiate.Column = column;
											continue;
										}
										if(x is ColumnIdentity) {
											identity = (ColumnIdentity)x;
											identity.Column = column;
											continue;
										}
										if(x is ColumnVisual) {
											visual = (ColumnVisual)x;
											visual.Column = column;
											continue;
										}
										if(x is IgnoreDiff) {
											ignoreDiff = (IgnoreDiff)x;
											continue;
										}
										if(x is IValidation) {
											IValidation validation = (IValidation)x;
											Reflector.SetValue(
												validation, "Column", column
											);
											validations.Add(validation);
											continue;
										}
									}

									// ColumnSpec
									if(null == spec) {
										continue;
									} else {
										if(Checker.IsNull(spec.ColumnName)) {
											spec.ColumnName = SqlNamingConvention
												.ToColumnName(this.GetType(), info);
										}
									}

									// CategoryAttribute
									if(null == category) {
										category = new CategoryAttribute(
											DataConstants.Default.Category
										);
									}

									// DescriptionAttribute
									if(null == description) {
										description = new DescriptionAttribute(
											Formator.DividePascalCasing(
												info.Name
											)
										);
									}

									// ColumnRefer
									if(null == refer) {
										refer = new ColumnRefer(ReferValue.None);
									}

									// ColumnInitiate
									if(null == initiate) {
										initiate = null == defaultValue
											? new ColumnInitiate(InitiateValue.None)
											: new ColumnInitiate(
												defaultValue.Value.ToString()
											);
										initiate.Column = column;
									}

									// ColumnIdentity
									if(null != identity) {
										string identityID = string.Format(
											"{0}::{1}",
											EntityName,
											spec.ColumnName
										);
										identity.IdentityID = identityID;
										identity.Column = column;
									}

									// ColumnVisual
									if(null == visual) {
										visual = new ColumnVisual();
									}

									column.TableName = TableName;
									column.EntityName = EntityName;
									column.Category = category;
									column.Description = description;
									column.Property = info;
									column.Spec = spec;
									column.Refer = refer;
									column.Initiate = initiate;
									column.Identity = identity;
									column.Visual = visual;
									column.Encryption = encryptor;
									column.Validations = validations;
									column.SkipDiff = 
										null != ignoreDiff
										||
										visual.SystemColumn;
									column.Foolproof();

									if(column.Spec.ColumnName == VERSION_NUMBER) {
										if(DataCurrent.Data.Concurrency) {
											list.Add(column);
										}
									} else {
										list.Add(column);
									}
								}

								#region sort
								// Key
								foreach(Column column in list) {
									if(!column.Spec.PrimaryKey) { continue; }
									_Columns.Add(column);
								}

								// Columns
								foreach(Column column in list) {
									if(column.Spec.PrimaryKey) { continue; }
									_Columns.Add(column);
								}
								#endregion
							}
						}
					}
				}
				return _Columns;
			}
		}

		private List<PropertyInfo> _SerializableColumns;
		[XmlIgnore]
		public List<PropertyInfo> SerializableColumns
		{
			get
			{
				if(null == _SerializableColumns) {
					lock(_Lock) {
						if(null == _SerializableColumns) {
							if(Heartbeat.Singleton.PreDatabaseStartFinished) {
								IEntity instance = EntityCache.Get(EntityName);
								if(null == instance) {
									throw new NullReferenceException();
								}
								_SerializableColumns = instance.SerializableColumns;
							} else {
								_SerializableColumns = new List<PropertyInfo>();
								PropertyInfo[] infos = this.GetType().GetProperties();
								foreach(PropertyInfo info in infos) {
									object[] objs = info.GetCustomAttributes(true);
									ColumnSerializable serializable = null;
									foreach(object x in objs) {
										if(x is ColumnSerializable) {
											serializable = (ColumnSerializable)x;
											break;
										}
									}
									if(null != serializable) {
										_SerializableColumns.Add(info);
									}
								}
							}
						}
					}
				}
				return _SerializableColumns;
			}
		}


		private List<PropertyInfo> _ColumnProperties;
		[XmlIgnore]
		public List<PropertyInfo> ColumnProperties
		{
			get
			{
				if(null == _ColumnProperties) {
					_ColumnProperties = Columns
						.AsQueryable()
						.Select<Column, PropertyInfo>(x => x.Property)
						.ToList();
				}
				return _ColumnProperties;
			}
		}

		[XmlIgnore]
		public Column KeyColumn
		{
			get
			{
				return Columns.Find(x => x.Spec.PrimaryKey == true);
			}
		}

		[XmlIgnore]
		public List<Column> KeyColumns
		{
			get
			{
				return Columns.FindAll(x => x.Spec.PrimaryKey == true);
			}
		}

		public Column GetColumn(string columnNameOrPropertyName)
		{
			return Columns.Find(x =>
				x.Spec.ColumnName == columnNameOrPropertyName
				||
				x.Property.Name == columnNameOrPropertyName
			);
		}

		public Column GetColumn(Expression<Func<Entity, object>> expression)
		{
			return DataUtility.ToColumn<Entity>(expression);
		}

		public bool DynamicDisplay(string columnNameOrPropertyName)
		{
			Column column = GetColumn(columnNameOrPropertyName);

			switch(column.Property.Name) {
				case "VersionNumber":
					return DataCurrent.Data.Concurrency;
				default:
					return true;
			}
		}

		public Result Validate(params string[] columnNamesOrPropertyNames)
		{
			List<Column> list = new List<Column>();
			foreach(string x in columnNamesOrPropertyNames) {
				Column column = GetColumn(x);
				list.Add(column);
			}
			return Validate(list.ToArray());
		}

		public Result Validate(params Column[] columns)
		{
			Result result = new Result(true);
			foreach(Column x in columns) {
				object value = GetValue(x);
				Result innerResult = x.Validate(value);
				result.InnerResults.Add(innerResult);
			}
			return result;
		}

		public void SetValue(params Any[] anys)
		{
			foreach(Any x in anys) {
				SetValue(x.Name, x.Value);
			}
		}

		public void SetValue(NameValueCollection nvc)
		{
			for(int i = 0; i < nvc.Count; i++) {
				SetValue(nvc.GetKey(i), nvc.Get(i));
			}
		}

		public void SetValue(string columnNameOrPropertyName, object value)
		{
			Column column = GetColumn(columnNameOrPropertyName);
			SetValue(column, value);
		}

		public void SetValue(Column column, object value)
		{
			if(column.IsBoolean) {
				if(null != value && value.ToString() == "on") {
					value = true;
				}
			}

			Reflector.SetValue(this, column.Property, value);
		}

		public object GetValue(string columnNameOrPropertyName)
		{
			Column column = GetColumn(columnNameOrPropertyName);
			return GetValue(column);
		}

		public object GetValue(Column column)
		{
			object val = Reflector.GetValue(column.Property, this);
			return val;
		}

		public string GetString(string columnNameOrPropertyName)
		{
			Column column = GetColumn(columnNameOrPropertyName);
			return GetString(column);
		}

		public string GetString(Column column)
		{
			object val = GetValue(column);

			switch(column.Format) {
				case ColumnDataFormat.String:
					return null == val ? string.Empty : val.ToString();
				case ColumnDataFormat.Integer:
				case ColumnDataFormat.Int64:
				case ColumnDataFormat.Decimal:
				case ColumnDataFormat.Long:
				case ColumnDataFormat.Short:
				case ColumnDataFormat.Double:
				case ColumnDataFormat.Float:
				case ColumnDataFormat.Byte:
					return null == val ? "0" : val.ToString();
				case ColumnDataFormat.Boolean:
					bool bVal = null == val
						? false
						: val.ToString().AirBagToBoolean();
					return Formator.Boolean2String(bVal);
				case ColumnDataFormat.Char:
					return null == val ? string.Empty : val.ToString();
				case ColumnDataFormat.Enum:
					if(null == val){ return string.Empty; }
					EnumReference ef = EnumCache.Get(column.Property.PropertyType);
					if(null == ef) { return val.ToString(); }
					return ef.GetTitle(val.ToString());
				case ColumnDataFormat.ByteArray:
					return "not support ByteArray";
				case ColumnDataFormat.DateTime:
					if(null == val) { return string.Empty; }
					DateTime dVal = Formator.AirBagToDateTime(val.ToString());
					switch (column.Visual.Input)
					{
						case VisualInput.Date:
							return dVal.yyyyMMdd();
						case VisualInput.Time:
							return dVal.hhmmss();
						default:
							return dVal.yyyyMMddHHmmssfff();
					}
				case ColumnDataFormat.Color:
					if(null == val) { return string.Empty; }
					Color cVal = (Color)val;
					return ColorTranslator.ToHtml(cVal);
				case ColumnDataFormat.Guid:
					if (null == val) { return string.Empty; }
					Guid gVal = (Guid)val;
					return gVal.ToString();
				default:
					return null == val
						? string.Empty
						: val.ToString();
			}
		}

		public object GetOriginalValue(string columnNameOrPropertyName)
		{
			return GetOriginalValue(GetColumn(columnNameOrPropertyName));
		}

		public object GetOriginalValue(Column column)
		{
			if(null == column) { return null; }
			return Record.GetValue(column.Spec.ColumnName);
		}

		public object GetInitiateValue(string columnNameOrPropertyName)
		{
			return GetInitiateValue(GetColumn(columnNameOrPropertyName));
		}

		public object GetInitiateValue(Column column)
		{
			if(null == column.Initiate) { return string.Empty; }

			if(column.Initiate.Style == InitiateValue.None) {
				switch(column.Initiate.Format) {
					case ColumnDataFormat.String:
						return string.Empty;
					case ColumnDataFormat.Integer:
					case ColumnDataFormat.Int64:
					case ColumnDataFormat.Decimal:
					case ColumnDataFormat.Long:
					case ColumnDataFormat.Short:
					case ColumnDataFormat.Double:
					case ColumnDataFormat.Float:
						return Formator.AirBagToInt(
							column.Initiate.Value.ToString(),
							0
						);
					case ColumnDataFormat.Boolean:
						return Formator.AirBagToBoolean(
							column.Initiate.Value.ToString(),
							false
						);
					case ColumnDataFormat.Char:
					case ColumnDataFormat.Enum:
						try {
							return Enum.Parse(
								column.Property.PropertyType,
								column.Initiate.Value.ToString(),
								true
							);
						} catch {
							EnumReference ef = EnumCache.Get(
								column.Property.PropertyType
							);
							foreach(var item in ef.Items) {
								if(item.Title == column.Initiate.Value.ToString()) {
									return item.Value;
								}
							}
							return 0;
						}
					case ColumnDataFormat.Byte:
						return 0;
					case ColumnDataFormat.ByteArray:
						return new byte[0];
					case ColumnDataFormat.DateTime:
						return Formator.AirBagToDateTime(
							column.Initiate.Value.ToString(),
							Constants.Date.Min
						);
					case ColumnDataFormat.Color:
						return string.Empty;
					case ColumnDataFormat.Guid:
						return Guid.NewGuid();
					default:
						throw new NotImplementedException();
				}
			} else {
				switch(column.Initiate.Style) {
					case InitiateValue.None:
						return string.Empty;
					case InitiateValue.Empty:
						return string.Empty;
					case InitiateValue.Uuid:
					case InitiateValue.UuidAndAutoUpdate:
						return Utility.GetUuid();
					case InitiateValue.UniqueTicks:
					case InitiateValue.UniqueTicksAndAutoUpdate:
						return Utility.GetUniqueTicks();
					case InitiateValue.NullDate:
						return Formator.ToString14(DataConstants.Date.Null);
					case InitiateValue.MaxDate:
						return Formator.ToString14(DataConstants.Date.Max);
					case InitiateValue.Date4:
						return Formator.ToString4();
					case InitiateValue.Date8s:
						return Formator.ToString8s();
					case InitiateValue.Date8:
						return Formator.ToString8();
					case InitiateValue.Date14s:
						return Formator.ToString14s();
					case InitiateValue.Date14:
						return Formator.ToString14();
					case InitiateValue.Date17s:
						return Formator.ToString17s();
					case InitiateValue.Date17:
						return Formator.ToString17();
					case InitiateValue.Date14AutoUpdate:
						return Formator.ToString14();
					case InitiateValue.Date17AutoUpdate:
						return Formator.ToString17();
					case InitiateValue.IntegerAndAutoIncremental:
						return column.Initiate.IntegerValue;
					case InitiateValue.ToUpper:
					case InitiateValue.ToLower:
						return string.Empty;
					case InitiateValue.CurrentUser:
					case InitiateValue.CurrentUserAndAutoUpdate:
						return DataCurrent.UserName;
					case InitiateValue.TimsSpanHex:
						return Convert.ToString(DateTime.Now.Ticks, 16).ToUpper();
					case InitiateValue.DaysOffset:
						return Formator.ToDate8().AddDays(column.Initiate.Days);
					default:
						throw new NotImplementedException();
				}
			}
		}

		public J GetJoin<J>()
			where J : class, IEntity, new()
		{
			J j = new J();
			foreach(Column column in j.Columns) {
				object value;
				if(TryGetValue(column.AsName, out value)) {
					j.SetValue(column, value);
				}
			}
			return j;
		}

		public string ToXml()
		{
			return Serializer.ToXml(this);
		}

		public string ToJson(params string[] columnNamesOrPropertyNames)
		{
			List<string> list = new List<string>();
			if(Checker.IsNull(columnNamesOrPropertyNames)) {
				foreach(Column column in Columns) {
					list.Add(column.Property.Name);
				}
				foreach(PropertyInfo info in SerializableColumns) {
					if(!list.Contains(info.Name)) {
						list.Add(info.Name);
					}
				}
			} else {
				foreach(string x in columnNamesOrPropertyNames) {
					Column column = GetColumn(x);
					if(null == column) {
						list.Add(x);
					} else {
						list.Add(column.Property.Name);
					}
				}
			}
			return Serializer.ToJson(this, list.ToArray());
		}

		[XmlIgnore]
		public virtual bool EnableConcurrency
		{
			get
			{
				return
					Concurrency.EnumIn(Flag.Enable, Flag.Default)
					&&
					DataCurrent.Data.Concurrency;
			}
		}

		public virtual Flag Concurrency
		{
			get
			{
				return Flag.Default;
			}
		}

		[DataMember]
		[ColumnSpec(VERSION_NUMBER)]
		[ColumnInitiate(InitiateValue.UuidAndAutoUpdate)]
		[ColumnVisual(VisualFlag.SystemColumn)]
		public string VersionNumber { get; set; }

		public virtual void Interceptor(Sql sql)
		{
		}

		public virtual void FrontEndInterceptor(Sql sql)
		{
		}

		public virtual IQueryable CacheInterceptor(IQueryable datas)
		{
			return datas;
		}

		public virtual bool EnableCache
		{
			get
			{
				return true;
			}
		}

		public MappingInstanceHelper Mapping(string memberEntityName)
		{
			MappingInstanceHelper mapping = new MappingInstanceHelper(
				this, memberEntityName
			);
			return mapping;
		}

		public MappingInstanceHelper Mapping<TMember>()
			where TMember : class, IEntity, new()
		{
			MappingInstanceHelper mapping = new MappingInstanceHelper(
				this, typeof(TMember).Name
			);
			return mapping;
		}

		public virtual void OnInit(object sender, EntityEventArgs e)
		{
		}

		public Sql CreateSql()
		{
			return CreateSql(EntityName);
		}

		public static Sql CreateSql(string entityName)
		{
			Sql sql = new Sql(entityName);
			return sql;
		}

		public virtual string GetInsertCommand(string columnNameOrPropertyName) 
		{
			return string.Empty;
		}

		[XmlIgnore]
		public virtual string FileRoot
		{
			get
			{
				return DataCurrent.Application.GetString("Web", "FileRoot", "~/upload");
			}
		}
		#endregion

		#region Access
		public static Sql Sql(string entityName)
		{
			return new Sql(entityName);
		}

		public static IEntity Get(string entityName, string keyValue)
		{
			IEntity schema = EntityCache.Get(entityName);
			if(schema.IsCompositeKey) {
				throw new KException(
@"Entity with CompositeKey use another method 
'Get(string entityName, params Any[] anys)'."
				);
			}

			return new Sql(entityName)
				.Where(schema.KeyColumn.Spec.ColumnName, keyValue)
				.QueryFirst();
		}
		public static IEntity Get(string entityName, params Any[] anys)
		{
			IEntity schema = EntityCache.Get(entityName);
			return new Sql(entityName)
				.Where(anys)
				.QueryFirst();
		}
		public static List<IEntity> GetAll(string entityName)
		{
			return new Sql(entityName).Query();
		}
		public static List<IEntity> Search(
			string entityName,
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return new Sql(entityName)
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Query();
		}
		public static List<IEntity> Search(
			string entityName,
			string keyword,
			Action<Sql> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			Sql sql = new Sql(entityName);
			interceptor(sql);
			return sql
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Query();
		}
		public static int SearchCount(
			string entityName,
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return new Sql(entityName)
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Count();
		}
		public static int SearchCount(
			string entityName,
			string keyword,
			Action<Sql> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			Sql sql = new Sql(entityName);
			interceptor(sql);
			return sql
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Count();
		}
		public static bool SearchExists(
			string entityName,
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return new Sql(entityName)
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Exists();
		}
		public static bool SearchExists(
			string entityName,
			string keyword,
			Action<Sql> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			Sql sql = new Sql(entityName);
			interceptor(sql);
			return sql
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Exists();
		}

		public static IEntity QueryFirst(string entityName, params Any[] anys)
		{
			return Sql(entityName).Where(anys).QueryFirst();
		}

		public static List<IEntity> Query(string entityName, params Any[] anys)
		{
			return Sql(entityName).Where(anys).Query();
		}

		public static List<IEntity> Query(string entityName, string condition)
		{
			return Sql(entityName).Where(condition).Query();
		}

		public static int Count(string entityName)
		{
			IEntity schema = EntityCache.Get(entityName);
			return schema.CreateSql().Count();
		}

		public static bool Exists(string entityName, string keyValue)
		{
			IEntity schema = EntityCache.Get(entityName);
			return Exists(
				entityName,
				new Any(schema.KeyColumn.Spec.ColumnName, keyValue)
			);
		}

		public static bool Exists(string entityName, params Any[] anys)
		{
			return Sql(entityName).Where(anys).Exists();
		}

		public static DataResult Modify(
			string entityName,
			string keyValue,
			params Any[] anys)
		{
			IEntity schema = EntityCache.Get(entityName);
			Sql sql = Sql(entityName).Where(schema.KeyName, keyValue);
			foreach(Any any in anys) {
				sql.SetValue(any.Name, any.ToString());
			}

			sql.Schema.AllowBatchModify = true;
			DataResult result = sql.Modify();

			return result;
		}

		public static DataResult Remove(string entityName, string keyValue)
		{
			IEntity schema = EntityCache.Get(entityName);
			return Sql(entityName)
				.Where(schema.KeyColumn.Spec.ColumnName, keyValue)
				.Remove();
		}

		public static DataResult SearchRemove(
			string entityName,
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return new Sql(entityName)
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Remove();
		}
		public static DataResult SearchRemove(
			string entityName,
			string keyword,
			Action<Sql> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			Sql sql = new Sql(entityName);
			interceptor(sql);
			return sql
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Remove();
		}
		public static List<Entity> ExecuteQuery(
			string sql,
			params IDbDataParameter[] parameters)
		{
			return ExecuteQuery(DataConstants.Entity.Name, sql, parameters);
		}
		public static DataResult ExecuteNonQuery(
			string sql,
			params IDbDataParameter[] parameters)
		{
			return ExecuteNonQuery(DataConstants.Entity.Name, sql, parameters);
		}
		public static List<Entity> ExecuteStoredProcedure(
			string sql,
			params IDbDataParameter[] parameters)
		{
			return ExecuteStoredProcedure(
				DataConstants.Entity.Name, sql, parameters
			);
		}
		public static Any ExecuteScalar(
			string sql,
			params IDbDataParameter[] parameters)
		{
			return ExecuteScalar(DataConstants.Entity.Name, sql, parameters);
		}

		public static List<Entity> ExecuteQuery(
			string entityName,
			string sql,
			params IDbDataParameter[] parameters)
		{
			Api api = Api.GetNew(entityName);
			DataSet ds = api.ExecuteQuery(sql, parameters);
			return null == ds ? new List<Entity>() : ds.ToEntity<Entity>();
		}
		public static DataResult ExecuteNonQuery(
			string entityName,
			string sql,
			params IDbDataParameter[] parameters)
		{
			Api api = Api.Get(entityName);
			DataResult result = api.ExecuteNonQuery(sql, parameters);
			return result;
		}
		public static List<Entity> ExecuteStoredProcedure(
			string entityName,
			string sql,
			params IDbDataParameter[] parameters)
		{
			Api api = Api.Get(entityName);
			DataSet ds = api.ExecuteStoredProcedure(sql, parameters);
			return null == ds ? new List<Entity>() : ds.ToEntity<Entity>();
		}
		public static Any ExecuteScalar(
			string entityName,
			string sql,
			params IDbDataParameter[] parameters)
		{
			Api api = Api.GetNew(entityName);
			Any any = api.ExecuteScalar(sql, parameters);
			return any;
		}
		#endregion

		#region dirty control
		private Anys _OriginalDataHash;
		internal void CompleteDataProcess()
		{
			_OriginalDataHash = new Anys();
			foreach(Column column in Columns) {
				object val = GetValue(column);
				int h = null == val ? 0 : val.GetHashCode();
				_OriginalDataHash.Add(column.Spec.ColumnName, h);
			}
		}

		protected bool SkipDirtyColumnCheck = false;

		public bool HasDirtyColumn
		{
			get
			{
				if(SkipDirtyColumnCheck) { return true; }

				foreach(Column column in GetDirtyColumns()) {
					if(!column.Initiate.Style.EnumIn(
						InitiateValue.UuidAndAutoUpdate,
						InitiateValue.UniqueTicksAndAutoUpdate,
						InitiateValue.Date14AutoUpdate,
						InitiateValue.Date17AutoUpdate,
						InitiateValue.IntegerAndAutoIncremental,
						InitiateValue.CurrentUserAndAutoUpdate)) {
						return true;
					}
				}

				return false;
			}
		}

		public List<Column> GetDirtyColumns()
		{
			if(null == _OriginalDataHash) { return Columns; }

			List<Column> dirtyColumns = new List<Column>();
			foreach(Column column in Columns) {
				if(column.Spec.PrimaryKey) { continue; }
				object val = GetValue(column);
				if(_OriginalDataHash.Exists(column.Spec.ColumnName)) {
					int currentHash = null == val ? 0 : val.GetHashCode();
					int originalHash = _OriginalDataHash.ToInteger(
						column.Spec.ColumnName
					);
					if(currentHash == originalHash) { continue; }
				}
				dirtyColumns.Add(column);
			}

			List<Column> nulls = NullColumns
				.Union(NullFields).Except(NullColumns.Intersect(NullFields))
				.ToList();
			if(!Checker.IsNull(nulls)) {
				foreach(Column column in nulls) {
					if(column.Spec.PrimaryKey) { continue; }
					if(!dirtyColumns.Exists(x =>
						x.Spec.ColumnName == column.Spec.ColumnName)) {
						dirtyColumns.Add(column);
					}
				}
			}

			return dirtyColumns;
		}

		private List<Column> _NullColumns;
		internal List<Column> NullColumns
		{
			get
			{
				if(null == _NullColumns) {
					_NullColumns = new List<Column>();
					foreach(Column column in Columns) {
						if(!column.Spec.NotAllowNull) {
							_NullColumns.Add(column);
						}
					}
				}
				return _NullColumns;
			}
			set
			{
				_NullColumns = new List<Column>();
				foreach(Column column in value) {
					_NullColumns.Add(column);
				}
			}
		}

		private List<Column> _NullFields;
		internal List<Column> NullFields
		{
			get
			{
				if(null == _NullFields) {
					_NullFields = NullColumns;
				}
				return _NullFields;
			}
			set
			{
				_NullFields = new List<Column>();
				foreach(Column column in value) {
					_NullFields.Add(column);
				}
			}
		}

		public bool IsNullCheck(string columnNameOrPropertyName)
		{
			Column column = GetColumn(columnNameOrPropertyName);
			return NullColumns.Contains(x =>
				x.Spec.ColumnName == column.Spec.ColumnName
			);
		}

		public void IsNull(string columnNameOrPropertyName)
		{
			if(!IsNullCheck(columnNameOrPropertyName)) {
				Column column = GetColumn(columnNameOrPropertyName);
				NullColumns.Add(column);
				SetNull(column);
			}
		}

		public void NotNull(string columnNameOrPropertyName)
		{
			if(IsNullCheck(columnNameOrPropertyName)) {
				Column column = GetColumn(columnNameOrPropertyName);
				NullColumns.Remove(column);
				SetNull(column);
			}
		}

		private void SetNull(Column column)
		{
			switch(column.Format) {
				case ColumnDataFormat.String:
					SetValue(column, default(string));
					break;
				case ColumnDataFormat.Integer:
					SetValue(column, default(int));
					break;
				case ColumnDataFormat.Decimal:
					SetValue(column, default(decimal));
					break;
				case ColumnDataFormat.Long:
					SetValue(column, default(long));
					break;
				case ColumnDataFormat.Short:
					SetValue(column, default(short));
					break;
				case ColumnDataFormat.Double:
					SetValue(column, default(double));
					break;
				case ColumnDataFormat.Float:
					SetValue(column, default(float));
					break;
				case ColumnDataFormat.Boolean:
					SetValue(column, default(bool));
					break;
				case ColumnDataFormat.Char:
					SetValue(column, default(char));
					break;
				case ColumnDataFormat.Enum:
					SetValue(column, default(string));
					break;
				case ColumnDataFormat.Byte:
					SetValue(column, default(byte));
					break;
				case ColumnDataFormat.ByteArray:
					SetValue(column, default(byte[]));
					break;
				case ColumnDataFormat.DateTime:
					SetValue(column, Constants.Date.Min);
					break;
				case ColumnDataFormat.Color:
					SetValue(column, default(Color));
					break;
				case ColumnDataFormat.Guid:
					SetValue(column, default(Guid));
					break;
				default:
					break;
			}
		}
		public virtual List<IEntity> References 
		{
			get
			{
				return GetAll(EntityName);
			}
		}

		public Int64 SizeOf()
		{
			int size = 0;
			foreach (var column in Columns) {
				switch (column.Format) {
					case ColumnDataFormat.Color:
					case ColumnDataFormat.Integer:
						size += sizeof(int);
						break;
					case ColumnDataFormat.Int64:
						size += sizeof(Int64);
						break;
					case ColumnDataFormat.Decimal:
						size += sizeof(decimal);
						break;
					case ColumnDataFormat.Long:
						size += sizeof(long);
						break;
					case ColumnDataFormat.Short:
						size += sizeof(short);
						break;
					case ColumnDataFormat.Double:
						size += sizeof(double);
						break;
					case ColumnDataFormat.Float:
						size += sizeof(float);
						break;
					case ColumnDataFormat.Boolean:
						size += sizeof(bool);
						break;
					case ColumnDataFormat.Char:
						size += sizeof(char);
						break;
					case ColumnDataFormat.Byte:
						size += sizeof(byte);
						break;
					case ColumnDataFormat.ByteArray:
						byte[] bs = GetValue(column) as byte[];
						if(null!= bs){
						size += sizeof(byte) * bs.Length;
						}
						break;
					case ColumnDataFormat.DateTime:
						size += 8;
						break;
					case ColumnDataFormat.Guid:
						size += 8; // ?
						break;
					case ColumnDataFormat.Undefined:
					case ColumnDataFormat.String:
					case ColumnDataFormat.Enum:
						object v = GetValue(column);
						if (null == v) { continue; }
						string value = v.ToString();
						size += ASCIIEncoding.Unicode.GetByteCount(value);
						break;
					default:
						break;
				}
			}
			return size;
		}

		public virtual bool EnableNullToEmptyAndTrim { get { return false; } }
		#endregion

		#region internal
		#region Intercept
		[XmlIgnore]
		internal bool Intercepting { get; set; }
		#endregion

		#region Encrypt / Decrypt
		internal void Encrypt()
		{
			foreach(Column x in Columns) {
				if(!x.IsString || null == x.Encryption) { continue; }
				string value = GetValue(x).ToString();
				if(value.IsEncrypted()) { continue; }

				switch(x.Encryption.Method) {
					case Encryption.Asymmetry:
						value =
							DataConstants.Prefix.Encrypted
							+
							value.ToSHA();
						break;
					case Encryption.Symmetry:
						value =
							DataConstants.Prefix.Encrypted
							+
							value.Encrypt(DataCurrent.EncryptKey);
						break;
					default:
						throw new NotImplementedException();
				}

				SetValue(x, value);
			}
		}

		internal void Decrypt()
		{
			foreach(Column x in Columns) {
				if(!x.IsString || null == x.Encryption) { continue; }
				string value = GetValue(x).ToString();
				if(!value.IsEncrypted()) { continue; }

				switch(x.Encryption.Method) {
					case Encryption.Asymmetry:
						break;
					case Encryption.Symmetry:
						if(Checker.IsEncrypted(value)) {
							value = value.Substring(
								DataConstants.Prefix.Encrypted.Length
							);
						}
						value = value.Decrypt(DataCurrent.EncryptKey);
						break;
					default:
						throw new NotImplementedException();
				}

				SetValue(x, value);
			}
		}
		#endregion

		#region EventHandler
		internal EntityEventArgs InvokeBeforeAdd()
		{
			EntityEventArgs e = new EntityEventArgs();
			if(null != BeforeAdd) { BeforeAdd(this, e); }
			return e;
		}
		internal EntityEventArgs InvokeBeforeModify()
		{
			EntityEventArgs e = new EntityEventArgs();
			if(null != BeforeModify) { BeforeModify(this, e); }
			return e;
		}
		internal EntityEventArgs InvokeBeforeRemove()
		{
			EntityEventArgs e = new EntityEventArgs();
			if(null != BeforeRemove) { BeforeRemove(this, e); }
			return e;
		}
		internal void InvokeAfterSelect()
		{
			if(null != AfterSelect) { AfterSelect(this); }
		}
		internal EntityEventArgs InvokeAfterAdd()
		{
			EntityEventArgs e = new EntityEventArgs();
			if(null != AfterAdd) { AfterAdd(this, e); }
			return e;
		}
		internal EntityEventArgs InvokeAfterModify()
		{
			EntityEventArgs e = new EntityEventArgs();
			if(null != AfterModify) { AfterModify(this, e); }
			return e;
		}
		internal EntityEventArgs InvokeAfterRemove()
		{
			EntityEventArgs e = new EntityEventArgs();
			if(null != AfterRemove) { AfterRemove(this, e); }
			return e;
		}
		internal void InvokeAfterQuery(ref List<IEntity> senders)
		{
			if(null != AfterQuery) { AfterQuery(ref senders); }
		}
		#endregion

		#region SetNullToEmptyAndTrim
		internal static void SetNullToEmptyAndTrim(IEntity sender)
		{
			foreach(var column in sender.Columns) {
				if(!column.IsString) { continue; }
				string value = sender.GetValue(column) as string;
				if(value.IsNullOrEmpty()) { value = string.Empty; }
				sender.SetValue(column, value.Trim());
			}
		}
		#endregion
		#endregion

		#region event handler
		private IEntity _Before;
		private void DifferenceBefore(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == Table.FollowDiff) { return; }
			if(Table.FollowDiff.Modify | Table.FollowDiff.Remove) {
				_Before = Get(EntityName, KeyValue);
			}
		}
		private void DifferenceAfterAdd(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == Table.FollowDiff) { return; }
			if(!Table.FollowDiff.Add) { return; }
			Difference.Handler(DiffMethod.Add, null, this);
		}
		private void DifferenceAfterModify(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == Table.FollowDiff) { return; }
			if(!Table.FollowDiff.Modify) { return; }
			Difference.Handler(DiffMethod.Modify, _Before, this);
		}
		private void DifferenceAfterRemove(IEntity sender, EntityEventArgs e)
		{
			if(!e.Success) { return; }
			if(null == Table.FollowDiff) { return; }
			if(!Table.FollowDiff.Remove) { return; }
			Difference.Handler(DiffMethod.Remove, _Before, null);
		}
		#endregion

		#region private
		private void InitiateColumnValue()
		{
			// 1. InitiateValue
			foreach(Column x in Columns) {
				SetValue(x, GetInitiateValue(x));
			}

			// 2. OnInit
			OnInit(this, new EntityEventArgs());

			// 3. Config
			List<ConfigSetting> settings = new List<ConfigSetting>();
			settings.AddRange(DataCurrent.Application.FindAll(EntityName));
			settings.AddRange(DataCurrent.Application.FindAll(TableName));
			if(settings.Count > 0) {
				foreach(ConfigSetting x in settings) {
					Column column = GetColumn(x.Name);
					if(null == column) { continue; }

					// skip PrimaryKey and AutoUpdate properties
					if(
						column.Spec.PrimaryKey
						||
						column.Initiate.Style.EnumIn(
							InitiateValue.CurrentUser,
							InitiateValue.CurrentUserAndAutoUpdate,
							InitiateValue.Date14AutoUpdate,
							InitiateValue.Date17AutoUpdate,
							InitiateValue.IntegerAndAutoIncremental,
							InitiateValue.UuidAndAutoUpdate,
							InitiateValue.UniqueTicksAndAutoUpdate)) {
						continue;
					}

					// set value
					Reflector.SetValue(this, column.Property, x.Value);
				}
			}
		}
		#endregion

		#region Dynamic
		private List<string> _DynamicMemberNames;
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			if(null == _DynamicMemberNames) {
				_DynamicMemberNames = new List<string>();

				if(null == Record) {
					//Columns
					foreach(Column column in Columns) {
						string name = column.Property.Name;
						if(!_DynamicMemberNames.Contains(name)) {
							_DynamicMemberNames.Add(name);
						}
					}
				} else {
					// Record
					foreach(Any any in Record) {
						string flatColumnName = any.Name.Replace(
							DataConstants.Symbol.UnderScore, string.Empty
						);

						bool exists = Columns.Exists(x =>
							x.Property.Name.Equals(
								flatColumnName, StringComparison.OrdinalIgnoreCase
							)
						);
						if(exists) { continue; }

						if(!_DynamicMemberNames.Contains(flatColumnName)) {
							_DynamicMemberNames.Add(flatColumnName);
						}
					}
				}
			}
			return _DynamicMemberNames;
		}
		#endregion

		#region Differences
		public static Difference Differences<T>(T from, T to)
			where T : class, IEntity, new()
		{
			return Differences(from, to);
		}

		public static Difference Differences(IEntity from, IEntity to)
		{
			if(null == from && null == to) { return null; }
			bool mustDifference = null == from || null == to;
			if(null != from && null != to) {
				if(from.GetType() != to.GetType()) {
					throw new Exception("from and to type don't match!");
				}
			}

			IEntity schema = (null == from)
				? EntityCache.Get(to.EntityName)
				: EntityCache.Get(from.EntityName);
			Difference diff = new Difference();
			diff.EntityName = schema.EntityName;
			foreach(Column column in schema.Columns) {
				if(column.SkipDiff) { continue; }

				string val1 = null == from 
					? string.Empty : from.GetString(column);
				string val2 = null == to
					? string.Empty : to.GetString(column);

				bool theSame = 
					!mustDifference
					&&
					from.IsNullCheck(column.Spec.ColumnName)
					==
					to.IsNullCheck(column.Spec.ColumnName)
					&&
					val1 == val2;
				if(theSame) { continue; }

				DiffValue diffValue = new DiffValue();
				diffValue.ColumnName = column.Spec.ColumnName;
				diffValue.OriginalIsNull =
					null == from || from.IsNullCheck(column.Spec.ColumnName);
				diffValue.OriginalValue = val1;
				diffValue.NewIsNull =
					null == to || to.IsNullCheck(column.Spec.ColumnName);
				diffValue.NewValue = val2;
				diff.Values.Add(diffValue);
			}

			return diff;
		}
		#endregion
	}

	public class Entity<T>
		: Entity, IEntity<T>
		where T : class, IEntity, new()
	{
		#region constructor
		public Entity()
			: base()
		{
		}
		#endregion

		#region property
		public static new string EntityName
		{
			get
			{
				return EntityCache.Find<T>().EntityName;
			}
		}
		public static new string Abbr
		{
			get
			{
				return EntityCache.Find<T>().Abbr;
			}
		}
		public static new Table Table
		{
			get
			{
				return EntityCache.Find<T>().Table;
			}
		}
		#endregion

		#region IEntity
		public Column GetColumn(Expression<Func<T, object>> expression)
		{
			return DataUtility.ToColumn<T>(expression);
		}

		public new MappingInstanceHelper<T, TMember> Mapping<TMember>()
			where TMember : class, IEntity, new()
		{
			MappingInstanceHelper<T, TMember> mapping =
				new MappingInstanceHelper<T, TMember>(this as T);
			return mapping;
		}

		public override bool EnableDynamic
		{
			get;
			set;
		}

		public override void Interceptor(Sql sql)
		{
			Sql<T> s = new Sql<T>(sql);
			Interceptor(s);
			sql = s.Clone();
		}

		public virtual void Interceptor(Sql<T> sql)
		{
		}

		public override void FrontEndInterceptor(Sql sql)
		{
			Sql<T> s = new Sql<T>(sql);
			FrontEndInterceptor(s);
			sql = s.Clone();
		}

		public virtual void FrontEndInterceptor(Sql<T> sql)
		{
		}

		public virtual IQueryable<T> CacheInterceptor(IQueryable<T> datas)
		{
			return datas;
		}

		public T Clone()
		{
			return Clone(null);
		}

		public T Clone(Func<T, T> delegateFunc)
		{
			T clone = new T();

			foreach(Column column in Columns) {
				clone.SetValue(column, GetValue(column));
			}

			switch(KeyColumn.Initiate.Style) {
				case InitiateValue.Uuid:
					clone.KeyValue = Utility.GetUuid();
					break;
				case InitiateValue.UniqueTicks:
					clone.KeyValue = Utility.GetUniqueTicks().ToString();
					break;
				case InitiateValue.None:
				case InitiateValue.Empty:
				case InitiateValue.UuidAndAutoUpdate:
				case InitiateValue.UniqueTicksAndAutoUpdate:
				case InitiateValue.NullDate:
				case InitiateValue.MaxDate:
				case InitiateValue.Date4:
				case InitiateValue.Date8s:
				case InitiateValue.Date8:
				case InitiateValue.Date14s:
				case InitiateValue.Date14:
				case InitiateValue.Date17s:
				case InitiateValue.Date17:
				case InitiateValue.DaysOffset:
				case InitiateValue.Date14AutoUpdate:
				case InitiateValue.Date17AutoUpdate:
				case InitiateValue.IntegerAndAutoIncremental:
				case InitiateValue.ToUpper:
				case InitiateValue.ToLower:
				case InitiateValue.CurrentUser:
				case InitiateValue.CurrentUserAndAutoUpdate:
				case InitiateValue.TimsSpanHex:
				default:
					break;
			}

			if(null != delegateFunc) { clone = delegateFunc(clone); }

			return clone;
		}

		public virtual DataResult DeepClone(Func<T, T> delegateFunc)
		{
			throw new NotImplementedException("Need implemented in each project.");
		}
		#endregion

		#region Access
		public static Sql<T> Sql()
		{
			return new Sql<T>();
		}

		public static T Get(int keyValue)
		{
			return Get(keyValue.ToString());
		}

		public static T Get(string keyValue)
		{
			if(
				!DataCurrent.Data.AllowEmptyKeyValue 
				&& 
				string.IsNullOrEmpty(keyValue)) {
				return null;
			}

			IEntity schema = EntityCache.Find<T>();
			if(schema.IsCompositeKey) {
				throw new KException(
@"There with CompositeKey of the Entity, please use other methods 
'Get(params Any[] anys)' or 'Get(Expression<Func<T, object>> expression)'."
				);
			}

			return Sql()
				.Where(schema.KeyColumn.Spec.ColumnName, keyValue)
				.QueryFirst();
		}
		public static T Get(params Any[] anys)
		{
			IEntity schema = EntityCache.Find<T>();
			return Sql()
				.Where(anys)
				.QueryFirst();
		}
		public static T Get(Expression<Func<T, object>> expression)
		{
			IEntity schema = EntityCache.Find<T>();
			return Sql()
				.Where(expression)
				.QueryFirst();
		}
		public static T GetOrAdd(string keyValue)
		{
			return GetOrAdd(keyValue, null);
		}
		public static T GetOrAdd(string keyValue, Action<T> setting)
		{
			T one = Get(keyValue);
			if(null == one) {
				one = new T();
				if(null != setting) { setting(one); }
				var result = one.Add();
				if(!result.Success) { one = null; }
			}
			return one;
		}
		public static List<T> GetAll()
		{
			return Sql().Query();
		}

		public static int Count(params Any[] anys)
		{
			return Sql()
				.Where(anys)
				.Count();
		}
		public static int Count(Expression<Func<T, object>> expression)
		{
			return Sql()
				.Where(expression)
				.Count();
		}
		public static int Count(
			Expression<Func<T, object>> expression,
			Action<Sql<T>> interceptor)
		{
			return Sql()
				.Where(expression)
				.Intercept(interceptor)
				.Count();
		}

		public static List<T> ConditionSearch(
			string keyword,
			string condition)
		{
			return Sql()
				.Where(condition)
				.WhereSearch(keyword)
				.Query();
		}
		public static List<T> Search(
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return Sql()
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Query();
		}
		public static List<T> Search(
			string keyword,
			Action<Sql<T>> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return Sql()
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Intercept(interceptor)
				.Query();
		}
		public static int SearchCount(
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return Sql()
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Count();
		}
		public static int SearchCount(
			string keyword,
			Action<Sql<T>> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return Sql()
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Intercept(interceptor)
				.Count();
		}
		public static bool SearchExists(
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return SearchExists(
				keyword,
				null,
				skipColumnNamesOrPropertyNames
			);
		}
		public static bool SearchExists(
			string keyword,
			Action<Sql<T>> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return SearchCount(
				keyword,
				interceptor,
				skipColumnNamesOrPropertyNames
			) > 0;
		}

		public static T QueryFirst(params Any[] anys)
		{
			return Sql().Where(anys).QueryFirst();
		}

		public static T QueryFirst(Expression<Func<T, object>> expression)
		{
			return Sql().Where(expression).QueryFirst();
		}

		public static List<T> Query(string condition)
		{
			return Entity
				.Query(typeof(T).Name, condition)
				.ConvertAll<T>(x => x as T);
		}

		public static List<T> Query(params Any[] anys)
		{
			return Entity
				.Query(typeof(T).Name, anys)
				.ConvertAll<T>(x => x as T);
		}

		public static List<T> Query(Expression<Func<T, object>> expression)
		{
			return Sql().Where(expression).Query();
		}

		public static bool Exists(string keyValue)
		{
			IEntity schema = EntityCache.Find<T>();
			return Sql().Where(new Any(schema.KeyName, keyValue)).Exists();
		}

		public static bool Exists(params Any[] anys)
		{
			return Sql().Where(anys).Exists();
		}

		public static bool Exists(Expression<Func<T, object>> expression)
		{
			return Sql().Where(expression).Exists();
		}

		public static Sql<T> Where(Expression<Func<T, object>> expression)
		{
			return Sql().Where(expression);
		}

		public static Sql<T> Where(
			bool addCondition, 
			Expression<Func<T, object>> expression)
		{
			return Sql().Where(addCondition, expression);
		}

		public static Sql<T> In(
			Expression<Func<T, object>> expression,
			params int[] values)
		{
			return Sql().In(expression, values);
		}

		public static Sql<T> In(
			Expression<Func<T, object>> expression,
			params string[] values)
		{
			return Sql().In(expression, values);
		}

		public static Sql<T> In(
			Expression<Func<T, object>> expression,
			Sql sql)
		{
			return Sql().In(expression, sql);
		}

		public static Sql<T> In<I>(
			Expression<Func<T, object>> expression,
			Sql<I> sql)
			where I : class, IEntity, new()
		{
			return Sql().In<I>(expression, sql);
		}

		public static Sql<T> Ascending(
			Expression<Func<T, object>> expression)
		{
			return Sql().Ascending(expression);
		}

		public static Sql<T> Descending(
			Expression<Func<T, object>> expression)
		{
			return Sql().Descending(expression);
		}

		public static DataResult Modify(string keyValue, params Any[] anys)
		{
			T schema = EntityCache.GetFirst<T>();
			Sql<T> sql = new Sql<T>().Where(schema.KeyName, keyValue);
			foreach(Any any in anys) {
				sql.SetValue(any.Name, any.ToString());
			}

			sql.Schema.AllowBatchModify = true;
			DataResult result = sql.Modify();

			return result;
		}

		public static DataResult Remove(string keyValue)
		{
			IEntity schema = EntityCache.Find<T>();
			return Sql()
				.Where(new Any(schema.KeyName, keyValue))
				.Remove();
		}

		public static DataResult Remove(Any any, params Any[] anys)
		{
			return Sql().Where(anys).Remove();
		}

		public static DataResult Remove(
			Expression<Func<T, object>> expression)
		{
			return Sql().Where(expression).Remove();
		}

		public static DataResult SearchRemove(
			string keyword,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return SearchRemove(
				keyword,
				null,
				skipColumnNamesOrPropertyNames
			);
		}
		public static DataResult SearchRemove(
			string keyword,
			Action<Sql<T>> interceptor,
			params string[] skipColumnNamesOrPropertyNames)
		{
			return Sql()
				.WhereSearch(keyword, skipColumnNamesOrPropertyNames)
				.Intercept(interceptor)
				.Remove();
		}

		public static List<T> ExecuteQuery(
			string sql, params DbParameter[] parameters)
		{
			DataSet ds = Api.GetNew(EntityName).ExecuteQuery(sql, parameters);
			return ds.ToEntity<T>();
		}
		public static DataResult ExecuteNonQuery(
			string sql, params DbParameter[] parameters)
		{
			DataResult result = Api
				.Get(EntityName)
				.ExecuteNonQuery(sql, parameters);
			return result;
		}
		public static List<T> ExecuteStoredProcedure(
			string sql, params DbParameter[] parameters)
		{
			DataSet ds = Api
				.Get(EntityName)
				.ExecuteStoredProcedure(sql, parameters);
			return ds.ToEntity<T>();
		}
		public static Any ExecuteScalar(
			string sql, params DbParameter[] parameters)
		{
			return Api.GetNew(EntityName).ExecuteScalar(sql, parameters);
		}
		#endregion

		#region Mapping
		public static List<TMember> Members<TMember>(
			params string[] containerIDs)
			where TMember : class,IEntity, new()
		{
			return Members<TMember>(null, containerIDs);
		}

		public static List<TMember> Members<TMember>(
			Func<Sql<TMember>, Sql<TMember>> sqlIntercepter,
			params string[] containerIDs)
			where TMember : class, IEntity, new()
		{
			return Builtins
				.Get<IMapping, NullMapping>()
				.Get<T, TMember>(sqlIntercepter, containerIDs);
		}
		#endregion

		#region Null Control
		public bool IsNullCheck(Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn(expression);
			return base.IsNullCheck(column.Spec.ColumnName);
		}

		public void IsNull(Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn(expression);
			base.IsNull(column.Spec.ColumnName);
		}

		public void NotNull(Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn(expression);
			base.NotNull(column.Spec.ColumnName);
		}
		#endregion

		#region ToHtmlDataAttribute
		public HtmlString ToHtmlDataAttribute(
			params Expression<Func<T, object>>[] expressions)
		{
			StringBuilder sb = new StringBuilder();
			foreach(var expression in expressions) {
				Column column = DataUtility.ToColumn(expression);
				object val = GetValue(column);
				string value = null == val 
					? string.Empty 
					: val.ToString().HtmlEncode();
				sb.AppendFormat(
					" data-{0}=\"{1}\"",
					column.Property.Name,
					value
				);
			}
			return new HtmlString(sb.ToString());
		}
		#endregion

		#region static
		public static string GetAlias()
		{
			return EntityCache.GetAlias(typeof(T).Name);
		}

		public static IEntity CachedSpec
		{
			get
			{
				return EntityCache.Get(typeof(T).Name);
			}
		}
		#endregion
	}
}
