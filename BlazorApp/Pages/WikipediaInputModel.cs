using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Pages;

public class WikipediaInputModel
{
    [Required] public string UrlSegment { get; set; } = "";

    [Required] public string WikiUrl { get; set; } = "https://en.wikipedia.org/wiki/";
}
