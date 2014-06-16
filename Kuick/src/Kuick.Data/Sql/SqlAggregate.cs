// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlAggregate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Linq.Expressions;

namespace Kuick.Data
{
	public class SqlAggregate : IWithOperator, ICloneable<SqlAggregate>
	{
		#region constructor
		public SqlAggregate(Column column)
			: this(column.FullName)
		{
		}

		public SqlAggregate(string columnName)
		{
			ColumnName = columnName;
		}
		#endregion

		#region ICloneable<T>
		public SqlAggregate Clone()
		{
			SqlAggregate clone = new SqlAggregate(this.ColumnName);
			clone.Function = this.Function;
			clone.AsName = this.AsName;
			return clone;
		}
		#endregion

		#region property
		public string ColumnName { get; private set; }
		public SqlAggregateFunction Function { get; private set; }
		public string AsName { get; private set; }
		#endregion

		#region Operator
		#region Average
		public SqlAggregate Average()
		{
			Function = SqlAggregateFunction.Average;
			return this;
		}
		#endregion

		#region Count
		public SqlAggregate Count()
		{
			Function = SqlAggregateFunction.Count;
			return this;
		}
		#endregion

		#region Maximum
		public SqlAggregate Maximum()
		{
			Function = SqlAggregateFunction.Maximum;
			return this;
		}
		#endregion

		#region Minimum
		public SqlAggregate Minimum()
		{
			Function = SqlAggregateFunction.Minimum;
			return this;
		}
		#endregion

		#region Sum
		public SqlAggregate Sum()
		{
			Function = SqlAggregateFunction.Sum;
			return this;
		}
		#endregion

		#region As
		public SqlAggregate As(string asName)
		{
			AsName = asName;
			return this;
		}
		#endregion

		#region Column
		public static SqlAggregate Column(string columnName)
		{
			return new SqlAggregate(columnName);
		}
		public static SqlAggregate Column(Column column)
		{
			return new SqlAggregate(column.FullName);
		}
		public static SqlAggregate Column<T>(
			Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return Column(DataUtility.ToColumn<T>(expression));
		}
		#endregion
		#endregion

		#region IWithOperator
		public string GetOperator()
		{
			return GetOperator(Function);
		}
		#endregion

		#region static
		public static string GetOperator(SqlAggregateFunction function)
		{
			switch(function) {
				case SqlAggregateFunction.Average:
					return " AVG ";
				case SqlAggregateFunction.Count:
					return " COUNT ";
				case SqlAggregateFunction.Maximum:
					return " MAX ";
				case SqlAggregateFunction.Minimum:
					return " MIN ";
				case SqlAggregateFunction.Sum:
					return " SUM ";
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
