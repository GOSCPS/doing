/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    parser.hpp
 * Content: doing parser header file
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include "../doing.hpp"

namespace doing {
		
	/**
	 * @brief Token类型
	*/
	enum class TokenType : int {
		// 空TYPE
		_null_type_,

		// 标识符
		identifier,

		// + - * /
		sign_add,
		sign_sub,
		sign_mul,
		sign_div,

		// ( ) { }
		sign_brackets,
		sign_brackets_end,
		sign_big_brackets,
		sign_big_brackets_end,

		// $
		sign_dollar,

		// 字符串
		string,

		// 数字
		number,

		// =
		// < <= 
		// == != 
		// >= >
		sign_equal,

		sign_bigger,
		sign_bigger_equal,

		sign_double_equal,
		sign_not_equal,

		sign_smaller_equal,
		sign_smaller,

		// ! | &
		sign_not,
		sign_or,
		sign_and,

		// ;
		sign_semicolon
	};

	/**
	 * @brief Token
	*/
	struct Token {
		// 默认无Type
		TokenType type = TokenType::_null_type_;

		// 值
		// 数字or字符串
		std::variant<std::string,long long> a;
	};

	/**
	 * @brief Token流
	*/
	class TokenStream {
	public:

		TokenStream(std::list<Token>& tokens) {
			this->tokens = tokens;
			now_it = this->tokens.cbegin();
		}

		// 下一个
		void next() {
			now_it++;
		}

		// 获取
		Token get(std::string errorMsg = "Index out of token stream!") {
			// 到达末尾
			if (now_it == tokens.cend())
				throw std::runtime_error(errorMsg);

			// 返回
			else
				return *now_it;
		}

		// 上一个
		void back(std::string errorMsg = "Index out of token stream!") {
			if (now_it != tokens.cbegin())
				now_it--;

			else
				throw std::runtime_error(errorMsg);
		}

		// 在当前位置往后添加Tokens
		void push_nows(std::list<Token>& tokens) {

			auto backer_it = now_it;

			for (auto &t : tokens) {
				tokens.insert(backer_it, t);
				backer_it++;
			}

			return;
		}

		// 是否在末尾（不可读）
		bool isEnd() {
			if (now_it == tokens.cend())
				return true;
			else return false;
		}

		// 是否有可读的下一个
		bool hasNext() {
			auto it = now_it;

			if ((it++) == tokens.cend())
				return false;
			else return true;
		}

	private:
		std::list<Token>::const_iterator now_it;
		std::list<Token> tokens;
	};




}