// David Wahid
using System;
using System.Globalization;
using System.IO;
using shared.Models.Analysis;
using Xamarin.Forms;

namespace mobile.Utils
{
    public class StatusToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (OrderStatus)value;
            switch (status)
            {
                case OrderStatus.Completed: return true;
                default: return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OrderStatus.Accepted;
        }
    }

    public class ByteToImageSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bytes = (byte[])value;
            var stream = new MemoryStream(bytes);

            return ImageSource.FromStream(() => stream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new byte[0];
        }
    }

    public class ScoreToStars : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var score = (int)value;

            if (score < 60) return string.Empty;
            else if (score < 70) return "\uf005";
            else if (score < 80) return "\uf005\uf005";
            else if (score < 90) return "\uf005\uf005\uf005";
            else if (score < 95) return "\uf005\uf005\uf005\uf005";
            else return "\uf005\uf005\uf005\uf005\uf005";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
