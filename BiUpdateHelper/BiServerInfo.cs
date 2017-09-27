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
			enabled = GetStringValue(server, "enable") == "1";
			if (enabled)
			{
				try
				{
					lanIp = GetStringValue(server, "lanip");
					port = int.Parse(GetStringValue(server, "port"));
					authenticate = (AuthenticationMode)int.Parse(GetStringValue(server, "authenticate"));
					secureonly = GetStringValue(server, "secureonly") == "1";
				}
				finally
				{
					enabled = false;
				}
			}
		}
		private static string GetStringValue(RegistryKey key, string name)
		{
			object obj = key.GetValue(name);
			if (obj == null)
				return "";
			return obj.ToString();
		}
	}
	public enum AuthenticationMode
	{
		No = 0,
		All_connections = 1,
		Non_LAN_only = 2
	}
}
