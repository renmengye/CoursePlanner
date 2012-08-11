using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Panta
{
    public class Program
    {
        public const string UTSavePath = "uoft.bin";

        static void Main(string[] args)
        {
            Program p = new Program();
        }

        public Program()
        {
            School UOfT;
            if (File.Exists(UTSavePath))
            {
                UOfT = School.Read(UTSavePath);
            }
            else
            {
                UOfT = new School("University of Toronto", "uoft", new UTDepartmentReader());
                UOfT.FetchDepartments();
                UOfT.Save();
            }
        }
    }
}
