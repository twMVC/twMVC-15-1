// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Tr.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-26 - Creation


using System;
using Kuick.Data;
using System.Text;

namespace Kuick.Web.UI
{
	public class Tr<T>
		: AttributesBase 
		where T : class ,IEntity, new()
	{
		public Tr()
			: base()
		{
		}

		public string CssClass { get; set; }
		public Func<T, int, string> GetClass { get; set; }
		public Func<T, int, bool> Skip { get; set; }

		internal void ToOpenHtml(StringBuilder sb, T one, int index)
		{
			sb.Append("<tr");
			if(null != GetClass) {
				string css = GetClass(one, index);
				if(!string.IsNullOrEmpty(css)) {
					sb.AppendFormat(
						" class=\"{0}{1}{2}\"",
						string.IsNullOrEmpty(CssClass) ? "" : CssClass,
						string.IsNullOrEmpty(CssClass) ? "" : " ",
						css
					);
				}
			}
			sb.AppendFormat(" KeyValue=\"{0}\"", one.KeyValue);
			AttributesToHtml(sb);
			sb.Append(">");
		}

		internal void ToCloseHtml(StringBuilder sb)
		{
			sb.Append("</tr>");
		}
	}
}
