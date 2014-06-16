// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GridVisual.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-05-01 - Creation


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kuick.Data;

namespace Kuick.Web.UI.Bootstrap
{
	public class GridVisual
	{
		public GridVisual()
		{
			this.ValueFilters = new Dictionary<string,Func<IEntity,string>>();
		}


		//// Index
		//public bool ShowIndex { get { return true; } }
		//public Func<IEntity, string> IndexCss { get; set; }
		//public Func<IEntity, string> Index { get; set; }

		//// Tools
		//public bool ShowTools { get { return true; } }
		//public Func<IEntity, string> ToolsCss { get; set; }
		//public Func<IEntity, string> Tools { get; set; }

		//// Title
		//public Dictionary<string, Func<IEntity, string>> TitleCss { get; set; }
		//public Dictionary<string, Func<IEntity, string>> TitleFilters { get; set; }

		//// Value
		//public Dictionary<string, Func<IEntity, string>> ValueCss { get; set; }
		//public Dictionary<string, Func<IEntity, string>> ValueFilters { get; set; }

		//// Row
		//public Func<IEntity, string> RowCss { get; set; }
		//public Func<IEntity, string> RowFilters { get; set; }

		//// Head

		//// Compute
		//public Action<IEntity> Compute { get; set; }

		//// TopRow

		//// BottomRow



		public Func<IEntity, string> BuildTools { get; set; }

		public Dictionary<string, Func<IEntity, string>> ValueFilters { get; set; }
		public Func<IEntity, RowStatus> RowFilter { get; set; }
	}
}
