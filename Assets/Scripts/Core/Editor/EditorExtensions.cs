#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Core.Editor {
    public static class EditorExtensions {
        public static string GetCLIArg(string name) {
            var trueName = $"-{name}";
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == trueName && args.Length > i + 1) {
                    return args[i + 1];
                }
            }
            return null;
        }
        
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
        
        public static T FindFirstAssetByType<T>() where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            foreach (var t in guids) {
                string assetPath = AssetDatabase.GUIDToAssetPath( t );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    return asset;
                }
            }

            return null;
        }
    }
}
#endif