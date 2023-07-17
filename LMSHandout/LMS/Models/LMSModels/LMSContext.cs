using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Enrollment> Enrollments { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.3-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PRIMARY");

                entity.ToTable("administrators");

                entity.Property(e => e.Uid)
                    .HasMaxLength(8)
                    .HasColumnName("uid")
                    .IsFixedLength();

                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_name");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("assignments");

                entity.HasIndex(e => e.CategoryId, "FK_category_id");

                entity.HasIndex(e => e.Name, "unique_assignment_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("category_id");

                entity.Property(e => e.Contents)
                    .HasMaxLength(8192)
                    .HasColumnName("contents");

                entity.Property(e => e.Due)
                    .HasColumnType("datetime")
                    .HasColumnName("due");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Points)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("points");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_category_id");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.ToTable("assignment_categories");

                entity.HasIndex(e => e.ClassId, "FK_class_id_2");

                entity.HasIndex(e => new { e.Name, e.ClassId }, "unique_name_class_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("class_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Weight)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("weight");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_class_id_2");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("classes");

                entity.HasIndex(e => e.ProfessorId, "FK_professor_id");

                entity.HasIndex(e => new { e.CourseId, e.Year, e.Season }, "unique_course_year_season")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("course_id");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time")
                    .HasColumnName("end_time");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.ProfessorId)
                    .HasMaxLength(8)
                    .HasColumnName("professor_id")
                    .IsFixedLength();

                entity.Property(e => e.Season)
                    .HasColumnType("enum('Spring','Fall','Summer')")
                    .HasColumnName("season");

                entity.Property(e => e.StartTime)
                    .HasColumnType("time")
                    .HasColumnName("start_time");

                entity.Property(e => e.Year)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("year");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_course_id");

                entity.HasOne(d => d.Professor)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ProfessorId)
                    .HasConstraintName("FK_professor_id");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("courses");

                entity.HasIndex(e => new { e.Department, e.Number }, "unique_department_number")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Department)
                    .HasMaxLength(4)
                    .HasColumnName("department");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Number)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("number");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK_department_courses");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.ToTable("departments");

                entity.Property(e => e.Subject)
                    .HasMaxLength(4)
                    .HasColumnName("subject");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.ClassId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("enrollment");

                entity.HasIndex(e => e.ClassId, "FK_class_id");

                entity.Property(e => e.StudentId)
                    .HasMaxLength(8)
                    .HasColumnName("student_id")
                    .IsFixedLength();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("class_id");

                entity.Property(e => e.Grade)
                    .HasColumnType("enum('A','A-','B+','B','B-','C+','C','C-','D+','D','D-','E','X','WF','EW','EU','F')")
                    .HasColumnName("grade");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("FK_class_id");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_student_id");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PRIMARY");

                entity.ToTable("professors");

                entity.HasIndex(e => e.Department, "FK_department_professor");

                entity.Property(e => e.Uid)
                    .HasMaxLength(8)
                    .HasColumnName("uid")
                    .IsFixedLength();

                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

                entity.Property(e => e.Department)
                    .HasMaxLength(4)
                    .HasColumnName("department");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_name");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK_department_professor");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PRIMARY");

                entity.ToTable("students");

                entity.HasIndex(e => e.Department, "FK_department_student");

                entity.Property(e => e.Uid)
                    .HasMaxLength(8)
                    .HasColumnName("uid")
                    .IsFixedLength();

                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

                entity.Property(e => e.Department)
                    .HasMaxLength(4)
                    .HasColumnName("department");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_name");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK_department_student");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.AssignmentId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("submission");

                entity.HasIndex(e => e.AssignmentId, "FK_assignment_id_2");

                entity.Property(e => e.StudentId)
                    .HasMaxLength(8)
                    .HasColumnName("student_id")
                    .IsFixedLength();

                entity.Property(e => e.AssignmentId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignment_id");

                entity.Property(e => e.Contents)
                    .HasMaxLength(8192)
                    .HasColumnName("contents");

                entity.Property(e => e.Score)
                    .HasColumnType("smallint(5) unsigned")
                    .HasColumnName("score");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .HasConstraintName("FK_assignment_id_2");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_student_id_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
