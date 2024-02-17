using Magicube.Net.Email;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Magicube.Net.Test {
    public class EmailTest {
        private readonly IServiceProvider ServiceProvider;
             
        public EmailTest() {
            var services = new ServiceCollection()
                .AddEmailServices(x => { 
                    
                });

            ServiceProvider = services.BuildServiceProvider();
        }

        public async Task Func_SendEmail() {
            var smtpService = ServiceProvider.GetService<IEmailSender>();


            await Task.CompletedTask;
        }
    }
}
