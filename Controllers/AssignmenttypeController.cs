using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FlowLabourApi.Controllers
{
    /// <summary>
    /// �������ͣ��������������裬�ȵȡ�
    /// </summary>
    [Route("api/[controller]")]
    public class AssignmenttypeController : Controller
    {
        private readonly XiangxpContext _context;

        public AssignmenttypeController(XiangxpContext context)
        {
            _context = context;
        }

        // GET: api/Assignmenttype
        /// <summary>
        /// ��ȡ�����������͡�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Assignmenttype> GetAssignmenttypes()
        {
            return _context.Assignmenttypes.ToList();
        }

        // GET: api/Assignmenttype/5
        /// <summary>
        /// ����id��ȡ�������͡�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetAssignmenttype([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignmenttype = _context.Assignmenttypes.SingleOrDefault(m => m.Id == id);

            if (assignmenttype == null)
            {
                return NotFound();
            }

            return Ok(assignmenttype);
        }

        // PUT: api/Assignmenttype/5
        /// <summary>
        /// ����id�����������͡�
        /// </summary>
        /// <param name="id"></param>
        /// <param name="assignmenttype"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult UpdateAssignmenttype([FromRoute] int id, [FromBody] Assignmenttype assignmenttype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != assignmenttype.Id)
            {
                return BadRequest();
            }

            _context.Entry(assignmenttype).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmenttypeExists(id))
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

        // POST: api/Assignmenttype
        /// <summary>
        /// ����һ���������͡�
        /// </summary>
        /// <param name="assignmenttype"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateAssignmenttype([FromBody] Assignmenttype assignmenttype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Assignmenttypes.Add(assignmenttype);
            _context.SaveChanges();

            return CreatedAtAction("GetAssignmenttype", new { id = assignmenttype.Id }, assignmenttype);
        }

        // DELETE: api/Assignmenttype/5
        /// <summary>
        /// ����idɾ���������͡�
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteAssignmenttype([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignmenttype = _context.Assignmenttypes.SingleOrDefault(m => m.Id == id);
            if (assignmenttype == null)
            {
                return NotFound();
            }

            _context.Assignmenttypes.Remove(assignmenttype);
            _context.SaveChanges();

            return Ok(assignmenttype);
        }

        private bool AssignmenttypeExists(int id)
        {
            return _context.Assignmenttypes.Any(e => e.Id == id);
        }
    }
}
