using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var student = db.Students.FirstOrDefault(s => s.Uid == uid);
            if (student == null)
            {
                return Json(new List<object>());
            }

            var enrollments = db.Enrollments
                .Where(e => e.StudentId == uid)
                .Select(e => new
                {
                    subject = e.Class.Course.Department,
                    number = e.Class.Course.Number,
                    name = e.Class.Course.Name,
                    season = e.Class.Season,
                    year = e.Class.Year,
                    grade = e.Grade ?? "--"
                }).ToList();

            return Json(enrollments);
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var student = db.Students.FirstOrDefault(s => s.Uid == uid);
            if (student == null)
            {
                return Json(new List<object>());
            }

            Debug.WriteLine($"Student with Uid '{uid}' exists.");

            /// Step 1: Get the class that the student is enrolled in based on the provided parameters
            var enrolledClass = db.Classes.FirstOrDefault(c =>
                c.Course.Department == subject &&
                c.Course.Number == num &&
                c.Season == season &&
                c.Year == year &&
                c.Enrollments.Any(e => e.StudentId == uid));

            if (enrolledClass == null)
            {
                Debug.WriteLine($"No class found for the student with Uid '{uid}' and provided course parameters.");
                return Json(new List<object>());
            }

            Debug.WriteLine($"Enrolled class found: ClassId: {enrolledClass.Id}, CourseId: {enrolledClass.CourseId}, ProfessorId: {enrolledClass.ProfessorId}, Year: {enrolledClass.Year}, Season: {enrolledClass.Season}, StartTime: {enrolledClass.StartTime}, EndTime: {enrolledClass.EndTime}, Location: {enrolledClass.Location}");


            // Step 2: Get all assignments for the class
            var assignments = db.Assignments
                .Where(a => a.Category.ClassId == enrolledClass.Id)
                .Select(a => new
                {
                    aname = a.Name,
                    cname = a.Category.Name,
                    due = a.Due,
                    score = (ushort?)null // Placeholder for the score, will be updated later
                })
                .ToList();

            Debug.WriteLine($"Total assignments found for the class with ClassId '{enrolledClass.Id}': {assignments.Count}");

            return Json(assignments);
        }


        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
            string category, string asgname, string uid, string contents)
        {
            // Find the student
            var student = db.Students.FirstOrDefault(s => s.Uid == uid);
            if (student == null)
            {
                return Json(new { success = false });
            }

            // Find the assignment category
            var assignmentCategory = db.AssignmentCategories.FirstOrDefault(c =>
                c.Class.Course.Department == subject &&
                c.Class.Course.Number == num &&
                c.Class.Season == season &&
                c.Class.Year == year &&
                c.Name == category);

            if (assignmentCategory == null)
            {
                return Json(new { success = false });
            }

            // Find the assignment within the category
            var assignment = db.Assignments.FirstOrDefault(a =>
                a.CategoryId == assignmentCategory.Id &&
                a.Name == asgname);

            if (assignment == null)
            {
                return Json(new { success = false });
            }

            // Find the submission for the student and assignment
            var submission = db.Submissions.FirstOrDefault(s =>
                s.StudentId == uid &&
                s.AssignmentId == assignment.Id);

            // If a submission already exists, update its contents and time
            if (submission != null)
            {
                submission.Contents = contents;
                submission.Time = DateTime.Now;
            }
            else
            {
                // Create a new submission
                submission = new Submission
                {
                    StudentId = uid,
                    AssignmentId = assignment.Id,
                    Time = DateTime.Now,
                    Contents = contents,
                    Score = null
                };
                db.Submissions.Add(submission);
            }

            db.SaveChanges();

            return Json(new { success = true });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            // Find the student
            var student = db.Students.FirstOrDefault(s => s.Uid == uid);
            if (student == null)
            {
                return Json(new { success = false });
            }

            // Find the class
            var course = db.Courses.FirstOrDefault(c => c.Department == subject && c.Number == num);
            if (course == null)
            {
                return Json(new { success = false });
            }

            var classObj = db.Classes.FirstOrDefault(c =>
                c.CourseId == course.Id &&
                c.Season == season &&
                c.Year == year);

            if (classObj == null)
            {
                return Json(new { success = false });
            }

            // Check if the student is already enrolled in the class
            var existingEnrollment = db.Enrollments.FirstOrDefault(e =>
                e.StudentId == uid &&
                e.ClassId == classObj.Id);

            if (existingEnrollment != null)
            {
                return Json(new { success = false });
            }

            // Check if there are any assignments for the enrolled class
            bool hasAssignments = db.Assignments.Any(a => a.Category.ClassId == classObj.Id);

            // Set the grade based on the existence of assignments
            string? grade = hasAssignments ? "E" : null;

            // Create a new enrollment
            var enrollment = new Enrollment
            {
                StudentId = uid,
                ClassId = classObj.Id,
                Grade = grade
            };

            db.Enrollments.Add(enrollment);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            // Find the student
            var student = db.Students.FirstOrDefault(s => s.Uid == uid);
            if (student == null)
            {
                return Json(new { gpa = 0.0 });
            }

            // Find the enrollments for the student
            var enrollments = db.Enrollments.Where(e => e.StudentId == uid).ToList();
            if (enrollments.Count == 0)
            {
                return Json(new { gpa = 0.0 });
            }

            double totalPoints = 0.0;
            int totalCredits = 0;

            // Calculate the total points and credits for the GPA
            foreach (var enrollment in enrollments)
            {
                if (enrollment.Grade != null && enrollment.Grade != "--")
                {
                    double points = Helper.GetGradePoint(enrollment.Grade);
                    Console.WriteLine($"Points for grade '{enrollment.Grade}': {points}");
                    totalPoints += points * 4.0; // Assuming all classes are 4 credit hours
                    totalCredits += 4;
                }
            }

            Console.WriteLine($"Total Points: {totalPoints}, Total Credits: {totalCredits}");

            // Calculate the GPA
            double gpa = totalCredits > 0 ? totalPoints / totalCredits : 0.0;

            Console.WriteLine($"GPA calculated: {gpa}");

            return Json(new { gpa });
        }



    }
}

