using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BPUtil;
using Microsoft.Win32;

namespace BiUpdateHelper
{
	public static class CameraWebInterfaceLinker
	{
		public static void GenerateWebInterfaceLinkDocument(string outPath)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html>");
			sb.AppendLine("	<head>");
			sb.AppendLine("		<title>Camera Configuration Links</title>");
			sb.AppendLine(@"		<style type=""text/css"">
body
{
	font-family: Arial;
}
table
{
	border-collapse: collapse;
}
td, th
{
	border-bottom: 1px solid #b5b5b5;
	padding: 2px 12px;
}
img
{
	max-width: 120px;
	max-height: 120px;
	cursor: pointer;
}
		</style>");
			sb.AppendLine("	</head>");
			sb.AppendLine("<body>");
			sb.AppendLine("<table><thead><tr><th>Camera Name</th><th>Short Name</th><th>Configuration URL</th><th>Snapshot</th></tr></thead><tbody>");
			RegistryKey cameras = RegistryUtil.HKLM.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\Cameras");
			if (cameras == null)
			{
				MessageBox.Show("Could not find Blue Iris's camera list in the registry.  Either you have no cameras configured or your Blue Iris version is not compatible.");
				return;
			}
			else
			{
				string[] cameraNames = cameras.GetSubKeyNames();
				if (cameraNames.Length == 0)
				{
					MessageBox.Show("No cameras were found in Blue Iris's registry settings.  Either you have no cameras configured or your Blue Iris version is not compatible.");
					return;
				}
				else
				{
					BiServerInfo.Reload();
					if (!BiServerInfo.enabled)
					{
						MessageBox.Show("This function is not supported on your system.  Possible reasons are that your Blue Iris version is incompatible, or your Blue Iris web server is not enabled.");
						return;
					}
					//BiUserInfo.Reload();
					AdvWebClient wc = new AdvWebClient();
					string session = null;
					try
					{
						wc.Proxy = null;
						try
						{
							UserInfo user = BiUserInfo.CreateTemporaryUser();
							session = GetSecureAuthenticatedSession(wc, out bool isAdmin, user.name, user.GetDecodedPassword());
							wc.CookieContainer.Add(new Cookie("session", session, "/", BiServerInfo.lanIp));
						}
						catch (Exception ex)
						{
							Logger.Debug(ex);
							wc = null;
						}

						List<CameraInfo> camList = new List<CameraInfo>(cameraNames.Length);
						foreach (string cameraName in cameraNames)
						{
							RegistryKey cam = cameras.OpenSubKey(cameraName);
							string shortName = cam.GetValue("shortname").ToString();
							string ip = cam.GetValue("ip").ToString();
							if (string.IsNullOrWhiteSpace(ip))
								continue;
							string port = cam.GetValue("ip_port").ToString();
							bool https = cam.GetValue("https").ToString() != "0";
							int index = int.Parse(cam.GetValue("pos").ToString());
							CameraInfo ci = new CameraInfo(cameraName, shortName, ip, port, https, index);
							camList.Add(ci);
						}
						camList.Sort(new Comparison<CameraInfo>((c1, c2) => c1.index.CompareTo(c2.index)));
						foreach (CameraInfo ci in camList)
							AddCameraLink(sb, ci, wc);
					}
					finally
					{
						wc?.UploadString(CameraWebInterfaceLinker.GetJsonURL(), "{\"cmd\":\"logout\",\"session\":\"" + session + "\"}");
						wc?.Dispose();
					}
				}
			}
			sb.AppendLine("</tbody></table></body>");
			sb.AppendLine("</html>");
			File.WriteAllText(outPath, sb.ToString());
			Process.Start(outPath);
		}
		public static string GetJsonURL()
		{
			return "http://" + BiServerInfo.lanIp + ":" + BiServerInfo.port + "/json";
		}
		public static string GetSecureAuthenticatedSession(WebClient wc, out bool isAdmin, string user, string pass)
		{
			string url = GetJsonURL();
			string response = wc.UploadString(url, "{\"cmd\":\"login\"}");
			Match m = Regex.Match(response, "\"session\": ?\"(.*?)\"");
			if (!m.Success)
				throw new Exception("Unexpected response from login command: " + response);
			string session = m.Groups[1].Value;
			string challengeResponse = Hash.GetMD5Hex(user + ":" + session + ":" + pass);
			response = wc.UploadString(url, "{\"cmd\":\"login\",\"response\":\"" + challengeResponse + "\",\"session\":\"" + session + "\"}");
			if (Regex.IsMatch(response, "\"result\": ?\"success\""))
			{
				isAdmin = Regex.IsMatch(response, "\"admin\": ?true");
				return session;
			}
			else
				throw new Exception("Unable to log in to server");
		}

		private static void AddCameraLink(StringBuilder sb, CameraInfo ci, WebClient wc)
		{
			string link = "http" + (ci.https ? "s" : "") + "://" + ci.ip + (ci.port == (ci.https ? "443" : "80") ? "" : (":" + ci.port)) + "/";
			string snapshot = wc == null ? "Unable to authenticate" : GetSnapshot(ci.shortName, ci.cameraName + " (" + ci.shortName + ") (" + link + ")", wc);
			sb.Append("<tr>");
			sb.Append("<td>" + ci.cameraName + "</td>");
			sb.Append("<td>" + ci.shortName + "</td>");
			sb.Append("<td><a href=\"" + link + "\">" + link + "</a></td>");
			sb.Append("<td><a href=\"" + link + "\">" + snapshot + "</a></td>");
			sb.AppendLine("</tr>");
		}

		private static string GetSnapshot(string shortName, string label, WebClient wc)
		{
			return "<img src=\"" + GetThumbnail(shortName, wc) + "\" alt=\"" + label + "\" />";
		}

		private static string GetThumbnail(string shortName, WebClient wc)
		{
			try
			{
				byte[] jpeg = wc.DownloadData("http://" + BiServerInfo.lanIp + ":" + BiServerInfo.port + "/image/" + shortName + "?&w=160&q=25");
				return "data:image/jpg;base64," + Convert.ToBase64String(jpeg);
			}
			catch { return ""; }
		}

		private class CameraInfo
		{
			public string cameraName;
			public string shortName;
			public string ip;
			public string port;
			public bool https;
			public int index;

			public CameraInfo(string cameraName, string shortName, string ip, string port, bool https, int index)
			{
				this.cameraName = cameraName;
				this.shortName = shortName;
				this.ip = ip;
				this.port = port;
				this.https = https;
				this.index = index;
			}
		}
	}
}
