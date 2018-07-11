using System.Collections.Generic;
using System.Text;

namespace NanoIoC
{
	public sealed class GraphNode
	{
		public Registration Registration { get; internal set; }
		public IList<GraphNode> Dependencies { get;}
		
		public GraphNode(Registration registration)
		{
			this.Registration = registration;
			this.Dependencies = new List<GraphNode>();
		}

		public override string ToString()
		{
			return this.ToString(0);
		}
		public string ToString(int depth)
		{
			var builder = new StringBuilder();
			
			if(depth > 0)
				builder.Indent(depth).Append("-> ");

			builder.AppendLine(this.Registration.ToString());

			foreach (var dependency in this.Dependencies)
				builder.Append(dependency.ToString(depth + 1));

			return builder.ToString();
		}
	}
}