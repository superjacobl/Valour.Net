namespace Valour.Net.Models
{
    public class PlanetRole
    {

        /// <summary>
        /// The unique Id of this role
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The position of the role: Lower has more authority
        /// </summary>
        public uint Position { get; set; }

        /// <summary>
        /// The ID of the planet or system this role belongs to
        /// </summary>
        public ulong Planet_Id { get; set; }

        /// <summary>
        /// The planet permissions for the role
        /// </summary>
        public ulong Permissions { get; set; }

        // RGB Components for role color
        public byte Color_Red { get; set; }

        public byte Color_Green { get; set; }

        public byte Color_Blue { get; set; }

        // Formatting options
        public bool Bold { get; set; }

        public bool Italics { get; set; }

        public uint GetAuthority()
        {
            return uint.MaxValue - (1 + Position);
        }
    }
}
