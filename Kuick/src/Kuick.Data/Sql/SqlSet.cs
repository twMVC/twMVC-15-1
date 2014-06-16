// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlSet.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Drawing;
using System.Linq.Expressions;

namespace Kuick.Data
{
	public class SqlSet : ICloneable<SqlSet>
	{
		#region constructor
		public SqlSet(string columnName)
		{
			this.ColumnName = columnName;
			this.Format = SqlSetFormat.Value;
		}
		#endregion

		#region ICloneable<T>
		public SqlSet Clone()
		{
			SqlSet clone = new SqlSet(this.ColumnName);
			clone.ColumnName = this.ColumnName;
			clone.Format = this.Format;
			clone.ValueFormat = this.ValueFormat;
			clone.Command = this.Command;
			clone.StringValue = this.StringValue;
			clone.IntegerValue = this.IntegerValue;
			clone.DecimalValue = this.DecimalValue;
			clone.LongValue = this.LongValue;
			clone.FloatValue = this.FloatValue;
			clone.BooleanValue = this.BooleanValue;
			clone.CharValue = this.CharValue;
			clone.ByteValue = this.ByteValue;
			clone.DateTimeValue = this.DateTimeValue;
			clone.ColorValue = this.ColorValue;
			clone.ColumnValue = this.ColumnValue;
			clone.SqlValue = this.SqlValue;
			return clone;
		}
		#endregion

		#region property
		public string ColumnName { get; private set; }
		public SqlSetFormat Format { get; private set; }
		public SqlSetValueFormat ValueFormat { get; private set; }
		public string Command { get; private set; }

		internal string StringValue { get; set; }
		internal int IntegerValue { get; set; }
		internal decimal DecimalValue { get; set; }
		internal long LongValue { get; set; }
		internal float FloatValue { get; set; }
		internal bool BooleanValue { get; set; }
		internal char CharValue { get; set; }
		internal byte ByteValue { get; set; }
		internal byte[] ByteArrayValue { get; set; }
		internal DateTime DateTimeValue { get; set; }
		internal Color ColorValue { get; set; }
		internal Guid GuidValue { get; set; }
		internal Column ColumnValue { get; set; }
		internal Sql SqlValue { get; set; }
		internal object Value { get; set; }
		#endregion

		#region Operator
		#region SetValue
		public SqlSet SetValue(string value)
		{
			ValueFormat = SqlSetValueFormat.String;
			StringValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(int value)
		{
			ValueFormat = SqlSetValueFormat.Integer;
			IntegerValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(decimal value)
		{
			ValueFormat = SqlSetValueFormat.Decimal;
			DecimalValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(long value)
		{
			ValueFormat = SqlSetValueFormat.Long;
			LongValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(float value)
		{
			ValueFormat = SqlSetValueFormat.Float;
			FloatValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(bool value)
		{
			ValueFormat = SqlSetValueFormat.Boolean;
			BooleanValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(char value)
		{
			ValueFormat = SqlSetValueFormat.Char;
			CharValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(byte value)
		{
			ValueFormat = SqlSetValueFormat.Byte;
			ByteValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(byte[] value)
		{
			ValueFormat = SqlSetValueFormat.ByteArray;
			ByteArrayValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(DateTime value)
		{
			ValueFormat = SqlSetValueFormat.DateTime;
			DateTimeValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(Color value)
		{
			ValueFormat = SqlSetValueFormat.Color;
			ColorValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(Guid value)
		{
			ValueFormat = SqlSetValueFormat.Guid;
			GuidValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(Column value)
		{
			ValueFormat = SqlSetValueFormat.Column;
			ColumnValue = value;
			Value = value;
			return this;
		}
		public SqlSet SetValue(Sql value)
		{
			ValueFormat = SqlSetValueFormat.Sql;
			SqlValue = value;
			Value = value;
			return this;
		}
		#endregion

		#region Commanding
		public SqlSet Commanding(string command)
		{
			Format = SqlSetFormat.Command;
			Command = command;
			return this;
		}
		#endregion

		#region Increase
		public SqlSet Increasing()
		{
			Format = SqlSetFormat.Increasing;
			return this;
		}
		public static SqlSet Increasing(Expression<Func<IEntity, object>> expression)
		{
			return DataUtility.ToSqlSet<IEntity>(expression).Increasing();
		}
		public static SqlSet Increasing<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return DataUtility.ToSqlSet<T>(expression).Increasing();
		}
		#endregion

		#region Decrease
		public SqlSet Decreasing()
		{
			Format = SqlSetFormat.Decreasing;
			return this;
		}
		public static SqlSet Decreasing(Expression<Func<IEntity, object>> expression)
		{
			return DataUtility.ToSqlSet<IEntity>(expression).Decreasing();
		}
		public static SqlSet Decreasing<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return DataUtility.ToSqlSet<T>(expression).Decreasing();
		}
		#endregion

		#region IsNull
		public SqlSet IsNull()
		{
			Format = SqlSetFormat.IsNull;
			return this;
		}
		public static SqlSet IsNull(Expression<Func<IEntity, object>> expression)
		{
			return DataUtility.ToSqlSet<IEntity>(expression).IsNull();
		}
		public static SqlSet IsNull<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return DataUtility.ToSqlSet<T>(expression).IsNull();
		}
		#endregion

		#region Column
		public static SqlSet Column(string columnName)
		{
			return new SqlSet(columnName);
		}
		public static SqlSet Column(Column column)
		{
			return new SqlSet(column.Spec.ColumnName);
		}
		public static SqlSet Column<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return DataUtility.ToSqlSet<T>(expression);
		}
		#endregion
		#endregion
	}
}
