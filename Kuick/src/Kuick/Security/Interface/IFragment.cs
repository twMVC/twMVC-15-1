// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IFragment.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-06-10 - Creation


using System;

namespace Kuick
{
	public interface IFragment
	{
		string AppID { get; set; }
		string FeatureID { get; set; }
		string FragmentID { get; set; }
		string Title { get; set; }
		string Description { get; set; }
		IApp App { get; }
		IFeature Feature { get; }
	}
}
