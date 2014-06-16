// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Table.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-26 - Creation


using System;
using System.Collections.Generic;
using Kuick.Data;
using System.Text;

namespace Kuick.Web.UI
{
	public class Table<T>
		: AttributesBase
		where T : class ,IEntity, new()
	{
		public Table()
			: base()
		{
			this.TableCss = "xTable";
			this.THeadCss = string.Empty;
			this.TopRowCss = string.Empty;
			this.BottomRowCss = string.Empty;
			this.Instances = new List<T>();
			this.Tr = new Tr<T>();
			this.Tfs = new List<Tf<T>>();
		}

		public bool ShowTopRow { get; set; }
		public bool ShowBottomRow { get; set; }
		public string TableCss { get; set; }
		public string THeadCss { get; set; }
		public string TopRowCss { get; set; }
		public string BottomRowCss { get; set; }
		public List<T> Instances { get; set; }
		public Tr<T> Tr { get; set; }
		public List<Tf<T>> Tfs { get; set; }

		public virtual string ToHtml()
		{
			using(IntervalLogger il = new IntervalLogger("Table.ToHtml")) {
				StringBuilder sb = new StringBuilder();
				ToHtml(sb);
				return sb.ToString();
			}
		}

		internal void ToHtml(StringBuilder sb)
		{
			if(Checker.IsNull(Tfs)) { return; }

			// 00
			sb.AppendFormat("<table class=\"{0}\"", TableCss);
			AttributesToHtml(sb);
			sb.AppendFormat(" EntityName=\"{0}\"", typeof(T).Name);
			sb.Append(">");

			// thead
			sb.Append("<thead>");
			sb.AppendFormat("<tr class=\"{0}\">", THeadCss);
			foreach(Tf<T> tf in Tfs) {
				tf.Th.ToHtml(sb);
			}
			sb.Append("</tr>");
			sb.Append("</thead>");

			// add
			if(ShowTopRow) {
				sb.AppendFormat("<tr class=\"{0}\">", TopRowCss);
				foreach(Tf<T> tf in Tfs) {
					tf.TTop.ToHtml(sb, null, 0);
				}
				sb.Append("</tr>");
			}

			// tbody
			if(!Checker.IsNull(Instances)) {
				int index = 0;
				sb.Append("<tbody>");
				foreach(T one in Instances) {
					if(null != Tr.Skip && Tr.Skip(one, index + 1)) {
						continue;
					}
					++index;
					Tr.ToOpenHtml(sb, one, index);
					foreach(Tf<T> tf in Tfs) {
						tf.Td.ToHtml(sb, one, index);
					}
					Tr.ToCloseHtml(sb);
				}
				sb.Append("</tbody>");
			}

			// bottom
			if(ShowBottomRow) {
				sb.AppendFormat("<tr class=\"{0}\">", BottomRowCss);
				foreach(Tf<T> tf in Tfs) {
					tf.TBottom.ToHtml(sb, null, 0);
				}
				sb.Append("</tr>");
			}

			// 99
			sb.Append("</table>");
		}
	}
}
