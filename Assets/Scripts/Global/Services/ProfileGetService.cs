﻿using System;
using System.Linq;
using Global.ConfigTemplate;
using UnityEngine;
using Zenject;

namespace Global.Services {
    public class ProfileGetService : IInitializable {
        private Profile _profile;
        
#if UNITY_EDITOR
        private readonly EditorCheckersConfig _editorConfig;
        
        public ProfileGetService(EditorCheckersConfig editorConfig) {
            _editorConfig = editorConfig;
        }
#endif

        public Profile GetProfile() {
            return _profile;
        }

        public void Initialize() {
            _profile = new Profile();
            
#if UNITY_EDITOR
            _profile.UserId = _editorConfig.UserId;
            _profile.FirstName = _editorConfig.FirstName;
            _profile.LastName = _editorConfig.LastName;
            _profile.Username = _editorConfig.Username;
#elif UNITY_WEBGL
            UnityEngine.Debug.Log(Location.href());

            _profile.UserId = GetQueryParam("id");
            _profile.FirstName = GetQueryParam("first_name");
            _profile.LastName = GetQueryParam("last_name");
            _profile.Username = GetQueryParam("username");
#endif
        }

#if UNITY_WEBGL
        private string GetQueryParam(string key) {
            var uri = new Uri(Location.href());

            var endQuery = uri.Query.Replace("?", string.Empty);
            var query = 
                endQuery.Split('&')
                    .ToDictionary(c => c.Split('=')[0],
                        c => Uri.UnescapeDataString(c.Split('=')[1]));

            if (query.TryGetValue(key, out var value)) {
                Debug.Log($"The key {key} the value {value}");

                return value;
            }

            return string.Empty;
        }
#endif
    }
}