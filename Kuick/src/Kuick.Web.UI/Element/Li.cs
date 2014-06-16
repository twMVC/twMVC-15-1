// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Li.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-20 - Creation


using System;
using System.Text;

namespace Kuick.Web.UI
{
	public class Li : AttributesBase
	{
		public Li()
			: base()
		{
		}

		public string CssClass { get; set; }
		public string Text { get; set; }
		public A A { get; set; }
		public Ul Ul { get; set; }

		public string ToHtml()
		{
			StringBuilder sb = new StringBuilder();
			ToHtml(sb);
			return sb.ToString();
		}

		internal void ToHtml(StringBuilder sb)
		{
			sb.Append("<li");
			if(!string.IsNullOrEmpty(CssClass)) { 
				sb.Append(string.Concat(" class=\"", CssClass, "\""));
			}
			AttributesToHtml(sb);
			sb.Append(">");

			if(!string.IsNullOrEmpty(Text)) { sb.Append(Text); }
			if(null != A) { A.ToHtml(sb); }
			if(null != Ul) { Ul.ToHtml(sb); }

			sb.Append("</li>");
		}
	}
}
