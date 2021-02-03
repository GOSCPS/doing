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
#include <vector>
#include "run.hpp"
#include "rule.hpp"
#include "target.hpp"

int Build(std::string fileName,std::set<std::string> tagrets);
std::string ReadNextWord(std::string::iterator &begin,std::string::iterator &end);

class Builder{
    public:
    std::vector<Target> TargetList;
    std::vector<Rule> RuleList;

    int BuildTarget(std::string target);

    private:
    int BuildRun(Run run);
    int BuildRule(Rule rule,std::map<std::string,std::string> with);
};