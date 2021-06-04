using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace FlushedSelfbot.Command.Commands.Utility
{
    public class Invert : Command
    {
        public Invert() : base("Invert", "Invert the specified image", "Invert <attachment or URL>", Category.Utility)
        {
            
        }

        public override void Execute()
        {
            if (Args.Count == 0 && Message.Attachment == null)
            {
                Message.Channel.SendMessage("> Missing arguments. Syntax: " + Syntax);
                return;
            }

            var webClient = new WebClient();
            var url = Args.Count != 0 ? Args[0] : Message.Attachment.Url;
            var bitmap =
                new Bitmap(System.Drawing.Image.FromStream(new MemoryStream(webClient.DownloadData(url))));

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var color = bitmap.GetPixel(x, y);
                    bitmap.SetPixel(x, y,
                        Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B));
                }
            }

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                Message.Channel.SendFile("Inverted.png", stream.ToArray());
                Message.Delete();
            }
        }
    }
}