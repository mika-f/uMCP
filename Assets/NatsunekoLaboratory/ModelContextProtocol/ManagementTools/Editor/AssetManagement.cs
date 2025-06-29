using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Models;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;
using NatsunekoLaboratory.ModelContextProtocol.ManagementTools.Models;

using UnityEditor;
using UnityEditor.Animations;

using UnityEngine;

using Object = UnityEngine.Object;

namespace NatsunekoLaboratory.ModelContextProtocol.ManagementTools
{
    [McpServerToolType]
    public static partial class AssetManagement
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
                    return new TextContentResult($"failed to create a new Asset, available factories are {string.Join(", ", Factories.Keys)}.");
                }

            ReloadEditor();

            return new TextContentResult($"asset created at: {path}");
        }

        [McpServerTool(Destructive = true)]
        [Description("delete an asset")]
        public static IToolResult DeleteAsset([Required] [Description("path to the asset to delete")] [StringIsNotNullOrEmpty] string path)
        {
            if (!AssetDatabase.DeleteAsset(path))
                return new ErrorContentResult($"failed to delete asset at: {path}");

            ReloadEditor();
            return new TextContentResult($"asset deleted at: {path}");
        }

        [McpServerTool(Readonly = true)]
        [Description("list the assets of the project, with optional filtering by type, or name")]
        public static IToolResult[] FindAsset([Required] [Description("filter by name")] string name, [Required] [CanBeNull] [Description("filter by type")] string type)
        {
            //  if both name and type are empty, return all assets with their path and guid
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(type))
            {
                var guids = AssetDatabase.GetAllAssetPaths();
                return guids.Select(w => (Path: w, Guid: AssetDatabase.AssetPathToGUID(Path.Combine("Assets", w)))).Select(tuple =>
                {
                    var path = tuple.Path;
                    var guid = tuple.Guid;
                    return new StructuredContentResult<Asset>(new Asset { Name = Path.GetFileName(path), Path = path, Guid = guid, Type = AssetDatabase.LoadAssetAtPath<Object>(path)?.GetType()?.FullName });
                }).ToArray<IToolResult>();
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
                    return new StructuredContentResult<Asset>(new Asset { Name = Path.GetFileName(path), Path = path, Guid = guid, Type = AssetDatabase.LoadAssetAtPath<Object>(path)?.GetType()?.FullName });
                }).ToArray<IToolResult>();
            }
        }

        [McpServerTool(Readonly = true)]
        [Description("get specified asset, if actual set to true, this func returns base64-encoded binary, otherwise; the asset metadata")]
        [return: McpServerToolReturnType(typeof(Asset))]
        public static IToolResult GetAsset([Required] [Description("path to the asset to get")] [StringIsNotNullOrEmpty] string path, [Description("read asset as the actual file, if actual set to true, this func returns base64-encoded binary, otherwise; the asset metadata")] bool actual)
        {
            if (!path.StartsWith("Assets/") && !path.StartsWith("Packages/"))
                path = Path.Combine("Assets", path);

            try
            {
                if (actual)
                {
                    if (!File.Exists(path))
                        return new ErrorContentResult($"file not found at: {path}");

                    var mime = MediaEncoder.GetMimeTypeFromFile(path);
                    if (mime.StartsWith("image/"))
                        return new ImageContentResult(path);

                    if (mime.StartsWith("audio/"))
                        return new AudioContentResult(path);

                    throw new NotSupportedException("unsupported media");
                }
                else
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (asset == null)
                        return new ErrorContentResult($"asset not found at: {path}");
                    var guid = AssetDatabase.AssetPathToGUID(path);

                    return new StructuredContentResult<Asset>(new Asset
                    {
                        Name = Path.GetFileName(path),
                        Guid = guid,
                        Path = path,
                        Type = asset.GetType().FullName
                    });
                }
            }
            catch
            {
                return new ErrorContentResult($"failed to load asset at: {path}");
            }
        }
    }
}
