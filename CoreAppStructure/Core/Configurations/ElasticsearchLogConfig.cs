using Nest;

namespace CoreAppStructure.Core.Configurations
{
    public class ElasticsearchLogConfig
    {
        private readonly string _elasticUri = "http://localhost:9200"; 
        private readonly string _indexName = "app-logs";

        public IElasticClient GetElasticClient()
        {
            var settings = new ConnectionSettings(new Uri(_elasticUri))
                            .DefaultIndex(_indexName);

            var client = new ElasticClient(settings);
            return client;
        }
    }
}
