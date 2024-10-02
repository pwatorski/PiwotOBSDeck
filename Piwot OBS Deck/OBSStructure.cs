using PiwotOBS;
using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBSDeck
{
    public static class OBSStructure
    {
        public static Scene? RootScene { get; private set; }
        public static void Init()
        {
            RootScene = OBSDeck.GetScene("SAFARI");
        }
    }
}
