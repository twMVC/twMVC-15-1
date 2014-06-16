//// XXXXXXX.cs
////
//// Copyright (c) Chung, Chun-Yi. All rights reserved.
//// kevin@kuicker.org

//using System;
//using System.Diagnostics;

//namespace Kuicker
//{
//	public class WinEventLog
//	{
//		public static void Error(string message)
//		{
//			Write(EventLogEntryType.Error, message);
//		}

//		public static void Warning(string message)
//		{
//			Write(EventLogEntryType.Warning, message);
//		}

//		public static void Information(string message)
//		{
//			Write(EventLogEntryType.Information, message);
//		}

//		private static void Write(EventLogEntryType logType, string message)
//		{
//			Write(logType, message, Kernel.AppID);
//		}

//		private static void Write(
//			EventLogEntryType logType, string message, string source)
//		{
//			try {
//				if(!EventLog.SourceExists(source)) {
//					EventLog.CreateEventSource(source, "Application");
//				}
//				EventLog log = new EventLog();
//				log.Source = source;
//				log.WriteEntry(message, logType);
//			} catch {
//				// swallow
//			}
//		}
//	}
//}
