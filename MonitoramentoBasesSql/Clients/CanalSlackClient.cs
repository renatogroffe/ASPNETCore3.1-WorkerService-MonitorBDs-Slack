using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MonitoramentoBasesSql.Clients
{
    public class CanalSlackClient
    {
        private HttpClient _client;
 
        public CanalSlackClient(HttpClient client,
            IConfiguration configuration)
        {
            _client = client;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            _client.BaseAddress = new Uri(configuration["UrlLogicAppSlack"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public void PostAlerta(string nomeConexao, string mensagemErro)
        {
            var requestMessage =
                  new HttpRequestMessage(HttpMethod.Post, String.Empty);

            requestMessage.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    baseDados = nomeConexao,
                    alerta = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensagemErro}",
                }), Encoding.UTF8, "application/json");

            var respLogicApp = _client
                .SendAsync(requestMessage).Result;
            respLogicApp.EnsureSuccessStatusCode();
        }
    }
}