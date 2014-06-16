// Logger.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Core;

namespace Kuicker
{
	public class Logger : ILog
	{
		private static object _Lock = new object();
		private static ILog _Log;




		//#region internal
		//internal static void Start()
		//{
		//	if(null == _Log) {
		//		lock(_Lock) {
		//			if(null == _Log) {
		//				var configPath = Path.Combine(
		//					AppDomain.CurrentDomain.BaseDirectory,
		//					"log4net.config"
		//				);
		//				XmlConfigurator.ConfigureAndWatch(
		//					new FileInfo(configPath)
		//				);
		//				_Log = LogManager.GetLogger(Constants.Framework.Name);
		//			}
		//		}
		//	}

		//}

		//internal static void Stop()
		//{
		//	if(null != _Log) {
		//		lock(_Lock) {
		//			if(null != _Log) {
		//				_Log = null;
		//			}
		//		}
		//	}
		//}
		//#endregion

		//#region Fatal
		//public static void Fatal(object message)
		//{
		//	Start();
		//	_Log.Fatal(message);
		//}

		//public static void Fatal(object message, Exception exception)
		//{
		//	Start();
		//	_Log.Fatal(message, exception);
		//}

		//public static void FatalFormat(string format, params object[] args)
		//{
		//	Start();
		//	_Log.FatalFormat(format, args);
		//}

		//public static void FatalFormat(
		//	IFormatProvider provider, string format, params object[] args)
		//{
		//	Start();
		//	_Log.FatalFormat(provider, format, args);
		//}

		////public static void FatalFormat(
		////	string format, object arg0, object arg1, object arg2)
		////{
		////	Start();
		////	_Log.FatalFormat(format, arg0, arg1, arg2);
		////}

		////public static void FatalFormat(
		////	string format, object arg0, object arg1)
		////{
		////	Start();
		////	_Log.FatalFormat(format, arg0, arg1);
		////}

		////public static void FatalFormat(string format, object arg0)
		////{
		////	Start();
		////	_Log.FatalFormat(format, arg0);
		////}
		//#endregion

		//#region Error
		//public static void Error(object message)
		//{
		//	Start();
		//	_Log.Error(message);
		//}

		//public static void Error(object message, Exception exception)
		//{
		//	Start();
		//	_Log.Error(message, exception);
		//}

		//public static void ErrorFormat(string format, params object[] args)
		//{
		//	Start();
		//	_Log.ErrorFormat(format, args);
		//}

		//public static void ErrorFormat(
		//	IFormatProvider provider, string format, params object[] args)
		//{
		//	Start();
		//	_Log.ErrorFormat(provider, format, args);
		//}

		////public static void ErrorFormat(
		////	string format, object arg0, object arg1, object arg2)
		////{
		////	Start();
		////	_Log.ErrorFormat(format, arg0, arg1, arg2);
		////}

		////public static void ErrorFormat(
		////	string format, object arg0, object arg1)
		////{
		////	Start();
		////	_Log.ErrorFormat(format, arg0, arg1);
		////}

		////public static void ErrorFormat(string format, object arg0)
		////{
		////	Start();
		////	_Log.ErrorFormat(format, arg0);
		////}
		//#endregion

		//#region Warn
		//public static void Warn(object message)
		//{
		//	Start();
		//	_Log.Warn(message);
		//}

		//public static void Warn(object message, Exception exception)
		//{
		//	Start();
		//	_Log.Warn(message, exception);
		//}

		//public static void WarnFormat(string format, params object[] args)
		//{
		//	Start();
		//	_Log.WarnFormat(format, args);
		//}

		//public static void WarnFormat(
		//	IFormatProvider provider, string format, params object[] args)
		//{
		//	Start();
		//	_Log.WarnFormat(provider, format, args);
		//}

		////public static void WarnFormat(
		////	string format, object arg0, object arg1, object arg2)
		////{
		////	Start();
		////	_Log.WarnFormat(format, arg0, arg1, arg2);
		////}

		////public static void WarnFormat(
		////	string format, object arg0, object arg1)
		////{
		////	Start();
		////	_Log.WarnFormat(format, arg0, arg1);
		////}

		////public static void WarnFormat(string format, object arg0)
		////{
		////	Start();
		////	_Log.WarnFormat(format, arg0);
		////}
		//#endregion

		//#region Info
		//public static void Info(object message)
		//{
		//	Start();
		//	_Log.Info(message);
		//}

		//public static void Info(object message, Exception exception)
		//{
		//	Start();
		//	_Log.Info(message, exception);
		//}

		//public static void InfoFormat(string format, params object[] args)
		//{
		//	Start();
		//	_Log.InfoFormat(format, args);
		//}

		//public static void InfoFormat(
		//	IFormatProvider provider, string format, params object[] args)
		//{
		//	Start();
		//	_Log.InfoFormat(provider, format, args);
		//}

		////public static void InfoFormat(
		////	string format, object arg0, object arg1, object arg2)
		////{
		////	Start();
		////	_Log.InfoFormat(format, arg0, arg1, arg2);
		////}

		////public static void InfoFormat(
		////	string format, object arg0, object arg1)
		////{
		////	Start();
		////	_Log.InfoFormat(format, arg0, arg1);
		////}

		////public static void InfoFormat(string format, object arg0)
		////{
		////	Start();
		////	_Log.InfoFormat(format, arg0);
		////}
		//#endregion

		//#region Debug
		//public static void Debug(object message)
		//{
		//	Start();
		//	_Log.Debug(message);
		//}

		//public static void Debug(object message, Exception exception)
		//{
		//	Start();
		//	_Log.Debug(message, exception);
		//}

		//public static void DebugFormat(string format, params object[] args)
		//{
		//	Start();
		//	_Log.DebugFormat(format, args);
		//}

		//public static void DebugFormat(
		//	IFormatProvider provider, string format, params object[] args)
		//{
		//	Start();
		//	_Log.DebugFormat(provider, format, args);
		//}

		////public static void DebugFormat(
		////	string format, object arg0, object arg1, object arg2)
		////{
		////	Start();
		////	_Log.DebugFormat(format, arg0, arg1, arg2);
		////}

		////public static void DebugFormat(
		////	string format, object arg0, object arg1)
		////{
		////	Start();
		////	_Log.DebugFormat(format, arg0, arg1);
		////}

		////public static void DebugFormat(string format, object arg0)
		////{
		////	Start();
		////	_Log.DebugFormat(format, arg0);
		////}
		//#endregion

		//#region property
		//public static bool IsFatalEnabled
		//{
		//	get
		//	{
		//		Start();
		//		return _Log.IsFatalEnabled;
		//	}
		//}

		//public static bool IsErrorEnabled
		//{
		//	get
		//	{
		//		Start();
		//		return _Log.IsErrorEnabled;
		//	}
		//}

		//public static bool IsWarnEnabled
		//{
		//	get
		//	{
		//		Start();
		//		return _Log.IsWarnEnabled;
		//	}
		//}

		//public static bool IsInfoEnabled
		//{
		//	get
		//	{
		//		Start();
		//		return _Log.IsInfoEnabled;
		//	}
		//}

		//public static bool IsDebugEnabled
		//{
		//	get
		//	{
		//		Start();
		//		return _Log.IsDebugEnabled;
		//	}
		//}
		//#endregion

		//#region Logger
		//public static ILogger Logger
		//{
		//	get
		//	{
		//		Start();
		//		return _Log.Logger;
		//	}
		//}
		//#endregion

		#region ILog Members

		public void Debug(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Debug(object message)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(string format, object arg0, object arg1)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(string format, object arg0)
		{
			throw new NotImplementedException();
		}

		public void DebugFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Error(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Error(object message)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, object arg0, object arg1)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, object arg0)
		{
			throw new NotImplementedException();
		}

		public void ErrorFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Fatal(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Fatal(object message)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, object arg0, object arg1)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, object arg0)
		{
			throw new NotImplementedException();
		}

		public void FatalFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void Info(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Info(object message)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, object arg0, object arg1)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, object arg0)
		{
			throw new NotImplementedException();
		}

		public void InfoFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public bool IsDebugEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsErrorEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFatalEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsInfoEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsWarnEnabled
		{
			get { throw new NotImplementedException(); }
		}

		public void Warn(object message, Exception exception)
		{
			throw new NotImplementedException();
		}

		public void Warn(object message)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(string format, object arg0, object arg1)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(string format, object arg0)
		{
			throw new NotImplementedException();
		}

		public void WarnFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ILoggerWrapper Members

		ILogger ILoggerWrapper.Logger
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
