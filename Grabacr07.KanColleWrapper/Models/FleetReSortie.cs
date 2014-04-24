using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦隊の再出撃のためのステータスを表します。
	/// </summary>
	public class FleetReSortie : TimerNotificator
	{
		public static int ReSortieCondition { get; set; }

		private bool notificated;
		private int minCondition;
		private Ship[] prevShips = new Ship[0];

		#region ReadyTime 変更通知プロパティ

		private DateTimeOffset? _ReadyTime;

		/// <summary>
		/// 再出撃の準備が完了する時刻を取得します。
		/// </summary>
		public DateTimeOffset? ReadyTime
		{
			get { return this._ReadyTime; }
			private set
			{
				if (this._ReadyTime != value)
				{
					this._ReadyTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Remaining 変更通知プロパティ

		private TimeSpan? _Remaining;

		public TimeSpan? Remaining
		{
			get { return this._Remaining; }
			private set
			{
				if (this._Remaining != value)
				{
					this._Remaining = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Reason 変更通知プロパティ

		private CanReSortieReason _Reason;

		public CanReSortieReason Reason
		{
			get { return this._Reason; }
			private set
			{
				if (this._Reason != value)
				{
					this._Reason = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(() => CanReSortie);
				}
			}
		}

		#endregion

		/// <summary>
		/// 艦隊が再出撃できるかどうかを示す値を取得します。
		/// </summary>
		public bool CanReSortie
		{
			get { return this.Reason == CanReSortieReason.NoProblem; }
		}

		/// <summary>
		/// 艦隊の出撃準備が完了したときに発生します。
		/// </summary>
		public event EventHandler Readied;

		public event EventHandler<ShipCriticalConditionEventArgs> CriticalCondition;
		public event EventHandler CriticalCleared;

		/// <summary>
		/// 艦隊に編成されている艦娘の状態から、再出撃可能かどうかを判定します。
		/// </summary>
		/// <param name="ships">艦隊に編成されている艦娘。</param>
		internal void Update(Ship[] ships, Repairyard repairyard)
		{
			if (ships.Length == 0)
			{
				this.ReadyTime = null;
				this.UpdateCore();
				if (this.prevShips.Length > 0)
					this.prevShips = new Ship[0];
				return;
			}

			var reason = CanReSortieReason.NoProblem;

			if (ships.Any(s => (s.HP.Current / (double)s.HP.Maximum) <= 0.25))
			{
				reason |= CanReSortieReason.Wounded;

				// We only send out the event once, when the ships has reached critical from its previous state.
				IEnumerable<Ship> CriticalShips = ships.Where(s => (s.HP.Current / (double)s.HP.Maximum) <= 0.25);
				foreach (Ship s in CriticalShips)
				{
					if (this.prevShips.Length > 0 && this.prevShips.First(p => p.Id == s.Id).HP.Current != s.HP.Current && this.CriticalCondition != null)
						this.CriticalCondition(this, new ShipCriticalConditionEventArgs(s));

				}

				var RepairingCritShips = ships.Where(s => (s.HP.Current / (double)s.HP.Maximum) <= 0.25 && repairyard.CheckRepairing(s.Id)).Count();

				if (ships.Where(s => (s.HP.Current / (double)s.HP.Maximum) <= 0.25).Count() == RepairingCritShips && this.CriticalCleared != null)
					this.CriticalCleared(this, new EventArgs());
			}
			else if (this.prevShips.Length > 0 && this.prevShips.Any(s => (s.HP.Current / (double)s.HP.Maximum) <= 0.25))
			{
				Array.Clear(this.prevShips, 0, this.prevShips.Length);
				if (this.CriticalCleared != null)
					this.CriticalCleared(this, new EventArgs());
			}

			if (ships.Any(s => s.Fuel.Current < s.Fuel.Maximum || s.Bull.Current < s.Bull.Maximum))
			{
				reason |= CanReSortieReason.LackForResources;
			}

			var min = ships.Min(x => x.Condition);
			if (min < 40)
			{
				reason |= CanReSortieReason.BadCondition;

				// コンディションの最小値が前回から変更された場合のみ、再出撃可能時刻を更新する
				// (サーバーから来るコンディション値が 3 分に 1 回しか更新されないので、毎回カウントダウンし直すと最大 3 分くらいズレる)
				if (min != this.minCondition)
				{
					this.ReadyTime = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(40 - min));
				}
			}
			else
			{
				this.ReadyTime = null;
			}
			
			this.minCondition = min;
			this.Reason = reason;

			this.UpdateCore();

			if (this.prevShips.Length > 0)
				this.prevShips = new Ship[0];

			this.prevShips = ships;
		}

		private void UpdateCore()
		{
			if (this.ReadyTime.HasValue)
			{
				var remaining = this.ReadyTime.Value - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Readied != null && remaining.Ticks <= 0)
				{
					this.Readied(this, new EventArgs());
					this.notificated = true;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			this.UpdateCore();
		}
	}
}
