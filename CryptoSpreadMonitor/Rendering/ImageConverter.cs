using System.Drawing;


namespace CryptoSpreadMonitor.Rendering
{
    public static class ImageConverter
    {
        public static Bitmap Base64HtmlImageToBitmap(string html)
        {
            const string marker = "base64,";
            int index = html.IndexOf(marker, StringComparison.Ordinal);

            if (index == -1)
                throw new Exception("No base64 image found.");

            string base64 = html[(index + marker.Length)..];

            int end = base64.IndexOf('"');
            if (end != -1)
                base64 = base64[..end];

            base64 = base64.Trim();

            byte[] bytes = Convert.FromBase64String(base64);

            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}
