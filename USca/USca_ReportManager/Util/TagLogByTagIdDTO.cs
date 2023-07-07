using System.Collections.Generic;

namespace USca_ReportManager.Util
{
    public class TagLogByTagIdDTO
    {
        public string TagName { get; set; } = "";
        public List<TagLogDTO> Logs { get; set; }
    }
}
