// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using Oracle.DataAccess.Client;
using OracleClient = Oracle.DataAccess.Client;

namespace Kuick.Data.OracleClientX86
{
	[Database("Oracle")]
	public class OracleDatabase
		: Kuick.Data.Oracle.OracleDatabase<
			OracleConnection,
			OracleTransaction,
			OracleCommand,
			OracleDataAdapter,
			OracleClient.OracleDataReader,
			Kuick.Data.OracleClientX86.OracleDataReader>
	{
		public OracleDatabase(ConfigDatabase config)
			: base(config, new OracleBuilder())
		{
		}

		public override OracleCommand BuildCommand()
		{
			// Obtaining Data from an OracleDataReader Object
			// http://docs.oracle.com/cd/B19306_01/win.102/b14307/featData.htm#i1007197

			OracleCommand cmd = new OracleCommand();
			cmd.InitialLONGFetchSize = -1;
			return cmd;
		}
	}
}
