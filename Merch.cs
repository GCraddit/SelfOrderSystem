using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giles_Chen_test_1
{
    public class Merch
    {
        private int merchID;
        private string merchImagePath;
        private string merchName;
        private string merchDescription;
        private int merchPoints;

        // Property for merchID
        public int MerchID
        {
            get { return merchID; }
            set { merchID = value; }
        }

        // Property for merchImagePath
        public string MerchImagePath
        {
            get { return merchImagePath; }
            set { merchImagePath = value; }
        }

        // Property for merchName
        public string MerchName
        {
            get { return merchName; }
            set { merchName = value; }
        }

        // Property for merchDescription
        public string MerchDescription
        {
            get { return merchDescription; }
            set { merchDescription = value; }
        }

        // Property for merchPoints
        public int MerchPoints
        {
            get { return merchPoints; }
            set { merchPoints = value; }
        }
    }
}
