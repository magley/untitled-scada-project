using System.ComponentModel.DataAnnotations;

namespace USca_Server.Tags
{
	public enum TagDir
	{
		Input,
		Output
	}

	public class Tag
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";

		public int Address { get; set; }
		public TagDir Direction { get; set; }
	}
}
