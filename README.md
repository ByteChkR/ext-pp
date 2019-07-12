# ext_pp
Small external text preprocessor that can introduce artificial static generics* different preprocessor keywords like #define #include #if and more to any kind of text file. Its main purpose is to keep me from copying my errors around when writing different opencl kernels.

## Status:
Master: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=master)](https://travis-ci.com/ByteChkR/ext-pp)  
Develop: [![Build Status](https://travis-ci.com/ByteChkR/ext-pp.svg?branch=develop)](https://travis-ci.com/ByteChkR/ext-pp)

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
	-ss|--setSeparator [char]
	-2c|--writeToConsole
	-kw:d|--keyWord:d [defineStatement]
	-kw:u|--keyWord:u [unDefineStatement]
	-kw:if|--keyWord:if [ifStatement]
	-kw:elif|--keyWord:elif [elseIfStatement]
	-kw:else|--keyWord:else [elseStatement]
	-kw:eif|--keyWord:eif [endIfStatement]
	-kw:w|--keyWord:w [warningStatement]
	-kw:e|--keyWord:e [errorStatement]
	-kw:i|--keyWord:i [includeStatement]
	-kw:t|--keyWord:t [typeGenStatement]

## Supported Statements:
	#include <path/to/file> <#type0> <#type1> ... <#typeN>
	#define <VAR> <VAR2> ...
	#undefine <VAR> <VAR2> ...
	#if <VAR> <VAR2> ... (only AND supported at this moment)
	#elseif <VAR> <VAR2> ... (only AND supported at this moment)
	#else
	(#endif)
	#error <Error Description>
	#warning <Warning Description>
	#typeN (The value to use instead of type when writing generic code)
		can also rename function names dynamically