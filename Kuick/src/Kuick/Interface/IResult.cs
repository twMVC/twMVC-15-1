// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IResult.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-26 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public interface IResult
	{
		bool Success { get; set; }
		string Message { get; set; }
		Anys Datas { get; set; }
		List<Result> InnerResults { get; set; }
		Error Error { get; set; }
		Exception Exception { get; set; }

		IResult AddResult(Result innerResult);
	}
}
