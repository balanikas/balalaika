using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Pages;

public class RedditInputModel
{
    [Required] public string UrlSegment { get; set; } = "";

    [Required] public string Url { get; set; } = "https://www.reddit.com/";
}
