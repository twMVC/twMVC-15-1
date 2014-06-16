// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WinEventLog.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Diagnostics;

namespace Kuick
{
	public class WinEventLog
	{
		public static void Error(string message)
		{
			Write(EventLogEntryType.Error, message);
		}

		public static void Warning(string message)
		{
			Write(EventLogEntryType.Warning, message);
		}

		public static void Information(string message)
		{
			Write(EventLogEntryType.Information, message);
		}

		private static void Write(EventLogEntryType logType, string message)
		{
			Write(logType, message, Constants.Framework.Name);
		}

		private static void Write(
			EventLogEntryType logType, string message, string source)
		{
			try {
				if(!EventLog.SourceExists(source)) {
					EventLog.CreateEventSource(source, "Application");
				}
				EventLog log = new EventLog();
				log.Source = source;
				log.WriteEntry(message, logType);
			} catch {
				// swallow
			}
		}
	}
}
