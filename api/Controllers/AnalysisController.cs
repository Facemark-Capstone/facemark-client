using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Hubs.Repository;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shared.Models;
using shared.Models.AI;
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
        private readonly IQueueService<Order> mOrderQueue;

        public IAiRepository AiRepository { get; }

        public AnalysisController(
                    UserManager<User> userManager,
                    ILogger<AccountController> logger,
                    FacemarkDbContext context,
                    IAiRepository aiRepository,
                    IQueueService<Order> orderQueue)
        {
            mUserManager = userManager;
            mLogger = logger;
            mContext = context;
            AiRepository = aiRepository;
            mOrderQueue = orderQueue;
        }


        [HttpPost]
        [Route("analyze")]
        public async Task<IActionResult> Analyze()
        {
            Order order;
            mLogger.LogInformation($"{Request.Headers["user-id"]}{Request.Headers["hub-id"]}");

            if (string.IsNullOrWhiteSpace(Request.Headers["user-id"])
                //|| string.IsNullOrWhiteSpace(Request.Headers["content-type"])
                || string.IsNullOrWhiteSpace(Request.Headers["hub-id"]))
            {
                return BadRequest(new { error = true });
            }

            User user = await mUserManager.Users.Where(user => user.Id == Request.Headers["user-id"].ToString()).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ForbidResult();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(stream);
                    order = new Order(
                            userId: user.Id,
                            createdAt: DateTime.UtcNow,
                            imageData: stream.ToArray(),
                            dataHeaders: Request.Headers["content-type"],
                            orderStatus: EOrderStatus.Accepted,
                            hubConnectionId: Request.Headers["hub-id"])
                    {
                        ModifiedAt = DateTime.UtcNow
                    };

                    await mContext.Orders.AddAsync(order);
                    await mContext.SaveChangesAsync();
                    mOrderQueue.Enqueue(order);
                    await AiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId, new OrderResult()));
                }
                return Accepted(new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Database error: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
