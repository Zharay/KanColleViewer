using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Properties;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ShipViewModel : ViewModel
	{
		public Ship Ship { get; private set; }

		public ShipViewModel(Ship ship)
		{
			this.Ship = ship;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(ResourceService.Current)
			{
				(sender, args) =>
				{
					this.RaisePropertyChanged("Ship");
				},
			});
		}
	}
}
