// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IDynamicData.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-18 - Creation


using System;
using System.Data;

namespace Kuick.Data
{
	public interface IDynamicData
	{
		DataRow Row { get; set; }
		DataTable DataTable { get; set; }
		Anys Record { get; set; }
		bool AsAny { get; set; }
		bool EnableDynamic { get; set; }
		dynamic AsDynamic();
	}
}
