using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Tool;

namespace Tools.Ui
{
    public class ObjectToImageConverter : MarkupExtension, IValueConverter
    {
        public static string PathTemplate = @"Images\{0}.{1}";
        public static string ImageMissing = "ImageMissing";
        public static string DefaultExtension = "png";

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ITool tool = value as ITool;
            if (tool == null)
                return null;
            string extension = parameter == null ? DefaultExtension : parameter.ToString();
            var path = string.Format(PathTemplate, tool.Name, extension);
            if (!File.Exists(path))
                path = string.Format(PathTemplate, ImageMissing, extension);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path, UriKind.Relative);
            image.EndInit();
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
