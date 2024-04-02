namespace AdmitGenius.Models.ViewModels
{
    public class CourseViewModel
    {
        public Courses CourseDetails { get; set; }

        public virtual Universities UniversityDetails { get; set; }
        public List<Counselors>? Counselors { get; set; }


        // Add the property for related courses
        public List<Courses> RelatedCourses { get; set; }

    }
}
