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
    public interface IShape
    {
        CanvasObject obj { get; set; }
        //ObservableCollection<Tag> Tags { get; set; }
    }
}
