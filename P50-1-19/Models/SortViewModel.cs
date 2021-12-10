using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P50_1_19.Models
{
    public class SortViewModel
    {
        public SortState IdSort { get; private set; }
        public SortState LoginSort { get; private set; }
        public SortState Current { get; private set; }
        public SortViewModel (SortState sortOrder)
        {
            IdSort = sortOrder == SortState.IdAsc ? SortState.IdDesc : SortState.IdAsc;
            LoginSort = sortOrder == SortState.LoginAsc ? SortState.LoginDesc : SortState.LoginAsc;
            Current = sortOrder;
        }
    }
}
