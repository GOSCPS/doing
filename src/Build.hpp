/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * THIS FILE IS FROM GOSCPS(goscps@foxmail.com)
 * IS LICENSED UNDER GOSCPS
 * File:     Build.hpp
 * Content:  doing Build c++ head file
 * Copyright (c) 2020 GOSCPS All rights reserved.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
#pragma once

#include "main.hpp"

class Build {
public:
  Build &operator=(const Build &) = delete;
  Build(const Build &) = delete;
  Build(std::string_view File) { this->File = std::string(File); }
  ~Build() {}

  //开始构建
  int BuildStart();
  //设置构建目标
  void SetStartTarget(std::string s) { StartTarget = s; }
  //设置构建文件
  void SetBuildFile(std::string s) { File = s; }

  //添加预定义
  void AdvanceDefine(std::string Key, std::string Vulan) {
    //已存在则不添加
    if (Defines.find(Key) != Defines.cend()) {
      std::cout << Color_Yellow
                << "Warning At Command Param:Pretreatment Defined"
                << Color_Clear << std::endl;
      return;
    } else
      Defines.insert(std::make_pair(Key, Vulan));
    return;
  }

private:
  std::string File = "build.do";
  std::string StartTarget = "Main";
  int Pretreatment(std::string &Source);
  int Parser(std::string &Source);
  std::map<std::string, std::string> Defines;
  std::vector<bool> PretreatmentIgnore; //由PretreatmentIF设置，用于忽略

  //预处理If
  int PretreatmentIF(const std::string &Source, int Row);
  //预处理Define
  int PretreatmentDefine(const std::string &Source, int Row);
  //预处理Replace
  void DefineReplace(std::string &Source);
  //导入编译器变量
  int Import(std::string &Source, int Row);
};
