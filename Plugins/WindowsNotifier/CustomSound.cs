using Grabacr07.KanColleViewer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using NAudio.Wave;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using StatusService = Grabacr07.KanColleViewer.Models.StatusService;
using Volume = Grabacr07.KanColleViewer.Models.Volume;

namespace Grabacr07.KanColleViewer.Plugins
{
	class CustomSound
	{
		private BlockAlignReductionStream BlockStream = null;
		private DirectSoundOut SoundOut = null;
		string Main_folder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		public void SoundOutput(string header, bool IsWin8)
		{
			/**
			 * 
			 * 출력할 소리가 wav인지 mp3인지 비프음인지 채크합니다.
			 * windows8 이상의 경우에는 비프음보다 윈도우8 기본 알림음이 더 알맞다고 생각하기에 IsWin8이 True면 아무 소리도 내보내지 않습니다.
			 * 
			**/

			DisposeWave();//알림이 동시에 여러개가 울릴 경우 소리가 겹치는 문제를 방지

			try
			{

				var Audiofile = GetRandomSound(header);

				if (!IsWin8 && Audiofile == null)
				{
					SystemSounds.Beep.Play();		//FileCheck에서 음소거 채크를 하고 음소거상태이거나 파일이 없는경우 비프음 출력
					return;
				}
				else if (IsWin8 && Audiofile == null)
					return;							//위와 동일한 조건이지만 윈도우8인경우는 이 소스에서는 아무 소리도 내보내지않음.

				float Volume = Settings.Current.CustomSoundVolume > 0 ? (float)Settings.Current.CustomSoundVolume / 100 : 0;

				if (Path.GetExtension(Audiofile).ToLower() == ".wav")
				{
					WaveStream pcm = new WaveChannel32(new WaveFileReader(Audiofile), Volume, 0);
					BlockStream = new BlockAlignReductionStream(pcm);
				}
				else if (Path.GetExtension(Audiofile).ToLower() == ".mp3")
				{
					WaveStream pcm = new WaveChannel32(new Mp3FileReader(Audiofile), Volume, 0);
					BlockStream = new BlockAlignReductionStream(pcm);
				}
				else
					return;

				SoundOut = new DirectSoundOut();
				SoundOut.Init(BlockStream);
				SoundOut.Play();
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify("Unable to play sound notification: " + ex.Message);
			}
		}


		public string GetRandomSound(string header)
		{
			try
			{
				if (!Directory.Exists("Sounds\\"))
				{
					Directory.CreateDirectory("Sounds");
				}

				if (!Directory.Exists("Sounds\\" + header))
				{
					Directory.CreateDirectory("Sounds\\" + header);
					return null;
				}

				List<string> FileList = Directory.GetFiles("Sounds\\" + header, "*.wav", SearchOption.AllDirectories)
					.Concat(Directory.GetFiles("Sounds\\" + header, "*.mp3", SearchOption.AllDirectories)).ToList();

				if (FileList.Count > 0)
				{
					Random Rnd = new Random();
					return FileList[Rnd.Next(0, FileList.Count)];
				}
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify("Failed to find sound file: " + ex.Message);
			}

			return null;
		}

		public string FileCheck(string header)
		{
			/**
			 * 
			 * 이 코드 안에서 팝업 타이틀 검증과 음소거에 대한 채크를 모두 수행합니다.
			 * 타이틀이 업데이트인경우, 파일이 존재하지 않는경우, 칸코레 뷰어가 음소거인 경우는 null을 return합니다
			 * 
			 * 그 외의 경우는 exe파일이 존재하는 기본 루트 폴더의 경로를 반환합니다.
			 * 파일은 MP3파일이 우선권을 가지며 그 다음으로 WAV파일, 해당 경로에 파일이 없는경우에는 루트 폴더에서 알림음을 찾습니다
			 * mp3인지 wav인지는 SoundOutput에서 찾습니다. string의 형태이고 파일명이 정해져있으므로 파일명을 기준으로 구별합니다.
			 * 
			**/
			string SelFolder = "";

			if (header == Resources.Updater_Notification_Title)
				return null;

			var checkV = Volume.GetInstance();

			if (header == Resources.Expedition_NotificationMessage_Title) SelFolder = "\\expedition";
			else if (header == Resources.Repairyard_NotificationMessage_Title) SelFolder = "\\repair";
			else if (header == Resources.ReSortie_NotificationMessage_Title) SelFolder = "\\resortie";
			else if (header == Resources.ReSortie_CriticalConditionMessage_Title) SelFolder = "\\Critical";
			else SelFolder = "";

			string MP3path = Main_folder + SelFolder + "\\notify.mp3";
			string Wavpath = Main_folder + SelFolder + "\\notify.wav";

			//toast선에서 xml변경으로 해볼라그랬는데 생각처럼 잘 안되서 파기. 생각해보면 mp3지원 할거같지도 않고...
			if (checkV.IsMute == false)
			{
				if (File.Exists(MP3path) == true) 
					return MP3path;								//mp3path에 파일이 있으면 mp3경로를 리턴
				else if (File.Exists(Wavpath) == true) 
					return Wavpath;
				else
				{
					if (File.Exists(Main_folder + "\\notify.mp3") == true) 
						return Main_folder + "\\notify.mp3";
					else if (File.Exists(Main_folder + "\\notify.wav") == true) 
						return Main_folder + "\\notify.wav";
					else 
						return null;	//파일이 없는 경우 null
				}
			}

			return null;	//음소거인경우 null
		}

		private void DisposeWave()
		{
			try
			{
				if (SoundOut != null)
				{
					if (SoundOut.PlaybackState == PlaybackState.Playing) 
						SoundOut.Stop();
					SoundOut.Dispose();
					SoundOut = null;
				}
				if (BlockStream != null)
				{
					BlockStream.Dispose();
					BlockStream = null;
				}
			}
			catch { }
		}

	}
}