﻿using BookStoreManager.Admin;
using BookStoreManager.Books;
using BookStoreManager.User;
using BookStoreRepository.Admin;
using BookStoreRepository.Books;
using BookStoreRepository.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAdminManager, AdminManager>();
            services.AddTransient<IAdminRepo, AdminRepo>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IUserRepo, UserRepo>();
            services.AddTransient<IBooksManager, BooksManager>();
            services.AddTransient<IBooksRepo, BooksRepo>();
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader();

            }));
            //services.AddSession();
            //services.AddDistributedMemoryCache();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Book Store App", Version = "v1", Description = "Book store Application" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                { "Bearer", new string[] {} }
              });
            });
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseSession();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store Api");
            });
            app.UseCors(x => x.AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true) // allow any origin
                  .AllowCredentials()
                  .AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
