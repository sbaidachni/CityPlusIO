using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminCrossPlatformClient.Models
{
    class Resources
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public double Lon { get; set; }

        public double Lat { get; set; }

        public int Food { get; set; }

        public int Shelter { get; set; }

        public int Clothes { get; set; }

        public int Medicine { get; set; }
    }
}
