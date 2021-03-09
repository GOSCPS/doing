/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    tool.cpp
 * Content: doing tool source file
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
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
