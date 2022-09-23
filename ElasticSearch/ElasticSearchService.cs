using ElasticSearch.Dtos;
using ElasticSearch.Entities;
using Nest;
using Newtonsoft.Json;

namespace ElasticSearch
{
    public class ElasticSearchService
    {
        private readonly ElasticClient _client;
        public ElasticSearchService(ElasticClientProvider elasticClientProvider) =>
            _client = elasticClientProvider.ElasticClient;

        public async Task<IEnumerable<MovieSuggest>> AutoComplete(string keyword)
        {
            ISearchResponse<Movie> searchResponse = await _client.SearchAsync<Movie>(s => s
                                    .Suggest(su => su
                                         .Completion("suggestions", c => c
                                              .Field(f => f.Name)
                                              .Prefix(keyword)
                                              .Fuzzy(f => f
                                                  .Fuzziness(Fuzziness.Auto)
                                              )
                                              .Size(10))
                                            ));
            
            return from suggest in searchResponse.Suggest["suggestions"]
                   from option in suggest.Options
                   select new MovieSuggest
                   {
                       Id = option.Source.Id,
                       Name = option.Source.Name,
                       SuggestedName = option.Text,
                       Score = option.Score
                   };
        }
       

        public async Task<bool> InsertMockData()
        {
            #region read file from project location
            
            var path = @"C:\Users\Monster\source\repos\ElasticSearch\MOCK_DATA.json";
            string json = File.ReadAllText(path);
            var movies = JsonConvert.DeserializeObject<List<Movie>>(json);

            #endregion

            #region Index Create
           
            var createIndexDescriptor = new CreateIndexDescriptor("movies")
                    .Aliases(x => x.Alias("movies_autocomplete"))
                                          .Map<Movie>(m => m
                                          .Properties(p => p.Text(t => t.Name(n => n.Name)))
                                           .Properties(p => p.Text(t => t.Name(n => n.Image)))
                                                  .AutoMap()
                                                  .Properties(ps => ps
                                                      .Completion(c => c
                                                          .Name(p => p.Name))) 
                                     
                                     );

            var createdIndex = await _client.Indices.CreateAsync(createIndexDescriptor);

            #endregion

            #region Bulk Insert

            var result = await _client.IndexDocumentAsync(createdIndex);

            var bulkIndexer = new BulkDescriptor();

            foreach (var movie in movies)
            {
                bulkIndexer.Index<Movie>(x => x
                .Document(movie)
                .Id(movie.Id));
            }

            #endregion

            return (await _client.BulkAsync(bulkIndexer)).IsValid;
        }
    }
}

