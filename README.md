# uMCP

uMCP is a server implementation of the MCP protocol, designed for working with the AI in Unity.  
It is a minimalistic and efficient server that can be used to connect AI agents to Unity applications.

## How it works (What is different from other MCP implementations)

uMCP could directly communicate with the MCP client using the Streamable HTTP protocol, without any other dependencies.
This allows for a more efficient and lightweight implementation compared to other MCP servers that rely on that requires additional dependencies such as Python, Node.js or other language runtimes.

```plain
Your MCP Client <-- Streamable HTTP --> uMCP Server
```

## Installation

### Prerequisites

- Unity Editor: Version 6000.1 LTS or newer.
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

## License

MIT by [@6jz](https://twitter.com/6jz)
