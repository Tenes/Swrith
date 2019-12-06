using System;
using System.IO;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;

namespace Swrith.Utils
{
    /// <summary>
    /// Wrapper around the MarkDig parser that provides a cached
    /// instance of the Markdown parser. Hooks up custom processing.
    /// </summary>
    public class MarkdownParserMarkdig : MarkdownParserBase
    {
        /// <summary>
        /// Cached pipeline instance
        /// </summary>
        public static MarkdownPipeline Pipeline;

        /// <summary>
        /// Removes script code if set
        /// </summary>
        public static bool StripScriptCode { get; set; } = true;

        private readonly bool _usePragmaLines;

        /// <summary>
        /// Optional global configuration for setting up the Markdig Pipeline
        /// </summary>
        public static Action<MarkdownPipelineBuilder> ConfigurePipelineBuilder { get; set; }

        public MarkdownParserMarkdig(bool usePragmaLines = false, bool force = false, Action<MarkdownPipelineBuilder> markdigConfiguration = null)
        {
            _usePragmaLines = usePragmaLines;
            if (force || Pipeline == null)
            {
                if (markdigConfiguration == null && ConfigurePipelineBuilder != null)
                    markdigConfiguration = ConfigurePipelineBuilder;

                var builder = CreatePipelineBuilder(markdigConfiguration);
                Pipeline = builder.Build();
            }
        }

        /// <summary>
        /// Parses the actual markdown down to html
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="sanitizeHtml">If true strips script tags and javascript: directives</param>
        /// <returns></returns>        
        public override string Parse(string markdown, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            string html;
            using (var htmlWriter = new StringWriter())
            {
                var renderer = CreateRenderer(htmlWriter);

                Markdig.Markdown.Convert(markdown, renderer, Pipeline);

                html = htmlWriter.ToString();
            }

            html = ParseFontAwesomeIcons(html);

            if (sanitizeHtml)
                html = Sanitize(html);

            return html;
        }

        public virtual MarkdownPipelineBuilder CreatePipelineBuilder(Action<MarkdownPipelineBuilder> markdigConfiguration)
        {
            MarkdownPipelineBuilder builder = null;

            // build it explicitly
            if (markdigConfiguration == null)
            {
                builder = new MarkdownPipelineBuilder()
                    .UseEmphasisExtras()
                    .UsePipeTables()
                    .UseGridTables()
                    .UseFooters()
                    .UseFootnotes()
                    .UseCitations()
                    .UseAutoLinks() // URLs are parsed into anchors
                    .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                    .UseAbbreviations()
                    .UseYamlFrontMatter()
                    .UseEmojiAndSmiley(true)
                    .UseMediaLinks()
                    .UseListExtras()
                    .UseFigures()
                    .UseTaskLists()
                    .UseCustomContainers()
                    .UseGenericAttributes();

                //builder = builder.UseSmartyPants();            

                if (_usePragmaLines)
                    builder = builder.UsePragmaLines();

                return builder;
            }

            // let the passed in action configure the builder
            builder = new MarkdownPipelineBuilder();
            markdigConfiguration.Invoke(builder);

            if (_usePragmaLines)
                builder = builder.UsePragmaLines();

            return builder;
        }

        protected virtual IMarkdownRenderer CreateRenderer(TextWriter writer)
        {
            return new HtmlRenderer(writer);
        }
    }
}