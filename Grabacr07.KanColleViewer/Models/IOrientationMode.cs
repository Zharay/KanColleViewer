using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Models
{
	public interface IOrientationMode
	{
		Orientation Mode { get; }

		OrientationType[] SupportedModes { get; }

		OrientationType CurrentMode { get; set; }
	}
}
