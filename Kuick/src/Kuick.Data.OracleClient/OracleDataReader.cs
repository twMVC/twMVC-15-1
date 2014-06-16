// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleDataReader.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-10-25 - Creation


using System;
using System.Data;
using System.Data.OracleClient;

namespace Kuick.Data.OracleClient
{
	public class OracleDataReader : SqlReader<System.Data.OracleClient.OracleDataReader>, IDisposable
	{
		public OracleDataReader(System.Data.OracleClient.OracleDataReader reader)
			: base(reader)
		{
		}
	}
}
