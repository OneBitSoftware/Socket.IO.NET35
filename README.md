# Socket.IO.NET35
A .NET 3.5 Socket.IO client used to create a websockets connection between a .NET client and a Node.JS socket.io server

## Overview and Credits
This project is a copied and modified version of the SocketIoClientDotNet project located here: https://github.com/Quobject/socketIoClientDotNet. My main goal was to reuse most of its socket-related code, but replace the Task Parallel Library with Microsoft's official backport (https://www.nuget.org/packages/TaskParallelLibrary). Thread management has been rewritten and improved to avoid unclosed threads.

Further to that, all other framework builds and supported platforms are removed as we don't need it. 

## Support
HTTP polling is disabled as we don't require it. This build allows websockets-only connectivity.
