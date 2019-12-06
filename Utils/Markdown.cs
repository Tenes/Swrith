namespace Swrith.Utils
{
    public static class Markdown
    {
        public static string Parse(string markdown,
                        bool usePragmaLines = false,
                        bool forceReload = false)
        {
            if (string.IsNullOrEmpty(markdown))
                return "";
            IMarkdownParser parser = MarkdownParserFactory
               .GetParser(usePragmaLines, forceReload);
            return parser.Parse(markdown);
        }
        public static Microsoft.AspNetCore.Html.HtmlString ParseHtmlString(string markdown, bool usePragmaLines = false, bool forceReload = false)
        {
            return new Microsoft.AspNetCore.Html.HtmlString(Parse(markdown, usePragmaLines, forceReload));
        }
    }
}