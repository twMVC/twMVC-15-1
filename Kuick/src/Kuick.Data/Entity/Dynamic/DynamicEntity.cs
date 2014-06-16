// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DynamicEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Kuick.Data
{
	public class DynamicEntity
	{
		public static DataResult ExecuteNonQuery(
			string sql, params DbParameter[] parameters)
		{
			DataResult result = Api.Get().ExecuteNonQuery(sql, parameters);
			return result;
		}
		public static List<DynamicData> ExecuteQuery(
			string sql, params DbParameter[] parameters)
		{
			DataSet ds = Api.GetNew().ExecuteQuery(sql, parameters);
			return ds.ToDynamic();
		}
		public static List<DynamicData> ExecuteStoredProcedure(
			string storedProcedure, params DbParameter[] parameters)
		{
			DataSet ds = Api.Get().ExecuteStoredProcedure(
				storedProcedure, parameters
			);
			return ds.ToDynamic();
		}
	}

	public class DynamicEntity<T>
		: DynamicEntity
		where T : class, IEntity, new()
	{
		#region constructor
		public DynamicEntity()
		{
		}
		#endregion

		#region property
		public DataSet LatestDataSet { get; private set; }
		#endregion

		#region Execute method
		public new DataResult ExecuteNonQuery(
			string sql, params DbParameter[] parameters)
		{
			DataResult result = Api.Get<T>().ExecuteNonQuery(sql, parameters);
			return result;
		}

		public new List<DynamicData> ExecuteQuery(
			string sql, params DbParameter[] parameters)
		{
			LatestDataSet = Api.GetNew<T>().ExecuteQuery(sql, parameters);
			return LatestDataSet.ToDynamic();
		}

		public new List<DynamicData> ExecuteStoredProcedure(
			string storedProcedure, params DbParameter[] parameters)
		{
			LatestDataSet = Api
				.Get<T>()
				.ExecuteStoredProcedure(storedProcedure, parameters);
			return LatestDataSet.ToDynamic();
		}
		#endregion
	}
}
