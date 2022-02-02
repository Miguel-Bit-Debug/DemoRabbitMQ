using DemoQueue.Models;
using DemoQueue.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoQueue.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IQueueService _service;

        public MessagesController(IQueueService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult PostMessage([FromBody] MessageInputModel message)
        {
            var success = _service.PostMessage(message);
            return Ok(success);
        }
    }
}
