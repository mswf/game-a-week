using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using BehaviorTree.DefinitionReader.Tokenizing;

namespace BehaviorTree.DefinitionReader
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
				TokenType.Comment,
				new Regex(@"//.*"),
				true));

			/*
			https://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx
			https://msdn.microsoft.com/en-us/library/h21280bw.aspx
			http://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
			*/

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Comment,
				new Regex(@"\/\*(.|\n)*?\*\/"),
				true));
			
			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.String,
				new Regex("\"[^\"]*\"|\'[^\']*\'")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Assignment,
				new Regex(@"\=")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Parenthesis_Open,
				new Regex(@"\(")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Parenthesis_Close,
				new Regex(@"\)")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Bracket_Open,
				new Regex(@"\[")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Bracket_Close,
				new Regex(@"\]")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Operator,
				new Regex(@"\*|\/|\+|\-|\.\.")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Dot,
				new Regex(@"\.")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Colon,
				new Regex(@"\:")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Comma,
				new Regex(@"\,")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Number,
				new Regex(@"\d+")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Tree_Reference,
				new Regex(@"\$\w+")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Using_Reference,
				new Regex(@"using [\w\.\d]+")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Include_Reference,
				new Regex(@"include [\w\.\d]+")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Variable,
				new Regex(@"\w+")));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Indentation,
				new Regex(@"\t+"),
				false));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Whitespace,
				new Regex(@"[^\S\r\n]+"),
				true));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Newline,
				new Regex(@"\r\n|\r|\n"),
				true));

			tokenizer.AddDefinition(new TokenDefinition(
				TokenType.Semicolon,
				new Regex(@";")));

			var tokens = tokenizer.Tokenize(text);

			var tokenList = new List<Token>();

			foreach (var token in tokens)
			{
				tokenList.Add(token);
				Debug.Log(token);
			}

			//tokenList = tokenList;
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
