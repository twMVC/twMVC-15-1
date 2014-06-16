// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RenderMoney.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-25 - Creation


using System;

namespace Kuick.Data
{
	public class RenderMoney : IRender
	{
		public string ToString(object value)
		{
			return Get(value).ToThousandPoint();
		}

		public string ToHtml(object value)
		{
			return ToString(value);
		}

		private int Get(object value)
		{
			return null == value ? 0 : Formator.AirBagToInt(value.ToString());
		}
	}
}
