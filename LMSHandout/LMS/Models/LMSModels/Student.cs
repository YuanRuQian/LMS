using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrollments = new HashSet<Enrollment>();
            Submissions = new HashSet<Submission>();
        }

        public string Uid { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Department { get; set; } = null!;

        public virtual Department DepartmentNavigation { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
