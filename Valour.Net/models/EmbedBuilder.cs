using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Valour.Net.Models
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

    public enum EmbedSize
    {
        Big,
        Normal,
        Small,
        VerySmall,
        Short,
        VeryShort
    }

    public enum EmbedItemType
    {
        Text,
        Button,
        InputBox
    }

    public class ClientEmbedItem
    {

        /// <summary>
        /// The type of this embed item
        /// </summary>
        public EmbedItemType Type { get; set; }

        /// <summary>
        /// The text within the embed.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Name of the embed. Not required.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If this component should be inlined
        /// </summary>
        public bool Inline { get; set; }

        /// <summary>
        /// The link this component leads to
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Must be in hex format, example: "ffffff"
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The color (hex) of this embed item's text
        /// </summary>
        public string TextColor { get; set; }

        /// <summary>
        /// True if this item should be centered
        /// </summary>
        public bool Center { get; set; }

        /// <summary>
        /// The size of this embed item
        /// </summary>
        public EmbedSize Size { get; set; }
        
        /// <summary>
        /// Used to identify this embed item for events and more
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The input value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The placeholder text for inputs
        /// </summary>
        public string Placeholder { get; set; }
    }
    /// <summary>
    /// This class exists render embeds
    /// </summary>
    public class ClientEmbed
    {

        public List<ClientEmbedItem> Items = new List<ClientEmbedItem>();
        public List<List<ClientEmbedItem>> Pages = new List<List<ClientEmbedItem>>();
        public ClientEmbed() {
        }
    }

    public class EmbedPageBuilder
    {
        public List<ClientEmbedItem> Items = new List<ClientEmbedItem>();
        
        public EmbedPageBuilder AddText(string Name = "", string Text = "", bool Inline = false, string TextColor = "ffffff") {
            ClientEmbedItem item = new ClientEmbedItem()
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

        public EmbedPageBuilder AddButton(string Id = "", string Text = "", string Name = "", string Link = "", string Color = "000000", string TextColor = "ffffff", EmbedSize Size = EmbedSize.Normal, bool Center = false, bool Inline = false)
        {
            ClientEmbedItem item = new ClientEmbedItem()
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

        public EmbedPageBuilder AddInputBox(string Placeholder = "", string Name = "", string NameTextColor = "", string Id = "", bool Inline = false, EmbedSize Size = EmbedSize.Normal)
        {
            ClientEmbedItem item = new ClientEmbedItem()
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
            ClientEmbedItem item = new ClientEmbedItem()
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

        public EmbedBuilder AddButton(string Id = "", string Text = "", string Name = "", string Link = "", string Color = "000000", string TextColor = "ffffff", EmbedSize Size = EmbedSize.Normal, bool Center = false, bool Inline = false)
        {
            ClientEmbedItem item = new ClientEmbedItem()
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

        public EmbedBuilder AddInputBox(string Placeholder = "", string Name = "", string NameTextColor = "", string Id = "", bool Inline = false, EmbedSize Size = EmbedSize.Normal)
        {
            ClientEmbedItem item = new ClientEmbedItem()
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