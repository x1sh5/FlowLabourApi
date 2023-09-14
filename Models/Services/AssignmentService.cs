using FlowLabourApi.Models.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;


namespace FlowLabourApi.Models.Services
{
    public class AssignmentService
    {
        private readonly FlowContext _context;

        public AssignmentService(FlowContext flowContext)
        {
            _context = flowContext;
        }

        public async Task<EntityEntry<Assignment>> Add(Assignment assignment)
        {
            var e = await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            return e;
        }

        public async Task<List<Assignment>> GetAssignments(uint count,
            Expression<Func<Assignment, bool>> expression)
        {
            return await _context.Assignments.Include(a => a.AuthUser)
            .Where(expression)
            .Take((int)count).ToListAsync();
        }

        public async Task<List<Assignment>> GetAssignments(int? typeid)
        {
            List<Assignment> assignments;
            if (typeid != null)
            {
                assignments = await _context.Assignments.Include(o => o.AuthUser)
                    .Where(e => e.TypeId == typeid).ToListAsync();
            }
            else
            {
                assignments = await _context.Assignments.Include(o => o.AuthUser).ToListAsync();
            }
            return assignments;
        }

        public async Task<List<Assignment>> GetAssignments(IEnumerable<int> ids)
        {
            return await _context.Assignments.Include(a => a.AuthUser)
            .Where(o => ids.Contains(o.Id)).ToListAsync();
        }

        public async Task<List<Assignment>> GetAssignmentsByUser(int id)
        {
            return await _context.Assignments.Where(a => a.UserId == id).ToListAsync();
        }

        public async Task<List<Assignment>> SearchByTitle(string title)
        {
            return await _context.Assignments.Include(a => a.AuthUser)
            .Where(a => a.Title!.Contains(title)).ToListAsync();
        }

        public async Task<Assignment> GetAssignment(int id,bool includeAuthuser = false)
        {
            if(includeAuthuser)
            {
                return await _context.Assignments
            .Include(o => o.AuthUser).FirstOrDefaultAsync(x => x.Id == id);
            }

            return await _context.Assignments
                    .FirstOrDefaultAsync(x => x.Id == id);
        }


    }
}
