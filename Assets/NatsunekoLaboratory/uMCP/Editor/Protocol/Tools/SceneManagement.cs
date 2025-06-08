using System.ComponentModel;
using System.Linq;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

using Newtonsoft.Json;

using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace NatsunekoLaboratory.uMCP.Protocol.Tools
{
    [McpServerToolType]
    public static class SceneManagement
    {
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

        private static object GetHierarchyTree(GameObject gameObject)
        {
            var children = gameObject.transform.Cast<Transform>().Select(child => GetHierarchyTree(child.gameObject)).ToArray();
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
    }
}
