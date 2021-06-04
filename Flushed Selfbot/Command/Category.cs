using System.Collections.Generic;

namespace FlushedSelfbot.Command
{
    public class Category
    {
        public static readonly HashSet<Category> Values;
        public static readonly Category Utility = new Category("Utility", ":tools:"), 
            Fun = new Category("Fun", ":laughing:"), 
            Image = new Category("Image", ":frame_photo:"),
            Nsfw = new Category("NSFW", ":underage:"),
            Misc = new Category("Misc", ":question:");

        public readonly string Name;
        public readonly string Emote;

        private Category(string name, string emote)
        {
            Name = name;
            Emote = emote;
        }
        
        static Category()
        {
            Values = new HashSet<Category> {Utility, Fun, Image, Nsfw, Misc};
        }
    }
}