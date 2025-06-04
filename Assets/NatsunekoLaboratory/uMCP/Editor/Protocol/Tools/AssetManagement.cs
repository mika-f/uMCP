using System.ComponentModel;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

using Newtonsoft.Json;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.uMCP.Protocol.Tools
{
    [McpServerToolType]
    public class AssetManagement
    {
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

            // if only name is empty, return all assets of the specified type with their path and guid
            if (string.IsNullOrWhiteSpace(name))
            {
                var guids = AssetDatabase.FindAssets($"t:{type}");
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

            // if only type is empty, return all assets with the specified name and their path and guid
            if (string.IsNullOrWhiteSpace(type))
            {
                var guids = AssetDatabase.FindAssets(name);
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

            // if both name and type are specified, return all assets with the specified name and type, with their path and guid
            {
                var guids = AssetDatabase.FindAssets($"{name} t:{type}");
                return guids.Select(w => (AssetDatabase.GUIDToAssetPath(w), w)).Select(tuple =>
                {
                    var path = tuple.Item1;
                    var guid = tuple.w;
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
        }
    }
}
