using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ODATAT.Data.DB;
using OdataToEntity;
using OdataToEntity.EfCore;
using Microsoft.AspNetCore.Http.Extensions;
using OdataToEntity.AspNetCore;

namespace odatahttp
{
    public class Startup
    {
        private static OeParser _parser;

        private static async Task<string> testODataQuery(string url)
        {
            
            //Query
            //var uri = new Uri("http://odata/Doctors?$filter=DoctorSpecializations/any()&$expand=DoctorSpecializations($select=SpecializationId,Specialization;$expand=Specialization($select=Name))&$select=Name");
            var uri = new Uri(url);
            //The result of the query
            var response = new MemoryStream();
            //Execute query
            await _parser.ExecuteGetAsync(uri, OeRequestHeaders.JsonDefault, response, CancellationToken.None);
            var s = Encoding.ASCII.GetString(response.ToArray());
            Console.WriteLine(s);
            return s;
        }

        private static OeParser InitParser()
        {
            var dataAdapter = new OeEfCoreDataAdapter<AppDB>();
            //Create query parser
            var parser = new OeParser(new Uri("http://localhost:5000/odata/"), dataAdapter.BuildEdmModelFromEfCoreModel());
            return parser;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

#pragma warning disable CS4014

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            _parser = InitParser();
            app.UseOdataToEntityMiddleware("/odata",_parser.EdmModel);

            app.Run(async (context) =>
            {
                var url = context.Request.GetEncodedUrl();
                if (!url.ToLower().Contains("odata/")) return;
                var result = await testODataQuery(url);
                context.Response.WriteAsync(result);
            });
        }
    }
}

#pragma warning restore CS4014
