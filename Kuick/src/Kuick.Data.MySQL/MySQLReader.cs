// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MySQLReader.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-22 - Creation


using System;
using MySql.Data.MySqlClient;

namespace Kuick.Data.MySQL
{
	public class MySQLReader : SqlReader<MySqlDataReader>, IDisposable
	{
		public MySQLReader(MySqlDataReader reader)
			: base(reader)
		{
		}
	}
}
