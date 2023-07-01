namespace USca_Server.Tags
{
    public class TagDTO
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = "";
        public string Desc { get; set; } = "";
        public TagMode Mode { get; set; }
        public TagType Type { get; set; }
        public int Address { get; set; }
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 10.0;
        public string Unit { get; set; } = "";
        public int ScanTime { get; set; } = 1000;
    }
}
