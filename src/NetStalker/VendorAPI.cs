using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using NetStalker.Properties;

namespace NetStalker;

public class VendorAPI
{
	private static string Token;

	private static HttpClient HttpClient;

	private static bool HttpClientReady;

	private static void InitHttpClient()
	{
		HttpClient = new HttpClient();
		if (!string.IsNullOrEmpty(Token))
		{
			HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
		}
	}

	public static async Task<VendorClass> GetVendorInfo(string MAC)
	{
		if (string.IsNullOrEmpty(MAC))
		{
			return null;
		}
		if (string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(Settings.Default.APIToken))
		{
			Token = Settings.Default.APIToken;
		}
		if (!HttpClientReady)
		{
			InitHttpClient();
			HttpClientReady = true;
		}
		string cleanMac = MAC.Replace("-", ":");
		string text = ((cleanMac.Length >= 8) ? cleanMac.Substring(0, 8) : cleanMac);
		if (!string.IsNullOrEmpty(Token))
		{
			try
			{
				HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(new Uri("https://api.macvendors.com/v1/lookup/" + text));
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					VendorClass vendorClass = JsonSerializer.Deserialize<VendorClass>(await httpResponseMessage.Content.ReadAsStringAsync());
					if (vendorClass != null && vendorClass.data?.organization_name != null)
					{
						return vendorClass;
					}
				}
			}
			catch
			{
			}
		}
		try
		{
			using HttpClient fallbackClient = new HttpClient();
			HttpResponseMessage httpResponseMessage2 = await fallbackClient.GetAsync(new Uri("https://api.maclookup.app/v2/macs/" + cleanMac));
			if (httpResponseMessage2.IsSuccessStatusCode)
			{
				using JsonDocument jsonDocument = JsonDocument.Parse(await httpResponseMessage2.Content.ReadAsStringAsync());
				if (jsonDocument.RootElement.TryGetProperty("company", out var value))
				{
					string text2 = value.GetString();
					if (!string.IsNullOrEmpty(text2))
					{
						return new VendorClass
						{
							data = new VendorClass.Data
							{
								organization_name = text2
							}
						};
					}
				}
			}
		}
		catch
		{
		}
		return null;
	}
}
