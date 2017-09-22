using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BiUpdateHelper
{
	public partial class RegKey : Form
	{
		public RegKey()
		{
			InitializeComponent();
		}

		private void RegKey_Load(object sender, EventArgs e)
		{
			LoadFromKey("Data30", txtReg3, txtName3, txtEmail3);
			LoadFromKey("Data40", txtReg4, txtName4, txtEmail4);
		}
		private void LoadFromKey(string key, TextBox txtReg, TextBox txtName, TextBox txtEmail)
		{
			byte[] regKeyData = GetHKLMValue<byte[]>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", "Data40", null);

			if (regKeyData == null)
				txtReg.Text = "Not Found!";
			else if (regKeyData.Length < 41)
				txtReg.Text = "Unrecognized format!";
			else
			{
				string keyPart5 = Encoding.ASCII.GetString(regKeyData, 16, 5);
				string keyPart1 = Encoding.ASCII.GetString(regKeyData, 21, 5);
				string keyPart3 = Encoding.ASCII.GetString(regKeyData, 26, 5);
				string keyPart2 = Encoding.ASCII.GetString(regKeyData, 31, 5);
				string keyPart4 = Encoding.ASCII.GetString(regKeyData, 36, 5);

				txtReg.Text = keyPart1 + '-' + keyPart2 + '-' + keyPart3 + '-' + keyPart4 + '-' + keyPart5;
			}

			string name = GetHKLMValue<string>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", "name", null);
			if (name == null)
				txtName.Text = "";
			else
				txtName.Text = name;

			string email = GetHKLMValue<string>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", "email", null);
			if (email == null)
				txtEmail.Text = "";
			else
				txtEmail.Text = email;
		}
		private T GetHKLMValue<T>(string path, string key, T defaultValue)
		{
			RegistryKey localKey;
			if (Environment.Is64BitOperatingSystem)
				localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			else
				localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
			object value = localKey.OpenSubKey(path)?.GetValue(key);
			if (value == null)
				return defaultValue;
			return (T)value;
		}
	}
}
