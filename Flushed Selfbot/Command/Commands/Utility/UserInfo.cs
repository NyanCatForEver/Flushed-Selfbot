using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Discord;

namespace FlushedSelfbot.Command.Commands.Utility
{
    public class UserInfo : Command
    {
        public UserInfo() : base("UserInfo", "Sends information about the specified user",
            "UserInfo <user/id (optional)>", Category.Utility)
        {

        }

        private readonly Font _font = new Font(Bot.FontCollection.Families.First(), 16);
        private readonly Font _fontBig = new Font(Bot.FontCollection.Families.First(), 24);
        private readonly Font _fontBigBold = new Font(Bot.FontCollection.Families.First(), 24, FontStyle.Bold);

        public override void Execute()
        {
            DiscordUser user = null;
            if (Args.Count == 0)
                user = Client.User;
            else if (Message.Mentions.Count > 0)
                user = Message.Mentions[0];
            else
            {
                try
                {
                    user = Bot.Client.GetUser(ulong.Parse(Args[0]));
                }
                catch (Exception)
                {
                    Message.Edit(new MessageEditProperties {Content = "> Invalid argument. Syntax: " + Syntax});
                }
            }

            if (user == null)
            {
                Message.Edit(new MessageEditProperties {Content = "> Invalid user."});
                return;
            }

            var image = new Bitmap(600, 200);
            var avatar = RoundImage(ResizeImage(user.Avatar.Download(DiscordCDNImageFormat.PNG).Image, 128, 128), 10);
            var g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            g.FillPath(new SolidBrush(Color.FromArgb(255, 18, 18, 18)),
                RoundedRect(new Rectangle(0, 0, image.Width, image.Height), 14));

            g.DrawImage(avatar, 24 + 1, image.Height / 2 - avatar.Height / 2 + 1, avatar.Width + 1,
                avatar.Height + 1);

            g.DrawString(user.Username, _fontBigBold, new SolidBrush(Color.FromArgb(255, 229, 229, 229)), 128 + 24 + 23,
                52 - g.MeasureString(user.Username, _fontBigBold).Height + 7);
            var usernameWidth = g.MeasureString(user.Username, _fontBigBold).Width;
            g.DrawString("#" + user.Discriminator, _fontBig,
                new SolidBrush(Color.FromArgb(255, 229, 229, 229)), 128 + 24 + 23 + usernameWidth - 8, 52
                - g.MeasureString("#" + user.Discriminator, _fontBig).Height + 7);

            var idWidth = DrawCategory("ID", user.Id.ToString(), 128 + 24 + 22, 64, _font, g);
            DrawCategory("Bot", (user.Type == DiscordUserType.Bot).ToString().ToLower(), 128 + 24 + 22 + idWidth + 8, 64, _font,
                g);

            var createdAtWidth = DrawCategory("Created at", user.CreatedAt.ToString("d"),
                128 + 24 + 22, 64 + 54 + 8, _font, g);

            var member = Message.Guild?.GetMember(user.Id);
            if (member != null)
                DrawCategory("Joined at", member.JoinedAt.ToString("d"), 128 + 24 + 22 + createdAtWidth + 8,
                    64 + 54 + 8, _font, g);
            
            var fontSmall = new Font(Bot.FontCollection.Families.First(), 12, FontStyle.Bold);
            g.DrawString(Bot.Name, fontSmall, new SolidBrush(Color.FromArgb(255, 229, 229, 229)),
                image.Width - 4 - g.MeasureString(Bot.Name, fontSmall).Width,
                image.Height - 4 - g.MeasureString(Bot.Name, fontSmall).Height);

            g.Dispose();

            using (var stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);;
                Message.Delete();
                Message.Channel.SendFile("UserInfo.png", stream.ToArray());
            }
        }

        private int DrawCategory(string title, string value, int x, int y, Font font, Graphics g)
        {
            var titleSize = g.MeasureString(title, new Font(font, FontStyle.Bold));
            var valueSize = g.MeasureString(value, font);

            g.FillPath(new SolidBrush(Color.FromArgb(255, 31, 31, 31)), RoundedRect(new Rectangle(x, y,
                (int) (titleSize.Width > valueSize.Width ? titleSize.Width : valueSize.Width) + 14, 54), 5));

            g.DrawString(title, new Font(font, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 248, 248, 248)),
                x + 8, y + 23 - titleSize.Height + 11);
            g.DrawString(value, font, new SolidBrush(Color.FromArgb(255, 248, 248, 248)), x + 8,
                y + 23 + 21 - valueSize.Height + 11);

            return (int) (titleSize.Width > valueSize.Width ? titleSize.Width : valueSize.Width) + 14;
        }

        private Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(bounds.Location, size);
            var path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);

            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private System.Drawing.Image RoundImage(System.Drawing.Image image, int cornerRadius)
        {
            var roundedImage = new Bitmap(image.Width, image.Height);
            using (var g = Graphics.FromImage(roundedImage))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(new TextureBrush(image), RoundedRect(new Rectangle(0, 0, image.Width, image.Height), cornerRadius));
                return roundedImage;
            }
        }
    }
}