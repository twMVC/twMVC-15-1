// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WinManagement.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-07-03 - Creation


using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace Kuick
{
	public class WinManagement
	{
		private const double TIME_OUT = 5.0d;
		private const string W3SVC = "W3SVC";

		#region IIS
		public static bool IISReset()
		{
			bool success = false;
			ServiceController sc = FindService(W3SVC);
			if(null == sc) { return success; }

			try {
				if(sc.Status == ServiceControllerStatus.Running) {
					success = StopService(sc);
					if(success) { success = StartService(sc); }
				} else if(sc.Status == ServiceControllerStatus.Stopped) {
					success = StartService(sc);
				}
			} catch(Exception ex) {
				Logger.Error("WinManagement.IISReset", ex);
			}

			return success;
		}
		#endregion

		#region Service
		public static ServiceController FindService(string serviceName)
		{
			ServiceController sc = null;
			ServiceController[] scs = ServiceController.GetServices(
				Environment.MachineName
			);
			if(Checker.IsNull(scs)) { return sc; }

			foreach(ServiceController one in scs) {
				if(null == one) { continue; }

				if(string.Compare(one.ServiceName, serviceName, true) == 0) {
					sc = one;
					break;
				}
			}
			return sc;
		}

		public static bool StopService(string serviceName)
		{
			ServiceController sc = FindService(serviceName);
			if(null == sc) { return false; }
			return StopService(sc);
		}

		public static bool StopService(ServiceController serviceController)
		{
			if(null == serviceController) {
				throw new ArgumentNullException("serviceController");
			}

			DateTime dt = DateTime.Now.AddMinutes(TIME_OUT);
			bool success = false;
			if(serviceController.CanStop) {
				serviceController.Stop();
				while(serviceController.Status != ServiceControllerStatus.Stopped) {
					serviceController.Refresh();
					Thread.Sleep(1000);
					if(dt < DateTime.Now) { return false; }
				}
				success = true;
			}
			return success;
		}

		public static bool StartService(string serviceName)
		{
			ServiceController sc = FindService(serviceName);
			if(null == sc) { return false; }
			return StartService(sc);
		}

		public static bool StartService(ServiceController serviceController)
		{
			if(null == serviceController) {
				throw new ArgumentNullException("serviceController");
			}

			DateTime dt = DateTime.Now.AddMinutes(TIME_OUT);
			serviceController.Start();
			while(serviceController.Status != ServiceControllerStatus.Running) {
				serviceController.Refresh();
				Thread.Sleep(1000);
				if(dt < DateTime.Now) { return false; }
			}
			return true;
		}
		#endregion

		#region GAC
		public static void RegisterGAC(string gacutil, string pathName)
		{
			string[] args = Environment.GetCommandLineArgs();
			FileInfo fi = new FileInfo(args[0]);
			string loc = fi.DirectoryName;
			if(!loc.EndsWith("" + Path.DirectorySeparatorChar)) {
				loc += Path.DirectorySeparatorChar;
			}

			string arg = string.Format("/if \"{0}\"", pathName);

			ProcessStartInfo psi = new ProcessStartInfo(gacutil, arg);
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			Process proc = Process.Start(psi);
			while(!proc.HasExited) {
				Thread.Sleep(1000);
			}
		}
		#endregion
	}
}
