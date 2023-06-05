namespace USca_Server.Measures
{
	public class MeasureFromRtuDTO
	{
		public int Address { get; set; }
		public string Name { get; set; }
		public double Value { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
