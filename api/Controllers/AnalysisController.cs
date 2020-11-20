using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Hubs.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shared.Models;
using shared.Models.AI;
using shared.Models.Analysis;
using shared.Models.Hub;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    //[Consumes("multipart/form-data", "image/jpg", "image/jpeg", "image/png")]
    public class AnalysisController : Controller
    {
        private readonly UserManager<User> mUserManager;
        private readonly ILogger<AccountController> mLogger;
        private readonly FacemarkDbContext mContext;
        public IAiRepository AiRepository { get; }

        public AnalysisController(
                    UserManager<User> userManager,
                    ILogger<AccountController> logger,
                    FacemarkDbContext context,
                    IAiRepository aiRepository)
        {
            mUserManager = userManager;
            mLogger = logger;
            mContext = context;
            AiRepository = aiRepository;
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Analyze([FromBody] IFormFile image)
        {
            //if (model == null || model.Image == null)
            //{
            //    return BadRequest(new { error = true });
            //}

            //User user = await mUserManager.Users.Where(user => user.Id == model.UserId).FirstOrDefaultAsync();
            //if (user == null)
            //{
            //    return new ForbidResult();
            //}


            //long size = model.Image.Length;
            //// add validation to size

            //if (model.Image.Length > 0)
            //{
            //    using (var stream = new MemoryStream())
            //    {
            //        await model.Image.CopyToAsync(stream);
            //        Order order = new Order(
            //                userId: user.Id,
            //                createdAt: DateTime.UtcNow,
            //                imageData: stream.ToArray(),
            //                dataHeaders: model.Image.ContentType,
            //                orderStatus: EOrderStatus.Accepted,
            //                hubConnectionId: model.HubConnectionId)
            //        {
            //            ModifiedAt = DateTime.UtcNow
            //        };
            //        await mContext.Orders.AddAsync(order);
            //        await mContext.SaveChangesAsync();
            //        await AiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId));
            //    }
            //}

            return StatusCode(202);
        }
    }
}
