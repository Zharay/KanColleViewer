using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;

namespace ExperienceCalculator
{
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "Experience Calculator")]
	[ExportMetadata("Description", "Experience Calculator for KanColleViewer!")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Yuubari")]
	public class ExperienceCalculator : IToolPlugin
	{
		private readonly CalculatorViewModel viewmodel = new CalculatorViewModel
		{
		};

		public string ToolName
		{
			get { return "ExperienceCalculator"; }
		}

		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
			return new CalculatorView { DataContext = this.viewmodel, };
		}
	}
}
