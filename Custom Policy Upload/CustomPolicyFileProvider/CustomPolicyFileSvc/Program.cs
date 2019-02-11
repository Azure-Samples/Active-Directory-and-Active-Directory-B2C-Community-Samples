using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPolicyFileSvc
{
    class Program
    {
        public static string CPConfigurationFilePath { get; set; } = @"C:\MV\LAB\25-01-2018\CustomPolicies\CustomPoliciesConfig.SapUat.json";
        public static string PolicyFilesFolder { get; set; } = @"C:\MV\LAB\25-01-2018\CustomPolicies\ROCP";
        public static string DestinationFolder { get; set; } = @"C:\MV\LAB\25-01-2018\CustomPolicies\Merged";
        static void Main()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PolicyFilesFolder) || !Directory.Exists(PolicyFilesFolder))
                {
                   // this.WriteWarning("Cannot find Policy Files Folder.");
                }
                else
                {
                    Dictionary<string, string> policiesConfig = ReadConfigFile(CPConfigurationFilePath);

                    string[] policyFiles = Directory.GetFiles(PolicyFilesFolder, "*.xml");
                    foreach (string policyFile in policyFiles)
                    {
                        string fileContent = File.ReadAllText(policyFile);
                        foreach (KeyValuePair<string, string> setting in policiesConfig)
                        {
                            fileContent = fileContent.Replace($"%%{setting.Key}", setting.Value);
                        }

                        string environment = "environment";
                        policiesConfig.TryGetValue("Environment", out environment);
                        string outFilePath = Path.Combine(DestinationFolder, $"{Path.GetFileNameWithoutExtension(policyFile)}.{environment}.xml");
                        File.WriteAllText(outFilePath, fileContent);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                }

                throw ex;
            }
        }
        private static Dictionary<string, string> ReadConfigFile(string configFilePath)
        {
            if (string.IsNullOrWhiteSpace(configFilePath) || !File.Exists(configFilePath))
            {
                throw new ArgumentException("ConfigFile is null or it does not exist");
            }

            try
            {
                string configFile = File.ReadAllText(configFilePath);
                Dictionary<string, string> properties = new Dictionary<string, string>();
                properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(configFile);

                return properties;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
