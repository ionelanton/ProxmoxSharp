using RestSharp;
using System.Collections.Generic;
using System.Threading;

namespace ProxmoxSharp.Client
{
	public class ApiClient
	{
		private readonly string _baseUrl;
		private readonly string _node;
		private ApiTicket _apiTicket;

		private const string TaskOk = "TASK OK";
		private const string RequestRootElement = "data";

		public ApiClient (Server server, string node)
		{
			_baseUrl = "https://" + server.Ip + ":" + server.Port + "/api2/json/";
			_node = node;
		}

		public IRestResponse<ApiTicket> Login(User user) {
			var restClient = new RestClient (_baseUrl);
		    var request = new RestRequest("access/ticket", Method.POST) {RequestFormat = DataFormat.Json};
		    request.AddParameter ("username", user.Username);
			request.AddParameter ("password", user.Password);
			request.AddParameter ("realm", user.Realm);
			request.RootElement = RequestRootElement;
			var response = restClient.Execute<ApiTicket>(request);
			_apiTicket = response.Data;
			return response;
		}

		public IRestResponse<Lxc> CtStatus(string vmId) {
			var client = new RestClient (_baseUrl);
			var request = PrepareGetRequest ($"nodes/{_node}/lxc/{vmId}/status/current");
			var response = client.Execute<Lxc> (request);

            return response;
		}

		public IRestResponse<Upid> StartCt(string vmId) {
			var client = new RestClient (_baseUrl);
			var request = PreparePostRequest ($"nodes/{_node}/lxc/{vmId}/status/start", "");
			return client.Execute<Upid>(request);
		}

		public IRestResponse<List<TaskLog>> TaskLog(string upid) {
			var client = new RestClient (_baseUrl);
			var request = PrepareGetRequest ($"nodes/{_node}/tasks/{upid}/log");
			return client.Execute<List<TaskLog>> (request);
		}

		public bool TaskHasFinished(string upid, int seconds = 30) {
			const int oneSecond = 1000;
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
			var client = new RestClient (_baseUrl);
			var request = PrepareGetRequest ($"nodes/{_node}/tasks/");
			var response = client.Execute<List<TaskStatus>>(request);
            return response;
		}

		public IRestResponse<TaskStatus> TaskStatus(string upid) {
			var client = new RestClient (_baseUrl);
			var request = PrepareGetRequest ($"nodes/{_node}/tasks/{upid}/status");
			return client.Execute<TaskStatus>(request);
		}

		public IRestResponse<Upid> StopCt(string vmId) {
			var client = new RestClient (_baseUrl);
			var request = PreparePostRequest ($"nodes/{_node}/lxc/{vmId}/status/stop", "");
			return client.Execute<Upid>(request);
		}

		public IRestResponse<Upid> CreateCt(LxcTemplate template) {
			var client = new RestClient (_baseUrl);
		    var request = new RestRequest($"nodes/{_node}/lxc", Method.POST) {RequestFormat = DataFormat.Json};
		    request.AddHeader ("CSRFPreventionToken", _apiTicket.CSRFPreventionToken);
			request.AddCookie ("PVEAuthCookie", _apiTicket.ticket);
			//request.RootElement = "data";
			request.AddParameter ("ostemplate", template.ostemplate);
			request.AddParameter ("vmid", template.vmid);
			request.AddParameter ("storage", template.storage);
			request.AddParameter ("password", template.password);
			request.AddParameter ("hostname", template.hostname);
			request.AddParameter ("memory", template.memory);
			request.AddParameter ("swap", template.swap);
			request.AddParameter ("cpuunits", template.cpuunits);
			request.AddParameter ("net0", template.net);
			request.AddParameter ("ostype", template.ostype);
			request.AddParameter ("pool", template.pool);
			var response = client.Execute<Upid>(request);
		    return response;
		}

		public IRestResponse<Upid> DeleteCt(string vmId) {
			var client = new RestClient (_baseUrl);
			var request = PrepareDeleteRequest ($"nodes/{_node}/lxc/{vmId}", "");
			return client.Execute<Upid>(request);
		}

		private RestRequest PreparePostRequest(string resource, string rootElement = RequestRootElement) {
		    var request = new RestRequest(resource, Method.POST) {RequestFormat = DataFormat.Json};
		    request.AddHeader ("CSRFPreventionToken", _apiTicket.CSRFPreventionToken);
			request.AddCookie ("PVEAuthCookie", _apiTicket.ticket);
			request.RootElement = rootElement;
			return request;
		}

		private RestRequest PrepareGetRequest(string resource, string rootElement = RequestRootElement) {
		    var request = new RestRequest(resource, Method.GET) {RequestFormat = DataFormat.Json};
		    request.AddCookie ("PVEAuthCookie", _apiTicket.ticket);
			request.RootElement = rootElement;
			return request;
		}

		private RestRequest PrepareDeleteRequest(string resource, string rootElement = RequestRootElement) {
		    var request = new RestRequest(resource, Method.DELETE) {RequestFormat = DataFormat.Json};
		    request.AddHeader ("CSRFPreventionToken", _apiTicket.CSRFPreventionToken);
			request.AddCookie ("PVEAuthCookie", _apiTicket.ticket);
			request.RootElement = rootElement;
			return request;
		}
	}
}

