﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing;

namespace LinqToElk
{
    public class GeneratorExpressionTreeVisitor : ThrowingExpressionVisitor
    {
        private static IList<QueryContainer> _queryContainers = new List<QueryContainer>();

        public string Field { get; set; }
        public string Value { get; set; }

        public static List<QueryContainer> GetNestExpression(Expression linqExpression)
        {
            var visitor = new GeneratorExpressionTreeVisitor();
            visitor.Visit(linqExpression);
            return visitor.GetNestExpression();
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            // _hqlExpression.Append("(");
            //
            Visit(expression.Left);
            //
            // // In production code, handle this via lookup tables.
            // switch (expression.NodeType)
            // {
            //     case ExpressionType.Equal:
            //         _hqlExpression.Append(" = ");
            //         break;
            //
            //     case ExpressionType.AndAlso:
            //     case ExpressionType.And:
            //         _hqlExpression.Append(" and ");
            //         break;
            //
            //     case ExpressionType.OrElse:
            //     case ExpressionType.Or:
            //         _hqlExpression.Append(" or ");
            //         break;
            //
            //     case ExpressionType.Add:
            //         _hqlExpression.Append(" + ");
            //         break;
            //
            //     case ExpressionType.Subtract:
            //         _hqlExpression.Append(" - ");
            //         break;
            //
            //     case ExpressionType.Multiply:
            //         _hqlExpression.Append(" * ");
            //         break;
            //
            //     case ExpressionType.Divide:
            //         _hqlExpression.Append(" / ");
            //         break;
            //
            //     default:
            //         base.VisitBinaryExpression(expression);
            //         break;
            // }
            //
            Visit(expression.Right);
            // _hqlExpression.Append (")");
            //


            if (expression.NodeType == ExpressionType.Equal)
            {
                _queryContainers.Add(new MatchPhraseQuery()
                {
                    Field = $"{Field}.keyword",
                    Query = Value
                });
            }
            
            return expression;
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            Value = (string) expression.Value;
            return expression;
        }
        
        protected override Expression VisitMember(MemberExpression expression)
        {
            Field = expression.Member.Name.ToLower();
            return expression;
        }


        public List<QueryContainer> GetNestExpression()
        {
            return _queryContainers.ToList();
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            // string itemText = FormatUnhandledItem(unhandledItem);
            var itemText = "";
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.",
                itemText, typeof(T));
            return new NotSupportedException(message);
        }
    }
}