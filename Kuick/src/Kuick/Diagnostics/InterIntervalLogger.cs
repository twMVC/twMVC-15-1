// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// InterIntervalLogger.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-03-02 - Creation


using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Kuick
{
	public class IILogger : InterIntervalLogger
	{
		public IILogger(string title, IntervalLogger il)
			: base(title, il)
		{
		}
	}

	public class InterIntervalLogger : IDisposable
	{
		#region constructor
		public InterIntervalLogger(string title, IntervalLogger il)
		{
			this.Title = title;
			this.OutsideIntervalLogger = il;

			Stopwatch = new Stopwatch();
			Stopwatch.Start();
		}
		#endregion

		#region property
		private string Title { get; set; }
		private IntervalLogger OutsideIntervalLogger { get; set; }

		private Stopwatch Stopwatch { get; set; }
		private decimal ExecutionTime { get; set; }
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Stopwatch.Stop();
			string elapsed = Formator.MillisecondToSecondString(
				Stopwatch.Elapsed.TotalMilliseconds
			);
			ExecutionTime = Convert.ToDecimal(elapsed);
			OutsideIntervalLogger.Add(
				"Inter Elapsed (seconds)", 
				string.Concat(elapsed, " -- ", Title)
			);
		}
		#endregion
	}
}
