using System;
using System.Collections.Generic;

namespace Valour.Net.EmbedMenu;

public class EmbedMenu
{
    public Dictionary<string, object> Data { get; set; }
    public string Id { get; set; }

    internal Func<EmbedMenu, EmbedBuilder, EmbedBuilder> HeaderFunction { get; set; }
    internal Func<EmbedMenu, EmbedBuilder, EmbedBuilder> LoadFunction { get; set; }

    public EmbedMenu(string id)
    {
        Data = new();
        Id = id;
    }

    public EmbedMenu AddHeader(Func<EmbedMenu, EmbedBuilder, EmbedBuilder> function)
    {
        HeaderFunction = function;
        return this;
    }
    public EmbedMenu AddOnLoad(Func<EmbedMenu, EmbedBuilder, EmbedBuilder> function)
    {
        LoadFunction = function;
        return this;
    }
}
