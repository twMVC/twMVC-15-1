// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnEncrypt.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnEncrypt : Attribute, ICloneable<ColumnEncrypt>
	{
		#region constructor
		public ColumnEncrypt(Encryption method)
			: this(method, string.Empty)
		{
		}

		public ColumnEncrypt(Encryption method, string key)
		{
			this.Method = method;
			this.Key = Formator.AirBag(key, DataCurrent.EncryptKey);
		}
		#endregion

		#region ICloneable<T>
		public ColumnEncrypt Clone()
		{
			return new ColumnEncrypt(Method, Key);
		}
		#endregion

		#region property
		public Encryption Method { get; internal set; }
		public string Key { get; internal set; }

		// Parent
		public Column Column { get; internal set; }
		#endregion
	}
}
