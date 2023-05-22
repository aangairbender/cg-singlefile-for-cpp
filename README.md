# cg-singlefile-for-cpp
![Build](https://github.com/aangairbender/cg-singlefile-for-cpp/actions/workflows/build.yml/badge.svg)

## Features

* Any include of the following format (quotation marks `""`)
	```cpp
	#include "some_file.h"
	```
	will be replaced with content of `some_file.h`
* Any include of the following format (angle brackets `<>`)
	```cpp
	#include <iostream>
	```
	will be moved to the top of the output file
* Same include will not be processed more than once (no duplication)
* Automatically skips lines containing `#pragma once`
* Automatically skips lines starting with `//` (single-line comments).
* If the line ends with "singlefile-skip-line" if will be skipped, e.g.
	```cpp
	#include "some_local_tool.h" // singlefile-skip-line
	```

## Building
```shell
git clone https://github.com/aangairbender/cg-singlefile-for-cpp.git
cd cg-singlefile-for-cpp
dotnet publish -c Release
```

## Usage example

```shell
cg-singlefile-for-cpp "D:/Projects/fall-challenge-2022/main.cpp" -o "output.txt"
```

## Help output

```
Singlefile for cpp

Usage: cg-singlefile-for-cpp [arguments] [options]

Arguments:
  main file  The path to the entry point file (usually main.cpp)

Options:
  -o | --output <value>  output file path
  -h | --help            Show help information
```