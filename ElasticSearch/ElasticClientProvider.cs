using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;

namespace ElasticSearch
{
    public class ElasticClientProvider
    {
        private readonly ConnectionSettings _connectionSettings;
        public ElasticClient ElasticClient { get; private set; }
        public ElasticClientProvider()
        {
            SingleNodeConnectionPool pool = new(new Uri("http://localhost:9200"));
            _connectionSettings = new ConnectionSettings(pool, (builtInSerializer, connectionSettings) =>
                                                             new JsonNetSerializer(
                                                                 builtInSerializer, connectionSettings, () =>
                                                                     new JsonSerializerSettings
                                                                     {
                                                                         ReferenceLoopHandling =
                                                                             ReferenceLoopHandling.Ignore
                                                                     }

                                                                     )).DefaultIndex("movies");

            ElasticClient = CreateClient();
        }

        private ElasticClient CreateClient()
        {
            return new ElasticClient(_connectionSettings);
        }
    }
}
