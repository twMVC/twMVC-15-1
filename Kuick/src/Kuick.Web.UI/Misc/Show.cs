// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Show.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-12-13 - Creation


using System;
using System.Collections.Generic;
using System.Text;
using Kuick.Data;
using System.Drawing;

namespace Kuick.Web.UI
{
	public class Show
	{
		public static string Leader { get; set; }
		public static string Separater { get; set; }

		#region Table
		public static string Table<T>(
			List<T> instances,
			params Any[] columns) where T : class ,IEntity, new()
		{
			if(Checker.IsNull(columns)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();

			// 00
			sb.Append("<table class=\"showTable\">");

			// thead
			sb.Append("<thead>");
			sb.Append("<tr>");
			foreach(Any any in columns) {
				sb.AppendFormat("<th scope=\"col\">{0}</th>", any.Value);
			}
			sb.Append("</tr>");
			sb.Append("</thead>");

			//// tfoot
			//sb.Append("<tfoot>");
			//sb.Append("<tr>");
			//sb.AppendFormat("<td colspan=\"{0}\">{1}</td>", columns.Length, "paginator");
			//sb.Append("</tr>");
			//sb.Append("</tfoot>");

			// tbody
			if(!Checker.IsNull(instances)) {
				sb.Append("<tbody>");
				foreach(T instance in instances) {
					sb.Append("<tr>");
					foreach(Any any in columns) {
						sb.AppendFormat("<td>{0}</td>", instance.GetValue(any.Name));
					}
					sb.Append("</tr>");
				}
				sb.Append("</tbody>");
			}

			// 99
			sb.Append("</table>");

			return sb.ToString();
		}

		public static string Table<T>(
			List<T> instances,
			params All<T>[] columns) where T : class ,IEntity, new()
		{
			if(Checker.IsNull(columns)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();

			// 00
			sb.Append("<table class=\"showTable\">");

			// thead
			sb.Append("<thead>");
			sb.Append("<tr>");
			foreach(All<T> all in columns) {
				sb.AppendFormat(
					"<th scope=\"col\">{0}</th>",
					null == all.Value ? all.Name : all.Value
				);
			}
			sb.Append("</tr>");
			sb.Append("</thead>");

			//// tfoot
			//sb.Append("<tfoot>");
			//sb.Append("<tr>");
			//sb.AppendFormat("<td colspan=\"{0}\">{1}</td>", columns.Length, "paginator");
			//sb.Append("</tr>");
			//sb.Append("</tfoot>");

			// tbody
			if(!Checker.IsNull(instances)) {
				sb.Append("<tbody>");
				foreach(T instance in instances) {
					sb.Append("<tr>");
					foreach(All<T> all in columns) {

						sb.Append("<td");
						foreach(All<T> attr in all.Alls) {
							sb.AppendFormat(
								" {0}=\"{1}\"",
								null == attr.Name ? attr.Value : attr.Name,
								null == attr.Delegate ? attr.Value : attr.Delegate(instance)
							);
						}
						sb.Append(">");

						if(null == all.Delegate) {
							sb.Append(instance.GetValue(all.Name));
						} else {
							sb.Append(all.Delegate(instance));
						}

						sb.Append("</td>");
					}
					sb.Append("</tr>");
				}
				sb.Append("</tbody>");
			}

			// 99
			sb.Append("</table>");

			return sb.ToString();
		}
		#endregion

		#region Paginator
		public static string Paginator(Paginator p)
		{
			return p.Count.ToString();
		}
		#endregion

		#region Box
		public static string Information(string message)
		{
			return string.Format("<div class=\"ShowInformation\">{0}</div>", message);
		}

		public static string Success(string message)
		{
			return string.Format("<div class=\"ShowSuccess\">{0}</div>", message);
		}

		public static string Warning(string message)
		{
			return string.Format("<div class=\"ShowWarning\">{0}</div>", message);
		}

		public static string Error(string message)
		{
			return string.Format("<div class=\"ShowError\">{0}</div>", message);
		}
		#endregion

		#region BuildIcon
		public static string BuildIcon(EditMode mode)
		{
			return BuildIcon(IconSize._16x16, mode);
		}

		public static string BuildIcon(IconSize size, EditMode mode)
		{
			return BuildIcon(
				size,
				mode.ToString(),
				EnumCache.Get<EditMode>().GetTitle(mode)
			);
		}

		public static string BuildIcon(IconSize size, string name, string title)
		{
			int px = size == IconSize._16x16
				? 16
				: size == IconSize._24x24
					? 24
					: 64;

			return string.Format(
				"<img class=\"icon\" src=\"{0}\" title=\"{1}\" width=\"{2}\" height=\"{2}\" />",
				WebTools.ResolveUrl(string.Format("~/img/icon/{0}x{0}/{1}.png", px, name)),
				title,
				px
			);
		}
		#endregion

		#region BuildIconSrc
		public static string BuildIconSrc(EditMode mode)
		{
			return BuildIconSrc(IconSize._16x16, mode.ToString());
		}

		public static string BuildIconSrc(IconSize size, string name)
		{
			int px = size == IconSize._16x16
				? 16
				: size == IconSize._24x24
					? 24
					: 64;

			return WebTools.ResolveUrl(string.Format(
				"~/img/icon/{0}x{0}/{1}.png", px, name
			));
		}
		#endregion

		#region Edit
		public static string EditColor(IEntity instance, string columnName)
		{
			return EditColor(instance, columnName, string.Empty);
		}
		public static string EditColor(IEntity instance, string columnName, string cssClass)
		{
			Column column = instance.GetColumn(columnName);
			if(null == column) { return string.Empty; }

			string htmlColor = string.Empty;
			try {
				object val = instance.GetValue(column);
				htmlColor = ColorTranslator.ToHtml(
					Color.FromArgb(
						null == val ? 0 : val.ToString().AirBagToInt()
					)
				);
			} catch {
			}

			return string.Format(@"
<input type=""color"" class=""xEdit{0}"" entityName=""{1}"" keyValue=""{2}"" columnName=""{3}"" value=""{4}"" /><span class=""xEditOriginalValue"">{4}</span>",
				string.IsNullOrEmpty(cssClass) ? "" : " " + cssClass,
				instance.EntityName,
				instance.KeyValue,
				column.Spec.ColumnName,
				htmlColor
			);
		}
		public static string EditText(IEntity instance, string columnName)
		{
			return EditText(instance, columnName, string.Empty);
		}
		public static string EditText(IEntity instance, string columnName, string cssClass)
		{
			Column column = instance.GetColumn(columnName);
			if(null == column) { return string.Empty; }

			object value = instance.GetValue(column);

			string val = null;
			if(null == value){
				val = string.Empty;
			} else{
				if(column.Visual.Input == VisualInput.Date) {
					DateTime d = Formator.AirBagToDateTime(value.ToString());
						val = d.yyyyMMdd();
				}else{
					val = value.ToString().HtmlEncode();
				}
			}

			return string.Format(
@"<input type=""text"" class=""xEdit{0}"" entityName=""{1}"" keyValue=""{2}"" columnName=""{3}"" value=""{4}"" /><span class=""xEditOriginalValue"">{4}</span>",
				string.IsNullOrEmpty(cssClass) ? "" : " " + cssClass,
				instance.EntityName,
				instance.KeyValue,
				column.Spec.ColumnName,
				val
			);
		}
		public static string EditTextArea(IEntity instance, string columnName)
		{
			return EditTextArea(instance, columnName, string.Empty);
		}
		public static string EditTextArea(IEntity instance, string columnName, string cssClass)
		{
			Column column = instance.GetColumn(columnName);
			if(null == column) { return string.Empty; }

			object value = instance.GetValue(column);
			string val = null == value ? "" : value.ToString().HtmlEncode();


			return string.Format(@"
<textarea type=""text"" class=""xEdit{0}"" entityName=""{1}"" keyValue=""{2}"" columnName=""{3}"" rows=""4"">{4}</textarea><span class=""xEditOriginalValue"">{4}</span>",
				string.IsNullOrEmpty(cssClass) ? "" : " " + cssClass,
				instance.EntityName,
				instance.KeyValue,
				column.Spec.ColumnName,
				val
			);
		}
		public static string EditSelect(IEntity instance, string columnName)
		{
			return EditSelect(instance, columnName, string.Empty);
		}
		public static string EditSelect(IEntity instance, string columnName, string cssClass)
		{
			Column column = instance.GetColumn(columnName);
			if(null == column) { return string.Empty; }

			object value = instance.GetValue(column);
			string val = null == value ? "" : value.ToString().HtmlEncode();

			EnumReference ef = EnumCache.Get(column.Property.PropertyType);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(
@"<select class=""xSelect{0}"" entityName=""{1}"" keyValue=""{2}"" columnName=""{3}"">",
				string.IsNullOrEmpty(cssClass) ? "" : " " + cssClass,
				instance.EntityName,
				instance.KeyValue,
				column.Spec.ColumnName
			);
			foreach(EnumItem ei in ef.Items) {
				sb.AppendFormat(
					"<option value=\"{0}\"{1}>{2}</option>",
					ei.Value,
					ei.Value == val ? " selected" : string.Empty,
					ei.Title
				);
			}
			sb.Append("</select>");

			return sb.ToString();
		}
		public static string EditCheckbox(IEntity instance, string columnName)
		{
			return EditCheckbox(instance, columnName, string.Empty);
		}
		public static string EditCheckbox(IEntity instance, string columnName, string cssClass)
		{
			Column column = instance.GetColumn(columnName);
			if(null == column) { return string.Empty; }

			object value = instance.GetValue(column);

			bool val = false;
			if(null == value) {
				val = false;
			} else {
				val = value.ToString().AirBagToBoolean();
			}

			return string.Format(
@"<input type=""checkbox"" class=""xCheckbox{0}"" entityName=""{1}"" keyValue=""{2}"" columnName=""{3}"" value=""{4}"" id=""{1}-{3}"" {5} /><span class=""xEditOriginalValue"">{4}</span>",
				string.IsNullOrEmpty(cssClass) ? "" : " " + cssClass,
				instance.EntityName,
				instance.KeyValue,
				column.Spec.ColumnName,
				val,
				val ? "checked" : ""
			);
		}
		#endregion

		#region Tag
		//public 
		#endregion

		#region private
		private static void Initialize()
		{
			if(string.IsNullOrEmpty(Leader)) { Leader = ": "; }
			if(string.IsNullOrEmpty(Separater)) { Separater = " >> "; }
		}
		#endregion
	}
}
