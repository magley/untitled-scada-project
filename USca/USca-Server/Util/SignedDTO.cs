using Newtonsoft.Json;

namespace USca_Server.Util
{
	public class SignedDTO<T>
	{
		[JsonProperty("payload")]
		public T Payload { get; set; }
		[JsonProperty("signature")]
		public byte[] Signature { get; set; }
		[JsonProperty("hash")]
		public byte[] Hash { get; set; }
	}
}
