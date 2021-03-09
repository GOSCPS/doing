/* * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 这个文件来自 GOSCPS(https://github.com/GOSCPS)
 * 使用 GOSCPS 许可证
 * File:    doing.cpp
 * Content: doing source file
 * Copyright (c) 2020-2021 GOSCPS 保留所有权利.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


#include "doing.hpp"
#include "config.hpp"
#include "tool/tool.hpp"
#include "parser/parser.hpp"

#pragma comment(lib, "lua.lib")

using namespace std;
using namespace doing;

/**
 * @brief 全局变量
*/
std::map<std::string, std::string> GLOBAL_VARS;

/**
 * @brief 文件名称
*/
std::string FILE_NAME(Default_Build_File_Name);

/**
 * @brief 构建目标
*/
std::vector<std::string> AIMS_TARGET;

/**
 * @brief 构建选项
*/
std::vector<std::string> OPTIONS;

int main(int argc,char *argv[])
{
	cout << "*** Doing Build System ***" << endl;
	cout << "***   Made by GOSCPS   ***" << endl;
	cout << "***   Versuon " << _DOING_VERSION_MAJOR
		<< "." << _DOING_VERSION_MINOR
		<< "." << _DOING_VERSION_PATCH
		<< "    ***" << endl;

	int args_ptr = 0;

	while (args_ptr < (argc-1)){
		string arg(argv[args_ptr + 1]);

		// 定义全局变量
		if (arg.find("-D") == 0) {
			if (arg.find('=') == arg.npos) {
				printf("Error:Param `%s` usage error"_error.c_str(),arg.c_str());
				return EXIT_FAILURE;
			}

			else {
				string key_value = arg.substr(2);

				GLOBAL_VARS.insert(
					std::pair<string,string>(
					key_value.substr(0, key_value.find('=')),
					key_value.substr(key_value.find('=')+1)));
			}
		}

		// 指定文件名称
		else if (arg.find("-F") == 0) {
			FILE_NAME = arg.substr(2);
		}

		// 选项
		else if (arg.find("--") == 0) {
			OPTIONS.push_back(arg.substr(2));
		}

		// Help
		else if (arg == "help") {
			if (argc != 2) {
				cout << "Warn:Paramter `help` will skip build!"_yellow << endl;
			}
			cout << "Usage:doing [-DOINGOPTIONS] [--OPTIONS] [TARGETS]" << endl;
			cout << "DOINGOPTIONS:" << endl;
			cout << "\t-D[KEY]=[VALUE]\t\t\tDefine a key equal the value." << endl;
			cout << "\t-F[FILENAME]\t\t\tBuild the file for name.(DEFAULT:" 
				<< Default_Build_File_Name << ")" << endl;
			return EXIT_SUCCESS;
		}

		// 丢进Target
		else {
			AIMS_TARGET.push_back(arg);
		}

		args_ptr++;
	}

#if defined(DEBUG) | defined(_DEBUG)

	for (auto&aim : AIMS_TARGET) {
		cout << "Aim Target:" << aim << endl;
	}

	for (auto &aim : OPTIONS) {
		cout << "Options:" << aim << endl;
	}

	for (auto &aim : GLOBAL_VARS) {
		cout << "GLOBAL_VARS:" << aim.first << ":" << aim.second << endl;
	}

	cout << "Build Files:" << FILE_NAME << endl;

#endif // DEBUG | _DEBUG

	Token a;
	Token b;
	Token c;
	Token d;

	a.a = "I'm a";
	b.a = "I'm b";
	c.a = "I'm c";
	d.a = "I'm d";


	TokenStream stream(std::list<Token>({ a,b,c }));

	stream.next();

	stream.push_nows((std::list<Token>({ d,a,d })));

	stream.back();

	while (!stream.isEnd()) {
		cout << get<string>(stream.get().a) << endl;
		stream.next();
	}

	// 移交控制权
	return doing_build();
}
