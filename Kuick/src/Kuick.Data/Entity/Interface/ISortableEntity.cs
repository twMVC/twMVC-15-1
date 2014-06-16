// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ISortableEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-12 - Creation


using System;

namespace Kuick.Data
{
	public interface ISortableEntity : IObjectEntity
	{
		int Order { get; set; }
	}
}
