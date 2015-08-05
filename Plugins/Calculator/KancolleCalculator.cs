using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Plugins.ViewModels;
using Grabacr07.KanColleViewer.Plugins.Views;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "Calculator")]
    [ExportMetadata("Description", "Calculator experience")]
	[ExportMetadata("Version", "1.1")]
    [ExportMetadata("Author", "@Zharay")]
	public class KancolleCalculator : IToolPlugin
	{
        private CalculatorViewModel calculatorViewModel;

		public string ToolName
		{
			get { return "Calculator"; }
		}


		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
            if (this.calculatorViewModel == null)
            {
                if (KanColleClient.Current.Homeport != null) this.calculatorViewModel = new CalculatorViewModel();
            }
            return new Calculator { DataContext = this.calculatorViewModel };
		}
	}
}
