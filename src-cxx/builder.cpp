/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    builder.cpp
 * @brief   builder.cpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include "Build.hpp"
#include "main.hpp"
#include <vector>
#include <set>
#include <map>
#include <string>
#include <iostream>
#include <sstream>

using namespace std;

constexpr const char* __UNKNOWN = "__ERROR__NOT__FOUND__";

string ReadNextVar(string::iterator& Begin,string::iterator& End){
    //${Name}
    if(Begin == End || (*Begin) != '$'){
        return "";
    }
    Begin++;
    if(Begin == End || (*Begin) != '{'){
        return "";
    }
    Begin++;

    string s;

    while (true)
    {
        if(Begin == End){
            return "";
        }

        if(*Begin == '}'){
            Begin++;
            return s;
        }
        else{
            s += *Begin;
            Begin++;
        }
    }

    //Name
    return "";
}

int Builder::BuildRule(Rule rule,std::map<std::string,std::string> with){
    stringstream command;
    stringstream depend;
    stringstream info;
    
    stringstream* writePtr = &command;

    {
    //解析命令
        string::iterator begin = rule.CommandLine.begin();
        string::iterator end = rule.CommandLine.end();

        while(begin != end){
        //空格
        if(isspace(*begin)){
            *writePtr << *begin;
        }
        //转义
        else if(*begin == '\\'){
            if(++begin != end){
                switch(*begin){
                    case '$':
                        *writePtr << '$';
                        break;
                    case '\\':
                        *writePtr << '\\';
                        break;
                    default:
                        cout << "Error:Unknown escape" << endl;
                        return -1;
                    }
                }
            else{
                    cout << "Error:Escape EOF" << endl;
                    return -1;
                }
            }
        //变量
        else if(*begin == '$'){
            string result = ReadNextVar(begin,end);
            //先内部再全局
            auto InVar = with.find(result);
            if(InVar == with.end()){
                auto GolVar = GolbalVarTable.find(result);
                if(GolVar == GolbalVarTable.end()){
                    cout << "Error:Var Not Found:" << result << endl;
                    return -1;
                }
                else{
                    *writePtr << GolVar->second;
                }
            }
            else{
                *writePtr << InVar->second;
            }


            continue;
        }
        //字符
        else {
            *writePtr << ReadNextWord(begin,end);
            continue;
        }
        begin++;
        }
    }

    writePtr = &depend;
    {
    //解析命令
        string::iterator begin = rule.Depend.begin();
        string::iterator end = rule.Depend.end();

        while(begin != end){
        //空格
        if(isspace(*begin)){
            *writePtr << *begin;
        }
        //转义
        else if(*begin == '\\'){
            if(++begin != end){
                switch(*begin){
                    case '$':
                        *writePtr << '$';
                        break;
                    case '\\':
                        *writePtr << '\\';
                        break;
                    default:
                        cout << "Error:Unknown escape" << endl;
                        return -1;
                    }
                }
            else{
                    cout << "Error:Escape EOF" << endl;
                    return -1;
                }
            }
        //变量
        else if(*begin == '$'){
            string result = ReadNextVar(begin,end);
            //先内部再全局
            auto InVar = with.find(result);
            if(InVar == with.end()){
                auto GolVar = GolbalVarTable.find(result);
                if(GolVar == GolbalVarTable.end()){
                    cout << "Error:Var Not Found:" << result << endl;
                    return -1;
                }
                else{
                    *writePtr << GolVar->second;
                }
            }
            else{
                *writePtr << InVar->second;
            }

            continue;
        }
        //字符
        else {
            *writePtr << ReadNextWord(begin,end);
            continue;
        }
        begin++;
        }
    }

    writePtr = &info;
    {
    //解析命令
        string::iterator begin = rule.Introduction.begin();
        string::iterator end = rule.Introduction.end();

        while(begin != end){
        //空格
        if(isspace(*begin)){
            *writePtr << *begin;
        }
        //转义
        else if(*begin == '\\'){
            if(++begin != end){
                switch(*begin){
                    case '$':
                        *writePtr << '$';
                        break;
                    case '\\':
                        *writePtr << '\\';
                        break;
                    default:
                        cout << "Error:Unknown escape" << endl;
                        return -1;
                    }
                }
            else{
                    cout << "Error:Escape EOF" << endl;
                    return -1;
                }
            }
        //变量
        else if(*begin == '$'){
            string result = ReadNextVar(begin,end);

            //先内部再全局
            auto InVar = with.find(result);
            if(InVar == with.end()){
                auto GolVar = GolbalVarTable.find(result);
                if(GolVar == GolbalVarTable.end()){
                    cout << "Error:Var Not Found:" << result << endl;
                    return -1;
                }
                else{
                    *writePtr << GolVar->second;
                }
            }
            else{
                *writePtr << InVar->second;
            }

            continue;
        }
        //字符
        else {
            *writePtr << ReadNextWord(begin,end);
            continue;
        }
        begin++;
        }
    }

    #ifdef DEBUG
    cout << "Run:" << command.str() << endl;
    #endif
    cout << info.str() << endl;
    int result = system(command.str().c_str());
    if(result == 127){
        cout << "Error:May Command Found Not:" << depend.str() << endl; 
        return result;
    }
    else{
        return result;
    }
}

int Builder::BuildRun(Run run){
    Rule t;
    t.Name = __UNKNOWN;
    for(auto a=RuleList.cbegin();a != RuleList.cend();a++){
        if(a->Name == run.RunRule){
            t = *a;
            break;
        }
    }

    if(t.Name == __UNKNOWN){
        cout << "Error:Unknown Rule:" << run.RunRule << endl;
        return -1;
    }

    else return BuildRule(t,run.RunWith);
}

int Builder::BuildTarget(string target){
    Target t;
    {
        t.Name = __UNKNOWN;

        for(auto a=TargetList.cbegin();a != TargetList.cend();a++){
            if(a->Name == target){
                t = *a;
                break;
            }
        }

        if(t.Name == __UNKNOWN){
            cout << "Error:Unknown target name:" << target << endl;
            return -1;
        }
    }

    //执行依赖
    for(auto a= t.TargetDepend.cbegin();a != t.TargetDepend.cend();a++){
        int result = BuildTarget(*a);
        if(result != 0){
            return result;
        }
    }

    //执行Run
    for(auto a= t.runList.cbegin();a != t.runList.cend();a++){
        int result = BuildRun(*a);
        if(result != 0){
            cout << "Error:Build " << target << 
            " run " << a->RunRule << 
            " return not 0:" << result << endl;
            return result;
        }
    }

    return 0;
}
