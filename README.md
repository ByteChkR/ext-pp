# ext-compiler
Small external text preprocessor that can introduce artificial static generics* different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.

## Status:
Master: [![Build Status](https://travis-ci.com/ByteChkR/ext-compiler.svg?branch=master)](https://travis-ci.com/ByteChkR/ext-compiler)  
Develop: [![Build Status](https://travis-ci.com/ByteChkR/ext-compiler.svg?branch=develop)](https://travis-ci.com/ByteChkR/ext-compiler)

## Usage:


ext_compiler.dll [-option VALUE]

### Options:
* -i/-input [InputFile]
* -o/-output [OutputFile]
* -tgen/-typeGenericKeyword [typeGenericKeyword]
* -inc/-includeStatement [includeStatement]
