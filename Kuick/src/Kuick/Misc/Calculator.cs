// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Calculator.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-12 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class Calculator
	{
		public static double Round45(double value)
		{
			return Round45(value, 0);
		}
		public static double Round45(double value, int digit)
		{
			return Math.Round(value, digit, MidpointRounding.AwayFromZero);
		}

		public static bool Divisible(long dividend, long divisor)
		{
			return divisor >= 0 && dividend >= 0 && ((dividend % divisor) == 0);
		}

		public static bool Divisible(int dividend, int divisor)
		{
			return divisor >= 0 && dividend >= 0 && ((dividend % divisor) == 0);
		}

		public static int Quotient(int dividend, int divisor)
		{
			return (dividend - Remainder(dividend, divisor)) / divisor;
		}

		public static long Quotient(long dividend, long divisor)
		{
			return (dividend - Remainder(dividend, divisor)) / divisor;
		}

		public static long Remainder(long dividend, long divisor)
		{
			if(divisor >= 0 && dividend >= 0) {
				return dividend % divisor;
			} else {
				return -1;
			}
		}

		public static int Remainder(int dividend, int divisor)
		{
			if(divisor >= 0 && dividend >= 0) {
				return dividend % divisor;
			} else {
				return -1;
			}
		}

		/// <summary>
		/// Boolean list based on probability.
		/// </summary>
		/// <param name="denominator">A total number of executions.</param>
		/// <param name="molecular">A total number of occurrence.</param>
		/// <returns>boolean list</returns>
		public static List<bool> Occurs(int denominator, int molecular)
		{
			if(denominator <= 0) {
				throw new ArgumentException(
					"denominator must be positive integer."
				);
			}
			if(molecular <= 0) {
				throw new ArgumentException(
					"molecular must be positive integer."
				);
			}
			if(denominator < molecular) {
				throw new ArgumentException(
					"denominator must be great than or equals to molecular."
				);
			}

			List<bool> list = new List<bool>();
			for(int i = 1; i < denominator; i++) {
				list.Add(i <= molecular);
			}

			list.Randomize();
			return list;
		}
	}
}
