using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P50_1_19.Models
{
    public class IndexViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }

}
}
