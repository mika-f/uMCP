using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using JetBrains.Annotations;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;

using Newtonsoft.Json;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

using Component = UnityEngine.Component;
using LightType = UnityEngine.LightType;
using Object = UnityEngine.Object;
using Transform = NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Models.Unity.Transform;

namespace NatsunekoLaboratory.ModelContextProtocol.ManagementTools
{
    [McpServerToolType]
    public static class SceneManagement
    {
        public static Dictionary<string, Func<string, UnityEngine.Transform>> Factories => new()
        {
            // GameObject
            { "GameObject", path => new GameObject(path).transform },

            // 3D Object
            { "Capsule", path => CreatePrimitive(path, PrimitiveType.Capsule) },
            { "Cube", path => CreatePrimitive(path, PrimitiveType.Cube) },
            { "Cylinder", path => CreatePrimitive(path, PrimitiveType.Cylinder) },
            { "Plane", path => CreatePrimitive(path, PrimitiveType.Plane) },
            { "Quad", path => CreatePrimitive(path, PrimitiveType.Quad) },
            { "Sphere", path => CreatePrimitive(path, PrimitiveType.Sphere) },
            { "Terrain", CreateTerrain },

            // Effects
            { "ParticleSystem", CreateAttachedComponent<ParticleSystem> },
            { "ParticleSystemForceShield", CreateAttachedComponent<ParticleSystemForceField> },
            { "Trail", CreateAttachedComponent<TrailRenderer> },
            { "Line", CreateAttachedComponent<LineRenderer> },

            // Lights
            { "DirectionalLight", path => CreateLight(path, LightType.Directional) },
            { "SpotLight", path => CreateLight(path, LightType.Spot) },
            { "PointLight", path => CreateLight(path, LightType.Point) },
            { "AreaLight", path => CreateLight(path, LightType.Rectangle) },
            { "ReflectionProbe", CreateAttachedComponent<ReflectionProbe> },

            // Audio
            { "AudioSource", CreateAttachedComponent<AudioSource> },
            { "AudioListener", CreateAttachedComponent<AudioReverbZone> },

            // Video
            { "VideoPlayer", CreateAttachedComponent<VideoPlayer> },

            // UI

            // Camera
            { "Camera", CreateAttachedComponent<Camera> }
        };

        [McpServerTool]
        [Description("open the specified scene")]
        public static IToolResult OpenScene([Required] [StringIsNotNullOrEmpty] string sceneName)
        {
            var scenePath = SceneManager.GetActiveScene().path;
            if (scenePath.EndsWith(sceneName + ".unity"))
            {
                EditorSceneManager.OpenScene(scenePath);
                return new TextContentResult($"opened scene: {sceneName}");
            }

            return new TextContentResult($"scene {sceneName} not found.");
        }

        [McpServerTool]
        [Description("get the current active scene name")]
        public static IToolResult GetActiveSceneName()
        {
            return new TextContentResult(SceneManager.GetActiveScene().name);
        }

        [McpServerTool]
        [Description("save the current active scene")]
        public static IToolResult SaveScene()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            return new TextContentResult($"saved scene: {SceneManager.GetActiveScene().name}");
        }

        [McpServerTool]
        [Description("close the current active scene")]
        public static IToolResult CloseScene()
        {
            EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true);
            return new TextContentResult($"closed scene: {SceneManager.GetActiveScene().name}");
        }

        [McpServerTool]
        [Description("get hierarchy tree on active scene")]
        public static IToolResult[] GetHierarchy([Description("root path of finding GameObject, if root is null or whitespace, find by scene. otherwise; find by children")] string root)
        {
            var scene = SceneManager.GetActiveScene();

            if (string.IsNullOrWhiteSpace(root))
            {
                var gameObjects = scene.GetRootGameObjects();
                var hierarchy = gameObjects.Select(GetHierarchyTree).ToList();

                return hierarchy.Select(w => new TextContentResult(JsonConvert.SerializeObject(w))).ToArray<IToolResult>();
            }
            else
            {
                var go = FindGameObjectAtThePath(root);
                var hierarchy = GetHierarchyTree(go.gameObject);

                return new IToolResult[] { new TextContentResult(JsonConvert.SerializeObject(hierarchy)) };
            }
        }

        [McpServerTool]
        [Description("create a new object in hierarchy")]
        public static IToolResult CreateGameObject(
            [Required] [Description("the hiderarchy path for creating a new GameObject, e.g. Main Camera for Root `Main Camera` object, `Lights/Volume Light` for `Volume Light` the child of `Lights`")] [StringIsNotNullOrEmpty]
            string path,
            [Required] [Description("the type for creating a new GameObject, such as GameObject, Directional Light, Particle System")] [StringIsNotNullOrEmpty]
            string type
        )
        {
            var name = path.Split("/").Last();
            var normalizedTypeName = type.Replace(" ", "");

            if (Factories.TryGetValue(normalizedTypeName, out var factory))
            {
                var obj = factory.Invoke(name);

                if (obj && path.Contains("/"))
                {
                    var transform = FindGameObjectAtThePath(string.Join("/", path.Split("/").SkipLast(1)));
                    if (transform)
                        obj.parent = transform;
                }

                SaveScene();
                return new TextContentResult($"successfully create a new GameObject<{type}> as the {path}");
            }

            return new TextContentResult($"failed to create a new GameObject, available factories are {string.Join(", ", Factories.Keys)}.");
        }

        [McpServerTool(Destructive = true, RequiresHumanApproval = true)]
        [Description("delete the specified object")]
        public static IToolResult DeleteObject([Required] [Description("the path for deleting object")] [StringIsNotNullOrEmpty] string path)
        {
            var obj = FindGameObjectAtThePath(path);
            if (obj)
            {
                Object.DestroyImmediate(obj.gameObject);
                return new TextContentResult($"successfully deleted the GameObject at the {path}");
            }

            return new ErrorContentResult($"the specified path ({path}) not found");
        }

        [McpServerTool]
        [Description("edit transform position, rotation (by euler), and scale of the specified path")]
        public static IToolResult EditTransform([Required] [Description("the path for editing transform")] [StringIsNotNullOrEmpty] string path, [Required] Transform transform)
        {
            var go = FindGameObjectAtThePath(path);
            if (go)
            {
                go.transform.localPosition = transform.Position.ToVector3();
                go.transform.localEulerAngles = transform.Rotation.ToVector3();
                go.transform.localScale = transform.Scale.ToVector3();

                return new TextContentResult($"{path}' transform edited");
            }

            return new ErrorContentResult("could not find GameObject");
        }

        [McpServerTool]
        [Description("create prefab to prefabPath of hierarchyPath")]
        public static IToolResult CreatePrefab([Required] [Description("the path for GameObject that contains in Prefab")] string hierarchyPath, [Required] [Description("the path for creating a new prefab")] [StringIsNotNullOrEmpty] string prefabPath)
        {
            var obj = FindGameObjectAtThePath(hierarchyPath);
            if (obj)
            {
                var prefab = PrefabUtility.SaveAsPrefabAsset(obj.gameObject, prefabPath);
                if (prefab)
                    return new TextContentResult($"successfully created a new prefab at {prefabPath}");

                return new ErrorContentResult($"failed to create a new prefab at {prefabPath}");
            }

            return new ErrorContentResult($"the specified path ({hierarchyPath}) not found");
        }

        [McpServerTool]
        [Description("attach component to specified GameObject")]
        public static IToolResult AttachComponent([Required] [Description("the path for GameObject that contains in Prefab")] [StringIsNotNullOrEmpty] string path, [Required] [Description("the fullly qualified type name of component to attach")] [StringIsNotNullOrEmpty] string type)
        {
            var obj = FindGameObjectAtThePath(path);
            if (obj)
            {
                var componentType = Type.GetType(type);
                if (componentType != null && typeof(Component).IsAssignableFrom(componentType))
                {
                    obj.gameObject.AddComponent(componentType);
                    return new TextContentResult($"successfully attached {type} to {path}");
                }

                return new ErrorContentResult($"the specified type ({type}) is not a valid Component type");
            }

            return new ErrorContentResult($"the specified path ({path}) not found");
        }

        [McpServerTool(Destructive = true, RequiresHumanApproval = true)]
        [Description("detach component from specified GameObject")]
        public static IToolResult DetachComponent([Required] [Description("the path for GameObject that contains in Prefab")] [StringIsNotNullOrEmpty] string path, [Required] [Description("the fullly qualified type name of component to detach")] [StringIsNotNullOrEmpty] string type)
        {
            var obj = FindGameObjectAtThePath(path);
            if (obj)
            {
                var componentType = Type.GetType(type);
                if (componentType != null && typeof(Component).IsAssignableFrom(componentType))
                {
                    var component = obj.GetComponent(componentType);
                    if (component)
                    {
                        Object.DestroyImmediate(component);
                        return new TextContentResult($"successfully detached {type} from {path}");
                    }

                    return new ErrorContentResult($"the specified component ({type}) not found on {path}");
                }

                return new ErrorContentResult($"the specified type ({type}) is not a valid Component type");
            }

            return new ErrorContentResult($"the specified path ({path}) not found");
        }

        private static object GetHierarchyTree(GameObject gameObject)
        {
            var children = gameObject.transform.Cast<UnityEngine.Transform>().Select(child => GetHierarchyTree(child.gameObject)).ToArray();
            return new
            {
                gameObject.name,
                active = gameObject.activeSelf,
                gameObject.tag,
                gameObject.layer,
                gameObject.isStatic,
                transform = new
                {
                    position = new
                    {
                        gameObject.transform.position.x,
                        gameObject.transform.position.y,
                        gameObject.transform.position.z
                    },
                    rotation = new
                    {
                        gameObject.transform.rotation.eulerAngles.x,
                        gameObject.transform.rotation.eulerAngles.y,
                        gameObject.transform.rotation.eulerAngles.z
                    },
                    scale = new
                    {
                        gameObject.transform.localScale.x,
                        gameObject.transform.localScale.y,
                        gameObject.transform.localScale.z
                    }
                },
                children
            };
        }

        [CanBeNull]
        private static UnityEngine.Transform FindGameObjectAtThePath(string path)
        {
            var scene = SceneManager.GetActiveScene();
            var gameObjects = scene.GetRootGameObjects();
            var root = path.Split("/");
            var current = gameObjects.FirstOrDefault(w => w.name == root[0]);
            var rest = string.Join("/", root.Skip(1));

            return current?.transform.Find(rest);
        }

        private static UnityEngine.Transform CreatePrimitive(string name, PrimitiveType type)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;

            return go.transform;
        }

        private static UnityEngine.Transform CreateLight(string name, LightType type)
        {
            var go = new GameObject(name);
            var light = go.AddComponent<Light>();
            light.type = type;

            return go.transform;
        }

        private static UnityEngine.Transform CreateAttachedComponent<T>(string name) where T : Component
        {
            var go = new GameObject(name);
            go.AddComponent<T>();

            return go.transform;
        }

        private static UnityEngine.Transform CreateTerrain(string name)
        {
            var data = new TerrainData();
            var go = Terrain.CreateTerrainGameObject(data);

            return go.transform;
        }
    }
}