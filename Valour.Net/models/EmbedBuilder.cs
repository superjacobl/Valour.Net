using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Valour.Net.Models
{

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

    // Valour lacks support for embed right now, so just do a message with new lines'

    public class EmbedBuilder
    {
        string Content = "";
        public EmbedBuilder WithUrl(string url)
        {
            Content += $"{url}";
            return this;
        }
        public EmbedBuilder WithAuthor(PlanetMember member)
        {
            return this;
        }

        public EmbedBuilder WithCurrentTimestamp()
        {
            return this;
        }
        public Color Color { get => Color; set {
            Color = value;
        }}
        public string Title { get => Title; set {
            Title = value;
        }}

        public void AddField(string name, string value) {
            Content += $"\n{name}: {value}";
        }
        public void AddField(string name, ulong value) {
            Content += $"\n{name}: {value}";
        }
        public void AddField(string name, int value) {
            Content += $"\n{name}: {value}";
        }
        public void AddField(string name, decimal value) {
            Content += $"\n{name}: {value}";
        }
        public void AddField(string name, double value) {
            Content += $"\n{name}: {value}";
        }
        public string Build()
        {
            return Content;
        }

    }
}