using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using JetBrains.Annotations;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

using Newtonsoft.Json;

using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

using Component = UnityEngine.Component;
using LightType = UnityEngine.LightType;
using Object = UnityEngine.Object;
using Transform = NatsunekoLaboratory.uMCP.Models.Transform;

namespace NatsunekoLaboratory.uMCP.Protocol.Tools
{
    [McpServerToolType]
    public static class SceneManagement
    {
        public static Dictionary<string, Func<string, UnityEngine.Transform>> Factory => new()
        {
            // GameObject
            { "GameObject", path => new GameObject(path).transform },

            // 3D
            { "Capsule", path => CreatePrimitive(path, PrimitiveType.Capsule) },
            { "Cube", path => CreatePrimitive(path, PrimitiveType.Cube) },
            { "Cylinder", path => CreatePrimitive(path, PrimitiveType.Cylinder) },
            { "Plane", path => CreatePrimitive(path, PrimitiveType.Plane) },
            { "Quad", path => CreatePrimitive(path, PrimitiveType.Quad) },
            { "Sphere", path => CreatePrimitive(path, PrimitiveType.Sphere) },

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
        public static ITextResult OpenScene([Required] [StringIsNotNullOrEmpty] string sceneName)
        {
            var scenePath = SceneManager.GetActiveScene().path;
            if (scenePath.EndsWith(sceneName + ".unity"))
            {
                EditorSceneManager.OpenScene(scenePath);
                return new TextResult($"opened scene: {sceneName}");
            }

            return new TextResult($"scene {sceneName} not found.");
        }

        [McpServerTool]
        [Description("get the current active scene name")]
        public static ITextResult GetActiveSceneName()
        {
            return new TextResult(SceneManager.GetActiveScene().name);
        }

        [McpServerTool]
        [Description("save the current active scene")]
        public static ITextResult SaveScene()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            return new TextResult($"saved scene: {SceneManager.GetActiveScene().name}");
        }

        [McpServerTool]
        [Description("close the current active scene")]
        public static ITextResult CloseScene()
        {
            EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true);
            return new TextResult($"closed scene: {SceneManager.GetActiveScene().name}");
        }

        [McpServerTool]
        [Description("get hierarchy tree on active scene")]
        public static ITextResult[] GetHierarchy()
        {
            var scene = SceneManager.GetActiveScene();
            var gameObjects = scene.GetRootGameObjects();
            var hierarchy = gameObjects.Select(GetHierarchyTree).ToList();

            return hierarchy.Select(w => new TextResult(JsonConvert.SerializeObject(w))).ToArray<ITextResult>();
        }

        [McpServerTool]
        [Description("create a new object in hierarchy")]
        public static IToolResult CreateObject(
            [Required] [Description("the path for creating a new GameObject")] [StringIsNotNullOrEmpty]
            string path,
            [Required] [Description("the type for creating a new GameObject, such as GameObject, Directional Light, Particle System")] [StringIsNotNullOrEmpty]
            string type
        )
        {
            var name = path.Split("/").Last();
            var normalizedTypeName = type.Replace(" ", "");

            if (Factory.TryGetValue(normalizedTypeName, out var factory))
            {
                var obj = factory.Invoke(name);

                if (obj && path.Contains("/"))
                {
                    var transform = FindGameObjectAtThePath(string.Join("/", path.Split("/").SkipLast(1)));
                    if (transform)
                        obj.parent = transform;
                }

                SaveScene();
                return new TextResult($"successfully create a new GameObject<{type}> as the {path}");
            }

            return new TextResult($"failed to create a new GameObject, available factories are {string.Join(", ", Factory.Keys)}.");
        }

        [McpServerTool(Destructive = true, RequiresHumanApproval = true)]
        [Description("delete the specified object")]
        public static IToolResult DeleteObject([Required] [Description("the path for deleting object")] [StringIsNotNullOrEmpty] string path)
        {
            var obj = FindGameObjectAtThePath(path);
            if (obj)
            {
                Object.DestroyImmediate(obj.gameObject);
                return new TextResult($"successfully deleted the GameObject at the {path}");
            }

            return new ErrorResult($"the specified path ({path}) not found");
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

                return new TextResult($"{path}' transform edited");
            }

            return new ErrorResult("could not find GameObject");
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
    }
}