/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * THIS FILE IS FROM GOSCPS(goscps@foxmail.com)
 * IS LICENSED UNDER GOSCPS
 * File:     Build.cpp
 * Content:  doing Build c++ file
 * Copyright (c) 2020 GOSCPS All rights reserved.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
#include "Build.hpp"
#include "main.hpp"

int Build::BuildStart() {
  std::cout << "Build For " << File << std::endl;

  std::ifstream IfStreeam(File);
  if (!IfStreeam.good()) {
    std::cout << "File Open Error" << std::endl;
    return -1;
  }

  //读取文件
  std::string Buf;
  std::string Source;
  while (std::getline(IfStreeam, Buf)) {
    Source.append(Buf);
    Source.append("\n");
  }

  //预处理
  if (Pretreatment(Source)) {
    std::cout << Color_Red << "Pretreatment Error" << Color_Clear << std::endl;
    return -1;
  }

  return Parser(Source);
}

inline std::string Trim(std::string Source) {
  if (Source.empty())
    return Source;
  {
    size_t a = 0;
    for (int b = 0; b < Source.size(); b++) {
      if (std::isspace(Source[b]))
        a++;
      else
        break;
    }
    Source = Source.substr(a);
  }
  {
    size_t a = 0;
    for (auto b = Source.rbegin(); b != Source.rend(); b++) {
      if (std::isspace(*b))
        a++;
      else
        break;
    }
    Source = Source.substr(0, Source.size() - a);
  }
  return Source;
}

inline std::vector<std::string> Split(std::string Source, std::string Regex) {
  std::regex re(Regex);
  std::vector<std::string> Line(
      std::sregex_token_iterator(Source.cbegin(), Source.cend(), re, -1),
      std::sregex_token_iterator());
  return Line;
}

//预处理
int Build::Pretreatment(std::string &Source) {
  std::cout << "Pretreatment..." << std::endl;

  auto Line = Split(Source, "\\n");
  int Row = 0;
  std::regex IsIf("(#Endif)|(#If\\S+)\\s+.+");
  std::regex IsImport("#Import\\s+\\S+");

  for (auto &s : Line) {
    Row++;

    s = Trim(s);
    if (s.empty())
      continue;
    if (s[0] == ';') {
      s = ""; //删除注释
      continue;
    }

    //是If，送入PretreatmentIF
    if (std::regex_match(s, IsIf)) {
      if (PretreatmentIF(s, Row))
        return -1;
      s = ""; //删除预处理语句
      continue;
    }

    //为忽略
    if (PretreatmentIgnore.size() != 0 && *PretreatmentIgnore.rbegin()) {
      s = "";
      continue;
    }

    // Import语句
    if (std::regex_match(s, IsImport)) {
      if (Import(s, Row))
        return -1;
      s = "";
      continue;
    }

    //非Define预处理器语句则替换
    if (PretreatmentDefine(s, Row)) {
      DefineReplace(s);
    } else
      s = ""; //删除预处理语句
  }
  std::string Out;
  for (auto s : Line) {
    Out.append(s);
    Out.append("\n");
  }

  std::cout << Out << std::endl;

  return 0;
}

//#If的处理
//处理正确为0
//错误为-1
int Build::PretreatmentIF(const std::string &Source, int Row) {
  //定义不忽略
  std::regex IfdefRegex("#Ifdef\\s+(\\S+)");
  //定义忽略
  std::regex IfndefRegex("#Ifndef\\s+(\\S+)");

  std::smatch Result;

  //优先处理#Endif
  if (Source == "#Endif") {
    if (PretreatmentIgnore.size() != 0)
      PretreatmentIgnore.pop_back();
    else {
      std::cout << Color_Red << "Error At line " << Row << ": Too Much #Endif"
                << Color_Clear << std::endl;
      return -1;
    }
    return 0;
  }

  //为忽略，不处理IF
  if (PretreatmentIgnore.size() != 0 && *PretreatmentIgnore.rbegin()) {
    PretreatmentIgnore.push_back(true);
    return 0;
  }

  if (std::regex_match(Source, Result, IfdefRegex)) {
    if (Defines.find(Result.str(1)) == Defines.cend())
      PretreatmentIgnore.push_back(true);
    else
      PretreatmentIgnore.push_back(false);

    return 0;
  } else if (std::regex_match(Source, Result, IfndefRegex)) {
    if (Defines.find(Result.str(1)) != Defines.cend())
      PretreatmentIgnore.push_back(true);
    else
      PretreatmentIgnore.push_back(false);

    return 0;
  }

  std::cout << Color_Red << "Error At line " << Row << ": Unknown If"
            << Color_Clear << std::endl;
  return -1;
}

//是Define或Undef
//否则为-1
int Build::PretreatmentDefine(const std::string &Source, int Row) {
  std::regex DefinRegex("#Define\\s+(\\S+)\\s+(\\S+)");
  std::regex UndefRegex("#Undef\\s+(\\S+)");

  std::smatch Result;
  // Define
  if (std::regex_match(Source, Result, DefinRegex)) {
    //重定义
    if (Defines.find(Result.str(1)) != Defines.cend()) {
      std::cout << Color_Yellow << "Warning At line " << Row
                << ":Pretreatment Defined:  " << Result.str(1) << Color_Clear
                << std::endl;
    }

    Defines.insert(std::make_pair(Result.str(1), Result.str(2)));
    std::cout << "Pretreatment Define: \'" << Result.str(1) << "\' -> \'"
              << Result.str(2) << "\'" << std::endl;
    return 0;
  }

  // Undef
  else if (std::regex_match(Source, Result, UndefRegex)) {
    std::cout << "Pretreatment Undef: " << Result.str(1) << std::endl;
    Defines.erase(Result.str(1));
    return 0;
  }

  return -1;
}

void Build::DefineReplace(std::string &Source) {
  bool Finded = true;
  while (Finded) {
    Finded = false;

    for (auto Def : Defines) {
      if (Source.find(Def.first) != Source.npos) {
        Finded = true;
        Source = Source.replace(Source.find(Def.first), Def.first.size(),
                                Def.second);
      } else
        continue;
    }
  }

  return;
}

//解析器
int Build::Parser(std::string &Source) {
  std::cout << "Build..." << std::endl;

  return 0;
}

//添加一些编译器变量
int Build::Import(std::string &Source, int Row) {
  std::regex ImportRegex("#Impoer\\s(\\S+)");

  std::smatch Result;

  if (std::regex_match(Source, Result, ImportRegex)) {
    if (Result.str(1) == "Clang") {
      AdvanceDefine("$c++$", "clang++");
      AdvanceDefine("$c$", "clang");
      AdvanceDefine("$linker$", "lld");
    } else if (Result.str(1) == "Gcc") {
      AdvanceDefine("$c++$", "g++");
      AdvanceDefine("$c$", "gcc");
      AdvanceDefine("$linker$", "lld");
    } else if (Result.str(1) == "msvc") {
      AdvanceDefine("$c++$", "msvc");
      AdvanceDefine("$c$", "msvc");
      AdvanceDefine("$linker$", "link");
    }

    return 0;
  }

  return -1;
}
