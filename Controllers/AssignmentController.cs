using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentController : ControllerBase
{
    private readonly FlowContext _context;
    private readonly ILogger<AssignmentController> _logger;
    //private readonly UserManager<AuthUser> _userManager;

    public AssignmentController(FlowContext context, ILogger<AssignmentController> logger)
    {
        _context = context;
        _logger = logger;
        //_userManager = userManager;
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    /// <param name="count"></param>
    /// <param name="offset"></param>
    /// <param name="typeid"></param>
    /// <example>GET api/assignment?count=10&offset=0</example>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    //[SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(NotFoundResult))]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignments([Required]uint count,
        [Required]int offset, int? typeid)
    {
        Expression<Func<Assignment, bool>> expression;
        if (typeid != null)
        {
            expression = o => o.Id > offset && o.Status != (sbyte)TaskState.Unfinished &&o.TypeId == typeid;
        }
        else
        {
            expression = o => o.Id > offset && o.Status != (sbyte)TaskState.Unfinished;
        }
        List<Assignment> assignments;
        assignments = await _context.Assignments.Include(a => a.AuthUser)
            .Where(expression)
            .Take((int)count).ToListAsync();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in assignments)
        {
            AssignmentView assignmentView = new AssignmentView();
            assignmentView.Id = e.Id;
            assignmentView.Title = e.Title;
            assignmentView.Description = e.Description;
            assignmentView.Branchid = e.Branchid;
            assignmentView.TypeId = e.TypeId;
            assignmentView.Deadline = e.Deadline;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.Payed = e.Payed;
            assignmentView.FixedReward = e.FixedReward;
            assignmentView.PercentReward = e.PercentReward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = e.AuthUser?.UserName;
            assignmentView.CanTake = e.CanTake;
            assignmentView.UserId = e.UserId;
            assignmentView.Main = e.Main;
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
                .Where(e => e.TypeId == typeid).ToListAsync();
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
            TypeId = e.TypeId,
            Deadline = e.Deadline,
            Publishtime = e.Publishtime,
            Status = e.Status,
            Verify = e.Verify,
            CanTake = e.CanTake,
            FixedReward = e.FixedReward,
            PercentReward = e.PercentReward,
            Rewardtype = e.Rewardtype,
            Username = e.AuthUser?.UserName,
            UserId = e.UserId,
            Main = e.Main,
            Payed = e.Payed,
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
            TypeId = e.TypeId,
            Deadline = e.Deadline,
            Publishtime = e.Publishtime,
            Status = e.Status,
            Verify = e.Verify,
            CanTake = e.CanTake,
            FixedReward = e.FixedReward,
            PercentReward = e.PercentReward,
            Rewardtype = e.Rewardtype,
            Username = e.AuthUser?.UserName,
            Main = e.Main,
            Payed = e.Payed,
        };

        return assignmentView;
    }

    /// <summary>
    /// get assignment by ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost("assignments")]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignments(IEnumerable<int> ids)
    {
        List<Assignment> assignments;
        assignments = await _context.Assignments.Include(a => a.AuthUser)
            .Where(o => ids.Contains(o.Id)).ToListAsync();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in assignments)
        {
            AssignmentView assignmentView = new AssignmentView();
            assignmentView.Id = e.Id;
            assignmentView.Title = e.Title;
            assignmentView.Description = e.Description;
            assignmentView.Branchid = e.Branchid;
            assignmentView.TypeId = e.TypeId;
            assignmentView.Deadline = e.Deadline;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.CanTake = e.CanTake;
            assignmentView.FixedReward = e.FixedReward;
            assignmentView.PercentReward = e.PercentReward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = e.AuthUser?.UserName;
            assignmentView.UserId = e.UserId;
            assignmentView.Main = e.Main;
            assignmentView.Payed = e.Payed;
            //assignmentView.Images = _context.Images.Where(et => et.AssignmentId == e.Id).Select(e => e.Url).ToArray(); ;
            assignmentViews.Add(assignmentView);
        }
        return assignmentViews;
    }
  

    /// <summary>
    /// 根据用户获取对应的任务
    /// </summary>
    /// <returns></returns>
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetAssignmentByUser()
    {
        var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        var userName = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.NameClaim).Value;
        List<Assignment> assignments;
        assignments = await _context.Assignments.Where(a => a.UserId == Convert.ToInt32(id)).ToListAsync();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in assignments)
        {
            AssignmentView assignmentView = new AssignmentView();
            assignmentView.Id = e.Id;
            assignmentView.Title = e.Title;
            assignmentView.Description = e.Description;
            assignmentView.Branchid = e.Branchid;
            assignmentView.TypeId = e.TypeId;
            assignmentView.Deadline = e.Deadline;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.CanTake = e.CanTake;
            assignmentView.FixedReward = e.FixedReward;
            assignmentView.PercentReward = e.PercentReward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = userName;
            assignmentView.Main = e.Main;
            assignmentView.Payed = e.Payed;
            //assignmentView.Images = _context.Images.Where(et => et.AssignmentId == e.Id).Select(e => e.Url).ToArray(); ;
            assignmentViews.Add(assignmentView);
        }
        return assignmentViews;
    }

    /// <summary>
    /// 获取子任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("childs/{id}")]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> GetChilds([Required] int id)
    {
        var userName = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.NameClaim).Value;
        var cs = _context.Relatedtasks
            .Where(x => x.AssignmentId == id)
            .Select(x => x.RelatedId).ToArray();
        if (cs.Length == 0)
        {
            return NoContent();
        }
        var es = _context.Assignments.Where(x => cs.Contains(x.Id)).ToArray();
        List<AssignmentView> assignmentViews = new List<AssignmentView>();
        foreach (var e in es)
        {
            assignmentViews.Add(new AssignmentView
            {
                Id = e.Id,
                Title = e.Title,
                Deadline = e.Deadline,
                Description = e.Description,
                Branchid = e.Branchid,
                CanTake = e.CanTake,
                Finishtime = e.Finishtime,
                FixedReward = e.FixedReward,
                PercentReward = e.PercentReward,
                Publishtime = e.Publishtime,
                TypeId = e.TypeId,
                Main = e.Main,
                Rewardtype = e.Rewardtype,
                Status = e.Status,
                UserId = e.UserId,
                Verify = e.Verify,
                Username = userName,
                Payed = e.Payed,
            });
        }

        return assignmentViews;
    }

    [HttpGet("parent/{id}")]
    public async Task<ActionResult<AssignmentView>> GetParent([Required] int id)
    {
        var userName = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.NameClaim).Value;
        var cs = _context.Relatedtasks
            .Where(x => x.RelatedId == id)
            .Select(x => x.AssignmentId).ToArray();
        if (cs.Length != 1)
        {
            return NoContent();
        }
        var e = _context.Assignments.Where(x => x.Id == cs[0]).FirstOrDefault();
        AssignmentView assignmentViews = new AssignmentView
            {
                Id = e.Id,
                Title = e.Title,
                Deadline = e.Deadline,
                Description = e.Description,
                Branchid = e.Branchid,
                CanTake = e.CanTake,
                Finishtime = e.Finishtime,
                FixedReward = e.FixedReward,
                PercentReward = e.PercentReward,
                Publishtime = e.Publishtime,
                TypeId = e.TypeId,
                Main = e.Main,
                Rewardtype = e.Rewardtype,
                Status = e.Status,
                UserId = e.UserId,
                Verify = e.Verify,
                Username = userName,
                Payed = e.Payed,
            };

        return assignmentViews;
    }

    /// <summary>
    /// 按标题搜索
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    [HttpGet("search/{title}")]
    public async Task<ActionResult<IEnumerable<AssignmentView>>> SearchByTitle(string title)
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
            assignmentView.TypeId = e.TypeId;
            assignmentView.Deadline = e.Deadline;
            assignmentView.Payed = e.Payed;
            assignmentView.Publishtime = e.Publishtime;
            assignmentView.Status = e.Status;
            assignmentView.Verify = e.Verify;
            assignmentView.CanTake = e.CanTake;
            assignmentView.FixedReward = e.FixedReward;
            assignmentView.PercentReward = e.PercentReward;
            assignmentView.Rewardtype = e.Rewardtype;
            assignmentView.Username = e.AuthUser?.UserName;
            assignmentView.UserId = e.UserId;
            assignmentView.Main = e.Main;
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
        //var user = await _userManager.FindByIdAsync(id);
        var user = await _context.AuthUsers.FindAsync(Convert.ToInt32(id));
        Assignment assignment = new Assignment
        {
            Title = assignmentView.Title,
            Description = assignmentView.Description,
            Branchid = assignmentView.Branchid,
            TypeId = assignmentView.TypeId,
            Deadline = assignmentView.Deadline,
            Publishtime = DateTime.Now,
            Status = assignmentView.Status,
            Verify = assignmentView.Verify,
            CanTake = assignmentView.Main==1 && assignmentView.PercentReward==10000 ? (sbyte)1:(sbyte)0,
            FixedReward = assignmentView.FixedReward,
            PercentReward = assignmentView.PercentReward,
            Rewardtype = assignmentView.Rewardtype,
            UserId = user.Id,
        };
        EntityEntry<Assignment>? e;
        try
        {
            e = _context.Assignments.Add(assignment);
            _context.SaveChanges();
        }catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("创建失败。");
        }


        return CreatedAtAction(nameof(GetAssignment), new { id = e.Entity.Id },"创建成功。");
    }

    [HttpPost("posts")]
    public async Task<ActionResult<ResponeMessage<SimpleResp>>> PostsAssignment([FromBody]List<AssignmentView> assignmentViews)
    {
        var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        //var user = await _userManager.FindByIdAsync(id);
        var user = await _context.AuthUsers.FindAsync(Convert.ToInt32(id));
        var m = assignmentViews.Where(o => o.Main == 1).ToArray();
        if(m.Length != 1 )
        {
            return BadRequest("数据不合规！");
        }

        using var transaction = _context.Database.BeginTransaction();
        transaction.CreateSavepoint("before");
        EntityEntry<Assignment> entityEntry = null;
        
        try
        {
            entityEntry = _context.Assignments.Add(new Assignment
            {
                Title = m[0].Title,
                Description = m[0].Description,
                Branchid = m[0].Branchid,
                TypeId = m[0].TypeId,
                Deadline = m[0].Deadline,
                Publishtime = DateTime.Now,
                Status = m[0].Status,
                Verify = m[0].Verify,
                CanTake = m[0].Main == 1 && m[0].PercentReward == 10000 ? (sbyte)0 : (sbyte)1,
                FixedReward = assignmentViews.Sum(x=>x.FixedReward),
                PercentReward = m[0].PercentReward,
                Rewardtype = m[0].Rewardtype,
                UserId = user.Id,
                Main= m[0].Main,
            });
            _context.SaveChanges();
        }catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("主任务创建失败。");
        }
        assignmentViews.Remove(m[0]);
        List<EntityEntry<Assignment>> asgs = new List<EntityEntry<Assignment>>();
        List<EntityEntry<RelatedAssignment>> rasgs = new List<EntityEntry<RelatedAssignment>>();
        try
        {
            for (int i= 0; i < assignmentViews.Count; i++)
            {
                var e1 = new Assignment
                {
                    Title = assignmentViews[i].Title,
                    Description = assignmentViews[i].Description,
                    Branchid = assignmentViews[i].Branchid,
                    TypeId = assignmentViews[i].TypeId,
                    Deadline = assignmentViews[i].Deadline,
                    Publishtime = DateTime.Now,
                    Status = assignmentViews[i].Status,
                    Verify = assignmentViews[i].Verify,
                    CanTake = assignmentViews[i].CanTake,
                    FixedReward = assignmentViews[i].FixedReward,
                    PercentReward = assignmentViews[i].PercentReward,
                    Rewardtype = assignmentViews[i].Rewardtype,
                    UserId = user.Id,
                };
               var e = _context.Assignments.Add(e1);
               _context.SaveChanges();
               asgs.Add(e);
                var e2 = new RelatedAssignment
                {
                    AssignmentId = entityEntry.Entity.Id,
                    RelatedId = e.Entity.Id
                };
               var re =  _context.Relatedtasks.Add(e2);
                
               rasgs.Add(re);

            }
            _context.SaveChanges();
            transaction.Commit();
        }
        catch(Exception ex1)
        {
            _logger.LogError(ex1.Message);
            transaction.RollbackToSavepoint("before");
            return BadRequest("副任务创建失败。");
        }

        return CreatedAtAction(nameof(PostsAssignment), new { id = entityEntry.Entity.Id,Msg= "创建成功。" });
    }

    /// <summary>
    /// 新建关联任务
    /// </summary>
    /// <param name="id">关联id</param>
    /// <param name="assignmentView"></param>
    /// <returns></returns>
    [HttpPost("contact/{id}")]
    public async Task<ActionResult<Assignment>> ContactAssignment(int id, AssignmentView assignmentView)
    {
        var userid = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        var a = await _context.Assignments.FindAsync(id);
        if(a==null)
        {
            return NotFound($"id为{id}的任务不存在。");
        }
        Assignment assignment = new Assignment
        {
            Title = assignmentView.Title,
            Description = assignmentView.Description,
            Branchid = assignmentView.Branchid,
            TypeId = assignmentView.TypeId,
            Deadline = assignmentView.Deadline,
            Publishtime = DateTime.Now,
            Status = assignmentView.Status,
            Verify = assignmentView.Verify,
            CanTake = assignmentView.CanTake,
            FixedReward = assignmentView.FixedReward,
            PercentReward = assignmentView.PercentReward,
            Rewardtype = assignmentView.Rewardtype,
            UserId = int.Parse(userid),
        };
        try
        {
            var e = _context.Assignments.Add(assignment);
            _context.SaveChanges();
            _context.Relatedtasks.Add(new RelatedAssignment { AssignmentId = e.Entity.Id, RelatedId = id });
            _context.SaveChanges();
        }catch(Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest("创建失败。");
        }

        return Ok();
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
        var userid = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        if (id != assignmentView.Id)
        {
            return BadRequest();
        }

        var oldAssignment = _context.Assignments.Find(id);
        if(oldAssignment == null)
        {
            return NotFound();
        }

        if(oldAssignment.Status != (sbyte)TaskState.WaitForAccept)
        {
            return Conflict(new ResponeMessage<SimpleResp>
            {
                ORCode = ORCode.AsgmHasPicked,
                Data = new SimpleResp
                {
                    Success = false,
                    Reason = "任务已被接取,请等待任务完成或被放弃。"
                }
            });
        }

        Assignment assignment = new Assignment
        {
            Id = assignmentView.Id,
            Title = assignmentView.Title,
            Description = assignmentView.Description,
            Branchid = assignmentView.Branchid,
            TypeId = assignmentView.TypeId,
            Deadline = assignmentView.Deadline,
            Publishtime = assignmentView.Publishtime,
            Status = assignmentView.Status,
            Verify = assignmentView.Verify,
            CanTake = assignmentView.CanTake,
            FixedReward = assignmentView.FixedReward,
            PercentReward = assignmentView.PercentReward,
            Rewardtype = assignmentView.Rewardtype,
            UserId = int.Parse(userid),
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
    /// 同意接取任务
    /// </summary>
    /// <param name="requestId">任务请求id</param>
    /// <returns></returns>
    [HttpGet("take/{requestId}")]
    public async Task<ActionResult<ResponeMessage<SimpleResp>>> Take(int requestId)
    {
        var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
        //var user = await _userManager.FindByIdAsync(id);
        var user = await _context.AuthUsers.FindAsync(Convert.ToInt32(id));
        
        if(user == null)
        {
            return new ResponeMessage<SimpleResp>
            {
                ORCode = ORCode.AsgmHasPicked,
                Data = new SimpleResp
                {
                    Success = false,
                    Reason = "用户异常。"
                }
            };
        }
        var r = _context.TaskRequests.SingleOrDefault(x => x.Id == requestId);
        if (r == null)
        {
            return new ResponeMessage<SimpleResp>
            {
                ORCode = ORCode.RequestNotFound,
                Data = new SimpleResp
                {
                    Success = false,
                    Reason = "任务请求异常。"
                }
            };
        }

        var aus = _context.Assignmentusers
            .Where(x => x.UserId == user.Id)
            .Include(o=>o.Assignment).ToList();

        var c = aus.Select(o => o.Assignment.Status == (sbyte)TaskState.WaitForAccept).Count();
        
        if(c>=1)
        {
            return new ResponeMessage<SimpleResp>
            {
                ORCode = ORCode.AsgmHasPicked,
                Data = new SimpleResp
                {
                    Success = false,
                    Reason = "有待完成的任务，请完成后再接取新任务。"
                }
            };
        }
        var assignmentId = r.TaskId;
        var assignment = _context.Assignments.Find(assignmentId);
        var resp = new ResponeMessage<SimpleResp>();
        if (assignment != null)
        {
            if (assignment.Status == 0)
            {
                assignment.Status = 1;
                _context.Entry(assignment).State = EntityState.Modified;
                _context.Assignmentusers.Add(new AssignmentUser
                {
                    AssignmentId = assignmentId,
                    UserId = user.Id,
                });
                assignment.Status = (sbyte)TaskState.Unfinished;
                r.Agree = 1;
                r.AgreeDate = DateTime.Now;

                _context.SaveChanges();
                return new ResponeMessage<SimpleResp> { 
                    ORCode = ORCode.AsgmTakeS,
                    Data = new SimpleResp
                    {
                        Success = true,
                    },
                    Message = "接取成功"
                };
            }
            return new ResponeMessage<SimpleResp> { 
                ORCode = ORCode.AsgmHasPicked,
                Data = new SimpleResp
                    {
                        Success = false,
                        Reason = "任务已被接取,请刷新页面"
                    }
                };
            }

        return new ResponeMessage<SimpleResp> { 
            ORCode = ORCode.AsgmNotFound,
            Data = new SimpleResp
            {
                Success = false,
                Reason = "任务以不存在，请刷新页面"
            }
        };
    }

    /// <summary>
    /// 根据ID删除任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAssignment(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment == null)
        {
            return NotFound();
        }
        var e = _context.Assignmentusers
            .Where(x => x.AssignmentId == assignment.Id).FirstOrDefault();
        if (e != null)
        {
            return  Conflict( new ResponeMessage<SimpleResp>
            {
                ORCode = ORCode.AsgmHasPicked,
                Data = new SimpleResp
                {
                    Success = false,
                    Reason = "任务已被他人接取,请等待任务完成或被放弃。"
                }
            });
        }

        try
        {
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("删除失败。");
        }


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
