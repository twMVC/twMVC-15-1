// IntervalLogger.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kuicker
{
	public class IntervalLogger : IDisposable
	{
		#region constructor
		public IntervalLogger()
			: this(RunTime.CalleeFullName(2))
		{
		}

		public IntervalLogger(string title)
			: this(title, LogLevel.Info)
		{
		}

		public IntervalLogger(LogLevel level)
			: this(RunTime.CalleeFullName(2), level)
		{
		}

		public IntervalLogger(string title, LogLevel level)
		{
			this.Title = title;
			this.Level = level;

			this.Anys = new List<Any>();
			Stopwatch = new Stopwatch();
			Stopwatch.Start();
		}

		//public IntervalLogger(string title, LogLevel level)
		//{
		//	this.Title = title;
		//	this.Level = level;

		//	this.Anys = new List<Any>();
		//	Stopwatch = new Stopwatch();
		//	Stopwatch.Start();
		//}
		#endregion

		#region property
		public string Title { get; set; }
		public LogLevel Level { get; set; }
		public int CriticalTime { get; private set; }
		private Action<IntervalLogger> OverdueAction { get; set; }

		private List<Any> Anys { get; set; }
		private Stopwatch Stopwatch { get; set; }
		private decimal ExecutionTime { get; set; }
		#endregion

		#region method
		public void Add(Exception ex, params Any[] anys)
		{
			Level = LogLevel.Error;
			Add(@"Exception occured", @"////////////////////////////////");
			if(null != anys && anys.Length > 0) { AddRange(anys); }
			AddRange(ex.ToAnys());
			Add(@"Exception occured", @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
		}

		public void Add(string name, object val)
		{
			if(null != Anys) { Anys.Add(new Any(name, val)); }
		}

		public void Add(Any any)
		{
			if(null != Anys) { Anys.Add(any); }
		}

		public void AddRange(List<Any> anys)
		{
			if(null != Anys) { Anys.AddRange(anys); }
		}

		public void AddRange(params Any[] anys)
		{
			if(null != Anys) { Anys.AddRange(anys); }
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Stopwatch.Stop();
			string elapsed = Stopwatch
				.Elapsed
				.TotalMilliseconds
				.MillisecondToSecondString();
			ExecutionTime = Convert.ToDecimal(elapsed);
			Anys.Insert(0, new Any("Elapsed (seconds)", elapsed));

			switch(Level) {
				case LogLevel.Info:
					//Logger.Message(Title, Anys.ToArray());
					break;
				case LogLevel.Error:
					//Logger.Error(Title, Anys.ToArray());
					break;
				//case LogLevel.Test:
				//	Logger.Test(Title, Anys.ToArray());
				//	break;
				default:
					throw ExHelper.NotImplementedEnum(Level);
			}

			// Compare and trigger an error message
			if(
				CriticalTime > 0
				&&
				Stopwatch.Elapsed.CompareTo(
					new TimeSpan(0, 0, 0, CriticalTime)
				) > 0) {
				//Logger.Test(
				//	Title,
				//	"Program segment execution time over expected.",
				//	new Any("Elapsed (seconds)", elapsed),
				//	new Any("Expected (seconds)", CriticalTime)
				//);
				if(null != OverdueAction) { OverdueAction(this); }
			}
		}
		#endregion
	}
}
