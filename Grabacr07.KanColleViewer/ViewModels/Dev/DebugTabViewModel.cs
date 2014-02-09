using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Dev
{
	public class DebugTabViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return "Debug"; }
			protected set { throw new NotImplementedException(); }
		}
		
		public void Notify()
		{
			WindowsNotification.Notifier.Show("Test", "This is a test notification.", () => App.ViewModelRoot.Activate());
		}

	}
}
