// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Result.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Text;

namespace Kuick
{
	public class Result : IResult
	{
		protected static object _Lock = new object();

		#region Constructor
		public Result()
			: this(true)
		{
		}

		public Result(bool success)
			: this(success, string.Empty)
		{
		}

		public Result(bool success, string message)
		{
			this.Success = success;
			this.Message = message;
			this.Datas = new Anys();
			this.InnerResults = new List<Result>();
		}
		#endregion

		#region property
		private bool _Success;
		public bool Success
		{
			get
			{
				lock(_Lock) {
					foreach(Result x in InnerResults) {
						if(null == x) { continue; }
						if(!x.Success) { return false; }
					}
				}
				return _Success;
			}
			set
			{
				_Success = value;
			}
		}

		private string _Message;
		public string Message
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(_Message);
				foreach(Result result in InnerResults) {
					if(result.Success) { continue; }
					if(string.IsNullOrEmpty(result.Message)) { continue; }
					sb.AppendLine();
					sb.Append(result.Message);
				}
				return sb.ToString();
			}
			set 
			{ 
				_Message = value; 
			}
		}
		public Anys Datas { get; set; }
		public List<Result> InnerResults { get; set; }
		public Error Error { get; set; }
		public Exception Exception { get; set; }

		public IResult AddResult(Result innerResult)
		{
			InnerResults.Add(innerResult);
			return this;
		}

		public override string ToString()
		{
			return Success ? "Success" : "Failure";
		}

		public string MessageToHtml()
		{
			string message = Message;
			if(string.IsNullOrEmpty(message)) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			string[] messages = message.SplitWith(Environment.NewLine);
			sb.AppendLine("<ol>");
			foreach(string one in messages) {
				sb.AppendLineFormat("<li>{0}</li>", one);
			}
			sb.AppendLine("</ol>");
			return sb.ToString();
		}
		#endregion

		#region Out-of-Box Result
		// Success
		public static Result BuildSuccess()
		{
			return BuildSuccess("Common Success");
		}
		public static Result BuildSuccess(Anys anys)
		{
			return BuildSuccess("Common Success", anys);
		}
		public static Result BuildSuccess(params Any[] anys)
		{
			return BuildSuccess("Common Success", anys);
		}

		public static Result BuildSuccess(string message, Anys anys)
		{
			Result result = new Result(true, message);
			result.Datas = anys;
			return result;
		}
		public static Result BuildSuccess(string message, params Any[] anys)
		{
			Result result = new Result(true, message);
			if(!Checker.IsNull(anys)) { result.Datas.AddRange(anys); }
			return result;
		}

		// Failure
		public static Result BuildFailure()
		{
			return new Result() { Success = false };
		}
		public static Result BuildFailure(string message, params Any[] anys)
		{
			return new Result() {
				Success = false,
				Message = message,
				Datas = new Anys(anys)
			};
		}
		public static Result BuildFailure(Error error, Anys anys)
		{
			return BuildFailure(error, "Common Failure", anys);
		}
		public static Result BuildFailure<E>(params Any[] anys) where E : Error, new()
		{
			return BuildFailure(new E(), "Common Failure", anys);
		}
		public static Result BuildFailure(Error error, params Any[] anys)
		{
			return BuildFailure(error, "Common Failure", anys);
		}
		public static Result BuildFailure<E>(List<Any> anys) where E : Error, new()
		{
			return BuildFailure(new E(), "Common Failure", anys);
		}
		public static Result BuildFailure(Error error, List<Any> anys)
		{
			return BuildFailure(error, "Common Failure", anys);
		}

		public static Result BuildFailure<E>(string message, Anys anys) where E : Error, new()
		{
			Result result = new Result(false, message);
			result.Error = new E();
			result.Datas = anys;
			return result;
		}
		public static Result BuildFailure(Error error, string message, Anys anys)
		{
			Result result = new Result(false, message);
			result.Error = error;
			result.Datas = anys;
			return result;
		}
		public static Result BuildFailure<E>(string message, params Any[] anys) where E : Error, new()
		{
			Result result = new Result(false, message);
			result.Error = new E();
			if(!Checker.IsNull(anys)) { result.Datas.AddRange(anys); }
			return result;
		}
		public static Result BuildFailure(Error error, string message, params Any[] anys)
		{
			Result result = new Result(false, message);
			result.Error = error;
			if(!Checker.IsNull(anys)) { result.Datas.AddRange(anys); }
			return result;
		}
		public static Result BuildFailure<E>(string message, List<Any> anys) where E : Error, new()
		{
			Result result = new Result(false, message);
			result.Error = new E();
			if(!Checker.IsNull(anys)) { result.Datas.AddRange(anys); }
			return result;
		}
		public static Result BuildFailure(Error error, string message, List<Any> anys)
		{
			Result result = new Result(false, message);
			result.Error = error;
			if(!Checker.IsNull(anys)) { result.Datas.AddRange(anys); }
			return result;
		}
		#endregion
	}

	public class Result<T> : Result where T : class, IResult, new()
	{
		#region Constructor
		public Result()
			: base()
		{
		}

		public Result(bool success)
			: base(success)
		{
		}

		public Result(bool success, string message)
			: base(success, message)
		{
		}
		#endregion

		#region Out-of-Box Result
		// Success
		public new static T BuildSuccess()
		{
			return new T(){ Success = true };
		}
		public new static T BuildSuccess(Anys anys)
		{
			return new T() { Success = true, Datas = anys };
		}
		public new static T BuildSuccess(params Any[] anys)
		{
			return new T() { Success = true, Datas = new Anys(anys) };
		}

		public new static T BuildSuccess(string message, Anys anys)
		{
			return new T() { 
				Success = true, 
				Message = message, 
				Datas = anys 
			};
		}
		public new static T BuildSuccess(string message, params Any[] anys)
		{
			return new T() { 
				Success = true, 
				Message = message, 
				Datas = new Anys(anys) 
			};
		}

		// Failure
		public new static T BuildFailure()
		{
			return new T() { Success = false };
		}
		public new static T BuildFailure(string message, params Any[] anys)
		{
			return new T() {
				Success = false,
				Message = message,
				Datas = new Anys(anys)
			};
		}
		public new static T BuildFailure(Error error, Anys anys)
		{
			return new T() { Success = false, Error = error, Datas = anys };
		}
		public new static T BuildFailure<E>(params Any[] anys) 
			where E : Error, new()
		{
			return new T() { 
				Success = false, 
				Error = new E(), 
				Datas = new Anys(anys) 
			};
		}
		public new static T BuildFailure(Error error, params Any[] anys)
		{
			return new T() { 
				Success = false, 
				Error = error, 
				Datas = new Anys(anys) 
			};
		}
		public new static T BuildFailure<E>(List<Any> anys) 
			where E : Error, new()
		{
			return new T() {
				Success = false, 
				Error = new E(), 
				Datas = new Anys(anys.ToArray()) 
			};
		}
		public new static T BuildFailure(Error error, List<Any> anys)
		{
			return new T() { 
				Success = false, 
				Error = error, 
				Datas = new Anys(anys.ToArray())
			};
		}
		public new static T BuildFailure<E>(string message, Anys anys) 
			where E : Error, new()
		{
			return new T() { 
				Success = false, 
				Message = message, 
				Error = new E(), 
				Datas = anys 
			};
		}
		public new static T BuildFailure(
			Error error, string message, Anys anys)
		{
			return new T() { 
				Success = false, 
				Message = message, 
				Error = error, 
				Datas = anys 
			};
		}
		public new static T BuildFailure<E>(string message, params Any[] anys) 
			where E : Error, new()
		{
			return new T() { 
				Success = false, 
				Message = message, 
				Error = new E(), 
				Datas = new Anys(anys)
			};
		}
		public new static T BuildFailure(
			Error error, string message, params Any[] anys)
		{
			return new T() { 
				Success = false, 
				Message = message, 
				Error = error, 
				Datas = new Anys(anys) 
			};
		}
		public new static T BuildFailure<E>(string message, List<Any> anys) 
			where E : Error, new()
		{
			return new T() { 
				Success = false, 
				Message = message, 
				Error = new E(), 
				Datas = new Anys(anys.ToArray())
			};
		}
		public new static T BuildFailure(Error error, string message, List<Any> anys)
		{
			return new T() { 
				Success = false, 
				Message = message, 
				Error = error, 
				Datas = new Anys(anys.ToArray())
			};
		}
		#endregion
	}
}
