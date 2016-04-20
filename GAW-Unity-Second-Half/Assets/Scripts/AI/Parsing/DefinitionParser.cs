using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BehaviorTree
{
	public class DefinitionParser
	{
		public DefinitionParser()
		{
			
		}

		public void ParseTestFile()
		{
			ParseFile("TestTree.txt");
		}
		// https://github.com/sprache/Sprache
		//http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/
		public void ParseFile(string relativePath)
		{
			var fullPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "BehaviorTrees" +
			               Path.DirectorySeparatorChar + relativePath;
			var lines = File.ReadAllText (fullPath);

			ParseLines(ref lines, relativePath);
		}

		/*
		private static readonly Parser<string> StripWhiteSpace =
			from leading in Parse.WhiteSpace.Many()
			from first in Parse.Letter.Once()
			from rest in Parse.LetterOrDigit.Many()
			from trailing in Parse.WhiteSpace.Many()
			select new string(first.Concat(rest).ToArray());

		private static readonly Parser<string> GetUsingDirective =
			from leading in Parse.Chars("using")
			from usingDirective in Parse.String()
			from trailing in Parse.Char(";")
			select usingDirective;

		*/

		//http://jakubdziworski.github.io/enkel/2016/03/11/enkel_2_technology.html

		public void ParseLines(ref string text, string relativePath)
		{
			int currentIndex = 0;
			int currentLine = 1;
			int currentColumn = 0;

			var rawResult = new RawResult();
			rawResult.path = relativePath;

			var tokenizer = new Tokenizer();

			tokenizer.AddDefinition(new TokenDefinition(
				"(comment)",
				new Regex(@"//.*"),
				true));

			/*
			https://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx
			https://msdn.microsoft.com/en-us/library/h21280bw.aspx
			http://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
			*/

			tokenizer.AddDefinition(new TokenDefinition(
				"(multiline-comment)",
				new Regex(@"\/\*(.|\n)*?\*\/"),
				true));
			
			tokenizer.AddDefinition(new TokenDefinition(
				"(string)",
				new Regex("\"[^\"]*\"|\'[^\']*\'")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(assignment)",
				new Regex(@"\=")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(parenthesis)",
				new Regex(@"\(|\)")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(bracket)",
				new Regex(@"\[|\]")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(operator)",
				new Regex(@"\*|\/|\+|\-|\.\.")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(dot)",
				new Regex(@"\.")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(colon)",
				new Regex(@"\:")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(comma)",
				new Regex(@"\,")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(number)",
				new Regex(@"\d+")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(tree-reference)",
				new Regex(@"\$\w+")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(using-reference)",
				new Regex(@"using [\w\.\d]+")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(include-reference)",
				new Regex(@"include [\w\.\d]+")));

			tokenizer.AddDefinition(new TokenDefinition(
				"(variable)",
				new Regex(@"\w+")));


			// TODO: tokenize tab chars
			///*
			tokenizer.AddDefinition(new TokenDefinition(
				"(indentation)",
				new Regex(@"\t+"),
				false));
			//*/
				
			tokenizer.AddDefinition(new TokenDefinition(
				"(white-space)",
				new Regex(@"\s+"),
				true));

			tokenizer.AddDefinition(new TokenDefinition(
				"(semicolon)",
				new Regex(@";")));

		//	Debug.Log(text);

			var tokens = tokenizer.Tokenize(text);

			var tokenList = new List<Token>();

			foreach (var token in tokens)
			{
				tokenList.Add(token);
				Debug.Log(token);
			}

			//tokenList = tokenList;
		}

		public class Token
		{
			public readonly string type;
			public readonly string value;
			public readonly TokenPosition position;

			public Token(string type, string value, int index, int line, int column)
			{
				this.type = type;
				this.value = value;

				this.position = new TokenPosition(index, line, column);
			}

			public Token(string type, string value, TokenPosition position)
			{
				this.type = type;
				this.value = value;
				this.position = position;
			}

			public override string ToString()
			{
				return string.Format("Token: {{ Type: \"{0}\", Value: \"{1}\", Position: {{ Index: \"{2}\", Line: \"{3}\", Column: \"{4}\" }} }}", type, value, position.Index, position.Line, position.Column);
			}
		}

		public class TokenPosition
		{
			public TokenPosition(int index, int line, int column)
			{
				Index = index;
				Line = line;
				Column = column;
			}

			public int Column { get; private set; }
			public int Index { get; private set; }
			public int Line { get; private set; }

			public override string ToString()
			{
				return string.Format("Position: {{ Index: \"{0}\", Line: \"{1}\", Column: \"{2}\" }}", Index, Line, Column);
			}
		}

		public class TokenDefinition
		{
			public TokenDefinition(
				string type,
				Regex regex)
				: this(type, regex, false)
			{
			}

			public TokenDefinition(
				string type,
				Regex regex,
				bool isIgnored)
			{
				Type = type;
				Regex = regex;
				IsIgnored = isIgnored;
			}

			public bool IsIgnored { get; private set; }
			public Regex Regex { get; private set; }
			public string Type { get; private set; }
		}

		public class Tokenizer
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
						throw new Exception(string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).", source[currentIndex], currentIndex, currentLine, currentColumn));
					}
					else
					{
						var value = source.Substring(currentIndex, matchLength);

						if (!matchedDefinition.IsIgnored)
							yield return new Token(matchedDefinition.Type, value, new TokenPosition(currentIndex, currentLine, currentColumn));

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

				yield return new Token("(end)", null, new TokenPosition(currentIndex, currentLine, currentColumn));
			}
		}


		public class RawResult
		{
			public class RawTree
			{
				public RawLine firstLine;

				public string name;
			}


			public class RawLine
			{
				public List<RawExpression> guards;
				public RawExpression node;

				public List<RawLine> children;

				public RawLine()
				{
					node = new RawExpression();
					guards = new List<RawExpression>();

					children = new List<RawLine>();
				}
			}

			// Node or Guard
			public class RawExpression
			{
				public string expressionName;
				public List<string> parameters;

				public RawExpression()
				{
					parameters = new List<string>();
				}
			}

			public string path;

			public List<string> usings;
			public List<string> includes;


			public RawTree root;
			public Dictionary<string, RawTree> subTrees;

			public RawResult()
			{
				usings = new List<string>();
				includes = new List<string>();

				root = new RawTree();
				subTrees = new Dictionary<string, RawTree>();
			}
		}
	}
}
