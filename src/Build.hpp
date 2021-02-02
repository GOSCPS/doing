/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    build.hpp
 * @brief   build.hpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
#pragma once

#include <string>
#include <set>
#include <map>

//#基础语法
//Version = 0
//
//for OS_WINDOWS
//CC = cl
//LINKER = ld-link
//end for
//
//for OS_LINUX
//CC = clang
//LINKER = ld.lld
//end for
//
//rule CompileC
//commandline = ${CC} ${CCFLAGS} ${Source}
//Introduction = CompileC
//depend = ${CC}
//end rule
//
//target doing need SomeDepend
//run CompileC with 
//Source = main.c
//end with
//end target

int Build(std::string fileName,std::set<std::string> tagrets);