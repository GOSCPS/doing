/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * THIS FILE IS FROM GOSCPS(goscps@foxmail.com)
 * IS LICENSED UNDER GOSCPS
 * File:     main.cpp
 * Content:  doing main c++ file
 * Copyright (c) 2020 GOSCPS All rights reserved.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include "Build.hpp"
#include "main.hpp"

using namespace std;

int main(int argc, char *argv[]) {
  cout << "Build Platform:" << CompilePlatform << endl;

  Build *BuildSystem = new Build("build.do");

  std::regex TargetRegex("-Target=(\\S+)");
  std::regex DefineRegex("-D(\\S+)=(\\S+)");
  std::smatch Result;
  std::string RegexResultStr;

  int ArgsPtr = 1;
  if (argc == 0) {
    cout << "Not Command Input" << endl;
    exit(-1);
  }

  while (ArgsPtr < argc) {
    //非选项
    //为文件
    if (argv[ArgsPtr][0] != '\0' && argv[ArgsPtr][0] != '-') {
      if (BuildSystem != nullptr)
        BuildSystem->SetBuildFile(std::string(argv[ArgsPtr]));
      ArgsPtr++;
      continue;
    }

    //定义Target
    else if (regex_match(RegexResultStr = std::string(argv[ArgsPtr]), Result,
                         TargetRegex)) {
      if (BuildSystem != nullptr) {
        BuildSystem->SetStartTarget(Result.str(1));
        cout << "Target:" << Result.str(1) << endl;
      }
      ArgsPtr++;
      continue;
    }

    //预定义
    else if (regex_match(RegexResultStr = std::string(argv[ArgsPtr]), Result,
                         DefineRegex)) {
      if (BuildSystem != nullptr) {
        BuildSystem->AdvanceDefine(Result.str(1), Result.str(2));
        cout << "Define: \'" << Result.str(1) << "\' : \'" << Result.str(2)
             << "\'" << endl;
      }
      ArgsPtr++;
      continue;
    }

    else {
      cout << Color_Red << "Command Not Found" << Color_Clear << endl;
      exit(-1);
    }
  }

  BuildSystem->AdvanceDefine(CompilePlatform, "1");

  if (BuildSystem != nullptr) {
    //返回非0，错误
    if (BuildSystem->BuildStart()) {
      delete BuildSystem;
      return -1;
    } else {
      delete BuildSystem;
    }
  }

  return 0;
}