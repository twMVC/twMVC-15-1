// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class ConfigDatabase : GroupNameBase
	{
		#region constructor
		public ConfigDatabase()
			: this(string.Empty, string.Empty, string.Empty, string.Empty)
		{
		}

		//public ConfigDatabase(string vender, string name, string connectionString)
		//    : this(vender, name, string.Empty, connectionString)
		//{
		//}

		public ConfigDatabase(string vender, string name, string Schema, string connectionString)
		{
			base.Group = vender;
			this.Vender = vender;
			this.Name = name;
			this.Schema = Schema;
			this.ConnectionString = connectionString;
		}
		#endregion

		#region property
		public string Vender { get; set; }
		public string ConnectionString { get; set; }
		public string Schema { get; set; }

		public string Key { get { return BuildKey(); } }
		#endregion

		#region override
		public override string ToString()
		{
			return ConnectionString;
		}
		#endregion

		#region private
		private string BuildKey()
		{
			return string.Concat(Group, Constants.Symbol.At, Name);
		}
		#endregion

		#region inner constants class
		internal class Xml
		{
			internal const string Tag = "add";

			internal class Attribute
			{
				internal const string Vender = "vender";
				internal const string Name = "name";
				internal const string Schema = "schema";
				internal const string ConnectionString = "connectionstring";
			}
		}
		#endregion
	}
}