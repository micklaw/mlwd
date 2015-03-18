﻿using System.Configuration;
using MLWD.Umbraco.Context;
using MLWD.Umbraco.Umbraco.Services.Container;

namespace MLWD.Umbraco
{
    public class App : CoreApp<ServiceContainer>
    {
        #region Web Config Settings

        public class WebConfig
        {
            private WebConfig() { }

            private WebConfig _current { get; set; }

            public WebConfig Current
            {
                get
                {
                    if (_current == null)
                    {
                        _current = new WebConfig();
                    }

                    return _current;
                }
            }
                
            public string GetString(string key)
            {
                return ConfigurationManager.AppSettings[key];
            }

            public int GetInt(string key)
            {
                int value;
                int.TryParse(GetString(key), out value);

                return value;
            }

            public bool GetBool(string key)
            {
                bool value;
                bool.TryParse(GetString(key), out value);

                return value;
            }
        }

        #endregion Web Config Settings
    }
}