using System.Drawing;
using System.Runtime.InteropServices;

namespace CryptoSpreadMonitor.Rendering
{
    internal static class ConsoleImageRenderer
    {
        // Source - https://stackoverflow.com/a/33652557
        // Posted by György Kőszeg, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-05-01, License - CC BY-SA 3.0

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern nint CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            int dwShareMode,
            nint lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            nint hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFont(
            nint hConsoleOutput,
            bool bMaximumWindow,
            [Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleFontInfo lpConsoleCurrentFont);

        [StructLayout(LayoutKind.Sequential)]
        internal class ConsoleFontInfo
        {
            internal int nFont;
            internal Coord dwFontSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Coord
        {
            [FieldOffset(0)]
            internal short X;
            [FieldOffset(2)]
            internal short Y;
        }

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = 0x40000000;
        private const int FILE_SHARE_READ = 1;
        private const int FILE_SHARE_WRITE = 2;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int OPEN_EXISTING = 3;

        public static void RenderImage(Image img, Size size, int x, int y)
        {
            using (var g = Graphics.FromHwnd(GetConsoleWindow()))
            {
                using (var image = img)
                {
                    var fontSize = GetConsoleFontSize();

                    var imageRect = new Rectangle(x, y,
                        size.Width * fontSize.Width,
                        size.Height * fontSize.Height);
                    g.DrawImage(image, imageRect);
                }
            }
        }

        // Source - https://stackoverflow.com/a/33652557
        // Posted by György Kőszeg, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-05-01, License - CC BY-SA 3.0

        public static Size GetConsoleFontSize()
        {
            // getting the console out buffer handle
            nint outHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                nint.Zero,
                OPEN_EXISTING,
                0,
                nint.Zero);
            int errorCode = Marshal.GetLastWin32Error();
            if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", errorCode);
            }

            ConsoleFontInfo cfi = new ConsoleFontInfo();
            if (!GetCurrentConsoleFont(outHandle, false, cfi))
            {
                throw new InvalidOperationException("Unable to get font information.");
            }

            return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
        }

    }
}
