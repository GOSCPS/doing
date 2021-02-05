/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    run.hpp
 * @brief   run.hpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
#pragma once

#include <string>
#include <map>

class Run{
    public:
    std::string RunRule;
    std::map<std::string,std::string> RunWith;
};
