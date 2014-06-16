// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IntervalLogger.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Kuick
{
	public class ILogger : IntervalLogger
	{
		public ILogger(string title)
			: base(title)
		{
		}

		public ILogger(string title, LogLevel level)
			: base(title, level)
		{
		}

		public ILogger(
			string title,
			LogLevel level,
			int criticalTimeInSeconds,
			Action<IntervalLogger> actionWhenOverdue)
			: base(title, level, criticalTimeInSeconds, actionWhenOverdue)
		{
		}
	}

	public class IntervalLogger : IDisposable
	{
		#region constructor
		public IntervalLogger(string title)
			: this(title, LogLevel.Message, 0, null)
		{
		}

		public IntervalLogger(string title, LogLevel level)
			: this(title, level, 0, null)
		{
		}

		public IntervalLogger(
			string title,
			LogLevel level,
			int criticalTimeInSeconds,
			Action<IntervalLogger> actionWhenOverdue)
		{
			this.Title = title;
			this.Level = level;
			this.CriticalTime = criticalTimeInSeconds;
			this.OverdueAction = actionWhenOverdue;

			this.Anys = new Anys();
			Stopwatch = new Stopwatch();
			Stopwatch.Start();
		}
		#endregion

		#region property
		public string Title { get; set; }
		public LogLevel Level { get; set; }
		public int CriticalTime { get; private set; }
		private Action<IntervalLogger> OverdueAction { get; set; }

		private Anys Anys { get; set; }
		private Stopwatch Stopwatch { get; set; }
		private decimal ExecutionTime { get; set; }
		#endregion

		#region method
		public void Add(Exception ex, params Any[] anys)
		{
			Level = LogLevel.Error;
			Add(@"Exception occured", @"////////////////////////////////");
			if(null != anys && anys.Length > 0) { AddRange(anys); }
			AddRange(ex.ToAny());
			Add(@"Exception occured", @"\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
		}

		public void Add(string name, object val)
		{
			if(null != Anys) { Anys.Add(name, val); }
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
			string elapsed = Formator.MillisecondToSecondString(
				Stopwatch.Elapsed.TotalMilliseconds
			);
			ExecutionTime = Convert.ToDecimal(elapsed);
			Anys.Insert(0, new Any("Elapsed (seconds)", elapsed));

			switch(Level) {
				case LogLevel.Message:
					Logger.Message(Title, Anys.ToArray());
					break;
				case LogLevel.Error:
					Logger.Error(Title, Anys.ToArray());
					break;
				case LogLevel.Track:
					Logger.Track(Title, Anys.ToArray());
					break;
				default:
					throw new NotImplementedException();
			}

			// Compare and trigger an error message
			if(
				CriticalTime > 0
				&&
				Stopwatch.Elapsed.CompareTo(new TimeSpan(0, 0, 0, CriticalTime)) > 0) {
				Logger.Track(
					Title,
					"Program segment execution time over expected.",
					new Any("Elapsed (seconds)", elapsed),
					new Any("Expected (seconds)", CriticalTime)
				);
				if(null != OverdueAction) { OverdueAction(this); }
			}
		}
		#endregion
	}
}
