using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class RankingsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return Properties.Resources.Rankings; }
            protected set { throw new NotImplementedException(); }
        }

        #region Rankings

        private RankingViewModel[] _Rankings;

        public RankingViewModel[] Rankings
        {
            get { return this._Rankings; }
            set
            {
                if (this._Rankings != value)
                {
                    this._Rankings = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region TotalRanked

        private int _TotalRanked;

        public int TotalRanked
        {
            get { return this._TotalRanked; }
            set
            {
                if (this._TotalRanked != value)
                {
                    this._TotalRanked = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region TotalPages

        private int _TotalPages;

        public int TotalPages
        {
            get { return this._TotalPages; }
            set
            {
                if (this._TotalPages != value)
                {
                    this._TotalPages = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentPage

        private int _CurrentPage;

        public int CurrentPage
        {
            get { return this._CurrentPage; }
            set
            {
                if (this._CurrentPage != value)
                {
                    this._CurrentPage = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        private void Update()
        {
            this.Rankings = KanColleClient.Current.Homeport.Rankings.Current.Select(x => new RankingViewModel(x)).ToArray();
            this.TotalRanked = KanColleClient.Current.Homeport.Rankings.TotalRanked;
            this.TotalPages = KanColleClient.Current.Homeport.Rankings.TotalPages;
            this.CurrentPage = KanColleClient.Current.Homeport.Rankings.CurrentPage;
        }

        public RankingsViewModel()
        {
            this.Name = Properties.Resources.Rankings;
            this.Rankings = KanColleClient.Current.Homeport.Rankings.Current.Select(x => new RankingViewModel(x)).ToArray();
            this.TotalRanked = KanColleClient.Current.Homeport.Rankings.TotalRanked;
            this.TotalPages = KanColleClient.Current.Homeport.Rankings.TotalPages;
            this.CurrentPage = KanColleClient.Current.Homeport.Rankings.CurrentPage;

            this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Rankings)
            {
                {
                    () => KanColleClient.Current.Homeport.Rankings.Current,
                    (sender, args) => Update()
                },
            });
        }
    }
}
