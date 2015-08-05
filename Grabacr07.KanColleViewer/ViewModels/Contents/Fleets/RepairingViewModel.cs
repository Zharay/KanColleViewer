using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
    public class RepairingViewModel : ViewModel
    {
        private readonly FleetDock source;
        private PropertyChangedEventListener listener;

        public string CompleteTime
        {
            get
            {
                return this.source.RepairingDock != null && this.source.RepairingDock.CompleteTime.HasValue
                    ? this.source.RepairingDock.CompleteTime.Value.LocalDateTime.ToString("MM/dd HH:mm")
                    : "--/-- --:--";
            }
        }

        public string Remaining
        {
            get
            {
                return this.source.RepairingDock != null && this.source.RepairingDock.Remaining.HasValue
                    ? string.Format("{0:D2}:{1}",
                        (int)this.source.RepairingDock.Remaining.Value.TotalHours,
                        this.source.RepairingDock.Remaining.Value.ToString(@"mm\:ss"))
                    : "--:--:--";
            }
        }

        public bool IsRepairing
        {
            get { return this.source.IsRepairing; }
        }


        public RepairingViewModel(FleetDock dock)
        {
            this.source = dock;
            if (dock.RepairingDock != null)
            {
                this.listener = new PropertyChangedEventListener(dock.RepairingDock, (sender2, args2) => this.RaisePropertyChanged(args2.PropertyName));
            }
            this.CompositeDisposable.Add(new PropertyChangedEventListener(dock,
                (sender, args) =>
                {
                    if (args.PropertyName == "RepairingDock")
                    {
                        if (this.listener != null) this.listener.Dispose();
                        if (dock.RepairingDock != null)
                        {
                            this.listener = new PropertyChangedEventListener(dock.RepairingDock, (sender2, args2) => this.RaisePropertyChanged(args2.PropertyName));
                        }
                        else this.listener = null;
                        this.RaisePropertyChanged("CompleteTime");
                        this.RaisePropertyChanged("Remaining");
                    }
                    else if (args.PropertyName == "IsRepairing")
                        this.RaisePropertyChanged("IsRepairing");
                }));
        }
    }
}
