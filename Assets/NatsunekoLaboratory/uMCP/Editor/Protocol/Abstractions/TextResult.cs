using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.uMCP.Protocol.Abstractions
{
    public class TextResult : ITextResult
    {
        public TextResult(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
