// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IObjectEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	public interface IObjectEntity : IEntity
	{
		DateTime CreateDate { get; set; }
		DateTime LastModifiedDate { get; set; }
		bool Flag { get; set; }

		bool SkipUpdateModifiedDate { get; set; }

		DataResult UpdateModifiedDate();

		void ResetCache();
	}
}
