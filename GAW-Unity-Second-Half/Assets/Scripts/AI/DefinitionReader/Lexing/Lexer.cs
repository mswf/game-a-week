using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BehaviorTree.DefinitionReader.Lexing
{
	public enum TokenType
	{
		Comment,

		String,
		Assignment,

		Parenthesis_Open,
		Parenthesis_Close,
		Bracket_Open,
		Bracket_Close,

		Operator,
		Dot,
		Colon,
		Comma,
		Number,

		Tree_Reference,
		Using_Reference,
		Include_Reference,
		Variable,

		Indentation,
		Whitespace,
		Newline,
		Semicolon,

		// Special
		End
	}

	public class Lexer
	{
		//	Regex endOfLineRegex = new Regex(@"\r\n|\r|\n", RegexOptions.Compiled);
		Regex endOfLineRegex = new Regex(@"\r\n|\r|\n");
		IList<TokenDefinition> tokenDefinitions = new List<TokenDefinition>();

		public void AddDefinition(TokenDefinition tokenDefinition)
		{
			tokenDefinitions.Add(tokenDefinition);
		}

		public IEnumerable<Token> Tokenize(string source)
		{
			int currentIndex = 0;
			int currentLine = 1;
			int currentColumn = 0;

			while (currentIndex < source.Length)
			{
				TokenDefinition matchedDefinition = null;
				int matchLength = 0;

				foreach (var rule in tokenDefinitions)
				{
					var match = rule.Regex.Match(source, currentIndex);

					if (match.Success && (match.Index - currentIndex) == 0)
					{
						matchedDefinition = rule;
						matchLength = match.Length;
						break;
					}
				}

				if (matchedDefinition == null)
				{
					throw new Exception(string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).",
						source[currentIndex], currentIndex, currentLine, currentColumn));
				}
				else
				{
					var value = source.Substring(currentIndex, matchLength);

					if (!matchedDefinition.IsIgnored)
						yield return new Token(matchedDefinition.Type, value, new TokenPosition(currentIndex, currentLine, currentColumn))
							;

					var endOfLineMatch = endOfLineRegex.Match(value);
					if (endOfLineMatch.Success)
					{
						currentLine += 1;
						currentColumn = value.Length - (endOfLineMatch.Index + endOfLineMatch.Length);
					}
					else
					{
						currentColumn += matchLength;
					}

					currentIndex += matchLength;
				}
			}

			yield return new Token(TokenType.End, null, new TokenPosition(currentIndex, currentLine, currentColumn));
		}
	}
}
