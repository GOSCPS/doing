/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * ����ļ����� GOSCPS(https://github.com/GOSCPS)
 * ʹ�� GOSCPS ���֤
 * File:    parser.hpp
 * Content: doing parser header file
 * Copyright (c) 2020-2021 GOSCPS ��������Ȩ��.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include "../doing.hpp"

namespace doing {
		
	/**
	 * @brief Token����
	*/
	enum class TokenType : int {
		// ��TYPE
		_null_type_,

		// ��ʶ��
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

		// �ַ���
		string,

		// ����
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
		// Ĭ����Type
		TokenType type = TokenType::_null_type_;

		// ֵ
		// ����or�ַ���
		std::variant<std::string,long long> a;
	};

	/**
	 * @brief Token��
	*/
	class TokenStream {
	public:

		TokenStream(std::list<Token>& tokens) {
			this->tokens = tokens;
			now_it = this->tokens.cbegin();
		}

		// ��һ��
		void next() {
			now_it++;
		}

		// ��ȡ
		Token get(std::string errorMsg = "Index out of token stream!") {
			// ����ĩβ
			if (now_it == tokens.cend())
				throw std::runtime_error(errorMsg);

			// ����
			else
				return *now_it;
		}

		// ��һ��
		void back(std::string errorMsg = "Index out of token stream!") {
			if (now_it != tokens.cbegin())
				now_it--;

			else
				throw std::runtime_error(errorMsg);
		}

		// �ڵ�ǰλ���������Tokens
		void push_nows(std::list<Token>& tokens) {

			auto backer_it = now_it;

			for (auto &t : tokens) {
				tokens.insert(backer_it, t);
				backer_it++;
			}

			return;
		}

		// �Ƿ���ĩβ�����ɶ���
		bool isEnd() {
			if (now_it == tokens.cend())
				return true;
			else return false;
		}

		// �Ƿ��пɶ�����һ��
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