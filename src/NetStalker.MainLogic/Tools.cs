using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace NetStalker.MainLogic;

public static class Tools
{
	public static string GetRootIp(IPAddress ipaddress)
	{
		string text = ipaddress.ToString();
		return text.Substring(0, text.LastIndexOf(".") + 1);
	}

	public static string GetMACString(PhysicalAddress physicaladdress)
	{
		string text = "";
		for (int i = 0; i <= 5; i++)
		{
			text = text + physicaladdress.GetAddressBytes()[i].ToString("X2") + ":";
		}
		return text.Substring(0, text.Length - 1);
	}

	public static bool AreCompatibleIPs(IPAddress ip1, IPAddress ip2)
	{
		return GetRootIp(ip1) == GetRootIp(ip2);
	}

	public static bool AreCompatibleIPs(IPAddress ip1, IPAddress ip2, int size)
	{
		return GetRoot(ip1, size) == GetRoot(ip2, size);
	}

	public static string GetRootIp(string ipaddress)
	{
		return ipaddress.Substring(0, ipaddress.LastIndexOf(".") + 1);
	}

	public static string GetRoot(IPAddress IP, int Mask)
	{
		if (IP == null)
		{
			return "";
		}
		string[] array = IP.ToString().Split('.');
		if (array.Length < 4)
		{
			return "";
		}
		return Mask switch
		{
			1 => array[0] + "." + array[1] + "." + array[2] + ".", 
			2 => array[0] + "." + array[1] + ".", 
			3 => array[0] + ".", 
			_ => array[0] + "." + array[1] + "." + array[2] + ".", 
		};
	}

	[DllImport("iphlpapi.dll", ExactSpelling = true)]
	private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref int PhysAddrLen);

	public static PhysicalAddress ResolveMacViaArp(IPAddress ip)
	{
		if (ip == null)
		{
			return null;
		}
		try
		{
			byte[] array = new byte[6];
			int PhysAddrLen = array.Length;
			if (SendARP(BitConverter.ToInt32(ip.GetAddressBytes(), 0), 0, array, ref PhysAddrLen) == 0 && PhysAddrLen == 6)
			{
				return new PhysicalAddress(array);
			}
		}
		catch
		{
		}
		return null;
	}

	public static PhysicalAddress GetLocalMac(string FriendlyName)
	{
		if (string.IsNullOrEmpty(FriendlyName))
		{
			throw new NullReferenceException("\"FriendlyName\" is null");
		}
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if (networkInterface.Name == FriendlyName)
			{
				return networkInterface.GetPhysicalAddress();
			}
		}
		return null;
	}

	public static IPAddress GetGatewayIP(string FriendlyName)
	{
		if (string.IsNullOrEmpty(FriendlyName))
		{
			throw new NullReferenceException("\"FriendlyName\" is null");
		}
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if (!(networkInterface.Name == FriendlyName))
			{
				continue;
			}
			foreach (GatewayIPAddressInformation gatewayAddress in networkInterface.GetIPProperties().GatewayAddresses)
			{
				if (gatewayAddress.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					return gatewayAddress.Address;
				}
			}
		}
		return null;
	}

	public static IPAddress GetBroadcastAddress(IPAddress LocalIp, string SubnetMask)
	{
		string[] array = (from x in LocalIp.GetAddressBytes()
			select Convert.ToString(x, 2).PadLeft(8, '0')).ToArray();
		string[] array2 = (from x in SubnetMask.Split('.')
			select Convert.ToString(int.Parse(x), 2).PadLeft(8, '0')).ToArray();
		int[] array3 = new int[4];
		int[] array4 = new int[4];
		for (int num = 0; num < 4; num++)
		{
			array3[num] = Convert.ToInt32(array[num], 2) & Convert.ToInt32(array2[num], 2);
		}
		for (int num2 = 0; num2 < 4; num2++)
		{
			array4[num2] = array3[num2] ^ (~Convert.ToInt32(array2[num2], 2) & 0xFF);
		}
		return IPAddress.Parse(string.Join(".", array4.Select((int x) => x.ToString())));
	}

	public static string DecryptText(string input, string password)
	{
		byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		bytes = SHA256.Create().ComputeHash(bytes);
		byte[] bytes2 = AES_Decrypt(bytesToBeDecrypted, bytes);
		return Encoding.UTF8.GetString(bytes2);
	}

	public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
	{
		byte[] array = null;
		byte[] salt = new byte[8] { 19, 9, 94, 1, 94, 9, 19, 2 };
		using MemoryStream memoryStream = new MemoryStream();
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 256;
		rijndaelManaged.BlockSize = 128;
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		rijndaelManaged.Mode = CipherMode.CBC;
		using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
		{
			cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
			cryptoStream.Close();
		}
		return memoryStream.ToArray();
	}

	public static string EncryptText(string input, string password)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(input);
		byte[] bytes2 = Encoding.UTF8.GetBytes(password);
		bytes2 = SHA256.Create().ComputeHash(bytes2);
		return Convert.ToBase64String(AES_Encrypt(bytes, bytes2));
	}

	public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
	{
		byte[] array = null;
		byte[] salt = new byte[8] { 19, 9, 94, 1, 94, 9, 19, 2 };
		using MemoryStream memoryStream = new MemoryStream();
		using RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.KeySize = 256;
		rijndaelManaged.BlockSize = 128;
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
		rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
		rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
		rijndaelManaged.Mode = CipherMode.CBC;
		using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
		{
			cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
			cryptoStream.Close();
		}
		return memoryStream.ToArray();
	}
}
