// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-10-25 - Creation


using System;
using System.Data.OracleClient;

namespace Kuick.Data.OracleClient
{
	[Database("Oracle")]
	public class OracleDatabase
		: Kuick.Data.Oracle.OracleDatabase<
			OracleConnection,
			OracleTransaction,
			OracleCommand,
			OracleDataAdapter,
			System.Data.OracleClient.OracleDataReader,
			Kuick.Data.OracleClient.OracleDataReader>
	{
		public OracleDatabase(ConfigDatabase config)
			: base(config, new OracleBuilder())
		{
		}

		public override OracleCommand BuildCommand()
		{
			OracleCommand cmd = new OracleCommand();
			return cmd;
		}
	}
}
