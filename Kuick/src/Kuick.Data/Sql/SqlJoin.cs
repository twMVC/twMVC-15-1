// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlJoin.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Linq.Expressions;

namespace Kuick.Data
{
	// LeftOuterJoin
	public class SqlJoin : ICloneable<SqlJoin>, IWithOperator
	{
		#region constructor
		public SqlJoin()
			: this(string.Empty)
		{
		}
		public SqlJoin(string asName)
		{
			this.AsName = Checker.IsNull(asName) 
				? string.Concat("JOIN_", DataCurrent.Index.ToString())
				: asName;
		}
		#endregion

		#region ICloneable<T>
		public SqlJoin Clone()
		{
			SqlJoin clone = new SqlJoin(AsName);
			clone.AsName = AsName;
			clone.LeftSchema = LeftSchema;
			clone.LeftColumnName = LeftColumnName;
			clone.RightSchema = RightSchema;
			clone.RightColumnName = RightColumnName;
			return clone;
		}
		#endregion

		#region property
		public string AsName { get; private set; }
		public IEntity LeftSchema { get; private set; }
		public string LeftColumnName { get; private set; }
		public IEntity RightSchema { get; private set; }
		public string RightColumnName { get; private set; }

		public string LeftFullName
		{
			get
			{
				return string.Format(
					"{0}.{1}", LeftSchema.TableName, LeftColumnName
				);
			}
		}
		public string RightFullName
		{
			get
			{
				return string.Format(
					"{0}.{1}", RightSchema.TableName, RightColumnName
				);
			}
		}
		#endregion

		#region Operator
		#region Left
		public SqlJoin Left<TLeft>()
			where TLeft : class, IEntity, new()
		{
			return Left<TLeft>(string.Empty);
		}
		public SqlJoin Left<TLeft>(string columnName)
			where TLeft : class, IEntity, new()
		{
			this.LeftSchema = EntityCache.Find<TLeft>();
			this.LeftColumnName = Checker.IsNull(columnName)
				? LeftSchema.KeyColumn.Spec.ColumnName
				: columnName;
			return this;
		}
		public SqlJoin Left<TLeft>(Expression<Func<TLeft, object>> expression)
			where TLeft : class, IEntity, new()
		{
			this.LeftSchema = EntityCache.Find<TLeft>();
			Column column = DataUtility.ToColumn<TLeft>(expression);
			this.LeftColumnName = column.Spec.ColumnName;
			return this;
		}
		#endregion

		#region Right
		public SqlJoin Right<TRight>()
			where TRight : class, IEntity, new()
		{
			return Left<TRight>(string.Empty);
		}
		public SqlJoin Right<TRight>(string columnName)
			where TRight : class, IEntity, new()
		{
			this.RightSchema = EntityCache.Find<TRight>();
			this.RightColumnName = Checker.IsNull(columnName)
				? RightSchema.KeyColumn.Spec.ColumnName
				: columnName;
			return this;
		}
		public SqlJoin Right<TRight>(Expression<Func<TRight, object>> expression)
			where TRight : class, IEntity, new()
		{
			this.RightSchema = EntityCache.Find<TRight>();
			Column column = DataUtility.ToColumn<TRight>(expression);
			this.RightColumnName = column.Spec.ColumnName;
			return this;
		}
		#endregion
		#endregion

		#region IWithOperator
		public string GetOperator()
		{
			return "LEFT OUTER JOIN";
		}
		#endregion
	}
}
