// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RenderString.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-25 - Creation


using System;

namespace Kuick.Data
{
	public class RenderString : IRender
	{
		public string ToString(object value)
		{
			return Get(value);
		}

		public string ToHtml(object value)
		{
			string str = Get(value);
			return str.IsNullOrEmpty() ? Constants.Html.Entity.nbsp : str;
		}

		private string Get(object value)
		{
			return null == value ? string.Empty : value.ToString();
		}
	}
}
