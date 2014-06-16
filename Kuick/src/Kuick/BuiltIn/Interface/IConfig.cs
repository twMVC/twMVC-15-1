// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IConfig.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-20 - Creation


using System.Collections.Generic;

namespace Kuick
{
	/// <summary>
	/// Self or global config.
	/// </summary>
	public interface IConfig : IBuiltin
	{
		bool Exists(string appID, string category, string path, string name);
		string Read(string appID, string category, string path, string name);
		Result Write(string appID, string category, string path, string name, string value);
		Result Clear(string appID, string category, string path, string name);

		bool GlobalExists(string category, string path, string name);
		string GlobalRead(string category, string path, string name);
		Result GlobalWrite(string category, string path, string name, string value);
		Result GlobalClear(string category, string path, string name);

		List<string> AppIDs();
		List<string> Categories(string appID);
		List<string> Paths(string appID, string category);
		List<string> Names(string appID, string category, string path);
	}
}
