namespace AdmitGenius.Models.ViewModels
{
    public class EnrollmentViewModel
    {
        public Guid CourseId { get; set; }
        public string CounselorEmailId { get; set; }

        public EligibilityTestViewModel EligibilityTest { get; set; }
    }
}
