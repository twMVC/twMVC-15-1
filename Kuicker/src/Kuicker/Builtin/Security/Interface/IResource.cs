// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
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
