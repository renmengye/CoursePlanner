using Panta.Fetchers.Extensions.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panta.Indexing.Extensions.UT
{
    public class UTEngHssCsChecker
    {
        private HashSet<string> HssCourses { get; set; }
        private List<string> CsCourses { get; set; }
        private List<string> CsExclusions { get; set; }
        private HashSet<string> EngCsCourses { get; set; }

        public UTEngHssCsChecker()
        {
            UTEngHssFetcher hssFetcher = new UTEngHssFetcher("http://www.undergrad.engineering.utoronto.ca/Office_of_the_Registrar/Electives/HSS_Electives.htm");
            this.HssCourses = new HashSet<string>();
            this.HssCourses.UnionWith(hssFetcher.FetchItems());

            this.EngCsCourses = new HashSet<string>();
            this.EngCsCourses.UnionWith(new string[]{
                "APS234",
                "ASP305",
                "ASP310",
                "ASP432",
                "ASP442",
                "APS443",
                "APS510",
                "APS520",
                "CHE488",
                "CIV488",
                "ECE488",
                "MIE488",
                "MSE488",
                "JRE300",
                "JRE410",
                "JRE420",
            });

            this.CsCourses = new List<string>();
            this.CsCourses.AddRange(new string[]{
                "ABS",
                "ANT",
                "ARC",
                "COG250Y1",
                "CLA",
                "CSC300",
                "DRM",
                "EAS",
                "ECO",
                "ECMA",
                "ENG",
                "ENV100",
                "ENV307",
                "ENV320",
                "ENV335",
                "ENV350",
                "EST",
                "FAH",
                "FIN",
                "FRE",
                "FSL",
                "FOR302",
                "FOR303",
                "FOR421",
                "GER",
                "GGR220",
                "GGR222",
                "GGR252",
                "GGR271",
                "GGR314",
                "GGR323",
                "GGR329",
                "GGR332",
                "GGR335",
                "GGR343",
                "GGR356",
                "GGR424",
                "GRK",
                "HIS",
                "HPS",
                "HUM",
                "HUN",
                "IAS",
                "INI",
                "ITA",
                "JAL",
                "JEF",
                "JHP",
                "JLP",
                "JMC",
                "JPE",
                "JPJ",
                "JPP",
                "JUP",
                "JGI346",
                "LAT",
                "LIN",
                "MEI",
                "MGT",
                "MGM",
                "MUS",
                "NEW",
                "NMC",
                "NML",
                "PHL",
                "POL",
                "PRT",
                "RLG",
                "RSM295Y",
                "SAS212Y",
                "SLA",
                "SMC",
                "SOC",
                "SPA",
                "TRN",
                "USA",
                "VIC",
                "WDW"
            });

            this.CsExclusions = new List<string>();
            this.CsExclusions.AddRange(new string[]{
                "INI341H",
                "PHL245H",
                "PHL246H",
                "PHL342H",
                "PHL345H"
            });
        }

        /// <summary>
        /// Check the course code match in the HSS hash set (format: AAA000H0)
        /// </summary>
        /// <param name="courseCode"></param>
        /// <returns></returns>
        public bool CheckHss(string courseCode)
        {
            return this.HssCourses.Contains(courseCode);
        }

        /// <summary>
        /// Check the course code match in the CS approved list (format: AAA00H0)
        /// </summary>
        /// <param name="courseCode"></param>
        /// <returns></returns>
        public bool CheckArtsciCs(string courseCode)
        {
            foreach (string exclusion in this.CsExclusions)
            {
                if (courseCode.StartsWith(exclusion))
                {
                    return false;
                }
            }
            foreach (string approved in this.CsCourses)
            {
                if (courseCode.StartsWith(approved))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check the course code match in Engineering CS hashset (format: AAA000)
        /// </summary>
        /// <param name="courseCode"></param>
        /// <returns></returns>
        public bool CheckEngCs(string courseCode)
        {
            return this.EngCsCourses.Contains(courseCode);
        }
    }
}
