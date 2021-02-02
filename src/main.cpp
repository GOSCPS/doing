/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    main.cpp
 * @brief   mian.cpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include <string>
#include <vector>
#include <iostream>
#include <set>

#include "main.hpp"
#include "build.hpp"

using namespace std;

//Doing版本
extern const int DoingVersion = 0;

//全局变量表
std::map<std::string,std::string> GolbalVarTable;

int main(int argc,char* argv[]){

    cout << "Doing build system for everything. Version-" << DoingVersion << endl;

    string buildFile = "build.doing";
    set<string> buildTargets;

    //解析参数
    argc--;
    for(int a=0;a < argc;a++){
        string param = argv[a+1];
    
        if(param == "-name"){
            if(++a >= argc){
                cout << "Error:set name but not input file name" << endl;
                return -1;
            }
            else{
                buildFile = argv[a+1];
                continue;
            }
        }
        else if(param == "-D"){
            ++a;
            if(++a >= argc){
                cout << "Error:define but not key or value" << endl;
                return -1;
            }
            else{
                GolbalVarTable.insert(pair<string,string>(argv[a],argv[a+1]));
                continue;
            }
        }
        else{
            buildTargets.insert(argv[a+1]);
        }
    }

    cout << "Will build file:" << buildFile << endl;
    cout << "Will build targets:" << endl;
    for(auto a=buildTargets.cbegin();a != buildTargets.cend();a++){
        cout << "\t" << *a << endl;
    }
    if(buildTargets.size() == 0){
        cout << "\tNo DEF" << endl;
    }

    //初始化全局变量表

    //操作系统设置
    GolbalVarTable.insert(pair<string,string>(DOINGOS,DOINGOS));


    return Build(buildFile,buildTargets);
}