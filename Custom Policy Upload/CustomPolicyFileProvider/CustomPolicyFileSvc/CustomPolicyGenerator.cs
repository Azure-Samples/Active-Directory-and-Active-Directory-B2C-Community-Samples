using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace CustomPolicyFileSvc
{
    [Cmdlet(VerbsLifecycle.Invoke, "GenerateCustomPolicyFiles")]
    public class GenerateCustomPolicyFiles : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string CPConfigurationFilePath { get; set; }
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public string PolicyFilesFolder { get; set; }
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        public string DestinationFolder { get; set; }


        protected override void ProcessRecord()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PolicyFilesFolder) || !Directory.Exists(PolicyFilesFolder))
                {
                    this.WriteWarning("Cannot find Policy Files Folder.");
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

        private Dictionary<string, string> ReadConfigFile(string configFilePath)
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
