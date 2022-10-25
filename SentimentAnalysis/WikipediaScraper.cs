using HtmlAgilityPack;

namespace SentimentAnalysis;

public class WikipediaScraper : IScraper
{
    public async Task<string?> Download(string url)
    {
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            throw new Exception("invalid url");
        }

        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);

        var nodesToRemove = doc.DocumentNode.SelectNodes("//sup");
        if (nodesToRemove is null)
        {
            return await Task.FromResult<string?>(null);
        }

        var nodeList = nodesToRemove.ToList();

        foreach (var node in nodeList)
        {
            node.Remove();
        }

        var nodes = doc.DocumentNode.SelectNodes("//p");


        var finalText = "";
        foreach (var n in nodes)
        {
            finalText += n.InnerText; //.Replace("\n", "");
        }

        return finalText;
    }
}
