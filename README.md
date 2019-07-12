# ext_pp
Small external text preprocessor that can introduce artificial static generics* different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.

## Status:
Master: [![Build Status](https://travis-ci.com/ByteChkR/ext-compiler.svg?branch=master)](https://travis-ci.com/ByteChkR/ext-compiler)  
Develop: [![Build Status](https://travis-ci.com/ByteChkR/ext-compiler.svg?branch=develop)](https://travis-ci.com/ByteChkR/ext-compiler)

## Usage:


dotnet ext_pp.dll [-option VALUE]

### Parameter:
	-i|--input <path>  
	-o|--output <path>  

#### Optional Parameter:
	-rd|--resolveDefine [true|false]  
	-ru|--resolveUndefine [true|false]  
	-rc|--resolveConditions [true|false]  
	-ri|--resolveInclude [true|false]  
	-rg|--resolveGenerics [true|false]  
	-ee|--enableErrors [true|false]  
	-ew|--enableWarnings [true|false]  
	-def|--defines [DefineSymbols]  
	-v|--verbosity [0(Silent)-10(Maximum Debug Log)]