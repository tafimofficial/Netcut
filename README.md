# NetVanguard - Network Toolkit 🛡️

> **Advanced Network Guardian, Bandwidth Limiter & ARP Device Manager for Windows**

NetVanguard is a desktop network management utility designed for local area networks (LAN / Wi-Fi). It allows administrators and users to discover connected devices, monitor real-time traffic, apply speed limits (bandwidth capping), and manage device access (cut/block internet).

---


# Frist Download the Zip from release or Clone the repo . Unzip it . run the npcap-installer.exe Then Run the NetVanguard.exe file with adnin .


---

## 🚀 Key Features

- **Network Discovery & Scanner:** Fast ARP probing across the local subnet to detect all active devices, hostnames, and MAC addresses.
- **Internet Cut / Block:** Cut or restore internet connectivity for any device on your local network with a single click.
- **Bandwidth Limiter (Speed Capper):**
  - Set custom **Download** limits (in KB/s).
  - Set custom **Upload** limits (in KB/s).
  - Live real-time speed monitoring per device.
- **Integrated Packet Sniffer:** Select any target device to inspect real-time packet flow, domain requests, and protocols.
- **Custom Device Aliases:** Rename any discovered device with a custom friendly label.

---

## 📋 System Requirements

| Requirement | Details |
| :--- | :--- |
| **Operating System** | Windows 10 / Windows 11 (32-bit or 64-bit) |
| **Runtime** | [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) or higher (pre-installed on modern Windows) |
| **Driver (Mandatory)** | **Npcap Driver** (Installer `npcap-installer.exe` included in this package) |
| **Permissions** | **Administrator privileges** are required to open raw network sockets |

---

## 🛠️ Installation & Setup

### 1. Install Npcap (First-time setup only)
1. Run `npcap-installer.exe` located in this directory.
2. During installation, ensure **"Install Npcap in WinPcap API-compatible Mode"** is checked.
3. Complete the installation wizard.
   > *(Optional: You can also download the latest version from [npcap.com](https://npcap.com/#download))*

### 2. Run NetVanguard
> **Important:** Keep `NetVanguard.exe`, all `.dll` files, and `NetVanguard.exe.config` in the same folder.

1. Right-click on **`NetVanguard.exe`** and select **"Run as administrator"**.
2. Select your active network adapter (Wi-Fi or Ethernet) from the list and click **SELECT**.
3. Click the **SCAN** button to discover devices connected to your network.

---

## 📖 Usage Guide

### ✂️ Cut / Block Internet
- Find the target device in the table.
- Check the **Block** checkbox to instantly disconnect the target from the gateway.
- Uncheck the box to restore normal connection.

### ⚡ Bandwidth Throttling / Speed Limiter
1. Double-click the target device row or click on the **Download Cap** / **Upload Cap** column.
2. The **Speed Limiter** modal will open:
   - **Download:** Enter the maximum download speed in KB/s (e.g., `200` or `500`).
   - **Upload:** Enter the maximum upload speed in KB/s (e.g., `100`).
   - Enter `0` for unlimited.
3. Click **SET** to activate live throttling and redirection.

### 🔍 Packet Sniffer
1. Select any device from the list.
2. Click the **SNIFFER** button.
3. View real-time packet headers, visited domains, and protocol stats.

### 🏷️ Rename Device
- Right-click any device row and select **Set Name** to give it a custom friendly name.

---

## 📁 Repository Structure

```text
NetVanguard/
├── NetVanguard.exe              # Main Application Executable
├── NetVanguard.exe.config       # Application Configuration
├── NetVanguard.pdb              # Debug Symbols
├── npcap-installer.exe          # Npcap Driver Installer
├── README.md                    # Project Documentation
├── README.txt                   # Plaintext Documentation
├── .gitignore                   # Git Ignore Rules
├── .gitattributes               # Git Binary / Text Attributes
└── *.dll                        # Runtime dependencies and packet drivers
```

---

## ⚠️ Disclaimer & Ethical Notice

This software is provided for educational, network administration, and testing purposes only. Use this tool only on networks that you own or have explicit authorization to monitor and manage.

---


- Compatible with Windows 10 & 11

