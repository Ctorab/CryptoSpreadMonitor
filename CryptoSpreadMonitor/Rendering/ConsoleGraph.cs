using ScottPlot;
using System.Drawing;

namespace CryptoSpreadMonitor.Rendering
{
    public static class ConsoleGraph
    {
        private const int DEFAULT_WIDTH = 600;
        private const int DEFAULT_HEIGHT = 400;

        public static void Draw(in IReadOnlyList<decimal> data, Size? imgSize = null)
        {
            var imageSize = imgSize ?? new Size(100, 30);

            var plt = new Plot();
            plt.Add.Signal(data);
            plt.Title("Spread Graph");
            plt.XLabel("Time");
            plt.YLabel("Spread Value");

            var html = plt.GetPngHtml(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            var bitmap = ImageConverter.Base64HtmlImageToBitmap(html);

            ConsoleImageRenderer.RenderImage(bitmap, imageSize, 0, 0);
        }
    }
}