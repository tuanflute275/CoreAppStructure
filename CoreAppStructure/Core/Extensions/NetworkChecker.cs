namespace CoreAppStructure.Core.Extensions
{
    public class NetworkChecker
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public int FailedCount { get; private set; } = 0;
        public NetworkChecker(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void EnsureConnectivity(string host, int port, string serviceName, int retries = 3)
        {
            using var client = new TcpClient();
            var policy = Polly.Policy
                .Handle<Exception>()
                .Retry(retries, (ex, retry) =>
                {
                    _logger.LogWithTime($"[{retry}/{retries}] - {serviceName} - {ex.Message}", MicrosoftLog.LogLevel.Warning);
                });

            _logger.LogWithTime($"Checking connectivity to {serviceName} at {host}:{port}");
            var result = policy.ExecuteAndCapture(() => client.Connect(host, port));

            if (result.Outcome == OutcomeType.Successful)
            {
                _logger.LogWithTime($"{serviceName} is reachable.");
            }
            else
            {
                _logger.LogWithTime($"{serviceName} connectivity failed. Exception: {result.FinalException}", MicrosoftLog.LogLevel.Error);
                FailedCount++;
            }
        }

        public void EnsureDatabaseConnectivity(string connectionString, string providerName, string serviceName, int retries = 3)
        {
            var policy = Polly.Policy
                .Handle<Exception>()
                .Retry(retries, (ex, retry) =>
                {
                    _logger.LogWithTime($"[{retry}/{retries}] - {serviceName} - {ex.Message}", MicrosoftLog.LogLevel.Warning);
                });

            _logger.LogWithTime($"Checking connectivity to {serviceName} using {providerName}.");

            var result = policy.ExecuteAndCapture(() =>
            {
                using var connection = CreateDbConnection(connectionString, providerName);
                connection.Open(); // Test if the connection can be opened.
            });

            if (result.Outcome == OutcomeType.Successful)
            {
                _logger.LogWithTime($"{serviceName} database connection is ready.");
            }
            else
            {
                _logger.LogWithTime($"{serviceName} database connection failed. Exception: {result.FinalException}", MicrosoftLog.LogLevel.Error);
                FailedCount++;
            }
        }

        private DbConnection CreateDbConnection(string connectionString, string providerName)
        {
            // Dynamically create a connection object based on the provider name.
            return providerName switch
            {
                //"SqlServer" => new System.Data.SqlClient.SqlConnection(connectionString),
                //"PostgreSQL" => new Npgsql.NpgsqlConnection(connectionString),
                //"MySql" => new MySql.Data.MySqlClient.MySqlConnection(connectionString),
                //"Oracle" => new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString),
                _ => throw new NotSupportedException($"Unsupported database provider: {providerName}"),
            };
        }

    }
}
