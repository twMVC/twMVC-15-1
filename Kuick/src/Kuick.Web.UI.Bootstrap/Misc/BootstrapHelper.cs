// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BootstrapHelper.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-15 - Creation


using System;
using Kuick.Data;

namespace Kuick.Web.UI.Bootstrap
{
	public class BootstrapHelper
	{
		public static string VisualSizeToCss(VisualSize size)
		{
			switch(size) {
				case VisualSize.Mini:
					return "input-mini";
				case VisualSize.Small:
					return "input-small";
				case VisualSize.Medium:
					return "input-medium";
				case VisualSize.Large:
					return "input-large";
				case VisualSize.XLarge:
					return "input-xlarge";
				case VisualSize.XXLarge:
					return "input-xxlarge";
				default:
					throw new NotImplementedException();
			}
		}
	}
}
