using Markdig.Helpers;
using System.Text.Json;
using Valour.Net.Client.MarkdownStuff;

namespace Valour.Net.Client.MessageHelper;

public static class MessageHelpers
{
    public static void GenerateForPost(Message msg)
    {
        // TODO: in the future, look into removing this function
        msg.ClearMentions();

        int pos = 0;

        string text = MarkdownManager.GetHtml(msg.Content);//msg.Content;
        string mention = "";
        while (pos < text.Length)
        {
            if (text[pos] == '«')
            {
                int s_len = text.Length - pos;
                // Must be at least this long ( «@x-x» )
                if (s_len < 6)
                {
                    pos++;
                    continue;
                }
                // Mentions (<@x-)
                if (text[pos + 1] == '@' &&
                    text[pos + 3] == '-')
                {
                    // Member mention (<@m-)
                    if (text[pos + 2] == 'm')
                    {
                        // Extract id
                        char c = ' ';
                        int offset = 4;
                        string id_chars = "";
                        while (offset < s_len &&
                               (c = text[pos + offset]).IsDigit())
                        {
                            id_chars += c;
                            offset++;
                        }
                        // Make sure ending tag is '>'
                        if (c != '»')
                        {
                            pos++;
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(id_chars))
                        {
                            pos++;
                            continue;
                        }
                        bool parsed = long.TryParse(id_chars, out long id);
                        if (!parsed)
                        {
                            pos++;
                            continue;
                        }
                        // Create object
                        Mention memberMention = new()
                        {
                            TargetId = id,
                            Type = MentionType.PlanetMember,                 
                        };
                        //msg.Content = msg.Content.Replace($"«@m-{id}»", "");
                        msg.Mentions.Add(memberMention);

                        
                    }
                    // Other mentions go here
                    else
                    {
                        pos++;
                        continue;
                    }
                }
                // Channel mentions (<#x-)
                if (text[pos + 1] == '#' &&
                    text[pos + 3] == '-')
                {
                    // Chat Channel mention (<#c-)
                    if (text[pos + 2] == 'c')
                    {
                        // Extract id
                        char c = ' ';
                        int offset = 4;
                        string id_chars = "";
                        while (offset < s_len &&
                               (c = text[pos + offset]).IsDigit())
                        {
                            id_chars += c;
                            offset++;
                        }
                        // Make sure ending tag is '>'
                        if (c != '»')
                        {
                            pos++;
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(id_chars))
                        {
                            pos++;
                            continue;
                        }
                        bool parsed = long.TryParse(id_chars, out long id);
                        if (!parsed)
                        {
                            pos++;
                            continue;
                        }
                        // Create object
                        Mention channelMention = new()
                        {
                            TargetId = id,
                            Type = MentionType.Channel
                        };
                        //msg.Content = msg.Content.Replace($"«#c-{id}»", "");
                        msg.Mentions.Add(channelMention);
                    }
                    else
                    {
                        pos++;
                        continue;
                    }
                }

                // Put future things here
                else
                {
                    pos++;
                    continue;
                }
            }

            pos++;
        }

        msg.MentionsData = JsonSerializer.Serialize(msg.Mentions);
    }
}