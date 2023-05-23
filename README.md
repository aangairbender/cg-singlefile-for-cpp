# cg-singlefile-for-cpp
![Build](https://github.com/aangairbender/cg-singlefile-for-cpp/actions/workflows/build.yml/badge.svg)

## Getting the tool

### Binary Download

Download the latest version from the [Releases](https://github.com/aangairbender/cg-singlefile-for-cpp/releases) page

### Building from source
```shell
git clone https://github.com/aangairbender/cg-singlefile-for-cpp.git
cd cg-singlefile-for-cpp
dotnet publish -c Release --use-current-runtime
```

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
* If the line ends with "singlefile-skip-line" it will be skipped, e.g.
	```cpp
	#include "some_local_tool.h" // singlefile-skip-line
	```


## Usage example

Let's create a folder for a project
```shell
mkdir cg-contest
cd cg-contest
```
Let's create a `my_math.h` file with a single `sum` function:
```cpp
int sum(int a, int b) {
    return a + b;
}
```
Let's create a `main.cpp` file and use `my_math.h` there:
```cpp
#include <iostream>
#include "my_math.h"

int main() {
    std::cout << sum(1, 2) << std::endl;
}
```
Now let's try build it and run:
```
g++ main.cpp -o out
./out
3
```
Works fine. Now let's bundle this toy project into a single file:

```shell
cg-singlefile-for-cpp "main.cpp" -o "bundled.cpp"
```
Let's check what the tool generated for us by checking content `bundled.cpp` file:
```c++
#include <iostream>
int sum(int a, int b) {
    return a + b;
}

int main() {
    std::cout << sum(1, 2) << std::endl;
}
```
So it just replaced `#include "my_math.h"` with the content of `my_math.h` file.
Let's make sure it still works:
```
g++ bundled.cpp -o out2
./out2
3
```

## `--help` output

```
Singlefile for cpp

Usage: cg-singlefile-for-cpp [arguments] [options]

Arguments:
  main file  The path to the entry point file (usually main.cpp)

Options:
  -o | --output <value>  output file path
  -h | --help            Show help information
  -v | --version         Show version information
```