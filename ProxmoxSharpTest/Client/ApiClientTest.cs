using System.Net;
using NUnit.Framework;
using ProxmoxSharp.Client;
using ProxmoxSharp.Util;
using RestSharp;

namespace ProxmoxSharpTest.Client
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
		private LxcTemplate template;

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

			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; 

			response = client.Login (user);

		    var tasks = client.TaskStatusList().Data;

		    if (tasks.Count > 0)
		    {
                upid =  tasks[0].Upid;
            }
            
			template = new LxcTemplate {
				Cpuunits = "1",
				Password = "password",
				Hostname = "api.proxmox.test",
				Net = "name=eth0,bridge=vmbr0,hwaddr=DE:88:0F:D7:79:33,ip=dhcp,ip6=dhcp,type=veth",
				Memory = "1000",
				Ostemplate = "disk1:vztmpl/debian-8.0-standard_8.6-1_amd64.tar.gz",
                Ostype = "debian",
				Storage = "disk1",
				Swap = "512",
				Vmid = vmId,
				Pool = pool,
			};
		}

		[Test]	
		public void Login() {
			Assert.That(response.StatusCode, Is.EqualTo (HttpStatusCode.OK));		
			Assert.That(response.Data.username, Is.EqualTo (user.Username + "@" + user.Realm));
		}

		[Test]
		public void CreateAndDestroyLxcContainer() {
			var response = client.CreateCt (template);
			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.Data));

			response = client.StartCt (vmId);
			Assert.That (response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.Data));


			var statusResponse = client.CtStatus (vmId);
			Assert.That (statusResponse.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.That (statusResponse.Data.Status.AsVmStatus (), Is.EqualTo (VmStatus.running));

			response = client.StopCt (vmId);
			Assert.That (response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.Data));

			response = client.DeleteCt (vmId);
			Assert.That (response.StatusCode, Is.EqualTo (HttpStatusCode.OK));
			Assert.True (client.TaskHasFinished (response.Data.Data));
		}

		[Test]
		public void CantGetStatusOfNonExistentLxcContainer() {
			var response = client.CtStatus ("-100");

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
