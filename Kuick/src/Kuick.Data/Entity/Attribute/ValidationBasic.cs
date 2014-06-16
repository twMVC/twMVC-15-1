// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ValidationBasic.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class ValidationBasic
		: Attribute, ICloneable<ValidationBasic>, IValidation
	{
		#region constants
		private readonly string _Js = @"
function " + typeof(ValidationBasic).Name + @"() {
	return function (){return '';}
}";
		#endregion

		#region constructor
		public ValidationBasic()
		{
		}
		#endregion

		#region ICloneable<ValidationBasic>
		public ValidationBasic Clone()
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IValidation
		public string JsFunction { get { return _Js; } }

		public string JsValidate()
		{
			return string.Concat(this.GetType().Name, "()");
		}

		public Result Validate(Column column, object value)
		{
			// Only string type column have basic validation.
			if(!column.IsString) { return Result.BuildSuccess(); }

			Result result = new Result();
			string val = value as string;

			// Null
			if(null == val) {
				if(column.Spec.NotAllowNull) {
					result.Success = false;
					result.Message = "Not allow null.";
				} else {
					result.Success = true;
				}
				return result;
			}
			if(Checker.IsNull(val) && column.Spec.PrimaryKey) {
				result.Success = false;
				result.Message = "PrimaryKey not allow empty.";
				return result;
			}
			if(Checker.IsNull(val) && !column.Spec.NotAllowNull) {
				result.Success = true;
				return result;
			}

			// DBCS
			if(!column.AllowDBCS && Checker.ContainDBCS(val)) {
				result.Success = false;
				result.Message = "Does not allow DBCS.";
				return result;
			}

			// Reference
			if(!Checker.IsNull(column.Refer)) {
				IEntity instance = Entity.Get(column.Refer.Schema.EntityName, val);
				if(Checker.IsNull(instance)) {
					result.Success = false;
					result.Message = String.Format(
						"Has not referred to any value by {0}.",
						instance.EntityName
					);
					return result;
				}
			}

			// Length
			if(Encoding.Default.GetByteCount(val) > column.ByteCount) {
				result.Success = false;
				result.Message = String.Format(
					"Exceed max len limitation of {0}.",
					column.Spec.Length
				);
				return result;
			}

			// SQL injection alert
			if(column.Visual.Input != VisualInput.HtmlEditor) {
				if(Checker.HasSqlInjection(val)) {
					result.Success = false;
					Logger.Track(
						"ValidationBasic.Check",
						"Has latent SQL injection danger"
					);
					return result;
				}
			}

			result.Success = true;
			return result;
		}

		public Result Validate(string value)
		{
			throw new NotSupportedException("ValidationBasic only can use in Entity column.");
		}
		#endregion

		#region property
		// Parent
		public Column Column { get; internal set; }
		#endregion
	}
}
