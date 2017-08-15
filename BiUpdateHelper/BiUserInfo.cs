using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPUtil;
using Microsoft.Win32;

namespace BiUpdateHelper
{
	public static class BiUserInfo
	{
		public static List<UserInfo> users { get; private set; }
		public static UserInfo preferredUser;
		static BiUserInfo()
		{
			Reload();
		}

		public static void Reload()
		{
			RegistryKey usersKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\server\\users");
			users = new List<UserInfo>();
			foreach (string name in usersKey.GetSubKeyNames())
			{
				try
				{
					users.Add(new UserInfo(usersKey.OpenSubKey(name), name));
				}
				catch (Exception ex)
				{
					if (Program.settings.logVerbose)
						Logger.Debug(ex);
				}
			}
			preferredUser = null;
			foreach (UserInfo user in users)
				if (user.name.ToLower() != "admin" && user.name.ToLower() != "anonymous" && string.IsNullOrEmpty(user.selgroups))
					preferredUser = user;
			if (preferredUser == null)
				foreach (UserInfo user in users)
					if (user.name.ToLower() != "admin" && user.name.ToLower() != "anonymous" && user.admin)
						preferredUser = user;
			if (preferredUser == null && users.Count > 0)
				preferredUser = users[0];
		}
	}
	public class UserInfo
	{
		public readonly string name;
		public readonly bool admin;
		private readonly string password_encoded;
		public readonly string selgroups;

		public UserInfo(RegistryKey key, string name)
		{
			this.name = name;
			admin = key.GetValue("admin").ToString() == "1";
			password_encoded = key.GetValue("password").ToString();
			selgroups = key.GetValue("selgroups").ToString();
		}
		public string GetDecodedPassword()
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(password_encoded));
		}
	}
}
