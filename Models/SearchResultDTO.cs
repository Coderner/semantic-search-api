namespace SemanticSearchApi.Models;

public class SearchResultDTO
{
    public int Id {get; set;}
    public string Content {get; set;} = string.Empty;
    public double SimilarityScore {get; set;}
}