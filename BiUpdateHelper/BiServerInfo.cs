using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BiUpdateHelper
{
	public static class BiServerInfo
	{
		public static string lanIp { get; private set; }
		public static int port { get; private set; }
		public static bool enabled { get; private set; }
		public static bool secureonly { get; private set; }
		public static AuthenticationMode authenticate { get; private set; }
		static BiServerInfo()
		{
			Reload();
		}

		public static void Reload()
		{
			RegistryKey server = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\server");
			enabled = server.GetValue("enable").ToString() == "1";
			if (enabled)
			{
				lanIp = server.GetValue("lanip").ToString();
				port = int.Parse(server.GetValue("port").ToString());
				authenticate = (AuthenticationMode)int.Parse(server.GetValue("authenticate").ToString());
				secureonly = server.GetValue("secureonly").ToString() == "1";
			}
		}
	}
	public enum AuthenticationMode
	{
		No = 0,
		All_connections = 1,
		Non_LAN_only = 2
	}
}
