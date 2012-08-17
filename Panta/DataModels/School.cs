using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Panta.Formatters;
using Panta.Indexing;

namespace Panta.DataModels
{
    /// <summary>
    /// A school that offers courses to students
    /// </summary>
    [Serializable]
    public class School
    {
        public string Name { get; set; }
        public string Abbr { get; set; }

        /// <summary>
        /// Signs a unique id to every course and store it
        /// </summary>
        public IdSigner Signer { get; set; }

        /// <summary>
        /// All the departments fetched from department reader.
        /// Has method to fetch courses individually.
        /// Not serialized because we don't need the department information any more.
        /// </summary>
        [NonSerialized]
        public Dictionary<string, Department> Departments;

        /// <summary>
        /// Reads and returns all the department from a webpage.
        /// Not serilized because we don't need the reader. 
        /// </summary>
        [NonSerialized]
        public IWebpageFormatter<Department> DepartmentFormatter;

        /// <summary>
        /// Final storage of the couses once got from the department
        /// </summary>
        public Dictionary<uint, Course> Courses { get; set; }

        public School(string name, string abbr, IWebpageFormatter<Department> formatter)
        {
            this.Name = name;
            this.Abbr = abbr;
            this.DepartmentFormatter = formatter;
            this.Signer = new IdSigner();
            this.Departments = new Dictionary<string, Department>();
            this.Courses = new Dictionary<uint, Course>();
        }

        // Read thw webpage and store departments in dictionary
        public virtual void FetchDepartments()
        {
            foreach (Department dep in DepartmentFormatter.Read())
            {
                this.Departments.Add(dep.Abbr, dep);
                dep.FetchCourses();
                foreach (Course course in dep.Courses.Values)
                {
                    // Generate course universal id as the key (not the course name any more)
                    this.Courses.Add((course.ID = Signer.SignId()), course);
                }
            }
        }

        #region Read/Save
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
        #endregion
    }
}
