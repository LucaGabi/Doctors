using ODATAT.Data.DB;
using OdataToEntity;
using OdataToEntity.EfCore;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ODATAT
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Create adapter data access, where OrderContext your DbContext
            await TestODataQuery();

            Console.ReadKey();
        }

        private static async Task TestODataQuery()
        {
            var dataAdapter = new OeEfCoreDataAdapter<AppDB>();
            //Create query parser
            var parser = new OeParser(new Uri("http://dummy"), dataAdapter.BuildEdmModelFromEfCoreModel());
            //Query
            var uri = new Uri("http://dummy/Doctors?$filter=DoctorSpecializations/any()&$expand=DoctorSpecializations($select=SpecializationId,Specialization;$expand=Specialization($select=Name))&$select=Name");
            //The result of the query
            var response = new MemoryStream();
            //Execute query
            await parser.ExecuteGetAsync(uri, OeRequestHeaders.JsonDefault, response, CancellationToken.None);
            var s = Encoding.ASCII.GetString(response.ToArray());
            Console.WriteLine(s);
        }
    }
}
