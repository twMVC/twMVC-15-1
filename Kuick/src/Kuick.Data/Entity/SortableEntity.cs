// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SortableEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-12 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace Kuick.Data
{
	[DataContract]
	[EntityIndex(false, FLAG, CREATE_DATE)]
	public class SortableEntity<T>
		: ObjectEntity<T>, ISortableEntity
		where T : SortableEntity<T>, new()
	{
		#region Schema
		public const string ORDER = "ORDER";
		#endregion

		#region constructor
		public SortableEntity()
			: base()
		{
		}
		#endregion

		#region property
		[DataMember]
		[Category(DataConstants.Entity.Category)]
		[Description("Ordering")]
		[ColumnInitiate(1)]
		[ColumnVisual(VisualFlag.SystemColumn)]
		[IgnoreDiff]
		public int Order { get; set; }
		#endregion
		
		#region IEntity
		public override void Interceptor(Sql<T> sql)
		{
			sql.Ascending(x => x.Order);
			base.Interceptor(sql);
		}
		#endregion

		#region class level
		#endregion

		#region event handler
		#endregion
	}
}
