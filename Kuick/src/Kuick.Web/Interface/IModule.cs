// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IModule.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public interface IModule : IWeb
	{
		HttpApplication Application { get; set; }
		IWeb WebBase { get; }
	}
}
