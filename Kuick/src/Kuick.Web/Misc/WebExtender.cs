// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebExtender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kuick.Data;
using System.Text;

namespace Kuick.Web
{
	public static class WebExtender
	{
		public static void Data(this DropDownList ddl, object data)
		{
			if(WebChecker.IsNull(data)) { return; }

			ListItem item = ddl.Items.FindByValue(data.ToString());
			if(item != null && !item.Selected) {
				ListItem selectedItem = ddl.SelectedItem;
				if(null != selectedItem) { selectedItem.Selected = false; }
				item.Selected = true;
			}
		}

		public static void Bind(this Page page, object obj)
		{
			WebTools.Bind(page, obj);
		}

		public static string ToQueryString(this Any[] anys)
		{
			if(null == anys) { anys = new Any[0]; }

			StringBuilder sb = new StringBuilder();
			foreach(Any any in anys) {
				sb.Append(WebConstants.Html.Entity.amp)
				.AppendFormat(
					"{0}={1}",
					any.Name,
					WebChecker.IsNull(any) 
						? string.Empty 
						: Formator.UrlEncode(any.ToString())
				);
			}

			return sb.Length > 0 ? sb.Remove(0, 1).ToString() : string.Empty;
		}

		public static string ToHtml(this string value)
		{
			return string.IsNullOrEmpty(value) 
				? string.Empty 
				: value.HtmlDecode();
		}

		public static string NewLineToBr(this string value)
		{
			return string.IsNullOrEmpty(value)
				? string.Empty
				: value.Replace("\n", "<br>");
		}

		public static string NewLineToOrderList(this string value)
		{
			return NewLineToList(value, "ol");
		}

		public static string NewLineToUnorderList(this string value)
		{
			return NewLineToList(value, "ul");
		}

		private static string NewLineToList(string value, string tag)
		{
			if(string.IsNullOrEmpty(value)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			string[] parts = value.SplitWith(Environment.NewLine);
			sb.AppendFormat("<{0}>", tag);
			foreach(string part in parts) {
				sb.AppendFormat("<li>{0}</li>", part);
			}
			sb.AppendFormat("</{0}>", tag);
			return sb.ToString();
		}

		//public static string GetHtmlValue(this IEntity one, ColumnSpec column)
		//{
		//    string val = one.FormatValue(
		//            column.PropertyName,
		//            one.GetValue(column.ColumnName).ToString().HtmlEncode()
		//        );

		//    if(!Checker.IsNull(column.Logic.Reference)) {
		//        if(!Checker.IsFlagEnum(column.Logic.Reference)) {
		//            if(column.Logic.Reference.IsDerived<IEntity>()) {
		//                TypeApi api = new TypeApi(column.Logic.Reference);
		//                IEntity refInstance = api.Get(val);
		//                if(!Checker.IsNull(refInstance)) { val = refInstance.TitleValue; }
		//            } else {
		//                IReference reference = Reference.GetFromCache(column.Logic.Reference);
		//                if(!Checker.IsNull(reference)) { val = reference.GetText(val); }
		//            }
		//        }
		//    }

		//    if(Checker.IsNull(val)) { return "&nbsp;"; }

		//    if(column.Config.InputType == ColumnInput.FileUpload) {
		//        val = WebUtility.IsImage(val)
		//            ? String.Format(
		//                "<img src=\"{0}\" rel=\"lytebox\">",
		//                WebUtility.ResolveUrl(one, column.ColumnName)
		//            )
		//            : String.Format("File ({0})", val);
		//    }

		//    return val;
		//}
	}
}
