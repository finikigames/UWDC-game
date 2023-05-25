using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Core
{
    public static class CoreUtils
    {
        public static IEnumerable<T> GetEnumValues<T>(params T[] except) where T : Enum {
            var values = new List<T>((T[])Enum.GetValues(typeof(T)));

            foreach (var exceptValue in except)
            {
                values.Remove(exceptValue);
            }

            return values;
        }

        public static T RandomEnumValue<T>(params T[] except) where T : Enum
        {
            var values = new List<T>((T[])Enum.GetValues(typeof(T)));
            
            foreach (var exceptValue in except)
            {
                values.Remove(exceptValue);
            }

            return values[Random.Range(0, values.Count)];
        }
        
        // Todo move to another utils file
        public static async Task ScheduleTask(Action action)
        {
            await Task.Factory.StartNew(action,
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        // Todo move to another utils file
        public static float MilliSecondsToSeconds(int milliseconds)
        {
            return milliseconds / 1000f;
        }
        
        // Todo move to another utils file
        public static float MilliSecondsToSeconds(long milliseconds)
        {
            return milliseconds / 1000f;
        }
        
        // Todo move to another utils file
        public static int SecondsToMilliseconds(float seconds)
        {
            return Mathf.RoundToInt(seconds * 1000);
        }
        
        public static string SafeAnyToString<T>(T value)
        {
            return value == null ? "" : value.ToString();
        }

        public struct LoadResult<T>
        {
            public T Value;
            public AsyncOperationStatus Status;
            public AsyncOperationHandle Handle;
            public AssetReference Reference;
        }

        public static async Task<LoadResult<T>> LoadAssetAsync<T>(AssetReference assetReference)
        {
            var handle = assetReference.LoadAssetAsync<T>();

            try
            {
                await handle.Task;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            
            return new LoadResult<T>
            {
                Value = handle.Result,
                Status = handle.Status,
                Handle = handle,
                Reference = assetReference
            };
        }

        public static void ResizeCollection<T>(int newSize, 
                                               Func<T> itemCreator,
                                               ICollection<T> collection,
                                               Action<T> itemDestroyer = null)
        {
            var size = collection.Count;

            if (newSize > size)
            {
                for (var i = size; i < newSize; i++)
                {
                    var item = itemCreator.Invoke();    
                    
                    collection.Add(item);
                }
            }
            else if (newSize == size)
            {
                return;
            }
            else
            {
                var deleteCount = size - newSize;

                foreach (var item in collection.ToList())
                {
                    if (deleteCount <= 0)
                    {
                        return;
                    }

                    if (itemDestroyer != null) itemDestroyer.Invoke(item);

                    collection.Remove(item);
                    
                    deleteCount--;
                }
            }
        }

        public static Texture2D TextureFromSprite(this Sprite sprite) {
            if (sprite.rect.width == sprite.texture.width) {
                return sprite.texture;
            }

            var newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

            var newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                (int)sprite.textureRect.y,
                                (int)sprite.textureRect.width,
                                (int)sprite.textureRect.height );
            newText.SetPixels(newColors);
            newText.Apply();

            return newText;
        }
    }

    public delegate void ActionRef<T>(ref T item);
}