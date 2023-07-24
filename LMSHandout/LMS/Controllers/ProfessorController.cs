using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var students = from e in db.Enrollments
                           where e.Class.Course.Department == subject &&
                                 e.Class.Course.Number == num &&
                                 e.Class.Season == season &&
                                 e.Class.Year == year
                           select new
                           {
                               fname = e.Student.FirstName,
                               lname = e.Student.LastName,
                               uid = e.Student.Uid,
                               dob = e.Student.DateOfBirth,
                               grade = e.Grade
                           };

            return Json(students);
        }

        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            var assignments = from a in db.Assignments
                              where a.Category.Class.Course.Department == subject &&
                                    a.Category.Class.Course.Number == num &&
                                    a.Category.Class.Season == season &&
                                    a.Category.Class.Year == year &&
                                    (category == null || a.Category.Name == category)
                              select new
                              {
                                  aname = a.Name,
                                  cname = a.Category.Name,
                                  due = a.Due,
                                  submissions = a.Submissions.Count()
                              };

            return Json(assignments);
        }

        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var categories = from ac in db.AssignmentCategories
                             where ac.Class.Course.Department == subject &&
                                   ac.Class.Course.Number == num &&
                                   ac.Class.Season == season &&
                                   ac.Class.Year == year
                             select new
                             {
                                 name = ac.Name,
                                 weight = ac.Weight
                             };

            return Json(categories);
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var course = db.Courses.FirstOrDefault(c => c.Department == subject && c.Number == num);
            if (course == null)
            {
                return Json(new { success = false });
            }

            var existingCategory = db.AssignmentCategories.FirstOrDefault(ac =>
                ac.Class.CourseId == course.Id &&
                ac.Class.Season == season &&
                ac.Class.Year == year &&
                ac.Name == category);

            if (existingCategory != null)
            {
                return Json(new { success = false });
            }

            var newCategory = new AssignmentCategory
            {
                Name = category,
                Weight = (ushort)catweight,
                ClassId = db.Classes
                    .FirstOrDefault(c => c.CourseId == course.Id && c.Season == season && c.Year == year)?.Id ?? 0
            };

            db.AssignmentCategories.Add(newCategory);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
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

            var assignmentCategory = db.AssignmentCategories.FirstOrDefault(ac =>
                ac.ClassId == classObj.Id &&
                ac.Name == category);

            if (assignmentCategory == null)
            {
                return Json(new { success = false });
            }

            var existingAssignment = db.Assignments.FirstOrDefault(a =>
                a.CategoryId == assignmentCategory.Id &&
                a.Name == asgname);

            if (existingAssignment != null)
            {
                return Json(new { success = false });
            }

            var newAssignment = new Assignment
            {
                CategoryId = assignmentCategory.Id,
                Name = asgname,
                Contents = asgcontents,
                Points = (ushort)asgpoints,
                Due = asgdue
            };

            db.Assignments.Add(newAssignment);
            db.SaveChanges();

            return Json(new { success = true });
        }

        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var course = db.Courses.FirstOrDefault(c => c.Department == subject && c.Number == num);
            if (course == null)
            {
                return Json(null);
            }

            var classObj = db.Classes.FirstOrDefault(c =>
                c.CourseId == course.Id &&
                c.Season == season &&
                c.Year == year);

            if (classObj == null)
            {
                return Json(null);
            }

            var assignmentCategory = db.AssignmentCategories.FirstOrDefault(ac =>
                ac.ClassId == classObj.Id &&
                ac.Name == category);

            if (assignmentCategory == null)
            {
                return Json(null);
            }

            var assignment = db.Assignments.FirstOrDefault(a =>
                a.CategoryId == assignmentCategory.Id &&
                a.Name == asgname);

            if (assignment == null)
            {
                return Json(null);
            }

            var submissions = db.Submissions
                .Where(s => s.AssignmentId == assignment.Id)
                .Select(s => new
                {
                    fname = s.Student.FirstName,
                    lname = s.Student.LastName,
                    uid = s.Student.Uid,
                    time = s.Time,
                    score = s.Score
                })
                .ToList();

            return Json(submissions);
        }

        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
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

            var assignmentCategory = db.AssignmentCategories.FirstOrDefault(ac =>
                ac.ClassId == classObj.Id &&
                ac.Name == category);

            if (assignmentCategory == null)
            {
                return Json(new { success = false });
            }

            var assignment = db.Assignments.FirstOrDefault(a =>
                a.CategoryId == assignmentCategory.Id &&
                a.Name == asgname);

            if (assignment == null)
            {
                return Json(new { success = false });
            }

            var submission = db.Submissions.FirstOrDefault(s =>
                s.AssignmentId == assignment.Id &&
                s.StudentId == uid);

            if (submission == null)
            {
                return Json(new { success = false });
            }

            submission.Score = (ushort)score;

            db.SaveChanges();

            // Update the student's grade for the class
            UpdateStudentGrade(uid, classObj.Id);

            return Json(new { success = true });
        }

        private void UpdateStudentGrade(string studentId, uint classId)
        {
            var enrollment = db.Enrollments.FirstOrDefault(e =>
                e.ClassId == classId &&
                e.StudentId == studentId);

            if (enrollment != null)
            {
                var classObj = enrollment.Class;

                // Get all assignment categories for the class
                var assignmentCategories = db.AssignmentCategories.Where(ac => ac.ClassId == classObj.Id).ToList();

                // Calculate the weighted percentage for each non-empty category
                var totalWeightedPercentage = 0.0;
                var totalCategoryWeights = 0.0;

                foreach (var category in assignmentCategories)
                {
                    var categoryAssignments = db.Assignments
                        .Where(a => a.CategoryId == category.Id)
                        .ToList();

                    if (categoryAssignments.Count > 0)
                    {
                        var totalPointsEarned = db.Submissions
                            .Where(s => s.StudentId == studentId && s.Assignment.CategoryId == category.Id)
                            .Sum(s => s.Score);

                        var totalMaxPoints = categoryAssignments.Sum(a => a.Points);

                        var categoryPercentage = (double)totalPointsEarned / totalMaxPoints;

                        var scaledTotal = categoryPercentage * category.Weight;

                        totalWeightedPercentage += scaledTotal;
                        totalCategoryWeights += category.Weight;
                    }
                }

                // Compute the total percentage for the class
                var totalPercentage = 0.0;
                if (totalCategoryWeights != 0)
                {
                    totalPercentage = (totalWeightedPercentage / totalCategoryWeights) * 100.0;
                }

                // TODO:
                // Convert the class percentage to a letter grade using the U of U grading system
                // var letterGrade = CalculateLetterGrade(totalPercentage);

                // enrollment.Grade = letterGrade;
                db.SaveChanges();
            }
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var professor = db.Professors.FirstOrDefault(p => p.Uid == uid);
            if (professor == null)
            {
                return Json(new List<object>());
            }

            var classes = db.Classes.Where(c => c.ProfessorId == uid)
                .Select(c => new
                {
                    subject = c.Course.Department,
                    number = c.Course.Number,
                    name = c.Course.Name,
                    season = c.Season,
                    year = c.Year
                }).ToList();

            return Json(classes);
        }


        /*******End code to modify********/
    }
}

