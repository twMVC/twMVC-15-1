// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Any.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using System.Drawing;

namespace Kuick
{
	[Serializable]
	public class Any : IName, ICloneable<Any>
	{
		public const string NAME = "Name";
		public const string VALUE = "Value";
		public const string VALUES = "Values";

		#region constructor
		public Any()
		{
		}

		public Any(string name)
		{
			this.Name = name;
		}

		public Any(string name, params object[] vals)
		{
			this.Name = name;
			if(Checker.IsNull(vals)) { return; }
			if(vals.Length == 1) {
				this.Value = vals[0];
			} else {
				this.Values = vals;
			}
		}
		#endregion

		#region ICloneable<T>
		public Any Clone()
		{
			Any clone = new Any();
			clone.Name = this.Name;
			clone.Value = this.Value;
			clone.Values = this.Values;

			return clone;
		}
		#endregion

		#region IName
		public string Name { get; set; }

		public bool Equals(string name)
		{
			if(Checker.IsNull(Name) || Checker.IsNull(name)) { return false; }
			return this.Name == name;
		}
		#endregion

		#region IEquatable<IName>
		public bool Equals(IName other)
		{
			if(null == other) { return false; }
			return this.Name == other.Name;
		}
		#endregion

		#region property
		private object _Value;
		public object Value 
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;

				if(Checker.IsNull(value)) {
					_ValueType = DataFormat.Unknown;
				} else {
					Type type = value.GetType();
					_ValueType = Reflector.GetDataType(type);

				}
			}
		}

		private object[] _Values;
		public object[] Values
		{
			get
			{
				return _Values;
			}
			set
			{
				_Values = value;

				if(Checker.IsNull(value)) {
					_ValueType = DataFormat.Objects;
				} else {
					object obj = value[0];
					if(obj.GetType().IsByte()) {
						_ValueType = DataFormat.ByteArray;
					} else {
						_ValueType = DataFormat.Objects;
					}
				}
			}
		}

		private DataFormat _ValueType;
		public DataFormat ValueType
		{
			get
			{
				if(_ValueType == DataFormat.Unknown) {
					return Checker.IsNull(Value)
						? Checker.IsNull(Values)
							? DataFormat.String
							: DataFormat.Objects
						: Reflector.GetDataType(Value);
				}
				return _ValueType;
			}
		}
		#endregion

		#region ToXxx
		public override string ToString()
		{
			switch(ValueType) {
				case DataFormat.Unknown:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
				case DataFormat.String:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
				case DataFormat.Char:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
				case DataFormat.Boolean:
					return ToBoolean().ToString().ToLower();
				case DataFormat.DateTime:
					return ToDateTime().yyyyMMddHHmmss();
				case DataFormat.Integer:
					return ToInteger().ToString();
				case DataFormat.Decimal:
					return ToDecimal().ToString("0.00");
				case DataFormat.Double:
					return ToDecimal().ToString("0.00");
				case DataFormat.Byte:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
				case DataFormat.Short:
					return ToDecimal().ToString("0.00");
				case DataFormat.Long:
					return ToDecimal().ToString("0.00");
				case DataFormat.Float:
					return ToDecimal().ToString("0.00");
				case DataFormat.Enum:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
				case DataFormat.Object:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
				case DataFormat.Objects:
					return Reflector.ToString(typeof(string), Values);
				case DataFormat.ByteArray:
					return Formator.BytesToString(ToByteArray());
				case DataFormat.Color:
					return ToColor().ToString();
				default:
					return Checker.IsNull(Value) ? string.Empty : Value.ToString();
			}
		}

		public bool ToBoolean()
		{
			if(ValueType == DataFormat.Boolean) {
				return null == Value ? false : (bool)Value;
			}
			return Formator.AirBagToBoolean(
				null == Value ? string.Empty : Value.ToString(), 
				false
			);
		}

		public DateTime ToDateTime()
		{
			if(ValueType == DataFormat.DateTime) {
				return null == Value ? DateTime.MinValue : (DateTime)Value;
			}
			return Formator.AirBagToDateTime(
				null == Value ? string.Empty : Value.ToString(), 
				Constants.Date.Null
			);
		}

		public byte ToByte()
		{
			if(ValueType == DataFormat.Byte) {
				return null == Value ? default(byte) : (byte)Value;
			}
			return Formator.AirBagToByte(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public char ToChar()
		{
			if(ValueType == DataFormat.Char) {
				return null == Value ? default(char) : (char)Value;
			}
			return Formator.AirBagToChar(Value.ToString());
		}

		public int ToInteger()
		{
			if(ValueType == DataFormat.Integer) {
				return null == Value ? default(int) : (int)Value;
			}
			return Formator.AirBagToInt(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public float ToFloat()
		{
			if(ValueType == DataFormat.Float) {
				return null == Value ? default(float) : (float)Value;
			}
			return Formator.AirBagToFloat(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public double ToDouble()
		{
			if(ValueType == DataFormat.Double) {
				return null == Value ? default(double) : (double)Value;
			}
			return Formator.AirBagToDouble(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public short ToShort()
		{
			if(ValueType == DataFormat.Short) {
				return null == Value ? default(short) : (short)Value;
			}
			return Formator.AirBagToShort(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public long ToLong()
		{
			if(ValueType == DataFormat.Long) {
				return null == Value ? default(long) : (long)Value;
			}
			return Formator.AirBagToLong(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public decimal ToDecimal()
		{
			if(ValueType == DataFormat.Decimal) {
				return null == Value ? default(decimal) : (decimal)Value;
			}
			return Formator.AirBagToDecimal(
				null == Value ? string.Empty : Value.ToString()
			);
		}

		public Color ToColor()
		{
			if(ValueType == DataFormat.Color) {
				return null == Value ? default(Color) : (Color)Value;
			}
			return default(Color);
		}

		public Guid ToGuid()
		{
			if (ValueType == DataFormat.Guid) {
				return null == Value ? default(Guid) : (Guid)Value;
			}
			return default(Guid);
		}

		public byte[] ToByteArray()
		{
			if(ValueType == DataFormat.ByteArray) {
				return Checker.IsNull(Values) ? default(byte[]) : (byte[])Value;
			}

			List<byte> list = new List<byte>();
			foreach(object obj in Values) {
				list.Add((byte)obj);
			}
			return list.ToArray();
		}

		public T ToEnum<T>()
		{
			return ToEnum<T>(default(T));
		}

		public T ToEnum<T>(T airBag)
		{
			return Formator.AirBagToEnum<T>(
				null == Value ? string.Empty : Value.ToString(), 
				airBag
			);
		}

		public string ToNameValueString()
		{
			return string.Format("{0}={1}", Name, ToString());
		}
		#endregion

		#region Concat
		public static Any[] Concat(params Any[] anys)
		{
			return anys;
		}

		public static Any[] Concat(Any any, Any[] anys)
		{
			List<Any> list = new List<Any>();
			if(!Checker.IsNull(any)) { list.Add(any); }
			if(!Checker.IsNull(anys)) { list.AddRange(anys); }
			return list.ToArray();
		}

		public static Any[] Concat(Any[] anys1, Any[] anys2)
		{
			List<Any> list = new List<Any>();
			if(!Checker.IsNull(anys1)) { list.AddRange(anys1); }
			if(!Checker.IsNull(anys2)) { list.AddRange(anys2); }
			return list.ToArray();
		}
		#endregion

		#region Array
		public static Any[] Array(object obj)
		{
			List<Any> list = new List<Any>();
			PropertyInfo[] infos = obj.GetType().GetProperties();
			if(!Checker.IsNull(infos)) {
				#region Parallel
				Parallel.ForEach<PropertyInfo>(infos, info => {
					if(!info.CanRead) { return; }
					object val = Reflector.GetValue(info.Name, obj);
					list.Add(new Any(info.Name, val));
				});
				#endregion

				#region obsolete: single threading
				//foreach(PropertyInfo info in infos) {
				//    if(!info.CanRead) { continue; }
				//    object val = Reflector.GetValue(info.Name, obj);
				//    list.Add(new Any(info.Name, val));
				//}
				#endregion
			}
			return list.ToArray();
		}

		public static Any[] Array(NameValueCollection nvs)
		{
			if(null == nvs) { return null; }
			if(Checker.IsNull(nvs.AllKeys)) { return null; }
			var list = new List<Any>();
			foreach(string key in nvs.AllKeys) {
				list.Add(new Any(key, nvs[key]));
			}
			return list.ToArray();
		}

		public static Any[] Array(params object[] objs)
		{
			if(Checker.IsNull(objs)) { return null; }
			if(!Checker.IsEven(objs.Length)) { return null; }
			string n = string.Empty;
			object v = null;
			var nvs = new Any[objs.Length / 2];
			for(int i = 0; i < objs.Length; i++) {
				if(!Checker.IsEven(i + 1)) {
					n = string.Empty;
					v = null;
					n = objs[i] as string;
				} else {
					v = objs[i];
					nvs[(i - 1) / 2] = new Any(n, v);
				}
			}
			return nvs;
		}
		#endregion
	}
}
