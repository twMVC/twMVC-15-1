// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Grid.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-20 - Creation


using System;
using Kuick.Data;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;

namespace Kuick.Web.UI.Bootstrap
{
	public class Grid
	{
		#region constructor
		public Grid(Page page, string entityName)
			: this(page, entityName, null, null)
		{
		}
		public Grid(Page page, string entityName, GridSource source)
			: this(page, entityName, source, null)
		{
		}
		public Grid(Page page, string entityName, GridVisual visual)
			: this(page, entityName, null, visual)
		{
		}
		public Grid(
			Page page, 
			string entityName, 
			GridSource source, 
			GridVisual visual)
		{
			this.Page = page;
			this.Parameter = new WebParameter(page.Request);
			this.Schema = EntityCache.Get(entityName);
			this.Source = null == source ? new GridSource() : source;
			this.Visual = null == visual ? new GridVisual() : visual;
		}
		#endregion

		#region property
		public IEntity Schema { get; private set; }
		public Page Page { get; set; }
		public WebParameter Parameter { get; private set; }

		public GridSource Source { get; set; }
		public GridVisual Visual { get; set; }
		#endregion

		public string RenderTable()
		{
			StringBuilder sb = new StringBuilder();

			List<IEntity> instances = Entity.GetAll(Schema.EntityName);
			int index = 0;

			sb.Append("<table class=\"table table-hover\">");

			sb.Append("<thead>");
			sb.Append("<tr>");
			sb.Append("<th>#</th>");
			foreach(Column column in Schema.Columns) {
				if(column.Visual.HideInList) { continue; }
				sb.AppendFormat("<th>{0}</th>", column.Description.Description);
			}
			if(null != Visual.BuildTools) {
				sb.Append("<th>tools</th>");
			}
			sb.Append("</tr>");
			sb.Append("</thead>");

			sb.Append("<tbody>");
			foreach(IEntity instance in instances) {
				if(null == Visual.RowFilter) {
					sb.Append("<tr>");
				} else {
					if(Visual.RowFilter(instance) == RowStatus.Default) {
						sb.Append("<tr>");
					} else {
						sb.AppendFormat(
							"<tr class=\"{0}\">",
							Visual.RowFilter(instance).ToString().ToLower()
						);
					}
				}
				sb.AppendFormat("<td>{0}</td>", ++index);
				foreach(Column column in Schema.Columns) {
					if(column.Visual.HideInList) { continue; }

					Func<IEntity, string> filter =
						Visual.ValueFilters.ContainsKey(column.Property.Name)
							? Visual.ValueFilters[column.Property.Name]
							: Visual.ValueFilters.ContainsKey(column.Spec.ColumnName)
								? Visual.ValueFilters[column.Spec.ColumnName]
								: null;
					object val = null == filter 
						? instance.GetValue(column) 
						: filter(instance);
					sb.AppendFormat("<td>{0}</td>", val);

					//if(column.IsBoolean) {
					//    if((bool)val) {
					//        val = string.Format(
					//            "<img src=\"{0}\">",
					//            WebTools.ResolveUrl("~/img/icon/checked.png")
					//        );
					//    } else {
					//        val = string.Empty;
					//    }

					//    sb.AppendFormat(
					//        "<td>{0}</td>",
					//        val
					//    );
					//} else {
					//    sb.AppendFormat("<td>{0}</td>", instance.GetValue(column));
					//}
				}

				if(null != Visual.BuildTools) {
					sb.Append("<td>");
					sb.Append(Visual.BuildTools(instance));
					sb.Append("</td>");
				}
				sb.Append("</tr>");
			}
			sb.Append("</tbody>");

			sb.Append("</table>");

			return sb.ToString();
		}

		public string RenderJavaScript()
		{
			StringBuilder sb = new StringBuilder();
			return sb.ToString();
		}

		public void Deal()
		{
		}

		#region static
		#endregion
	}

	public class Grid<T> : Grid
		where T : class, IEntity, new()
	{
		#region constructor
		public Grid(Page page)
			: base(page, typeof(T).Name)
		{
		}
		#endregion

		#region property
		#endregion
	}
}
