using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

namespace ChargebackForDotNet.Properties
{
    public class Configuration
    {
        private Dictionary<string, string> _configDictionary;

        public Configuration()
        {
            string configPath = Environment.GetEnvironmentVariable("chargebackConfigPath");
            if (configPath == null)
            {
                throw new ChargebackException("No configuration file." +
                                              " Please set the environment variable 'chargebackConfigPath'" +
                                              " to the location of config file.");
            }

            InitializeConfig();
            ReadAllSettings(configPath);
            ValidateConfigDictionary();
        }

        public Configuration(string configPath)
        {
            InitializeConfig();
            ReadAllSettings(configPath);
            ValidateConfigDictionary();
        }

        public Configuration(Dictionary<string, string> config)
        {
            InitializeConfig();
            foreach (var key in config.Keys)
            {
                AddSettingToConfigDictionary(key, config[key]); 
            }

            ValidateConfigDictionary();
        }

        public string Get(string key)
        {
            return _configDictionary[key];
        }

        public void Set(string key, string value)
        {
            string oldValue = _configDictionary[key];
            _configDictionary[key] = value;
        }
        
        private void InitializeConfig()
        {
            _configDictionary = new Dictionary<string, string>();
            _configDictionary["username"] = null;
            _configDictionary["password"] = null;
            _configDictionary["merchantId"] = null;
            _configDictionary["host"] = null;
//            _configDictionary["downloadDirectory"] = null;
            _configDictionary["printXml"] = null;
            _configDictionary["proxyHost"] = null;
            _configDictionary["proxyPort"] = null;
        }

        private void ReadAllSettings(string filePath)
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
                
                AddSettingToConfigDictionary(keyValPair[0].Trim(), keyValPair[1].Trim());   
            }
        }

        private void AddSettingToConfigDictionary(string key, string value)
        {
            if (!_configDictionary.ContainsKey(key))
            {
                Console.WriteLine("Warning: '{0}' is not a valid key[skipped]", key);
                return;
            }
            _configDictionary[key] = value;
        }

        private void ValidateConfigDictionary()
        {
            foreach (var key in _configDictionary.Keys)
            {
                if (_configDictionary[key] == null)
                {
                    throw new ChargebackException(string.Format("Missing value for {0} in config", key));
                }
            }
        }

    }
}