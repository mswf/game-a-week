using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using Sprache;

namespace TestSprache
{
	/// <summary>
	/// Simple calculator grammar.
	/// Supports arithmetic operations and parentheses.
	/// </summary>
	public class SimpleCalculator
	{
		protected internal virtual Parser<string> DecimalWithoutLeadingDigits
		{
			get
			{
				//throw new System.NotImplementedException();

				///*
				return
					from dot in Parse.Char('.')
					from fraction in Parse.Number
					select dot + fraction;

				//*/
			}
		}

		protected internal virtual Parser<string> DecimalWithLeadingDigits
		{
			get
			{
				throw new System.NotImplementedException();

//				return Parse.Number.Then(n => DecimalWithoutLeadingDigits.XOr(Parse.Return(string.Empty)).Select(f => n + f));
			}
		}

		protected internal virtual Parser<string> Decimal
		{
			get
			{
				//throw new System.NotImplementedException();
				return DecimalWithLeadingDigits.XOr(DecimalWithoutLeadingDigits);
			}
		}

		protected internal virtual Parser<Expression> Constant
		{
			get
			{
				//throw new System.NotImplementedException();
				return Decimal.Select(x => Expression.Constant(double.Parse(x, CultureInfo.InvariantCulture))).Named("Constant");
			}
		}

		protected internal Parser<ExpressionType> Operator(string op, ExpressionType opType)
		{
			//throw new System.NotImplementedException();

			return Parse.String(op).Token().Return(opType);
		}

		protected internal virtual Parser<ExpressionType> Add
		{
			get
			{
				//throw new System.NotImplementedException();
				return Operator("+", ExpressionType.AddChecked);
			}
		}

		protected internal virtual Parser<ExpressionType> Subtract
		{
			get
			{
				//throw new System.NotImplementedException();
				return Operator("-", ExpressionType.SubtractChecked);
			}
		}

		protected internal virtual Parser<ExpressionType> Multiply
		{
			get
			{
				//throw new System.NotImplementedException();
				return Operator("*", ExpressionType.MultiplyChecked);
			}
		}

		protected internal virtual Parser<ExpressionType> Divide
		{
			get
			{
				//throw new System.NotImplementedException();
				return Operator("/", ExpressionType.Divide);
			}
		}

		protected internal virtual Parser<ExpressionType> Modulo
		{
			get
			{
				//throw new System.NotImplementedException();
				return Operator("%", ExpressionType.Modulo);
			}
		}

		protected internal virtual Parser<ExpressionType> Power
		{
			get
			{
				//throw new System.NotImplementedException();
				return Operator("^", ExpressionType.Power);
			}
		}

		protected virtual Parser<Expression> ExpressionInParentheses
		{
			get
			{
				//throw new System.NotImplementedException();

				///*
				return
					from lparen in Parse.Char('(')
					from expr in Expr
					from rparen in Parse.Char(')')
					select expr;
				//*/
			}
		}

		protected internal virtual Parser<Expression> Factor
		{
			get
			{
				//throw new System.NotImplementedException();
				return ExpressionInParentheses.XOr(Constant);
			}
		}

		protected internal virtual Parser<Expression> NegativeFactor
		{
			get
			{
				//throw new System.NotImplementedException();
				///*
				return
					from sign in Parse.Char('-')
					from factor in Factor
					select Expression.NegateChecked(factor);
				//*/
			}
		}

		protected internal virtual Parser<Expression> Operand
		{
			get
			{
				//throw new System.NotImplementedException();

				return (NegativeFactor.XOr(Factor)).Token();
			}
		}

		protected internal virtual Parser<Expression> InnerTerm
		{
			get
			{
				//throw new System.NotImplementedException();

				return Parse.ChainRightOperator(Power, Operand, Expression.MakeBinary);
			}
		}

		protected internal virtual Parser<Expression> Term
		{
			get
			{
			//	throw new System.NotImplementedException();

				return Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), InnerTerm, Expression.MakeBinary);
			}
		}

		protected internal virtual Parser<Expression> Expr
		{
			get
			{
			//	throw new System.NotImplementedException();

				return Parse.ChainOperator(Add.Or(Subtract), Term, Expression.MakeBinary);
			}
		}

		protected internal virtual Parser<LambdaExpression> Lambda
		{
			get
			{
//				throw new System.NotImplementedException();
				return Expr.End().Select(body => Expression.Lambda<Func<double>>(body)); 		
			}
		}

		public virtual Expression<Func<double>> ParseExpression(string text)
		{
			return Lambda.Parse(text) as Expression<Func<double>>;
		}
	}
}
