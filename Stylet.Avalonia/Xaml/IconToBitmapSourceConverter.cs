using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Stylet.Logging;

namespace Stylet.Xaml
{
    /// <summary>
    /// Converter to take an Icon, and convert it to a BitmapSource
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
        Justification = "Don't agree with prefixing static method calls with the class name")]
    public class IconToBitmapSourceConverter : IValueConverter
    {
        private static readonly ILogger logger = LogManager.GetLogger(typeof(IconToBitmapSourceConverter));

        /// <summary>
        /// Singleton instance of this converter. Usage e.g. Converter="{x:Static s:IconToBitmapSourceConverter.Instance}"
        /// </summary>
        public static readonly IconToBitmapSourceConverter Instance = new IconToBitmapSourceConverter();

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">value as produced by source binding</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">converter parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>Converted value</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is string rawUri && targetType.IsAssignableFrom(typeof(Bitmap)))
            {
                Uri uri;
                // Allow for assembly overrides

                if (rawUri.StartsWith("avares://"))
                {
                    uri = new Uri(rawUri);
                }
                else
                {
                    string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                    uri = new Uri($"avares://{assemblyName}{rawUri}");
                }
                
                //var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var asset = AssetLoader.Open(uri);
                return new Bitmap(asset);
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts a value back. Not supported.
        /// </summary>
        /// <param name="value">value, as produced by target</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">converter parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>Converted back value</returns>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}