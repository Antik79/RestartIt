using System;
using System.Windows;
using System.Windows.Media;

namespace RestartIt
{
    /// <summary>
    /// Singleton service for managing application theming.
    /// Applies font and color settings dynamically to all windows.
    /// </summary>
    public class ThemeService
    {
        private static ThemeService _instance;
        
        /// <summary>
        /// Gets the singleton instance of ThemeService.
        /// </summary>
        public static ThemeService Instance => _instance ??= new ThemeService();

        /// <summary>
        /// Event raised when the theme is changed.
        /// Subscribers should refresh their UI elements.
        /// </summary>
        public event EventHandler ThemeChanged;

        private ThemeService()
        {
        }

        /// <summary>
        /// Applies theme settings from AppSettings to the application.
        /// Updates all resource dictionaries with new font and color values.
        /// </summary>
        /// <param name="settings">The application settings containing theme properties</param>
        public void ApplyTheme(AppSettings settings)
        {
            if (settings == null)
                return;

            var resources = Application.Current.Resources;

            try
            {
                // Apply font settings
                resources["AppFontFamily"] = new FontFamily(settings.FontFamily);
                resources["AppFontSize"] = settings.FontSize;

                // Apply color settings
                resources["BackgroundColor"] = CreateBrush(settings.BackgroundColor);
                resources["TextColor"] = CreateBrush(settings.TextColor);
                resources["HighlightColor"] = CreateBrush(settings.HighlightColor);
                resources["BorderColor"] = CreateBrush(settings.BorderColor);
                resources["SurfaceColor"] = CreateBrush(settings.SurfaceColor);
                resources["SecondaryTextColor"] = CreateBrush(settings.SecondaryTextColor);

                // Update derived colors (for compatibility with existing styles)
                resources["PrimaryBlue"] = CreateBrush(settings.HighlightColor);
                resources["PrimaryBlueHover"] = CreateDarkerBrush(settings.HighlightColor);
                resources["PrimaryBlueLight"] = CreateLighterBrush(settings.HighlightColor);
                resources["SurfaceWhite"] = CreateBrush(settings.SurfaceColor);
                resources["SurfaceGray"] = CreateBrush(settings.BackgroundColor);
                resources["BorderLight"] = CreateBrush(settings.BorderColor);
                resources["TextPrimary"] = CreateBrush(settings.TextColor);
                resources["TextSecondary"] = CreateBrush(settings.SecondaryTextColor);

                // Raise event to notify subscribers
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a SolidColorBrush from a hex color string.
        /// </summary>
        /// <param name="hexColor">Hex color string (e.g., "#FF0000" or "#F5F5F5")</param>
        /// <returns>A SolidColorBrush with the specified color</returns>
        private SolidColorBrush CreateBrush(string hexColor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hexColor))
                    return new SolidColorBrush(Colors.White);

                // Ensure hex color starts with #
                if (!hexColor.StartsWith("#"))
                    hexColor = "#" + hexColor;

                var color = (Color)ColorConverter.ConvertFromString(hexColor);
                return new SolidColorBrush(color);
            }
            catch
            {
                // Fallback to white if color parsing fails
                return new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Creates a darker version of the specified color.
        /// </summary>
        private SolidColorBrush CreateDarkerBrush(string hexColor)
        {
            try
            {
                var brush = CreateBrush(hexColor);
                var color = brush.Color;
                
                // Darken by reducing RGB values by 20%
                var darkerColor = Color.FromRgb(
                    (byte)(color.R * 0.8),
                    (byte)(color.G * 0.8),
                    (byte)(color.B * 0.8)
                );
                
                return new SolidColorBrush(darkerColor);
            }
            catch
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        /// <summary>
        /// Creates a lighter version of the specified color.
        /// </summary>
        private SolidColorBrush CreateLighterBrush(string hexColor)
        {
            try
            {
                var brush = CreateBrush(hexColor);
                var color = brush.Color;
                
                // Lighten by increasing RGB values (towards white)
                var lighterColor = Color.FromRgb(
                    (byte)Math.Min(255, color.R + (255 - color.R) * 0.7),
                    (byte)Math.Min(255, color.G + (255 - color.G) * 0.7),
                    (byte)Math.Min(255, color.B + (255 - color.B) * 0.7)
                );
                
                return new SolidColorBrush(lighterColor);
            }
            catch
            {
                return new SolidColorBrush(Colors.LightGray);
            }
        }

        /// <summary>
        /// Resets theme to default values.
        /// </summary>
        public void ResetToDefaults()
        {
            var defaultSettings = new AppSettings();
            ApplyTheme(defaultSettings);
        }
    }
}

