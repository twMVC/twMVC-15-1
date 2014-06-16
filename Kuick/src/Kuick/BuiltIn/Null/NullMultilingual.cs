// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullMultilingual.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;

namespace Kuick
{
	public class NullMultilingual : NullBuiltin, IMultilingual
	{
		public string Read(string appID, string Language, string path, string name)
		{
			return string.Empty;
		}

		public Result Write(string appID, string Language, string path, string name, string value)
		{
			return Result.BuildSuccess();
		}

		public Result Clear(string appID, string Language, string path, string name)
		{
			return Result.BuildSuccess();
		}

		public string GlobalRead(string Language, string path, string name)
		{
			return string.Empty;
		}

		public Result GlobalWrite(string Language, string path, string name, string value)
		{
			return Result.BuildSuccess();
		}

		public Result GlobalClear(string Language, string path, string name)
		{
			return Result.BuildSuccess();
		}

		public List<string> AppIDs()
		{
			return new List<string>();
		}

		public List<string> LangIDs(string appID)
		{
			return new List<string>();
		}

		public List<string> Paths(string appID, string Language)
		{
			return new List<string>();
		}

		public List<string> Names(string appID, string Language, string path)
		{
			return new List<string>();
		}
	}
}
