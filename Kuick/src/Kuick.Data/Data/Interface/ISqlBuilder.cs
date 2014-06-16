// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ISqlBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Drawing;
using System.Data.Common;
using System.Data;

namespace Kuick.Data
{
	public interface ISqlBuilder
	{
		string Vender { get; }

		string OpenTag { get; }
		string CloseTag { get; }
		string ValueMark { get; }
		string Wildcard { get; }
		string UnicodeTag { get; }
		string AssignNullMaxVarChar { get; }
		string ParameterPrefix { get; }
		string ParameterSuffix { get; }
		string BeforeCommand { get; }
		string AfterCommand { get; }

		string Tag(string columnName);
		string UnTag(string columnName);
		string MarkValue(string value);
		string Filter(string value);

		string BuildColumnName(string columnName);
		string BuildLiteral(SqlLiteral literal);

		string BuildExpression(SqlExpression expression);
		string BuildValue(SqlExpression expression);
		string BuildValue(string value);
		string BuildValue(int value);
		string BuildValue(decimal value);
		string BuildValue(long value);
		string BuildValue(float value);
		string BuildValue(bool value);
		string BuildValue(char value);
		string BuildValue(byte value);
		string BuildValue(DateTime value);
		string BuildValue(string[] values);
		string BuildValue(int[] values);
		string BuildValue(IEntity[] values);
		string BuildValue(Color value);
		string BuildValue(Column value);
		string BuildValue(Sql value);

		string BuildSet(SqlSet set);

		string BuildOrderBy(SqlOrderBy orderBy);
		string BuildTopCommand(string command, int top);

		string BuildParameterName(string columnName);
		void AddDbParameter(IDbCommand cmd, Column column, object value);

		string BuildCreateTableCommandText(IEntity schema);
		string BuildCreateIndexCommandText(EntityIndex index);
		string BuildDropIndexCommandText(string tableName, string indexName);
		string BuildAlterColumnCommandText(Column column);
		string BuildAddColumnCommandText(Column column);
	}
}
