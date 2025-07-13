using System.Collections.Generic;

namespace Munyn.ViewModels.Data
{
    public class ContextDto : NodeDto
    {
        public List<NodeDto> Nodes { get; set; }
        public List<PathDto> Paths { get; set; }
        public List<ContextDto> ChildrenContexts { get; set; }
    }
}
