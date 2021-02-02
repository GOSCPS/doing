/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    rule.hpp
 * @brief   rule.hpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include <string>

class Rule{
    public:
        std::string Name;
        std::string CommandLine;
        std::string Introduction;
        std::string Depend;
};