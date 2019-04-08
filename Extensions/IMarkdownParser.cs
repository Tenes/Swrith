namespace Dice_Driven_Stories.Extensions
{
    public interface IMarkdownParser
    {
        /// <summary>
        /// Returns parsed markdown
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        string Parse(string markdown, bool stripScriptTags = true);
    }
}