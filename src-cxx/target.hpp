/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    target.hpp
 * @brief   target.hpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#pragma once

#include <string>
#include <vector>
#include "run.hpp"

class Target{
  public:
  std::string  Name;
  std::vector<std::string> TargetDepend;
    std::vector<Run> runList;
};