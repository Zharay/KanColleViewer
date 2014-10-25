using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public class GridViewBehaviors : GridViewColumnHeader
	{
		public static readonly DependencyProperty CollapseableColumnProperty = DependencyProperty.RegisterAttached("CollapseableColumn", typeof(bool), typeof(GridViewBehaviors), new UIPropertyMetadata(false, OnCollapseableColumnChanged));

		public static bool GetCollapseableColumn(DependencyObject d)
		{
			return (bool)d.GetValue(CollapseableColumnProperty);
		}

		public static void SetCollapseableColumn(DependencyObject d, bool value)
		{
			d.SetValue(CollapseableColumnProperty, value);
		}

		private static void OnCollapseableColumnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			GridViewColumnHeader header = sender as GridViewColumnHeader;
			if (header == null)
				return;

			header.IsVisibleChanged += new DependencyPropertyChangedEventHandler(AdjustWidth);
		}

		static void AdjustWidth(object sender, DependencyPropertyChangedEventArgs e)
		{
			GridViewColumnHeader header = sender as GridViewColumnHeader;
			if (header == null)
				return;

			if (header.Visibility == Visibility.Collapsed)
				header.Column.Width = 0;
			else
				header.Column.Width = double.NaN;   // "Auto"
		}
	}
	public class GridViewColumnVisibilityManager
	{
		static Dictionary<GridViewColumn, DataTemplate> originalCellTemplates = new Dictionary<GridViewColumn, DataTemplate>();
		static Dictionary<GridViewColumn, double> originalColumnWidths = new Dictionary<GridViewColumn, double>();

		public static bool GetIsVisible(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsVisibleProperty);
		}

		public static void SetIsVisible(DependencyObject obj, bool value)
		{
			obj.SetValue(IsVisibleProperty, value);
		}

		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(GridViewColumnVisibilityManager), new UIPropertyMetadata(true, OnIsVisibleChanged));

		private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GridViewColumn gc = d as GridViewColumn;
			if (gc == null)
				return;

			if (GetIsVisible(gc) == false)
			{
				originalCellTemplates[gc] = gc.CellTemplate;
				gc.CellTemplate = null;
				originalColumnWidths[gc] = gc.Width;
				gc.Width = 0;
			}
			else
			{
				if (gc.CellTemplate == null)
					gc.CellTemplate = originalCellTemplates[gc];
				if (gc.Width == 0)
					gc.Width = originalColumnWidths[gc];
			}
		}
	}
}
