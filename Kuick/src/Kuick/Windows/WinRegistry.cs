// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WinRegistry.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-07-03 - Creation


using System;
using Microsoft.Win32;

namespace Kuick
{
	public class WinRegistry
	{
		public static void DeleteLocalMachineRegistryValue(
			string subKeyName, string name)
		{
			DeleteRegistryValue(Registry.LocalMachine, subKeyName, name);
		}

		public static void DeleteLocalMachineRegistryKey(string subKeyName)
		{
			DeleteRegistryKey(Registry.LocalMachine, subKeyName);
		}

		public static void DeleteCurrentUserRegistryValue(
			string subKeyName, string name)
		{
			DeleteRegistryValue(Registry.CurrentUser, subKeyName, name);
		}

		public static void DeleteCurrentUserRegistryKey(string subKeyName)
		{
			DeleteRegistryKey(Registry.CurrentUser, subKeyName);
		}

		public static void SetLocalMachineRegistryValue(
			string subKeyName, string name, Object val)
		{
			SetRegistryValue(Registry.LocalMachine, subKeyName, name, val);
		}

		public static Object GetLocalMachineRegistryValue(
			string subKeyName, string name)
		{
			return GetRegistryValue(Registry.LocalMachine, subKeyName, name);
		}

		public static string[] GetLocalMachineRegistryValueNames(string subKeyName)
		{
			return GetRegistryValueNames(Registry.LocalMachine, subKeyName);
		}

		public static string[] GetLocalMachineRegistrySubKeyNames(string subKeyName)
		{
			return GetRegistrySubKeyNames(Registry.LocalMachine, subKeyName);
		}

		public static void SetCurrentUserRegistryValue(
			string subKeyName, string name, Object val)
		{
			SetRegistryValue(Registry.CurrentUser, subKeyName, name, val);
		}

		public static Object GetCurrentUserRegistryValue(
			string subKeyName, string name)
		{
			return GetRegistryValue(Registry.CurrentUser, subKeyName, name);
		}

		public static string[] GetCurrentUserRegistryValueNames(string subKeyName)
		{
			return GetRegistryValueNames(Registry.CurrentUser, subKeyName);
		}

		public static string[] GetCurrentUserRegistrySubKeyNames(string subKeyName)
		{
			return GetRegistrySubKeyNames(Registry.CurrentUser, subKeyName);
		}

		public static void SetRegistryValue(
			RegistryKey key, string subKeyName, string valueName, Object val)
		{
			RegistryKey subKey = key.OpenSubKey(subKeyName, true);
			try {
				if(null == subKey) { subKey = key.CreateSubKey(subKeyName); }
				if(null != subKey) { subKey.SetValue(valueName, val); }
			} catch(Exception) {
				// swallow
			} finally {
				if(null != subKey) { subKey.Close(); }
			}
		}

		public static Object GetRegistryValue(
			RegistryKey key, string subKeyName, string valueName)
		{
			Object obj = null;
			key = key.OpenSubKey(subKeyName);
			if(null != key) { obj = key.GetValue(valueName); }
			if(null != key) { key.Close(); }
			return obj;
		}

		public static string[] GetRegistryValueNames(
			RegistryKey key, string subKeyName)
		{
			string[] names = null;
			key = key.OpenSubKey(subKeyName);
			if(null != key) { names = key.GetValueNames(); }
			if(null != key) { key.Close(); }
			return names;
		}

		public static string[] GetRegistrySubKeyNames(
			RegistryKey key, string subKeyName)
		{
			string[] names = null;
			key = key.OpenSubKey(subKeyName);
			if(null != key) { names = key.GetSubKeyNames(); }
			if(null != key) { key.Close(); }
			return names;
		}

		public static void DeleteRegistryValue(
			RegistryKey key, string subKeyName, string valueName)
		{
			key = key.OpenSubKey(subKeyName, true);
			if(null != key) { key.DeleteValue(valueName); }
			if(null != key) { key.Close(); }
		}

		public static void DeleteRegistryKey(RegistryKey key, string keyName)
		{
			key.DeleteSubKeyTree(keyName);
		}
	}
}
