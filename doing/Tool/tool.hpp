/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    tool.hpp
 * Content: doing tool header file
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include "../doing.hpp"

std::string operator"" _green(const char const* str, size_t len);
std::string operator"" _red(const char const* str, size_t len);
std::string operator"" _yellow(const char const* str, size_t len);
std::string operator"" _error(const char const* str, size_t len);