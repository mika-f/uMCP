using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

using Newtonsoft.Json;

using UnityEditor;
using UnityEditor.Animations;

using UnityEngine;

namespace NatsunekoLaboratory.uMCP.Protocol.Tools
{
    [McpServerToolType]
    public class AssetManagement
    {
        private static void CreateParentDirectoryIfNotExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
                AssetDatabase.Refresh();
            }
        }

        private static void ReloadEditor()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        [McpServerTool]
        [Description("create an asset, if the parent directory is not exists, create with one")]
        public static ITextResult CreateAsset([Required] [Description("path to create the asset at")] string path, [Required] [Description("type of the asset to create")] string type)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(type))
                return new TextResult("Path and type cannot be empty.");

            CreateParentDirectoryIfNotExists(path);

            switch (type)
            {
                case "AnimatorController":
                    var animatorController = AnimatorController.CreateAnimatorControllerAtPath(path);
                    if (animatorController == null)
                        return new TextResult($"Failed to create AnimatorController at: {path}");
                    AssetDatabase.SaveAssets();
                    break;

                case "AnimationClip":
                    var animationClip = new AnimationClip();
                    AssetDatabase.CreateAsset(animationClip, path);
                    break;

                default:
                    var asset = ScriptableObject.CreateInstance(type);
                    if (asset == null)
                        return new TextResult($"Failed to create asset of type: {type}");

                    AssetDatabase.CreateAsset(asset, path);
                    break;
            }

            ReloadEditor();

            return new TextResult($"Asset created at: {path}");
        }

        [McpServerTool]
        [Description("delete an asset")]
        public static ITextResult DeleteAsset([Required] [Description("path to the asset to delete")] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new TextResult("Path cannot be empty.");
            if (!AssetDatabase.DeleteAsset(path))
                return new TextResult($"Failed to delete asset at: {path}");

            ReloadEditor();
            return new TextResult($"Asset deleted at: {path}");
        }

        [McpServerTool]
        [Description("list the assets of the project, with optional filtering by type, or name")]
        public static ITextResult[] GetAsset([Required] [Description("filter by name")] string name, [Required] [CanBeNull] [Description("filter by type")] string type)
        {
            //  if both name and type are empty, return all assets with their path and guid
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(type))
            {
                var guids = AssetDatabase.GetAllAssetPaths();
                return guids.Select(w => (Path: w, Guid: AssetDatabase.AssetPathToGUID(Path.Combine("Assets", w)))).Select(tuple =>
                {
                    var path = tuple.Path;
                    var guid = tuple.Guid;
                    var json = JsonConvert.SerializeObject(new
                    {
                        name = Path.GetFileName(path),
                        path,
                        guid,
                        type = AssetDatabase.LoadAssetAtPath<Object>(path)?.GetType().FullName
                    });
                    return new TextResult(json);
                }).ToArray<ITextResult>();
            }
            else
            {
                var sb = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(name))
                    sb.Append($"{name} ");
                if (!string.IsNullOrWhiteSpace(type))
                    sb.Append($"t:{type} ");

                var guids = AssetDatabase.FindAssets(sb.ToString().TrimEnd());
                return guids.Select(w => (AssetDatabase.GUIDToAssetPath(w), w)).Select(tuple =>
                {
                    var path = tuple.Item1;
                    var guid = tuple.w;
                    var json = JsonConvert.SerializeObject(new
                    {
                        name = Path.GetFileName(path),
                        path,
                        guid,
                        type = AssetDatabase.LoadAssetAtPath<Object>(path).GetType().FullName
                    });
                    return new TextResult(json);
                }).ToArray<ITextResult>();
            }
        }
    }
}
