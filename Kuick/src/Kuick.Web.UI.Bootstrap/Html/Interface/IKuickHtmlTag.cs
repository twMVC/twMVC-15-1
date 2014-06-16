// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IKuickHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;
using System.Collections.Generic;
using System.Text;
using Kuick.Data;

namespace Kuick.Web.UI.Bootstrap
{
	public interface IKuickHtmlTag : IHtmlTag
	{
		IEntity Schema { get; }
		string KuickEntityName { get; set; }
		string KuickKeyValue { get; set; }
		string KuickColumnName { get; set; }
		string KuickOriginalValue { get; set; }
	}
}
