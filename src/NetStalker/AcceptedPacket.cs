using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NetStalker.MainLogic;
using PacketDotNet;

namespace NetStalker;

internal class AcceptedPacket
{
	private Packet packet;

	private string host = "";

	private DateTime time = DateTime.Now;

	public Packet Packet
	{
		get
		{
			return packet;
		}
		set
		{
			packet = value;
		}
	}

	public IPv4Packet IPV4Packet => packet?.Extract<IPv4Packet>();

	public TcpPacket TCPPacket => IPV4Packet?.Extract<TcpPacket>();

	public UdpPacket UDPPacket => IPV4Packet?.Extract<UdpPacket>();

	public DateTime Time => time;

	public IPAddress Source => IPV4Packet?.SourceAddress;

	public IPAddress Destination => IPV4Packet?.DestinationAddress;

	public string Type
	{
		get
		{
			if (IsRequest)
			{
				if (TCPPacket != null)
				{
					if (TCPPacket.DestinationPort == 443)
					{
						return "HTTPS";
					}
					if (TCPPacket.DestinationPort == 80)
					{
						return "HTTP";
					}
					if (TCPPacket.DestinationPort == 21 || TCPPacket.DestinationPort == 20)
					{
						return "FTP";
					}
					if (TCPPacket.DestinationPort == 23)
					{
						return "TELNET";
					}
					if (TCPPacket.DestinationPort == 25)
					{
						return "SMTP";
					}
					if (TCPPacket.DestinationPort == 53)
					{
						return "DNS";
					}
					if (TCPPacket.DestinationPort == 137 || TCPPacket.DestinationPort == 138 || TCPPacket.DestinationPort == 139)
					{
						return "NetBIOS";
					}
					if (TCPPacket.DestinationPort == 110)
					{
						return "POP";
					}
					if (TCPPacket.DestinationPort == 143)
					{
						return "IMAP";
					}
					if (TCPPacket.DestinationPort == 161 || TCPPacket.DestinationPort == 162)
					{
						return "SNMP";
					}
					if (TCPPacket.DestinationPort == 989 || TCPPacket.DestinationPort == 990)
					{
						return "FTPS";
					}
				}
				else if (UDPPacket != null)
				{
					if (UDPPacket.DestinationPort == 63)
					{
						return "DNS";
					}
					if (UDPPacket.DestinationPort == 67 || UDPPacket.DestinationPort == 68)
					{
						return "DHCP";
					}
					if (UDPPacket.DestinationPort == 69)
					{
						return "TFTP";
					}
					if (UDPPacket.DestinationPort == 123)
					{
						return "NTP";
					}
					if (UDPPacket.DestinationPort == 161 || UDPPacket.DestinationPort == 162)
					{
						return "SNMP";
					}
					if (UDPPacket.DestinationPort == 137 || UDPPacket.DestinationPort == 138 || UDPPacket.DestinationPort == 139)
					{
						return "NetBIOS";
					}
				}
			}
			else if (IsResponse)
			{
				if (TCPPacket != null)
				{
					if (TCPPacket.SourcePort == 443)
					{
						return "HTTPS";
					}
					if (TCPPacket.SourcePort == 80)
					{
						return "HTTP";
					}
					if (TCPPacket.SourcePort == 21 || TCPPacket.SourcePort == 20)
					{
						return "FTP";
					}
					if (TCPPacket.SourcePort == 23)
					{
						return "TELNET";
					}
					if (TCPPacket.SourcePort == 25)
					{
						return "SMTP";
					}
					if (TCPPacket.SourcePort == 53)
					{
						return "DNS";
					}
					if (TCPPacket.SourcePort == 137 || TCPPacket.SourcePort == 138 || TCPPacket.SourcePort == 139)
					{
						return "NetBIOS";
					}
					if (TCPPacket.SourcePort == 110)
					{
						return "POP";
					}
					if (TCPPacket.SourcePort == 143)
					{
						return "IMAP";
					}
					if (TCPPacket.SourcePort == 161 || TCPPacket.SourcePort == 162)
					{
						return "SNMP";
					}
					if (TCPPacket.SourcePort == 989 || TCPPacket.SourcePort == 990)
					{
						return "FTPS";
					}
				}
				else if (UDPPacket != null)
				{
					if (UDPPacket.SourcePort == 63)
					{
						return "DNS";
					}
					if (UDPPacket.SourcePort == 67 || UDPPacket.SourcePort == 68)
					{
						return "DHCP";
					}
					if (UDPPacket.SourcePort == 69)
					{
						return "TFTP";
					}
					if (UDPPacket.SourcePort == 123)
					{
						return "NTP";
					}
					if (UDPPacket.SourcePort == 161 || UDPPacket.SourcePort == 162)
					{
						return "SNMP";
					}
					if (UDPPacket.SourcePort == 137 || UDPPacket.SourcePort == 138 || UDPPacket.SourcePort == 139)
					{
						return "NetBIOS";
					}
				}
			}
			return "N/A";
		}
	}

	public bool IsRequest
	{
		get
		{
			if (Source == null)
			{
				return false;
			}
			return Tools.AreCompatibleIPs(Source, AppConfiguration.GatewayIp, AppConfiguration.NetworkSize);
		}
	}

	public bool IsResponse => !IsRequest;

	public string Host
	{
		get
		{
			try
			{
				if (string.IsNullOrEmpty(host) && Type.Equals("HTTP") && HTTPData != null && HTTPData.Count > 0)
				{
					string text = HTTPData.FirstOrDefault((string l) => l.StartsWith("Host:", StringComparison.OrdinalIgnoreCase));
					if (text != null)
					{
						return text.Substring(5).Trim();
					}
					return HTTPData[0].Trim();
				}
				if (string.IsNullOrEmpty(host) && Type.Equals("HTTPS") && TCPPacket?.PayloadData != null)
				{
					string value = ExtractSNI(TCPPacket.PayloadData);
					if (!string.IsNullOrEmpty(value))
					{
						host = value;
						return host;
					}
				}
			}
			catch (Exception)
			{
			}
			return host;
		}
		set
		{
			host = value;
		}
	}

	public ushort ID
	{
		get
		{
			if (IPV4Packet == null)
			{
				return 0;
			}
			return IPV4Packet.Id;
		}
	}

	public string Protocol
	{
		get
		{
			if (IPV4Packet == null)
			{
				return "N/A";
			}
			return IPV4Packet.Protocol.ToString();
		}
	}

	public List<string> HTTPData
	{
		get
		{
			if (TCPPacket != null && TCPPacket.PayloadData != null)
			{
				return Encoding.UTF8.GetString(TCPPacket.PayloadData).Split('\n').ToList();
			}
			return new List<string>();
		}
	}

	public static string ExtractSNI(byte[] data)
	{
		if (data == null || data.Length < 43)
		{
			return null;
		}
		try
		{
			if (data[0] != 22)
			{
				return null;
			}
			if (data[5] != 1)
			{
				return null;
			}
			int num = 43;
			if (num >= data.Length)
			{
				return null;
			}
			int num2 = data[num];
			num += 1 + num2;
			if (num + 2 > data.Length)
			{
				return null;
			}
			int num3 = (data[num] << 8) | data[num + 1];
			num += 2 + num3;
			if (num + 1 > data.Length)
			{
				return null;
			}
			int num4 = data[num];
			num += 1 + num4;
			if (num + 2 > data.Length)
			{
				return null;
			}
			int num5 = (data[num] << 8) | data[num + 1];
			num += 2;
			int num6 = num + num5;
			if (num6 > data.Length)
			{
				num6 = data.Length;
			}
			while (num + 4 <= num6)
			{
				int num7 = (data[num] << 8) | data[num + 1];
				int num8 = (data[num + 2] << 8) | data[num + 3];
				num += 4;
				if (num7 == 0 && num + 5 <= data.Length && data[num + 2] == 0)
				{
					int num9 = (data[num + 3] << 8) | data[num + 4];
					if (num + 5 + num9 <= data.Length)
					{
						return Encoding.ASCII.GetString(data, num + 5, num9);
					}
				}
				num += num8;
			}
		}
		catch
		{
		}
		return null;
	}
}
