// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.Text;

namespace Kuicker
{
	public class Result
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
			this.Datas = new List<Any>();
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
					foreach(var result in InnerResults) {
						if(null == result) { continue; }
						if(!result.Success) { return false; }
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
				var sb = new StringBuilder();
				sb.Append(_Message);
				foreach(var result in InnerResults) {
					if(result.Success) { continue; }
					var message = result.Message;
					if(message.IsNullOrEmpty()) { continue; }
					sb.AppendLine().Append(message);
				}
				return sb.ToString();
			}
			set
			{
				_Message = value;
			}
		}

		public List<string> Messages
		{
			get
			{
				var list = new List<string>();
				string message = Message;
				if(message.IsNullOrEmpty()) { return list; }

				string[] messages = message.Split(Environment.NewLine);
				list.AddRange(messages);
				return list;
			}
		}

		public List<Result> InnerResults { get; set; }
		public List<Any> Datas { get; set; }
		public Exception Exception { get; set; }

		public Result Add(Result innerResult)
		{
			InnerResults.Add(innerResult);
			return this;
		}

		public override string ToString()
		{
			return Success ? "Success" : "Failure";
		}
		#endregion

		#region Out-of-Box Result
		// Success
		public static Result BuildSuccess(params Any[] anys)
		{
			return BuildSuccess("Common Success", anys);
		}
		public static Result BuildSuccess(string message, params Any[] anys)
		{
			Result result = new Result(true, message);
			if(!anys.IsNullOrEmpty()) { result.Datas.AddRange(anys); }
			return result;
		}

		// Failure
		public static Result BuildFailure(params Any[] anys)
		{
			return BuildFailure("Common Failure", anys);
		}
		public static Result BuildFailure(string message, params Any[] anys)
		{
			Result result = new Result(false, message);
			if(!anys.IsNullOrEmpty()) { result.Datas.AddRange(anys); }
			return result;
		}

		public static Result BuildFailure(Exception ex, params Any[] anys)
		{
			return BuildFailure(ex.Message, ex, anys);
		}

		public static Result BuildFailure(
			string message, Exception ex, params Any[] anys)
		{
			Result result = new Result(false, ex.Message);
			result.Exception = ex;
			if(!anys.IsNullOrEmpty()) { result.Datas.AddRange(anys); }
			return result;
		}
		#endregion
	}
}
