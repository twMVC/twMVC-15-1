// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IApp.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public interface IApp
	{
		string AppID { get; set; }
		string Title { get; set; }
		List<IFeature> Features { get; }

		IFeature GetCurrentFeature(object o);
	}
}
