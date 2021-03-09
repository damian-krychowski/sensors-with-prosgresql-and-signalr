using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SensorsHub
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/send-sensor-data/{count:int}", async context =>
                {
                    await HandleSendingSensorData(context);
                });

                endpoints.MapHub<SensorsHub>("/sensorsHub");
            });
        }

        private static async Task HandleSendingSensorData(HttpContext context)
        {
            var sensorsHubContext = context
                .RequestServices
                .GetRequiredService<IHubContext<SensorsHub>>();

            var count = int.Parse(context.Request.RouteValues["count"].ToString());

            for (var i = 0; i < count; i++)
            {
                var randomSensorData = RandomSensorData
                    .Create();

                await sensorsHubContext
                    .Clients
                    .All
                    .SendAsync("SensorData", randomSensorData);
            }

            await context.Response.WriteAsync($"Number of performed SignalR requests: {count}");
        }
    }
}
