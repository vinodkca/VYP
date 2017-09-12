using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Todo.Models;


//Install-Package Microsoft.EntityFrameworkCore.InMemory -Version 2.0.0

//$PSVersionTable.PSVersion
namespace Todo
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
            //string conn = "";
            //var conn = @"Server=(localdb)\MSSQLLocalDB;Database=MyCodeCampDb;Trusted_Connection=True;";
            //services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase(conn));
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddMvc();

            //Register the Swagger gnerator , define one or more swagger documents
            services.AddSwaggerGen(c =>
               {
                   c.SwaggerDoc("v1", new Info
                   {
                       Version = "V1.2",
                       Title = "Todo Web API",
                       Description = "CRUD using ASP.NET Core Web API",
                       TermsOfService = "None",
                       Contact = new Contact { Name = "VK", Email = "", Url = "" },
                       License = new License { Name = "", Url = "" }
                   });


                   //Exmaple of Multiple Swagger docs
                   c.SwaggerDoc("v2", new Info
                   {
                       Version = "V2.2",
                       Title = "Todo Web API - Future",
                       Description = "ASP.NET Core Web API - Doc2",
                       TermsOfService = "None",
                       Contact = new Contact { Name = "VK", Email = "", Url = "" },
                       License = new License { Name = "", Url = "" }
                   });


                   //Set the path of comments for xml fpr swagger ui
                   var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                   var xmlPath = Path.Combine(basePath, "Todo.xml");
                   c.IncludeXmlComments(xmlPath);

                   //Multiple versions of api method



               });


            //Multiple versions of api method //http://www.talkingdotnet.com/support-multiple-versions-of-asp-net-core-web-api/

            //services.AddApiVersioning();
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0); //ApiVersion(1, 0) for 1.1
                //o.ApiVersionReader = new HeaderApiVersionReader("api-version", "x-api-version"); //specify custome header api-version  or x-api-version or ver


                //told my API that the header “x-api-version” is now how I define an API version.//https://dotnetcoretutorials.com/2017/01/17/api-versioning-asp-net-core/
                //o.ApiVersionReader = new QueryStringOrHeaderApiVersionReader("x-api-version"); //set the version reader to use the header, you can no longer specify the version like so /api/home?api-version=2.0

                //svc?api-version=1.0
                o.ApiVersionReader = ApiVersionReader.Combine(
                                                               new QueryStringApiVersionReader("api-version"),
                                                               new HeaderApiVersionReader()
                                                               {
                                                                   HeaderNames = { "api-version", "x-api-version" }
                                                               });
            });


            //Disable ApplicationInsight
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.EnableDebugLogger = false;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            // loggerFactory.AddDebug();


            //Enable static files for Swagger UI custom menu index.html
            //download index.html from 
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();





            //Enable middleware to generate swagger as a json end point
            app.UseSwagger();

            //Enable moddle ware to server swagger-ui(HTMl, CSS, JS) specify the json doc
            //http://localhost:60592/swagger/v1/swagger.json
            // Ui - http://localhost:60592/swagger
            app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Project API");
               //Example of Multiple Swagger docs
               c.SwaggerEndpoint("/swagger/v2/swagger2.json", "Todo Project API - Future");

               //c.RoutePrefix = "swagger/ui"; //"documents"

               //CUSTOMIZATION OF SWAGGER UI
               c.RoutePrefix = "api-docs"; //changes the default help url from /swagger to /api-docs
                                           //c.DocumentTitle("MY UI"); //Not support yet


               //c.EnabledValidator();
               //c.BooleanValues(new object[] { 0, 1 });
               //c.DocExpansion("full");
               //c.SupportedSubmitMethods(new[] { "get", "post", "put", "patch" });
               //c.SupportedSubmitMethods(new[] { "post", "put", "patch" }); //TRY IT OUT button
               //c.InjectOnCompleteJavaScript("/ext/custom-script.js");
               //c.InjectStylesheet("/ext/custom-stylesheet.css");              
               c.InjectStylesheet("/swagger/ui/customvk.css");

           });


        }
    }
}
