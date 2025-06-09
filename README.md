# uMCP

uMCP is a server implementation of the MCP protocol, designed for working with the AI in Unity.  
It is a minimalistic and efficient server that can be used to connect AI agents to Unity applications.

## Features

- **Safety**: uMCP does not allow the AI ​​to execute arbitrary code, and any and all operations can only be performed through pre-authorized operations.
- **Extensible**: uMCP is designed to be easily extensible, allowing you to add your own custom commands and operations via `McpServerToolType` and `McpServerTool` attribute.
  - You can find examples in the `Assets/NatsunekoLaboratory/VMCP` directory.

## How it works (What is different from other MCP implementations)

uMCP could directly communicate with the MCP client using the Streamable HTTP protocol, without any other dependencies.
This allows for a more efficient and lightweight implementation compared to other MCP servers that rely on that requires additional dependencies such as Python, Node.js or other language runtimes.

```plain
Your MCP Client <-- Streamable HTTP --> uMCP Server
```

## Installation

### Prerequisites

- Unity Editor: Version 2022.3 LTS or newer.
- The MCP client must support Streamable HTTP (e.g., VSCode Agent Mode, Cursor, CLINE, etc.).

### 1. Install the UnityPackage

1. Open your Unity project.
2. Go to `Window > Package Manager`.
3. Click `+` to `Add package from git URL...`.
4. Enter the URL:
   ```
   https://github.com/mika-f/uMCP.git?path=/Assets/NatsunekoLaboratory/uMCP
   ```
5. Click `Add`.
6. The MCP server automatically starts when you run the Unity project.

### 2. Configure your MCP client

Connect your MCP client that support Streamable HTTP (VSCode Agent Mode, Cursor, CLINE, etc.) to the MCP server.

```
http://localhost:7225/sse
```

Example for VSCode Agent Mode:

```json
{
  "servers": {
    "uMCP": {
      "url": "http://localhost:7225/sse"
    }
  }
}
```

## Usage

1. Open your Unity project.
2. Start your MCP client (VSCode Agent Mode, Cursor, CLINE, etc.).
3. Interact! Your MCP client should now be able to communicate with the Unity application.

## Extension

You can extend uMCP by creating your own custom commands and operations.

```csharp
using System;
using System.ComponentModel;

using NatsunekoLaboratory.uMCP.Protocol.Attributes;
using NatsunekoLaboratory.uMCP.Protocol.Abstractions;
using NatsunekoLaboratory.uMCP.Protocol.Interfaces;

namespace NatsunekoLaboratory.Examples.MyCustomCommands
{
    [McpServerToolType]
    public class MyCustomCommand
    {
        [McpServerTool]
        [Description("This is a custom command that does something.")]
        public static IToolResult Execute([Description("An example parameter for the custom command.")] string exampleParameter)
        {
            // Your custom command logic here
            return new TextResult($"Executed custom command with parameter: {exampleParameter}");
        }
    }
}
```

## License

MIT by [@6jz](https://twitter.com/6jz)
