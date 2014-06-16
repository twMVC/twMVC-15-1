// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// A.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-20 - Creation


using System;
using System.Text;

namespace Kuick.Web.UI
{
	public class A : AttributesBase
	{
		public A()
			: base()
		{
		}

		public string Url { get; set; }
		public string CssClass { get; set; }
		public string Text { get; set; }

		public string ToHtml()
		{
			StringBuilder sb = new StringBuilder();
			ToHtml(sb);
			return sb.ToString();
		}

		internal void ToHtml(StringBuilder sb)
		{
			if(string.IsNullOrEmpty(Text)) {
				throw new ArgumentException("Ka.Text property can not be empty.");
			}

			sb.AppendFormat(
				"<a href=\"{0}\"{1}",
				string.IsNullOrEmpty(Url) ? "" : WebTools.ResolveUrl(Url),
				string.IsNullOrEmpty(CssClass) ? "" : string.Concat(" class=\"", CssClass, "\"")
			);
			AttributesToHtml(sb);
			sb.Append(">");
			sb.Append(Text);
			sb.Append("</a>");
		}
	}
}
