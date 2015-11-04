using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	class GridViewColumnCollapsible : GridViewColumn
	{
		public Visibility Visibility
		{
			get
			{
				return (Visibility)GetValue(VisibilityProperty);
			}
			set
			{
				SetValue(VisibilityProperty, value);
			}
		}

		public static readonly DependencyProperty VisibilityProperty =
			DependencyProperty.Register("Visibility", typeof(Visibility),
			typeof(GridViewColumnCollapsible),
			new FrameworkPropertyMetadata(Visibility.Visible,
			FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
			new PropertyChangedCallback(OnVisibilityPropertyChanged)));

		private static void OnVisibilityPropertyChanged(DependencyObject d,
									  DependencyPropertyChangedEventArgs e)
		{
			var column = d as GridViewColumnCollapsible;
			if (column != null)
			{
				column.OnVisibilityChanged((Visibility)e.NewValue);
			}
		}

		private void OnVisibilityChanged(Visibility visibility)
		{
			if (visibility == Visibility.Visible)
			{
				Width = _visibleWidth;
				CellTemplate = _visibleTemplate;
			}
			else
			{
				_visibleWidth = Width;
				_visibleTemplate = CellTemplate;
				Width = 0.0;
				CellTemplate = new DataTemplate();				
			}
		}

		private double _visibleWidth;
		private DataTemplate _visibleTemplate;
	}
}
