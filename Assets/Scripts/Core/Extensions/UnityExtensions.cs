using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Core.Extensions {
    public static class UnityExtensions {
        public static float GetAspectRatio(float height, float width) {
            return height / width;
        }
        
        public static RectTransform AsRectTransform(this Transform transform) {
            return transform as RectTransform;
        }

        public static async Task LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            asyncOperation.allowSceneActivation = false;
            while (true) {
                await Task.Yield();
                if (asyncOperation.progress>=0.9f)
                    break;               
            }
            asyncOperation.allowSceneActivation = true;            
            
        }
        
        public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
        {
            var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            return localToWorldMatrix.MultiplyPoint3x4(position);
        }
 
        public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
        {
            var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            return worldToLocalMatrix.MultiplyPoint3x4(position);
        }

        public static void DestroyGameObject(this Object obj) {
            if (obj is Component component) { 
                Object.Destroy(component.gameObject);
            }

            if (obj is GameObject gameObject) {
                Object.Destroy(gameObject);
            }
        }

        public static T As<U, T>(this U init) where T : class 
                                              where U : class{
            return init as T;
        }

        public static Vector3 RandomizeVector3(this Vector3 vector3, float randomizeValue) {
            return new Vector3(vector3.x + UnityEngine.Random.Range(-randomizeValue, randomizeValue),
                vector3.y + UnityEngine.Random.Range(-randomizeValue, randomizeValue),
                vector3.z + UnityEngine.Random.Range(-randomizeValue, randomizeValue));
        }

        public static void LookAtZ(this Transform forTransform, Vector3 start, Vector3 end, bool invert = false) {
            Vector3 difference1 = end - start;
            if (invert) {
                difference1 = start - end;
            }
            float rotationZ1 = Mathf.Atan2(difference1.x, difference1.y) * Mathf.Rad2Deg;

            if (rotationZ1 < 0) {
                rotationZ1 = Mathf.Abs(rotationZ1);
            }
            else {
                rotationZ1 = -rotationZ1;
            }

            forTransform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ1); 
        }
    }
}