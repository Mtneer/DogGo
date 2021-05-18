using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Walks
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Owner Client { get; set; }
        public int Duration { get; set; }
    }
}
