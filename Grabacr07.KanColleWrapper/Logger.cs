using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class Logger : NotificationObject
	{
		public bool EnableLogging { get; set; }

		private bool waitingForShip;
		private int dockid;
		private readonly int[] shipmats;
		private readonly string LogTimestampFormat = "yyyy-MM-dd HH:mm:ss";

		private enum LogType
		{
			BuildItem,
			BuildShip,
			ShipDrop,
			Materials
		};

		internal Logger(KanColleProxy proxy)
		{
			this.shipmats = new int[5];

			// ちょっと考えなおす
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x => this.CreateShip(x.Request));
			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.KDock(x.Data));
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));
			proxy.api_get_member_material.TryParse<kcsapi_material[]>().Subscribe(x => this.MaterialsHistory(x.Data));
			proxy.api_req_hokyu_charge.TryParse<kcsapi_charge>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
			proxy.api_req_kousyou_destroyship.TryParse<kcsapi_destroyship>().Subscribe(x => this.MaterialsHistory(x.Data.api_material));
		}
		
		private void CreateItem(kcsapi_createitem item, NameValueCollection req)
		{
			Log(LogType.BuildItem, "{0},{1},{2},{3},{4},{5},{6}",
				DateTime.Now.ToString(this.LogTimestampFormat),
				item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slot_item.api_slotitem_id].Name : "Penguin",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.ShipType.Name,
				req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"]);
		}

		private void CreateShip(NameValueCollection req)
		{
			this.waitingForShip = true;
			this.dockid = Int32.Parse(req["api_kdock_id"]);
			this.shipmats[0] = Int32.Parse(req["api_item1"]);
			this.shipmats[1] = Int32.Parse(req["api_item2"]);
			this.shipmats[2] = Int32.Parse(req["api_item3"]);
			this.shipmats[3] = Int32.Parse(req["api_item4"]);
			this.shipmats[4] = Int32.Parse(req["api_item5"]);
		}

		private void KDock(kcsapi_kdock[] docks)
		{
			foreach (var dock in docks.Where(dock => this.waitingForShip && dock.api_id == this.dockid))
			{
				this.Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6}", DateTime.Now.ToString(this.LogTimestampFormat), KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name, this.shipmats[0], this.shipmats[1], this.shipmats[2], this.shipmats[3], this.shipmats[4]);
				this.waitingForShip = false;
			}
		}

		private void BattleResult(kcsapi_battleresult br)
		{
			if (br.api_get_ship == null)
				return;

			Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", DateTime.Now.ToString(this.LogTimestampFormat),
				KanColleClient.Current.Translations.GetTranslation(br.api_get_ship.api_ship_name, TranslationType.Ships, br),
				KanColleClient.Current.Translations.GetTranslation(br.api_quest_name, TranslationType.OperationMaps, br),
				KanColleClient.Current.Translations.GetTranslation(br.api_enemy_info.api_deck_name, TranslationType.OperationSortie, br),
				br.api_win_rank);
		}

		private void MaterialsHistory(kcsapi_material[] source)
		{
			if (source == null || source.Length != 7)
				return;

			Log(LogType.Materials, "{0},{1},{2},{3},{4},{5},{6},{7}", 
				DateTime.Now.ToString(this.LogTimestampFormat), 
				source[0].api_value, source[1].api_value, source[2].api_value, source[3].api_value, source[6].api_value, source[5].api_value, source[4].api_value);
		}

		private void MaterialsHistory(int[] source)
		{
			if (source == null || source.Length != 4)
				return;

			Log(LogType.Materials, "{0},{1},{2},{3},{4},{5},{6},{7}", 
				DateTime.Now.ToString(this.LogTimestampFormat), 
				source[0], source[1], source[2], source[3], 
				KanColleClient.Current.Homeport.Materials.DevelopmentMaterials, 
				KanColleClient.Current.Homeport.Materials.InstantRepairMaterials, 
				KanColleClient.Current.Homeport.Materials.InstantBuildMaterials);
		}

		private void Log(LogType type, string format, params object[] args)
		{
			if (!this.EnableLogging) return;

			try
			{
				string mainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

				switch (type)
				{
					case LogType.BuildItem:
						if (!File.Exists(mainFolder + "\\ItemBuildLog.csv"))
						{
							using (var w = File.AppendText(mainFolder + "\\ItemBuildLog.csv"))
							{
								w.WriteLine("Date,Result,Secretary,Fuel,Ammo,Steel,Bauxite");
							}
						}
						using (var w = File.AppendText(mainFolder + "\\ItemBuildLog.csv"))
						{
							w.WriteLine(format, args);
						}
						break;

					case LogType.BuildShip:
						if (!File.Exists(mainFolder + "\\ShipBuildLog.csv"))
						{
							using (var w = File.AppendText(mainFolder + "\\ShipBuildLog.csv"))
							{
								w.WriteLine("Date,Result,Fuel,Ammo,Steel,Bauxite,# of Build Materials");
							}
						}
						using (var w = File.AppendText(mainFolder + "\\ShipBuildLog.csv"))
						{
							w.WriteLine(format, args);
						}
						break;

					case LogType.ShipDrop:
						if (!File.Exists(mainFolder + "\\DropLog.csv"))
						{
							using (var w = File.AppendText(mainFolder + "\\DropLog.csv"))
							{
								w.WriteLine("Date,Result,Operation,Enemy Fleet,Rank");
							}
						}
						using (var w = File.AppendText(mainFolder + "\\DropLog.csv"))
						{
							w.WriteLine(format, args);
						}
						break;
					case LogType.Materials:
						if (!File.Exists(mainFolder + "\\MaterialsLog.csv"))
						{
							using (var w = File.AppendText(mainFolder + "\\MaterialsLog.csv"))
							{
								w.WriteLine("Date,Fuel,Ammunition,Steel,Bauxite,DevKits,Buckets,Flamethrowers");
							}
						}
						using (var w = File.AppendText(mainFolder + "\\MaterialsLog.csv"))
						{
							w.WriteLine(format, args);
						}
						break;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
