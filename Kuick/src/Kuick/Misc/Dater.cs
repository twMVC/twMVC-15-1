// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Dater.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class Dater
	{
		#region Year
		public static DateTime StartOfYear()
		{
			return StartOfYear(DateTime.Now);
		}

		public static DateTime StartOfYear(DateTime timeStamp)
		{
			return StartOfYear(timeStamp.Year);
		}

		public static DateTime StartOfYear(int year)
		{
			return new DateTime(year, 1, 1, 0, 0, 0, 0);
		}

		public static DateTime EndOfYear()
		{
			return EndOfYear(DateTime.Now);
		}

		public static DateTime EndOfYear(DateTime timeStamp)
		{
			return EndOfYear(timeStamp.Year);
		}

		public static DateTime EndOfYear(int year)
		{
			return new DateTime(
				year,
				12,
				DateTime.DaysInMonth(year, 12),
				23,
				59,
				59,
				999
			);
		}
		#endregion

		#region Month
		public static DateTime StartOfMonth()
		{
			return StartOfMonth(DateTime.Now);
		}
		public static DateTime StartOfMonth(DateTime timeStamp)
		{
			return StartOfMonth(timeStamp.Year, timeStamp.Month);
		}
		public static DateTime StartOfMonth(int year, int month)
		{
			return new DateTime(
				year,
				month,
				1,
				0,
				0,
				0,
				0
			);
		}

		public static DateTime EndOfMonth()
		{
			return EndOfMonth(DateTime.Now);
		}

		public static DateTime EndOfMonth(DateTime timeStamp)
		{
			return EndOfMonth(timeStamp.Year, timeStamp.Month);
		}

		public static DateTime EndOfMonth(int year, int month)
		{
			return new DateTime(
				year,
				month,
				DateTime.DaysInMonth(year, month),
				23,
				59,
				59,
				999
			);
		}
		#endregion

		#region Week
		public static DateTime StartOfWeek()
		{
			return StartOfWeek(DateTime.Now);
		}
		public static DateTime StartOfWeek(DayOfWeek firstDayOfWeek)
		{
			return StartOfWeek(DateTime.Now, firstDayOfWeek);
		}
		public static DateTime StartOfWeek(DateTime timeStamp)
		{
			return StartOfWeek(timeStamp.Year, timeStamp.Month, timeStamp.Day);
		}
		public static DateTime StartOfWeek(DateTime timeStamp, DayOfWeek firstDayOfWeek)
		{
			return StartOfWeek(
				timeStamp.Year,
				timeStamp.Month,
				timeStamp.Day,
				firstDayOfWeek
			);
		}
		public static DateTime StartOfWeek(int year, int month, int day)
		{
			return StartOfWeek(year, month, day, DayOfWeek.Sunday);
		}
		public static DateTime StartOfWeek(
			int year,
			int month,
			int day,
			DayOfWeek firstDayOfWeek)
		{
			DateTime date = new DateTime(year, month, day, 0, 0, 0);
			int difference = (int)firstDayOfWeek - (int)date.DayOfWeek;
			if(difference > 0) { difference -= 6; }
			return date.AddDays(difference);
		}

		public static DateTime EndOfWeek()
		{
			return EndOfWeek(DateTime.Now);
		}
		public static DateTime EndOfWeek(DayOfWeek firstDayOfWeek)
		{
			return EndOfWeek(DateTime.Now, firstDayOfWeek);
		}
		public static DateTime EndOfWeek(DateTime timeStamp)
		{
			return EndOfWeek(timeStamp.Year, timeStamp.Month, timeStamp.Day);
		}
		public static DateTime EndOfWeek(DateTime timeStamp, DayOfWeek firstDayOfWeek)
		{
			return EndOfWeek(timeStamp.Year, timeStamp.Month, timeStamp.Day, firstDayOfWeek);
		}
		public static DateTime EndOfWeek(int year, int month, int day)
		{
			return EndOfWeek(year, month, day, DayOfWeek.Sunday);
		}
		public static DateTime EndOfWeek(
			int year,
			int month,
			int day,
			DayOfWeek firstDayOfWeek)
		{
			DateTime date = new DateTime(year, month, day, 23, 59, 59);
			int difference = (int)firstDayOfWeek - (int)date.DayOfWeek;
			if(difference > 0) { difference -= 6; }
			return date.AddDays(difference + 6);
		}
		#endregion

		#region Day
		public static DateTime StartOfDay()
		{
			return StartOfDay(DateTime.Now);
		}

		public static DateTime StartOfDay(DateTime timeStamp)
		{
			return StartOfDay(timeStamp.Year, timeStamp.Month, timeStamp.Day);
		}

		public static DateTime StartOfDay(int year, int month, int day)
		{
			return new DateTime(year, month, day, 0, 0, 0, 0);
		}

		public static DateTime EndOfDay()
		{
			return EndOfDay(DateTime.Now);
		}

		public static DateTime EndOfDay(DateTime timeStamp)
		{
			return EndOfDay(timeStamp.Year, timeStamp.Month, timeStamp.Day);
		}

		public static DateTime EndOfDay(int year, int month, int day)
		{
			return new DateTime(year, month, day, 23, 59, 59, 999);
		}

		public static bool IsToday(DateTime timeStamp)
		{
			DateTime now = DateTime.Now;
			if(timeStamp.Day != now.Day) { return false; }
			if(timeStamp.Month != now.Month) { return false; }
			if(timeStamp.Year != now.Year) { return false; }
			return true;
		}
		#endregion

		#region Hour
		public static DateTime StartOfHour()
		{
			return StartOfHour(DateTime.Now);
		}
		public static DateTime StartOfHour(DateTime timeStamp)
		{
			return StartOfHour(
				timeStamp.Year,
				timeStamp.Month,
				timeStamp.Day,
				timeStamp.Hour
			);
		}
		public static DateTime StartOfHour(int year, int month, int day, int hour)
		{
			return new DateTime(year, month, day, hour, 0, 0, 0);
		}

		public static DateTime EndOfHour()
		{
			return EndOfHour(DateTime.Now);
		}
		public static DateTime EndOfHour(DateTime timeStamp)
		{
			return EndOfHour(timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour);
		}
		public static DateTime EndOfHour(int year, int month, int day, int hour)
		{
			return new DateTime(year, month, day, hour, 59, 59, 999);
		}
		#endregion

		#region Minute
		public static DateTime StartOfMinute()
		{
			return StartOfMinute(DateTime.Now);
		}
		public static DateTime StartOfMinute(DateTime timeStamp)
		{
			return StartOfMinute(
				timeStamp.Year,
				timeStamp.Month,
				timeStamp.Day,
				timeStamp.Hour,
				timeStamp.Minute
			);
		}
		public static DateTime StartOfMinute(
			int year,
			int month,
			int day,
			int hour,
			int minute)
		{
			return new DateTime(year, month, day, hour, minute, 0, 0);
		}

		public static DateTime EndOfMinute()
		{
			return EndOfMinute(DateTime.Now);
		}
		public static DateTime EndOfMinute(DateTime timeStamp)
		{
			return EndOfMinute(
				timeStamp.Year,
				timeStamp.Month,
				timeStamp.Day,
				timeStamp.Hour,
				timeStamp.Minute
			);
		}
		public static DateTime EndOfMinute(int year, int month, int day, int hour, int minute)
		{
			return new DateTime(year, month, day, hour, minute, 59, 999);
		}
		#endregion

		#region Others
		public static int GetDaysBetweenDates(DateTime oldTimeStamp, DateTime newTimeStamp)
		{
			TimeSpan ts = (newTimeStamp - oldTimeStamp);
			return (int)Calculator.Round45(ts.TotalDays);
		}

		public static int GetHoursBetweenDates(DateTime oldTimeStamp, DateTime newTimeStamp)
		{
			TimeSpan ts = (newTimeStamp - oldTimeStamp);
			return (int)Calculator.Round45(ts.TotalHours);
		}

		public static int GetMinutesBetweenDates(DateTime oldTimeStamp, DateTime newTimeStamp)
		{
			TimeSpan ts = (newTimeStamp - oldTimeStamp);
			return (int)Calculator.Round45(ts.TotalMinutes);
		}

		public static double GetTotalDaysBetweenDates(
			DateTime oldTimeStamp,
			DateTime newTimeStamp)
		{
			TimeSpan ts = (newTimeStamp - oldTimeStamp);
			return ts.TotalDays;
		}

		public static bool IsWorkDay(DateTime timeStamp)
		{
			return !IsWeekend(timeStamp);
		}

		public static bool IsWeekend(DateTime timeStamp)
		{
			return
				timeStamp.DayOfWeek == DayOfWeek.Saturday
				||
				timeStamp.DayOfWeek == DayOfWeek.Sunday;
		}

		public static bool InCurrentMonth(int year, int month)
		{
			return InCurrentMonth(new DateTime(year, month, 1));
		}

		public static bool InCurrentMonth(DateTime timeStamp)
		{
			return InTheSameMonth(DateTime.Now, timeStamp);
		}

		private static bool InTheSameMonth(DateTime dateOne, DateTime dateTwo)
		{
			return
				dateOne.Year == dateTwo.Year
				&&
				dateOne.Month == dateTwo.Month;
		}
		#endregion
	}
}
