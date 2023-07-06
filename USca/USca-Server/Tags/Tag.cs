using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using USca_Server.Alarms;

namespace USca_Server.Tags
{
    public enum TagMode
    {
        Input,
        Output
    }

    public enum TagType
    {
        Digital,
        Analog
    }

    public class Tag
	{
		[Key]
		public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Desc { get; set; } = "";
        public TagMode Mode { get; set; }
        public TagType Type { get; set; }
        public int Address { get; set; }
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 10.0;
        public string Unit { get; set; } = "";
        public int ScanTime { get; set; } = 1000;
        public bool IsScanning { get; set; } = true;
        [JsonIgnore]
        public virtual List<Alarm> Alarms { get; set; } = new();

        public Tag()
        {

        }

        public Tag(TagAddDTO dto)
        {
            Name = dto.Name;
            Desc = dto.Desc;
            Mode = dto.Mode;
            Type = dto.Type;
            Address = dto.Address;
            Min = dto.Min;
            Max = dto.Max;
            Unit = dto.Unit;
            ScanTime = dto.ScanTime;
            IsScanning = dto.IsScanning;
        }
    }
}
