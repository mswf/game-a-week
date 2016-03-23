using System;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using Sprache;

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
			var lines = File.ReadAllLines(fullPath);

			ParseLines(ref lines, relativePath);
		}

		private enum ParseStep
		{
			Using,
			Include,
			LookingForTree,
			ParsingTree
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

		public void ParseLines(ref string[] lines, string relativePath)
		{
			
			

			var currentParseStep = ParseStep.Using;

			var rawResult = new RawResult();
			rawResult.path = relativePath;

			for (int i = 0; i < lines.Length; i++)
			{
				var currentLine = lines[i];
				switch (currentParseStep)
				{
					case ParseStep.Using:
						break;
					case ParseStep.Include:
						break;
					case ParseStep.LookingForTree:
						break;
					case ParseStep.ParsingTree:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				Debug.Log(currentLine);
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
