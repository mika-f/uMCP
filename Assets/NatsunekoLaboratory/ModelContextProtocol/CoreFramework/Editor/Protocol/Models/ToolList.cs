using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Attributes;
using NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Extensions;

using Newtonsoft.Json;

namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class ToolList
    {
        [JsonProperty("tools")]
        public List<Tool> Tools { get; set; }

        public static async Task<ToolList> CreateAsync(List<MethodInfo> tools)
        {
            return new ToolList
            {
                Tools = tools
                        .Select(w =>
                        {
                            var inputSchema = JsonSchema.JsonSchema.FromMethodInfo(w);
                            var outputSchema = w.ReturnTypeCustomAttributes.HasCustomAttribute<McpServerToolReturnTypeAttribute>() ? JsonSchema.JsonSchema.FromType(w.ReturnTypeCustomAttributes.GetCustomAttribute<McpServerToolReturnTypeAttribute>().T) : null;
                            var attribute = w.GetCustomAttribute<McpServerToolAttribute>();
                            return new Tool
                            {
                                Name = w.Name,
                                Description = w.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "",
                                InputSchema = inputSchema,
                                OutputSchema = outputSchema,
                                Annotations = new Dictionary<string, object>
                                {
                                    { "title", attribute.Name },
                                    { "readOnlyHint", attribute.Readonly },
                                    { "destructiveHint", attribute.Destructive },
                                    { "idempotentHint", attribute.Idempotent },
                                    { "openWorldHint", attribute.OpenWorld }
                                    // { "costly", w.GetCustomAttribute<McpServerToolAttribute>().Costly },
                                    // { "requiresHumanApproval", w.GetCustomAttribute<McpServerToolAttribute>().RequiresHumanApproval }
                                }
                            };
                        })
                        .ToList()
            };
        }
    }
}
