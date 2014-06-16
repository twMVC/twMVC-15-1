// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ValidationDate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Text;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ValidationDate : Attribute, ICloneable<ValidationDate>, IValidation
	{
		#region constants
		public static readonly string _Js = @"function " + typeof(ValidationDate).Name + @"() {
		var args = $.makeArray(arguments);
		var NOT_DATE = 'Not a valid date';
		var boundary = args.shift();
		var allowNull = args.shift();
		var i = new Date(args[0]), j = new Date(args[1]);
		switch(boundary) {
			case 'Between':
				return function(val){
					var d = new Date(val);
					return isNaN(d)
						? val+NOT_DATE
						: (i<=d&&d<=j) ? '' : 'Date must between '+i+' and '+j;
				};break;
			case 'Besides':
				return function(val){
					var d = new Date(val);
					return isNaN(d)
						? val+NOT_DATE
						: (i>d||d<j) ? '' : 'Date must beside '+i+' and '+j;
				};break;
			case 'Greater':
				return function(val){
					var d = new Date(val);
					return isNaN(d)
						? val+NOT_DATE
						: (d>i) ? '' : 'Date must greater than '+i;
				};break;
			case 'Smaller':
				return function(val){
					var d = new Date(val);
					return isNaN(d)
						? val+NOT_DATE
						: (d<i) ? '' : 'Date must smaller than '+i;
				};break;
			default:throw 'not support type :' + boundary;
		}
	}
";
		#endregion

		#region constructor
		public ValidationDate()
		{
		}

		public ValidationDate(BoundaryDate boundary, params string[] dateTimes)
		{
			bool hasError = false;
			switch(boundary) {
				case BoundaryDate.Between:
				case BoundaryDate.Besides:
					hasError = dateTimes.Length < 2;
					break;
				case BoundaryDate.GreaterThan:
				case BoundaryDate.SmallerThan:
					hasError = dateTimes.Length < 1;
					break;
				default:
					throw new NotSupportedException();
			}

			if(hasError) {
				string msg = "Invalide parameter";
				Logger.Error(
					msg,
					Any.Concat
					(
						Any.Concat
						(
							new Any("DateTimeBoundary", boundary.ToString()),
							new Any("dateTimes.Length", dateTimes.Length.ToString())
						),
						dateTimes.ToAnys()
					)
				);
				throw new ArgumentException(msg);
			}

			List<DateTime> list = new List<DateTime>();
			foreach(string dateTime in dateTimes) {
				list.Add(dateTime.AirBagToDateTime());
			}

			this.Boundary = boundary;
			this.DateTimes = list.ToArray();
		}
		#endregion

		#region property
		public Column Column { get; internal set; }
		public BoundaryDate Boundary { get; set; }
		public DateTime[] DateTimes { get; set; }
		#endregion

		#region ICloneable<ValidationDate>
		public ValidationDate Clone()
		{
			ValidationDate clone = new ValidationDate();
			clone.Column = Column;
			clone.Boundary = Boundary;
			clone.DateTimes = DateTimes;
			return clone;
		}
		#endregion

		#region IValidation
		public string JsFunction { get { return _Js; } }

		public string JsValidate()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(this.GetType().Name)
				.Append("(")
				.Append(Formator.JavaScriptEncode(Boundary.ToString()));

			foreach(var item in DateTimes) {
				sb.Append(",").Append(Formator.JavaScriptEncode(item.yyyyMMddHHmmss()));
			}
			return sb.Append(")").ToString();
		}

		public Result Validate(Column column, object value)
		{
			Result result = new Result(true);
			string val = ((DateTime)value).yyyyMMddHHmmssfff();
			if(Checker.IsNull(val.AirBagToDateTime()) && !column.Spec.NotAllowNull) { return result; }
			return Validate(val);
		}

		public Result Validate(string value)
		{
			DateTime val;

			if(value is string) {
				string v = value as string;
				if(!DateTime.TryParse(v, out val)) {
					return new Result() {
						Success = false,
						Message = String.Format("Value \"{0}\" is not DateTime.", v)
					};
				}
			} else {
				val = value.AirBagToDateTime();
			}

			Result result = new Result(true);
			switch(Boundary) {
				case BoundaryDate.Between:
					result.Success = val.Between(
						DateTimes[0],
						DateTimes[1]
					);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"DateTime must between {0} and {1}.",
							DateTimes[0].yyyyMMddHHmmssfff(),
							DateTimes[1].yyyyMMddHHmmssfff()
						);
					break;
				case BoundaryDate.Besides:
					result.Success = val.Besides(
						DateTimes[0],
						DateTimes[1]
					);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"DateTime Can not between {0} and {1}.",
							DateTimes[0].yyyyMMddHHmmssfff(),
							DateTimes[1].yyyyMMddHHmmssfff()
						);
					break;
				case BoundaryDate.GreaterThan:
					result.Success = val.Greater(DateTimes[0]);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"DateTime must greater or equal than {0}.",
							DateTimes[0].yyyyMMddHHmmssfff()
						);
					break;
				case BoundaryDate.SmallerThan:
					result.Success = val.Smaller(DateTimes[0]);
					result.Message = result.Success
						? string.Empty
						: String.Format(
							"DateTime must smaller or equal than {0}.",
							DateTimes[0].yyyyMMddHHmmssfff()
						);
					break;
			}

			return result;
		}
		#endregion
	}
}
