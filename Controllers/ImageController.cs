﻿#define linux
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.state;
using FlowLabourApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly FlowContext _context;

        public ImageController(FlowContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根据文件md5获取图片
        /// </summary>
        /// <param name="names">文件md5</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponeMessage<Image>>>> GetImage(IEnumerable<string> names)
        {
            var imgs = await _context.Images.Where(e=> names.Contains(e.Md5)).ToListAsync();
            List<ResponeMessage<Image>> responeMessages = new List<ResponeMessage<Image>>();
            foreach (var img in imgs)
            {
                responeMessages.Add(new ResponeMessage<Image>()
                {
                    ORCode = 200,
                    Data = img
                });
            }
            return responeMessages;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="files">图片文件</param>
        /// <param name="pub">是否公开</param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<ActionResult<IEnumerable<Image>>> Upload(IEnumerable<IFormFile> files)
        {
            if(files==null||files.Count()==0)
                files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            List<Image> images = new List<Image>();
            foreach (IFormFile formFile in files)
            {
                var supportedMediaTypes = new[] { "image/jpeg", "image/png", "application/pdf", "application/msword","text/plain" };

                if (!supportedMediaTypes.Contains(formFile.ContentType.ToLowerInvariant()))
                {
                    return BadRequest(new { errMsg = $"The file {formFile.FileName} is of unsupported format." });
                }

                if (formFile.Length > 5*1024*1024)
                {
                    return BadRequest(new { errMsg = $"The file {formFile.FileName} exceeds the maximum size of 2 MB." });
                }

                var fileName = Path.GetFileName(formFile.FileName);
                var openstream = formFile.OpenReadStream();
                var md5 = HashUtil.CalculateMD5(openstream);
                var newName = md5 + "_" + fileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/flow/static", newName);

                Image image = new Image()
                {
                    Md5 = md5,
                    Url = "/flow/static/"+newName,
                };
                var msg = Create(image);
                images.Add(image);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
            return CreatedAtAction(nameof(Upload),images);
        }

        /// <summary>
        /// 上传图片，非公开
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("pupload")]
        public async Task<ActionResult<IEnumerable<PrivateImage>>> PUpload(IEnumerable<IFormFile> files)
        {
            if (files == null || files.Count() == 0)
                files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            List<PrivateImage> images = new List<PrivateImage>();

            List<PrivateImage> results = new List<PrivateImage>();

            foreach (IFormFile formFile in files)
            {
                var supportedMediaTypes = new[] { "image/jpeg", "image/png", "application/pdf", "application/msword", "text/plain" };

                if (!supportedMediaTypes.Contains(formFile.ContentType.ToLowerInvariant()))
                {
                    return BadRequest(new { errMsg = $"The file {formFile.FileName} is of unsupported format." });
                }

                if (formFile.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { errMsg = $"The file {formFile.FileName} exceeds the maximum size of 2 MB." });
                }

                var fileName = Path.GetFileName(formFile.FileName);
                var openstream = formFile.OpenReadStream();
                var md5 = HashUtil.CalculateMD5(openstream);
                var newName = md5 + "_" + fileName;
                string filePath;
#if win
                    filePath = Path.Combine("D:\\LocalPrivateWebData\\flow\\image", newName);
#else
                    filePath = Path.Combine("/usr/local/xxp/PrivateWebData/flow/image", newName);
#endif

                PrivateImage image = new PrivateImage()
                {
                    Md5 = md5,
                    Url = newName,
                };
                var img = _context.PrivateImages.Where(e => e.Md5 == image.Md5).FirstOrDefault();
                if (img != null)
                {
                    results.Add(img);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
                else
                {
                    results.Add(image);
                    images.Add(image);
                }
                

            }

            _context.PrivateImages.AddRange(images);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Upload), results);
        }

        [NonAction]
        public ResponeMessage<Image> Create(Image image)
        {
            var img = _context.Images.Where(e => e.Md5 == image.Md5).FirstOrDefault();
            if (img != null)
            {
                return new ResponeMessage<Image>()
                {
                    ORCode = 200,
                    Data = img
                };
            }

            EntityEntry<Image>? e = _context.Images.Add(image);
            try
            {
                _context.SaveChanges();
                Console.WriteLine(e.State);
                if (e.State == EntityState.Unchanged)
                {
                    return new ResponeMessage<Image>()
                    {
                        ORCode = 200,
                        Data = image
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Insert operation failed. Unique constraint violation.");
            }

            return new ResponeMessage<Image>()
            {
                ORCode = 500,
                Message = "文件上传失败",
                Data = null
            };

        }

    }
}
