// David Wahid
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using api.Data;
using api.Hubs.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using shared.Models.AI;
using shared.Models.Hub;

namespace api.Services
{
    public class ModelQueueWorker : BackgroundService
    {
        private readonly IQueueService<Order> _orderQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ModelQueueWorker> _logger;
        private readonly IAiRepository _aiRepository;

        public ModelQueueWorker(
            IQueueService<Order> orderQueue,
            IServiceScopeFactory scopeFactory,
            ILogger<ModelQueueWorker> logger,
            IAiRepository aiRepository)
        {
            _orderQueue = orderQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _aiRepository = aiRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ModelQueueWorker)} is now running in the background...");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100, stoppingToken);
                var order = _orderQueue.Dequeue();
                if (order == null) continue;

                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var _clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                        var _context = scope.ServiceProvider.GetRequiredService<FacemarkDbContext>();


                        order.OrderStatus = EOrderStatus.Running;
                        order.ModifiedAt = DateTime.UtcNow;
                        _context.Orders.Update(order);
                        await _aiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId, new OrderResult()));

                        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:6000/internal-api/model/analyze");
                        var client = _clientFactory.CreateClient();

                        request.Content = new StreamContent(new MemoryStream(order.ImageData));
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                        HttpResponseMessage response = await client.SendAsync(request);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"Model API returned {response.StatusCode} : {response.ReasonPhrase} at {nameof(ModelQueueWorker)}");

                            order.OrderStatus = EOrderStatus.Failed;
                            order.ModifiedAt = DateTime.UtcNow;
                            _context.Orders.Update(order);
                            await _aiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId, new OrderResult()));
                            continue;
                        }

                        if (response.Content == null)
                        {
                            _logger.LogError($"Content from model request is empty for order ID: {order.Id} at {nameof(ModelQueueWorker)}");

                            order.OrderStatus = EOrderStatus.Failed;
                            order.ModifiedAt = DateTime.UtcNow;
                            _context.Orders.Update(order);
                            await _aiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId, new OrderResult()));

                            continue;
                        }

                        var responseStream = await response.Content.ReadAsStringAsync();
                        //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var result = JsonConvert.DeserializeObject<OrderResult>(responseStream);

                        if (result == null)
                        {
                            _logger.LogError($"Result from model is not a proper JSON: {result} at {nameof(ModelQueueWorker)}");

                            order.OrderStatus = EOrderStatus.Failed;
                            order.ModifiedAt = DateTime.UtcNow;
                            _context.Orders.Update(order);
                            await _aiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId, new OrderResult()));

                            continue;
                        }

                        order.OrderStatus = EOrderStatus.Completed;
                        order.ModifiedAt = DateTime.UtcNow;
                        _context.Orders.Update(order);
                        await _aiRepository.UpdateOrderStatus(new UpdateOrderStatusModel(order.Id, order.OrderStatus, order.HubConnectionId, result));
                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception e)
                {
                    _logger.LogCritical($"An error occured while processing a model order at {nameof(ModelQueueWorker)}: Exception --- {e.Message}");
                }
            }
        }
    }
}
