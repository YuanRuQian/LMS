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
            double gradePoint = cumulativePoints / totalPoints * 100;

            // Now, we'll reverse the scale using the GetGradePoint method
            // reference: https://eli.utah.edu/about_the_eli/graduationreq.php#:~:text=University%20Connected%20Learning%20%20%20%20Letter%20Grade,the%20objective%20...%20%208%20more%20rows%20
            if (gradePoint >= 93 )
            {
                return "A";
            }
            else if (gradePoint >= 90)
            {
                return "A-";
            }
            else if (gradePoint >= 87)
            {
                return "B+";
            }
            else if (gradePoint >= 83)
            {
                return "B";
            }
            else if (gradePoint >= 80)
            {
                return "B-";
            }
            else if (gradePoint >= 77)
            {
                return "C+";
            }
            else if (gradePoint >= 73)
            {
                return "C";
            }
            else if (gradePoint >= 70)
            {
                return "C-";
            }
            else if (gradePoint >= 67)
            {
                return "D+";
            }
            else if (gradePoint >= 63)
            {
                return "D";
            }
            else if (gradePoint >= 60)
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
