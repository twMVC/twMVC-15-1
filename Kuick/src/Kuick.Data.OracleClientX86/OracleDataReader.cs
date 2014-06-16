// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleDataReader.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Data;
using OracleClient = Oracle.DataAccess.Client;

namespace Kuick.Data.OracleClientX86
{
	public class OracleDataReader : SqlReader<OracleClient.OracleDataReader>, IDisposable
	{
		public OracleDataReader(OracleClient.OracleDataReader reader)
			: base(reader)
		{
		}
	}
}
