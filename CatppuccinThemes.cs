using System;
using System.Collections.Generic;

namespace RestartIt
{
    /// <summary>
    /// Represents a Catppuccin theme flavor with all palette colors.
    /// </summary>
    public class CatppuccinTheme
    {
        public string Name { get; set; }
        public string Base { get; set; }
        public string Crust { get; set; }
        public string Mantle { get; set; }
        public string Surface0 { get; set; }
        public string Surface1 { get; set; }
        public string Surface2 { get; set; }
        public string Overlay0 { get; set; }
        public string Overlay1 { get; set; }
        public string Overlay2 { get; set; }
        public string Text { get; set; }
        public string Subtext0 { get; set; }
        public string Subtext1 { get; set; }
        public string Blue { get; set; }
        public string Red { get; set; }
        public string Green { get; set; }
        public string Yellow { get; set; }
        public string Pink { get; set; }
        public string Mauve { get; set; }
        public string Peach { get; set; }
        public string Teal { get; set; }
        public string Sky { get; set; }
        public string Lavender { get; set; }
        public string Rosewater { get; set; }
        public string Flamingo { get; set; }

        /// <summary>
        /// Maps Catppuccin theme colors to AppSettings color properties following the style guide.
        /// </summary>
        public void ApplyToAppSettings(AppSettings settings)
        {
            if (settings == null)
                return;

            // Following Catppuccin style guide mapping:
            // Background → Base (main background pane)
            settings.BackgroundColor = Base;
            // Text → Text (body copy, main headlines)
            settings.TextColor = Text;
            // Highlight → Blue (primary accent, buttons, links)
            settings.HighlightColor = Blue;
            // Border → Overlay0 (borders, dividers)
            settings.BorderColor = Overlay0;
            // Surface → Surface0 (surface elements, cards)
            settings.SurfaceColor = Surface0;
            // Secondary Text → Subtext1 (sub-headlines, labels)
            settings.SecondaryTextColor = Subtext1;
        }
    }

    /// <summary>
    /// Provides access to all Catppuccin theme flavors.
    /// </summary>
    public static class CatppuccinThemes
    {
        private static readonly Dictionary<string, CatppuccinTheme> _themes = new Dictionary<string, CatppuccinTheme>();

        static CatppuccinThemes()
        {
            InitializeThemes();
        }

        /// <summary>
        /// Gets a Catppuccin theme by name (Latte, Frappe, Macchiato, Mocha).
        /// </summary>
        public static CatppuccinTheme GetTheme(string name)
        {
            return _themes.TryGetValue(name, out var theme) ? theme : null;
        }

        /// <summary>
        /// Gets all available theme names.
        /// </summary>
        public static IEnumerable<string> GetThemeNames()
        {
            return _themes.Keys;
        }

        private static void InitializeThemes()
        {
            // Latte (Light theme)
            _themes["Latte"] = new CatppuccinTheme
            {
                Name = "Latte",
                Base = "#EFF1F5",
                Crust = "#DCE0E8",
                Mantle = "#E6E9EF",
                Surface0 = "#CCD0DA",
                Surface1 = "#BCC0CC",
                Surface2 = "#ACB0BE",
                Overlay0 = "#9CA0B0",
                Overlay1 = "#8C8FA1",
                Overlay2 = "#7C7F93",
                Text = "#4C4F69",
                Subtext0 = "#6C6F85",
                Subtext1 = "#5C5F77",
                Blue = "#1E66F5",
                Red = "#D20F39",
                Green = "#40A02B",
                Yellow = "#DF8E1D",
                Pink = "#EA76CB",
                Mauve = "#8839EF",
                Peach = "#FE640B",
                Teal = "#179299",
                Sky = "#04A5E5",
                Lavender = "#7287FD",
                Rosewater = "#DC8A78",
                Flamingo = "#DD7878"
            };

            // Frappe
            _themes["Frappe"] = new CatppuccinTheme
            {
                Name = "Frappe",
                Base = "#303446",
                Crust = "#232634",
                Mantle = "#292C3C",
                Surface0 = "#414559",
                Surface1 = "#51576D",
                Surface2 = "#626880",
                Overlay0 = "#737994",
                Overlay1 = "#838BA7",
                Overlay2 = "#949CBB",
                Text = "#C6D0F5",
                Subtext0 = "#A5ADCE",
                Subtext1 = "#B5BFE2",
                Blue = "#8CAAEE",
                Red = "#E78284",
                Green = "#A6D189",
                Yellow = "#E5C890",
                Pink = "#F4B8E4",
                Mauve = "#CA9EE6",
                Peach = "#EF9F76",
                Teal = "#81C8BE",
                Sky = "#99D1DB",
                Lavender = "#BABBF1",
                Rosewater = "#F2D5CF",
                Flamingo = "#EEBEBE"
            };

            // Macchiato
            _themes["Macchiato"] = new CatppuccinTheme
            {
                Name = "Macchiato",
                Base = "#24273A",
                Crust = "#181926",
                Mantle = "#1E2030",
                Surface0 = "#363A4F",
                Surface1 = "#494D64",
                Surface2 = "#5B6078",
                Overlay0 = "#6E738D",
                Overlay1 = "#8087A2",
                Overlay2 = "#939AB7",
                Text = "#CAD3F5",
                Subtext0 = "#A5ADCB",
                Subtext1 = "#B8C0E0",
                Blue = "#8AADF4",
                Red = "#ED8796",
                Green = "#A6DA95",
                Yellow = "#EED49F",
                Pink = "#F5BDE6",
                Mauve = "#C6A0F6",
                Peach = "#F5A97F",
                Teal = "#8BD5CA",
                Sky = "#91D7E3",
                Lavender = "#B7BDF8",
                Rosewater = "#F4DBD6",
                Flamingo = "#F0C6C6"
            };

            // Mocha (Dark theme)
            _themes["Mocha"] = new CatppuccinTheme
            {
                Name = "Mocha",
                Base = "#1E1E2E",
                Crust = "#11111B",
                Mantle = "#181825",
                Surface0 = "#313244",
                Surface1 = "#45475A",
                Surface2 = "#585B70",
                Overlay0 = "#6C7086",
                Overlay1 = "#7F849C",
                Overlay2 = "#9399B2",
                Text = "#CDD6F4",
                Subtext0 = "#A6ADC8",
                Subtext1 = "#BAC2DE",
                Blue = "#89B4FA",
                Red = "#F38BA8",
                Green = "#A6E3A1",
                Yellow = "#F9E2AF",
                Pink = "#F5C2E7",
                Mauve = "#CBA6F7",
                Peach = "#FAB387",
                Teal = "#94E2D5",
                Sky = "#89DCEB",
                Lavender = "#B4BEFE",
                Rosewater = "#F5E0DC",
                Flamingo = "#F2CDCD"
            };
        }
    }
}

