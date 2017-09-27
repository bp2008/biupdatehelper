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
			RegistryKey server = RegistryUtil.HKLM.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\server");
			if (server == null)
				return;
			enabled = RegistryUtil.GetStringValue(server, "enable") == "1";
			if (enabled)
			{
				try
				{
					lanIp = RegistryUtil.GetStringValue(server, "lanip");
					port = RegistryUtil.GetIntValue(server, "port", 80);
					authenticate = (AuthenticationMode)RegistryUtil.GetIntValue(server, "authenticate", 0);
					secureonly = RegistryUtil.GetStringValue(server, "secureonly") == "1";
				}
				catch
				{
					enabled = false;
				}
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
