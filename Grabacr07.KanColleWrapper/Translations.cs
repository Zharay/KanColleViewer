using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class Translations : NotificationObject
	{
		private XDocument shipsXml;
		private XDocument shipTypesXml;
		private XDocument equipmentXml;
		private XDocument operationsXml;
		private XDocument questsXml;
		private XDocument expeditionsXml;
		private string currentCulture;

		public bool EnableTranslations { get; set; }
		public bool EnableAddUntranslated { get; set; }
		
		#region EquipmentVersion 変更通知プロパティ

		private string _EquipmentVersion;

		public string EquipmentVersion
		{
			get { return this._EquipmentVersion; }
			set
			{
				if (this._EquipmentVersion != value)
				{
					this._EquipmentVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region OperationsVersion 変更通知プロパティ

		private string _OperationsVersion;

		public string OperationsVersion
		{
			get { return this._OperationsVersion; }
			set
			{
				if (this._OperationsVersion != value)
				{
					this._OperationsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region QuestsVersion 変更通知プロパティ

		private string _QuestsVersion;

		public string QuestsVersion
		{
			get { return this._QuestsVersion; }
			set
			{
				if (this._QuestsVersion != value)
				{
					this._QuestsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipsVersion 変更通知プロパティ

		private string _ShipsVersion;

		public string ShipsVersion
		{
			get { return this._ShipsVersion; }
			set
			{
				if (this._ShipsVersion != value)
				{
					this._ShipsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipTypesVersion 変更通知プロパティ

		private string _ShipTypesVersion;

		public string ShipTypesVersion
		{
			get { return this._ShipTypesVersion; }
			set
			{
				if (this._ShipTypesVersion != value)
				{
					this._ShipTypesVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        #region ExpeditionsVersion 変更通知プロパティ

        private string _ExpeditionsVersion;

        public string ExpeditionsVersion
        {
            get { return this._ExpeditionsVersion; }
            set
            {
                if (this._ExpeditionsVersion != value)
                {
	                this._ExpeditionsVersion = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

		internal Translations()
		{
			try
			{
				if (File.Exists("Translations\\Ships.xml")) this.shipsXml = XDocument.Load("Translations\\Ships.xml");
				if (File.Exists("Translations\\ShipTypes.xml")) this.shipTypesXml = XDocument.Load("Translations\\ShipTypes.xml");
				if (File.Exists("Translations\\Equipment.xml")) this.equipmentXml = XDocument.Load("Translations\\Equipment.xml");
				if (File.Exists("Translations\\Operations.xml")) this.operationsXml = XDocument.Load("Translations\\Operations.xml");
				if (File.Exists("Translations\\Quests.xml")) this.questsXml = XDocument.Load("Translations\\Quests.xml");
                if (File.Exists("Translations\\Expeditions.xml")) this.expeditionsXml = XDocument.Load("Translations\\Expeditions.xml");

				this.GetVersions();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void ChangeCulture(string culture)
		{
			this.currentCulture = culture == "en-US" || culture == "en" ? "" : (culture == "ja-JP" || culture == "ja" ? "ja-JP" : (culture + "\\"));

			this.shipsXml = null;
			this.shipTypesXml = null;
			this.equipmentXml = null;
			this.operationsXml = null;
			this.questsXml = null;
			this.expeditionsXml = null;

			if (!this.EnableTranslations || this.currentCulture == "ja-JP")
			{
				this.RaisePropertyChanged("CurrentCulture");
				return;
			}

			try
			{
				if (!Directory.Exists("Translations")) Directory.CreateDirectory("Translations");
				if (File.Exists("Translations\\Ships.xml")) this.shipsXml = XDocument.Load("Translations\\Ships.xml");
				if (File.Exists("Translations\\ShipTypes.xml")) this.shipTypesXml = XDocument.Load("Translations\\ShipTypes.xml");
				if (File.Exists("Translations\\Equipment.xml")) this.equipmentXml = XDocument.Load("Translations\\Equipment.xml");
				if (File.Exists("Translations\\Operations.xml")) this.operationsXml = XDocument.Load("Translations\\Operations.xml");
				if (File.Exists("Translations\\Quests.xml")) this.questsXml = XDocument.Load("Translations\\Quests.xml");
                if (File.Exists("Translations\\Expeditions.xml")) this.expeditionsXml = XDocument.Load("Translations\\Expeditions.xml");

				this.GetVersions();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			this.RaisePropertyChanged("CurrentCulture");
		}

		private void GetVersions()
		{
			// ReSharper disable PossibleNullReferenceException
			try
			{
				this.ShipsVersion = this.shipsXml.Root.Attribute("Version").Value;
			}
			catch (NullReferenceException)
			{
				this.ShipsVersion = "0.0.0";
			}

			try
			{
				this.ShipTypesVersion = this.shipTypesXml.Root.Attribute("Version").Value;
			}
			catch (NullReferenceException)
			{
				this.ShipTypesVersion = "0.0.0";
			}

			try
			{
				this.EquipmentVersion = this.equipmentXml.Root.Attribute("Version").Value;
			}
			catch (NullReferenceException)
			{
				this.EquipmentVersion = "0.0.0";
			}

			try
			{
				this.OperationsVersion = this.operationsXml.Root.Attribute("Version").Value;
			}
			catch (NullReferenceException)
			{
				this.OperationsVersion = "0.0.0";
			}

			try
			{
				this.QuestsVersion = this.questsXml.Root.Attribute("Version").Value;
			}
			catch (NullReferenceException)
			{
				this.QuestsVersion = "0.0.0";
			}

			try
			{
				this.ExpeditionsVersion = this.expeditionsXml.Root.Attribute("Version").Value;
			}
			catch (NullReferenceException)
			{
				this.ExpeditionsVersion = "0.0.0";
			}
			// ReSharper restore PossibleNullReferenceException
		}

		private IEnumerable<XElement> GetTranslationList(TranslationType type)
		{
			switch(type)
			{
				case TranslationType.Ships:
					if (this.shipsXml != null) 
						return this.shipsXml.Descendants("Ship");
					break;
				case TranslationType.ShipTypes:
					if (this.shipTypesXml != null) 
						return this.shipTypesXml.Descendants("Type");
					break;
				case TranslationType.Equipment:
					if (this.equipmentXml != null) 
						return this.equipmentXml.Descendants("Item");
					break;
				case TranslationType.OperationMaps:
					if (this.operationsXml != null) 
						return this.operationsXml.Descendants("Map");
					break;
				case TranslationType.OperationSortie:
					if (this.operationsXml != null) 
						return this.operationsXml.Descendants("Sortie");
					break;
				case TranslationType.Quests:
				case TranslationType.QuestTitle:
				case TranslationType.QuestDetail:
					if (this.questsXml != null) 
						return this.questsXml.Descendants("Quest");
					break;
                case TranslationType.Expeditions:
                case TranslationType.ExpeditionTitle:
                case TranslationType.ExpeditionDetail:
                    if (this.expeditionsXml != null)
                        return this.expeditionsXml.Descendants("Expedition");
                    break;
			}

			return null;
		}

		public string GetTranslation(string jpString, TranslationType type, Object rawData, int id = -1)
		{
			if (!this.EnableTranslations || this.currentCulture == "ja-JP")
				return jpString;

			try
			{
				IEnumerable<XElement> translationList = this.GetTranslationList(type);

				if (translationList == null)
				{
					this.AddTranslation(rawData, type);
					return jpString;
				}

				string jpChildElement = "JP-Name";
				string trChildElement = "TR-Name";

				if (type == TranslationType.QuestDetail || type == TranslationType.ExpeditionDetail)
				{
					jpChildElement = "JP-Detail";
					trChildElement = "TR-Detail";
				}

				string translate = jpString;
				if (this.GetTranslation(jpString, translationList, jpChildElement, trChildElement, id, ref translate))
				{
					return translate;
				}
#if DEBUG
				Debug.WriteLine("Can't find Translation: {0,-20} {1}", jpString, id);
#endif
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			this.AddTranslation(rawData, type);

			return jpString;
		}

		public bool GetTranslation(string jpString, IEnumerable<XElement> translationList, String jpChildElement, String trChildElement, int id , ref string translate)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			IEnumerable<XElement> foundTranslation = translationList.Where(el =>
			{
				try
				{
					// ReSharper disable PossibleNullReferenceException
					if (el.Element(jpChildElement).Value.Equals(jpString)) return true;
					if (el.Attribute("mode") != null)
					{
						if (el.Attribute("mode").Value.Equals("suffix"))
						{
							int sl = el.Element(jpChildElement).Value.Length;
							if (jpString.Length > sl)
							{
								if (el.Element(jpChildElement).Value.Equals(jpString.Substring(jpString.Length - sl))) return true;
							}
						}
					}
					// ReSharper restore PossibleNullReferenceException
				}
				catch
				{
					return false;
				}
				return false;
			});

			bool foundWrongId = false;
			int n;
			foreach (XElement el in foundTranslation)
			{
				// #if DEBUG
				// 					if (ID >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == ID)
				// 						Debug.WriteLine(string.Format("Translation: {0,-20} {1,-20} {2}", JPString, el.Element(TRChildElement).Value, ID));
				// #endif

				if (el.Attribute("mode") != null && !el.Attribute("mode").Value.Equals("normal"))
				{
					if (el.Attribute("mode").Value.Equals("suffix"))
					{
						try
						{
							// ReSharper disable PossibleNullReferenceException
							string t = jpString.Substring(0, jpString.Length - el.Element(jpChildElement).Value.Length);
							// ReSharper disable once PossibleMultipleEnumeration
							if (this.GetTranslation(t, translationList, jpChildElement, trChildElement, -1, ref t))
							{
								if ((el.Attribute("suffixType") != null) && el.Attribute("suffixType").Value.Equals("pre")) translate = el.Element(trChildElement).Value + t;
								else translate = t + el.Element(trChildElement).Value;
								return true;
							}
							// ReSharper restore PossibleNullReferenceException
						}
						catch (NullReferenceException)
						{
							
						}
					}
					continue;
				}

				try
				{
					// ReSharper disable PossibleNullReferenceException
					if (id >= 0)
					{
						if (!Int32.TryParse(el.Element("ID").Value, out n))
						{
							foundWrongId = true;
							translate = el.Element(trChildElement).Value;
						}
						else
						{
							if (id >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == id)
							{
								translate = el.Element(trChildElement).Value;
								return true;
							}
						}
					}
					else
					{
						translate = el.Element(trChildElement).Value;
						return true;
					}
					// ReSharper restore PossibleNullReferenceException
				}
				catch (NullReferenceException)
				{
					
				}
			}

			if (foundWrongId)
			{
				// #if DEBUG
				// 					Debug.WriteLine(string.Format("Wrong ID: {0,-20} {1,-20} {2}", JPString, translate, ID));
				// #endif
				return true;
			}
						
			return false;
		}

		public void AddTranslation(Object rawData, TranslationType type)
		{
			if (rawData == null || !this.EnableAddUntranslated)
				return;
            
			try
			{
				switch (type)
				{
					case TranslationType.Ships:
						if (this.shipsXml == null)
						{
							this.shipsXml = new XDocument();
							this.shipsXml.Add(new XElement("Ships"));
							// ReSharper disable once PossibleNullReferenceException
							this.shipsXml.Root.SetAttributeValue("Version", "0.0.0");
							this.ShipsVersion = "0.0.0";
						}

						kcsapi_mst_ship shipData = rawData as kcsapi_mst_ship;

						if (shipData == null)
							return;

						// ReSharper disable once PossibleNullReferenceException
						this.shipsXml.Root.Add(new XElement("Ship",
							new XElement("JP-Name", shipData.api_name),
							new XElement("TR-Name", shipData.api_name)
						));

						this.shipsXml.Save("Translations\\Ships.xml");
						break;

					case TranslationType.ShipTypes:
						if (this.shipTypesXml == null)
						{
							this.shipTypesXml = new XDocument();
							this.shipTypesXml.Add(new XElement("ShipTypes"));
							// ReSharper disable once PossibleNullReferenceException
							this.shipTypesXml.Root.SetAttributeValue("Version", "0.0.0");
							this.ShipTypesVersion = "0.0.0";
						}

						kcsapi_mst_stype typeData = rawData as kcsapi_mst_stype;

						if (typeData == null)
							return;

						// ReSharper disable once PossibleNullReferenceException
						this.shipTypesXml.Root.Add(new XElement("Type",
							new XElement("ID", typeData.api_id),
							new XElement("JP-Name", typeData.api_name),
							new XElement("TR-Name", typeData.api_name)
							));

						this.shipTypesXml.Save("Translations\\ShipTypes.xml");
						break;

					case TranslationType.Equipment:
						if (this.equipmentXml == null)
						{
							this.equipmentXml = new XDocument();
							this.equipmentXml.Add(new XElement("Equipment"));
							// ReSharper disable once PossibleNullReferenceException
							this.equipmentXml.Root.SetAttributeValue("Version", "0.0.0");
							this.EquipmentVersion = "0.0.0";
						}

						kcsapi_mst_slotitem eqiupData = rawData as kcsapi_mst_slotitem;

						if (eqiupData == null)
							return;

						// ReSharper disable once PossibleNullReferenceException
						this.equipmentXml.Root.Add(new XElement("Item",
							new XElement("JP-Name", eqiupData.api_name),
							new XElement("TR-Name", eqiupData.api_name)
							));

						this.equipmentXml.Save("Translations\\Equipment.xml");
						break;

					case TranslationType.OperationMaps:
					case TranslationType.OperationSortie:
						if (this.operationsXml == null)
						{
							this.operationsXml = new XDocument();
							this.operationsXml.Add(new XElement("Operations"));
							// ReSharper disable once PossibleNullReferenceException
							this.operationsXml.Root.SetAttributeValue("Version", "0.0.0");
							this.OperationsVersion = "0.0.0";
						}

						kcsapi_battleresult operationsData = rawData as kcsapi_battleresult;

						if (operationsData == null)
							return;

						if (type == TranslationType.OperationMaps)
						{
							// ReSharper disable once PossibleNullReferenceException
							this.operationsXml.Root.Add(new XElement("Map",
								new XElement("JP-Name", operationsData.api_quest_name),
								new XElement("TR-Name", operationsData.api_quest_name)
								));
						}
						else
						{
							// ReSharper disable once PossibleNullReferenceException
							this.operationsXml.Root.Add(new XElement("Sortie",
								new XElement("JP-Name", operationsData.api_enemy_info.api_deck_name),
								new XElement("TR-Name", operationsData.api_enemy_info.api_deck_name)
								));
						}

						this.operationsXml.Save("Translations\\Operations.xml");
						break;

					case TranslationType.Quests:
					case TranslationType.QuestTitle:
					case TranslationType.QuestDetail:
						if (this.questsXml == null)
						{
							this.questsXml = new XDocument();
							this.questsXml.Add(new XElement("Quests"));
							// ReSharper disable once PossibleNullReferenceException
							this.questsXml.Root.SetAttributeValue("Version", "0.0.0");
							this.QuestsVersion = "0.0.0";
						}

						kcsapi_quest questData = rawData as kcsapi_quest;

						if (questData == null)
							return;

						IEnumerable<XElement> foundTranslation = this.questsXml.Descendants("Quest").Where(
							b => b.Element("ID") != null && b.Element("JP-Name") != null && b.Element("JP-Detail") != null && b.Element("ID").Value.Equals(questData.api_no.ToString())
						);

						// ReSharper disable PossibleMultipleEnumeration
						if (foundTranslation.Any())
						{
							foreach (XElement el in foundTranslation)
							{
								if (el.Element("JP-Name") == null) el.Add(new XElement("JP-Name", questData.api_title));
								else el.Element("JP-Name").Value = questData.api_title;
								if (el.Element("JP-Detail") == null) el.Add(new XElement("JP-Detail", questData.api_detail));
								else el.Element("JP-Detail").Value = questData.api_detail;
							}
						}
						else
						{
							int n;
							bool needAdd = true;
							IEnumerable<XElement> foundTranslationDetail = this.questsXml.Descendants("Quest").Where(b => b.Element("JP-Detail").Value.Equals(questData.api_detail));
							IEnumerable<XElement> foundTranslationTitle = this.questsXml.Descendants("Quest").Where(b => b.Element("JP-Name").Value.Equals(questData.api_title));

							// Check the current list for any errors and fix them before writing a whole new element.
							if (type == TranslationType.QuestTitle && foundTranslationDetail.Any())
							{
								// The title is wrong, but the detail is right. Fix the title.
								foreach (XElement el in foundTranslationDetail)
								{
									if (!Int32.TryParse(el.Element("ID").Value, out n))
									{
										if (el.Element("JP-Name") == null) el.Add(new XElement("JP-Name", questData.api_title));
										else el.Element("JP-Name").Value = questData.api_title;
										needAdd = false;
									}
								}

							}
							else if (type == TranslationType.QuestDetail && foundTranslationTitle.Any())
							{
								// We found an existing detail, the title must be broken. Fix it.
								foreach (XElement el in foundTranslationTitle)
								{
									if (!Int32.TryParse(el.Element("ID").Value, out n))
									{
										if (el.Element("JP-Detail") == null) el.Add(new XElement("JP-Detail", questData.api_detail));
										else el.Element("JP-Detail").Value = questData.api_detail;
										needAdd = false;
									}
								}									
							}
							
							if (needAdd)
							{
								// The quest doesn't exist at all. Add it.
								// ReSharper disable once PossibleNullReferenceException
								this.questsXml.Root.Add(new XElement("Quest",
									new XElement("ID", questData.api_no),
									new XElement("JP-Name", questData.api_title),
									new XElement("TR-Name", questData.api_title),
									new XElement("JP-Detail", questData.api_detail),
									new XElement("TR-Detail", questData.api_detail)
									));
							}
						}
						// ReSharper restore PossibleMultipleEnumeration

						this.questsXml.Save("Translations\\Quests.xml");
						break;
                    case TranslationType.Expeditions:
                    case TranslationType.ExpeditionTitle:
                    case TranslationType.ExpeditionDetail:
                        if (this.expeditionsXml == null)
                        {
	                        this.expeditionsXml = new XDocument();
	                        this.expeditionsXml.Add(new XElement("Expeditions"));
	                        // ReSharper disable once PossibleNullReferenceException
	                        this.expeditionsXml.Root.SetAttributeValue("Version", "0.0.0");
	                        this.ExpeditionsVersion = "0.0.0";
                        }

                        kcsapi_mission expeditionData = rawData as kcsapi_mission;

                        if (expeditionData == null)
							return;

                        IEnumerable<XElement> foundTranslationExpedition = this.expeditionsXml.Descendants("Expedition").Where(b => b.Element("ID").Value.Equals(expeditionData.api_id.ToString()));
                        
                        // Check the current list for any errors and fix them before writing a whole new element.
						// ReSharper disable PossibleMultipleEnumeration
						if (foundTranslationExpedition.Any())
                        {
							foreach (var el in foundTranslationExpedition)
							{
								if (el.Element("JP-Name") == null) el.Add(new XElement("JP-Name", expeditionData.api_name));
								else el.Element("JP-Name").Value = expeditionData.api_name;
								if (el.Element("JP-Detail") == null) el.Add(new XElement("JP-Detail", expeditionData.api_details));
								else el.Element("JP-Detail").Value = expeditionData.api_details;
							}

                        }
                        else
                        {
                            // The quest doesn't exist at all. Add it.
	                        // ReSharper disable once PossibleNullReferenceException
	                        this.expeditionsXml.Root.Add(new XElement("Expedition",
                                new XElement("ID", expeditionData.api_id),
                                new XElement("JP-Name", expeditionData.api_name),
                                new XElement("TR-Name", expeditionData.api_name),
                                new XElement("JP-Detail", expeditionData.api_details),
                                new XElement("TR-Detail", expeditionData.api_details)
                                ));
                        }

						this.expeditionsXml.Save("Translations\\Expeditions.xml");
						// ReSharper restore PossibleMultipleEnumeration
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
