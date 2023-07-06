using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FlowLabourApi.Config;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentController : ControllerBase
{
    private readonly XiangxpContext _context;
    private readonly UserManager<AuthUser> _userManager;

    public AssignmentController(XiangxpContext context, UserManager<AuthUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    //[SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignments()
    {
        List<Assignment> assignments; 
        assignments = await _context.Assignments
            .Include(a => a.Assignmentuser) // 关联AssignmentUser表
        .ThenInclude(au => au.User) // 关联User表ToListAsync();
        .ToListAsync();
        List<AssignmentView> assignmentViews;
        assignmentViews = assignments.Select(e => new AssignmentView
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Branchid = e.Branchid,
            Typeid = e.Typeid,
            Presumedtime = e.Presumedtime,
            Publishtime = e.Publishtime,
            Status = e.Status,
            Verify = e.Verify,
            Reward = e.Reward,
            Rewardtype = e.Rewardtype,
            Username = e.Publish?.UserName,
            Images = _context.Images.Where(et=>et.Id==e.Id).Select(e => e.Url).ToList(),
        }).ToList();
        return assignmentViews;
    }

    /// <summary>
    /// 获取所有类型为typeid的任务
    /// </summary>
    /// <returns></returns>
    [HttpGet("type/{typeid?}")]
    [AllowAnonymous]
    //[SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignments(int? typeid)
    {
        List<Assignment> assignments;
        if (typeid != null)
        {
            assignments =  await _context.Assignments.Include(o => o.Publish)
                .Where(e => e.Typeid == typeid).ToListAsync();
        }
        assignments = await _context.Assignments.Include(o => o.Publish).ToListAsync();

        List<AssignmentView> assignmentViews;
        assignmentViews = assignments.Select(e => new AssignmentView
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Branchid = e.Branchid,
            Typeid = e.Typeid,
            Presumedtime = e.Presumedtime,
            Publishtime = e.Publishtime,
            Status = e.Status,
            Verify = e.Verify,
            Reward = e.Reward,
            Rewardtype = e.Rewardtype,
            Username = e.Publish?.UserName,
        }).ToList();
        return assignmentViews;
    }


    /// <summary>
    /// 根据ID获取任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentView>> GetAssignment(int id)
    {
        //var assignment = await _context.Assignments.Include(a => a.Publish).FirstOrDefaultAsync(x=>x.Id==id);

        Assignment? e;
        e = await _context.Assignments
            .Include(o => o.Publish).FirstOrDefaultAsync(x => x.Id == id);

        if (e == null)
        {
            return NotFound();
        }

        AssignmentView assignmentView = new AssignmentView
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Branchid = e.Branchid,
            Typeid = e.Typeid,
            Presumedtime = e.Presumedtime,
            Publishtime = e.Publishtime,
            Status = e.Status,
            Verify = e.Verify,
            Reward = e.Reward,
            Rewardtype = e.Rewardtype,
            Username = e.Publish?.UserName,
        };

        return assignmentView;
    }

    /// <summary>
    /// 新建任务
    /// </summary>
    /// <param name="assignmentView"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Assignment>> PostAssignment(AssignmentView assignmentView)
    {
        var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        var user = await _userManager.FindByIdAsync(id);
        Assignment assignment = new Assignment
        {
            Title = assignmentView.Title,
            Description = assignmentView.Description,
            Branchid = assignmentView.Branchid,
            Typeid = assignmentView.Typeid,
            Presumedtime = assignmentView.Presumedtime,
            Publishtime = assignmentView.Publishtime,
            Status = assignmentView.Status,
            Verify = assignmentView.Verify,
            Reward = assignmentView.Reward,
            Rewardtype = assignmentView.Rewardtype,
        };
        var e = _context.Assignments.Add(assignment);
         _context.SaveChanges();
        _context.Assignmentusers.Add(new AssignmentUser
        {
            Assignmentid = e.Entity.Id,
            Userid = user.Id,
        });
        await _context.SaveChangesAsync();


        return CreatedAtAction(nameof(GetAssignment), new { id = e.Entity.Id }, e.Entity);
    }

    /// <summary>
    /// 根据ID修改任务
    /// </summary>
    /// <param name="id"></param>
    /// <param name="assignmentView"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAssignment(int id, Assignment assignmentView)
    {

        if (id != assignmentView.Id)
        {
            return BadRequest();
        }
        Assignment assignment = new Assignment
        {
            Id = assignmentView.Id,
            Title = assignmentView.Title,
            Description = assignmentView.Description,
            Branchid = assignmentView.Branchid,
            Typeid = assignmentView.Typeid,
            Presumedtime = assignmentView.Presumedtime,
            Publishtime = assignmentView.Publishtime,
            Status = assignmentView.Status,
            Verify = assignmentView.Verify,
            Reward = assignmentView.Reward,
            Rewardtype = assignmentView.Rewardtype,
        };
        _context.Entry(assignment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AssignmentExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// 根据ID删除任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment == null)
        {
            return NotFound();
        }

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AssignmentExists(int id)
    {
        return _context.Assignments.Any(e => e.Id == id);
    }
}
