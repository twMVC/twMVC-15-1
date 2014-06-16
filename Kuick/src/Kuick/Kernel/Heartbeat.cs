// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Heartbeat.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using System.Threading;
using System.IO;
using System.Text;

namespace Kuick
{
	public class Heartbeat : IDisposable
	{
		#region field
		private static object _Lock = new object();
		private static Heartbeat _Singleton;
		private List<IStart> _Starts = new List<IStart>();
		#endregion

		#region Event
		// Start
		private event EventHandler PreStart;
		private event EventHandler BuiltinStart;
		private event EventHandler PreDatabaseStart;
		private event EventHandler DatabaseStart;
		private event EventHandler PostDatabaseStart;
		private event EventHandler PostStart;

		// Terminate
		private event EventHandler BuiltinTerminate;
		#endregion

		#region constructor
		private Heartbeat()
		{
			this.StartUpTime = DateTime.Now;
			this.Position = KernelPosition.Stopped;
			this.Stage = KernelStage.Stopped;

			foreach(Assembly asm in Reflector.Assemblies) {
				var types = Reflector.GatherByInterface<IStart>(asm);
				foreach(Type type in types) {
					var start = Reflector.CreateInstance(type) as IStart;
					if(null == start) { continue; }
					Attach(start);
				}
			}
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Terminate();

			foreach(IStart start in _Starts) {
				Detach(start);
			}
			_Starts = new List<IStart>();
			Stage = KernelStage.Stopped;

			PreStartFinished = false;
			BuiltinStartFinished = false;
			PreDatabaseStartFinished = false;
			DatabaseStartFinished = false;
			PostDatabaseStartFinished = false;
			PostStartFinished = false;

			GC.SuppressFinalize(this);
		}
		#endregion

		#region property
		public static Heartbeat Singleton
		{
			get
			{
				if(null == _Singleton) {
					lock(_Lock) {
						if(null == _Singleton) {
							_Singleton = new Heartbeat();
							_Singleton.Start();
						}
					}
				}
				return _Singleton;
			}
		}
		internal static Heartbeat OnlySingleton
		{
			get
			{
				if(null == _Singleton) {
					lock(_Lock) {
						if(null == _Singleton) {
							_Singleton = new Heartbeat();
						}
					}
				}
				return _Singleton;
			}
		}

		public KernelPosition Position { get; internal set; }

		private KernelStage _Stage;
		public KernelStage Stage
		{
			get
			{
				return _Stage;
			}
			private set
			{
				var originalStage = this.Stage;
				_Stage = value;

				switch(_Stage) {
					case KernelStage.Starting:
						WinEventLog.Information(string.Format(
							"{0} is starting: {1}",
							Constants.Application.Name,
							Current.AppID
						));
						break;
					case KernelStage.Stopped:
						WinEventLog.Information(string.Format(
							"{0} was stopped: {1}",
							Constants.Application.Name,
							Current.AppID
						));
						break;
					default:
						Logger.Message(
							"Kernel Stage Changed",
							new Any("Original Stage", originalStage.ToString()),
							new Any("Kernel Stage", this.Stage.ToString())
						);
						break;
				}
			}
		}

		public bool InGeneralStage
		{
			get
			{
				return Stage.In(
					KernelStage.Stopped,
					KernelStage.Running
				);
			}
		}

		public bool InTransitionalStage
		{
			get
			{
				return Stage.In(
					KernelStage.Starting,
					KernelStage.Terminating
				);
			}
		}

		public bool InWorkingStage
		{
			get
			{
				return Stage != KernelStage.Stopped;
			}
		}

		public bool PreStartFinished { get; private set; }
		public bool BuiltinStartFinished { get; private set; }
		public bool PreDatabaseStartFinished { get; private set; }
		public bool DatabaseStartFinished { get; private set; }
		public bool PostDatabaseStartFinished { get; private set; }
		public bool PostStartFinished { get; private set; }

		public DateTime StartUpTime { get; private set; }
		#endregion

		#region Attach & Detach
		private void Attach(IStart start)
		{
			if(null == start) { return; }

			if(!_Starts.Contains(start)) {
				lock(_Lock) {
					if(!_Starts.Contains(start)) {
						_Starts.Add(start);

						// Start
						PreStart += new EventHandler(start.DoPreStart);
						BuiltinStart += new EventHandler(start.DoBuiltinStart);
						PreDatabaseStart += new EventHandler(start.DoPreDatabaseStart);
						DatabaseStart += new EventHandler(start.DoDatabaseStart);
						PostDatabaseStart += new EventHandler(start.DoPostDatabaseStart);
						PostStart += new EventHandler(start.DoPostStart);

						// Terminate
						BuiltinTerminate += new EventHandler(start.DoBuiltinTerminate);
					}
				}
			}
		}

		private void Detach(IStart start)
		{
			if(null == start) { return; }

			if(_Starts.Contains(start)) {
				lock(_Lock) {
					if(_Starts.Contains(start)) {
						// Start
						PreStart -= new EventHandler(start.DoPreStart);
						BuiltinStart -= new EventHandler(start.DoBuiltinStart);
						PreDatabaseStart -= new EventHandler(start.DoPreDatabaseStart);
						DatabaseStart -= new EventHandler(start.DoDatabaseStart);
						PostDatabaseStart -= new EventHandler(start.DoPostDatabaseStart);
						PostStart -= new EventHandler(start.DoPostStart);

						// Terminate
						BuiltinTerminate -= new EventHandler(start.DoBuiltinTerminate);
					}
				}
			}
		}
		#endregion

		#region Action
		private Result Start()
		{
			Result result;

			// status check
			result = InStatus(KernelStage.Stopped | KernelStage.Terminating);
			if(!result.Success) { return result; }

			lock(_Lock) {
				try {
					using(var il = new IntervalLogger("Heartbeat.Start")) {
						il.Add("ApplicationID", Current.AppID);

						// Execute
						Position = KernelPosition.Starting;
						Stage = KernelStage.Starting;
						ExecStart();
						Stage = KernelStage.Running;
						Position = KernelPosition.Running;

						// Success
						result = Result.BuildSuccess(BuildSuccessMessage());
						Logger.Track(result.Message);

						// Audit
					}
				} catch(Exception ex) {
					WinEventLog.Error(
						ex.ToAny(new Any("Heartbeat Action", "Start")).List()
					);

					string msg = BuildErrorMessage();

					// Audit

					// Failure
					result = Result.BuildFailure<__KernelError_HeartbeatStart>(
						msg, 
						ex.ToAny()
					);
					Logger.Error(
						"Heartbeat.Start", result.Message, ex.ToAny()
					);
				}
			}

			return result;
		}

		private Result Terminate()
		{
			Result result;

			// status check
			result = InStatus(KernelStage.Starting | KernelStage.Running);
			if(!result.Success) { return result; }

			lock(_Lock) {
				try {
					using(var il = new IntervalLogger("Heartbeat.Terminate")) {
						il.Add("ApplicationID", Current.AppID);

						// Execute
						Position = KernelPosition.Terminating;
						Stage = KernelStage.Terminating;
						ExecTerminate();
						Stage = KernelStage.Stopped;
						Position = KernelPosition.Stopped;

						//Success
						result = Result.BuildSuccess(BuildSuccessMessage());
						Logger.Track(result.Message);

						// Audit
					}
				} catch(Exception ex) {
					WinEventLog.Error(
						ex.ToAny(new Any("Heartbeat Action", "Terminate")).List()
					);

					string msg = BuildErrorMessage();

					// Audit

					// Failure
					result = Result.BuildFailure<__KernelError_HeartbeatStart>(
						msg, 
						ex.ToAny()
					);
					// can not write in log anymore
					WinEventLog.Error(msg);
				}
			}

			return result;
		}
		#endregion

		#region Exec
		private void ExecStart()
		{
			if(!Current.Started) {
				// Pre
				Position = KernelPosition.PreStart;
				using(var il = new IntervalLogger("PreStart")) {
					if(null != PreStart) {
						PreStart(this, EventArgs.Empty);
					}
					PreStartFinished = true;
				}
				Position = KernelPosition.Starting;

				// Builtin
				Position = KernelPosition.BuiltinStart;
				using(var il = new IntervalLogger("BuiltinStart")) {
					if(null != BuiltinStart) {
						BuiltinStart(this, EventArgs.Empty);
					}
					BuiltinStartFinished = true;
				}
				Position = KernelPosition.Starting;

				// PreDatabase
				Position = KernelPosition.PreDatabaseStart;
				using(var il = new IntervalLogger("PreDatabaseStart")) {
					if(null != PreDatabaseStart) { 
						PreDatabaseStart(this, EventArgs.Empty);
					}
					PreDatabaseStartFinished = true;
				}
				Position = KernelPosition.Starting;

				// Database
				Position = KernelPosition.DatabaseStart;
				using(var il = new IntervalLogger("DatabaseStart")) {
					if(null != DatabaseStart) {
						DatabaseStart(this, EventArgs.Empty);
					}
					DatabaseStartFinished = true;
				}
				Position = KernelPosition.Starting;

				// PostDatabase
				Position = KernelPosition.PostDatabaseStart;
				using(var il = new IntervalLogger("PostDatabaseStart")) {
					if(null != PostDatabaseStart) {
						PostDatabaseStart(this, EventArgs.Empty);
					}
					PostDatabaseStartFinished = true;
				}
				Position = KernelPosition.Starting;

				// Post
				Position = KernelPosition.PostStart;
				using(var il = new IntervalLogger("PostStart")) {
					if(null != PostStart) {
						PostStart(this, EventArgs.Empty);
					}
					PostStartFinished = true;
				}
				Position = KernelPosition.Starting;
			}
		}

		private void ExecTerminate()
		{
			// Builtin
			Position = KernelPosition.BuiltinTerminate;
			using(var il = new IntervalLogger("BuiltinTerminate")) {
				if(null != BuiltinTerminate) {
					BuiltinTerminate(this, EventArgs.Empty);
				}
			}
			Position = KernelPosition.Terminating;
		}
		#endregion

		#region Assistance
		private Result InStatus(KernelStage stage)
		{
			if((stage & Stage) == Stage) { return Result.BuildSuccess(); }

			var result = Result.BuildFailure<__KernelError_HeartbeatStart>(
				"Improper Action",
				new Any("Action Name", new StackTrace().GetFrame(1).GetMethod().Name),
				new Any("Kernel Status", Stage.ToString())
			);
			Logger.Error("Kuick.Heartbeat", "Improper Action", result.Datas.ToArray());
			return result;
		}

		private string BuildErrorMessage()
		{
			string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
			return String.Format(
				"{0} {1} action error, but the heart still beating!",
				Current.AppID,
				methodName
			);
		}

		private string BuildSuccessMessage()
		{
			string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
			return String.Format(
				"{0} invoked {1} action successfully!",
				Current.AppID,
				methodName
			);
		}

		private string BuildForcedMessage()
		{
			string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
			return String.Format(
				"{0} invoked {0} action",
				Current.AppID,
				methodName
			);
		}

		internal static bool WorkingAssert()
		{
			if(Heartbeat.Singleton.InWorkingStage) { return true; }

			string msg = String.Format(
				"{0} was {1}, need to check log before restart service.",
				Current.AppID,
				Heartbeat.Singleton.Stage.ToString()
			);
			Logger.Error("Kuick.Kerne.Heartbeat.WorkingAssert", msg);

			return false;
		}
		#endregion

		#region static
		public static string ScopeID
		{
			get
			{
				try {
					if(Current.IsWebApplication) {
						if(null == HttpContext.Current.Session) {
							return Utility.GetUuid();
						}
						return string.Concat(
							HttpContext.Current.Session.SessionID,
							":",
							HttpContext.Current.Timestamp.Ticks.ToString()
						);
					} else {
						return Thread.CurrentThread.ManagedThreadId.ToString();
					}
				} catch(Exception ex) {
					Logger.Error(
						"Heartbeat.ScopeID",
						ex.ToAny()
					);
					return Utility.GetUuid();
				}
			}
		}

		internal static string CreateFolder(string path)
		{
			try {
				string physicalPath = path;
				if(Current.IsWebApplication && null != HttpContext.Current.Server) {
					if(path.Length < 3 || path.Substring(1, 2) != ":\\") {
						if(!path.StartWith("~", "..", "/")) { path = "~/" + path; }
						string url = ResolveUrl(path);
						physicalPath = HttpContext.Current.Server.MapPath(url);
					}
				} else {
					physicalPath = Path.Combine(
						AppDomain.CurrentDomain.BaseDirectory, 
						path
					);
				}

				if(!Directory.Exists(physicalPath)) {
					var difInfo = Directory.CreateDirectory(physicalPath);
					if(null == difInfo) {
						throw new Exception(String.Format(
							"Create folder failure: {0}", physicalPath
						));
					}
				}

				return physicalPath;
			} catch(Exception ex) {
				Logger.InnerMessage = Formator.ListAnys(ex.ToAny().ToArray());
				throw;
			}
		}

		internal static string ResolveUrl(string url)
		{
			if(Checker.IsNull(url)) {
				throw new ArgumentNullException("relativeUrl");
			}
			if(url[0] == '/' || url[0] == '\\') { return url; }

			int pos = url.IndexOf(@"://", StringComparison.Ordinal);
			if(pos != -1) {
				int index = url.IndexOf('?');
				if(index == -1 || index > pos) { return url; }
			}

			var sb = new StringBuilder();
			sb.Append(HttpRuntime.AppDomainAppVirtualPath);
			if(sb.Length == 0 || sb[sb.Length - 1] != '/') { sb.Append('/'); }

			bool hasQM = false;
			bool hasSlash = url[0] == '~' && (url[1] == '/' || url[1] == '\\');
			if(hasSlash) { url = url.Substring(2); }

			foreach(char c in url) {
				if(!hasQM) {
					if(c == '?') {
						hasQM = true;
					} else {
						if(c == '/' || c == '\\') {
							if(!hasSlash) {
								sb.Append('/');
								hasSlash = true;
							}
							continue;
						} else if(hasSlash) {
							hasSlash = false;
						}
					}
				}
				sb.Append(c);
			}

			return sb.ToString();
		}
		#endregion
	}
}
