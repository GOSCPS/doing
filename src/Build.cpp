/** * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * @author GOSCPS
 * @license GOSCPS 许可证
 * @file    build.cpp
 * @brief   build.cpp \n
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

#include "main.hpp"
#include "target.hpp"
#include "rule.hpp"
#include "build.hpp"
#include "run.hpp"

#include <ctype.h>
#include <iterator>
#include <string>
#include <set>
#include <map>
#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include <regex>
#include <cerrno>
#include <cstring>
#include <optional>

using namespace std;

//跳过空格
void JumpSpace(string::iterator& begin,string::iterator& end){
    while(begin != end){
        if(isspace(*begin)){
            begin++;
        }
        else break;
    }
    return;
}

//读取下一个单词
string ReadNextWord(string::iterator &begin,string::iterator &end){
string a;
while(begin != end){
        if(!isspace(*begin)){
            a += *begin;
            begin++;
        }
        else break;
    }
return a;
}

//Trim函数
string Trim(string s) 
{
    if (s.empty()) 
    {
        return s;
    }
    s.erase(0,s.find_first_not_of(" "));
    s.erase(s.find_last_not_of(" ") + 1);

    return s;
}


//读取定义
//Key = Value
pair<string,string> Parse_Define(string::iterator &begin,string::iterator &end){
    pair<string,string> ret("","");

    JumpSpace(begin,end);
    string key = ReadNextWord(begin,end);
    JumpSpace(begin,end);
    string equal = ReadNextWord(begin,end);
    JumpSpace(begin,end);

    //将等号后的所有字符串Trim后视为Value
    string value;
    while(begin != end){
        value += *begin;
        begin++;
    }
    value = Trim(value);

    if(key.empty()){
        cout << "Error:for key error" << endl;
        return ret;
    }
    if(equal.empty() || equal != "="){
        cout << "Error:miss token`=`" << endl;
        return ret;
    }
    if(key.empty()){
        cout << "Error:for value error" << endl;
        return ret;
    }

    ret.first = key;
    ret.second = value;

    return ret;
}

int Parse_For(vector<string>::iterator& Begin,vector<string>::iterator& End){
    /*
    {
        auto a = Begin->begin();
        auto b = Begin->end();
        string IsFor = ReadNextWord(a,b);

        if(IsFor != "for"){
            cout << "Error:Doing Error:Much For" << endl;
            return -1;
        }
    }*/
    string First = *Begin;

    auto FirstB = First.begin();
    auto FirstE = First.end();

    //跳过for
    ReadNextWord(FirstB,FirstE);
    JumpSpace(FirstB,FirstE);

    string If =  ReadNextWord(FirstB,FirstE);
    if(If.empty()){
        cout << "Error:for but not keyword"  <<  *Begin << endl;
        return -1;
    }

    //判断是否在全局变量当中
    bool NeedDefine = false;
    for(auto a = GolbalVarTable.cbegin();a != GolbalVarTable.cend();a++){
        if(a->first == If){
            NeedDefine = true;
            break;
        }
    }
    Begin++;

    //解析变量
    while(true){
        if(Begin == End){
            cout << "Error:for but get EOF" << endl;
            return -1;
        }
        if(Begin->empty()){
            Begin++;
            continue;
        }
        string def = *Begin;
        auto bgn = def.begin();
        auto end = def.end();

        if(def == "end for"){
            Begin++;
            break;
        }

        auto define = Parse_Define(bgn,end);

        if(define.first.empty() || define.second.empty()){
            cout << "Error:Parse Define Error:" << def << endl;
            return -1;
        }
        else if(NeedDefine){
            GolbalVarTable.insert(define);
        }
        Begin++;
    }

    return 0;
}

optional<Rule> Parse_Rule(vector<string>::iterator& Begin,vector<string>::iterator& End){
    Rule rule;

    //获取规则名称
    string First = *Begin;

    auto FirstB = First.begin();
    auto FirstE = First.end();

    //跳过rule
    ReadNextWord(FirstB,FirstE);
    JumpSpace(FirstB,FirstE);

    string Name =  ReadNextWord(FirstB,FirstE);
    if(Name.empty()){
        cout << "Error:rule but not name" <<  *Begin << endl;
        return nullopt;
    }
    else{
        rule.Name = Name;
    }

    Begin++;
    //解析内容
    while(true){
        if(Begin == End){
            cout << "Error:rule but get EOF" << endl;
            return nullopt;
        }
        if(Begin->empty()){
            Begin++;
            continue;
        }
        string def = *Begin;
        auto bgn = def.begin();
        auto end = def.end();

        if(def == "end rule"){
            Begin++;
            break;
        }

        auto define = Parse_Define(bgn,end);

        if(define.first.empty() || define.second.empty()){
            cout << "Error:Parse Define Error:" << def << endl;
            return nullopt;
        }
        //define分类 
        if(define.first == "commandline"){
            rule.CommandLine = define.second;
        }
        else if(define.first == "introduction"){
            rule.Introduction = define.second;
        }
        else if(define.first == "depend"){
            rule.Depend = define.second;
        }
        else{
            cout << "Error:Unknown Rule:" << *Begin << endl;
            return nullopt;
        }

        Begin++;
    }

    return rule;
}


optional<Run> Parse_Run(vector<string>::iterator& Begin,vector<string>::iterator& End){
    Run run;

    //获取规则名称
    string First = *Begin;

    auto FirstB = First.begin();
    auto FirstE = First.end();

    //跳过run
    ReadNextWord(FirstB,FirstE);
    JumpSpace(FirstB,FirstE);

    string Name =  ReadNextWord(FirstB,FirstE);
    JumpSpace(FirstB,FirstE);
    //读取RuleName
    if(Name.empty()){
        cout << "Error:run but not rule" <<  *Begin << endl;
        return nullopt;
    }
    else{
        run.RunRule = Name;
    }

    //没有with
    //则直接返回
    JumpSpace(FirstB,FirstE);
    if(FirstB != FirstE){
        string keyWith = ReadNextWord(FirstB,FirstE);

        if(keyWith != "with"){
            cout << "Error:Unknown Run options" << *Begin << endl;
            return nullopt;
        }
    }
    else{
        return run;
    }


    Begin++;
    //解析内容
    while(true){
        if(Begin == End){
            cout << "Error:rule but get EOF" << endl;
            return nullopt;
        }
        if(Begin->empty()){
            Begin++;
            continue;
        }
        string def = *Begin;
        auto bgn = def.begin();
        auto end = def.end();

        if(def == "end run"){
            Begin++;
            break;
        }

        auto define = Parse_Define(bgn,end);

        if(define.first.empty() || define.second.empty()){
            cout << "Error:Parse Define Error:" << def << endl;
            return nullopt;
        }
        else{
            run.RunWith.insert(define);
        }


        Begin++;
    }

    return run;
}

optional<Target> Parse_Target(vector<string>::iterator& Begin,vector<string>::iterator& End){
    Target target;

    //获取规则名称
    string First = *Begin;

    auto FirstB = First.begin();
    auto FirstE = First.end();

    //跳过target
    ReadNextWord(FirstB,FirstE);
    JumpSpace(FirstB,FirstE);

    string Name =  ReadNextWord(FirstB,FirstE);
    JumpSpace(FirstB,FirstE);
    if(Name.empty()){
        cout << "Error:rule but not name" << endl;
        return nullopt;
    }
    else{
        target.Name = Name;
    }

    //解析need
    if(FirstB != FirstE){
        string NeedKey = ReadNextWord(FirstB,FirstE);
        if(NeedKey != "need"){
            cout << "Error:Target Parse Error:" << *Begin << endl;
            return nullopt;
        }

        while(FirstB != FirstE){
            JumpSpace(FirstB,FirstE);
            string Needs = ReadNextWord(FirstB,FirstE);

            if(Needs.empty())
                break;
            target.TargetDepend.push_back(Needs);
        }
    }

    Begin++;
    //解析内容
    while(true){
        if(Begin == End){
            cout << "Error:target but get EOF" << endl;
            return nullopt;
        }
        if(Begin->empty()){
            Begin++;
            continue;
        }
        string def = *Begin;
        auto bgn = def.begin();
        auto end = def.end();

        if(def == "end target"){
            Begin++;
            break;
        }

        auto define = ReadNextWord(bgn,end);
            
        if(define == "run"){
            auto runs = Parse_Run(Begin,End);
            if(!runs.has_value()){
                cout << "Error:target Parse Error:" << *Begin << endl;
                return nullopt;
            }
            else{
                target.runList.push_back(runs.value());
            }
        }
        else{
            cout << "Error:Unknown Rule:" << *Begin << endl;
            return nullopt;
        }

        Begin++;
    }

    return target;
}

//解析总线
int Parse(vector<string> in,set<string> RunTargets){
    auto NowLine = in.begin();
    auto EndLine = in.end();

    vector<Rule> rules;
    vector<Target> targets;

    while(true){
        if(NowLine == EndLine){
            break;
        }

        auto Begin = (*NowLine).begin();
        auto End = (*NowLine).end();

        //跳过空行和注释
        if(Begin == End){
            NowLine++;
            continue;
        }
        else if(*Begin == '#'){
            NowLine++;
            continue;
        }

        string keyword = ReadNextWord(Begin,End);

        //解析关键字
        if(keyword == "for"){
            auto result = Parse_For(NowLine,EndLine);
            if(result != 0){
                return result;
            }
            continue;
        }
        else if(keyword == "rule"){
            auto result = Parse_Rule(NowLine,EndLine);
            if(!result.has_value()){
                return -1;
            }
            rules.push_back(result.value());
        }
            else if(keyword == "target"){
            auto result = Parse_Target(NowLine,EndLine);
            if(!result.has_value()){
                return -1;
            }
            targets.push_back(result.value());
            }
        else{
            cout << "Error Unknow Word:" << keyword << endl;
            return -1; 
        }

    }

    #ifdef DEBUG
    cout << "ParseInfo:" << endl << endl;

    cout << "GolbalVarTable:" << endl;
    for(auto a = GolbalVarTable.cbegin();a != GolbalVarTable.cend();a++){
        cout << "\t" << a->first << ":" << a->second << endl;
    }

     cout << "Targets:" << endl;
    for(auto a=targets.cbegin();a != targets.cend();a++){
        cout << "\tTarget:" << a->Name << endl;
        
        cout << "\tDepends:" << endl;
        for(auto b=a->TargetDepend.cbegin();b != a->TargetDepend.end();b++){
            cout << "\t\t" << *b << endl;
        }

        cout << "\tRuns:" << endl;
        for(auto b=a->runList.cbegin();b != a->runList.end();b++){
            cout << "\t\t" << b->RunRule << endl;

            cout << "\t\tRunWith:" << endl;
            for(auto c = b->RunWith.cbegin();c != b->RunWith.cend();c++){
                cout << "\t\t\t" << c->first << ":" << c->second << endl;
            }
        }
    }

     cout << "Rules:" << endl;
    for(auto a=rules.cbegin();a != rules.cend();a++){
        cout << "\tRule:" << a->Name << endl;
        cout << "\t\tCommandLine:" << a->CommandLine << endl;
        cout << "\t\tDepend:" << a->Depend << endl;
        cout << "\t\tIntroduction:" << a->Introduction << endl;
    }

    cout << "END:" << endl << endl;
    #endif

    //构建
    Builder builder;
    builder.RuleList = rules;
    builder.TargetList = targets;
    
    if(RunTargets.size() == 0){
        cout << "Warn:Unknown Targets.Default `default`" << endl;
        return builder.BuildTarget("default");
    }
    else{
        for(auto a=RunTargets.begin();a != RunTargets.end();a++){
            int result = builder.BuildTarget(*a);
            if(result != 0){
                return result;
            }
        }
    }

return 0;
}


//构建初始化
int Build(std::string fileName,std::set<std::string> tagrets){

    stringstream DoingStdIO;

    fstream ifs;
    ifs.open(fileName,ios::in);

    if(!(ifs.good() && ifs.is_open())){
        cout << "File Open Error Code:" << errno << " Because:" << strerror(errno) << endl;
        return -1;
    }

    char reader;

    while(ifs.read(&reader,1))
        DoingStdIO << reader;

    ifs.close();

    string Doing = DoingStdIO.str();

    //开始处理
    regex lines("(\\r)|(\\n)|(\\r\\n)");
    vector<string> DoingLines(sregex_token_iterator(Doing.begin(),Doing.end(),lines,-1),
    sregex_token_iterator());
    
    //Trim
    for(auto a=DoingLines.begin();a != DoingLines.cend();a++){
        *a = Trim(*a);
    }

    return Parse(DoingLines,tagrets);
}