using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace Valour.Net.Models.Embed
{

    public class EmbedFormDataItem
    {
        public string Element_Id {get; set;}
        public string Value {get; set;}
        public EmbedItemType Type {get; set;}
    }

    public class InteractionEvent
    {
        public string Event {get; set;}
        public string Element_Id {get; set;}
        public ulong Planet_Id {get; set;}
        public ulong Message_Id {get; set;}
        public ulong Author_Member_Id  {get; set;}
        public ulong Member_Id {get; set;}
        public ulong Channel_Id {get; set;}
        public DateTime Time_Interacted  {get; set;}
        public List<EmbedFormDataItem> Form_Data  {get; set;}
    }

    public class Color
    {
        public int R;
        public int G;
        public int B;
        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public class ClientEmbedPagesOnly
    {
        [JsonPropertyName("Pages")]
        public List<List<EmbedItem>> Pages { get; set; }
    }

    /// <summary>
    /// This class exists render embeds
    /// </summary>
    public class ClientEmbed
    {

        public List<EmbedItem> Items = new List<EmbedItem>();
        public List<List<EmbedItem>> Pages = new List<List<EmbedItem>>();
        public ClientEmbed() {
        }
    }

    public class EmbedPageBuilder
    {
        public List<EmbedItem> Items = new List<EmbedItem>();
        
        public EmbedPageBuilder AddText(string Name = "", string Text = "", bool Inline = false, string TextColor = "ffffff") {
            EmbedItem item = new EmbedItem()
            {
                Type = EmbedItemType.Text,
                Text = Text,
                Inline = Inline,
                TextColor = TextColor
            };
            if (Name == null) {
                Name = "";
            }
            if (Name != "") {
                item.Name = Name;
            }
            Items.Add(item);
            return this;
        }

        public EmbedPageBuilder AddButton(string Id = "", string Text = "", string Name = "", string Link = "", string Color = "000000", string TextColor = "ffffff", EmbedItemSize Size = EmbedItemSize.Normal, bool Center = false, bool Inline = false)
        {
            EmbedItem item = new EmbedItem()
            {
                Type = EmbedItemType.Button,
                Text = Text,
                Color = Color,
                Inline = Inline,
                TextColor = TextColor,
                Size = Size,
                Center = Center,
                Id = Id
            };
            if (Name == null) {
                Name = "";
            }
            if (Name != "") {
                item.Name = Name;
            }
            if (Link == null) {
                Link = "";
            }
            if (Link != "") {
                item.Link = Link;
            }
            Items.Add(item);
            return this;
        }

        public EmbedPageBuilder AddInputBox(string Placeholder = "", string Name = "", string NameTextColor = "", string Id = "", bool Inline = false, EmbedItemSize Size = EmbedItemSize.Normal)
        {
            EmbedItem item = new EmbedItem()
            {
                Type = EmbedItemType.InputBox,
                Placeholder = Placeholder,
                Inline = Inline,
                TextColor = NameTextColor,
                Id = Id,
                Size = Size
            };
            if (Name == null) {
                Name = "";
            }
            if (Name != "") {
                item.Name = Name;
            }
            Items.Add(item);
            return this;
        }
    }

    public class EmbedBuilder
    {

        public ClientEmbed Embed = new ClientEmbed();
        
        public EmbedBuilder()
        {
        }

        public EmbedBuilder AddPage(EmbedPageBuilder Page)
        {
            Embed.Pages.Add(Page.Items);
            return this;
        }

        public EmbedBuilder AddText(string Name = "", string Text = "", bool Inline = false, string TextColor = "ffffff") {
            EmbedItem item = new EmbedItem()
            {
                Type = EmbedItemType.Text,
                Text = Text,
                Inline = Inline,
                TextColor = TextColor
            };
            if (Name == null) {
                Name = "";
            }
            if (Name != "") {
                item.Name = Name;
            }
            Embed.Items.Add(item);
            return this;
        }

        public EmbedBuilder AddButton(string Id = "", string Text = "", string Name = "", string Link = "", string Color = "000000", string TextColor = "ffffff", EmbedItemSize Size = EmbedItemSize.Normal, bool Center = false, bool Inline = false)
        {
            EmbedItem item = new EmbedItem()
            {
                Type = EmbedItemType.Button,
                Text = Text,
                Color = Color,
                Inline = Inline,
                TextColor = TextColor,
                Size = Size,
                Center = Center,
                Id = Id
            };
            if (Name == null) {
                Name = "";
            }
            if (Name != "") {
                item.Name = Name;
            }
            if (Link == null) {
                Link = "";
            }
            if (Link != "") {
                item.Link = Link;
            }
            Embed.Items.Add(item);
            return this;
        }

        public EmbedBuilder AddInputBox(string Placeholder = "", string Name = "", string NameTextColor = "", string Id = "", bool Inline = false, EmbedItemSize Size = EmbedItemSize.Normal)
        {
            EmbedItem item = new EmbedItem()
            {
                Type = EmbedItemType.InputBox,
                Placeholder = Placeholder,
                Inline = Inline,
                TextColor = NameTextColor,
                Id = Id,
                Size = Size
            };
            if (Name == null) {
                Name = "";
            }
            if (Name != "") {
                item.Name = Name;
            }
            Embed.Items.Add(item);
            return this;
        }
    }
}