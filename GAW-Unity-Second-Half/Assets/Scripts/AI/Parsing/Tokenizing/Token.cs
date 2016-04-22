using UnityEngine;
using System.Collections;

namespace BehaviorTree.DefinitionReader.Tokenizing
{
	public class Token
	{
		public readonly TokenType type;
		public readonly string value;
		public readonly TokenPosition position;

		public Token(TokenType type, string value, int index, int line, int column)
		{
			this.type = type;
			this.value = value;

			this.position = new TokenPosition(index, line, column);
		}

		public Token(TokenType type, string value, TokenPosition position)
		{
			this.type = type;
			this.value = value;
			this.position = position;
		}

		public override string ToString()
		{
			return
				string.Format(
					"Token: {{ Type: \"{0}\", Value: \"{1}\", Position: {{ Index: \"{2}\", Line: \"{3}\", Column: \"{4}\" }} }}", type,
					value, position.Index, position.Line, position.Column);
		}
	}
}
