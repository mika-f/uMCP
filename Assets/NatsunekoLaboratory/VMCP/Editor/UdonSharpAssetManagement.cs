using System.ComponentModel;
using System.IO;
using System.Linq;

using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

using UdonSharp;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.VMCP
{
    [McpServerToolType]
    public class UdonSharpAssetManagement : MonoBehaviour
    {
        [McpServerTool]
        [Description("create a new UdonSharp script")]
        public static IToolResult CreateUdonSharpScript([Required] string path)
        {
            var script = $@"
using UdonSharp;

using UnityEngine;

using VRC.SDK3.Components;
using VRC.Udon;

namespace {string.Join(".", path.Split("/").SkipLast(1)).Replace(".cs", "")}
{{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class {path.Split("/").Last().Replace(".cs", "")} : UdonSharpBehaviour 
    {{
    }}
}}
";
            if (!path.StartsWith("Assets/"))
                path = Path.Combine("Assets", path);

            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            using var sw = new StreamWriter(path);
            sw.WriteLine(script);
            AssetDatabase.Refresh();

            // 
            var asset = ScriptableObject.CreateInstance<UdonSharpProgramAsset>();
            asset.sourceCsScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            AssetDatabase.CreateAsset(asset, Path.ChangeExtension(path, ".asset"));
            AssetDatabase.Refresh();

            return new TextResult("successfully create a new UdonSharp script");
        }
    }
}
