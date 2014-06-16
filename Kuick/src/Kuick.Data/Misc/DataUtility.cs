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
using System.Collections.Generic;

namespace Kuick.Data
{
	public class DataUtility
	{
		public static object GetValue(MemberExpression expression)
		{
			var objectMember = Expression.Convert(expression, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember);
			var getter = getterLambda.Compile();
			return getter();
		}

		public static object GetValue(Expression expression)
		{
			object value = null;

			try {
				var objectMember = Expression.Convert(expression, typeof(object));
				var getterLambda = Expression.Lambda<Func<object>>(objectMember);
				var getter = getterLambda.Compile();
				value = getter();
				return value;
			} catch(Exception ex) {
				Logger.Error("DataUtility.GetValue", ex);
			}


			// ConstantExpression
			ConstantExpression constExp = expression as ConstantExpression;
			if(null != constExp) { return constExp.Value; }

			// MemberExpression
			MemberExpression memExp = expression as MemberExpression;
			if(null != memExp) {
				value = GetValue(memExp);
				return value;
			}

			// NewExpression
			NewExpression newExp = expression as NewExpression;
			if(null != newExp) {
				List<object> list = new List<object>();
				foreach(Expression exp in newExp.Arguments) {
					object obj = GetValue(exp);
					list.Add(obj);
				}
				value = newExp.Constructor.Invoke(list.ToArray());
				return value;
			}

			// MethodCallExpression
			MethodCallExpression methodCallExp = expression as MethodCallExpression;
			if(null != methodCallExp) {
				ConstantExpression ce = methodCallExp.Arguments[0] as ConstantExpression;
				if(ce != null) {
					value = ce.Value.ToString();
					return value;
				}

				MemberExpression me = methodCallExp.Arguments[0] as MemberExpression;
				if(me != null) {
					value = GetValue(me);
					return value;
				}

				switch(methodCallExp.Method.Name) {
					case "Contains":
						value = " like '%" + value + "%'";
						return value;
					case "StartsWith":
						value = " like '" + value + "%'";
						return value;
					case "EndsWith":
						value = " like '%" + value + "'";
						return value;
					default:
						Logger.Error(
							"Kuick.Data.DataUtility.GetValue",
							"Unhandled Method of MethodCallExpression.",
							new Any("Expression", methodCallExp.ToString()),
							new Any("Method Name", methodCallExp.Method.Name)
						);
						throw new NotImplementedException();
				}
			}

			// BinaryExpression
			BinaryExpression binaryExp = expression as BinaryExpression;
			if(null != binaryExp) {
				object left = GetValue(binaryExp.Left);
				object right = GetValue(binaryExp.Right);

				// String
				if(binaryExp.Type.IsString()) {
					value = string.Concat(left, right);
					return value;
				}

				// Integer
				if(binaryExp.Type.IsInteger()) {
					switch(binaryExp.NodeType) {
						case ExpressionType.Add:
							value = (int)left + (int)right;
							break;
						case ExpressionType.Subtract:
							value = (int)left - (int)right;
							break;
						case ExpressionType.Negate:
							value = -(int)left;
							break;
						case ExpressionType.Multiply:
							value = (int)left * (int)right;
							break;
						case ExpressionType.Increment:
							value = (int)left + 1;
							break;
						case ExpressionType.Divide:
							value = (int)left / (int)right;
							break;
						default:
							Logger.Error(
								"Kuick.Data.DataUtility.GetValue",
								"Unhandled NodeType of BinaryExpression.",
								new Any("Expression", binaryExp.ToString()),
								new Any("Value Type", binaryExp.Type.Name),
								new Any("Node Type", binaryExp.NodeType)
							);
							throw new NotImplementedException();
					}
					return value;
				}

				// Boolean
				if(binaryExp.Type.IsBoolean()) {
					switch(binaryExp.NodeType) {
						case ExpressionType.And:
						case ExpressionType.AndAlso:
							value = (bool)left && (bool)right;
							break;
						case ExpressionType.Or:
						case ExpressionType.OrAssign:
							value = (bool)left || (bool)right;
							break;
						case ExpressionType.IsTrue:
							value = null == left ? (bool)right : (bool)left;
							break;
						case ExpressionType.IsFalse:
							value = null == left ? !(bool)right : !(bool)left;
							break;
						default:
							Logger.Error(
								"Kuick.Data.DataUtility.GetValue",
								"Unhandled NodeType of BinaryExpression.",
								new Any("Expression", binaryExp.ToString()),
								new Any("Value Type", binaryExp.Type.Name),
								new Any("Node Type", binaryExp.NodeType)
							);
							throw new NotImplementedException();
					}
					return value;
				}
			}

			// UnaryExpression
			UnaryExpression unaryExp = expression as UnaryExpression;
			if(null != unaryExp) {
				MemberExpression memberUnaryExp = unaryExp.Operand as MemberExpression;
				if(null != memberUnaryExp) {
					return GetValue(memberUnaryExp);
				}
			}

			Logger.Error(
				"Kuick.Data.DataUtility.GetValue",
				"Unhandled Expression Type.",
				new Any("Type Name", expression.GetType().Name)
			);
			throw new NotImplementedException();
		}

		public static SqlCriterion<T> ToSqlCriterion<T>(
			Expression<Func<T, object>> expression)
			where T : class, IEntity, new()
		{
			return ToSqlCriterionMain<T>(expression.Body);
		}

		public static SqlCriterion<T> ToSqlCriterionMain<T>(Expression expression)
			where T : class, IEntity, new()
		{
			SqlCriterion<T> c = new SqlCriterion<T>();

			// UnaryExpression
			UnaryExpression unaryExp = expression as UnaryExpression;
			if(null != unaryExp) {
				if(unaryExp.NodeType == ExpressionType.Not) {
					c.SetNot();
				}
				c.Criteria.Add(ToSqlCriterionMain<T>(unaryExp.Operand));
				//BinaryExpression binaryExp = (BinaryExpression)unaryExpression.Operand;
				//Column column = ToColumn<T>(binaryExp.Left);
				//SqlOperator opt = SqlExpression.FromExpressionType(binaryExp.NodeType);
				//object value = GetValue(binaryExp.Right);
				//c.Where(ToSqlExpression(column, opt, value));
				return c;
			}

			// MemberExpression
			MemberExpression memberExp = expression as MemberExpression;
			if(null != memberExp) {
				Column column = ToColumn<T>(expression);
				SqlExpression sqlExp = SqlExpression.Column(column);
				c.Where(sqlExp);
				return c;
			}

			// BinaryExpression
			BinaryExpression binaryExp = expression as BinaryExpression;
			if(null != binaryExp) {
				BinaryExpression innerBinaryExp = binaryExp.Left as BinaryExpression;
				if(null == innerBinaryExp) {
					Column column = ToColumn<T>(binaryExp.Left);
					ExpressionType type = expression.NodeType;
					SqlOperator opt = SqlExpression.FromExpressionType(type);
					object value = GetValue(binaryExp.Right);
					c.Where(ToSqlExpression(column, opt, value));
					return c;
				} else {
					switch(expression.NodeType) {
						case ExpressionType.And:
							c.Logic = SqlLogic.And;
							break;
						case ExpressionType.Or:
							c.Logic = SqlLogic.Or;
							break;
						default:
							Logger.Error(
								"Kuick.Data.DataUtility.ToSqlCriterion",
								"Unhandled NodeType of BinaryExpression.",
								new Any("Expression", binaryExp.ToString()),
								new Any("Node Type", expression.NodeType)
							);
							throw new NotImplementedException();
					}
					c.Where(ToSqlCriterionMain<T>(binaryExp.Left));
					c.Where(ToSqlCriterionMain<T>(binaryExp.Right));
					return c;
				}
			}

			// 
			throw new NotImplementedException(string.Format(
				"Not implemented the Expression type of '{0}'.",
				expression.GetType().Name
			));
		}

		public static SqlExpression ToSqlExpression(
			Column column, SqlOperator opt, object value)
		{
			SqlExpression sqlExp = SqlExpression.Column(column).SetOperator(opt);
			switch(column.Spec.DbType) {
				case SqlDataType.Unknown:
					break;
				case SqlDataType.Boolean:
					sqlExp.SetValue((bool)value);
					break;
				case SqlDataType.Bit:
					sqlExp.SetValue((bool)value);
					break;
				case SqlDataType.Long:
					sqlExp.SetValue((Int64)value);
					break;
				case SqlDataType.Decimal:
					sqlExp.SetValue((decimal)value);
					break;
				case SqlDataType.Double:
					sqlExp.SetValue((float)Convert.ToSingle(value));
					break;
				case SqlDataType.Integer:
					sqlExp.SetValue((int)value);
					break;
				case SqlDataType.Enum:
					string eName = Enum.GetName(column.Property.PropertyType, (int)value);
					sqlExp.SetValue(eName);
					break;
				case SqlDataType.MaxVarBinary:
					throw new NotImplementedException();
				case SqlDataType.TimeStamp:
					sqlExp.SetValue((DateTime)value);
					break;
				case SqlDataType.MaxVarChar:
				case SqlDataType.MaxVarWChar:
				case SqlDataType.Char:
				case SqlDataType.VarChar:
				case SqlDataType.VarWChar:
				case SqlDataType.WChar:
					if(null == value) {
						if(opt == SqlOperator.EqualTo) {
							sqlExp.SetOperator(SqlOperator.IsNull);
						}
						if(opt == SqlOperator.NotEqualTo) {
							sqlExp.SetOperator(SqlOperator.IsNotNull);
						}
					} else {
						sqlExp.SetValue((string)value);
					}
					break;
				case SqlDataType.Uuid:
					sqlExp.SetValue((string)value);
					break;
				case SqlDataType.Identity:
					sqlExp.SetValue((int)value);
					break;
				default:
					throw new NotImplementedException();
			}
			return sqlExp;
		}

		public static SqlExpression ToSqlExpression<T>(
			Expression<Func<T, object>> expression)
			where T : IEntity
		{
			// UnaryExpression
			UnaryExpression unaryExp = expression.Body as UnaryExpression;
			if(null != unaryExp) {
				BinaryExpression innerBinaryExp = (BinaryExpression)unaryExp.Operand;
				Column column = ToColumn<T>(innerBinaryExp.Left);
				ExpressionType type = innerBinaryExp.NodeType;
				SqlOperator opt = SqlExpression.FromExpressionType(type);
				object value = GetValue(innerBinaryExp.Right);
				return ToSqlExpression(column, opt, value);
			}

			// MemberExpression
			MemberExpression memberExp = expression.Body as MemberExpression;
			if(null != memberExp) {
				Column column = ToColumn<T>(expression.Body);
				SqlExpression sqlExpression = SqlExpression.Column(column);
				return sqlExpression;
			}

			// BinaryExpression
			BinaryExpression binaryExp = expression.Body as BinaryExpression;
			if(null != binaryExp) {
				Column column = ToColumn<T>(binaryExp.Left);
				ExpressionType type = expression.Body.NodeType;
				SqlOperator opt = SqlExpression.FromExpressionType(type);
				object value = GetValue(binaryExp.Right);
				return ToSqlExpression(column, opt, value);
			}

			throw new NotImplementedException(string.Format(
				"Not implemented the Expression type of '{0}'.",
				expression.GetType().Name
			));
		}

		public static SqlSet ToSqlSet(Column column, SqlOperator opt, object value)
		{
			SqlSet sqlSet = SqlSet.Column(column);
			switch(column.Spec.DbType) {
				case SqlDataType.Unknown:
					break;
				case SqlDataType.Boolean:
					sqlSet.SetValue((bool)value);
					break;
				case SqlDataType.Char:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.Bit:
					sqlSet.SetValue((byte)value);
					break;
				case SqlDataType.Long:
					sqlSet.SetValue((Int64)value);
					break;
				case SqlDataType.Decimal:
					sqlSet.SetValue((decimal)value);
					break;
				case SqlDataType.Double:
					sqlSet.SetValue((long)value);
					break;
				case SqlDataType.Integer:
					sqlSet.SetValue((int)value);
					break;
				case SqlDataType.Enum:
					sqlSet.SetValue((int)value);
					break;
				case SqlDataType.MaxVarBinary:
					throw new NotImplementedException();
				case SqlDataType.MaxVarChar:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.MaxVarWChar:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.TimeStamp:
					sqlSet.SetValue((DateTime)value);
					break;
				case SqlDataType.VarChar:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.VarWChar:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.WChar:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.Uuid:
					sqlSet.SetValue((string)value);
					break;
				case SqlDataType.Identity:
					sqlSet.SetValue((int)value);
					break;
				default:
					throw new NotImplementedException();
			}
			return sqlSet;
		}

		public static SqlSet ToSqlSet<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return ToSqlSet<T>(expression.Body);
		}

		private static SqlSet ToSqlSet<T>(Expression expression)
			where T : IEntity
		{
			// UnaryExpression
			UnaryExpression unaryExp = expression as UnaryExpression;
			if(null != unaryExp) {
				return ToSqlSet<T>(unaryExp.Operand);
			}

			// MemberExpression
			MemberExpression memberExp = expression as MemberExpression;
			if(null != memberExp) {
				Column column = ToColumn<T>(memberExp);
				SqlSet sqlSet = SqlSet.Column(column);
				return sqlSet;
			}

			// BinaryExpression
			BinaryExpression binaryExp = expression as BinaryExpression;
			if(null != binaryExp) {
				Column column = ToColumn<T>(binaryExp.Left);
				ExpressionType type = binaryExp.NodeType;
				SqlOperator opt = SqlExpression.FromExpressionType(type);
				object value = GetValue(binaryExp.Right);
				return ToSqlSet(column, opt, value);
			}

			//
			Logger.Error(
				"Kuick.Data.DataUtility.ToSqlSet",
				"Unhandled Expression Type.",
				new Any("Type Name", expression.GetType().Name)
			);
			throw new NotImplementedException();
		}

		public static Column ToColumn<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			return ToColumn<T>(expression.Body);
		}

		internal static Column ToColumn<T>(Expression expression)
			where T : IEntity
		{
			if(null == expression) { return null; }

			MemberInfo memberInfo = null;
			MemberExpression memberExp = null;
			if(expression.NodeType == ExpressionType.Convert) {
				memberExp = ((UnaryExpression)expression).Operand as MemberExpression;
				if(null != memberExp) {
					memberInfo = memberExp.Member;
				}
			} else if(expression.NodeType == ExpressionType.MemberAccess) {
				memberExp = expression as MemberExpression;
				if(null != memberExp) {
					memberInfo = memberExp.Member;
				}
			} else if(expression.NodeType == ExpressionType.Call) {
				memberInfo = ((MethodCallExpression)expression).Method;
			} else {
				throw new NotImplementedException();
			}

			T schema = EntityCache.Find<T>();
			PropertyInfo info = memberInfo as PropertyInfo;
			if(null == info) { return null; }
			string name = info.Name == DataConstants.Entity.KeyName
				? Reflector.GetValue(info, schema) as string
				: info.Name;
			Column column = schema.GetColumn(name);

			return column;
		}

		#region FullName vs. AsName
		internal static string FullNameToAsName(
			string fullName)
		{
			return FullNameToAsName(fullName, null);
		}

		internal static string FullNameToAsName(
			string fullName, Func<string, string> unTag)
		{
			if(fullName.Contains(" AS ")) { return fullName; }
			if(!fullName.Contains(".")) { return fullName; }

			string[] parts = fullName.SplitWith(".");
			if(parts.Length != 2) {
				throw new Exception(string.Format(
					"Field fullname({0}) error format.",
					fullName
				));
			}
			string tableName = null == unTag ? parts[0] : unTag(parts[0]);
			string fieldName = null == unTag ? parts[1] : unTag(parts[1]);
			string aliasName = EntityCache.GetAlias(tableName);

			return null == unTag
				? aliasName + fieldName
				: aliasName + unTag(fieldName);
		}

		internal static string FullNameToFullNameAsName(string fullName)
		{
			return FullNameToFullNameAsName(fullName, null, null);
		}

		internal static string FullNameToAliasFullName(
			string fullName, Func<string, string> tag, Func<string, string> unTag)
		{
			if(!fullName.Contains(".")) {
				return null == tag ? fullName : tag(fullName);
			}

			string[] parts = fullName.SplitWith(".");
			if(parts.Length != 2) {
				throw new Exception(string.Format(
					"Field fullname({0}) error format.",
					fullName
				));
			}
			string tableName = null == unTag ? parts[0] : unTag(parts[0]);
			string fieldName = null == unTag ? parts[1] : unTag(parts[1]);
			string aliasName = EntityCache.GetAlias(tableName);

			return null == tag
				? aliasName + "." + fieldName
				: tag(aliasName) + "." + tag(fieldName);
		}

		internal static string FullNameToFullNameAsName(
			string fullName, Func<string, string> tag, Func<string, string> unTag)
		{
			if(fullName.Contains(" AS ")) { return fullName; }
			if(!fullName.Contains(".")) {
				return null == tag ? fullName : tag(fullName);
			}

			string[] parts = fullName.SplitWith(".");
			if(parts.Length != 2) {
				throw new Exception(string.Format(
					"Field fullname({0}) error format.",
					fullName
				));
			}
			string tableName = null == unTag ? parts[0] : unTag(parts[0]);
			string fieldName = null == unTag ? parts[1] : unTag(parts[1]);
			string aliasName = EntityCache.GetAlias(tableName);

			return string.Concat(
				null == tag
					? aliasName + "." + fieldName
					: tag(aliasName) + "." + tag(fieldName), 
				" AS ",
				null == unTag 
					? aliasName + fieldName
					: aliasName + unTag(fieldName)
			);
		}
		#endregion
	}
}
