using System.IO;
using System.Linq;
using Panta.DataModels;
using Panta.Formatters;
using Panta.Indexing;
using System.Collections.Generic;

namespace Panta
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            School UOfT;
            UOfT = new School("University of Toronto", "uoft", new UTDepartmentFormatter());
            UOfT.FetchDepartments();
            UOfT.Save();
        }
    }
}
