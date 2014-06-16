// Kernel.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Kuicker
{
	public sealed class Kernel : IDisposable
	{
		#region field
		private DateTime StartUpTime = DateTime.Now;
		private static object _BlockLock = new object();
		private static object _PieceLock = new object();
		private static Kernel _Current;
		private ReadOnlyDictionary<string, ILifeCycle> _LifeCycles;
		private ReadOnlyDictionary<string, IBuiltin> _Builtins;
		private ReadOnlyDictionary<string, IPlugin> _Plugins;

		private IKernelLifeCycle _KernelLifeCycle;
		private IEnumerable<ILifeCycle> _OtherLifeCycles;
		#endregion

		#region constructor
		private Kernel()
		{
			this.Register();

			this.Position = KernelPosition.Stopped;
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			if(Stop()) {
				Position = KernelPosition.Stopped;
			} else {
				//WinEventLog.Error(
				//	string.Format(
				//		"An error occurred during the {0} invoke stop action.",
				//		Config.Kernel.AppID
				//	)
				//);
			}

			//Logger.Stop(ref _Listener);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region singleton
		public static Kernel Current
		{
			get
			{
				if(null == _Current) {
					lock(_BlockLock) {
						if(null == _Current) {
							_Current = new Kernel();
							if(_Current.Start()) {
								_Current.Position = KernelPosition.Running;
							} else {
								_Current = null;
							}
						}
					}
				}
				return _Current;
			}
		}
		#endregion

		#region property
		public KernelStatus Status { get; private set; }
		private KernelPosition _Position;
		public KernelPosition Position
		{
			get
			{
				return _Position;
			}
			private set
			{
				_Position = value;
				switch(value) {
					case KernelPosition.Stopped:
						Status = KernelStatus.Stopped;
						break;
					case KernelPosition.BeforeStart:
					case KernelPosition.BeforeBuiltinStart:
					case KernelPosition.BuiltinStart:
					case KernelPosition.AfterBuiltinStart:
					case KernelPosition.BeforePluginStart:
					case KernelPosition.PluginStart:
					case KernelPosition.AfterPluginStart:
					case KernelPosition.AfterStart:
						Status = KernelStatus.Starting;
						break;
					case KernelPosition.Running:
						Status = KernelStatus.Running;
						break;
					case KernelPosition.BeforeStop:
					case KernelPosition.PluginStop:
					case KernelPosition.BuiltinStop:
					case KernelPosition.AfterStop:
						Status = KernelStatus.Stopping;
						break;
					default:
						throw ExHelper.NotImplementedEnum(value);
				}
			}
		}
		#endregion

		#region Action
		private bool Start()
		{
			if(KernelPosition.Stopped == Position) {
				lock(_BlockLock) {
					if(KernelPosition.Stopped == Position) {
						using(var il = new IntervalLogger(LogLevel.Info)) {
							try {
								InvokeStart(il);
								return true;
							} catch(Exception ex) {
								il.Level = LogLevel.Error;
								il.Add(ex);
								il.Add("KernelPosition", Position);
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		private bool Stop()
		{
			if(KernelPosition.Running == Position) {
				lock(_BlockLock) {
					if(KernelPosition.Running == Position) {
						using(var il = new IntervalLogger(LogLevel.Info)) {
							try {
								InvokeStop(il);
								return true;
							} catch(Exception ex) {
								il.Level = LogLevel.Error;
								il.Add(ex);
								il.Add("KernelPosition", Position);
								return false;
							}
						}
					}
				}
			}
			return true;
		}
		#endregion

		#region Invoke
		private void InvokeStart(IntervalLogger il)
		{
			// BeforeStart
			Position = KernelPosition.BeforeStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoBeforeStart();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoBeforeStart()
				);
			}

			// BeforeBuiltinStart
			Position = KernelPosition.BeforeBuiltinStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoBeforeBuiltinStart();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoBeforeBuiltinStart()
				);
			}

			// BuiltinStart
			Position = KernelPosition.BuiltinStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoBuiltinStart();
			}

			// AfterBuiltinStart
			Position = KernelPosition.AfterBuiltinStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoAfterBuiltinStart();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoAfterBuiltinStart()
				);
			}

			// BeforePluginStart
			Position = KernelPosition.BeforePluginStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoBeforePluginStart();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoBeforePluginStart()
				);
			}

			// PluginStart
			Position = KernelPosition.PluginStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoPluginStart();
			}

			// AfterPluginStart
			Position = KernelPosition.AfterPluginStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoAfterPluginStart();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoAfterPluginStart()
				);
			}

			// AfterStart
			Position = KernelPosition.AfterStart;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoAfterStart();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoAfterStart()
				);
			}
		}

		private void InvokeStop(IntervalLogger il)
		{
			// BeforeStop
			Position = KernelPosition.BeforeStop;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoBeforeStop();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoBeforeStop()
				);
			}

			// PluginStop
			Position = KernelPosition.PluginStop;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoPluginStop();
			}

			// BuiltinStop
			Position = KernelPosition.BuiltinStop;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoBuiltinStop();
			}

			// AfterStop
			Position = KernelPosition.AfterStop;
			using(var iil = new IILogger(il, Position)) {
				_KernelLifeCycle.DoAfterStop();
				Parallel.ForEach(
					_OtherLifeCycles, x => x.DoAfterStop()
				);
			}
		}
		#endregion

		#region private
		private void Register<T>(Assembly asm, IDictionary<string, T> list) 
			where T : class
		{
			var types = Reflector.GatherByInterface<T>(
				asm
			);
			if(types.Any()) {
				foreach(var type in types) {
					var one = Activator.CreateInstance(type) as T;
					if(null == one) { continue; }

					string name = type.FullName;

					if(typeof(T).IsDerived<ILifeCycle>()) {
						ILifeCycle lifeCycle = one as ILifeCycle;
						list.Add(name, one);
						continue;
					}
					if(typeof(T).IsDerived<IBuiltin>()) {
						IBuiltin builtin = one as IBuiltin;
						list.Add(builtin.Name, one);
						continue;
					}
					if(typeof(T).IsDerived<IPlugin>()) {
						IPlugin plugin = one as IPlugin;
						list.Add(plugin.Name, one);
						continue;
					}
				}
			}
		}

		private void Register()
		{
			var lifeCycles = new Dictionary<string, ILifeCycle>();
			var builtins = new Dictionary<string, IBuiltin>();
			var plugins = new Dictionary<string, IPlugin>();

			using(var il = new IntervalLogger(LogLevel.Debug)) {
				foreach(var asm in Reflector.Assemblies) {
					try {
						Register(asm, lifeCycles);
						Register(asm, builtins);
						Register(asm, plugins);
					} catch(Exception ex) {
						il.Level = LogLevel.Error;
						il.Add(ex);
					}
				}
			}

			_LifeCycles = new ReadOnlyDictionary<string, ILifeCycle>(lifeCycles);
			_Builtins = new ReadOnlyDictionary<string, IBuiltin>(builtins);
			_Plugins = new ReadOnlyDictionary<string, IPlugin>(plugins);

			// KernelLifeCycle
			var kvp = lifeCycles.FirstOrDefault(x =>
				x.IsDerived<IKernelLifeCycle>()
			);
			_KernelLifeCycle = kvp.Value as IKernelLifeCycle;

			// ILifeCycle
			var kvps = lifeCycles.Where(x =>
				!x.IsDerived<IKernelLifeCycle>()
			);
			if(kvps.Any()) {
				_OtherLifeCycles = kvps
					.Select<
						KeyValuePair<string, ILifeCycle>, 
						ILifeCycle
						>((x, i) => 
							x.Value
					);
			}
			if(null == _OtherLifeCycles) { 
				_OtherLifeCycles = Enumerable.Empty<ILifeCycle>();
			}

		}
		#endregion
	}
}
