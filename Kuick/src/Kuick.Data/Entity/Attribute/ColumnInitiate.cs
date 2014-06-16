// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnInitiate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Drawing;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ColumnInitiate : Attribute, ICloneable<ColumnInitiate>
	{
		#region constructor
		public ColumnInitiate(string value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(int value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(decimal value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(long value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(short value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(double value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(float value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(bool value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(char value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(int value, bool isEnum)
		{
			if(isEnum) {
				this.SetEnumValue(value);
			} else {
				this.SetValue(value);
			}
		}
		public ColumnInitiate(byte value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(byte[] value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(DateTime value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(Color value)
		{
			this.SetValue(value);
		}
		public ColumnInitiate(InitiateValue style)
		{
			this.Style = style;
		}
		public ColumnInitiate(InitiateValue style, int days)
		{
			this.Style = style;
			this.Days = days;
		}
		public ColumnInitiate(object value)
		{
			if(null == value) { return; }
			if(value.GetType().IsEnum) {
				this.SetEnumValue((int)value);
			} else {
				this.SetValue(value.ToString());
			}
		}
		#endregion

		#region ICloneable<T>
		public ColumnInitiate Clone()
		{
			ColumnInitiate clone = new ColumnInitiate(InitiateValue.None);
			clone.Style = Style;
			clone.Format = Format;
			clone.StringValue = StringValue;
			clone.IntegerValue = IntegerValue;
			clone.DecimalValue = DecimalValue;
			clone.LongValue = LongValue;
			clone.ShortValue = ShortValue;
			clone.DoubleValue = DoubleValue;
			clone.FloatValue = FloatValue;
			clone.BooleanValue = BooleanValue;
			clone.CharValue = CharValue;
			clone.EnumValue = EnumValue;
			clone.ByteValue = ByteValue;
			clone.ByteArrayValue = ByteArrayValue;
			clone.DateTimeValue = DateTimeValue;
			clone.ColorValue = ColorValue;
			return clone;
		}
		#endregion

		#region property
		public InitiateValue Style { get; internal set; }
		public ColumnDataFormat Format { get; internal set; }

		internal string StringValue { get; private set; }
		internal int IntegerValue { get; private set; }
		internal decimal DecimalValue { get; private set; }
		internal long LongValue { get; private set; }
		internal short ShortValue { get; private set; }
		internal double DoubleValue { get; private set; }
		internal float FloatValue { get; private set; }
		internal bool BooleanValue { get; private set; }
		internal char CharValue { get; private set; }
		internal int EnumValue { get; private set; }
		internal byte ByteValue { get; private set; }
		internal byte[] ByteArrayValue { get; private set; }
		internal DateTime DateTimeValue { get; private set; }
		internal Color ColorValue { get; private set; }
		internal Guid GuidValue { get; private set; }
		internal int Days { get; private set; }

		// Parent
		public Column Column { get; internal set; }
		#endregion

		#region DefaultValue
		public object Value
		{
			get 
			{
				switch(Format) {
					case ColumnDataFormat.String:
						return StringValue;
					case ColumnDataFormat.Integer:
						return IntegerValue;
					case ColumnDataFormat.Decimal:
						return DecimalValue;
					case ColumnDataFormat.Long:
						return LongValue;
					case ColumnDataFormat.Short:
						return ShortValue;
					case ColumnDataFormat.Double:
						return DoubleValue;
					case ColumnDataFormat.Float:
						return FloatValue;
					case ColumnDataFormat.Boolean:
						return BooleanValue;
					case ColumnDataFormat.Char:
						return CharValue;
					case ColumnDataFormat.Enum:
						return EnumValue;
					case ColumnDataFormat.Byte:
						return ByteValue;
					case ColumnDataFormat.ByteArray:
						return ByteArrayValue;
					case ColumnDataFormat.DateTime:
						return DateTimeValue;
					case ColumnDataFormat.Color:
						return ColorValue;
					case ColumnDataFormat.Guid:
						return GuidValue;
					default:
						return null;
				}
			}
		}
		#endregion

		#region SetValue
		internal void SetValue(string value)
		{
			StringValue = value;
			Format = ColumnDataFormat.String;
		}
		internal void SetValue(int value)
		{
			IntegerValue = value;
			Format = ColumnDataFormat.Integer;
		}
		internal void SetValue(decimal value)
		{
			DecimalValue = value;
			Format = ColumnDataFormat.Decimal;
		}
		internal void SetValue(long value)
		{
			LongValue = value;
			Format = ColumnDataFormat.Long;
		}
		internal void SetValue(short value)
		{
			ShortValue = value;
			Format = ColumnDataFormat.Short;
		}
		internal void SetValue(double value)
		{
			DoubleValue = value;
			Format = ColumnDataFormat.Double;
		}
		internal void SetValue(float value)
		{
			FloatValue = value;
			Format = ColumnDataFormat.Float;
		}
		internal void SetValue(bool value)
		{
			BooleanValue = value;
			Format = ColumnDataFormat.Boolean;
		}
		internal void SetValue(char value)
		{
			CharValue = value;
			Format = ColumnDataFormat.Char;
		}
		internal void SetEnumValue(int value)
		{
			EnumValue = value;
			Format = ColumnDataFormat.Enum;
		}
		internal void SetValue(byte value)
		{
			ByteValue = value;
			Format = ColumnDataFormat.Byte;
		}
		internal void SetValue(byte[] value)
		{
			ByteArrayValue = value;
			Format = ColumnDataFormat.ByteArray;
		}
		internal void SetValue(DateTime value)
		{
			DateTimeValue = value;
			Format = ColumnDataFormat.DateTime;
		}
		internal void SetValue(Color value)
		{
			ColorValue = value;
			Format = ColumnDataFormat.Color;
		}
		#endregion
	}
}
