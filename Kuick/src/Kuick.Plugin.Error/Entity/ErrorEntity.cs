// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ErrorEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-07 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kuick.Data;

namespace Kuick.Plugin.Error
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(Schema.TimeStamp, Schema.Title, Schema.User)]
	public class ErrorEntity
		: Entity<ErrorEntity>
	{
		#region Schema
		public class Schema
		{
			// table name
			public const string TableName = "T_PLUGIN_ERROR";

			// fields
			public const string ErrorID = "ERROR_ID";
			public const string Title = "TITLE";
			public const string Message = "MESSAGE";
			public const string Detail = "DETAIL";
			public const string User = "USER";
			public const string TimeStamp = "TIME_STAMP";
		}
		#endregion

		#region Constructor
		public ErrorEntity()
		{
		}
		#endregion

		#region Property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string ErrorID { get; set; }
		[DataMember]
		[Description("Title")]
		[ColumnSpec(SpecFlag.ReadOnly, 200)]
		public string Title { get; set; }

		[DataMember]
		[Description("Message")]
		[ColumnSpec(SpecFlag.ReadOnly, 200)]
		public string Message { get; set; }

		[DataMember]
		[Description("Detail")]
		[ColumnSpec(SqlDataType.MaxVarWChar, SpecFlag.ReadOnly)]
		public string Detail { get; set; }

		[DataMember]
		[Description("User ID")]
		[ColumnSpec(SpecFlag.ReadOnly, 50)]
		public string User { get; set; }

		[DataMember]
		[Description("Time Stamp")]
		[ColumnSpec(SpecFlag.ReadOnly)]
		[ColumnInitiate(InitiateValue.Date17s)]
		public DateTime TimeStamp { get; set; }
		#endregion

		#region IEntity
		public override string TitleValue
		{
			get
			{
				return Title;
			}
		}

		public override void Interceptor(Sql<ErrorEntity> sql)
		{
			sql.Descending(x => x.TimeStamp);
			base.Interceptor(sql);
		}
		#endregion

		#region class level
		#endregion

		#region instance level
		#endregion

		#region Event Handler
		#endregion
	}
}
