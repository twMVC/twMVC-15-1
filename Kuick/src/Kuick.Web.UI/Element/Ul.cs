// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Ul.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-20 - Creation


using System;
using System.Collections.Generic;
using System.Text;

namespace Kuick.Web.UI
{
	public class Ul : AttributesBase
	{
		public Ul()
			: base()
		{
			this.Lis = new List<Li>();
		}

		public string CssClass { get; set; }
		public string Text { get; set; }
		public A A { get; set; }
		public List<Li> Lis { get; set; }

		public string ToHtml()
		{
			StringBuilder sb = new StringBuilder();
			ToHtml(sb);
			return sb.ToString();
		}

		internal void ToHtml(StringBuilder sb)
		{
			sb.AppendFormat(
				"<ul{0}",
				string.IsNullOrEmpty(CssClass) ? "" : string.Concat(" class=\"", CssClass, "\"")
			);
			AttributesToHtml(sb);
			sb.Append(">");

			if(!string.IsNullOrEmpty(Text)) { sb.Append(Text); }
			if(null != A) { A.ToHtml(sb); }

			if((!string.IsNullOrEmpty(Text) || null != A) && !Checker.IsNull(Lis)) {
				sb.Append(Show.Leader);
			}

			if(!Checker.IsNull(Lis)) {
				foreach(Li li in Lis) {
					li.ToHtml(sb);
				}
			}

			sb.Append("</ul>");
		}
	}
}
