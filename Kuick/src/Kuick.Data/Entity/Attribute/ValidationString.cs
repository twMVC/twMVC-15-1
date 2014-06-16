// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ValidationString.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ValidationString : Attribute, ICloneable<ValidationString>, IValidation
	{
		#region constants
		public static readonly string _Js = @"function " + typeof(ValidationString).Name + @"() {
		var args = $.makeArray(arguments);
		var boundary = args.shift();
		var allowNull = args.shift();
		var i = args[0], j = args[1];
		switch(boundary) {
			case 'In':
				return function(val){
					return (allowNull&&val=='') || $.inArray(val, args)>-1 ? '' : 'Must be one of these following data: ' + args
				}; break;
			case 'Exclude':
				return function(val){
					return (allowNull&&val=='') || $.inArray(val, args)==-1 ? '' : 'Can not be one of these following data: ' + args
				}; break;
			case 'ContainsAny':
				return function(val){
					return (allowNull&&val=='') || (function (val,args) {
						var r = !0;
						$.each(args,function(i,o){
							if(val.indexOf(o)>-1) {return r=!1;}
						});
						return !r;
					})(val,args) ? '' : 'Must contains one of these following data: ' + args
				}; break;
			case 'ContainsAll':
				return function(val){
					return (allowNull&&val=='') || (function (val,args) {
						var r = !0;
						$.each(args,function(i,o){
							if(val.indexOf(o)<0) {return r=!1;}
						});
						return r;
					})(val,args) ? '' : 'Must contains all of these following data: ' + args
				}; break;
			case 'StartWith':
				return function(val){
					return (allowNull&&val=='') || (function (val,args) {
						var r = !0;
						$.each(args,function(i,o){
							if(val.indexOf(o)==0) {return r=!1;}
						});
						return !r;
					})(val,args) ? '' : 'Must start with one of these following data: ' + args
				}; break;
			case 'EndWith':
				return function(val){
					return (allowNull&&val=='') || (function (val,args) {
						var r = !0;
						$.each(args,function(i,o){
							if(val.indexOf(o)==val.length-o.length) {return r=!1;}
						});
						return !r;
					})(val,args) ? '' : 'Must start with one of these following data: ' + args
				}; break;
			case 'IsMatch':
				return function(val){
					return (allowNull&&val=='') || (function (val,args) {
						var r = !0;
						$.each(args,function(i,o){
							if(o.test(val)) {return r=!1;}
						});
						return !r;
					})(val,args) ? '' : 'Must be one of these regular expression : ' + args
				}; break;
			case 'LengthRange':
				return function(val){
					return (allowNull&&val=='') || i<=val.length&&val.length<=j ? '' : 'String length must between '+i+' and '+j;
				}; break;
			case 'LengthGreaterThan':
				return function(val){
					return (allowNull&&val=='') || val.length>i ? '' : 'String length must greater than '+i;
				}; break;
			case 'LengthSmallerThan':
				return function(val){
					return (allowNull&&val=='') || val.length<i ? '' : 'String length must smaller than '+i;
				}; break;
			case 'IdentificationCardNumberTW':
				return function(val){
					return (allowNull&&val=='') || (" + Checker.GetJs_IsTaiwanIdentificationCardNumber("tid") + @")(val) ? '' : 'This is not Taiwan identification card number';
				}; break;
			case 'IdentificationCardNumberCN':
			case 'CreditCardNumber':
			case 'UnifiedSerialNumber':
				throw 'Not implement';
			case 'IsEmailAddress':
				return function(val){
					return (allowNull&&val=='') || /" + DataConstants.Pattern.Email + @"/.test(val) ? '' : 'This is not email address';
				}; break;
			default:throw 'not support type :' + boundary;
		}
	}
";
		#endregion

		#region constructor
		public ValidationString()
		{
		}

		public ValidationString(BoundaryString boundary) 
			: this(boundary, false)
		{
		}

		public ValidationString(BoundaryString boundary, bool allowNullOrEmpty)
		{
			switch(boundary) {
				case BoundaryString.IdentificationCardNumberTW:
				case BoundaryString.IdentificationCardNumberCN:
				case BoundaryString.CreditCardNumber:
				case BoundaryString.UnifiedSerialNumber:
				case BoundaryString.IsEmailAddress:
					break;
				default:
					throw new ArgumentException(String.Format(
						"\"{0}\" must have other parameters.",
						boundary
					));
			}

			this.Boundary = boundary;
			this.AllowNullOrEmpty = allowNullOrEmpty;
		}

		public ValidationString(BoundaryString boundary, params string[] strings)
			: this(boundary, false, strings)
		{
		}

		public ValidationString(
			BoundaryString boundary, bool allowNullOrEmpty, params string[] strings)
		{
			if(strings.Length < 1) {
				string msg = "Invalide parameter";
				Logger.Error(
					msg,
					Any.Concat
					(
						Any.Concat
						(
							new Any("DateTimeBoundary", boundary.ToString()),
							new Any("strings.Length", strings.Length.ToString())
						),
						strings.ToAnys()
					)
				);
				throw new ArgumentException(msg);
			}

			switch(boundary) {
				case BoundaryString.In:
				case BoundaryString.Exclude:
				case BoundaryString.ContainsAny:
				case BoundaryString.ContainsAll:
				case BoundaryString.StartWith:
				case BoundaryString.EndWith:
				case BoundaryString.IsMatch:
					break;
				default:
					throw new ArgumentException(String.Format(
						"\"{0}\" can not have string parameters.",
						boundary
					));
			}

			this.Boundary = boundary;
			this.Strings = strings;
			this.AllowNullOrEmpty = allowNullOrEmpty;
		}

		public ValidationString(
			BoundaryString boundary, params int[] integers)
			: this(boundary, false, integers)
		{
		}

		public ValidationString(
			BoundaryString boundary, bool allowNullOrEmpty, params int[] integers)
		{
			bool hasError = false;
			switch(boundary) {
				case BoundaryString.LengthRange:
					hasError = integers.Length < 2;
					break;
				case BoundaryString.LengthGreaterThan:
				case BoundaryString.LengthSmallerThan:
					hasError = integers.Length < 1;
					break;
				default:
					throw new ArgumentException(String.Format(
						"\"{0}\" can not have integer parameters.",
						boundary
					));
			}

			if(hasError) {
				string msg = "Invalide parameter";
				Logger.Error(
					msg,
					Any.Concat
					(
						Any.Concat
						(
							new Any("StringBoundary", boundary.ToString()),
							new Any("integers.Length", integers.Length.ToString())
						),
						integers.ToAnys()
					)
				);
				throw new ArgumentException(msg);
			}

			this.Boundary = boundary;
			this.Integers = integers;
			this.AllowNullOrEmpty = allowNullOrEmpty;
		}
		#endregion

		#region property
		public Column Column { get; internal set; }
		public BoundaryString Boundary { get; internal set; }
		public string[] Strings { get; internal set; }
		public int[] Integers { get; internal set; }
		public bool AllowNullOrEmpty { get; internal set; }
		#endregion

		#region ICloneable<ValidationString>
		public ValidationString Clone()
		{
			ValidationString clone = new ValidationString();
			clone.Column = Column;
			clone.Boundary = Boundary;
			clone.Strings = Strings;
			clone.Integers = Integers;
			clone.AllowNullOrEmpty = AllowNullOrEmpty;
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
				.Append("'")
				.Append(Formator.JavaScriptEncode(Boundary.ToString()))
				.Append("'")
				.Append(",")
				.Append(AllowNullOrEmpty ? "!0" : "!1");

			if(!Checker.IsNull(Integers)) {
				sb.Append(",").Append(Integers.ColsIntoStr());
			} else if(!Checker.IsNull(Strings)) {
				if(Boundary == BoundaryString.IsMatch) {
					foreach(var item in Strings) {
						sb.Append(",/").Append(item).Append("/");
					}
				} else {
					foreach(var item in Strings) {
						sb.Append(",").Append(Formator.JavaScriptEncode(item));
					}
				}
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
			var val = value as string ?? String.Empty;
			Result result = new Result(true);
			if(AllowNullOrEmpty && String.IsNullOrEmpty(val)) { return result; }

			switch(Boundary) {
				case BoundaryString.In:
					result.Success = val.In(Strings);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Must be one of these following datas {0}.",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.Exclude:
					result.Success = val.Exclude(Strings);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Can not be one of these following datas {0}.",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.ContainsAny:
					result.Success = val.ContainsAny(Strings);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Must contains one of these following datas {0}.",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.ContainsAll:
					result.Success = val.ContainsAll(Strings);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Must contains all of these following datas {0}.",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.StartWith:
					result.Success = val.StartWith(Strings);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Must start with one of these following datas {0}.",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.EndWith:
					result.Success = val.EndWith(Strings);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Must end with one of these following datas {0}",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.IsMatch:
					foreach(var item in Strings) {
						result.Success = val.IsMatch(item);
						if(result.Success) {
							break;
						}
					}
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"Must match one of these regular expression: {0}",
							Strings.ToString(",")
						);
					break;
				case BoundaryString.LengthRange:
					result.Success = val.LengthRange(
						Integers[0],
						Integers[1]
					);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"String length must between {0} and {1}.",
							Integers[0],
							Integers[1]
						);
					break;
				case BoundaryString.LengthGreaterThan:
					result.Success = val.LengthBiggerThan(Integers[0]);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"String length must bigger than {0}.",
							Integers[0]
						);
					break;
				case BoundaryString.LengthSmallerThan:
					result.Success = val.LengthSmallerThan(Integers[0]);
					result.Message = result.Success
						? String.Empty
						: String.Format(
							"String length must smaller than {0}.",
							Integers[0]
						);
					break;
				case BoundaryString.IdentificationCardNumberTW:
					result.Success = Checker.IsTaiwanIdentificationCardNumber(val);
					result.Message = result.Success
						? String.Empty
						: "This is not Taiwan identification card number.";
					break;
				case BoundaryString.IdentificationCardNumberCN:
					result.Success = Checker.IsChinaIdentificationCardNumber(val);
					result.Message = result.Success
						? String.Empty
						: "This is not China identification card number.";
					break;
				case BoundaryString.CreditCardNumber:
					result.Success = Checker.IsCreditcardNumber(val);
					result.Message = result.Success
						? String.Empty
						: "This is not credit card number.";
					break;
				case BoundaryString.UnifiedSerialNumber:
					result.Success = Checker.IsUnifiedSerialNumber(val);
					result.Message = result.Success
						? String.Empty
						: "This is not unified serial number.";
					break;
				case BoundaryString.IsEmailAddress:
					result.Success = Checker.IsEmailAddress(val);
					result.Message = result.Success
						? String.Empty
						: "This is not email address.";
					break;
				default:
					throw new NotSupportedException();
			}

			return result;
		}
		#endregion
	}
}
