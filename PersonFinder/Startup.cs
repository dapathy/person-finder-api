using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

namespace PersonFinder
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
			// Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddMicrosoftWebApi(options =>
					{
						Configuration.Bind("AzureAdB2C", options);

						options.TokenValidationParameters.NameClaimType = "name";
					},
					options => { Configuration.Bind("AzureAdB2C", options); });

			services.AddControllers();
			services.AddAuthorization();

			// Require auth on all routes.
			services.AddMvc(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			ApplySecureHeaders(app);

			app.UseHttpsRedirection();

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private static void ApplySecureHeaders(IApplicationBuilder app)
		{
			var policies = new HeaderPolicyCollection()
				.AddFrameOptionsDeny()
				.AddXssProtectionBlock()
				.AddContentTypeOptionsNoSniff()
				.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365) // maxage = one year in seconds
				.AddReferrerPolicyStrictOriginWhenCrossOrigin()
				.RemoveServerHeader()
				.AddContentSecurityPolicy(builder =>
				{
					builder.AddObjectSrc().None();
					builder.AddFormAction().Self();
					builder.AddFrameAncestors().None();
				});

			app.UseSecurityHeaders(policies);
		}
	}
}
