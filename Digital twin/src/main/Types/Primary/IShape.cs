using Digital_twin.Dataset.Types.Canvas;
using Digital_twin.Dataset.Types.Secondary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Primary
{
    public abstract class IShape : ViewModelBase
    {
        public CanvasObject obj { get; set; }
        private bool _IsObjectSelected = false;

        public bool IsObjectSelected
        {
            get { return _IsObjectSelected; }
            set
            {
                _IsObjectSelected = value;
                OnPropertyChanged(nameof(IsObjectSelected));
            }
        }
        //ObservableCollection<Tag> Tags { get; set; }
    }
}
