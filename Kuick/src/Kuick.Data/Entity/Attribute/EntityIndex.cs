// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntityIndex.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class EntityIndex : Attribute, ICloneable<EntityIndex>, IEquatable<EntityIndex>
	{
		#region constructor
		public EntityIndex(params string[] columnNames)
			: this(false, columnNames)
		{
		}
		public EntityIndex(bool unique, params string[] columnNames)
		{
			if(Checker.IsNull(columnNames)) {
				throw new ArgumentNullException("columnNames");
			}

			this.Unique = unique;
			this.ColumnNames = columnNames;
		}
		#endregion

		#region static
		public static EntityIndex UniqueIndex<T>(Expression<Func<T, object>> expression) 
			where T : class, IEntity, new()
		{
			throw new NotImplementedException();
		}
		public static EntityIndex NonUniqueIndex<T>(Expression<Func<T, object>> expression)
			where T : class, IEntity, new()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region ICloneable<T>
		public EntityIndex Clone()
		{
			return new EntityIndex(Unique, ColumnNames);
		}
		#endregion

		#region property
		public bool Unique { get; internal set; }
		public string[] ColumnNames { get; internal set; }

		// Parent
		public Table Table { get; internal set; }
		#endregion

		#region assistance
		public string IndexName
		{
			get
			{
				return Table.BuildIndexName(this);
			}
		}
		#endregion

		#region IEquatable<EntityIndex>
		public bool Equals(EntityIndex other)
		{
			if(Unique != other.Unique){return false;}
			if(ColumnNames.Length != other.ColumnNames.Length){return false;}
			
			for(int i = 0; i < ColumnNames.Length;i++){
				if(ColumnNames[i] != other.ColumnNames[i]){return false;}
			}

			return true;
		}
		#endregion

		public override string ToString()
		{
			return string.Format(
				"{0} with columns of: {1}.",
				Unique ? "Unique index" : "Index",
				ColumnNames.ToString(DataConstants.Default.Joiner)
			);
		}
	}
}
