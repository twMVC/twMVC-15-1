// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Editor.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-03 - Creation


using System;
using Kuick.Data;
using System.Text;
using System.Web.UI;

namespace Kuick.Web.UI.Bootstrap
{
	public class Editor
	{
		#region constructor
		public Editor(Page page, string entityName)
			: this(page, entityName, null, null)
		{
			this.InAdd = true;
		}

		public Editor(Page page, IEntity instance)
			: this(page, instance.EntityName, null, null)
		{
			this.InAdd = false;
			this.Instance = instance;
		}


		public Editor(
			Page page, 
			string entityName, 
			EditorSource source, 
			EditorVisual visual)
		{
			this.Page = page;
			this.Parameter = new WebParameter(page.Request);
			this.Schema = EntityCache.Get(entityName);
			this.Dealing = Parameter.RequestBoolean("Kuick_Deal");

			this.Source = null == source ? new EditorSource() : source;
			this.Visual = null == visual ? new EditorVisual() : visual;
		}
		#endregion

		#region property
		public IEntity Schema { get; private set; }
		public IEntity Instance { get; private set; }
		public bool InAdd { get; private set; }
		public bool Dealing { get; set; }
		public Page Page { get; set; }
		public WebParameter Parameter { get; private set; }

		public EditorSource Source { get; set; }
		public EditorVisual Visual { get; set; }
		#endregion

		public string RenderFluidForm()
		{
			//int columnMax = 2;
			//int columnIndex = 0;

			StringBuilder sb = new StringBuilder();
			sb.Append("<form class=\"form-inline span12\">");


			// Columns
			foreach(Column column in Schema.Columns) {
				RenderColumn(sb, column);
			}

			// Button
			sb.Append("<div class=\"control-group\">");
			sb.Append(
				"<label class=\"control-label\" >&nbsp;</label>"
			);
			sb.Append("<div class=\"controls\">");
			sb.Append(
				"<button type=\"submit\" class=\"btn btn-primary btn-large button-loading\" data-loading-text=\"Loading...\"/ name=\"Kuick_Deal\" value=\"true\"> <i class=\"icon-ok icon-white\"></i>&nbsp;&nbsp;OK </button>"
			);
			sb.Append("</div>");
			sb.Append("</div>");


			sb.Append("</form>");
			return sb.ToString();
		}


		public string RenderForm()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<form class=\"form-horizontal\">");

			// Key
			RenderKeyColumn(sb, Schema.KeyColumn);

			foreach(Column column in Schema.Columns) {
				if(column.Spec.PrimaryKey) { continue; }
				RenderColumn(sb, column);
			}

			// button
			sb.Append("<div class=\"control-group\">");
			sb.Append(
				"<label class=\"control-label\" >&nbsp;</label>"
			);
			sb.Append("<div class=\"controls\">");
			sb.Append(
				"<button type=\"submit\" class=\"btn btn-primary btn-large button-loading\" data-loading-text=\"Loading...\"/ name=\"Kuick_Deal\" value=\"true\"> <i class=\"icon-ok icon-white\"></i>&nbsp;&nbsp;OK </button>"
			);
			sb.Append("</div>");
			sb.Append("</div>");


			sb.Append("</form>");
			return sb.ToString();
		}

		public string RenderJavaScript()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine("<script>");
			sb.AppendLine("$(function(){");

			foreach(Column column in Schema.Columns) {
				switch(column.Visual.Input) {
					case VisualInput.TextBox:
						break;
					case VisualInput.TextArea:
						break;
					case VisualInput.Password:
						break;
					case VisualInput.Date:
						sb.AppendFormat("$('#{0}').datepicker();", column.AliasName);
						sb.AppendLine();
						break;
					case VisualInput.Time:
						sb.AppendFormat("$('#{0}').timepicker();", column.AliasName);
						sb.AppendLine();
						break;
					case VisualInput.TimeStamp:
						break;
					case VisualInput.FileUpload:
						break;
					case VisualInput.DropDownList:
						break;
					case VisualInput.CheckBox:
						break;
					case VisualInput.CheckBoxList:
						break;
					case VisualInput.HtmlEditor:
						sb.AppendFormat("$('#{0}').wysihtml5();", column.AliasName);
						sb.AppendLine();
						break;
					case VisualInput.Color:
						break;
					default:
						break;
				}
			}

			sb.AppendLine("});");
			sb.AppendLine("</script>");
			return sb.ToString();
		}

		public void Deal()
		{
			if(!Dealing) { return; }

			if(InAdd) {
				IEntity one = Reflector.CreateInstance(Schema.GetType()) as IEntity;
				foreach(Column column in one.Columns) {
					if(column.Visual.SystemColumn) { continue; }
					if(InAdd && column.Visual.HideInAdd) { continue; }
					one.SetValue(column, Parameter.RequestValue(column.AliasName));
				}
				one.Add();
				Page.Response.Redirect(WebTools.BuildQueryString());
			} else {
				foreach(Column column in Instance.Columns) {
					if(column.Visual.SystemColumn) { continue; }
					if(column.Visual.HideInModify) { continue; }
					Instance.SetValue(column, Parameter.RequestValue(column.AliasName));
				}
				Instance.Modify();
			}
		}

		#region Render
		public void RenderKeyColumn(StringBuilder sb, Column column)
		{
			if(InAdd) {
				if(column.Visual.SystemColumn || column.Visual.HideInAdd) {
					return;
				}
			} else {
				if(column.Visual.SystemColumn || column.Visual.HideInModify) {
					sb.AppendFormat(
						"<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\"/>",
						BootstrapConstants.KeyValue,
						Instance.KeyValue
					);
				} else {
					RenderColumn(sb, column);
				}
			}
		}

		public void RenderColumn(StringBuilder sb, Column column)
		{
			if(column.Visual.SystemColumn) { return; }
			if(InAdd && column.Visual.HideInAdd) { return; }
			if(!InAdd && column.Visual.HideInModify) { return; }
			object val = InAdd
				? Schema.GetValue(column)
				: Instance.GetValue(column);
			bool isNull = InAdd
				? !column.Spec.NotAllowNull
				: Instance.IsNullCheck(column.Spec.ColumnName);
			if(null == val) { val = string.Empty; }

			sb.Append("<div class=\"control-group\">");
			sb.AppendFormat(
				"<label class=\"control-label\" for=\"{0}\">{1}</label>",
				column.AliasName,
				column.Description.Description
			);

			switch(column.Visual.Input) {
				case VisualInput.Date:
					RenderDateColumn(sb, column, isNull, val);
					break;
				case VisualInput.Time:
					RenderTimeColumn(sb, column, isNull, val);
					break;
				case VisualInput.HtmlEditor:
					RenderHtmlEditorColumn(sb, column, isNull, val);
					break;
				case VisualInput.CheckBox:
					RenderCheckBoxColumn(sb, column, isNull, val);
					break;
				case VisualInput.RadioButtons:
					RenderRadioButtonsColumn(sb, column, isNull, val);
					break;
				case VisualInput.TextBox:
					RenderTextBoxColumn(sb, column, isNull, val);
					break;
				case VisualInput.TextArea:
					RenderTextAreaColumn(sb, column, isNull, val);
					break;
				case VisualInput.Password:
					RenderPasswordColumn(sb, column, isNull, val);
					break;
				case VisualInput.TimeStamp:
					RenderTimeStampColumn(sb, column, isNull, val);
					break;
				case VisualInput.FileUpload:
					RenderFileUploadColumn(sb, column, isNull, val);
					break;
				case VisualInput.DropDownList:
					RenderDropDownListColumn(sb, column, isNull, val);
					break;
				case VisualInput.CheckBoxList:
					RenderCheckBoxListColumn(sb, column, isNull, val);
					break;
				case VisualInput.Color:
					RenderColorColumn(sb, column, isNull, val);
					break;
				default:
					RenderDefaultColumn(sb, column, isNull, val);
					break;
			}

			sb.Append("</div>");
		}

		public void RenderDateColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.Append("<div class=\"controls\">");
			sb.AppendFormat(
				"<div class=\"input-append date\">",
				column.AliasName
			);
			sb.AppendFormat(
				"<input class=\"input-small\" type=\"text\" id=\"{0}\" name=\"{0}\" value=\"{1}\" data-date-format=\"yyyy-mm-dd\"/><span class=\"add-on\"><i class=\"icon-calendar\"></i></span>",
				column.AliasName,
				isNull ? string.Empty : val.ToString().HtmlEncode()
			);
			sb.Append("</div>");
			sb.Append("</div>");
		}


		private void RenderDefaultColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.Append("<div class=\"controls\">");
			sb.AppendFormat(
				"<input type=\"text\" class=\"{0}\" id=\"{1}\" name=\"{1}\" value=\"{2}\"/>",
				BootstrapHelper.VisualSizeToCss(column.Visual.Size),
				column.AliasName,
				isNull ? string.Empty : val.ToString()
			);
			sb.Append("</div>");
		}


		private void RenderHiddenColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.AppendFormat(
				"<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\"/>",
				column.AliasName,
				isNull ? string.Empty : val.ToString()
			);
		}

		private void RenderColorColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderCheckBoxListColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderDropDownListColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderFileUploadColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderTimeStampColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderPasswordColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderTextAreaColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderTextBoxColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			RenderDefaultColumn(sb, column, isNull, val);
		}

		private void RenderRadioButtonsColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.Append("<div class=\"controls\">");
			sb.Append("<div class=\"btn-group\" data-toggle=\"buttons-radio\">");

			EnumReference er = EnumCache.Get(column.Refer.Type);
			foreach(EnumItem item in er.Items) {
				sb.AppendFormat("<input type=\"radio\" id=\"{0}{1}\" name=\"{0}\" data-toggle=\"button\" value=\"{1}\" {2}/><label class=\"btn btn-primary{3}\" for=\"{0}{1}\">{4}</label>",
					column.AliasName,
					item.Value,
					val.ToString() == item.Value ? "checked" : "",
					val.ToString() == item.Value ? " active" : "",
					item.Title// item.Name
				);
			}

			sb.Append("</div>");
			sb.Append("</div>");
		}

		private void RenderCheckBoxColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.Append("<div class=\"controls\">");
			sb.AppendFormat(
				"<div class=\"switch\" data-on-label=\"{0}\" data-off-label=\"{1}\">",
				column.Visual.TrueText,
				column.Visual.FalseText
			);
			sb.AppendFormat(
				"<input type=\"checkbox\" id=\"{0}\" name=\"{0}\" {1}/>",
				column.AliasName,
				isNull
					? string.Empty
					: val.ToString().AirBagToBoolean()
						? "checked"
						: ""
			);
			sb.Append("</div>");
			sb.Append("</div>");
		}

		private void RenderHtmlEditorColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.Append("<div class=\"controls\">");
			sb.AppendFormat(
				"<textarea type=\"text\" style=\"width :570px;\" id=\"{0}\" name=\"{0}\" placeholder=\"HTML\"/>{1}</textarea>",
				column.AliasName,
				isNull ? string.Empty : val.ToString()
			);
			sb.Append("</div>");
		}

		private void RenderTimeColumn(
			StringBuilder sb, Column column, bool isNull, object val)
		{
			sb.Append("<div class=\"controls\">");
			sb.Append("<div class=\"input-append bootstrap-timepicker\">");
			sb.AppendFormat(
				"<input class=\"input-small\" type=\"text\" id=\"{0}\" name=\"{0}\" value=\"{1}\"/><span class=\"add-on\"><i class=\"icon-time\"></i></span>",
				column.AliasName,
				isNull ? string.Empty : val.ToString().HtmlEncode()
			);
			sb.Append("</div>");
			sb.Append("</div>");
		}
		#endregion

		#region static
		#endregion
	}

	public class Editor<T> : Editor
		where T : class, IEntity, new()
	{
		#region constructor
		public Editor(Page page)
			: base(page, typeof(T).Name)
		{
		}
		public Editor(Page page, T instance)
			: base(page, instance)
		{
		}
		#endregion

		#region property
		#endregion
	}
}
