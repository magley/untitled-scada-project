﻿namespace USca_Server.Measures
{
	public interface IMeasureService
	{
		public void PutBatch(List<MeasureFromRtuDTO> batch);
		public List<Measure> Get();
		public List<int> GetAddresses();
	}
}
