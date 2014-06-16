// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlOrderBy.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Linq.Expressions;

namespace Kuick.Data
{
	public class SqlOrderBy : IWithOperator, ICloneable<SqlOrderBy>
	{
		#region constructor
		public SqlOrderBy(Column column)
		{
			this.ColumnName = column.Spec.ColumnName;
			this.TableName = column.TableName;
		}

		public SqlOrderBy(string columnName)
		{
			this.ColumnName = columnName;
		}
		#endregion

		#region ICloneable<T>
		public SqlOrderBy Clone()
		{
			SqlOrderBy clone = new SqlOrderBy(this.ColumnName);
			clone.ColumnName = this.ColumnName;
			clone.Direction = this.Direction;
			clone.TableName = this.TableName;
			return clone;
		}
		#endregion

		#region property
		public string ColumnName { get; private set; }
		public SqlDirection Direction { get; private set; }

		internal bool Ambiguous { get; set; }
		internal string TableName { get; set; }
		internal string FullName
		{
			get
			{
				return string.IsNullOrEmpty(TableName)
					? ColumnName
					: string.Concat(TableName, ".", ColumnName);
			}
		}
		#endregion

		#region Operator
		#region Ascending
		public SqlOrderBy Ascending()
		{
			Direction = SqlDirection.Ascending;
			return this;
		}
		public static SqlOrderBy Ascending(Column column)
		{
			return new SqlOrderBy(column).Ascending();
		}
		public static SqlOrderBy Ascending(string columnName)
		{
			return new SqlOrderBy(columnName).Ascending();
		}
		public static SqlOrderBy Ascending(
			Expression<Func<IEntity, object>> expression)
		{
			Column column = DataUtility.ToColumn<IEntity>(expression);
			return new SqlOrderBy(column).Ascending();
		}
		public static SqlOrderBy Ascending<T>(
			Expression<Func<T, object>> expression)
			where T : IEntity
		{
			Column column = DataUtility.ToColumn<T>(expression);
			return new SqlOrderBy(column).Ascending();
		}
		#endregion

		#region Descending
		public SqlOrderBy Descending()
		{
			Direction = SqlDirection.Descending;
			return this;
		}
		public static SqlOrderBy Descending(Column column)
		{
			return new SqlOrderBy(column).Descending();
		}
		public static SqlOrderBy Descending(string columnName)
		{
			return new SqlOrderBy(columnName).Descending();
		}
		public static SqlOrderBy Descending(
			Expression<Func<IEntity, object>> expression)
		{
			Column column = DataUtility.ToColumn<IEntity>(expression);
			return new SqlOrderBy(column).Descending();
		}
		public static SqlOrderBy Descending<T>(
			Expression<Func<T, object>> expression)
			where T : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<T>(expression);
			return new SqlOrderBy(column).Descending();
		}
		#endregion

		#region Column
		public static SqlOrderBy Column(Column column)
		{
			return new SqlOrderBy(column);
		}
		public static SqlOrderBy Column(string columnName)
		{
			return new SqlOrderBy(columnName);
		}
		public static SqlOrderBy Column(
			Expression<Func<IEntity, object>> expression)
		{
			return new SqlOrderBy(DataUtility.ToColumn<IEntity>(expression));
		}
		public static SqlOrderBy Column<T>(Expression<Func<T, object>> expression)
			where T : class, IEntity, new()
		{
			return new SqlOrderBy(DataUtility.ToColumn<T>(expression));
		}
		#endregion
		#endregion

		#region IWithOperator
		public string GetOperator()
		{
			return GetOperator(Direction);
		}
		#endregion

		#region static
		public static string GetOperator(SqlDirection direction)
		{
			switch(direction) {
				case SqlDirection.Ascending:
					return " ASC ";
				case SqlDirection.Descending:
					return " DESC ";
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
