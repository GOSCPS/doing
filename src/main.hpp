/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * THIS FILE IS FROM GOSCPS(goscps@foxmail.com)
 * IS LICENSED UNDER GOSCPS
 * File:     main.hpp
 * Content:  doing main c++ head file
 * Copyright (c) 2020 GOSCPS All rights reserved.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include <cstring>
#include <fstream>
#include <functional>
#include <iostream>
#include <map>
#include <memory>
#include <optional>
#include <regex>
#include <set>
#include <string>
#include <string_view>
#include <utility>
#include <vector>

enum class PlatformOs : uint32_t {
  Window32, // No supports
  Window64,
  Linux64,
  Mac64
};

constexpr const char *Color_Red = "\033[31m";
constexpr const char *Color_Yellow = "\033[33m";
constexpr const char *Color_Green = "\033[32m";
constexpr const char *Color_Clear = "\033[0m";

#if defined(WIN32)
#if defined(WIN64)
constexpr const char *CompilePlatform = "WIN64";
#else
constexpr const char *CompilePlatform = "WIN32";
#endif

#elif defined(__linux__)
constexpr const char *CompilePlatform = "LINUX";

#elif defined(__APPLE__) || defined(TARGET_OS_MAC)
constexpr const char *CompilePlatform = "MAC";

#else
constexpr const char *CompilePlatform = "UNKNOWN";
#endif