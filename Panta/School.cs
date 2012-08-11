using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Panta
{
    /// <summary>
    /// A school that offers courses to students
    /// </summary>
    [Serializable]
    public class School
    {
        public string Name { get; set; }
        public string Abbr { get; set; }

        public Dictionary<string, Department> Departments { get; set; }
        public IFormatReader<Department> DepartmentReader { get; set; }

        // So courses in each department can be found by concatenating the department abbreviation
        public string CourseDescriptionRoot { get; set; }

        public School(string name, string abbr, IFormatReader<Department> reader)
        {
            this.Name = name;
            this.Abbr = abbr;
            this.DepartmentReader = reader;
            this.Departments = new Dictionary<string, Department>();
        }

        // Read thw webpage and store departments in dictionary
        public void FetchDepartments()
        {
            foreach (Department dep in DepartmentReader.Read())
            {
                this.Departments.Add(dep.Abbr, dep);
                dep.FetchCourses();
                dep.FetchCoursesDetail();
            }
        }

        // Read a school from a serialized binary file
        public static School Read(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            School obj = (School)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        // Save the instance to a serialized binary file
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(this.Abbr + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }
    }
}
