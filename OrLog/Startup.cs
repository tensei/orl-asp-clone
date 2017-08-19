using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OrLog
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        //private void Log()
        //{
        //    var ip = HttpContext.Request.Headers?["CF-Connecting-IP"] ??
        //             HttpContext.Connection?.RemoteIpAddress.ToString();
        //    Console.WriteLine($"{HttpContext.Request.Method} {HttpContext.Request?.Host} - {HttpContext.Request?.Path}{HttpContext.Request.QueryString.Value} - {ip} in {elapsed}");
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.Use(async (context, next) =>
            {
                var watch = new Stopwatch();
                watch.Start();
                var ip = context.Request.Headers?["CF-Connecting-IP"] ??
                         context.Connection?.RemoteIpAddress.ToString();
                await next.Invoke();
                watch.Stop();
                Console.WriteLine($"{context.Request.Method} {context.Request?.Host} - {context.Request?.Path}{context.Request.QueryString.Value} - {ip} in {watch.ElapsedMilliseconds}");
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/");
            }
            app.UseStatusCodePagesWithRedirects("/");
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
