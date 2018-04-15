using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI.Model
{
    public class Currency
    {
        public string FullName { get; set; }
        public string Abbreviation { get; set; }

        public Currency()
        {

        }

        public Currency(string fullName, string abbreviation)
        {
            FullName = fullName;
            Abbreviation = abbreviation;
        }
    }
}
