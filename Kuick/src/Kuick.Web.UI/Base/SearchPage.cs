// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SearchPage.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-27 - Creation


using System;
using Kuick.Data;
using System.Collections.Generic;
using System.Text;

namespace Kuick.Web.UI
{
	public class SearchPage : PageBase
	{
		public SearchPage()
			: base()
		{
		}

		public override bool RenderJQueryFile { get { return false; } }
		public virtual string EntityName { get { return string.Empty; } }
		public virtual bool ShowIndex { get; set; }
	}

	public class SearchPage<T>
		: SearchPage
		where T : class, IEntity, new()
	{
		public SearchPage()
			: base()
		{
			Instances = new List<T>();
			this.Tr = new Tr<T>();
			this.Tfs = new List<Tf<T>>();
		}

		public override string EntityName { get { return typeof(T).Name; } }
		public List<T> Instances { get; set; }
		public Tr<T> Tr { get; set; }
		public List<Tf<T>> Tfs { get; set; }

		public virtual string RenderTable()
		{
			if(ShowIndex) {
				Tfs.Insert(
					0,
					new Tf<T>() {
						Th = new Th<T>() { TitleValue = "#" },
						Td = new Td<T>() { GetValue = (x, i) => i.ToString() }
					}
				);
			}

			return new Table<T>() {
				Instances = Instances,
				Tr = Tr,
				Tfs = Tfs
			}.ToHtml();
		}
	}
}
