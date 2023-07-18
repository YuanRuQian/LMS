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

```

All users' password is `RmyUPz3FKFNtpMx`.