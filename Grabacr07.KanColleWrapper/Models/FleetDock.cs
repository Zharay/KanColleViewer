using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
    public class FleetDock : DisposableNotifier
    {
        #region RepairingDock / IsRepairing 変更通知プロパティ

        /// <summary>
		/// 疲労回復の目安時間を取得します。
		/// </summary>
        private RepairingDock _RepairingDock;

        public RepairingDock RepairingDock
		{
            get { return this._RepairingDock; }
			private set
			{
                if (this._RepairingDock != value)
				{
                    this._RepairingDock = value;
					this.RaisePropertyChanged();
                    this.RaisePropertyChanged("IsRepairing");
				}
			}
		}
        
        /// <summary>
        /// 艦隊に編成されている艦娘の疲労を自然回復しているかどうかを示す値を取得します。
        /// </summary>
        public bool IsRepairing
        {
            get { return this.RepairingDock != null && this.RepairingDock.State != RepairingDockState.Repairing; }
        }

        #endregion

        internal void Update(Ship[] s)
        {
            if (s.Length == 0)
            {
                this.RepairingDock = null;
                return;
            }

            var shipIds = s.Select(x => x.Id).ToArray();
            var repairyards = KanColleClient.Current.Homeport.Repairyard.Docks.Values.Where(x => x.Ship != null)
                .Where(x => shipIds.Any(X => X == x.ShipId));

			this.RepairingDock = repairyards.Any() ? repairyards.OrderByDescending(x => x.Remaining).First() : null;
        }
    }
}
