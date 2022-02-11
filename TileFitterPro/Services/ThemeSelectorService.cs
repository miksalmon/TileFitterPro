using System;
using System.Threading.Tasks;

using TileFitterPro.Helpers;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace TileFitterPro.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme { get; set; } = ElementTheme.Default;

        public static void Initialize()
        {
            Theme = LoadThemeFromSettings();
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();
            SaveThemeInSettings(Theme);
        }

        public static async Task SetRequestedThemeAsync()
        {
            foreach (var view in CoreApplication.Views)
            {
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = Theme;
                    }
                });
            }
        }

        private static ElementTheme LoadThemeFromSettings()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = ApplicationData.Current.LocalSettings.Values[SettingsKey] as string;

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private static void SaveThemeInSettings(ElementTheme theme)
        {
             ApplicationData.Current.LocalSettings.Values[SettingsKey] = theme.ToString();
        }
    }
}
