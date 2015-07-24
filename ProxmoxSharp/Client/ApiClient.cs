using System;
using RestSharp;
using System.Net;
using System.Collections.Generic;
using System.Threading;

namespace ProxmoxSharp.Client
{
	public class ApiClient
	{
		private string baseUrl;
		private string node;
		private ApiTicket ApiTicket;

		public const string TaskOk = "TASK OK";
		private const string RequestRootElement = "data";

		public ApiClient (Server server, string node)
		{
			this.baseUrl = "https://" + server.Ip + ":" + server.Port + "/api2/json/";
			this.node = node;
		}

		public IRestResponse<ApiTicket> Login(User user) {
			var restClient = new RestClient (baseUrl);
			var request = new RestRequest ("access/ticket", Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddParameter ("username", user.Username);
			request.AddParameter ("password", user.Password);
			request.AddParameter ("realm", user.Realm);
			request.RootElement = RequestRootElement;
			var response = restClient.Execute<ApiTicket>(request);
			ApiTicket = response.Data;
			return response;
		}

		public IRestResponse<OpenvzCT> CTStatus(string vmId) {
			var client = new RestClient (baseUrl);
			var request = PrepareGetRequest (string.Format("nodes/{0}/openvz/{1}/status/current", this.node, vmId));
			return client.Execute<OpenvzCT> (request);
		}

		public IRestResponse<Upid> StartCT(string vmId) {
			var client = new RestClient (baseUrl);
			var request = PreparePostRequest (string.Format ("nodes/{0}/openvz/{1}/status/start", node, vmId), "");
			return client.Execute<Upid>(request);
		}

		public IRestResponse<List<TaskLog>> TaskLog(string upid) {
			var client = new RestClient (baseUrl);
			var request = PrepareGetRequest (string.Format ("nodes/{0}/tasks/{1}/log", node, upid));
			return client.Execute<List<TaskLog>> (request);
		}

		public bool TaskHasFinished(string upid, int seconds = 30) {
			var oneSecond = 1000;
			for (var i = 0; i < seconds * oneSecond;) {
				var logs = TaskLog (upid).Data;
				foreach (var log in logs) {
					if (log.t == TaskOk) {
						return true;
					}
				}
				i += oneSecond;
				Thread.Sleep (oneSecond);
			}
			return false;
		}

		public IRestResponse<List<TaskStatus>> TaskStatusList() {
			var client = new RestClient (baseUrl);
			var request = PrepareGetRequest (string.Format ("nodes/{0}/tasks/", node));
			return client.Execute<List<TaskStatus>>(request);
		}

		public IRestResponse<TaskStatus> TaskStatus(string upid) {
			var client = new RestClient (baseUrl);
			var request = PrepareGetRequest (string.Format ("nodes/{0}/tasks/{1}/status", node, upid));
			return client.Execute<TaskStatus>(request);
		}

		public IRestResponse<Upid> StopCT(string vmId) {
			var client = new RestClient (baseUrl);
			var request = PreparePostRequest (string.Format ("nodes/{0}/openvz/{1}/status/stop", node, vmId), "");
			return client.Execute<Upid>(request);
		}

		public IRestResponse<Upid> CreateCT(OpenvzTemplate template) {
			var client = new RestClient (baseUrl);
			var request = new RestRequest (string.Format("nodes/{0}/openvz", node), Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader ("CSRFPreventionToken", ApiTicket.CSRFPreventionToken);
			request.AddCookie ("PVEAuthCookie", ApiTicket.ticket);
			request.RootElement = "root";
			request.AddParameter ("ostemplate", template.ostemplate);
			request.AddParameter ("vmid", template.vmid);
			request.AddParameter ("storage", template.storage);
			request.AddParameter ("password", template.password);
			request.AddParameter ("hostname", template.hostname);
			request.AddParameter ("memory", template.memory);
			request.AddParameter ("swap", template.swap);
			request.AddParameter ("disk", template.disk);
			request.AddParameter ("cpus", template.cpus);
			request.AddParameter ("ip_address", template.ip_address);
			request.AddParameter ("pool", template.pool);
			return client.Execute<Upid>(request);
		}

		public IRestResponse<Upid> DeleteCT(string vmId) {
			var client = new RestClient (baseUrl);
			var request = PrepareDeleteRequest (string.Format ("nodes/{0}/openvz/{1}", node, vmId), "");
			return client.Execute<Upid>(request);
		}

		private RestRequest PreparePostRequest(string resource, string rootElement = RequestRootElement) {
			var request = new RestRequest (resource, Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader ("CSRFPreventionToken", ApiTicket.CSRFPreventionToken);
			request.AddCookie ("PVEAuthCookie", ApiTicket.ticket);
			request.RootElement = rootElement;
			return request;
		}

		private RestRequest PrepareGetRequest(string resource, string rootElement = RequestRootElement) {
			var request = new RestRequest (resource, Method.GET);
			request.RequestFormat = DataFormat.Json;
			request.AddCookie ("PVEAuthCookie", ApiTicket.ticket);
			request.RootElement = rootElement;
			return request;
		}

		private RestRequest PrepareDeleteRequest(string resource, string rootElement = RequestRootElement) {
			var request = new RestRequest (resource, Method.DELETE);
			request.RequestFormat = DataFormat.Json;
			request.AddHeader ("CSRFPreventionToken", ApiTicket.CSRFPreventionToken);
			request.AddCookie ("PVEAuthCookie", ApiTicket.ticket);
			request.RootElement = rootElement;
			return request;
		}
	}
}

