// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public class Formator
	{
		#region Switch
		public static void Switch(ref DateTime value0, ref DateTime value9)
		{
			if(value0 > value9) {
				DateTime tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref int value0, ref int value9)
		{
			if(value0 > value9) {
				int tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref decimal value0, ref decimal value9)
		{
			if(value0 > value9) {
				decimal tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref long value0, ref long value9)
		{
			if(value0 > value9) {
				long tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref float value0, ref float value9)
		{
			if(value0 > value9) {
				float tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}

		public static void Switch(ref byte value0, ref byte value9)
		{
			if(value0 > value9) {
				byte tmp = value0;
				value0 = value9;
				value9 = tmp;
			}
		}
		#endregion
	}
}
