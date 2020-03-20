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
		//public static List<UserInfo> users { get; private set; }
		//public static UserInfo preferredUser;
		/// <summary>
		/// Creates or updates a temporary BI web server user for anonymous performance data collection.
		/// </summary>
		/// <returns></returns>
		public static UserInfo CreateTemporaryUser()
		{
			UserInfo me = new UserInfo("biupdatehelperuser", StringUtil.GetRandomAlphaNumericString(16));
			me.admin = true;
			me.noalerts = true;
			me.lanonly = false;
			me.Commit();
			return me;
		}
		//static BiUserInfo()
		//{
		//	Reload();
		//}

		//public static void Reload()
		//{
		//	users = new List<UserInfo>();
		//	foreach (string name in usersKey.GetSubKeyNames())
		//	{
		//		try
		//		{
		//			users.Add(new UserInfo(usersKey.OpenSubKey(name), name));
		//		}
		//		catch (Exception ex)
		//		{
		//			if (Program.settings.logVerbose)
		//				Logger.Debug(ex);
		//		}
		//	}
		//	preferredUser = null;
		//	foreach (UserInfo user in users)
		//		if (preferredUser == null && !BadName(user) && user.admin && string.IsNullOrEmpty(user.selgroups))
		//			preferredUser = user;
		//	if (preferredUser == null)
		//		foreach (UserInfo user in users)
		//			if (preferredUser == null && !BadName(user) && string.IsNullOrEmpty(user.selgroups))
		//				preferredUser = user;
		//	if (preferredUser == null)
		//		foreach (UserInfo user in users)
		//			if (preferredUser == null && !BadName(user) && user.admin)
		//				preferredUser = user;
		//	if (preferredUser == null && users.Count > 0)
		//		preferredUser = users[0];
		//}
		//private static bool BadName(UserInfo user)
		//{
		//	string nameLower = user.name.ToLower();
		//	return nameLower == "admin" || nameLower == "anonymous" || nameLower == "local_console";
		//}
	}
	public class UserInfo
	{
		public string name;
		public bool admin;
		/// <summary>
		/// Password, encoded as Base64.
		/// </summary>
		public string password_encoded;
		public string selgroups;
		public bool noalerts;
		public bool lanonly;

		public UserInfo(string name, string password_raw)
		{
			this.name = name;
			this.password_encoded = EncodePassword(password_raw);
		}
		public UserInfo(RegistryKey key, string name)
		{
			this.name = name;
			admin = RegistryUtil.GetStringValue(key, "admin") == "1";
			password_encoded = RegistryUtil.GetStringValue(key, "password");
			selgroups = RegistryUtil.GetStringValue(key, "selgroups");
			noalerts = RegistryUtil.GetStringValue(key, "noalerts") == "1";
			lanonly = RegistryUtil.GetStringValue(key, "lanonly") == "1";
		}
		public string GetDecodedPassword()
		{
			return ByteUtil.Utf8NoBOM.GetString(Convert.FromBase64String(password_encoded));
		}
		public static string EncodePassword(string password_raw)
		{
			return Convert.ToBase64String(ByteUtil.Utf8NoBOM.GetBytes(password_raw));
		}

		/// <summary>
		/// Adds or updates this user in the registry.
		/// </summary>
		public void Commit()
		{
			RegistryKey usersKey = RegistryUtil.HKLM.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\server\\users", true);
			RegEdit edit = new RegEdit(usersKey.CreateSubKey(name));
			edit.DWord("admin", admin ? 1 : 0);
			edit.String("password", password_encoded);
			edit.String("selgroups", selgroups);
			edit.DWord("selgroups", noalerts ? 1 : 0);
			edit.DWord("lanonly", lanonly ? 1 : 0);
		}
		/// <summary>
		/// <para>Deletes the user with this name from the registry.</para>
		/// <para>DANGEROUS because Blue Iris may write additional subkeys to this user's registry key after deletion has occurred, causing the user to be re-created with no password.</para>
		/// </summary>
		public void Delete()
		{
			RegistryKey usersKey = RegistryUtil.HKLM.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\server\\users", true);
			usersKey.DeleteSubKeyTree(name, false);
		}
	}
}
