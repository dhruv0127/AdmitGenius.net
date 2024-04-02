namespace AdmitGenius.Models.ViewModels
{
    public class AllCompaniesViewModel
    {
        public IEnumerable<Company> Company { get; set; }
        public IEnumerable<Job> Job { get; set; }
    }
}
