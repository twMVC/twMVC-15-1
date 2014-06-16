// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WinWMI.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-07-03 - Creation


using System;
using System.Collections.Generic;
using System.Management;

namespace Kuick
{
	public class WinWMI
	{
		public static string GetSystemDirectoryVolumeSerial()
		{
			try {
				string letter = Formator.AirBag(
					Environment.SystemDirectory.Split(
						Constants.Symbol.Char.Colon
					)[0].ToString(),
					Constants.Default.SystemDirectoryLetter
				);

				ManagementObject disk = new ManagementObject(
					String.Format("win32_logicaldisk.deviceid=\"{0}:\"", letter)
				);
				disk.Get();
				return disk["VolumeSerialNumber"].ToString();
			} catch(Exception) {
			}
			return string.Empty;
		}

		public static string[] GetMACAddresses()
		{
			ManagementClass mc = new ManagementClass(
				"Win32_NetworkAdapterConfiguration"
			);
			ManagementObjectCollection moc = mc.GetInstances();
			List<string> list = new List<string>();
			foreach(ManagementObject mo in moc) {
				if(true == (bool)mo["IPEnabled"]) {
					list.Add(mo["MacAddress"].ToString());
				}
				mo.Dispose();
			}
			return list.ToArray();
		}

		public static string[] GetCPUIDs()
		{
			ManagementClass mc = new ManagementClass("Win32_Processor");
			ManagementObjectCollection moc = mc.GetInstances();
			List<string> list = new List<string>();
			foreach(ManagementObject mo in moc) {
				list.Add(mo.Properties["ProcessorId"].Value.ToString());
				mo.Dispose();
			}
			return list.ToArray();
		}
	}
}
