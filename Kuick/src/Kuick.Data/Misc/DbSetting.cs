// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DbSetting.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class DbSetting : NameBase
	{
		public DbSetting()
		{
		}

		public DbSetting(string vender, string name, string schema, string connectionString)
		{
			this.Vender = vender;
			this.Name = name;
			this.ConnectionString = connectionString;
			this.Schema = schema;
			this.Version = string.Empty;
			this.TableNames = new List<string>();
		}

		public string Vender { get; private set; }
		public string ConnectionString { get; private set; }
		public string Schema { get; internal set; }
		public string Version { get; internal set; }
		public List<string> TableNames { get; private set; }
	}
}
