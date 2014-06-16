// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IMultilingual.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-18 - Creation


using System.Collections.Generic;

namespace Kuick
{
	/// <summary>
	/// Self or global multilingual.
	/// </summary>
	public interface IMultilingual : IBuiltin
	{
		string Read(string appID, string Language, string path, string name);
		Result Write(string appID, string Language, string path, string name, string value);
		Result Clear(string appID, string Language, string path, string name);

		string GlobalRead(string Language, string path, string name);
		Result GlobalWrite(string Language, string path, string name, string value);
		Result GlobalClear(string Language, string path, string name);

		List<string> AppIDs();
		List<string> LangIDs(string appID);
		List<string> Paths(string appID, string Language);
		List<string> Names(string appID, string Language, string path);
	}
}
