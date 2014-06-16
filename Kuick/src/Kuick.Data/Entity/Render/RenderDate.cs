// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RenderDate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-25 - Creation


using System;

namespace Kuick.Data
{
	public class RenderDate : IRender
	{
		public string ToString(object value)
		{
			DateTime dt = Get(value);
			if(default(DateTime) == dt) { return string.Empty; }
			return dt.yyyyMMdd();
		}

		public string ToHtml(object value)
		{
			DateTime dt = Get(value);
			if(default(DateTime) == dt) { return Constants.Html.Entity.nbsp; }
			return dt.yyyyMMdd();
		}

		private DateTime Get(object value)
		{
			if(null == value) { return default(DateTime); }

			DateTime dt = default(DateTime);
			if(value.GetType().IsDateTime()) {
				dt = Convert.ToDateTime(value);
			} else {
				dt = Formator.AirBagToDateTime(value.ToString());
			}
			return dt;
		}
	}
}
