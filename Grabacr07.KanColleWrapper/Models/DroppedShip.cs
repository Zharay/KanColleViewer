using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 工廠で開発された装備アイテムを表します。
	/// </summary>
	public class DroppedShip : RawDataWrapper<kcsapi_battleresult_getship>
	{
		public ShipInfo ShipInfo { get; private set; }

		public DroppedShip(kcsapi_battleresult_getship rawData)
			: base(rawData)
		{
			try
			{
				this.ShipInfo = KanColleClient.Current.Master.Ships[rawData.api_ship_id];
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
