using System.Security.Cryptography;

namespace USca_Server.Util
{
	public class CryptoUtil
	{
		public static bool VerifySignedMessage(byte[] hash, byte[] signature)
		{
			CspParameters csp = new CspParameters();
			csp.KeyContainerName = "USca_RTU_Key";
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(csp))
			{
				var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
				deformatter.SetHashAlgorithm("SHA256");
				return deformatter.VerifySignature(hash, signature);
			}
		}

		public static void ImportPublicKey(string path, string keyStoreName)
		{
			FileInfo fi = new FileInfo(path);
			if (fi.Exists)
			{
				using (StreamReader reader = new StreamReader(path))
				{
					var csp = new CspParameters();
					csp.KeyContainerName = keyStoreName;
					var rsa = new RSACryptoServiceProvider(csp);
					string publicKeyText = reader.ReadToEnd();
					rsa.FromXmlString(publicKeyText);
					rsa.PersistKeyInCsp = true;
				}
			}
		}
	}
}
