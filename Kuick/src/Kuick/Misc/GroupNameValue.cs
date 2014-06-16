// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupNameValue.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-05-08 - Creation


using System;
using System.Drawing;

namespace Kuick
{
	public class GroupNameValue : GroupNameBase
	{
		public GroupNameValue()
		{
			this.Any = new Any();
		}

		private Any Any { get; set; }

		public object Value
		{
			get
			{
				return Any.ToString();
			}
			set
			{
				Any.Value = value;
			}
		}

		#region ToXxx
		public override string ToString()
		{
			return Any.ToString();
		}

		public bool ToBoolean()
		{
			return Any.ToBoolean();
		}

		public DateTime ToDateTime()
		{
			return Any.ToDateTime();
		}

		public byte ToByte()
		{
			return Any.ToByte();
		}

		public char ToChar()
		{
			return Any.ToChar();
		}

		public int ToInteger()
		{
			return Any.ToInteger();
		}

		public float ToFloat()
		{
			return Any.ToFloat();
		}

		public double ToDouble()
		{
			return Any.ToDouble();
		}

		public short ToShort()
		{
			return Any.ToShort();
		}

		public long ToLong()
		{
			return Any.ToLong();
		}

		public decimal ToDecimal()
		{
			return Any.ToDecimal();
		}

		public Color ToColor()
		{
			return Any.ToColor();
		}

		public Guid ToGuid()
		{
			return Any.ToGuid();
		}

		public byte[] ToByteArray()
		{
			return Any.ToByteArray();
		}

		public T ToEnum<T>()
		{
			return Any.ToEnum<T>();
		}

		public T ToEnum<T>(T airBag)
		{
			return Any.ToEnum<T>(airBag);
		}

		public string ToNameValueString()
		{
			return Any.ToNameValueString();
		}
		#endregion
	}
}
