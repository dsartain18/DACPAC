using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

using Microsoft.Data.Tools.Schema;
using Microsoft.SqlServer.Dac;
using System.IO;

namespace RetrieveDatabases
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDatabase();

            Console.ReadLine();
        }

        private static void CreateDatabase()
        {
            //DacPackage x = DacPackage.Load(@"C:\Users\Don\Documents\SQL Server Management Studio\DAC Packages\EventRegistration.dacpac");

            try
            {
                DacDeployOptions options = new DacDeployOptions();
                options.CreateNewDatabase = true;

                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

                DacServices dacServices = new DacServices(connectionString);

                Stream dacPackage = new MemoryStream();

                dacServices.Extract(dacPackage, "EventRegistration", "EventRegistration", new Version("1.0.0.0"));

                DacPackage x = DacPackage.Load(dacPackage);

                dacServices.Deploy(x, "EventRegistration2");


                //string script = DacServices.GenerateCreateScript(x, "EventRegistration2", null);



                Console.WriteLine("DAC Created");
            }
            catch (Exception ex)
            {

            }

        }

        private static List<string> GetDatabases()
        {
            List<string> list = new List<string>();

            // Open connection to the database
            string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases WHERE owner_sid <> 0x01", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }

            return list;
        }
    }
}
