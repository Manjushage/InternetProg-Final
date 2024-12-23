using AnketPortali.Models;

namespace AnketPortali.Repositories
{
    public class UserAnswerRepository : GenericRepository<UserAnswer>
    {
        public UserAnswerRepository(AppDbContext context) : base(context, context.UserAnswers)
        {
        }
        public async Task<List<UserAnswer>> GetAllUserAsync(Func<UserAnswer, bool> predicate)
        {
            return await Task.FromResult(_context.UserAnswers.Where(predicate).ToList());
        }
    }
} 