using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P50_1_19.Models
{
    public class FilterViewModel
    {
        public int? SelectId { get; private set; }
        public string SelectLogin { get; private set; }

        public FilterViewModel (int? id, string login)
        {
            SelectId = id;
            SelectLogin = login;
        }
    }
}
