using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;
using Grabacr07.KanColleViewer.Models;
using Setting = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.Views.Converters
{
    class PreviewSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			try
			{
				if (values.Length != 9) throw new ArgumentException();
                double[] posString = ((String)parameter).Split(',').Select(x => Double.Parse(x)).ToArray();
				Double rootWidth = (Double)values[0];
				Double rootHeight = (Double)values[1];
				Double contentWidth = (Double)values[2];
				Double contentHeight = (Double)values[3];
				Double containerWidth = (Double)values[4];
				Double containerHeight = (Double)values[5];
				Double posX = posString[0] + (containerWidth - contentWidth) / 2;
				Double posY = posString[1] + (containerHeight - contentHeight) / 2;
				if (Setting.Current.Orientation.Mode == Orientation.Horizontal && 
                    Setting.Current.BrowserHorizontalPosition == "Right")
                {
                    posX = rootWidth - contentWidth - posX - posString[2] + posString[0];
                }
				else if (Setting.Current.Orientation.Mode == Orientation.Vertical &&
                    Setting.Current.BrowserVerticalPosition == "Bottom")
                {
                    posY = rootHeight - contentHeight - posY - posString[3] + posString[1];
                }
				return new Thickness(posX, posY, rootWidth - contentWidth - posX, rootHeight - contentHeight - posY);
			}
			catch
			{
				return new Thickness(0,0,0,0);
			}
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
