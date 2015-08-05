using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class DroppedShipViewModel : NewItemViewModel
	{
		public DroppedShipViewModel()
		{
			this.Name = "-----";
		}

		public void Update(DroppedShip item)
		{
			this.Name = item.ShipInfo.Name;
		}
	}
}
