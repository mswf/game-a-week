
using System.Collections.Generic;
using JetBrains.Annotations;


namespace BehaviorTree.DefinitionReader.Parsing
{
	public class AbstractTreeDefinition
	{
		public readonly string name;
		public AbstractNode childNode;

		public AbstractTreeDefinition(string treeName)
		{
			this.name = treeName;
		}
	}

	public class AbstractNode
	{
		public readonly string nodeType;
		public List<KeyValuePair<string, string>> parameters;

		public List<AbstractNode> children;

		public AbstractNode(string nodeType)
		{
			this.nodeType = nodeType;
			this.parameters = new List<KeyValuePair<string, string>>();
			this.children = new List<AbstractNode>();
		}

		public void AddParameter(string parameterName, string parameterValue)
		{
			parameters.Add(new KeyValuePair<string, string>(parameterName, parameterValue));
		}

		public void AddChild(AbstractNode childNode)
		{
			children.Add(childNode);
		}
	}
}

