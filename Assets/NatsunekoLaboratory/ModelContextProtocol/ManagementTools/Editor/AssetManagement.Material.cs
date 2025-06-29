using System.ComponentModel;

using JetBrains.Annotations;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes.Validators;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Abstractions;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Interfaces;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.ModelContextProtocol.ManagementTools
{
    public static partial class AssetManagement {
        [McpServerTool]
        [Description("create a new material, with shader, if shader is not specified, use the standard shader")]
        public static IToolResult CreateMaterialWithShader([Required] [Description("path to create the asset at")] [StringIsNotNullOrEmpty] string path, [Description("shader name to use")] [CanBeNull] string shaderName)
        {
            CreateParentDirectoryIfNotExists(path);

            var shader = Shader.Find(shaderName ?? "Standard");
            if (shader == null)
                return new ErrorContentResult($"shader '{shaderName}' not found.");

            var material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
            ReloadEditor();

            return new TextContentResult($"material created at: {path}");
        }
    }
}
