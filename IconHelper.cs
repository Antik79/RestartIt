using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;

namespace RestartIt
{
    public static class IconHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        /// <summary>
        /// Creates a persistent application icon
        /// </summary>
        public static Icon CreateApplicationIcon()
        {
            // Create a larger bitmap for the application icon (32x32)
            var bitmap = new Bitmap(32, 32);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Fill background
                graphics.Clear(Color.Transparent);

                // Draw gradient circle background
                using (var brush = new LinearGradientBrush(
                    new Rectangle(0, 0, 32, 32),
                    Color.FromArgb(255, 40, 167, 69),
                    Color.FromArgb(255, 28, 115, 48),
                    LinearGradientMode.Vertical))
                {
                    graphics.FillEllipse(brush, 3, 3, 26, 26);
                }

                // Draw white border
                using (var pen = new Pen(Color.White, 2))
                {
                    graphics.DrawEllipse(pen, 3, 3, 25, 25);
                }

                // Draw "R" letter with shadow
                using (var font = new Font("Arial", 18, FontStyle.Bold))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    // Shadow
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        graphics.DrawString("R", font, shadowBrush,
                            new RectangleF(1, 1, 32, 32), format);
                    }

                    // Main text
                    using (var textBrush = new SolidBrush(Color.White))
                    {
                        graphics.DrawString("R", font, textBrush,
                            new RectangleF(0, -1, 32, 32), format);
                    }
                }
            }

            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);

            // Clone to ensure it persists
            Icon clonedIcon = (Icon)icon.Clone();

            // Clean up
            DestroyIcon(hIcon);
            bitmap.Dispose();

            return clonedIcon;
        }

        /// <summary>
        /// Creates a system tray icon (16x16)
        /// </summary>
        public static Icon CreateTrayIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Fill background
                graphics.Clear(Color.Transparent);

                // Draw a green circle
                using (var brush = new SolidBrush(Color.FromArgb(255, 40, 167, 69)))
                {
                    graphics.FillEllipse(brush, 1, 1, 14, 14);
                }

                // Draw white border
                using (var pen = new Pen(Color.White, 1.5f))
                {
                    graphics.DrawEllipse(pen, 1, 1, 13, 13);
                }

                // Draw white "R"
                using (var font = new Font("Arial", 9, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString("R", font, textBrush,
                        new RectangleF(0, -1, 16, 16), format);
                }
            }

            // Convert bitmap to icon
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);

            // Clone to ensure it persists
            Icon clonedIcon = (Icon)icon.Clone();

            // Clean up
            DestroyIcon(hIcon);
            bitmap.Dispose();

            return clonedIcon;
        }

        /// <summary>
        /// Saves an icon to a .ico file (useful for creating app.ico)
        /// </summary>
        public static void SaveIconToFile(string filePath)
        {
            using (var icon = CreateApplicationIcon())
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                // Save the icon
                icon.Save(stream);
            }
        }
    }
}