using System.ComponentModel.DataAnnotations;

namespace USca_Server.Measures
{
	public class Measure
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public double Value { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
