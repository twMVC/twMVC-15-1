using System;
using Kuick;

namespace KuickConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			using(Heartbeat hb = Heartbeat.Singleton) {
				// code here
			}
		}
	}
}
