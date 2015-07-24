using System;
using IniParser;
using System.Configuration;

namespace ProxmoxSharp.Util
{
	public class AppConfig
	{
		public IniParser.Model.IniData Data { get; set; }
		private string configFile = ConfigurationManager.AppSettings ["config"];

		public AppConfig ()
		{
			InitAppConfig (configFile);
		}

		public AppConfig (string configFile)
		{
			InitAppConfig (configFile);
		}

		public IniParser.Model.KeyDataCollection Section(string section) {
			InitAppConfig (configFile);
			return Data[section];
		}

		public IniParser.Model.KeyDataCollection Section(string configFile, string section) {
			InitAppConfig (configFile);
			return Data[section];
		}

		private void InitAppConfig(string configFile) {
			var parser = new FileIniDataParser();
			Data = parser.ReadFile(Environment.CurrentDirectory + "/" + configFile);
		}
	}
}

