﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace SitecoreBlazorHosted.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression();
            services.AddHttpClient();

            //For server-side
            //services.AddRazorComponents();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //For server-side
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseFileServer(new FileServerOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //       Path.Combine(
            //           $@"{Directory.GetParent(Directory.GetCurrentDirectory())}\SitecoreBlazorHosted.Client",
            //           @"wwwroot"
            //           )
            //      ),
            //    RequestPath = new PathString("")
            //});

            //app.UseRouting(routes =>
            //{
            //    routes.MapRazorPages();
            //    routes.MapComponentHub<Client.App>("app");
            //});


            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });

            //For client-side
            app.UseBlazor<Client.Startup>();
            app.UseBlazorDebugging();


        }
    }
}
