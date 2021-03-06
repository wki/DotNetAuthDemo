﻿using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Auth2Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // must use TryAddSingleton for all dependencies being
            // mocked in Tests.
            services.TryAddSingleton<IUserRepository, UserRepository>();

            services.AddAuthenticationCore(options =>
            {
                options.AddScheme<BasicAuthenticationHandler>("Basic", "Name");
                options.DefaultAuthenticateScheme = "Basic";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "Users",
                    builder =>
                    {
                        builder.AddAuthenticationSchemes("Basic");
                        builder.RequireClaim(ClaimTypes.Name);
                    });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}