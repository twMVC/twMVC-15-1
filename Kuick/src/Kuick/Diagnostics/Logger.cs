// Kuick Application Framework
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Logger.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Kuick
{
	public class Logger
	{
		#region const & field
		public const string PREFIX_TITLE = "Title  : ";
		public const string PREFIX_MESSAGE = "Message: ";
		private const int MAX_FLUSH = 5;
		private const int LOG_CREATE_RETRY_COUNT = 6;      // Max Retry Count

		private static object _Lock = new Object();
		private static long _Index = 0L;
		private static long _SplitIndex = 0L;
		private static StreamWriter _StreamWriter = null; // internal
		private static StreamWriter _Listener = null;     // external
		private static bool _Starting = false;
		private static bool _Internal = false;
		#endregion

		#region property
		private static DateTime CreateTime { get; set; }
		public static string FileFullName { get; private set; }
		public static string FileName { get; private set; }
		public static string TraceName { get { return "K Logger"; } }
		public static Int64 CurrentLogSize { get; private set; }
		//public static LogBlock FirstError { get; private set; }
		//public static LogBlock LastError { get; private set; }
		public static string InnerMessage { get; internal set; }

		private static string _FolderPath;
		public static string FolderPath
		{
			get
			{
				if(string.IsNullOrEmpty(_FolderPath)) {
					_FolderPath = Heartbeat.CreateFolder(Current.Log.Folder);
				}
				return _FolderPath;
			}
			set
			{
				_FolderPath = value;
			}
		}
		#endregion

		#region Error
		public static string Error()
		{
			StackFrame sf = new StackTrace().GetFrame(1);
			return Error(sf);
		}

		public static string Error(Exception ex)
		{
			return Apppend(
				LogLevel.Error,
				LogLevel.Error.ToString(),
				ex,
				string.Empty,
				new Any[0]
			);
		}

		public static string Error(Exception ex, string message)
		{
			return Apppend(LogLevel.Error, string.Empty, ex, message, new Any[0]);
		}

		public static string Error(Exception ex, List<Any> anys)
		{
			return Error(ex, anys.ToArray());
		}
		public static string Error(Exception ex, params Any[] anys)
		{
			return Apppend(LogLevel.Error, string.Empty, ex, string.Empty, anys);
		}

		public static string Error(Exception ex, string message, List<Any> anys)
		{
			return Error(ex, message, anys.ToArray());
		}
		public static string Error(Exception ex, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Error, string.Empty, ex, message, anys);
		}

		public static string Error(string title)
		{
			return Apppend(LogLevel.Error, title, null, string.Empty, new Any[0]);
		}

		public static string Error(string title, Exception ex)
		{
			return Apppend(LogLevel.Error, title, ex, string.Empty, new Any[0]);
		}

		public static string Error(string title, string message)
		{
			return Apppend(LogLevel.Error, title, null, message, new Any[0]);
		}

		public static string Error(string title, string message, List<Any> anys)
		{
			return Error(title, message, anys.ToArray());
		}
		public static string Error(string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Error, title, null, message, anys);
		}

		public static string Error(string title, List<Any> anys)
		{
			return Error(title, string.Empty, anys.ToArray());
		}
		public static string Error(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Error, title, null, string.Empty, anys);
		}

		public static string Error(string title, Exception ex, string message)
		{
			return Apppend(LogLevel.Error, title, ex, message, new Any[0]);
		}

		public static string Error(string title, Exception ex, List<Any> anys)
		{
			return Error(title, ex, anys.ToArray());
		}
		public static string Error(string title, Exception ex, params Any[] anys)
		{
			return Apppend(LogLevel.Error, title, ex, string.Empty, anys);
		}

		public static string Error(string title, string message, Exception ex, List<Any> anys)
		{
			return Error(title, message, ex, anys.ToArray());
		}
		public static string Error(string title, string message, Exception ex, params Any[] anys)
		{
			return Apppend(LogLevel.Error, title, ex, message, anys);
		}

		public static string Error(StackFrame stackFrame)
		{
			return Apppend(LogLevel.Error, stackFrame, null, string.Empty, new Any[0]);
		}

		public static string Error(StackFrame stackFrame, Exception ex)
		{
			return Apppend(LogLevel.Error, stackFrame, ex, string.Empty, new Any[0]);
		}

		public static string Error(StackFrame stackFrame, string message)
		{
			return Apppend(LogLevel.Error, stackFrame, null, message, new Any[0]);
		}

		public static string Error(StackFrame stackFrame, string message, List<Any> anys)
		{
			return Error(stackFrame, message, anys.ToArray());
		}
		public static string Error(StackFrame stackFrame, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Error, stackFrame, null, message, anys);
		}

		public static string Error(StackFrame stackFrame, List<Any> anys)
		{
			return Error(stackFrame, anys.ToArray());
		}
		public static string Error(StackFrame stackFrame, params Any[] anys)
		{
			return Apppend(LogLevel.Error, stackFrame, null, string.Empty, anys);
		}

		public static string Error(StackFrame stackFrame, Exception ex, string message)
		{
			return Apppend(LogLevel.Error, stackFrame, ex, message, new Any[0]);
		}

		public static string Error(StackFrame stackFrame, Exception ex, List<Any> anys)
		{
			return Error(stackFrame, ex, anys.ToArray());
		}
		public static string Error(StackFrame stackFrame, Exception ex, params Any[] anys)
		{
			return Apppend(LogLevel.Error, stackFrame, ex, string.Empty, anys);
		}

		public static string Error(
			StackFrame stackFrame,
			string message,
			Exception ex,
			List<Any> anys)
		{
			return Error(stackFrame, message, ex, anys.ToArray());
		}

		public static string Error(
			StackFrame stackFrame,
			string message,
			Exception ex,
			params Any[] anys)
		{
			return Apppend(LogLevel.Error, stackFrame, ex, message, anys);
		}

		public static Result Error(Result result)
		{
			string title = null == result.Error
				? result.Message
				: result.Error.Cause;
			Error(title, result.Datas);
			return result;
		}
		#endregion

		#region Message
		public static string Message(string title)
		{
			return Apppend(LogLevel.Message, title, null, null, new Any[0]);
		}

		public static string Message(string title, string message)
		{
			return Apppend(LogLevel.Message, title, null, message, new Any[0]);
		}

		public static string Message(string title, List<Any> anys)
		{
			return Message(title, anys.ToArray());
		}
		public static string Message(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Message, title, null, null, anys);
		}

		public static string Message(string title, string message, List<Any> anys)
		{
			return Message(title, message, anys.ToArray());
		}
		public static string Message(string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Message, title, null, message, anys);
		}
		#endregion

		#region Track
		public static string Track(string title)
		{
			return Apppend(LogLevel.Track, title, null, null, new Any[0]);
		}

		public static string Track(string title, string message)
		{
			return Apppend(LogLevel.Track, title, null, message, new Any[0]);
		}

		public static string Track(string title, List<Any> anys)
		{
			return Track(title, anys.ToArray());
		}
		public static string Track(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Track, title, null, null, anys);
		}

		public static string Track(string title, string message, List<Any> anys)
		{
			return Track(title, message, anys.ToArray());
		}
		public static string Track(string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Track, title, null, message, anys);
		}
		#endregion

		#region MessageOrError
		public static bool MesssageOrError(bool success, string title)
		{
			Apppend(
				success ? LogLevel.Message : LogLevel.Error,
				title,
				null,
				null,
				new Any[0]
			);

			return success;
		}

		public static bool MesssageOrError(bool success, string title, string message)
		{
			Apppend(
				success ? LogLevel.Message : LogLevel.Error,
				title,
				null,
				message,
				new Any[0]
			);

			return success;
		}

		public static bool MesssageOrError(bool success, string title, params Any[] anys)
		{
			Apppend(
				success ? LogLevel.Message : LogLevel.Error,
				title,
				null,
				null,
				anys
			);

			return success;
		}

		public static bool MesssageOrError(
			bool success,
			string title,
			string message,
			params Any[] anys)
		{
			Apppend(
				success ? LogLevel.Message : LogLevel.Error,
				title,
				null,
				message,
				anys
			);

			return success;
		}

		public static Result MesssageOrError(Result result)
		{
			List<Any> list = new List<Any>();
			if(!Checker.IsNull(result.InnerResults)) {
				foreach(Result one in result.InnerResults) {
					if(one.Success) { continue; }
					list.Add(new Any("Message", one.Message));
				}
			}
			list.AddRange(result.Datas.ToArray());

			Apppend(
				result.Success ? LogLevel.Message : LogLevel.Error,
				result.Message,
				result.Exception,
				null,
				list.ToArray()
			);

			return result;
		}

		public static Result LogOnError(string title, Result result, params Any[] anys)
		{
			if(!result.Success) {
				MesssageOrError(title, result, anys);
			}
			return result;
		}

		public static Result MesssageOrError(string title, Result result, params Any[] anys)
		{
			List<Any> list = new List<Any>();
			if(!Checker.IsNull(anys)) { list.AddRange(anys); }
			if(!Checker.IsNull(result.InnerResults)) {
				foreach(Result one in result.InnerResults) {
					if(one.Success) { continue; }
					list.Add(new Any("Message", one.Message));
				}
			}
			list.AddRange(result.Datas.ToArray());

			Apppend(
				result.Success ? LogLevel.Message : LogLevel.Error,
				title,
				result.Exception,
				result.Message,
				list.ToArray()
			);
			return result;
		}
		#endregion

		#region Append & WriteLine
		private static string Apppend(
			LogLevel level,
			StackFrame stackFrame,
			Exception ex,
			string message,
			List<Any> anys)
		{
			return Apppend(level, stackFrame, ex, message, anys.ToArray());
		}
		private static string Apppend(
			LogLevel level,
			StackFrame stackFrame,
			Exception ex,
			string message,
			params Any[] anys)
		{
			string title = "Error occured, please check following message!";
			return Apppend(level, title, ex, message, anys);
		}

		private static string Apppend(
			LogLevel level,
			string title,
			Exception ex,
			string message,
			List<Any> anys)
		{
			return Apppend(level, title, ex, message, anys.ToArray());
		}

		private static string Apppend(
			LogLevel level,
			string title,
			Exception ex,
			string message,
			params Any[] anys)
		{
			// controller
			if(Current.Mode == KernelMode.Released && level == LogLevel.Track) {
				return string.Empty;
			}

			bool showSource = true;
			if(Checker.IsNull(title) && null != ex) {
				title = ex.Source;
				showSource = false;
			}

			string uuid = Utility.GetUuid();
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(uuid + " " + level);
			sb.Append(PREFIX_TITLE);
			sb.Append(title);
			if(!String.IsNullOrEmpty(message)) {
				sb.AppendLine();
				sb.Append(PREFIX_MESSAGE);
				sb.Append(message);
			}

			List<Any> list = new List<Any>();
			list.AddRange(anys);
			if(null != ex) {
				if(showSource) { list.Add(new Any("source", ex.Source)); }
				Exception e = ex;
				while(null != e) {
					list.Add(new Any("Message", e.Message));
					e = e.InnerException;
				}
				list.Add(new Any("TargetSite", ex.TargetSite));
				list.Add(new Any("HelpLink", ex.HelpLink));
				if(ex.Data.Count > 0) {
					list.Add(new Any(
						"error datas",
						"following " + ex.Data.Count.ToString() + " items"
					));
					foreach(DictionaryEntry de in ex.Data) {
						list.Add(new Any(de.Key.ToString(), de.Value));
					}
				}
				list.Add(new Any(
					"Stack Trace",
					Checker.IsNull(ex.StackTrace)
						? string.Empty
						: "..." + Environment.NewLine + ex.StackTrace
				));
			}
			string nvStr = Formator.ListAnys(list.ToArray());
			if(!String.IsNullOrEmpty(nvStr)) {
				sb.AppendLine();
				sb.Append(nvStr);
			}

			DateTime timeStamp = DateTime.Now;
			string msg = message;
			if(level != LogLevel.Error || null == ErrorInterceptor) {
				msg = WriteLine(sb.ToString(), out timeStamp);
			}

			// Cumulative error blocks
			if(
				!SkipErrorInterceptor 
				&& 
				level == LogLevel.Error 
				&& 
				null != ErrorInterceptor) {
				SkipErrorInterceptor = true;
				LogBlock block = new LogBlock();
				block.Uuid = uuid;
				block.Index = _Index;
				block.TimeStamp = timeStamp;
				block.Level = level;
				block.Title = title;
				block.Message = message;
				block.Detail = sb.ToString();
				block.Datas = new Anys(list.ToArray());
				block.Exception = ex;

				ErrorInterceptor(block);
				SkipErrorInterceptor = false;
			}

			// throw exception in developing mode
			if(Current.Mode == KernelMode.Developing && null != ex) {
				throw ex;
			}

			// return
			return msg;
		}

		private static string WriteLine(string message, out DateTime timeStamp)
		{
			lock(_Lock) {
				// Refresh log file by size when SystemSetting.LogRefreshSize great than 10.
				// Otherwise by time according to SystemSetting.LogRefreshDurationBy setting.
				if(
					Current.Log.RefreshSize >= 10
						? CurrentLogSize > Current.Log.RefreshSize * 1024
						: Checker.Overdue(CreateTime, Current.Log.RefreshFrequency)) {
					if(_Internal) {
						StopLogger(ref _StreamWriter);
					} else {
						StopLogger(ref _Listener);
					}
					Initialize();
				}

				// txt
				timeStamp = DateTime.Now;
				string txt = String.Format(
					"{0}{1} {2} >>> {3}",
					CurrentLogSize > 0 ? Environment.NewLine : string.Empty,
					 Formator.ToString17(timeStamp),
					++_Index,
					message
				);

				// Trace
				for(int i = 0; i < MAX_FLUSH; ++i) {
					try {
						CurrentLogSize += txt.Trim().Length;
						Trace.WriteLine(txt);
						Trace.Flush();
						break;
					} catch(System.IO.IOException) {
						// swallow it
					} catch {
						// swallow it
					}
				}

				return string.Format("{0} --> {1}", FileName, _Index);
			}
		}
		#endregion

		#region Initialize & StopLogger
		public static void Initialize()
		{
			//if(!_Starting) { return; }

			_Internal = _Starting;
			Initialize(ref _StreamWriter);
		}

		public static void Initialize(ref StreamWriter logger)
		{
			lock(_Lock) {
				if(!Current.Log.Enable) { return; }
				if(!_Internal) {
					_Starting = true;
					_SplitIndex = 0;
				} else {
					_Internal = true;
					++_SplitIndex;
				}
				if(!_Starting) { return; }

				Exception ex = null;
				FileFullName = string.Empty;

				string fileName = string.Concat(
					DateTime.Now.ToString("yyyy-MM-dd+HH-mm-ss"),
					"+",
					Environment.MachineName
				);
				FileFullName = Path.Combine(
					FolderPath,
					string.Concat(
						fileName,
						"=",
						("000" + _SplitIndex.ToString()).Right(3),
						".log"
					)
				);
				for(int i = 0; i < LOG_CREATE_RETRY_COUNT; ++i) {
					if(null != logger) { return; }

					try {
						FileInfo fi = new FileInfo(FileFullName);
						if(null != fi) {
							if(fi.Exists) {
								int index = 1;
								string tmpFileFullName = FileFullName;
								for(
									int j = 0;
									File.Exists(tmpFileFullName);
									++j, index++) {
									tmpFileFullName = Path.Combine(
										Current.Log.FolderPath,
										string.Concat(
											fileName,
											"=",
											("000" + index.ToString()).Right(3),
											".log"
										)
									);
								}
								FileFullName = tmpFileFullName;
							}
						}

						FileName = Path.GetFileName(FileFullName);
						logger = new StreamWriter(FileFullName, true, Encoding.UTF8);
						TextWriterTraceListener traceListener = new TextWriterTraceListener(
							logger, TraceName
						);
						traceListener.Filter = new TraceFilter();
						Logger.CreateTime = DateTime.Now;
						Trace.Listeners.Add(traceListener);

						if(!_Internal) { _Listener = logger; }

						ex = null;
						break;
					} catch(Exception e) {
						WinEventLog.Error(e.Message);
					}

					// retry
					try {
						Thread.Sleep(1000);
					} catch {
					}
				}

				CurrentLogSize = 0; // renew LogSize

				Logger.Message(
					"Log file was created and system is running",
					new Any("log file name", FileFullName),
					new Any("user name", Environment.UserName)
				);

				if(null != ex) { throw ex; }
			}
		}

		public static void StopLogger(ref StreamWriter logger)
		{
			if(null != logger) {
				try {
					if(logger.BaseStream.CanWrite) {
						logger.Flush();
						logger.Close();
						logger = null;
						Trace.Listeners.Remove(TraceName);
					}
				} catch {
					// swallow it
				}
			}
		}
		#endregion

		#region Interceptor
		public static bool SkipErrorInterceptor { get; set; }

		//private static Action<LogBlock> _ErrorInterceptor;
		public static Action<LogBlock> ErrorInterceptor
		{
			get; set;
			//get
			//{
			//	if(null == _ErrorInterceptor) {
			//		return null;
			//		//return delegate(LogBlock block) { };
			//	}
			//	return _ErrorInterceptor;
			//}
			//set
			//{
			//	_ErrorInterceptor = value;
			//}
		}
		#endregion
	}
}
