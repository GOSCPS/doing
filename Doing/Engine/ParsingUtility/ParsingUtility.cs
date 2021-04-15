using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doing.Engine.ParsingUtility
{
    /// <summary>
    /// 解析单元
    /// </summary>
    static class ParsingUtility
    {

        /// <summary>
        /// 检查分号
        /// 检查成功则Next
        /// </summary>
        /// 
        /// <exception cref="CompileException">
        /// 检查失败见:<see cref="CheckTokenType(TokenMake, TokenType)"/>
        /// </exception>
        /// 
        /// <param name="token"></param>
        public static void CheckSemicolon(TokenMake token)
        {
            CheckTokenType(token, TokenType.semicolon);
            return;
        }

        /// <summary>
        /// 检查标识符
        /// 检查成功则Next
        /// </summary>
        /// 
        /// <exception cref="CompileException">
        /// 检查失败
        /// </exception>
        /// 
        /// <param name="token"></param>
        public static void CheckTokenType(TokenMake token,TokenType expect)
        {
            if (token.IsEnd())
                throw new CompileException($"Expect {expect:G} but get End-Of-Token!", token.GetLastToken());

            if (token.Current.type != expect)
                throw new CompileException($"Expect {expect:G} but get `{token.Current.type}`!", token.Current);
            token.Next();

            return;
        }

        /// <summary>
        /// 检查标识符
        /// 检查成功则Next
        /// </summary>
        /// 
        /// <exception cref="CompileException">
        /// 检查失败见:<see cref="CheckTokenType(TokenMake, TokenType)"/>
        /// </exception>
        /// 
        /// <param name="token"></param>
        /// <returns>返回检查到的标识符</returns>
        public static string GetIdentifier(TokenMake token)
        {
            CheckTokenType(token, TokenType.identifier);
            token.Back();

            string value = token.Current.ValueString;

            token.Next();

            return value;
        }

        /// <summary>
        /// 检查数字
        /// 检查成功则Next
        /// </summary>
        /// 
        /// <exception cref="CompileException">
        /// 检查失败见:<see cref="CheckTokenType(TokenMake, TokenType)"/>
        /// </exception>
        /// 
        /// <param name="token"></param>
        /// <returns>返回检查到的数字</returns>
        public static long GetNumber(TokenMake token)
        {
            CheckTokenType(token, TokenType.number);
            token.Back();

            long value = token.Current.ValueNumber;

            token.Next();

            return value;
        }

        /// <summary>
        /// 检查字符串
        /// 检查成功则Next
        /// </summary>
        /// 
        /// <exception cref="CompileException">
        /// 检查失败见:<see cref="CheckTokenType(TokenMake, TokenType)"/>
        /// </exception>
        /// 
        /// <param name="token"></param>
        /// <returns>返回检查到的字符串</returns>
        public static string GetString(TokenMake token)
        {
            CheckTokenType(token, TokenType.str);
            token.Back();

            string value = token.Current.ValueString;

            token.Next();

            return value;
        }




    }
}
