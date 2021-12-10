using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P50_1_19.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string author { get; set; }
    }
}
