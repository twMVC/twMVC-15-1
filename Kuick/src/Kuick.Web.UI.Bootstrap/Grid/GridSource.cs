// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GridSource.cs
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
	public class GridSource
	{
		public GridSource()
		{
			//this.PageSize = -1;
			//this.PageIndex = -1;
		}

		//public int PageSize { get; set; }
		//public int PageIndex { get; set; }

		public Sql Sql { get; set; }
		public List<IEntity> Datas { get; set; }
	}
}
