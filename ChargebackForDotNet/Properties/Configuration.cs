using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

namespace ChargebackForDotNet.Properties
{
    public class Configuration
    {

        private Dictionary<string, string> configDictionary;

        public Configuration()
        {
            string configPath = Environment.GetEnvironmentVariable("configPath");
            if (configPath == null)
            {
                throw new ChargebackException("No config file. Please set the environment variable 'ConfigPath'" +
                                              " to the location of config file.");
            }

            initializeConfig();
            readAllSettings(configPath);
            validateConfigDictionary();
        }

        public Configuration(string configPath)
        {
            initializeConfig();
            readAllSettings(configPath);
            validateConfigDictionary();
        }

        public Configuration(Dictionary<string, string> config)
        {
            initializeConfig();
            foreach (var key in config.Keys)
            {
                addSettingToConfigDictionary(key, config[key]); 
            }

            validateConfigDictionary();
        }
        
        private void initializeConfig()
        {
            configDictionary = new Dictionary<string, string>();
            configDictionary["username"] = null;
            configDictionary["password"] = null;
            configDictionary["merchantId"] = null;
            configDictionary["host"] = null;
            configDictionary["downloadDirectory"] = null;
            configDictionary["printXml"] = null;
            configDictionary["proxyHost"] = null;
            configDictionary["proxyPort"] = null;
        }


        private void readAllSettings(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ChargebackException("No Configuration file at " + filePath);
            }
            StreamReader reader = new StreamReader(File.OpenRead(filePath));
            string[] settings = reader.ReadToEnd().Split('\n');
            reader.Close();
            foreach (var setting in settings)
            {
                string[] keyValPair = setting.Split(new char[]{'='}, 2);
                if (keyValPair.Length != 2)
                {
                    Console.WriteLine("Warning: '{0}' is not a valid setting[skipped]", setting);
                    continue;
                }           
                
                addSettingToConfigDictionary(keyValPair[0].Trim(), keyValPair[1].Trim());   
            }
        }

        private void addSettingToConfigDictionary(string key, string value)
        {
            if (!configDictionary.ContainsKey(key))
            {
                Console.WriteLine("Warning: '{0}' is not a valid key[skipped]", key);
                return;
            }
            configDictionary[key] = value;
        }

        private void validateConfigDictionary()
        {
            foreach (var key in configDictionary.Keys)
            {
                if (configDictionary[key] == null)
                {
                    throw new ChargebackException(string.Format("Missing value for {0} in config", key));
                }
            }
        }

        public string getConfig(string key)
        {
            return configDictionary[key];
        }

        public void setConfigValue(string key, string value)
        {
            string oldValue = configDictionary[key];
            configDictionary[key] = value;
        }

    }
}