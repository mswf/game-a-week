
using System.Collections.Generic;

using BehaviorTree.DefinitionReader.Lexing;


namespace BehaviorTree.DefinitionReader.Parsing
{
	public class AbstractSyntaxTree
	{
		public string filePath;

		private List<Token> usingStatements;
		private List<Token> includeStatements;

		private Dictionary<string, AbstractTreeDefinition> treeDefinitions; 

		public AbstractSyntaxTree()
		{
			
		}

	}

}
