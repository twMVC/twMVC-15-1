// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlExpression.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;

namespace Kuick.Data
{
	public class SqlExpression : IWithOperator, ICloneable<SqlExpression>
	{
		#region constructor
		public SqlExpression(Column column)
		{
			this.ColumnName = column.Spec.ColumnName;
			this.TableName = column.TableName;
		}

		public SqlExpression(string columnName)
		{
			this.ColumnName = columnName;
		}
		#endregion

		#region ICloneable<T>
		public SqlExpression Clone()
		{
			SqlExpression clone = new SqlExpression(this.ColumnName);
			clone.Operator = this.Operator;
			clone.Format = this.Format;
			clone.StringValue = this.StringValue;
			clone.IntegerValue = this.IntegerValue;
			clone.DecimalValue = this.DecimalValue;
			clone.LongValue = this.LongValue;
			clone.FloatValue = this.FloatValue;
			clone.BooleanValue = this.BooleanValue;
			clone.CharValue = this.CharValue;
			clone.ByteValue = this.ByteValue;
			clone.DateTimeValue = this.DateTimeValue;
			clone.StringArrayValue = this.StringArrayValue;
			clone.IntegerArrayValue = this.IntegerArrayValue;
			clone.EntityArrayValue = this.EntityArrayValue;
			clone.ColorValue = this.ColorValue;
			clone.ColumnValue = this.ColumnValue;
			clone.SqlValue = this.SqlValue;
			clone.TableName = this.TableName;
			return clone;
		}
		#endregion

		#region property
		public string ColumnName { get; private set; }
		public SqlOperator Operator { get; internal set; }
		public SqlDataFormat Format { get; private set; }
		public string Literal { get; private set; }

		private string StringValue { get;  set; }
		private int IntegerValue { get;  set; }
		private decimal DecimalValue { get;  set; }
		private long LongValue { get;  set; }
		private float FloatValue { get;  set; }
		private bool BooleanValue { get;  set; }
		private char CharValue { get;  set; }
		private byte ByteValue { get;  set; }
		private DateTime DateTimeValue { get; set; }
		private string[] StringArrayValue { get; set; }
		private int[] IntegerArrayValue { get;  set; }
		private IEntity[] EntityArrayValue { get; set; }
		private Color ColorValue { get;  set; }
		private Column ColumnValue { get;  set; }
		private Sql SqlValue { get;  set; }

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

		#region static Operstor
		public static SqlExpression Column(Column column)
		{
			return new SqlExpression(column);
		}
		public static SqlExpression Column(string columnName)
		{
			return new SqlExpression(columnName);
		}
		public static SqlExpression Column(
			Expression<Func<IEntity, object>> expression)
		{
			Column column = DataUtility.ToColumn(expression);
			return new SqlExpression(column);
		}
		public static SqlExpression Column<T>(
			Expression<Func<T, object>> expression)
			where T : IEntity
		{
			Column column = DataUtility.ToColumn<T>(expression);
			return new SqlExpression(column);
		}
		#endregion

		#region Operator
		#region LessThan
		public SqlExpression LessThan(string value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(int value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(decimal value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(long value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(float value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(char value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(byte value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		public SqlExpression LessThan(DateTime value)
		{
			Operator = SqlOperator.LessThan;
			SetValue(value);
			return this;
		}
		#endregion

		#region LessEqualTo
		public SqlExpression LessEqualTo(string value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(int value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(decimal value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(long value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(float value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(char value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(byte value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression LessEqualTo(DateTime value)
		{
			Operator = SqlOperator.LessEqualTo;
			SetValue(value);
			return this;
		}
		#endregion

		#region EqualTo
		public SqlExpression EqualTo(string value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(int value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(decimal value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(long value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(float value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(bool value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(char value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(byte value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(DateTime value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(Color value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression EqualTo(Column value)
		{
			this.Operator = SqlOperator.EqualTo;
			SetValue(value);
			return this;
		}
		#endregion

		#region GreatEqualTo
		public SqlExpression GreatEqualTo(string value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(int value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(decimal value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(long value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(float value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(char value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(byte value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatEqualTo(DateTime value)
		{
			Operator = SqlOperator.GreatEqualTo;
			SetValue(value);
			return this;
		}
		#endregion

		#region GreatThan
		public SqlExpression GreatThan(string value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(int value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(decimal value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(long value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(float value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(char value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(byte value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		public SqlExpression GreatThan(DateTime value)
		{
			Operator = SqlOperator.GreatThan;
			SetValue(value);
			return this;
		}
		#endregion

		#region NotEqualTo
		public SqlExpression NotEqualTo(string value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(int value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(decimal value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(long value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(float value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(bool value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(char value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(byte value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(DateTime value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(Color value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		public SqlExpression NotEqualTo(Column value)
		{
			this.Operator = SqlOperator.NotEqualTo;
			SetValue(value);
			return this;
		}
		#endregion

		#region Like
		public SqlExpression Like(string value)
		{
			this.Operator = SqlOperator.Like;
			SetValue(value);
			return this;
		}
		#endregion

		#region NotLike
		public SqlExpression NotLike(string value)
		{
			this.Operator = SqlOperator.NotLike;
			SetValue(value);
			return this;
		}
		#endregion

		#region In
		public SqlExpression In(params string[] values)
		{
			this.Operator = SqlOperator.In;
			SetValues(values);
			return this;
		}
		public SqlExpression In(params int[] values)
		{
			this.Operator = SqlOperator.In;
			SetValues(values);
			return this;
		}
		public SqlExpression In(params IEntity[] values)
		{
			this.Operator = SqlOperator.In;
			SetValues(values);
			return this;
		}
		public SqlExpression In(Sql sql)
		{
			this.Operator = SqlOperator.In;
			SetValue(sql);
			return this;
		}
		#endregion

		#region NotIn
		public SqlExpression NotIn(params string[] values)
		{
			Operator = SqlOperator.NotIn;
			SetValues(values);
			return this;
		}
		public SqlExpression NotIn(params int[] values)
		{
			Operator = SqlOperator.NotIn;
			SetValues(values);
			return this;
		}
		public SqlExpression NotIn(params IEntity[] values)
		{
			Operator = SqlOperator.NotIn;
			SetValues(values);
			return this;
		}
		public SqlExpression NotIn(Sql sql)
		{
			Operator = SqlOperator.NotIn;
			SetValue(sql);
			return this;
		}
		#endregion

		#region IsNull
		public SqlExpression IsNull()
		{
			Operator = SqlOperator.IsNull;
			return this;
		}

		public static SqlExpression IsNull(Column column)
		{
			return IsNull(column.Spec.ColumnName);
		}
		public static SqlExpression IsNull(string columnName)
		{
			return new SqlExpression(columnName).IsNull();
		}
		public static SqlExpression IsNull(
			Expression<Func<IEntity, object>> exprerss)
		{
			return SqlExpression
				.Column(DataUtility.ToColumn<IEntity>(exprerss.Body))
				.IsNull();
		}
		public static SqlExpression IsNull<T>(
			Expression<Func<T, object>> exprerss)
			where T : IEntity
		{
			return DataUtility.ToSqlExpression<T>(exprerss).IsNull();
		}
		#endregion

		#region IsNotNull
		public SqlExpression IsNotNull()
		{
			Operator = SqlOperator.IsNotNull;
			return this;
		}
		public static SqlExpression IsNotNull(
			Expression<Func<IEntity, object>> exprerss)
		{
			return SqlExpression
				.Column(DataUtility.ToColumn<IEntity>(exprerss.Body))
				.IsNotNull();
		}
		public static SqlExpression IsNotNull<T>(
			Expression<Func<T, object>> exprerss)
			where T : IEntity
		{
			return DataUtility.ToSqlExpression<T>(exprerss).IsNotNull();
		}
		#endregion

		#region StartWith
		public SqlExpression StartWith(string value)
		{
			Operator = SqlOperator.StartWith;
			SetValue(value);
			return this;
		}
		#endregion

		#region NotStartWith
		public SqlExpression NotStartWith(string value)
		{
			Operator = SqlOperator.NotStartWith;
			SetValue(value);
			return this;
		}
		#endregion

		#region EndWith
		public SqlExpression EndWith(string value)
		{
			Operator = SqlOperator.EndWith;
			SetValue(value);
			return this;
		}
		#endregion

		#region NotEndWith
		public SqlExpression NotEndWith(string value)
		{
			Operator = SqlOperator.NotEndWith;
			SetValue(value);
			return this;
		}
		#endregion

		#region SetOperator
		public SqlExpression SetOperator(SqlOperator opt)
		{
			Operator = opt;
			return this;
		}
		#endregion
		#endregion

		#region GetXxx
		public string GetString()
		{
			if(this.Format != SqlDataFormat.String) { 
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return StringValue;
		}

		public int GetInteger()
		{
			if(this.Format != SqlDataFormat.Integer) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return IntegerValue;
		}

		public decimal GetDecimal()
		{
			if(this.Format != SqlDataFormat.Decimal) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return DecimalValue;
		}

		public long GetLong()
		{
			if(this.Format != SqlDataFormat.Long) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return LongValue;
		}

		public float GetFloat()
		{
			if(this.Format != SqlDataFormat.Float) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return FloatValue;
		}

		public bool GetBoolean()
		{
			if(this.Format != SqlDataFormat.Boolean) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return BooleanValue;
		}

		public char GetChar()
		{
			if(this.Format != SqlDataFormat.Char) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return CharValue;
		}

		public byte GetByte()
		{
			if(this.Format != SqlDataFormat.Byte) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return ByteValue;
		}

		public DateTime GetDateTime()
		{
			if(this.Format != SqlDataFormat.DateTime) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return DateTimeValue;
		}

		public string[] GetStringArray()
		{
			if(this.Format != SqlDataFormat.StringArray) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return StringArrayValue;
		}

		public int[] GetIntegerArray()
		{
			if(this.Format != SqlDataFormat.IntegerArray) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return IntegerArrayValue;
		}

		public IEntity[] GetEntityArray()
		{
			if(this.Format != SqlDataFormat.EntityArray) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return EntityArrayValue;
		}

		public Color GetColor()
		{
			if(this.Format != SqlDataFormat.Color) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return ColorValue;
		}

		public Column GetColumn()
		{
			if(this.Format != SqlDataFormat.Column) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return ColumnValue;
		}

		public Sql GetSql()
		{
			if(this.Format != SqlDataFormat.Sql) {
				throw new Exception(DataConstants.Error.Message.InvalidType);
			}
			return SqlValue;
		}
		#endregion

		#region SetValue
		public void SetValue(string value)
		{
			Format = SqlDataFormat.String;
			StringValue = value;
		}

		public void SetValue(int value)
		{
			Format = SqlDataFormat.Integer;
			IntegerValue = value;
		}

		public void SetValue(decimal value)
		{
			Format = SqlDataFormat.Decimal;
			DecimalValue = value;
		}

		public void SetValue(long value)
		{
			Format = SqlDataFormat.Long;
			LongValue = value;
		}

		public void SetValue(float value)
		{
			Format = SqlDataFormat.Float;
			FloatValue = value;
		}

		public void SetValue(bool value)
		{
			Format = SqlDataFormat.Boolean;
			BooleanValue = value;
		}

		public void SetValue(char value)
		{
			Format = SqlDataFormat.Char;
			CharValue = value;
		}

		public void SetValue(byte value)
		{
			Format = SqlDataFormat.Byte;
			ByteValue = value;
		}

		public void SetValue(DateTime value)
		{
			Format = SqlDataFormat.DateTime;
			DateTimeValue = value;
		}

		private void SetValues(string[] values)
		{
			Format = SqlDataFormat.StringArray;
			StringArrayValue = values;
		}

		private void SetValues(int[] values)
		{
			Format = SqlDataFormat.IntegerArray;
			IntegerArrayValue = values;
		}

		private void SetValues(IEntity[] values)
		{
			Format = SqlDataFormat.EntityArray;
			EntityArrayValue = values;
		}

		public void SetValue(Color value)
		{
			Format = SqlDataFormat.Color;
			ColorValue = value; // ToArgb()
		}

		public void SetValue(Column column)
		{
			Format = SqlDataFormat.Column;
			ColumnValue = column;
		}

		public void SetValue(Sql value)
		{
			if(
				Checker.IsNull(value.SelectColumnNames) 
				&& 
				Checker.IsNull(value.SelectDistinctColumnNames)) {
				value.Select(value.Schema.KeyName);
			}

			Format = SqlDataFormat.Sql;
			SqlValue = value;
		}
		#endregion

		#region IsNullValue
		public bool IsNullValue
		{
			get
			{
				switch(Format) {
					case SqlDataFormat.String:
						return null == GetString();
					case SqlDataFormat.Integer:
						return false;
					case SqlDataFormat.Decimal:
						return false;
					case SqlDataFormat.Long:
						return false;
					case SqlDataFormat.Float:
						return false;
					case SqlDataFormat.Boolean:
						return false;
					case SqlDataFormat.Char:
						return false;
					case SqlDataFormat.Byte:
						return false;
					case SqlDataFormat.DateTime:
						DateTime dt = GetDateTime();
						return 
							DateTime.MinValue == dt 
							|| 
							DataConstants.Date.Min == dt;
					case SqlDataFormat.StringArray:
						return null == GetStringArray();
					case SqlDataFormat.IntegerArray:
						return null == GetIntegerArray();
					case SqlDataFormat.EntityArray:
						return null == GetEntityArray();
					case SqlDataFormat.Color:
						return null == GetColor();
					case SqlDataFormat.Column:
						return null == GetColumn();
					case SqlDataFormat.Sql:
						return null == GetSql();
					default:
						throw new NotImplementedException();
				}
			}
		}
		#endregion

		#region IWithOperator
		public string GetOperator()
		{
			return GetOperator(Operator);
		}
		#endregion

		#region static
		public static string GetOperator(SqlOperator op)
		{
			switch(op) {
				case SqlOperator.LessThan:
					return " < ";
				case SqlOperator.LessEqualTo:
					return " <= ";
				case SqlOperator.EqualTo:
					return " = ";
				case SqlOperator.GreatEqualTo:
					return " >= ";
				case SqlOperator.GreatThan:
					return " > ";
				case SqlOperator.NotEqualTo:
					return " != ";
				case SqlOperator.Like:
					return " LIKE ";
				case SqlOperator.NotLike:
					return " NOT LIKE ";
				case SqlOperator.In:
					return " IN ";
				case SqlOperator.NotIn:
					return " NOT IN ";
				case SqlOperator.IsNull:
					return " IS NULL ";
				case SqlOperator.IsNotNull:
					return " IS NOT NULL ";
				case SqlOperator.StartWith:
					return " LIKE ";
				case SqlOperator.NotStartWith:
					return " NOT LIKE ";
				case SqlOperator.EndWith:
					return " LIKE ";
				case SqlOperator.NotEndWith:
					return " NOT LIKE ";
				default:
					throw new NotImplementedException();
			}
		}

		public static SqlOperator FromExpressionType(ExpressionType type)
		{
			switch(type) {
				case ExpressionType.Equal:
					return SqlOperator.EqualTo;
				case ExpressionType.GreaterThan:
					return SqlOperator.GreatThan;
				case ExpressionType.GreaterThanOrEqual:
					return SqlOperator.GreatEqualTo;
				case ExpressionType.LessThan:
					return SqlOperator.LessThan;
				case ExpressionType.LessThanOrEqual:
					return SqlOperator.LessEqualTo;
				case ExpressionType.NotEqual:
					return SqlOperator.NotEqualTo;
				case ExpressionType.Add:
				case ExpressionType.AddAssign:
				case ExpressionType.AddAssignChecked:
				case ExpressionType.AddChecked:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.AndAssign:
				case ExpressionType.ArrayIndex:
				case ExpressionType.ArrayLength:
				case ExpressionType.Assign:
				case ExpressionType.Block:
				case ExpressionType.Call:
				case ExpressionType.Coalesce:
				case ExpressionType.Conditional:
				case ExpressionType.Constant:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.DebugInfo:
				case ExpressionType.Decrement:
				case ExpressionType.Default:
				case ExpressionType.Divide:
				case ExpressionType.DivideAssign:
				case ExpressionType.Dynamic:
				case ExpressionType.ExclusiveOr:
				case ExpressionType.ExclusiveOrAssign:
				case ExpressionType.Extension:
				case ExpressionType.Goto:
				case ExpressionType.Increment:
				case ExpressionType.Index:
				case ExpressionType.Invoke:
				case ExpressionType.IsFalse:
				case ExpressionType.IsTrue:
				case ExpressionType.Label:
				case ExpressionType.Lambda:
				case ExpressionType.LeftShift:
				case ExpressionType.LeftShiftAssign:
				case ExpressionType.ListInit:
				case ExpressionType.Loop:
				case ExpressionType.MemberAccess:
				case ExpressionType.MemberInit:
				case ExpressionType.Modulo:
				case ExpressionType.ModuloAssign:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyAssign:
				case ExpressionType.MultiplyAssignChecked:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.New:
				case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
				case ExpressionType.Not:
				case ExpressionType.OnesComplement:
				case ExpressionType.Or:
				case ExpressionType.OrAssign:
				case ExpressionType.OrElse:
				case ExpressionType.Parameter:
				case ExpressionType.PostDecrementAssign:
				case ExpressionType.PostIncrementAssign:
				case ExpressionType.Power:
				case ExpressionType.PowerAssign:
				case ExpressionType.PreDecrementAssign:
				case ExpressionType.PreIncrementAssign:
				case ExpressionType.Quote:
				case ExpressionType.RightShift:
				case ExpressionType.RightShiftAssign:
				case ExpressionType.RuntimeVariables:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractAssign:
				case ExpressionType.SubtractAssignChecked:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Switch:
				case ExpressionType.Throw:
				case ExpressionType.Try:
				case ExpressionType.TypeAs:
				case ExpressionType.TypeEqual:
				case ExpressionType.TypeIs:
				case ExpressionType.UnaryPlus:
				case ExpressionType.Unbox:
				default:
					throw new NotSupportedException();
			}
		}

		public static ExpressionType ToExpressionType(SqlOperator type)
		{
			switch(type) {
				case SqlOperator.LessThan:
					return ExpressionType.LessThan;
				case SqlOperator.LessEqualTo:
					return ExpressionType.LessThanOrEqual;
				case SqlOperator.EqualTo:
					return ExpressionType.Equal;
				case SqlOperator.GreatEqualTo:
					return ExpressionType.GreaterThanOrEqual;
				case SqlOperator.GreatThan:
					return ExpressionType.GreaterThan;
				case SqlOperator.NotEqualTo:
					return ExpressionType.NotEqual;
				case SqlOperator.Like:
				case SqlOperator.NotLike:
				case SqlOperator.In:
				case SqlOperator.NotIn:
				case SqlOperator.IsNull:
				case SqlOperator.IsNotNull:
				case SqlOperator.StartWith:
				case SqlOperator.NotStartWith:
				case SqlOperator.EndWith:
				case SqlOperator.NotEndWith:
				default:
					throw new NotSupportedException();
			}
		}

		public static SqlExpression Build<T>(
			Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return DataUtility.ToSqlExpression<T>(expression);
		}
		#endregion
	}
}
