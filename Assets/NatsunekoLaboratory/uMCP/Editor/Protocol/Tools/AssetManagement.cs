using System;
using System.Collections.Generic;
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

using Object = UnityEngine.Object;

namespace NatsunekoLaboratory.uMCP.Protocol.Tools
{
    [McpServerToolType]
    public class AssetManagement
    {
        public static Dictionary<string, Action<string>> Factories => new()
        {
            // Animations
            { "AnimatorController", CreateAnimatorController },
            { "AnimationClip", CreateAnimationClip }
        };

        private static void CreateAnimatorController(string path)
        {
            AnimatorController.CreateAnimatorControllerAtPath(path);
        }

        private static void CreateAnimationClip(string path)
        {
            var animationClip = new AnimationClip();
            AssetDatabase.CreateAsset(animationClip, path);
        }

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
        public static IToolResult CreateAsset([Required] [Description("path to create the asset at")] [StringIsNotNullOrEmpty] string path, [Required] [Description("type of the asset to create")] [StringIsNotNullOrEmpty] string type)
        {
            CreateParentDirectoryIfNotExists(path);

            if (Factories.TryGetValue(type, out var factory))
                factory(path);
            else
                try
                {
                    var asset = ScriptableObject.CreateInstance(type);
                    if (asset == null)
                        throw new Exception();

                    AssetDatabase.CreateAsset(asset, path);
                }
                catch
                {
                    return new TextResult($"failed to create a new Asset, available factories are {string.Join(", ", Factories.Keys)}.");
                }

            ReloadEditor();

            return new TextResult($"asset created at: {path}");
        }

        [McpServerTool]
        [Description("delete an asset")]
        public static IToolResult DeleteAsset([Required] [Description("path to the asset to delete")] [StringIsNotNullOrEmpty] string path)
        {
            if (!AssetDatabase.DeleteAsset(path))
                return new ErrorResult($"failed to delete asset at: {path}");

            ReloadEditor();
            return new TextResult($"asset deleted at: {path}");
        }

        [McpServerTool]
        [Description("list the assets of the project, with optional filtering by type, or name")]
        public static ITextResult[] FindAsset([Required] [Description("filter by name")] string name, [Required] [CanBeNull] [Description("filter by type")] string type)
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
