using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
    /// <param name="count"></param>
    /// <param name="offset"></param>
    /// <example>GET api/assignment?count=10&offset=0</example>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    //[SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignments([Required]uint count,
        [Required]int offset)
    {
        List<Assignment> assignments;
        assignments = await _context.Assignments.Include(a => a.AuthUser).Where(o=>o.Id>offset).Take((int)count).ToListAsync();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in assignments)
        {
            AssignmentView assignmentView = new AssignmentView();
            assignmentView.Id = e.Id;
            assignmentView.Title = e.Title;
            assignmentView.Description = e.Description;
            assignmentView.Branchid = e.Branchid;
            assignmentView.Typeid = e.Typeid;
            assignmentView.Presumedtime = e.Presumedtime;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.Reward = e.Reward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = e.AuthUser?.UserName;
            assignmentView.UserId = e.UserId;
            //assignmentView.Images = _context.Images.Where(et => et.AssignmentId == e.Id).Select(e => e.Url).ToArray(); ;
            assignmentViews.Add(assignmentView);
        }
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
            assignments = await _context.Assignments.Include(o => o.AuthUser)
                .Where(e => e.Typeid == typeid).ToListAsync();
        }
        else
        {
            assignments = await _context.Assignments.Include(o => o.AuthUser).ToListAsync();
        } 

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
            Username = e.AuthUser?.UserName,
            UserId = e.UserId,
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
        //var assignment = await _context.Assignments.Include(a => a.AuthUser).FirstOrDefaultAsync(x=>x.Id==id);

        Assignment? e;
        e = await _context.Assignments
            .Include(o => o.AuthUser).FirstOrDefaultAsync(x => x.Id == id);

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
            Username = e.AuthUser?.UserName,
        };

        return assignmentView;
    }

    /// <summary>
    /// 根据用户获取对应的任务
    /// </summary>
    /// <returns></returns>
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignmentByUser()
    {
        var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        var user = await _userManager.FindByIdAsync(id);
        List<Assignment> assignments;
        assignments = await _context.Assignments.Include(a => a.AuthUser).Where(a => a.UserId == user.Id).ToListAsync();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in assignments)
        {
            AssignmentView assignmentView = new AssignmentView();
            assignmentView.Id = e.Id;
            assignmentView.Title = e.Title;
            assignmentView.Description = e.Description;
            assignmentView.Branchid = e.Branchid;
            assignmentView.Typeid = e.Typeid;
            assignmentView.Presumedtime = e.Presumedtime;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.Reward = e.Reward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = e.AuthUser?.UserName;
            //assignmentView.Images = _context.Images.Where(et => et.AssignmentId == e.Id).Select(e => e.Url).ToArray(); ;
            assignmentViews.Add(assignmentView);
        }
        return assignmentViews;
    }

    /// <summary>
    /// 按标题搜索
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    [HttpGet("search/{title}")]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> SearchByTitile(string title)
    {
        var assignments = await _context.Assignments.Include(a => a.AuthUser)
            .Where(a => a.Title!.Contains(title)).ToListAsync();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in assignments)
        {
            AssignmentView assignmentView = new AssignmentView();
            assignmentView.Id = e.Id;
            assignmentView.Title = e.Title;
            assignmentView.Description = e.Description;
            assignmentView.Branchid = e.Branchid;
            assignmentView.Typeid = e.Typeid;
            assignmentView.Presumedtime = e.Presumedtime;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.Reward = e.Reward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = e.AuthUser?.UserName;
            assignmentView.UserId = e.UserId;
            //assignmentView.Images = _context.Images.Where(et => et.AssignmentId == e.Id).Select(e => e.Url).ToArray(); ;
            assignmentViews.Add(assignmentView);
        }
        return assignmentViews;
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
            Publishtime = DateTime.Now,
            Status = assignmentView.Status,
            Verify = assignmentView.Verify,
            Reward = assignmentView.Reward,
            Rewardtype = assignmentView.Rewardtype,
            UserId = user.Id,
        };
        var e = _context.Assignments.Add(assignment);
        _context.SaveChanges();
        _context.Assignmentusers.Add(new AssignmentUser
        {
            AssignmentId = e.Entity.Id,
            UserId = user.Id,
        });
        _context.SaveChanges();

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

    /// <summary>
    /// 获取任务总数
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("total")]
    [AllowAnonymous]
    public IActionResult Total()
    {
        int total = _context.Assignments.Count();
        return Ok(total);
    }

    private bool AssignmentExists(int id)
    {
        return _context.Assignments.Any(e => e.Id == id);
    }
}
