using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Kuick.Web;
using Kuick.Data;
using Kuick;
using Kuick.Web.UI.Bootstrap;
using KuickSample;

public partial class List : Page
{

	[RequestParameter(BootstrapConstants.KeyValue)]
	public string KeyValue { get; set; }
	public IEntity Instance { get; set; }

	public virtual bool ShowSidebar { get { return true; } }

	public string EntityName { get { return UserEntity.EntityName; } }
	public IEntity CachedSpec
	{
		get
		{
			return EntityCache.Get(EntityName);
		}
	}

	protected override void OnPreInit(EventArgs e)
	{
		base.OnPreInit(e);

		// Instance
		Instance = Entity.Get(EntityName, KeyValue);

		// Grid
		if(null == Instance) {
			this.Grid = new Grid(this, EntityName);
			Grid.Visual.BuildTools = x => {
				return string.Format(
					"<a href=\"{0}\">修改</a>",
					WebTools.BuildQueryString(
						new Any(BootstrapConstants.KeyValue, x.KeyValue)
					)
				);
			};
		}

		// Editor
		if(null == Instance) {
			this.Editor = new Editor(this, EntityName);
		} else {
			this.Editor = new Editor(this, Instance);
		}
	}

	protected override void OnInitComplete(EventArgs e)
	{
		base.OnInitComplete(e);

		Editor.Deal();
		if(Editor.Dealing) {
			Response.Redirect(WebTools.GetCurrentFullPath());
		}
	}

	public Grid Grid { get; set; }
	public Editor Editor { get; set; }

	public string Render()
	{
		StringBuilder sb = new StringBuilder();

		sb.Append("<div class=\"tabbable\">");
		sb.Append(RenderMainbar());
		sb.Append("</div>");

		if(ShowSidebar) {
			sb.Append("</div>");
		}

		return sb.ToString();
	}

	public string RenderMainbar()
	{
		StringBuilder sb = new StringBuilder();
		if(null == Instance) {
			// List
			sb.Append("<ul class=\"nav nav-tabs\">");
			sb.Append("<li class=\"active\"><a href=\"#tab1\" data-toggle=\"tab\">列表</a></li>");
			sb.Append("<li><a href=\"#tab2\" data-toggle=\"tab\">查詢</a></li>");
			sb.Append("<li><a href=\"#tab3\" data-toggle=\"tab\">新增</a></li>");
			sb.Append("</ul>");
			sb.Append("");
			sb.Append("<div class=\"tab-content\">");
			sb.Append("<div class=\"tab-pane active\" id=\"tab1\">");
			sb.Append(Grid.RenderTable());
			sb.Append("</div>");
			sb.Append("<div class=\"tab-pane\" id=\"tab2\">");
			sb.Append("<p>");
			sb.Append("todo..</p>");
			sb.Append("</div>");
			sb.Append("<div class=\"tab-pane\" id=\"tab3\">");
			sb.Append(Editor.RenderForm());
			sb.Append("</div>");
			sb.Append("</div>");
		} else {
			// Modify
			sb.Append("<ul class=\"nav nav-tabs\">");
			sb.Append("<li class=\"active\"><a href=\"#tab1\" data-toggle=\"tab\">修改</a></li>");
			sb.Append("</ul>");
			sb.Append("");
			sb.Append("<div class=\"tab-content\">");
			sb.Append("<div class=\"tab-pane active\" id=\"tab1\">");
			sb.Append(Editor.RenderForm());
			sb.Append("</div>");
			sb.Append("</div>");
		}
		return sb.ToString();
	}

	public string RenderJavaScript()
	{
		StringBuilder sb = new StringBuilder();
		if(null != Grid) { sb.Append(Grid.RenderJavaScript()); }
		if(null != Editor) { sb.Append(Editor.RenderJavaScript()); }
		return sb.ToString();
	}

}