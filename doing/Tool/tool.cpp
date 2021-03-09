/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * ����ļ����� GOSCPS(https://github.com/GOSCPS)
 * ʹ�� GOSCPS ���֤
 * File:    tool.cpp
 * Content: doing tool source file
 * Copyright (c) 2020-2021 GOSCPS ��������Ȩ��.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include "../doing.hpp"
#include "tool.hpp"

std::string operator"" _red(const char const* str, size_t len) {
	std::stringstream output;
	output << "\x1b[31m" << str << "\x1b[0m";
	return output.str();
}

std::string operator"" _green(const char const* str, size_t len) {
	std::stringstream output;
	output << "\x1b[32m" << str << "\x1b[0m";
	return output.str();
}

std::string operator"" _yellow(const char const* str, size_t len) {
	std::stringstream output;
	output << "\x1b[33m" << str << "\x1b[0m";
	return output.str();
}

std::string operator"" _error(const char const* str, size_t len) {
	std::stringstream output;
	output << "\x1b[31m\x1b[1m" << str << "\x1b[0m";
	return output.str();
}
