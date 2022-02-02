using DemoQueue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoQueue.Services
{
    public interface IQueueService
    {
        Task<bool> PostMessage(MessageInputModel message);
    }
}
