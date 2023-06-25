using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly XiangxpContext _context;

    public AssignmentController(XiangxpContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    //[SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
    {
        return await _context.Assignments.ToListAsync();
    }

    /// <summary>
    /// 根据ID获取任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Assignment>> GetAssignment(int id)
    {
        //var assignment = await _context.Assignments.Include(a => a.Publish).FirstOrDefaultAsync(x=>x.Id==id);
        var assignment = await _context.Assignments.FindAsync(id);

        if (assignment == null)
        {
            return NotFound();
        }

        return assignment;
    }

    /// <summary>
    /// 新建任务
    /// </summary>
    /// <param name="assignmentView"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Assignment>> PostAssignment(AssignmentView assignmentView)
    {
        //var s
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
            Publish = _context.AuthUsers.Find(assignmentView.Publishid),
        };
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
    }

    /// <summary>
    /// 根据ID修改任务
    /// </summary>
    /// <param name="id"></param>
    /// <param name="assignmentView"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAssignment(int id, AssignmentView assignmentView)
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
            Publish = _context.AuthUsers.Find(assignmentView.Id),
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
