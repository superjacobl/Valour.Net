using Markdig;
using Markdig.Extensions;
using System.Text.RegularExpressions;
using Markdig.Extensions.MediaLinks;
using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

#nullable enable

namespace Valour.Net.Client.MarkdownStuff;

/*  Valour - A free and secure chat client
*  Copyright (C) 2021 Vooper Media LLC
*  This program is subject to the GNU Affero General Public license
*  A copy of the license should be included - if not, see <http://www.gnu.org/licenses/>
*/


public class VooperMediaLinkExtension : IMarkdownExtension
{
    public VooperMediaLinkExtension() : this(new MediaOptions())
    {
    }

    public VooperMediaLinkExtension(MediaOptions? options)
    {
        Options = options ?? new MediaOptions();
        Options.ExtensionToMimeType.Add(".mov", "video/quicktime");
    }

    public MediaOptions Options { get; }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<LinkInlineRenderer>();
            if (inlineRenderer != null)
            {
                inlineRenderer.TryWriters.Remove(TryLinkInlineRenderer);
                inlineRenderer.TryWriters.Add(TryLinkInlineRenderer);
            }
        }
    }

    private bool TryLinkInlineRenderer(HtmlRenderer renderer, LinkInline linkInline)
    {
        if (!linkInline.IsImage || linkInline.Url is null)
        {
            return false;
        }

        bool isSchemaRelative = false;
        // Only process absolute Uri
        if (!Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out Uri? uri) || !uri.IsAbsoluteUri)
        {
            // see https://tools.ietf.org/html/rfc3986#section-4.2
            // since relative uri doesn't support many properties, "http" is used as a placeholder here.
            if (linkInline.Url.StartsWith("//", StringComparison.Ordinal) && Uri.TryCreate("http:" + linkInline.Url, UriKind.Absolute, out uri))
            {
                isSchemaRelative = true;
            }
            else
            {
                return false;
            }
        }

        if (TryRenderIframeFromKnownProviders(uri, isSchemaRelative, renderer, linkInline))
        {
            return true;
        }

        if (TryGuessAudioVideoFile(uri, isSchemaRelative, renderer, linkInline))
        {
            return true;
        }

        return false;
    }

    private static HtmlAttributes GetHtmlAttributes(LinkInline linkInline)
    {
        var htmlAttributes = new HtmlAttributes();
        var fromAttributes = linkInline.TryGetAttributes();
        if (fromAttributes != null)
        {
            fromAttributes.CopyTo(htmlAttributes, false, false);
        }

        return htmlAttributes;
    }

    private bool TryGuessAudioVideoFile(Uri uri, bool isSchemaRelative, HtmlRenderer renderer, LinkInline linkInline)
    {
        var path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        // Otherwise try to detect if we have an audio/video from the file extension
        var lastDot = path.LastIndexOf('.');
        if (lastDot >= 0 &&
            Options.ExtensionToMimeType.TryGetValue(path.Substring(lastDot), out string? mimeType))
        {
            var htmlAttributes = GetHtmlAttributes(linkInline);
            var isAudio = mimeType.StartsWith("audio", StringComparison.Ordinal);
            // var tagType = isAudio ? "audio" : "video";

            // Fix bug, it's dirty but it works
            renderer.Write("<video ");

            if (isAudio){
                renderer.Write("style='height:50px !important'");
            }
            else{
                htmlAttributes.AddPropertyIfNotExist("width", Options.Width);
                htmlAttributes.AddPropertyIfNotExist("height", Options.Height);
            }

            htmlAttributes.AddPropertyIfNotExist("controls", null);
            htmlAttributes.AddPropertyIfNotExist("preload", "auto");
            htmlAttributes.AddPropertyIfNotExist("playsinline", null);

            if (!string.IsNullOrEmpty(Options.Class))
                htmlAttributes.AddClass(Options.Class); 

            renderer.WriteAttributes(htmlAttributes);

            if (isAudio){
                renderer.Write($"><source type=\"{mimeType}\" src=\"{linkInline.Url}\"></source></video>");
            }
            else{
                renderer.Write($" type=\"{mimeType}\" src=\"{linkInline.Url}\"></video>");
            }
            

            return true;
        }
        return false;
    }

    private bool TryRenderIframeFromKnownProviders(Uri uri, bool isSchemaRelative, HtmlRenderer renderer, LinkInline linkInline)
    {
        IHostProvider? foundProvider = null;
        string? iframeUrl = null;
        foreach (var provider in Options.Hosts)
        {
            if (!provider.TryHandle(uri, isSchemaRelative, out iframeUrl))
                continue;
            foundProvider = provider;
            break;
        }

        if (foundProvider is null)
        {
            return false;
        }

        var htmlAttributes = GetHtmlAttributes(linkInline);
        renderer.Write("<iframe src=\"");
        renderer.WriteEscapeUrl(iframeUrl);
        renderer.Write('"');

        if (!string.IsNullOrEmpty(Options.Width))
            htmlAttributes.AddPropertyIfNotExist("width", Options.Width);

        if (!string.IsNullOrEmpty(Options.Height))
            htmlAttributes.AddPropertyIfNotExist("height", Options.Height);

        if (!string.IsNullOrEmpty(Options.Class))
            htmlAttributes.AddClass(Options.Class);

        if (foundProvider.Class is { Length: > 0 } className)
            htmlAttributes.AddClass(className);

        htmlAttributes.AddPropertyIfNotExist("frameborder", "0");
        if (foundProvider.AllowFullScreen)
        {
            htmlAttributes.AddPropertyIfNotExist("allowfullscreen", null);
        }
        renderer.WriteAttributes(htmlAttributes);
        renderer.Write("></iframe>");

        return true;
    }
}

public static class MarkdownManager
{
    public static MarkdownPipeline pipeline;

    public static MarkdownPipelineBuilder UseVooperMediaLinks(this MarkdownPipelineBuilder pipeline, MediaOptions? options = null)
    {
        if (!pipeline.Extensions.Contains<VooperMediaLinkExtension>())
        {
            pipeline.Extensions.Add(new VooperMediaLinkExtension(options));
        }
        return pipeline;
    }

    static MarkdownManager()
    {

        pipeline = new MarkdownPipelineBuilder().DisableHtml()
                                                .UseVooperMediaLinks()
                                                .UseAutoLinks()
                                                .UseMathematics()
                                                .UseAbbreviations()
                                                .UseCitations()
                                                .UseCustomContainers()
                                                .UseDiagrams()
                                                .UseFigures()
                                                .UseFootnotes()
                                                .UseGlobalization()
                                                .UseGridTables()
                                                .UseListExtras()
                                                .UsePipeTables()
                                                .UseTaskLists()
                                                .UseEmphasisExtras()
                                                .UseEmojiAndSmiley(true)
                                                .UseReferralLinks("nofollow")
                                                .UseSoftlineBreakAsHardlineBreak()
                                                .Build();
    }

    public static readonly Regex sanitizeLink = new("(?<=follow\">).+?(?=<)");

    public static string GetHtml(string content)
    {
        string markdown = "Error: Message could not be parsed.";

        try
        {
            markdown = Markdown.ToHtml(content, pipeline);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error parsing message!");
            Console.WriteLine("This may be nothing to worry about, a user may have added an insane table or such.");
            Console.WriteLine(e.Message);
        }

        markdown = markdown.Replace("<a", "<a target='_blank'");

        return markdown;
    }
}

