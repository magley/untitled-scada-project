﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace USca_RTU.Util
{
	internal class CryptoUtil
	{
		public static string ContainerName { get; private set; } = "USca_RTU_Key";

		public static byte[] SignMessage(object o, out byte[] hashValue)
		{
			string message = JsonSerializer.Serialize(o);

			using (SHA256 sha = SHA256.Create())
			{
				hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
				CspParameters csp = new CspParameters();
				csp.KeyContainerName = ContainerName;
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp);
				var formatter = new RSAPKCS1SignatureFormatter(rsa);
				formatter.SetHashAlgorithm("SHA256");
				return formatter.CreateSignature(hashValue);
			}
		}
	}
}
