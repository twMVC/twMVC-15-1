// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IFeature.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public interface IFeature
	{
		string AppID { get; set; }
		string FeatureID { get; set; }
		string Title { get; set; }
		string RelativeUrl { get; set; }
		IApp App { get; }
		List<IFragment> Fragments { get; }

		IFragment GetCurrentFragment(string title, string description);
	}
}
