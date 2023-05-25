using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Extensions {
    [Serializable]
    public class IdAssetReferenceDictionary : SerializedDictionary<string, AssetReferenceT<TextAsset>> { } 
}