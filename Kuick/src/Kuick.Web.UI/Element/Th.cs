// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Th.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-26 - Creation


using System;
using Kuick.Data;
using System.Text;
using System.Linq.Expressions;

namespace Kuick.Web.UI
{
	public class Th<T>
		: AttributesBase 
		where T : class ,IEntity, new()
	{
		public Th()
			: this(null)
		{
		}
		public Th(Expression<Func<T, object>> expression)
			: base()
		{
			if(null == expression) { return; }
			Column column = DataUtility.ToColumn<T>(expression);
			if(null == column) { return; }
			TitleValue = column.Title;
		}

		public string TitleValue { get; set; }
		public string CssClass { get; set; }

		internal void ToHtml(StringBuilder sb)
		{
			sb.Append("<th");
			if(!string.IsNullOrEmpty(CssClass)) {
				sb.AppendFormat(" class=\"{0}\"", CssClass);
			}
			foreach(Any any in Attributes) {
				sb.AppendFormat(" {0}=\"{1}\"", any.Name, any.ToString());
			}
			AttributesToHtml(sb);
			sb.AppendFormat(">{0}</th>", TitleValue);
		}
	}
}
