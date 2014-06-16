// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Kuicker
{
	public class _Logger
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
		public static string TraceName { get { return "Logger"; } }
		public static Int64 CurrentLogSize { get; private set; }
		public static string InnerMessage { get; internal set; }

		private static string _FolderPath;
		public static string FolderPath
		{
			get
			{
				if(string.IsNullOrEmpty(_FolderPath)) {
					_FolderPath = RunTime.CreateFolder(Config.Log.FolderName);
					if(Config.Log.SeperateFolder) {
						foreach(var one in Enum.GetValues(typeof(LogLevel))) {
							var subFolderPath = Path.Combine(
								_FolderPath, one.ToString()
							);
							RunTime.CreateFolder(subFolderPath);
						}

					}
				}
				return _FolderPath;
			}
		}
		#endregion

		public static bool Keep(LogLevel level)
		{
			switch(level) {
				case LogLevel.Fatal:
				case LogLevel.Error:
				case LogLevel.Warn:
				case LogLevel.Info:
					return true;
				case LogLevel.Debug:
					return Config.Kernel.Debug;
				default:
					throw ExHelper.NotImplementedEnum<LogLevel>(level);
			}
		}

		public static bool Testing
		{
			get
			{
				return Keep(LogLevel.Test);
			}
		}

		public static bool Debugging
		{
			get
			{
				return Keep(LogLevel.Debug);
			}
		}

		#region Message
		public static LogBlock Message(
			string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Info, title, message, anys);
		}
		public static LogBlock Message(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Info, title, anys);
		}
		public static LogBlock Message(params Any[] anys)
		{
			return Apppend(LogLevel.Info, anys);
		}

		public static LogBlock Message(
			string title, string message, List<Any> anys)
		{
			return Apppend(LogLevel.Info, title, message, anys.ToArray());
		}
		public static LogBlock Message(string title, List<Any> anys)
		{
			return Apppend(LogLevel.Info, title, anys.ToArray());
		}
		public static LogBlock Message(List<Any> anys)
		{
			return Apppend(LogLevel.Info, anys.ToArray());
		}
		#endregion


		#region Error
		public static LogBlock Error(
			string title,
			string message,
			Exception exception,
			params Any[] anys)
		{
			return Apppend(LogLevel.Error, title, message, exception, anys);
		}
		public static LogBlock Error(
			string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Error, title, message, anys);
		}
		public static LogBlock Error(
			string title,
			Exception exception,
			params Any[] anys)
		{
			return Apppend(
				LogLevel.Error, title, string.Empty, exception, anys
			);
		}
		public static LogBlock Error(string title, params Any[] anys)
		{
			return Apppend(
				LogLevel.Error, title, anys
			);
		}
		public static LogBlock Error(Exception exception, Any[] anys)
		{
			return Apppend(
				LogLevel.Error, exception, anys
			);
		}




		public static LogBlock Error(
			string title,
			string message,
			Exception exception,
			List<Any> anys)
		{
			return Apppend(
				LogLevel.Error, title, message, exception, anys.ToArray()
			);
		}
		public static LogBlock Error(
			string title, string message, List<Any> anys)
		{
			return Apppend(LogLevel.Error, title, message, anys.ToArray());
		}
		public static LogBlock Error(
			string title,
			Exception exception,
			List<Any> anys)
		{
			return Apppend(
				LogLevel.Error, title, string.Empty, exception, anys.ToArray()
			);
		}
		public static LogBlock Error(string title, List<Any> anys)
		{
			return Apppend(
				LogLevel.Error, title, anys.ToArray()
			);
		}
		public static LogBlock Error(Exception exception, List<Any> anys)
		{
			return Apppend(
				LogLevel.Error, exception, anys.ToArray()
			);
		}
		#endregion

		#region Monitor
		public static LogBlock Monitor(
			string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Monitor, title, message, anys);
		}
		public static LogBlock Monitor(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Monitor, title, anys);
		}
		public static LogBlock Monitor(params Any[] anys)
		{
			return Apppend(LogLevel.Monitor, anys);
		}

		public static LogBlock Monitor(
		string title, string message, List<Any> anys)
		{
			return Apppend(LogLevel.Monitor, title, message, anys.ToArray());
		}
		public static LogBlock Monitor(string title, List<Any> anys)
		{
			return Apppend(LogLevel.Monitor, title, anys.ToArray());
		}
		public static LogBlock Monitor(List<Any> anys)
		{
			return Apppend(LogLevel.Monitor, anys.ToArray());
		}
		#endregion

		#region Test
		public static LogBlock Test(
			string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Test, title, message, anys);
		}
		public static LogBlock Test(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Test, title, anys);
		}
		public static LogBlock Test(params Any[] anys)
		{
			return Apppend(LogLevel.Test, anys);
		}

		public static LogBlock Test(
			string title, string message, List<Any> anys)
		{
			return Apppend(LogLevel.Test, title, message, anys.ToArray());
		}
		public static LogBlock Test(string title, List<Any> anys)
		{
			return Apppend(LogLevel.Test, title, anys.ToArray());
		}
		public static LogBlock Test(List<Any> anys)
		{
			return Apppend(LogLevel.Test, anys.ToArray());
		}
		#endregion

		#region Debug
		public static LogBlock Debug(
			string title, string message, params Any[] anys)
		{
			return Apppend(LogLevel.Debug, title, message, anys);
		}
		public static LogBlock Debug(string title, params Any[] anys)
		{
			return Apppend(LogLevel.Debug, title, anys);
		}
		public static LogBlock Debug(params Any[] anys)
		{
			return Apppend(LogLevel.Debug, anys);
		}

		public static LogBlock Debug(
			string title, string message, List<Any> anys)
		{
			return Apppend(LogLevel.Debug, title, message, anys.ToArray());
		}
		public static LogBlock Debug(string title, List<Any> anys)
		{
			return Apppend(LogLevel.Debug, title, anys.ToArray());
		}
		public static LogBlock Debug(List<Any> anys)
		{
			return Apppend(LogLevel.Debug, anys.ToArray());
		}
		#endregion

		#region Append
		private static LogBlock Apppend(
			LogLevel level, string title, string message, params Any[] anys)
		{
			return Apppend(
				level, title, message, null, anys
			);
		}
		private static LogBlock Apppend(LogLevel level, params Any[] anys)
		{
			return Apppend(
				level, string.Empty, string.Empty, null, new Any[0]
			);
		}
		private static LogBlock Apppend(
			LogLevel level, string title, params Any[] anys)
		{
			return Apppend(
				level, title, string.Empty, null, anys
			);
		}
		private static LogBlock Apppend(
			LogLevel level, Exception exception, Any[] anys)
		{
			return Apppend(
				level, string.Empty, string.Empty, exception, anys
			);
		}

		private static LogBlock Apppend(
			LogLevel level,
			string title,
			string message,
			Exception exception,
			params Any[] anys)
		{
			if(!Keep(level)) { return new LogBlock(); }

			bool showSource = true;
			if(title.IsNullOrEmpty() && null != exception) {
				title = exception.Source;
				showSource = false;
			}

			string guid = Guid.NewGuid().ToString();
			StringBuilder sb = new StringBuilder();
			sb.Append(guid + " " + level);
			if(!title.IsNullOrEmpty()) {
				sb.AppendLine();
				sb.Append(PREFIX_TITLE);
				sb.Append(title);
			}
			if(!String.IsNullOrEmpty(message)) {
				sb.AppendLine();
				sb.Append(PREFIX_MESSAGE);
				sb.Append(message);
			}

			List<Any> list = new List<Any>();
			list.AddRange(anys);
			if(null != exception) {
				if(showSource) {
					list.Add(new Any("source", exception.Source));
				}
				Exception e = exception;
				while(null != e) {
					list.Add(new Any("Message", e.Message));
					e = e.InnerException;
				}
				list.Add(new Any("TargetSite", exception.TargetSite));
				list.Add(new Any("HelpLink", exception.HelpLink));
				if(exception.Data.Count > 0) {
					list.Add(new Any(
						"error datas",
						"following " +
						exception.Data.Count.ToString() +
						" items"
					));
					foreach(DictionaryEntry de in exception.Data) {
						list.Add(new Any(de.Key.ToString(), de.Value));
					}
				}
				list.Add(new Any(
					"Stack Trace",
					exception.StackTrace.IsNullOrEmpty()
						? string.Empty
						: "..." + Environment.NewLine + exception.StackTrace
				));
			}
			string nvStr = list.ToString();
			if(!nvStr.IsNullOrEmpty()) {
				sb.AppendLine();
				sb.Append(nvStr);
			}

			DateTime timeStamp = DateTime.Now;
			var output = WriteLine(sb.ToString(), out timeStamp);

			var block = new LogBlock() {
				ComputerName = Environment.CommandLine,
				FileName = FileName,
				AppID = Kernel.AppID,
				TimeStamp = timeStamp,
				Index = _Index,
				Guid = guid,
				Level = level,
				Title = title,
				Message = message,
				Exception = exception,
				Anys = anys,
				Output = output,
			};

			if(!SkipInterceptor && null != Interceptor) {
				SkipInterceptor = true;
				Interceptor(block);
				SkipInterceptor = false;
			}

			return block;
		}
		#endregion

		#region WriteLine
		private static string WriteLine(string message, out DateTime timeStamp)
		{
			lock(_Lock) {
				if(CurrentLogSize > Config.Log.RefreshSize * 1024) {
					if(_Internal) {
						Stop(ref _StreamWriter);
					} else {
						Stop(ref _Listener);
					}
					Start();
				}

				// output
				timeStamp = DateTime.Now;
				string output = string.Format(
					"{0}{1} {2} >>> {3}",
					CurrentLogSize > 0 ? Environment.NewLine : string.Empty,
					timeStamp.yyyy_MM_dd_HH_mm_ss_fff(),
					++_Index,
					message
				);

				// Trace
				for(int i = 0; i < MAX_FLUSH; ++i) {
					try {
						CurrentLogSize += output.Trim().Length;
						Trace.WriteLine(output);
						Trace.Flush();
						break;
					} catch {
						// swallow it
					}
				}

				return output;
			}
		}
		#endregion

		#region Start & Stop
		public static void Start()
		{
			if(!Config.Log.Enable) { return; }

			_Internal = _Starting;
			Start(ref _StreamWriter);
		}

		public static void Start(ref StreamWriter logger)
		{
			if(!Config.Log.Enable) { return; }

			lock(_Lock) {
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
						("0000000" + _SplitIndex.ToString()).Right(7),
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
										FolderPath,
										string.Concat(
											fileName,
											"=",
											("0000000" + index.ToString())
												.Right(7),
											".log"
										)
									);
								}
								FileFullName = tmpFileFullName;
							}
						}

						FileName = Path.GetFileName(FileFullName);
						logger = new StreamWriter(
							FileFullName, true, Encoding.UTF8
						);
						TextWriterTraceListener traceListener =
							new TextWriterTraceListener(logger, TraceName);
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
						Thread.Sleep(50);
					} catch {
					}
				}

				CurrentLogSize = 0; // renew LogSize

				Logger.Message(
					"Log file was created and system is running",
					new Any("log file name", FileFullName)
				);

				if(null != ex) { throw ex; }
			}
		}

		public static void Stop(ref StreamWriter logger)
		{
			if(!Config.Log.Enable) { return; }

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
		public static bool SkipInterceptor { get; set; }

		public static Action<LogBlock> Interceptor
		{
			get;
			set;
		}
		#endregion
	}
}
