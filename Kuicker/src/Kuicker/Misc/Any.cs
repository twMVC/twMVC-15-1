// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Kuicker
{
	public class Any : ICloneable //, IEquatable<Any>
	{
		#region constructor
		public Any()
			: this(string.Empty, string.Empty)
		{
		}

		public Any(string name)
			: this(name, string.Empty)
		{
		}

		public Any(string name, params object[] vals)
		{
			this.Name = name;
			if(vals.IsNullOrEmpty()) { return; }
			if(vals.Length == 1) {
				this.Value = vals[0];
			} else {
				this.Values = vals;
			}
		}
		#endregion

		public string Name { get; set; }

		#region ICloneable
		public object Clone()
		{
			Any clone = new Any();

			clone.Name = this.Name;
			clone.Value = this.Value;
			clone.Values = this.Values;

			return clone;
		}
		#endregion

		public override bool Equals(object obj)
		{
			if(null == obj) { return false; }
			Any any = obj as Any;
			if(null == any) { return false; }
			return this.Name.Equals(any.Name);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region property
		public object Value { get; set; }
		public object[] Values { get; set; }

		public DataFormat Format
		{
			get
			{
				if(null != Value) { Reflector.GetDataFormat(Value); }
				if(null != Values) { Reflector.GetDataFormat(Values); }
				return DataFormat.Unknown;
			}
		}
		#endregion

		#region ToXxx
		public override string ToString()
		{
			return ToString(string.Empty);
		}
		public string ToString(string airBag)
		{
			switch(Format) {
				case DataFormat.Unknown:
				case DataFormat.String:
				case DataFormat.Char:
					return null == Value ? airBag : Value.ToString();
				case DataFormat.Boolean:
					return ToBoolean().ToString().ToLower();
				case DataFormat.DateTime:
					return ToDateTime().yyyy_MM_dd_HH_mm_ss();
				case DataFormat.Integer:
					return ToInteger().ToString();
				case DataFormat.Decimal:
					return ToDecimal().ToString("0.00");
				case DataFormat.Double:
					return ToDecimal().ToString("0.00");
				case DataFormat.Byte:
					return ToByte().ToString();
				case DataFormat.Short:
					return ToShort().ToString("0.00");
				case DataFormat.Long:
					return ToLong().ToString("0.00");
				case DataFormat.Float:
					return ToFloat().ToString("0.00");
				case DataFormat.Enum:
					return Value.ToString();
				case DataFormat.Object:
					return null == Value ? airBag : Value.ToString();
				case DataFormat.Objects:
					return Reflector.ToString(typeof(string), Values);
				case DataFormat.ByteArray:
					return ToByteArray().ToString();
				case DataFormat.Color:
					return ToColor().ToString();
				default:
					return null == Value ? airBag : Value.ToString();
			}
		}

		public string[] ToStrings()
		{
			return ToStrings(new string[0]);
		}
		public string[] ToStrings(string[] airBag)
		{
			return ToStrings(Constants.Symbol.Comma, airBag);
		}
		public string[] ToStrings(string seperator)
		{
			return ToStrings(seperator, new string[0]);
		}
		public string[] ToStrings(string seperator, string[] airBag)
		{
			var list = new List<string>();
			var parts = ToString().Split(seperator);
			foreach(var part in parts) {
				if(part.IsNullOrEmpty()) { continue;}
				list.Add(part);
			}
			if(list.IsNullOrEmpty()) { return airBag; }
			return list.ToArray();
		}

		public int[] ToIntegers()
		{
			return ToIntegers(new int[0]);
		}
		public int[] ToIntegers(int[] airBag)
		{
			return ToIntegers(Constants.Symbol.Comma, airBag);
		}
		public int[] ToIntegers(string seperator)
		{
			return ToIntegers(seperator, new int[0]);
		}
		public int[] ToIntegers(string seperator, int[] airBag)
		{
			var list = new List<int>();
			var parts = ToString().Split(seperator);
			foreach(var part in parts) {
				if(part.IsNumeric()) {
					int value = part.ToInt();
					list.Add(value);
				}
			}
			if(list.IsNullOrEmpty()) { return airBag; }
			return list.ToArray();
		}

		public bool ToBoolean()
		{
			return ToBoolean(false);
		}

		public bool ToBoolean(bool airBag)
		{
			if(Format == DataFormat.Boolean) {
				return null == Value ? airBag : (bool)Value;
			}
			return airBag;
		}

		public DateTime ToDateTime()
		{
			return ToDateTime(DateTime.MinValue);
		}

		public DateTime ToDateTime(DateTime airBag)
		{
			if(Format == DataFormat.DateTime) {
				return null == Value ? airBag : (DateTime)Value;
			}
			return airBag;
		}

		public byte ToByte()
		{
			return ToByte(default(byte));
		}

		public byte ToByte(byte airBag)
		{
			if(Format == DataFormat.Byte) {
				return null == Value ? airBag : (byte)Value;
			}
			return airBag;
		}

		public char ToChar()
		{
			return ToChar(default(char));
		}

		public char ToChar(char airBag)
		{
			if(Format == DataFormat.Char) {
				return null == Value ? airBag : (char)Value;
			}
			return airBag;
		}

		public int ToInteger()
		{
			return ToInteger(default(int));
		}

		public int ToInteger(int airBag)
		{
			if(Format == DataFormat.Integer) {
				return null == Value ? airBag : (int)Value;
			}
			return airBag;
		}

		public float ToFloat()
		{
			return ToFloat(default(float));
		}

		public float ToFloat(float airBag)
		{
			if(Format == DataFormat.Float) {
				return null == Value ? airBag : (float)Value;
			}
			return airBag;
		}

		public double ToDouble()
		{
			return ToDouble(default(double));
		}

		public double ToDouble(double airBag)
		{
			if(Format == DataFormat.Double) {
				return null == Value ? airBag : (double)Value;
			}
			return airBag;
		}

		public short ToShort()
		{
			return ToShort(default(short));
		}

		public short ToShort(short airBag)
		{
			if(Format == DataFormat.Short) {
				return null == Value ? airBag : (short)Value;
			}
			return airBag;
		}

		public long ToLong()
		{
			return ToLong(default(long));
		}

		public long ToLong(long airBag)
		{
			if(Format == DataFormat.Long) {
				return null == Value ? airBag : (long)Value;
			}
			return airBag;
		}

		public decimal ToDecimal()
		{
			return ToDecimal(default(decimal));
		}

		public decimal ToDecimal(decimal airBag)
		{
			if(Format == DataFormat.Decimal) {
				return null == Value ? airBag : (decimal)Value;
			}
			return airBag;
		}

		public Color ToColor()
		{
			return ToColor(default(Color));
		}

		public Color ToColor(Color airBag)
		{
			if(Format == DataFormat.Color) {
				return null == Value ? airBag : (Color)Value;
			}
			return airBag;
		}

		public Guid ToGuid()
		{
			return ToGuid(default(Guid));
		}

		public Guid ToGuid(Guid airBag)
		{
			if(Format == DataFormat.Guid) {
				return null == Value ? airBag : (Guid)Value;
			}
			return airBag;
		}

		public byte[] ToByteArray()
		{
			return ToByteArray(new byte[0]);
		}

		public byte[] ToByteArray(byte[] airBag)
		{
			if(Format == DataFormat.ByteArray) {
				return null == Values ? airBag : (byte[])Value;
			}
			return airBag;
		}

		public T ToEnum<T>()
		{
			return ToEnum<T>(default(T));
		}

		public T ToEnum<T>(T airBag)
		{
			if(Format == DataFormat.Enum) {
				return (T)Value;
			}
			return airBag;
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
			if(null != any) { list.Add(any); }
			if(!anys.IsNullOrEmpty()) { list.AddRange(anys); }
			return list.ToArray();
		}

		public static Any[] Concat(Any[] anys1, Any[] anys2)
		{
			List<Any> list = new List<Any>();
			if(!anys1.IsNullOrEmpty()) { list.AddRange(anys1); }
			if(!anys2.IsNullOrEmpty()) { list.AddRange(anys2); }
			return list.ToArray();
		}
		#endregion
	}
}
