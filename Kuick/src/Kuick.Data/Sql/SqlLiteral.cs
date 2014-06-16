// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlLiteral.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	public class SqlLiteral : ICloneable<SqlLiteral>
	{
		#region constructor
		public SqlLiteral()
		{
		}
		public SqlLiteral(string columnName)
		{
			this.ColumnName = columnName;
		}
		#endregion

		#region ICloneable<T>
		public SqlLiteral Clone()
		{
			SqlLiteral clone = new SqlLiteral();
			clone.ColumnName = ColumnName;
			clone.CommandText = CommandText;
			clone.AsName = AsName;
			clone.Format = Format;
			clone.Range = Range;
			return clone;
		}
		#endregion

		#region property
		public string ColumnName { get; private set; }
		public string CommandText { get; private set; }
		public string AsName { get; private set; }
		public SqlLiteralFormat Format { get; private set; }
		public SqlDistinctDate Range { get; private set; }
		#endregion

		#region static Operator
		public static SqlLiteral BuildCommanding(string commandText)
		{
			return new SqlLiteral().Commanding(commandText);
		}
		public static SqlLiteral BuildCommanding(string commandText, string asName)
		{
			return new SqlLiteral().Commanding(commandText, asName);
		}
		public static SqlLiteral BuildDistinctDate(
			string columnName, SqlDistinctDate range)
		{
			return new SqlLiteral(columnName).DistinctDate(range);
		}
		#endregion

		#region Operator
		#region Commanding
		public SqlLiteral Commanding(string commandText)
		{
			Format = SqlLiteralFormat.Command;
			CommandText = commandText;
			return this;
		}

		public SqlLiteral Commanding(string commandText, string asName)
		{
			Format = SqlLiteralFormat.Command;
			CommandText = commandText;
			AsName = asName;
			return this;
		}
		#endregion

		#region DistinctDate
		public SqlLiteral DistinctDate(SqlDistinctDate range)
		{
			Format = SqlLiteralFormat.DistinctDate;
			Range = range;
			AsName = SqlLiteralFormat.DistinctDate.ToString();
			return this;
		}
		#endregion
		#endregion
	}
}
