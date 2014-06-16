//// InterIntervalLogger.cs
////
//// Copyright (c) Chung, Chun-Yi. All rights reserved.
//// kevin@kuicker.org

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace Kuicker
//{
//	public class IILogger : InterIntervalLogger
//	{
//		public IILogger(IntervalLogger il)
//			: base(il)
//		{
//		}
//		public IILogger(IntervalLogger il, object title)
//			: base(il, title)
//		{
//		}
//	}

//	public class InterIntervalLogger : IDisposable
//	{
//		#region constructor
//		public InterIntervalLogger(IntervalLogger il)
//			: this(il, RunTime.CalleeFullName(2))
//		{
//		}
//		public InterIntervalLogger(IntervalLogger il, object title)
//		{
//			this.Title = null == title 
//				? RunTime.CalleeFullName(2) 
//				: title.ToString();
//			this.OutsideIntervalLogger = il;

//			Stopwatch = new Stopwatch();
//			Stopwatch.Start();
//		}
//		#endregion

//		#region property
//		private string Title { get; set; }
//		private IntervalLogger OutsideIntervalLogger { get; set; }

//		private Stopwatch Stopwatch { get; set; }
//		private decimal ExecutionTime { get; set; }
//		#endregion

//		#region IDisposable
//		public void Dispose()
//		{
//			Stopwatch.Stop();
//			string elapsed = Stopwatch
//				.Elapsed
//				.TotalMilliseconds
//				.MillisecondToSecondString();
//			ExecutionTime = Convert.ToDecimal(elapsed);
//			OutsideIntervalLogger.Add(
//				"Inter Elapsed (seconds)",
//				string.Concat(elapsed, " -- ", Title)
//			);
//		}
//		#endregion
//	}
//}
