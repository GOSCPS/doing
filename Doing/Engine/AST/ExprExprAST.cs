/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    ExprExprAST.cs
 * Content: ExprExprAST Source File
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Doing.Engine.Utility;


namespace Doing.Engine.AST
{
    /// <summary>
    /// 二元运算符表达式AST
    /// </summary>
    public class ExprExprAST : IExprAST
    {
        public ExprExprAST(Token token) : base(token) { }

        /// <summary>
        /// 左值
        /// </summary>
        public IExprAST LFT = new NopAST(null);

        /// <summary>
        /// 右值
        /// </summary>
        public IExprAST RGH = new NopAST(null);

        /// <summary>
        /// 操作类型
        /// </summary>
        public TokenType op = TokenType.null_token;


        public override Variable Execute(Context context)
        {
            Variable Left = LFT.Execute(context);
            Variable Right = RGH.Execute(context);

            // 确保类型相同且为string 或 number
            if (Left.Type != Right.Type)
                throw new RuntimeException("The type of operation object is different! " +
                    $"Left is `{Left.Type}` and right is `{Right.Type}`", this);

            if (Left.Type == Variable.VariableType.NoType)
                throw new RuntimeException("Try to manipulate the `NoType` type!", this);

            if (Left.Type == Variable.VariableType.Boolean)
                throw new RuntimeException("Try to manipulate the `Boolean` type!", this);

            if (Left.Type == Variable.VariableType.Object)
                throw new RuntimeException("Try to manipulate the `Object` type!", this);

            // 开始运算
            Variable output = new Variable
            {
                Type = Left.Type
            };

            switch (output.Type)
            {
                // 数字支持+-*/
                case Variable.VariableType.Number:
                    output.ValueNumber = op switch
                    {
                        TokenType.add => Left.ValueNumber + Right.ValueNumber,
                        TokenType.sub => Left.ValueNumber - Right.ValueNumber,
                        TokenType.mul => Left.ValueNumber * Right.ValueNumber,
                        TokenType.div => Left.ValueNumber / Right.ValueNumber,
                        _ => throw new RuntimeException($"Unknown Expr Tpye `{op}` for `{output.Type}`", this),
                    };
                    break;

                // 字符串只支持+
                case Variable.VariableType.String:
                    output.ValueString = op switch
                    {
                        TokenType.add => Left.ValueString + Right.ValueString,
                        _ => throw new RuntimeException($"Unknown Expr Tpye `{op}` for `{output.Type}`", this),
                    };
                    break;

                default:
                    throw new RuntimeException($"Unknown Varibale Tpye `{output.Type}`", this);
            }

            return output;
        }

    }
}
