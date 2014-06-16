// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlReader.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Data.SqlClient;

namespace Kuick.Data.MSSql
{
	public class MSSqlReader : SqlReader<SqlDataReader>, IDisposable
	{
		public MSSqlReader(SqlDataReader reader)
			: base(reader)
		{
		}
	}
}
