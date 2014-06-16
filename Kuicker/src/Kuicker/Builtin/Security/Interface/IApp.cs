// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;

namespace Kuicker
{
	public interface IApp
	{
		string AppID { get; set; }
		string Title { get; set; }
		//List<IFeature> Features { get; }

		//IFeature GetCurrentFeature(object o);
	}
}
