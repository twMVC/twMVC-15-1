// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;

namespace Kuick.Data
{
	public abstract class SqlBuilder : ISqlBuilder
	{
		#region constructor
		public SqlBuilder()
		{
		}
		#endregion

		#region property
		public abstract string Vender { get; }
		public abstract string OpenTag { get; }
		public abstract string CloseTag { get; }
		public virtual string ValueMark
		{
			get
			{
				return DataConstants.Symbol.Quoter;
			}
		}
		public abstract string Wildcard { get; }
		public virtual string UnicodeTag { get { return "N"; } }
		public virtual string AssignNullMaxVarChar
		{
			get
			{
				return DataConstants.Sql.Null;
			}
		}
		public abstract string ParameterPrefix { get; }
		public abstract string ParameterSuffix { get; }
		public virtual string BeforeCommand { get { return string.Empty; } }
		public virtual string AfterCommand { get { return string.Empty; } }
		#endregion

		#region method
		public string Tag(string columnName)
		{
			if(columnName.Contains(".")) {
				string[] parts = columnName.Split('.');
				StringBuilder sb = new StringBuilder();
				foreach(string part in parts) {
					if(sb.Length > 0) { sb.Append("."); }
					sb.Append(Tag(part));
				}
				return sb.ToString();
			} else {
				return string.Concat(
					columnName.StartsWith(OpenTag) ? string.Empty : OpenTag,
					columnName,
					columnName.StartsWith(CloseTag) ? string.Empty : CloseTag
				);
			}
		}
		public string UnTag(string columnName)
		{
			if(columnName.Contains(".")) {
				string[] parts = columnName.Split('.');
				StringBuilder sb = new StringBuilder();
				foreach(string part in parts) {
					if(sb.Length > 0) { sb.Append("."); }
					sb.Append(UnTag(part));
				}
				return sb.ToString();
			} else {
				return columnName.TrimStart(OpenTag).TrimEnd(CloseTag);
			}
		}
		public string MarkValue(string value)
		{
			return string.Concat(ValueMark, value, ValueMark);
		}
		public string Filter(string value)
		{
			return Checker.IsNull(value)
				? string.Empty
				: value.Replace(
					DataConstants.Symbol.Quoter,
					DataConstants.Symbol.Quoter + DataConstants.Symbol.Quoter
				);
		}

		public string BuildColumnName(string columnName)
		{
			return Tag(columnName);
		}

		public abstract string BuildLiteral(SqlLiteral literal);

		public string BuildExpression(SqlExpression expression)
		{
			// in, not in
			if(expression.Operator.EnumIn(SqlOperator.In, SqlOperator.NotIn)) {
				string value = BuildValue(expression);
				if(Checker.IsNull(value)) {
					Logger.Track(
						"Return 1 = 2 instead when the expression operator is 'in' or 'not in' and values is null.",
						new Any("ColumnName", expression.FullName)
					);
					return "1 = 2";
				}
				return string.Concat(
					DataUtility.FullNameToAliasFullName(
						expression.FullName, Tag, UnTag
					),
					expression.GetOperator(), 
					"(" + value + ")"
				);
			}

			// is null, is not null
			if(expression.Operator.EnumIn(SqlOperator.IsNull, SqlOperator.IsNotNull)) {
				return String.Concat(
					DataUtility.FullNameToAliasFullName(
						expression.FullName, Tag, UnTag
					), 
					expression.GetOperator()
				);
			}

			// others
			return string.Concat(
				DataUtility.FullNameToAliasFullName(
					expression.FullName, Tag, UnTag
				),
				expression.GetOperator(),
				BuildValue(expression)
			);
		}

		public string BuildValue(SqlExpression expression)
		{
			switch(expression.Format) {
				case SqlDataFormat.String:
					if(expression.Operator.EnumIn(
						SqlOperator.Like, SqlOperator.NotLike)) {
						string val = expression.GetString();
						if(string.IsNullOrEmpty(val)) { val = Wildcard; }
						if(
							!val.StartsWith(Wildcard) 
							&& 
							!val.EndsWith(Wildcard)) {
							val = string.Concat(Wildcard, val, Wildcard);
						}
						return BuildValue(val);
					} else {
						return BuildValue(expression.GetString());
					}
				case SqlDataFormat.Integer:
					return BuildValue(expression.GetInteger());
				case SqlDataFormat.Decimal:
					return BuildValue(expression.GetDecimal());
				case SqlDataFormat.Long:
					return BuildValue(expression.GetLong());
				case SqlDataFormat.Float:
					return BuildValue(expression.GetFloat());
				case SqlDataFormat.Boolean:
					return BuildValue(expression.GetBoolean());
				case SqlDataFormat.Char:
					return BuildValue(expression.GetChar());
				case SqlDataFormat.Byte:
					return BuildValue(expression.GetByte());
				case SqlDataFormat.DateTime:
					return BuildValue(expression.GetDateTime());
				case SqlDataFormat.StringArray:
					return BuildValue(expression.GetStringArray());
				case SqlDataFormat.IntegerArray:
					return BuildValue(expression.GetIntegerArray());
				case SqlDataFormat.EntityArray:
					return BuildValue(expression.GetEntityArray());
				case SqlDataFormat.Color:
					return BuildValue(expression.GetColor());
				case SqlDataFormat.Column:
					return BuildValue(expression.GetColumn());
				case SqlDataFormat.Sql:
					return BuildValue(expression.GetSql());
				default:
					throw new NotImplementedException();
			}
		}

		public virtual string BuildValue(string value)
		{
			return value == AssignNullMaxVarChar
				? value
				: string.Concat(UnicodeTag, MarkValue(Filter(value)));
		}
		public virtual string BuildValue(int value)
		{
			return value.ToString();
		}
		public virtual string BuildValue(decimal value)
		{
			return value.ToString();
		}
		public virtual string BuildValue(long value)
		{
			return value.ToString();
		}
		public virtual string BuildValue(float value)
		{
			return value.ToString();
		}
		public virtual string BuildValue(bool value)
		{
			return Formator.Boolean2Bit(value);
		}
		public virtual string BuildValue(char value)
		{
			return MarkValue(value.ToString());
		}
		public virtual string BuildValue(byte value)
		{
			return value.ToString();
		}

		public abstract string BuildValue(DateTime value);

		public virtual string BuildValue(string[] values)
		{
			if(Checker.IsNull(values)) { return string.Empty; }
			return string.Concat(
				DataConstants.Symbol.OpenParenthesis,
				Formator.ColsIntoStr(
					values, DataConstants.Symbol.Comma, ValueMark
				),
				DataConstants.Symbol.CloseParenthesis
			);
		}
		public virtual string BuildValue(int[] values)
		{
			if(Checker.IsNull(values)) { return string.Empty; }
			List<string> list = new List<string>();

			StringBuilder sb = new StringBuilder();
			foreach(int x in values) {
				if(sb.Length > 0) { sb.Append(", "); }
				sb.Append(x.ToString());
			}
			return sb.ToString();
		}
		public virtual string BuildValue(IEntity[] values)
		{
			if(Checker.IsNull(values)) { return string.Empty; }
			string[] pks = values.PrimaryKeys().ToArray();
			return BuildValue(pks);
		}
		public virtual string BuildValue(Color value)
		{
			return value.ToArgb().ToString();
		}
		public virtual string BuildValue(Column value)
		{
			return Tag(value.Spec.ColumnName);
		}
		public virtual string BuildValue(Sql value)
		{
			string command = new SqlParser(this, value, string.Empty)
				.ParseSelect();
			return string.Concat(
				DataConstants.Symbol.OpenParenthesis,
				command,
				DataConstants.Symbol.CloseParenthesis
			);
		}

		public bool BuildSet(SqlSet set, StringBuilder sb)
		{
			string value = string.Empty;

			switch(set.Format) {
				case SqlSetFormat.Value:
					sb.AppendFormat(
						"{0} = {1}", 
						Tag(set.ColumnName), 
						BuildParameterName(set.ColumnName)
					);
					return true;
				case SqlSetFormat.Command:
					sb.Append(set.Command);
					return false;
				case SqlSetFormat.Increasing:
					sb.AppendFormat("{0} = {0} + 1", Tag(set.ColumnName));
					return false;
				case SqlSetFormat.Decreasing:
					sb.AppendFormat("{0} = {0} - 1", Tag(set.ColumnName));
					return false;
				case SqlSetFormat.IsNull:
					sb.AppendFormat("{0} = NULL", Tag(set.ColumnName));
					return false;
				default:
					throw new NotImplementedException();
			}
		}

		public virtual string BuildSet(SqlSet set)
		{
			switch(set.Format) {
				case SqlSetFormat.Value:
					string value = string.Empty;
					switch(set.ValueFormat) {
						case SqlSetValueFormat.String:
							value = BuildValue(set.StringValue);
							break;
						case SqlSetValueFormat.Integer:
							value = BuildValue(set.IntegerValue);
							break;
						case SqlSetValueFormat.Decimal:
							value = BuildValue(set.DecimalValue);
							break;
						case SqlSetValueFormat.Long:
							value = BuildValue(set.LongValue);
							break;
						case SqlSetValueFormat.Float:
							value = BuildValue(set.FloatValue);
							break;
						case SqlSetValueFormat.Boolean:
							value = BuildValue(set.BooleanValue);
							break;
						case SqlSetValueFormat.Char:
							value = BuildValue(set.CharValue);
							break;
						case SqlSetValueFormat.Byte:
							value = BuildValue(set.ByteValue);
							break;
						case SqlSetValueFormat.DateTime:
							value = BuildValue(set.DateTimeValue);
							break;
						case SqlSetValueFormat.Color:
							value = BuildValue(set.ColorValue);
							break;
						case SqlSetValueFormat.Column:
							value = BuildValue(set.ColumnValue);
							break;
						case SqlSetValueFormat.Sql:
							value = BuildValue(set.SqlValue);
							break;
						default:
							throw new NotImplementedException();
					}
					return string.Format("{0} = {1}", Tag(set.ColumnName), value);
				case SqlSetFormat.Command:
					return set.Command;
				case SqlSetFormat.Increasing:
					return string.Format("{0} = {0} + 1", Tag(set.ColumnName));
				case SqlSetFormat.Decreasing:
					return string.Format("{0} = {0} - 1", Tag(set.ColumnName));
				case SqlSetFormat.IsNull:
					return string.Format("{0} = NULL", Tag(set.ColumnName));
				default:
					throw new NotImplementedException();
			}
		}

		public virtual string BuildOrderBy(SqlOrderBy orderBy)
		{
			return String.Concat(
				DataUtility.FullNameToAliasFullName(orderBy.FullName, Tag, UnTag), 
				orderBy.GetOperator()
			);
		}

		public abstract string BuildTopCommand(string command, int top);

		public virtual string BuildParameterName(string columnName)
		{
			return string.Concat(ParameterPrefix, columnName, ParameterSuffix);
		}

		public abstract void AddDbParameter(
			IDbCommand cmd, Column column, object value
		);
		#endregion

		#region Build CommandText
		public abstract string BuildCreateTableCommandText(IEntity schema);
		public abstract string BuildCreateIndexCommandText(EntityIndex index);
		public virtual string BuildDropIndexCommandText(
			string tableName, string indexName)
		{
			string sql = String.Format(
				"DROP INDEX {0}.{1}",
				Tag(tableName),
				Tag(indexName)
			);
			return sql;
		}
		public abstract string BuildAlterColumnCommandText(Column column);
		public abstract string BuildAddColumnCommandText(Column column);
		#endregion
	}
}
