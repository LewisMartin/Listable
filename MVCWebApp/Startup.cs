using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Listable.MVCWebApp.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Listable.MVCWebApp
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
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect(options => 
            {
                Configuration.GetSection("AzureAd").Bind(options);

                options.Events = new OpenIdConnectEvents
                {
                    OnAuthorizationCodeReceived = async ctx => 
                    {
                        var request = ctx.HttpContext.Request; ;
                        var currentUri = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path);
                        var credential = new ClientCredential(ctx.Options.ClientId, ctx.Options.ClientSecret);

                        var distributedCache = ctx.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                        string userId = ctx.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

                        var cache = new AdalDistributedTokenCache(distributedCache, userId);

                        var authContext = new AuthenticationContext(ctx.Options.Authority, cache);

                        var collectionAPIResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                            ctx.ProtocolMessage.Code, new Uri(currentUri), credential, Configuration["CollectionAPI:Resource"]);

                        ctx.HandleCodeRedemption(collectionAPIResult.AccessToken, collectionAPIResult.IdToken);

                        var blobAPIResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                            ctx.ProtocolMessage.Code, new Uri(currentUri), credential, Configuration["BlobServiceAPI:Resource"]);

                        ctx.HandleCodeRedemption(blobAPIResult.AccessToken, blobAPIResult.IdToken);
                    }
                };
                options.ResponseType = "code id_token";
            })
            .AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
