// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlReader.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Data;
using System.Web;
using System.Data.Common;
using System.Collections.Generic;

namespace Kuick.Data
{
	public abstract class SqlReader<T>
		where T : IDataReader, IDisposable
	{
		public SqlReader(T reader)
		{
			this.Reader = reader;
		}

		#region Reader
		protected T Reader { get; set; }

		public virtual bool Read()
		{
			return Reader.Read();
		}

		public virtual void Close()
		{
			Reader.Close();
		}

		public virtual int GetOrdinal(string name)
		{
			try {
				int order;
				if(FieldOrders.TryGetValue(name.ToUpper(), out order)) {
					return order;
				}
				return -1;
			} catch {
				return -1;
			}
		}

		public virtual object this[string columnName]
		{
			get
			{
				int index = GetOrdinal(columnName);
				return this[index];
			}
		}

		public virtual object this[int index]
		{
			get
			{
				if(index < 0 || null == Reader) { return null; }
				return Reader[index];
			}
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Reader.Dispose();
		}
		#endregion

		// Current
		private IEntity CurrentSchema { get; set; }
		private Column CurrentColumn { get; set; }

		// Bind
		public void Bind(IntervalLogger il, IEntity instance)
		{
			CurrentSchema = instance;

			// null control
			((Entity)instance).NullFields = new List<Column>();

			foreach(Column column in instance.Columns) {
				CurrentColumn = column;

				string asName = Contains(column.Spec.ColumnName)
					? column.Spec.ColumnName
					: Contains(column.AsName)
						? column.AsName
						: null;
				if(null == asName) { continue; }

				try {
					// null control
					if(IsDBNull(asName)) {
						((Entity)instance).NullFields.Add(column);
					}

					object val = null;
					Type type = column.Property.PropertyType;

					switch(column.Format) {
						case ColumnDataFormat.String:
							val = this.GetString(asName);

							if(
								null != val
								&&
								column.Visual.Input == VisualInput.HtmlEditor) {
								val = HttpUtility.HtmlDecode((string)val);
							}
							break;
						case ColumnDataFormat.Integer:
							val = this.GetInteger(asName);
							break;
						case ColumnDataFormat.Int64:
							val = this.GetInt64(asName);
							break;
						case ColumnDataFormat.Decimal:
							val = this.GetDecimal(asName);
							break;
						case ColumnDataFormat.Long:
							val = this.GetLong(asName);
							break;
						case ColumnDataFormat.Short:
							val = this.GetShort(asName);
							break;
						case ColumnDataFormat.Double:
							val = this.GetDouble(asName);
							break;
						case ColumnDataFormat.Float:
							val = this.GetFloat(asName);
							break;
						case ColumnDataFormat.Boolean:
							string s = string.Empty;
							if(column.Spec.DbType.In(
								SqlDataType.Bit, 
								SqlDataType.Integer)) {
								s = this.GetInteger(asName).ToString();
							}else{
								s = this.GetString(asName);
							}
							val =
								s.Equals("True", StringComparison.OrdinalIgnoreCase)
								||
								s.Equals("1", StringComparison.OrdinalIgnoreCase)
								||
								s.Equals("T", StringComparison.OrdinalIgnoreCase)
								||
								s.Equals("Y", StringComparison.OrdinalIgnoreCase);
							break;
						case ColumnDataFormat.Char:
							break;
						case ColumnDataFormat.Enum:
							string original = this.GetString(asName);
							if(original.IsNullOrEmpty()) { original = string.Empty; }
							try {
								var er = EnumCache.Get(type);
								var ei = er.Get(original.ToString());
								val = Enum.Parse(type, ei.Value, true);
							} catch(Exception ex) {
								string values = Reflector
									.GetEnumPossibleValues(type)
									.Join(", ");
								Logger.Error(
									"SqlReader.Bind",
									"Dirty enum data alert! " + ex.Message,
									new Any("Entity Name", instance.EntityName),
									new Any("Key Vlue", instance.KeyValue),
									new Any("Column Name", column.Spec.ColumnName),
									new Any(
										"Property Type",
										column.Property.PropertyType),
									new Any("Dirty Value", original),
									new Any("Possible Values", values)
								);
								string defValue = Reflector.GetEnumDefaultValue(type);
								val = Enum.Parse(type, defValue, true);
							}
							break;
						case ColumnDataFormat.Byte:
							val = this.GetByte(asName);
							break;
						case ColumnDataFormat.ByteArray:
							val = this.GetBytes(asName);
							break;
						case ColumnDataFormat.DateTime:
							val = this.GetDateTime(asName);
							break;
						case ColumnDataFormat.Color:
							int color = this.GetInteger(asName);
							val = System.Drawing.Color.FromArgb(color);
							break;
						case ColumnDataFormat.Guid:
							val = this.GetGuid(asName);
							break;
						default:
							Logger.Error(
								this.GetType().Name + ".Bind",
								"Unhandled Property Type",
								new Any("Property Type", type.Name)
							);
							throw new NotImplementedException();
					}
					column.Property.SetValue(instance, val, new Object[0]);
				} catch(Exception ex) {
					Logger.Error(
						this.GetType().Name + ".Bind",
						"Error entity property type.",
						ex.ToAny(
							new Any("Entity Name", instance.GetType().Name),
							new Any("Column Name", column.Spec.ColumnName),
							new Any(
								"Error Property Type", 
								column.Property.PropertyType.Name
							)
						)
					);
				}
			}

			// null control
			((Entity)instance).NullColumns = ((Entity)instance).NullFields;

			// dynamic
			if(instance.EnableDynamic) {
				try {
					instance.Record = new Anys();
					for(int i = 0; i < Reader.FieldCount; i++) {
						instance.Record.Add(Reader.GetName(i), GetValue(i));
					}
				} catch(Exception ex) {
					Logger.Error(
						"SqlReader.Bind: in dynamic section (Record)",
						ex
					);
				}
			}
		}
		public void Bind(Anys anys)
		{
			string[] allKeys = anys.AllNames;
			anys = new Anys();
			foreach(string key in allKeys) {
				object val = this.GetValue(key);
				anys.Add(new Any(key, val));
			}
		}

		// IsDBNull
		public bool IsDBNull(string columnName)
		{
			return IsDBNull(GetOrdinal(columnName));
		}
		public virtual bool IsDBNull(int index)
		{
			if(index < 0 || null == Reader) { return true; }

			try {
				return Reader.IsDBNull(index);
			} catch(Exception ex) {
				WriteErrorMessage("IsDBNull", ex);
			}
			return true;
		}

		// GetInteger
		public int GetInteger(string columnName)
		{
			return GetInteger(GetOrdinal(columnName));
		}
		public virtual int GetInteger(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { return 0; }

			try {
				// return Reader.GetInt32(index);

				int v = 0;
				object obj = GetValue(index);
				if(null == obj) { obj = 0; }
				if(!int.TryParse(obj.ToString(), out v)) {
					string s = obj.ToString();
					v = s.Equals(
						DataConstants.StringBoolean.True, 
						StringComparison.OrdinalIgnoreCase
					)
						? 1
						: s.Equals(
							DataConstants.StringBoolean.Yes, 
							StringComparison.OrdinalIgnoreCase
						)
							? 1 
							: s == "1"
								? 1 
								: 0;
				}
				return v;
			} catch(Exception ex) {
				WriteErrorMessage("GetInteger", ex);
			}
			return default(int);
		}

		// GetLong
		public Int64 GetLong(string columnName)
		{
			return GetLong(GetOrdinal(columnName));
		}
		public virtual Int64 GetLong(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { return 0; }
			try {
				return Reader.GetInt64(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetLong", ex);
			}
			return default(Int64);
		}

		// GetShort
		public short GetShort(string columnName)
		{
			return GetShort(GetOrdinal(columnName));
		}
		public virtual short GetShort(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(short); 
			}

			try {
				return Reader.GetInt16(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetShort", ex);
			}
			return default(short);
		}

		// GetGuid
		public Guid GetGuid(string columnName)
		{
			return GetGuid(GetOrdinal(columnName));
		}
		public virtual Guid GetGuid(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) {
				return default(Guid); 
			}

			try {
				return Reader.GetGuid(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetGuid", ex);
			}
			return default(Guid);
		}

		// GetGuid
		public Int64 GetInt64(string columnName)
		{
			return GetInt64(GetOrdinal(columnName));
		}
		public virtual Int64 GetInt64(int index)
		{
			if (index < 0 || null == Reader || IsDBNull(index)) {
				return default(Int64);
			}

			try {
				return Reader.GetInt64(index);
			} catch (Exception ex) {
				WriteErrorMessage("GetInt64", ex);
			}
			return default(Int64);
		}

		// GetString
		public string GetString(string columnName)
		{
			return GetString(GetOrdinal(columnName));
		}
		public virtual string GetString(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(string);
			}

			try {
				return Reader.GetString(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetString", ex);
			}
			return default(string);
		}

		// GetDateTime
		public DateTime GetDateTime(string columnName)
		{
			return GetDateTime(GetOrdinal(columnName));
		}
		public virtual DateTime GetDateTime(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(DateTime); 
			}

			try {
				return Reader.GetDateTime(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetDateTime", ex);
			}
			return default(DateTime);
		}

		// GetFloat
		public float GetFloat(string columnName)
		{
			return GetFloat(GetOrdinal(columnName));
		}
		public virtual float GetFloat(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(float);
			}

			try {
				object value = Reader.GetValue(index);
				return Convert.ToSingle(value);
			} catch(Exception ex) {
				WriteErrorMessage("GetFloat", ex);
			}
			return default(float);
		}

		// GetDouble
		public double GetDouble(string columnName)
		{
			return GetDouble(GetOrdinal(columnName));
		}
		public virtual double GetDouble(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(double); 
			}

			try {
				return Reader.GetDouble(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetDouble", ex);
			}
			return default(double);
		}

		// GetDecimal
		public decimal GetDecimal(string columnName)
		{
			return GetDecimal(GetOrdinal(columnName));
		}
		public virtual decimal GetDecimal(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(decimal); 
			}

			try {
				return Reader.GetDecimal(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetDecimal", ex);
			}
			return default(decimal);
		}

		// GetBytes
		public byte[] GetBytes(string columnName)
		{
			return GetBytes(GetOrdinal(columnName));
		}
		public virtual byte[] GetBytes(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(byte[]); 
			}

			try {
				if(IsDBNull(index)) { return default(byte[]); }
				object obj = Reader.GetValue(index);
				byte[] bytes = obj as byte[];
				return bytes;
			} catch(Exception ex) {
				WriteErrorMessage("GetBytes", ex);
			}
			return default(byte[]);
		}

		// GetByte
		public byte GetByte(string columnName)
		{
			return GetByte(GetOrdinal(columnName));
		}
		public virtual byte GetByte(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(byte); 
			}

			try {
				return Reader.GetByte(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetByte", ex);
			}
			return default(byte);
		}

		// GetValue
		public object GetValue(string columnName)
		{
			return GetValue(GetOrdinal(columnName));
		}
		public virtual object GetValue(int index)
		{
			if(index < 0 || null == Reader || IsDBNull(index)) { 
				return default(object); 
			}

			try {
				return Reader.GetValue(index);
			} catch(Exception ex) {
				WriteErrorMessage("GetValue", ex);
			}
			return default(object);
		}

		// Fields
		private Dictionary<string, int> _FieldOrders;
		public Dictionary<string, int> FieldOrders
		{
			get
			{
				if(null == _FieldOrders) {
					_FieldOrders = new Dictionary<string, int>();
					for(int i = 0; i < Reader.FieldCount; i++) {
						string name = Reader.GetName(i).ToUpper();
						int order = Reader.GetOrdinal(name);
						_FieldOrders.Add(name, order);
					}
				}
				return _FieldOrders;
			}
		}

		// Contains
		public bool Contains(string fieldName)
		{
			return FieldOrders.ContainsKey(fieldName.ToUpper());
		}

		// WriteErrorMessage
		private void WriteErrorMessage(string methodName, Exception ex)
		{
			Logger.Error(
				this.GetType().FullName,
				methodName,
				ex.ToAny(
					new Any("Table Name", CurrentSchema.Table.TableName),
					new Any("Column Name", CurrentColumn.Spec.ColumnName),
					new Any("Entity Name", CurrentSchema.Table.EntityName),
					new Any("Property Name", CurrentColumn.Property.Name),
					new Any("Property Type", CurrentColumn.Property.PropertyType)
				)
			);
		}
	}
}
