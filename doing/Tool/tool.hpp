/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * ����ļ����� GOSCPS(https://github.com/GOSCPS)
 * ʹ�� GOSCPS ���֤
 * File:    tool.hpp
 * Content: doing tool header file
 * Copyright (c) 2020-2021 GOSCPS ��������Ȩ��.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include "../doing.hpp"

std::string operator"" _green(const char const* str, size_t len);
std::string operator"" _red(const char const* str, size_t len);
std::string operator"" _yellow(const char const* str, size_t len);
std::string operator"" _error(const char const* str, size_t len);