using System;

namespace LMS.Controllers
{
    public static class Helper
    {
        // Helper method to get the grade point value based on the letter grade
        public static double GetGradePoint(string grade)
        {
            switch (grade.ToUpper())
            {
                case "A":
                    return 4.0;
                case "A-":
                    return 3.7;
                case "B+":
                    return 3.3;
                case "B":
                    return 3.0;
                case "B-":
                    return 2.7;
                case "C+":
                    return 2.3;
                case "C":
                    return 2.0;
                case "C-":
                    return 1.7;
                case "D+":
                    return 1.3;
                case "D":
                    return 1.0;
                case "D-":
                    return 0.7;
                case "E":
                    return 0.0;
                default:
                    return 0.0;
            }
        }

        public static string PercentageToGradePoint(double cumulativePoints, double totalPoints)
        {
            double gradePoint = cumulativePoints / totalPoints * 4.0;

            // Now, we'll reverse the scale using the GetGradePoint method
            if (gradePoint == 4.0)
            {
                return "A";
            }
            else if (gradePoint >= 3.7)
            {
                return "A-";
            }
            else if (gradePoint >= 3.3)
            {
                return "B+";
            }
            else if (gradePoint >= 3.0)
            {
                return "B";
            }
            else if (gradePoint >= 2.7)
            {
                return "B-";
            }
            else if (gradePoint >= 2.3)
            {
                return "C+";
            }
            else if (gradePoint >= 2.0)
            {
                return "C";
            }
            else if (gradePoint >= 1.7)
            {
                return "C-";
            }
            else if (gradePoint >= 1.3)
            {
                return "D+";
            }
            else if (gradePoint >= 1.0)
            {
                return "D";
            }
            else if (gradePoint >= 0.7)
            {
                return "D-";
            }
            else
            {
                return "E";
            }
        }
    }
}
