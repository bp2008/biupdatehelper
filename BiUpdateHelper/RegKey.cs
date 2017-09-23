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
			bool original32BitRegistryAccessFlag = RegistryUtil.Force32BitRegistryAccess;
			try
			{
				byte[] regKeyData = RegistryUtil.GetHKLMValue<byte[]>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", key, null);

				if (regKeyData == null)
				{
					RegistryUtil.Force32BitRegistryAccess = !RegistryUtil.Force32BitRegistryAccess;
					regKeyData = RegistryUtil.GetHKLMValue<byte[]>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", key, null);
				}
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

				string name = RegistryUtil.GetHKLMValue<string>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", "name", null);
				if (name == null)
					txtName.Text = "";
				else
					txtName.Text = name;

				string email = RegistryUtil.GetHKLMValue<string>(@"SOFTWARE\Perspective Software\Blue Iris\Registration", "email", null);
				if (email == null)
					txtEmail.Text = "";
				else
					txtEmail.Text = email;

			}
			finally
			{
				RegistryUtil.Force32BitRegistryAccess = original32BitRegistryAccessFlag;
			}
		}
	}
}
