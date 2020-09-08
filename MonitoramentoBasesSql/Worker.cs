using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitoramentoBasesSql.Clients;

namespace MonitoramentoBasesSql
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        public readonly CanalSlackClient _canalSlackClient;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration,
            CanalSlackClient canalSlackClient)
        {
            _logger = logger;
            _configuration = configuration;
            _canalSlackClient = canalSlackClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var conexoes = _configuration["Conexoes"]
                    .Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (string nomeConexao in conexoes)
                {
                    try
                    {
                        _logger.LogInformation($"Verificando a conexão {nomeConexao}...");
                        using var conexao = new SqlConnection(
                            _configuration.GetConnectionString(nomeConexao));
                        conexao.Open();
                        conexao.Close();
                        _logger.LogInformation($"Acesso à base da conexão {nomeConexao} efetuado com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro com a conexão {nomeConexao}: {ex.Message}");
                        _canalSlackClient.PostAlerta(nomeConexao, ex.Message);
                        _logger.LogInformation("Envio de alerta");
                    }
                }

                _logger.LogInformation($"Monitoramento executado em: {DateTime.Now}");

                await Task.Delay(30000, stoppingToken); // Esperar por 30 segundos
            }
        }
    }
}