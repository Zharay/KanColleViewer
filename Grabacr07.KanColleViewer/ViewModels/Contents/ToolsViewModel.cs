using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents.Tools;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class ToolsViewModel : TabItemViewModel
    {
        public CalculatorViewModel Calculator { get; private set; }
        public RankingsViewModel Rankings { get; private set; }

        public IList<TabItemViewModel> TabItems { get; private set; }

        #region SelectedItem 変更通知プロパティ

        private TabItemViewModel _SelectedItem;

        public TabItemViewModel SelectedItem
        {
            get { return this._SelectedItem; }
            set
            {
                if (this._SelectedItem != value)
                {
                    this._SelectedItem = value;
                    this.RaisePropertyChanged();

                    App.ViewModelRoot.StatusBar = value;
                }
            }
        }

        #endregion

        public override string Name
        {
            get { return "Tools"; }
            protected set { throw new NotImplementedException(); }
        }

        public override string Icon
        {
            get { return "📝"; }
            protected set { throw new NotImplementedException(); }
        }

        public ToolsViewModel()
        {
            this.Calculator = new CalculatorViewModel();
            this.Rankings = new RankingsViewModel();

            this.TabItems = new List<TabItemViewModel> 
            {
                this.Calculator,
                this.Rankings,
            };

            this.SelectedItem = this.TabItems.FirstOrDefault();
        }

    }
}
