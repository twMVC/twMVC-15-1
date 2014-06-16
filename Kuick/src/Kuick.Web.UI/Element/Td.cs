// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Table.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-26 - Creation


using System;
using Kuick.Data;
using System.Text;
using System.Linq.Expressions;

namespace Kuick.Web.UI
{
	public class Td<T>
		: AttributesBase<T>
		where T : class ,IEntity, new()
	{
		public Td()
			: this(null)
		{
		}
		public Td(Expression<Func<T, object>> expression)
			: base()
		{
			if(null == expression) { return; }
			Column column = DataUtility.ToColumn<T>(expression);
			if(null == column) { return; }
			ColumnName = column.Spec.ColumnName;
		}

		public string ColumnName { get; set; }
		public string CssClass { get; set; }
		public Func<T, int, string> GetValue { get; set; }
		public Func<T, int, string> GetClass { get; set; }

		internal void ToHtml(StringBuilder sb, T one, int index)
		{
			try {
				sb.Append("<td");

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
				AttributesToHtml(sb, one, index);
				if(!ColumnName.IsNullOrEmpty()) {
					sb.AppendFormat(" ColumnName=\"{0}\"", ColumnName);
				}
				sb.Append(">");

				if(null == GetValue) {
					if(ColumnName.IsNullOrEmpty()) {
						sb.Append(Constants.Html.Escape.nbsp);
					} else {
						Column column = one.GetColumn(ColumnName);
						if(null == column) {
							sb.Append(Constants.Html.Escape.nbsp);
						} else {
							object o = one.GetValue(ColumnName);
							string v = null == o ? string.Empty : o.ToString();

							if(
								null != column.Refer.Type
								&&
								column.Spec.DbType != SqlDataType.Enum
								&&
								column.Spec.DbType != SqlDataType.Bit) {
								IEntity[] schemas = EntityCache
									.Get(column.Refer.Type);
								if(!schemas.IsNullOrEmpty()) {
									IEntity schema = schemas[0];
									IEntity instance = Entity
										.Get(schema.EntityName, v);
									v = null == instance
										? Constants.Html.Escape.nbsp
										: instance.TitleValue;
								}
							
							}

							sb.Append(v);
						}
					}
				} else {
					try {
						sb.Append(GetValue(one, index));
					} catch(Exception e) {
						Logger.Error("Kuick.Web.UI.Td.ToHtml()", e);
						sb.Append("error");
					}
				}

				sb.Append("</td>");
			} catch(Exception ex) {
				Logger.Error("Kuick.Web.UI.Td.ToHtml()", ex);
				throw;
			}
		}
	}
}
