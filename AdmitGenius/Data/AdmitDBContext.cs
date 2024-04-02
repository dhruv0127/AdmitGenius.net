using AdmitGenius.Controllers;
using AdmitGenius.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmitGenius.Data
{
    public class AdmitDBContext : DbContext
    {
        public AdmitDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Universities> Universities { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Counselors> Counselors { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<StudentEnrollRequest> StudentEnrollRequest { get; set; }
        public DbSet<EligibilityTest> EligibilityTest { get; set; }
        public DbSet<Interest> Interest { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<S3FileDetails> S3FileDetails { get; set; }
        public DbSet<Community> Community { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Experiences> Experiences { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<InternRequest> InternRequest { get; set; }
        public DbSet<Scholarship> Scholarship { get; set; }
        public DbSet<CommunityMember> CommunityMember { get; set; }
        public DbSet<CounselorSubscribers> CounselorSubscribers { get; set; }


        
    }
}
