/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    main.hpp
 * @brief   main.hpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once
#include <string>
#include <map>

extern const int DoingVersion;


extern std::map<std::string,std::string> GolbalVarTable;

#ifdef _WIN32 
constexpr const char* DOINGOS = "__OS_WINDOW";
#elif defined(__linux__ )
constexpr const char* DOINGOS = "__OS_LINUX";
#else
#error("Unsupports OS")
#endif  