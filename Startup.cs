using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
	public class Startup
	{
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment  env)
		{
			Configuration = configuration;
			_env = env;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder.AllowAnyOrigin()
						   .AllowAnyHeader()
						   .AllowAnyMethod();
				});
			});
			services.AddSingleton<IWebHostEnvironment>(_env);


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseHsts();
			}
		//	app.UseMiddleware<HeaderValidationMiddleware>("1234567888");

			app.UseStaticFiles();
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();
		

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}

	public class HeaderValidationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _expectedHeaderValue;

		public HeaderValidationMiddleware(RequestDelegate next, string expectedHeaderValue)
		{
			_next = next;
			_expectedHeaderValue = expectedHeaderValue;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Check if the expected header exists and has the correct value
			if (context.Request.Headers.TryGetValue("key", out var headerValues) &&
				headerValues.Contains(_expectedHeaderValue))
			{
				// If the header is valid, continue with the request pipeline
				await _next(context);
			}
			else
			{
				// If the header is invalid, return a forbidden response or handle it as needed
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync("Forbidden");
			}
		}
	}
}
