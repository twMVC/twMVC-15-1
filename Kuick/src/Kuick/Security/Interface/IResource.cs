// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IResource.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;

namespace Kuick
{
	public interface IResource
	{
		string ResourceID { get; set; }
		ResourceType Type { get; set; }
		string AppID { get; set; }
		string Group { get; set; }
		string Name { get; set; }
	}
}
