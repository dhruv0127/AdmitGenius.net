namespace AdmitGenius.Models
{
    public class Hashtag
    {
        public int HashtagId { get; set; }
        public string Name { get; set; }

        // Add other properties as needed

        public virtual ICollection<Courses> Courses { get; set; } = new List<Courses>();
    }
}
