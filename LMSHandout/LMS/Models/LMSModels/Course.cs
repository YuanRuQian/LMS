﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public uint Id { get; set; }
        public string Department { get; set; } = null!;
        public ushort Number { get; set; }
        public string Name { get; set; } = null!;

        public virtual Department DepartmentNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
