namespace AdmitGenius.Models.ViewModels
{
    public class CoursePageViewModel
    {
        public Courses CourseDetails { get; set; }
        public virtual Universities UniversityDetails { get; set; }
        public List<Counselors>? Counselors { get; set; }

        // Add the property for related courses
        public List<Courses> RelatedCourses { get; set; }

        // Constructor
        public CoursePageViewModel(Courses course, Universities university, List<Counselors>? counselors, List<Courses> relatedCourses)
        {
            CourseDetails = course;
            UniversityDetails = university;
            Counselors = counselors;
            RelatedCourses = relatedCourses;
        }
    }
}
