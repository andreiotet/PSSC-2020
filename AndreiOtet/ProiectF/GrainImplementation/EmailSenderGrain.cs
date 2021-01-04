using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainImplementation
{
    public class EmailSenderGrain : Orleans.Grain, IEmailSender
    {
        private readonly ILogger logger;

        public EmailSenderGrain(ILogger<EmailSenderGrain> logger)
        {
            this.logger = logger;
        }

        async Task<string> IEmailSender.SendEmailAsync(string message)
        {
            IAsyncStream<string> stream = this.GetStreamProvider("SMSProvider").GetStream<string>(Guid.Empty, "chat");
            await stream.OnNextAsync($"{this.GetPrimaryKeyString()} - {message}");


            logger.LogInformation($"\n Message received: greeting = '{message}'");
            return ($"\n Client said: '{message}'!");
        }
    }
}
