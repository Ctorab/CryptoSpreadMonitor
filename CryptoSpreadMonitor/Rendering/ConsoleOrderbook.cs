using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace CryptoSpreadMonitor.Rendering
{
    internal static class ConsoleOrderbook
    {
        private static readonly Font _fontTitle = new("Consolas", 14, FontStyle.Bold);
        private static readonly Font _font = new("Consolas", 12, FontStyle.Regular);

        public static Bitmap RenderContent(Action<Graphics> drawAction, int width = 300, int height = 200)
        {
            var bmp = new Bitmap(width, height);

            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(Color.Black);

            drawAction(g);

            return bmp;
        }

        public static void DrawText(Graphics g, int x, int y, string text, bool title = false)
        {
            var font = title ? _fontTitle : _font;
            g.DrawString(text, font, Brushes.White, x, y);
        }
    }
}
