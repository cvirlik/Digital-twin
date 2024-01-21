using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Digital_twin.Dataset.Types.Primary;
using Digital_twin.Dataset.Types.Secondary;
using Digital_twin.Draw_tools;
using OsmSharp;

namespace Digital_twin.Dataset.Types.Canvas
{
    public abstract class CanvasObject
    {
        public ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();
        protected ObservableCollection<IShape> shapes = new ObservableCollection<IShape>();
        public ObservableCollection<IShape> Shapes
        {
            get
            {
                return shapes;
            }
        }
    }
}
