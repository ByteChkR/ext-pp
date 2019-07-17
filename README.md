# ext_pp
Small plugin based external text preprocessor that can introduce artificial static generics, different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.


You can find the documentation [HERE](https://bytechkr.github.io/ext-pp/)

## Status:
Master: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=master)](https://travis-ci.com/ByteChkR/ext-pp)  
Develop: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=develop)](https://travis-ci.com/ByteChkR/ext-pp)

## Usage:

### ext_pp
The core library of the Solution.
It contains all the nessecary components to run the text processor

### ext_pp_base
A Library containing interfaces to be able to develop plugins without recompiling the project

### ext_pp_cli
A console application that wraps around the core library and offers the full functionality from the command line.
To set a value for a plugin the syntax is:

#includeinl readme/plugin_console_commands.md