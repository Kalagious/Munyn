using System.Collections.Generic;

namespace Munyn.ViewModels.Data
{
    public class NodeDto
    {
        public string Id { get; set; }
        public string NodeType { get; set; }
        public string NodeName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<NodePropertyDto> Properties { get; set; }
        public string ThemeColor1 { get; set; }
        public string ThemeColor2 { get; set; }
    }

    public class NodePropertyDto
    {
        public string PropertyName { get; set; }
        public bool IsVisableOnGraphNode { get; set; }
        public bool IsSecret { get; set; }
        public bool IsDeletable { get; set; }
        public int PropertyType { get; set; }
        public string Value { get; set; }

        // For list properties
        public List<NodePropertyDto> ListContent { get; set; }

        // For text properties
        public string TextContent { get; set; }

        // For executable properties
        public string ExecutableCommand { get; set; }
        public int ExecutableType { get; set; }

        // For command properties
        public string Command { get; set; }
        public string Description { get; set; }

        // For link properties
        public string Url { get; set; }
        public string DisplayText { get; set; }

        // For code properties
        public string Code { get; set; }
        public string Icon { get; set; }
    }
}
