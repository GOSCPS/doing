/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    doing.hpp
 * Content: doing header file
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#ifndef __DOING_HPP_
#define __DOING_HPP_

#include <iostream>
#include <map>
#include <string>
#include <fstream>
#include <sstream>
#include <utility>
#include <set>
#include <list>
#include <deque>
#include <optional>
#include <vector>
#include <bitset>
#include <initializer_list>
#include <string_view>
#include <variant>
#include <exception>

/**
 * @brief 全局变量
*/
extern std::map<std::string, std::string> GLOBAL_VARS;

/**
 * @brief 文件名称
*/
extern std::string FILE_NAME;

/**
 * @brief 构建目标
*/
extern std::vector<std::string> AIMS_TARGET;

/**
 * @brief 构建选项
*/
extern std::vector<std::string> OPTIONS;

//默认构建名称
constexpr const char const* Default_Build_File_Name = "make.doing";

/**
 * @brief 构建
*/
int doing_build();

#endif __DOING_HPP_