// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RenderBoolean.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-25 - Creation


using System;

namespace Kuick.Data
{
	public class RenderBoolean : IRender
	{
		public string ToString(object value)
		{
			return Get(value) 
				? Constants.StringBoolean.True 
				: Constants.StringBoolean.False;
		}

		public string ToHtml(object value)
		{
			return ToString(value);
		}

		private bool Get(object value)
		{
			return null == value ? false : Formator.AirBagToBoolean(value.ToString());
		}
	}
}
