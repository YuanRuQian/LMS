# Generate & Insert test data

```sql
USE Team12LMS;

INSERT INTO departments (subject, name) VALUES
  ('CS', 'Computer Science'),
  ('MATH', 'Mathematics');

INSERT INTO courses (department, number, name) VALUES
  ('CS', 101, 'Introduction to Computer Science'),
  ('CS', 201, 'Data Structures'),
  ('MATH', 101, 'Calculus I'),
  ('MATH', 201, 'Linear Algebra');

INSERT INTO classes (course_id, professor_id, year, season, start_time, end_time, location) VALUES
  (1, 'professor1', 2023, 'Fall', '08:00:00', '10:00:00', 'Room 101'),
  (2, 'professor1', 2023, 'Fall', '10:00:00', '12:00:00', 'Room 102'),
  (3, 'professor2', 2023, 'Fall', '14:00:00', '16:00:00', 'Room 201'),
  (4, 'professor2', 2023, 'Fall', '16:00:00', '18:00:00', 'Room 202');

```