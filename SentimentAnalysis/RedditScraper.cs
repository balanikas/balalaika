using HtmlAgilityPack;

namespace SentimentAnalysis;

public class RedditScraper : IScraper
{
    public async Task<string?> Download(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            throw new Exception("invalid url");
        }

        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);

        var nodes = doc.DocumentNode.SelectNodes("//div[@data-testid=\"comment\"]");
        if (nodes is null)
        {
            return await Task.FromResult<string?>(null);
        }

        var finalText = "";
        foreach (var n in nodes)
        {
            finalText += n.InnerText;
        }

        return finalText;
    }
}
