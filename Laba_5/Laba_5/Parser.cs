using System;
using System.Collections.Generic;
using System.Text;

namespace Laba_5
{
    internal enum TokenType
    {
        None,
        Plus,
        Minus,
        Multiply,
        Divide,
        Number,
        LeftParenthesis,
        RightParenthesis
    }
    internal class Parser
    {
        private Token curToken;
        private int curPos;
        private int charCount;
        private char curChar;
        public string Text { get; private set; }

        public Parser(string text)
        {
            this.Text = string.IsNullOrEmpty(text) ? string.Empty : text;
            this.charCount = this.Text.Length;
            this.curToken = Token.None();

            this.curPos = -1;
            this.Advance();
        }

        internal Expression Parse()
        {
            this.NextToken();
            Expression node = this.GrabExpr();
            this.ExpectToken(TokenType.None);
            return node;
        }

        private Token ExpectToken(TokenType tokenType)
        {
            if (this.curToken.Type == tokenType)
            {
                return this.curToken;
            }
            else
            {
                throw new InvalidSyntaxException(string.Format("Invalid syntax at position {0}. Expected {1} but {2} is given.", this.curPos, tokenType, this.curToken.Type.ToString()));
            }
        }

        private Expression GrabExpr()
        {
            Expression left = this.GrabTerm();

            while (this.curToken.Type == TokenType.Plus
                || this.curToken.Type == TokenType.Minus)
            {
                Token op = this.curToken;
                this.NextToken();
                Expression right = this.GrabTerm();
                left = new BinOp(op, left, right);
            }

            return left;
        }

        private Expression GrabTerm()
        {
            Expression left = this.GrabFactor();

            while (this.curToken.Type == TokenType.Multiply
                || this.curToken.Type == TokenType.Divide)
            {
                Token op = this.curToken;
                this.NextToken();
                Expression right = this.GrabFactor();
                left = new BinOp(op, left, right);
            }

            return left;
        }

        private Expression GrabFactor()
        {
            if (this.curToken.Type == TokenType.Plus
                || this.curToken.Type == TokenType.Minus)
            {
                Expression node = this.GrabUnaryExpr();
                return node;
            }
            else if (this.curToken.Type == TokenType.LeftParenthesis)
            {
                Expression node = this.GrabBracketExpr();
                return node;
            }
            else
            {
                Token token = this.ExpectToken(TokenType.Number);
                this.NextToken();
                return new Num(token);
            }
        }

        private Expression GrabUnaryExpr()
        {
            Token op;

            if (this.curToken.Type == TokenType.Plus)
            {
                op = this.ExpectToken(TokenType.Plus);
            }
            else
            {
                op = this.ExpectToken(TokenType.Minus);
            }

            this.NextToken();

            if (this.curToken.Type == TokenType.Plus
                || this.curToken.Type == TokenType.Minus)
            {
                Expression expr = this.GrabUnaryExpr();
                return new UnaryOp(op, expr);
            }
            else
            {
                Expression expr = this.GrabFactor();
                return new UnaryOp(op, expr);
            }
        }

        private Expression GrabBracketExpr()
        {
            this.ExpectToken(TokenType.LeftParenthesis);
            this.NextToken();
            Expression node = this.GrabExpr();
            this.ExpectToken(TokenType.RightParenthesis);
            this.NextToken();
            return node;
        }

        private void NextToken()
        {
            if (this.curChar == char.MinValue)
            {
                this.curToken = Token.None();
                return;
            }

            if (this.curChar == ' ')
            {
                while (this.curChar != char.MinValue && this.curChar == ' ')
                {
                    this.Advance();
                }

                if (this.curChar == char.MinValue)
                {
                    this.curToken = Token.None();
                    return;
                }
            }

            if (this.curChar == '+')
            {
                this.curToken = new Token(TokenType.Plus, this.curChar.ToString());
                this.Advance();
                return;
            }

            if (this.curChar == '-')
            {
                this.curToken = new Token(TokenType.Minus, this.curChar.ToString());
                this.Advance();
                return;
            }

            if (this.curChar == '*')
            {
                this.curToken = new Token(TokenType.Multiply, this.curChar.ToString());
                this.Advance();
                return;
            }

            if (this.curChar == '/')
            {
                this.curToken = new Token(TokenType.Divide, this.curChar.ToString());
                this.Advance();
                return;
            }

            if (this.curChar == '(')
            {
                this.curToken = new Token(TokenType.LeftParenthesis, this.curChar.ToString());
                this.Advance();
                return;
            }

            if (this.curChar == ')')
            {
                this.curToken = new Token(TokenType.RightParenthesis, this.curChar.ToString());
                this.Advance();
                return;
            }

            if (this.curChar >= '0' && this.curChar <= '9')
            {
                string num = string.Empty;
                while (this.curChar >= '0' && this.curChar <= '9')
                {
                    num += this.curChar.ToString();
                    this.Advance();
                }

                if (this.curChar == '.')
                {
                    num += this.curChar.ToString();
                    this.Advance();

                    if (this.curChar >= '0' && this.curChar <= '9')
                    {
                        while (this.curChar >= '0' && this.curChar <= '9')
                        {
                            num += this.curChar.ToString();
                            this.Advance();
                        }
                    }
                    else
                    {
                        throw new InvalidSyntaxException(string.Format("Invalid syntax at position {0}. Unexpected symbol {1}.", this.curPos, this.curChar));
                    }
                }

                this.curToken = new Token(TokenType.Number, num);
                return;
            }

            throw new InvalidSyntaxException(string.Format("Invalid syntax at position {0}. Unexpected symbol {1}.", this.curPos, this.curChar));
        }

        private void Advance()
        {
            this.curPos += 1;

            if (this.curPos < this.charCount)
            {
                this.curChar = this.Text[this.curPos];
            }
            else
            {
                this.curChar = char.MinValue;
            }
        }
    }

    internal abstract class Expression : INode
    {
        abstract public object Accept(INodeVisitor visitor);
    }

    internal class Num : Expression
    {
        internal Token Token { get; private set; }

        public Num(Token token)
        {
            this.Token = token;
        }

        override public object Accept(INodeVisitor visitor)
        {
            return visitor.VisitNum(this.Token);
        }
    }

    internal class UnaryOp : Expression
    {
        internal Token Op { get; private set; }
        internal Expression Node { get; private set; }

        public UnaryOp(Token op, Expression node)
        {
            this.Op = op;
            this.Node = node;
        }

        override public object Accept(INodeVisitor visitor)
        {
            return visitor.VisitUnaryOp(this.Op, this.Node);
        }
    }

    internal class BinOp : Expression
    {
        internal Token Op { get; private set; }
        internal Expression Left { get; private set; }
        internal Expression Right { get; private set; }

        public BinOp(Token op, Expression left, Expression right)
        {
            this.Op = op;
            this.Left = left;
            this.Right = right;
        }

        override public object Accept(INodeVisitor visitor)
        {
            return visitor.VisitBinOp(this.Op, this.Left, this.Right);
        }
    }
}
