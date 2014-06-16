// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Utility.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace Kuick
{
	public class Utility
	{
		private static readonly Random _Random = new Random();
		public static int GetRandom(int min, int max)
		{
			using(var rng = new RNGCryptoServiceProvider()) {
				return rng.Next(min, max);
			}
		}

		public static string GetUuid()
		{
			return Base32Encoding.ToString(Guid.NewGuid().ToByteArray());
		}

		private static object _TicksLock = new object();
		private static double _PreTicks = 0;
		public static double GetUniqueTicks()
		{
			lock(_TicksLock) {
				double ticks = Calculator.Round45(DateTime.Now.Ticks / Math.Pow(10, 5), 0);
				if(ticks <= _PreTicks) { ticks = _PreTicks + 1; }
				_PreTicks = ticks;
				return _PreTicks;
			}
		}

		public static string GetFileName(string filePath)
		{
			string fileName = GetFileNameWithExt(filePath);
			if(
				!Checker.IsNull(fileName)
				&&
				fileName.Contains(Constants.Symbol.Period)) {
				int pos = fileName.LastIndexOf(Constants.Symbol.Char.Period);
				if(pos >= 0) { fileName = fileName.Substring(0, pos); }
			}

			return fileName;
		}

		public static string GetFileNameWithExt(string filePath)
		{
			string fileNameWithExt = filePath;
			if(
				!Checker.IsNull(fileNameWithExt)
				&&
				fileNameWithExt.Contains(Path.DirectorySeparatorChar.ToString())) {
				int pos = fileNameWithExt.LastIndexOf(Path.DirectorySeparatorChar);
				fileNameWithExt = filePath.Substring(pos + 1);
			}

			return fileNameWithExt;
		}

		public static string ToPhysicalFilePath(string virtualPath)
		{
			return Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				virtualPath
			);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <example>
		/// 789 >> 24 >> 6
		/// </example>
		public static int SumIntoSingleDigit(int value)
		{
			while(value > 9) {
				value = SumIntoSingleDigit((value / 10) + (value % 10));
			}
			return value;
		}

		#region Base32
		// http://stackoverflow.com/questions/641361/base32-decoding
		public class Base32Encoding
		{
			public static byte[] ToBytes(string input)
			{
				if(string.IsNullOrEmpty(input)) {
					throw new ArgumentNullException("input");
				}

				input = input.TrimEnd('='); //remove padding characters
				int byteCount = input.Length * 5 / 8; //this must be TRUNCATED
				byte[] returnArray = new byte[byteCount];

				byte curByte = 0, bitsRemaining = 8;
				int mask = 0, arrayIndex = 0;

				foreach(char c in input) {
					int cValue = CharToValue(c);

					if(bitsRemaining > 5) {
						mask = cValue << (bitsRemaining - 5);
						curByte = (byte)(curByte | mask);
						bitsRemaining -= 5;
					} else {
						mask = cValue >> (5 - bitsRemaining);
						curByte = (byte)(curByte | mask);
						returnArray[arrayIndex++] = curByte;
						curByte = (byte)(cValue << (3 + bitsRemaining));
						bitsRemaining += 3;
					}
				}

				//if we didn't end with a full byte
				if(arrayIndex != byteCount) {
					returnArray[arrayIndex] = curByte;
				}

				return returnArray;
			}

			public static string ToString(byte[] input)
			{
				if(input == null || input.Length == 0) {
					throw new ArgumentNullException("input");
				}

				int charCount = (int)Math.Ceiling(input.Length / 5d * 8);
				char[] returnArray = new char[charCount];

				byte nextChar = 0, bitsRemaining = 5;
				int arrayIndex = 0;

				foreach(byte b in input) {
					nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
					returnArray[arrayIndex++] = ValueToChar(nextChar);

					if(bitsRemaining < 4) {
						nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
						returnArray[arrayIndex++] = ValueToChar(nextChar);
						bitsRemaining += 5;
					}

					bitsRemaining -= 3;
					nextChar = (byte)((b << bitsRemaining) & 31);
				}

				//if we didn't end with a full char
				if(arrayIndex != charCount) {
					returnArray[arrayIndex++] = ValueToChar(nextChar);
					while(arrayIndex != charCount)
						returnArray[arrayIndex++] = '='; //padding
				}

				return new string(returnArray);
			}

			private static int CharToValue(char c)
			{
				int value = (int)c;

				//65-90 == uppercase letters
				if(value < 91 && value > 64) {
					return value - 65;
				}
				//50-55 == numbers 2-7
				if(value < 56 && value > 49) {
					return value - 24;
				}
				//97-122 == lowercase letters
				if(value < 123 && value > 96) {
					return value - 97;
				}

				throw new ArgumentException("Character is not a Base32 character.", "c");
			}

			private static char ValueToChar(byte b)
			{
				if(b < 26) {
					return (char)(b + 65);
				}

				if(b < 32) {
					return (char)(b + 24);
				}

				throw new ArgumentException("Byte is not a value Base32 value.", "b");
			}

		}
		#endregion

		#region IQueryable
		public IQueryable<T> EmptyQueryable<T>()
		{
			return Enumerable.Empty<T>().AsQueryable();
		}
		#endregion
	}
}
