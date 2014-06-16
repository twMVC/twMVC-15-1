// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ListPage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-24 - Creation


using System;
using Kuick.Data;
using System.Collections.Generic;
using System.Text;

namespace Kuick.Web.UI
{
	public class ListPage : PageBase
	{
		#region const
		public const string ENTITY_NAME = "EntityName";
		#endregion

		public ListPage()
			: base()
		{
		}

		#region properties
		public override bool RenderJQueryFile { get { return false; } }

		public virtual string EntityName { get { return string.Empty; } }

		[RequestParameter]
		public int PageSize { get; set; }

		[RequestParameter]
		public int PageIndex { get; set; }

		[RequestParameter]
		public int CurrentIndex { get; set; }

		public Paginator Paging { get; internal set; }

		public int TotleCount { get; internal set; }

		public int CurrentNo { get; set; }
		#endregion
	}

	public class ListPage<T>
		: ListPage
		where T : class, IEntity, new()
	{
		public ListPage()
			: base()
		{
			Instances = new List<T>();
			Fields = new List<All<T>>();
		}

		public override string EntityName { get { return typeof(T).Name; } }
		public List<T> Instances { get; set; }
		public List<All<T>> Fields { get; set; }

		#region event handler
		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);

			// TotleCount


			// Paging
			if(PageSize > 1) {
				Paging = new Paginator(PageSize, PageIndex, TotleCount);
				Paging.ViewportSize = 8;
			}

			if(IsPostBack) {

			} else {

			}
		}
		#endregion

		#region Render
		public virtual string RenderTool()
		{
			return string.Empty;
			//return "<div class=\"tool\">Tool</div>";
		}

		public virtual string RenderTable()
		{
			CurrentNo = PageSize * CurrentIndex;

			Fields.Insert(
				0, 
				new All<T>(
					"#", 
					delegate(T x) {
						return (++CurrentNo).ToString(); 
					}
				)
			);
			return Show.Table<T>(Instances, Fields.ToArray());
		}

		public virtual string RenderPaginator()
		{
			return string.Empty; 

			//if(null == Paging) { return string.Empty; }

			//StringBuilder sb = new StringBuilder();
			//sb.Append();

			//return "<div class=\"paginator\">Paginator</div>";
		}
		#endregion
	}
}
