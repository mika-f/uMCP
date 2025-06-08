using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Abstractions
{
    public class ErrorResult : IErrorResult
    {
        public ErrorResult(string error)
        {
            Error = error;
        }

        public string Error { get; }
    }
}
