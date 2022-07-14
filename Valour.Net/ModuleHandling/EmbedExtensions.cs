using System.Linq.Expressions;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Valour.Net.CommandHandling.Embeds;

public static class EmbedExtension
{
    public static EmbedBuilder ToEmbed<T>(this IEnumerable<T> source, Expression<Func<T, EmbedItem>> arg) where T : class
    {
        EmbedBuilder embed = new();
        EmbedPageBuilder page = new();
        var d = arg.Compile();
        foreach(T item in source) {
            page.Items.Add(d.Invoke(item));
        }
        embed.AddPage(page);
        return embed;
    }
}