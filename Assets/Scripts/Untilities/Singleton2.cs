// using UnityEngine;
//
// public class Singleton2<T> : MonoBehaviour where T : MonoBehaviour
// {
//     private static T instanceBase;
//
//     protected static Singleton2<T> InstanceBase
//     {
//         get
//         {
//             if (ReferenceEquals(instanceBase, null))
//             {
//                 if (InsNeedCreateSingleton())
//                 {
//                     CreateSingleton();
//                 }
//             }
//
//             return instanceBase;
//         }
//         set => instanceBase = value as T;
//     }
//     
//     /// <summary>
//     /// 检测场景中的单例
//     /// </summary>
//     private static bool IsNeedCreateSingleton()
//     {
//         var toCreate = false;
//     
//         var components = FindObjectsOfType<T>();
//     
//         // 场景中没有预先创建此单例
//         if (components.Length == 0)
//         {
//             toCreate = true;
//         }
//     
//         // 场景中预先创建了此单例
//         else if (components.Length == 1)
//         {
//             instanceBase = components[0];
//             DontDestroyOnLoad(instanceBase);
//         }
//     
//         // 错误, 场景中预先创建了多个此单例
//         else if (components.Length > 1)
//         {
//             Debug.LogError("错误: 预先创建了多个单例组件在场景中! 请检查并修改!");
//             foreach (var component in components)
//             {
//                 Destroy(component);
//             }
//             toCreate = true;
//         }
//     
//         return toCreate;
//     }
//
//     /// <summary>
//     /// 创建一个单例
//     /// </summary>
//     private static void CreateSingleton()
//     {
//         var gameObject = new GameObject($"Singleton_{typeof(T).Name}", typeof(T));
//         instanceBase = gameObject.GetComponent<T>();
//         DontDestroyOnLoad(instanceBase);
//     }
// }