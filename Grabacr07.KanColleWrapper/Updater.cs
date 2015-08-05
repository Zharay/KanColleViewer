using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	public class Updater
	{

		private XDocument VersionXML;

		/// <summary>
		/// Loads the version XML file from a remote URL. This houses all current online version info.
		/// </summary>
		/// <param name="UpdateURL">String URL to the version XML file.</param>
		/// <returns>True: Successful, False: Failed</returns>
		public bool LoadVersion(string UpdateURL)
		{
			return LoadVersion(UpdateURL, UpdateURL);
		}

		public bool LoadVersion(string UpdateURL, string UpdateTransURL)
		{
			try
			{
				VersionXML = XDocument.Load(UpdateURL);

				if (VersionXML == null)
					return false;

				if (UpdateURL != UpdateTransURL)
				{
					XDocument TransVersionXML = XDocument.Load(UpdateTransURL);
					if (TransVersionXML != null)
					{
						IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
						foreach (XElement node in TransVersionXML.Root.Elements("Item"))
						{
							// skip app version
							if (!node.Element("Name").Value.Equals("App"))
							{
								var OldNode = Versions.Where(x => x.Element("Name").Value.Equals(node.Element("Name").Value)).FirstOrDefault();
								OldNode.ReplaceWith(node);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Updates any translation files that differ from that found online.
		/// </summary>
		/// <param name="BaseTranslationURL">String URL folder that contains all the translation XML files.</param>
		/// <param name="Culture">Language version to download</param>
		/// <param name="TranslationsRef">Link to the translation engine to obtain current translation versions.</param>
		/// <returns>Returns a state code depending on how it ran. [-1: Error, 0: Nothing to update, 1: Update Successful]</returns>
		public int UpdateTranslations(Translations TranslationsRef, bool CheckVersion = true)
		{
			using (WebClient Client = new WebClient())
			{
				XDocument TestXML;
				int ReturnValue = 0;

				try
				{
					if (!Directory.Exists("Translations")) Directory.CreateDirectory("Translations");
					if (!Directory.Exists("Translations\\tmp\\")) Directory.CreateDirectory("Translations\\tmp\\");

					// In every one of these we download it to a temp folder, check if the file works, then move it over.
					if (!CheckVersion || IsOnlineVersionGreater(TranslationType.Equipment, TranslationsRef.EquipmentVersion))
					{
						Client.DownloadFile(GetOnlineVersion(TranslationType.Equipment, true), "Translations\\tmp\\Equipment.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Equipment.xml");
							if (File.Exists("Translations\\Equipment.xml")) 
								File.Delete("Translations\\Equipment.xml");
							File.Move("Translations\\tmp\\Equipment.xml", "Translations\\Equipment.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (!CheckVersion || IsOnlineVersionGreater(TranslationType.Operations, TranslationsRef.OperationsVersion))
					{
						Client.DownloadFile(GetOnlineVersion(TranslationType.Operations, true), "Translations\\tmp\\Operations.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Operations.xml");
							if (File.Exists("Translations\\Operations.xml"))
								File.Delete("Translations\\Operations.xml");
							File.Move("Translations\\tmp\\Operations.xml", "Translations\\Operations.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (!CheckVersion || IsOnlineVersionGreater(TranslationType.Quests, TranslationsRef.QuestsVersion))
					{
						Client.DownloadFile(GetOnlineVersion(TranslationType.Quests, true), "Translations\\tmp\\Quests.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Quests.xml");
							if (File.Exists("Translations\\Quests.xml"))
								File.Delete("Translations\\Quests.xml");
							File.Move("Translations\\tmp\\Quests.xml", "Translations\\Quests.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (!CheckVersion || IsOnlineVersionGreater(TranslationType.Ships, TranslationsRef.ShipsVersion))
					{
						Client.DownloadFile(GetOnlineVersion(TranslationType.Ships, true), "Translations\\tmp\\Ships.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Ships.xml");
							if (File.Exists("Translations\\Ships.xml"))
								File.Delete("Translations\\Ships.xml");
							File.Move("Translations\\tmp\\Ships.xml", "Translations\\Ships.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (!CheckVersion || IsOnlineVersionGreater(TranslationType.ShipTypes, TranslationsRef.ShipTypesVersion))
					{
						Client.DownloadFile(GetOnlineVersion(TranslationType.ShipTypes, true), "Translations\\tmp\\ShipTypes.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\ShipTypes.xml");
							if (File.Exists("Translations\\ShipTypes.xml"))
								File.Delete("Translations\\ShipTypes.xml");
							File.Move("Translations\\tmp\\ShipTypes.xml", "Translations\\ShipTypes.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (!CheckVersion || IsOnlineVersionGreater(TranslationType.Expeditions, TranslationsRef.ExpeditionsVersion))
                    {
						Client.DownloadFile(GetOnlineVersion(TranslationType.Expeditions, true), "Translations\\tmp\\Expeditions.xml");

                        try
                        {
                            TestXML = XDocument.Load("Translations\\tmp\\Expeditions.xml");
                            if (File.Exists("Translations\\Expeditions.xml"))
                                File.Delete("Translations\\Expeditions.xml");
                            File.Move("Translations\\tmp\\Expeditions.xml", "Translations\\Expeditions.xml");
                            ReturnValue = 1;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            ReturnValue = -1;
                        }
                    }

				}
				catch (Exception ex)
				{
					// Failed to download files.
					Debug.WriteLine(ex);
					return -1;
				}

				if (Directory.Exists("Translations\\tmp\\")) Directory.Delete("Translations\\tmp\\");

				return ReturnValue;
			}
		}

		/// <summary>
		/// Uses the downloaded Version XML document to return a specific version number as a string.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="bGetURL">If true, returns the URL of the online file instead of the version.</param>
		/// <returns>String value of either the Version or URL to the file.</returns>
		public string GetOnlineVersion(TranslationType Type, bool bGetURL = false)
		{
			if (VersionXML == null)
				return "";

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName =  !bGetURL ? "Version" : "URL";

			try
			{
				switch (Type)
				{
					case TranslationType.App:
						return Versions.Where(x => x.Element("Name").Value.Equals("App")).FirstOrDefault().Element(ElementName).Value;
					case TranslationType.Equipment:
						return Versions.Where(x => x.Element("Name").Value.Equals("Equipment")).FirstOrDefault().Element(ElementName).Value;
					case TranslationType.Operations:
					case TranslationType.OperationSortie:
					case TranslationType.OperationMaps:
						return Versions.Where(x => x.Element("Name").Value.Equals("Operations")).FirstOrDefault().Element(ElementName).Value;
					case TranslationType.Quests:
					case TranslationType.QuestDetail:
					case TranslationType.QuestTitle:
						return Versions.Where(x => x.Element("Name").Value.Equals("Quests")).FirstOrDefault().Element(ElementName).Value;
					case TranslationType.Ships:
						return Versions.Where(x => x.Element("Name").Value.Equals("Ships")).FirstOrDefault().Element(ElementName).Value;
					case TranslationType.ShipTypes:
						return Versions.Where(x => x.Element("Name").Value.Equals("ShipTypes")).FirstOrDefault().Element(ElementName).Value;
					case TranslationType.Expeditions:
					case TranslationType.ExpeditionDetail:
					case TranslationType.ExpeditionTitle:
						return Versions.Where(x => x.Element("Name").Value.Equals("Expeditions")).FirstOrDefault().Element(ElementName).Value;

				}
			} catch
			{
				return "";
			}
			return "";
		}

		/// <summary>
		/// Conditional function to determine whether the supplied version is greater than the one found online.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="LocalVersionString">Version string of the local file to check against</param>
		/// <returns></returns>
		public bool IsOnlineVersionGreater(TranslationType Type, string LocalVersionString)
		{
			if (VersionXML == null)
				return false;

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = "Version";
			Version LocalVersion = new Version(LocalVersionString);

			try
			{
				switch (Type)
				{
					case TranslationType.App:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("App")).FirstOrDefault().Element(ElementName).Value)) < 0;
					case TranslationType.Equipment:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Equipment")).FirstOrDefault().Element(ElementName).Value)) < 0;
					case TranslationType.Operations:
					case TranslationType.OperationSortie:
					case TranslationType.OperationMaps:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Operations")).FirstOrDefault().Element(ElementName).Value)) < 0;
					case TranslationType.Quests:
					case TranslationType.QuestDetail:
					case TranslationType.QuestTitle:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Quests")).FirstOrDefault().Element(ElementName).Value)) < 0;
					case TranslationType.Ships:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Ships")).FirstOrDefault().Element(ElementName).Value)) < 0;
					case TranslationType.ShipTypes:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("ShipTypes")).FirstOrDefault().Element(ElementName).Value)) < 0;
					case TranslationType.Expeditions:
					case TranslationType.ExpeditionDetail:
					case TranslationType.ExpeditionTitle:
						return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Expeditions")).FirstOrDefault().Element(ElementName).Value)) < 0;
				}
			}
			catch
			{
				return false;
			}

			return false;
		}

	}
}
