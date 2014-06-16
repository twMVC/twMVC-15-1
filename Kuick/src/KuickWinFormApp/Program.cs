using System;
using System.Windows.Forms;
using Kuick;

namespace KuickWinFormApp
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			using(Heartbeat hb = Heartbeat.Singleton) {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
		}
	}
}
