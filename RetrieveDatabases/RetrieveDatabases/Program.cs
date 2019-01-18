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
            try
            {
                // Create DacDeployOptions
                DacDeployOptions options = new DacDeployOptions();
                options.CreateNewDatabase = true;

                // Specify Connection string for Source DB
                string sourceConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

                // Create DacServices to access Source DB
                DacServices sourceDacServices = new DacServices(sourceConnectionString);

                // Create memory object to hold the DACPAC package vs saving to file system
                Stream dacPackage = new MemoryStream();

                // Extract database into DACPAC memory object

                //NOTE: The second "EventRegistration" is the application name. I'm not sure how this is really used in the creation of the DACPAC. Same with the version.
                sourceDacServices.Extract(dacPackage, "EventRegistration", "EventRegistration", new Version("1.0.0.0"));

                // Load DACPAC memory object into an actual DACPAC typed object
                DacPackage dacPac = DacPackage.Load(dacPackage);

                // This version will create a NEW database with a new name, provided in quotes, on the same DB Server as the Source DB because we are using the same DacServices instance.
                sourceDacServices.Deploy(dacPac, "EventRegistration2");

                // This will give you the text version of the script in the event you want to save the script to a file and run the changes manually or have a written record of the changes that occurred.
                //string script = DacServices.GenerateCreateScript(x, "EventRegistration2", null);


                /*************** I have provided the below as an example only. I haven't been able to test this yet. ********************/
                // To update an existing database you will need to create a new DacServices instance with the Destination Connection String
                string destConnectionString = "";
                DacServices destDacServices = new DacServices(destConnectionString);

                // Deploy to destination
                destDacServices.Deploy(dacPac, "EventRegistration", true, options);
                /*********************************************/

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
