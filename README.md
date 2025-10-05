<p align="center">
     <img src="./branding/iconography/webicon.svg" 
          width="45%" />
</p>

# About Network Inspector
Network Inspector is a basic cross-platform network scanner written in C# .NET using [Avalonia UI](https://github.com/AvaloniaUI/Avalonia). The program's primary purpose is to determine which IP addresses from a given range are online and which ports they have open. This makes it a useful tool for locating devices and troubleshooting network issues.

## Installation
The latest version of Network Inspector is [1.0.0](https://github.com/josh-reeves/network-inspector/releases/tag/v1.0.0). For more information, reference the linked release.

## Regarding UDP Support
Currently scanning for open UPD ports is not supported. Due to the UDP protocol's "fire-and-forget" nature, while it is sometimes possible to determine when a system isn't listening for UDP traffic on a specific port, it usually isn't possible to confirm when a system is.
<br>
<br>
<br>
**NOTE:** This program was written explicitly for the purpose of being a helpful troubleshooting tool. Please use it responsibly.