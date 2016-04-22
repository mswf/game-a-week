
using System.Collections;
using System.Collections.Generic;

using BehaviorTree.DefinitionReader.Lexing;

namespace BehaviorTree.DefinitionReader.Parsing
{
	public class Parser
	{
		public Parser()
		{
			
		}

		public AbstractSyntaxTree Parse (IEnumerable<Token> tokens)
		{
			var abstractSyntaxTree = new AbstractSyntaxTree();
			
			return abstractSyntaxTree;
		}


	}
}

