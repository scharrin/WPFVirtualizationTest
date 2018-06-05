using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace VirtualizationTest
{
    public class ParticalCrewListMaker : IListMaker<Crew>
    {
        private readonly int _count;
        private readonly int _sleepTermInMS;

        public ParticalCrewListMaker(int _count, int _sleepTermInMS)
        {
            // TODO: Complete member initialization
            this._count = _count;
            this._sleepTermInMS = _sleepTermInMS;
        }

        public int getAvailableRowsCount()
        {
            Trace.WriteLine("getAvailableRowsCount");
            Thread.Sleep(_sleepTermInMS);
            return _count;
        }

        public IList<Crew> getAvailableRows(int idxFrom, int count)
        {
            Trace.WriteLine("getAvailableRows from " + idxFrom + "and " + count + "rows");
            Thread.Sleep(_sleepTermInMS);

            List<Crew> _tmpLst = new List<Crew>();
            for ( int i = idxFrom; i < idxFrom + count; i++ )
            {
                //// crew for test
                _tmpLst.Add( new Crew
                {
                    id = (i + 1).ToString()
                    , name = "crew " + (i + 1)
                    , department = "dep is " + (i + 1)
                    , jobTitle = "job title is " + (i + 1)
                    , latitude = i + 2
                    , longitude = i + 3
                    , major = i + 4
                    , minor = i + 5
                    , status = i + 6
                });
            }
            return _tmpLst;
        }
    }
}
