# CMakeList.txt: doing 的 CMake 项目，在此处包括源代码并定义
# 项目特定的逻辑。
#
cmake_minimum_required (VERSION 3.8)

project ("doing"  VERSION 0.1.0 LANGUAGES CXX)

# Enable c++ 17
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED true)

# 版本号
set(_DOING_VERSION_MAJOR 0)
set(_DOING_VERSION_MINOR 1)
set(_DOING_VERSION_PATCH 0)


# 将源代码添加到此项目的可执行文件。
add_executable (doing 
"doing.cpp" "doing.hpp"
"tool/tool.hpp" "tool/tool.cpp"
"build.cpp" 
 "parser/parser.hpp")

# 配置头文件
configure_file(
${PROJECT_SOURCE_DIR}/config.hpp.in 
${PROJECT_BINARY_DIR}/config.hpp)

include_directories(${PROJECT_BINARY_DIR})

# TODO: 如有需要，请添加测试并安装目标。
