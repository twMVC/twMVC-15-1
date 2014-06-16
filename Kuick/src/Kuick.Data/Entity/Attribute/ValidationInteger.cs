// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ValidationInteger.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ValidationInteger : Attribute, ICloneable<ValidationInteger>, IValidation
	{
		#region constants
		public static readonly string _Js = @"function " + typeof(ValidationInteger).Name + @"() {
		var args = $.makeArray(arguments);
		var NOT_INTEGER = ' is not integer';
		var boundary = args.shift();
		var i = args[0], j = args[1];
		switch(boundary) {
			case 'Between':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: (i<=val&&val<=j) ? '' : 'Integer must between '+i+' and '+j;
				}; break;
			case 'Besides':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: (val<i||j<val)
							? '' : 'Integer must beside '+i+' and '+j;
				}; break;
			case 'GreaterThan':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: (val>i)
							? '' : 'Integer must greater than '+i;
				}; break;
			case 'SmallerThan':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: (val<i)
							? '' : 'Integer must smaller than '+i;
				}; break;
			case 'In':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: $.inArray(args)
							? '' : 'Integer must in '+ args;
				}; break;
			case 'Exclude':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: !$.inArray(args)
							? '' : 'Integer must exclude '+ args;
				}; break;
			case 'IsEven':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: (val%2==0)
							? ''
							: 'Integer must be even';
				}; break;
			case 'IsOdd':
				return function(val){
					return isNaN(1*val)
						? val+NOT_INTEGER
						: (val%2==1)
							? ''
							: 'Integer must be odd';
				}; break;
			default:throw 'not support type :' + boundary;
		}
	}
";
		#endregion

		#region constructor
		public ValidationInteger()
		{
		}

		public ValidationInteger(BoundaryInteger boundary, params int[] integers)
		{
			bool hasError = false;

			switch(boundary) {
				case BoundaryInteger.Between:
				case BoundaryInteger.Besides:
					hasError = integers.Length != 2;
					break;
				case BoundaryInteger.GreaterThan:
				case BoundaryInteger.SmallerThan:
					hasError = integers.Length != 1;
					break;
				case BoundaryInteger.In:
				case BoundaryInteger.Exclude:
					hasError = integers.Length == 0;
					break;
				case BoundaryInteger.IsEven:
				case BoundaryInteger.IsOdd:
					hasError = integers.Length != 0;
					break;
				default:
					throw new NotSupportedException();
			}

			if(hasError) {
				string msg = string.Format(
					"Numbers of parameter is not valid for {0}",
					boundary
				);
				Logger.Error(
					msg,
					Any.Concat
					(
						Any.Concat
						(
							new Any("IntegerBoundary", boundary.ToString()),
							new Any("dateTimes.Length", integers.Length.ToString())
						),
						integers.ToAnys()
					)
				);
				throw new ArgumentException(msg);
			}

			this.Boundary = boundary;
			this.Integers = integers;
		}
		#endregion

		#region property
		// Parent
		public Column Column { get; internal set; }
		public BoundaryInteger Boundary { get; internal set; }
		public int[] Integers { get; internal set; }
		#endregion

		#region ICloneable<ValidationInteger>
		public ValidationInteger Clone()
		{
			ValidationInteger clone = new ValidationInteger();
			clone.Column = Column;
			clone.Boundary = Boundary;
			return clone;
		}
		#endregion

		#region IValidation Members
		public string JsFunction { get { return _Js; } }

		public string JsValidate()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.GetType().Name)
				.Append("(")
				.Append(Formator.JavaScriptEncode(Boundary.ToString()));

			if(Integers.Length > 0) {
				sb.Append(",").Append(Integers.ColsIntoStr());
			}
			return sb.Append(")").ToString();
		}

		public Result Validate(Column column, object value)
		{
			Result result = new Result(true);
			string val = value as string;
			if(Checker.IsNull(val) && !column.Spec.NotAllowNull) { return result; }
			return Validate(val);
		}

		public Result Validate(string value)
		{
			int val;
			if(value is string) {
				string v = value as string;
				if(!int.TryParse(v, out val)) {
					return new Result() {
						Success = false,
						Message = String.Format("Value \"{0}\" is not integer.", v)
					};
				}
			} else {
				val = value.AirBagToInt();
			}

			Result result = new Result(true);

			switch(Boundary) {
				case BoundaryInteger.Between:
					result.Success = val.Between(Integers[0], Integers[1]);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"Integer must between {0} and {1}.",
							Integers[0].ToString(),
							Integers[1].ToString()
						);
					break;
				case BoundaryInteger.Besides:
					result.Success = val.Besides(Integers[0], Integers[1]);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"Integer must beside {0} and {1}.",
							Integers[0].ToString(),
							Integers[1].ToString()
						);
					break;
				case BoundaryInteger.GreaterThan:
					result.Success = val.Greater(Integers[0]);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"Integer must greater or equal than {0}.",
							Integers[0].ToString()
						);
					break;
				case BoundaryInteger.SmallerThan:
					result.Success = val.Smaller(Integers[0]);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"Integer must smaller or equal than {0}.",
							Integers[0].ToString()
						);
					break;
				case BoundaryInteger.In:
					result.Success = val.In(Integers);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"Must be one of these following datas {0}.",
							Integers.ColsIntoStr()
						);
					break;
				case BoundaryInteger.Exclude:
					result.Success = val.Exclude(Integers);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"Can not be one of these following datas {0}.",
							Integers.ColsIntoStr()
						);
					break;
				case BoundaryInteger.IsEven:
					result.Success = val.IsEven();
					result.Message = result.Success
						? string.Empty
						: "Must be even value.";
					break;
				case BoundaryInteger.IsOdd:
					result.Success = val.IsOdd();
					result.Message = result.Success
						? string.Empty
						: "Must be odd value.";
					break;
				default:
					throw new NotSupportedException();
			}

			return result;
		}
		#endregion
	}
}
