using AdmitGenius.Data;

namespace AdmitGenius.Models
{
    public class DataAccessLayer
    {
        private readonly AdmitDBContext _dbContext;

        public DataAccessLayer(AdmitDBContext dbContext)
        {
            _dbContext = dbContext;
        }
                
        public Users GetUserById(Guid userId)
        {
            return _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
        }
    }
}
