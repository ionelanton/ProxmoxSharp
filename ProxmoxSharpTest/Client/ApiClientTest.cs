using System;
using NUnit.Framework;
using IniParser;
using RestSharp;
using System.Net;
using System.Dynamic;
using System.Threading;
using ProxmoxSharp.Util;

namespace ProxmoxSharp.Client
{
	[TestFixture]
	public class ApiClientTest
	{
		private ApiClient client;
		private IRestResponse<ApiTicket> response;
		private string upid;

		private string node;
		private User user;
		private Server server;
		private string vmId;
		private string pool;
		private OpenvzCTTemplate template;

		[TestFixtureSetUp]
		public void ApiClientSetup() {
			
			var appConfig = new AppConfig ();

			var config = appConfig.Data ["TestProxmox"];
			user = new User { Username = config["Username"], Password = config["Password"], Realm = config["Realm"] };
			server = new Server { Ip = config["Ip"], Port = config["Port"] };
			node = config ["Node"];
			pool = config["Pool"];
			vmId = config ["VmId"];

			client = new ApiClient (server, node);

			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; }; 

			response = client.Login (user);

			upid = client.TaskStatusList ().Data [0].upid;

			template = new OpenvzCTTemplate {
				cpus = "1",
				password = "root",
				disk = "8",
				hostname = "api.proxmox.test",
				ip_address = "192.168.1.234",
				memory = "1000",
				ostemplate = "disk1:vztmpl/debian-7.0-standard_7.0-2_i386.tar.gz",
				storage = "disk1",
				swap = "512",
				vmid = vmId,
				pool = pool,
			};
		}

		[Test]	
		public void Login() {
			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));		
			Assert.That(response.Data.username, Is.EqualTo (user.Username + "@" + user.Realm));
		}

		[Test]
		public void ManipulateCT() {
			var response = client.CreateCT (template);
			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.data));

			response = client.StartCT (vmId);
			Assert.That (response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.data));


			var statusResponse = client.CTStatus (vmId);
			Assert.That (statusResponse.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.That (statusResponse.Data.status.AsVmStatus (), Is.EqualTo (VmStatus.running));

			response = client.StopCT (vmId);
			Assert.That (response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.data));

			response = client.DeleteCT (vmId);
			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.data));
		}

		[Test]
		public void CantGetStatusOfNonExistentCT() {
			var response = client.CTStatus ("-100");

			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.Forbidden));
			Assert.That (response.Data, Is.Null);
		}

		[Test]
		public void TaskStatusList() {
			var response = client.TaskStatusList ();

			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.That (response.Data, Is.Not.Empty); 
		}

		[Test]
		public void TaskStatus() {
			var response = client.TaskStatus (upid);

			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
		}

		[Test]
		public void TaskLog() {
			var response = client.TaskLog (upid);

			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
		}
	}
}
