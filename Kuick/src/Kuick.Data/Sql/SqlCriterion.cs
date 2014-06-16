// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlCriterion.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;

namespace Kuick.Data
{
	public class SqlCriterion : IWithOperator, ICloneable<SqlCriterion>
	{
		#region constructor
		public SqlCriterion()
			: this(SqlLogic.And)
		{
		}

		public SqlCriterion(SqlLogic logic)
		{
			this.Logic = logic;
			this.IsNot = false;
			this.Criteria = new List<SqlCriterion>();
			this.Expressions = new List<SqlExpression>();
			this.Literals = new List<SqlLiteral>();
		}
		#endregion

		#region ICloneable<T>
		public SqlCriterion Clone()
		{
			SqlCriterion clone = new SqlCriterion();
			return Clone<SqlCriterion>(clone);
		}
		public K Clone<K>(K clone) where K : SqlCriterion
		{
			clone.Logic = this.Logic;
			clone.Criteria = this.Criteria;
			clone.Expressions = this.Expressions;
			clone.Literals = this.Literals;
			return clone;
		}
		#endregion

		#region property
		public SqlLogic Logic { get; internal set; }
		public bool IsNot { get; protected set; }
		public List<SqlCriterion> Criteria { get; protected set; }
		public List<SqlExpression> Expressions { get; protected set; }
		public List<SqlLiteral> Literals { get; protected set; }
		#endregion

		#region static Operstor
		public static SqlCriterion And()
		{
			return new SqlCriterion(SqlLogic.And);
		}
		public static SqlCriterion Or()
		{
			return new SqlCriterion(SqlLogic.Or);
		}
		#endregion

		#region Operator
		#region Where
		public SqlCriterion Where(params SqlCriterion[] criteria)
		{
			Criteria.AddRange(criteria);
			return this;
		}
		public SqlCriterion Where(params SqlExpression[] expressions)
		{
			Expressions.AddRange(expressions);
			return this;
		}
		public SqlCriterion Where(string columnName, string value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, int value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, decimal value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, long value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, float value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, bool value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, char value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, byte value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, DateTime value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, params string[] values)
		{
			if(null == values) { return this; }
			if(values.Length == 1) {
				Expressions.Add(SqlExpression.Column(columnName).EqualTo(values[0]));
			} else {
				Expressions.Add(SqlExpression.Column(columnName).In(values));
			}
			return this;
		}
		public SqlCriterion Where(string columnName, params int[] values)
		{
			if(null == values) { return this; }
			if(values.Length == 1) {
				Expressions.Add(SqlExpression.Column(columnName).EqualTo(values[0]));
			} else {
				Expressions.Add(SqlExpression.Column(columnName).In(values));
			}
			return this;
		}
		public SqlCriterion Where(string columnName, Color value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, Column value)
		{
			Expressions.Add(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public SqlCriterion Where(string columnName, Sql value)
		{
			Expressions.Add(SqlExpression.Column(columnName).In(value));
			return this;
		}
		public SqlCriterion Where<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			Expressions.Add(SqlExpression.Build<T>(expression));
			return this;
		}
		#endregion

		#region WhereRange
		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			string value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			int value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			decimal value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			long value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			float value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			char value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			byte value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange(
			string smallColumnName,
			string bigColumnName,
			DateTime value)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(smallColumnName).LessEqualTo(value),
					SqlExpression.Column(bigColumnName).GreatEqualTo(value)
				);
			return Where(c);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			string value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			int value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			decimal value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			long value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			float value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			char value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			byte value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}

		public SqlCriterion WhereRange<T>(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			DateTime value)
			where T : IEntity
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			return WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
		}
		#endregion

		#region WhereBetween
		public SqlCriterion WhereBetween(
			string columnName, string smallValue, string bigValue)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, int smallValue, int bigValue)
		{
			Formator.Switch(ref smallValue, ref bigValue);
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, decimal smallValue, decimal bigValue)
		{
			Formator.Switch(ref smallValue, ref bigValue);
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, long smallValue, long bigValue)
		{
			Formator.Switch(ref smallValue, ref bigValue);
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, float smallValue, float bigValue)
		{
			Formator.Switch(ref smallValue, ref bigValue);
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, char smallValue, char bigValue)
		{
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, byte smallValue, byte bigValue)
		{
			Formator.Switch(ref smallValue, ref bigValue);
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		public SqlCriterion WhereBetween(
			string columnName, DateTime smallValue, DateTime bigValue)
		{
			Formator.Switch(ref smallValue, ref bigValue);
			SqlCriterion c = SqlCriterion
				.And()
				.Where(
					SqlExpression.Column(columnName).GreatEqualTo(smallValue),
					SqlExpression.Column(columnName).LessEqualTo(bigValue)
				);
			return Where(c);
		}
		#endregion

		#region WhereEnum
		public SqlCriterion WhereEnum<E>(
			string columnName, int enumValue) where E : struct
		{
			Type type = typeof(E);
			if(!type.IsEnum) {
				throw new ArgumentException("Only enum type allowed.");
			}

			SqlCriterion c = SqlCriterion.Or();

			// BelongsTo
			string[] belongsTo = Checker.Flag.BelongsTo(type, enumValue);
			foreach(string x in belongsTo) {
				c
					.Where(SqlExpression.Column(columnName).EqualTo(x))
					.Where(SqlExpression.Column(columnName).Like(x + ", "))
					.Where(SqlExpression.Column(columnName).Like(", " + x));
			}

			// CombinedBy
			string[] combinedBy = Checker.Flag.CombinedBy(type, enumValue);
			if(!Checker.IsNull(combinedBy)) {
				SqlCriterion and = SqlCriterion.Or();
				foreach(string x in combinedBy) {
					SqlCriterion or = SqlCriterion
						.Or()
						.Where(SqlExpression.Column(columnName).EqualTo(x))
						.Where(SqlExpression.Column(columnName).Like(x + ", "))
						.Where(SqlExpression.Column(columnName).Like(", " + x));
					and.Where(or);
				}
				c.Where(and);
			}
			return Where(c);
		}
		#endregion

		#region WhereCommand
		public SqlCriterion WhereCommand(string command)
		{
			Literals.Add(SqlLiteral.BuildCommanding(command));
			return this;
		}
		public SqlCriterion Where(string command)
		{
			Literals.Add(SqlLiteral.BuildCommanding(command));
			return this;
		}
		#endregion

		#region Like
		public SqlCriterion Like<T>(
			Expression<Func<T, object>> field, string keyword)
			where T : IEntity
		{
			SqlExpression exp = SqlExpression.Column<T>(field).Like(keyword);
			Where(exp);
			return this;
		}
		#endregion

		#region SetLogic
		public SqlCriterion SetLogic(SqlLogic logic)
		{
			Logic = logic;
			return this;
		}
		#endregion

		#region SetNot
		public SqlCriterion SetNot()
		{
			IsNot = true;
			return this;
		}
		#endregion
		#endregion

		#region IWithOperator
		public string GetOperator()
		{
			return GetOperator(Logic);
		}
		#endregion

		#region static
		public static string GetOperator(SqlLogic logic)
		{
			switch(logic) {
				case SqlLogic.And:
					return " AND ";
				case SqlLogic.Or:
					return " OR ";
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}

	public class SqlCriterion<T>
		 : SqlCriterion
		where T : class, IEntity, new()
	{
		#region constructor
		public SqlCriterion()
			: base(SqlLogic.And)
		{
		}

		public SqlCriterion(SqlLogic logic)
			: base(logic)
		{
		}
		#endregion

		#region ICloneable<T>
		public new SqlCriterion<T> Clone()
		{
			SqlCriterion<T> clone = new SqlCriterion<T>();
			clone = base.Clone<SqlCriterion<T>>(clone);
			return clone;
		}
		public new K Clone<K>(K clone) where K : SqlCriterion<T>
		{
			clone = base.Clone<K>(clone);
			return clone;
		}
		#endregion

		#region property
		#endregion

		#region static Operstor
		public static new SqlCriterion<T> And()
		{
			return new SqlCriterion<T>(SqlLogic.And);
		}
		public static new SqlCriterion<T> Or()
		{
			return new SqlCriterion<T>(SqlLogic.Or);
		}
		#endregion

		#region Operator
		#region Where
		public new SqlCriterion<T> Where(params SqlCriterion[] criteria)
		{
			base.Where(criteria);
			return this;
		}
		public new SqlCriterion<T> Where(params SqlExpression[] expressions)
		{
			base.Where(expressions);
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, string value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, int value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, decimal value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, long value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, float value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, bool value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, char value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, byte value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, DateTime value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, params string[] values)
		{
			base.Where(columnName, values);
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, params int[] values)
		{
			base.Where(columnName, values);
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, Color value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}
		public new SqlCriterion<T> Where(string columnName, Column value)
		{
			base.Where(SqlExpression.Column(columnName).EqualTo(value));
			return this;
		}

		public SqlCriterion<T> Where(string columnName, Sql<T> value)
		{
			base.Where(columnName, value);
			return this;
		}

		public SqlCriterion<T> Where(Expression<Func<T, object>> expression)
		{
			base.Where(DataUtility.ToSqlCriterion<T>(expression));
			return this;
		}
		#endregion

		#region WhereRange
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, string value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, int value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, decimal value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, long value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, float value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, char value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, byte value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new SqlCriterion<T> WhereRange(
			string smallColumnName, string bigColumnName, DateTime value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			string value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			int value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			decimal value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			long value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			float value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			char value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			byte value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public SqlCriterion<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			DateTime value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}
		#endregion

		#region WhereBetween
		public new SqlCriterion<T> WhereBetween(
			string columnName, string smallValue, string bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, int smallValue, int bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, decimal smallValue, decimal bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, long smallValue, long bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, float smallValue, float bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, char smallValue, char bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, byte smallValue, byte bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new SqlCriterion<T> WhereBetween(
			string columnName, DateTime smallValue, DateTime bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		#endregion

		#region WhereEnum
		public new SqlCriterion<T> WhereEnum<E>(
			string columnName, int enumValue) where E : struct
		{
			base.WhereEnum<E>(columnName, enumValue);
			return this;
		}
		#endregion

		#region WhereCommand
		public new SqlCriterion<T> WhereCommand(string command)
		{
			base.WhereCommand(command);
			return this;
		}
		#endregion

		#region WhereIsNullOrEmpty
		public SqlCriterion<T> WhereIsNullOrEmpty(
			Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn<T>(expression);

			SqlCriterion<T> c = new SqlCriterion<T>(SqlLogic.Or)
				.Where(column.Spec.ColumnName, string.Empty)
				.Where(new SqlExpression(column).IsNull());

			base.Where(c);
			return this;
		}
		#endregion

		#region Like
		public SqlCriterion<T> Like(
			Expression<Func<T, object>> field, string keyword)
		{
			SqlExpression exp = SqlExpression.Column<T>(field).Like(keyword);
			Where(exp);
			return this;
		}
		#endregion

		#region SetLogic
		public new SqlCriterion<T> SetLogic(SqlLogic logic)
		{
			base.SetLogic(logic);
			return this;
		}
		#endregion
		#endregion

		#region IWithOperator
		public new string GetOperator()
		{
			return GetOperator(Logic);
		}
		#endregion

		#region static
		public new static string GetOperator(SqlLogic logic)
		{
			switch(logic) {
				case SqlLogic.And:
					return " AND ";
				case SqlLogic.Or:
					return " OR ";
				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
