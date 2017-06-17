using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
.linkWrapper
{
	/* position: inline-block; */
	vertical-align: top;
}
.linkWrapper img
{
	max-width: 320px;
	max-height: 240px;
}
		</style>");
			sb.AppendLine("	</head>");
			sb.AppendLine("<body>");
			sb.AppendLine("<table><thead><tr><th>Camera Name</th><th>Short Name</th><th>Configuration URL</th></tr></thead><tbody>");
			RegistryKey cameras = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Perspective Software\\Blue Iris\\Cameras");
			string[] cameraNames = cameras.GetSubKeyNames();
			List<CameraInfo> camList = new List<CameraInfo>(cameraNames.Length);
			foreach (string cameraName in cameraNames)
			{
				RegistryKey cam = cameras.OpenSubKey(cameraName);
				string shortName = cam.GetValue("shortname").ToString();
				string ip = cam.GetValue("ip").ToString();
				string port = cam.GetValue("ip_port").ToString();
				bool https = cam.GetValue("https").ToString() != "0";
				int index = int.Parse(cam.GetValue("pos").ToString());
				CameraInfo ci = new CameraInfo(cameraName, shortName, ip, port, https, index);
				camList.Add(ci);
			}
			camList.Sort(new Comparison<CameraInfo>((c1, c2) => c1.index.CompareTo(c2.index)));
			foreach (CameraInfo ci in camList)
				AddCameraLink(sb, ci);
			sb.AppendLine("</tbody></table></body>");
			sb.AppendLine("</html>");
			File.WriteAllText(outPath, sb.ToString());
			Process.Start(outPath);
		}

		private static void AddCameraLink(StringBuilder sb, CameraInfo ci)
		{
			string link = "http" + (ci.https ? "s" : "") + "://" + ci.ip + (ci.port == (ci.https ? "443" : "80") ? "" : (":" + ci.port)) + "/";
			//string label = ci.cameraName + " (" + ci.shortName + ") (" + link + ")";
			sb.Append("<tr>");
			sb.Append("<td>" + ci.cameraName + "</td>");
			sb.Append("<td>" + ci.shortName + "</td>");
			sb.Append("<td><a href=\"" + link + "\">" + link + "</a></td>");
			sb.AppendLine("</tr>");
		}

		//private static string GetLabel(string shortName, string label)
		//{
		//	return "<img src=\"" + GetThumbnail(shortName) + "\" alt=\"" + label + "\" />";
		//}

		//private static string GetThumbnail(string shortName)
		//{
		//	return "";
		//}

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
