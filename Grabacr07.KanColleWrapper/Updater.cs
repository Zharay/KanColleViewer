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
			try
			{
				VersionXML = XDocument.Load(UpdateURL);

				if (VersionXML == null)
					return false;
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
		public int UpdateTranslations(string BaseTranslationURL, string Culture, Translations TranslationsRef)
		{
			using (WebClient Client = new WebClient())
			{
				string CurrentCulture = (Culture == null || Culture == "en-US" || Culture == "ja-JP" || Culture == "en") ? "" : Culture;
				string CurrentCultureDir = (CurrentCulture != "" ? CurrentCulture + "\\" : "");
                string TranslationURL = (BaseTranslationURL.TrimEnd(new[] { '/' }) + "/" + CurrentCulture).TrimEnd(new[] { '/' }) + "/";
				XDocument TestXML;
				int ReturnValue = 0;

				try
				{
					if (!Directory.Exists("Translations")) Directory.CreateDirectory("Translations");
					if (!Directory.Exists("Translations\\" + CurrentCultureDir)) Directory.CreateDirectory("Translations\\" + CurrentCultureDir);
					if (!Directory.Exists("Translations\\tmp\\")) Directory.CreateDirectory("Translations\\tmp\\");

					// In every one of these we download it to a temp folder, check if the file works, then move it over.
					if (IsOnlineVersionGreater(TranslationType.Equipment, TranslationsRef.EquipmentVersion))
					{
						Client.DownloadFile(TranslationURL + "Equipment.xml", "Translations\\tmp\\Equipment.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Equipment.xml");
							if (File.Exists("Translations\\" + CurrentCultureDir + "Equipment.xml")) 
								File.Delete("Translations\\" + CurrentCultureDir + "Equipment.xml");
							File.Move("Translations\\tmp\\Equipment.xml", "Translations\\" + CurrentCultureDir + "Equipment.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.Operations, TranslationsRef.OperationsVersion))
					{
						Client.DownloadFile(TranslationURL + "Operations.xml", "Translations\\tmp\\Operations.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Operations.xml");
							if (File.Exists("Translations\\" + CurrentCultureDir + "Operations.xml"))
								File.Delete("Translations\\" + CurrentCultureDir + "Operations.xml");
							File.Move("Translations\\tmp\\Operations.xml", "Translations\\" + CurrentCultureDir + "Operations.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.Quests, TranslationsRef.QuestsVersion))
					{
						Client.DownloadFile(TranslationURL + "Quests.xml", "Translations\\tmp\\Quests.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Quests.xml");
							if (File.Exists("Translations\\" + CurrentCultureDir + "Quests.xml"))
								File.Delete("Translations\\" + CurrentCultureDir + "Quests.xml");
							File.Move("Translations\\tmp\\Quests.xml", "Translations\\" + CurrentCultureDir + "Quests.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.Ships, TranslationsRef.ShipsVersion))
					{
						Client.DownloadFile(TranslationURL + "Ships.xml", "Translations\\tmp\\Ships.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\Ships.xml");
							if (File.Exists("Translations\\" + CurrentCultureDir + "Ships.xml"))
								File.Delete("Translations\\" + CurrentCultureDir + "Ships.xml");
							File.Move("Translations\\tmp\\Ships.xml", "Translations\\" + CurrentCultureDir + "Ships.xml");
							ReturnValue = 1;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.ShipTypes, TranslationsRef.ShipTypesVersion))
					{
						Client.DownloadFile(TranslationURL + "ShipTypes.xml", "Translations\\tmp\\ShipTypes.xml");

						try
						{
							TestXML = XDocument.Load("Translations\\tmp\\ShipTypes.xml");
							if (File.Exists("Translations\\" + CurrentCultureDir + "ShipTypes.xml"))
								File.Delete("Translations\\" + CurrentCultureDir + "ShipTypes.xml");
							File.Move("Translations\\tmp\\ShipTypes.xml", "Translations\\" + CurrentCultureDir + "ShipTypes.xml");
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
				return true;

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = "Version";
			Version LocalVersion = new Version(LocalVersionString);

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

			}

			return false;
		}

	}
}
