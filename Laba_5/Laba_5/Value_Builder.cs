using System;

namespace Лабораторная_5
{
    internal interface INode
    {
        object Accept(INodeVisitor visitor);
    }
    internal interface INodeVisitor
    {
        object VisitNum(Token num);
        object VisitUnaryOp(Token op, INode node);
        object VisitBinOp(Token op, INode left, INode right);
    }
    internal class ValueBuilder : INodeVisitor
    {
        public object VisitBinOp(Token op, INode left, INode right)
        {
            switch (op.Type)
            {
                case TokenType.Plus:
                    return (decimal)left.Accept(this) + (decimal)right.Accept(this);
                case TokenType.Minus:
                    return (decimal)left.Accept(this) - (decimal)right.Accept(this);
                case TokenType.Multiply:
                    return (decimal)left.Accept(this) * (decimal)right.Accept(this);
                case TokenType.Divide:
                    return (decimal)left.Accept(this) / (decimal)right.Accept(this);
                default:
                    throw new Exception(string.Format("Token of type {0} cannot be evaluated.", op.Type.ToString()));
            }
        }

        public object VisitNum(Token num)
        {
            return decimal.Parse(num.Value);
        }

        public object VisitUnaryOp(Token op, INode node)
        {
            switch (op.Type)
            {
                case TokenType.Plus:
                    return (decimal)node.Accept(this);
                case TokenType.Minus:
                    return -(decimal)node.Accept(this);
                default:
                    throw new Exception(string.Format("Token of type {0} cannot be evaluated.", op.Type.ToString()));
            }
        }
    }
}
