using AnketPortali.Models;
using AnketPortali.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AnketPortali.Repositories
{
    public class AnswersRepository : GenericRepository<Answer>
    {
        private readonly AppDbContext _context;

        public AnswersRepository(AppDbContext context) : base(context, context.Answers)
        {
            _context = context;
        }

        public async Task<List<Answer>> GetAllAsync(Expression<Func<Answer, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return await _context.Answers.Where(predicate).ToListAsync();
            }
            return await _context.Answers.ToListAsync();
        }
    }
}
