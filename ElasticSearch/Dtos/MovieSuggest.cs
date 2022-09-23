namespace ElasticSearch.Dtos
{
    public class MovieSuggest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SuggestedName { get; set; }
        public double Score { get; set; }
    }
}
