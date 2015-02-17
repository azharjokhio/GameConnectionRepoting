using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameConnectionReporting
{
    using System.Collections.Generic;

    using OxyPlot;

    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Points = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };

            this.Points2 = new List<DataPoint>
                              {
                                  new DataPoint(0, 7),
                                  new DataPoint(10, 3),
                                  new DataPoint(20, 17),
                                  new DataPoint(30, 13),
                                  new DataPoint(40, 1),
                                  new DataPoint(50, 15)
                              };
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }

        public IList<DataPoint> Points2 { get; private set; }
    }
}
