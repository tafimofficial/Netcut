========================================================================
                      NETVANGUARD - NETWORK TOOLKIT
           (Advanced Network Guardian, Bandwidth Limiter & Cut)
========================================================================

Follow these instructions to run NetVanguard on any Windows PC.

------------------------------------------------------------------------
1. Prerequisites / Requirements:
------------------------------------------------------------------------

[1] Npcap Driver (MANDATORY):
    - NetVanguard uses Npcap for raw packet sniffing, ARP spoofing, and speed capping.
    - An installer (`npcap-installer.exe`) is already included in this folder for your convenience.
    - On a new PC, run `npcap-installer.exe` first.
      (During install, make sure "Install Npcap in WinPcap API-compatible Mode" is checked).
    - Official site: https://npcap.com/#download

[2] .NET Framework 4.7.2 or higher:
    - Pre-installed by default on Windows 10 (version 1803+) and Windows 11.

[3] Administrator Privileges (Run as Administrator):
    - NetVanguard requires Administrator privileges to access network adapters.

[4] Keep All Folder Contents Together:
    - Do not separate `NetVanguard.exe` from its `.dll` files and `NetVanguard.exe.config`.
    - If downloaded as a ZIP file, right-click and choose "Extract All" before running.


------------------------------------------------------------------------
2. How to Run NetVanguard:
------------------------------------------------------------------------

Step 1:
If Npcap is not installed on the PC, double-click `npcap-installer.exe` and complete installation.

Step 2:
Right-click `NetVanguard.exe` and select "Run as administrator".

Step 3:
A network adapter selection dialog will appear. Choose your active card (e.g., Wi-Fi or Ethernet) and click "SELECT".

Step 4:
Click the "SCAN" button at the bottom. NetVanguard will probe the local network and display all connected devices.


------------------------------------------------------------------------
3. Features & Usage:
------------------------------------------------------------------------

* Cut / Block Internet:
  - Check the "Block" checkbox on any device to immediately disconnect it from the network.
  - Uncheck to restore access.

* Set Speed Limit (Download / Upload Cap):
  - Click directly on the "Download Cap" or "Upload Cap" column cell for any device (or double-click the device row).
  - A "Speed Limiter" window will appear:
    - DOWNLOAD: Set maximum download speed in KB/s (e.g., 200 or 500).
    - UPLOAD: Set maximum upload speed in KB/s (e.g., 100).
    - Enter 0 for unlimited.
  - Click "SET".
  - Redirection and bandwidth throttling are activated automatically!
  - The live speed and cap (e.g., `[Cap: 200 KB/s]`) will display in real time.

* Packet Sniffer:
  - Select a device from the list and click "SNIFFER".
  - View real-time packets, domains, and protocols for that target.

* Rename Device:
  - Right-click any device and select "Set Name".


------------------------------------------------------------------------
Presented by: Hamzi Alsheikh & NetVanguard Student Team
Compatible with: Windows 10 / Windows 11 (x86 & x64)
========================================================================
