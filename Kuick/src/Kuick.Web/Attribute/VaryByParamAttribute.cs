// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// VaryByParamAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Web
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class VaryByParamAttribute : Attribute, ICloneable<VaryByParamAttribute>
	{
		#region constructor
		public VaryByParamAttribute()
			: this(String.Empty)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keys">Request parameter keys</param>
		public VaryByParamAttribute(params string[] keys)
		{
			List<string> ls = new List<string>(keys);
			ls.Sort();
			ls.Remove(String.Empty);
			Keys = ls.ToArray();
		}
		#endregion

		#region ICloneable<T>
		public VaryByParamAttribute Clone()
		{
			return new VaryByParamAttribute(Keys);
		}
		#endregion

		#region property
		/// <summary>
		/// Cache duration , in second.
		/// </summary>
		public string[] Keys { get; private set; }
		#endregion

		public override string ToString()
		{
			return Keys.Length == 0 ? "Cache vary by none." :
				String.Concat("Cache vary by parameter(s): ", String.Join(", ", Keys));
		}
	}
}

