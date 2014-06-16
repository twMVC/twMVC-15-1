// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HttpHelper.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-10-02 - Creation


using System;

namespace Kuick
{
	public class HttpHelper
	{

		public static string MethodToString(HttpMethod method)
		{
			switch(method) {
				case HttpMethod.Get:
					return Constants.HttpMethod.Get;
				case HttpMethod.Head:
					return Constants.HttpMethod.Head;
				case HttpMethod.Post:
					return Constants.HttpMethod.Post;
				case HttpMethod.Put:
					return Constants.HttpMethod.Put;
				case HttpMethod.Delete:
					return Constants.HttpMethod.Delete;
				case HttpMethod.Trace:
					return Constants.HttpMethod.Trace;
				case HttpMethod.Options:
					return Constants.HttpMethod.Options;
				default:
					throw new NotImplementedException(method.ToString());
			}
		}
	}
}
