namespace NatsunekoLaboratory.ModelContextProtocol.CoreFramework.Protocol.Models
{
    public class CallToolResultEmbeddedResourceContent : CallToolResultContentBase
    {
        public CallToolResultEmbeddedResourceContent()
        {
            Type = "resource";
        }

        public Resource Resource { get; set; }
    }
}
