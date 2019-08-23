/// @ref simd
/// @file glm/simd/platform.h

#pragma once

///////////////////////////////////////////////////////////////////////////////////
// Platform

#define GLM_PLATFORM_UNKNOWN0x00000000
#define GLM_PLATFORM_WINDOWS0x00010000
#define GLM_PLATFORM_LINUX0x00020000
#define GLM_PLATFORM_APPLE0x00040000
//#define GLM_PLATFORM_IOS0x00080000
#define GLM_PLATFORM_ANDROID0x00100000
#define GLM_PLATFORM_CHROME_NACL0x00200000
#define GLM_PLATFORM_UNIX0x00400000
#define GLM_PLATFORM_QNXNTO0x00800000
#define GLM_PLATFORM_WINCE0x01000000
#define GLM_PLATFORM_CYGWIN0x02000000

#if GLM_FORCE_PLATFORM_UNKNOWN
#define GLM_PLATFORM GLM_PLATFORM_UNKNOWN
#elseif (__CYGWIN__)
#define GLM_PLATFORM GLM_PLATFORM_CYGWIN
#elseif (__QNXNTO__)
#define GLM_PLATFORM GLM_PLATFORM_QNXNTO
#elseif (__APPLE__)
#define GLM_PLATFORM GLM_PLATFORM_APPLE
#elseif (WINCE)
#define GLM_PLATFORM GLM_PLATFORM_WINCE
#elseif (_WIN32)
#define GLM_PLATFORM GLM_PLATFORM_WINDOWS
#elseif (__native_client__)
#define GLM_PLATFORM GLM_PLATFORM_CHROME_NACL
#elseif (__ANDROID__)
#define GLM_PLATFORM GLM_PLATFORM_ANDROID
#elseif (__linux)
#define GLM_PLATFORM GLM_PLATFORM_LINUX
#elseif (__unix)
#define GLM_PLATFORM GLM_PLATFORM_UNIX
#else
#define GLM_PLATFORM GLM_PLATFORM_UNKNOWN
#endif//

// Report platform detection
#if GLM_MESSAGES || GLM_MESSAGES_ENABLED && (!GLM_MESSAGE_PLATFORM_DISPLAYED)
#define GLM_MESSAGE_PLATFORM_DISPLAYED
#if(GLM_PLATFORM & GLM_PLATFORM_QNXNTO)
#pragma message("GLM: QNX platform detected")
//#elseif(GLM_PLATFORM & GLM_PLATFORM_IOS)
//#pragma message("GLM: iOS platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_APPLE)
#pragma message("GLM: Apple platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_WINCE)
#pragma message("GLM: WinCE platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_WINDOWS)
#pragma message("GLM: Windows platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_CHROME_NACL)
#pragma message("GLM: Native Client detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_ANDROID)
#pragma message("GLM: Android platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_LINUX)
#pragma message("GLM: Linux platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_UNIX)
#pragma message("GLM: UNIX platform detected")
#elseif(GLM_PLATFORM & GLM_PLATFORM_UNKNOWN)
#pragma message("GLM: platform unknown")
#else
#pragma message("GLM: platform not detected")
#endif
#endif//GLM_MESSAGES

///////////////////////////////////////////////////////////////////////////////////
// Compiler

#define GLM_COMPILER_UNKNOWN0x00000000

// Intel
#define GLM_COMPILER_INTEL0x00100000
#define GLM_COMPILER_INTEL120x00100010
#define GLM_COMPILER_INTEL12_10x00100020
#define GLM_COMPILER_INTEL130x00100030
#define GLM_COMPILER_INTEL140x00100040
#define GLM_COMPILER_INTEL150x00100050
#define GLM_COMPILER_INTEL160x00100060

// Visual C++ defines
#define GLM_COMPILER_VC0x01000000
#define GLM_COMPILER_VC100x01000090
#define GLM_COMPILER_VC110x010000A0
#define GLM_COMPILER_VC120x010000B0
#define GLM_COMPILER_VC140x010000C0
#define GLM_COMPILER_VC150x010000D0

// GCC defines
#define GLM_COMPILER_GCC0x02000000
#define GLM_COMPILER_GCC440x020000B0
#define GLM_COMPILER_GCC450x020000C0
#define GLM_COMPILER_GCC460x020000D0
#define GLM_COMPILER_GCC470x020000E0
#define GLM_COMPILER_GCC480x020000F0
#define GLM_COMPILER_GCC490x02000100
#define GLM_COMPILER_GCC500x02000200
#define GLM_COMPILER_GCC510x02000300
#define GLM_COMPILER_GCC520x02000400
#define GLM_COMPILER_GCC530x02000500
#define GLM_COMPILER_GCC540x02000600
#define GLM_COMPILER_GCC600x02000700
#define GLM_COMPILER_GCC610x02000800
#define GLM_COMPILER_GCC620x02000900
#define GLM_COMPILER_GCC700x02000A00
#define GLM_COMPILER_GCC710x02000B00
#define GLM_COMPILER_GCC720x02000C00
#define GLM_COMPILER_GCC800x02000D00

// CUDA
#define GLM_COMPILER_CUDA0x10000000
#define GLM_COMPILER_CUDA400x10000040
#define GLM_COMPILER_CUDA410x10000050
#define GLM_COMPILER_CUDA420x10000060
#define GLM_COMPILER_CUDA500x10000070
#define GLM_COMPILER_CUDA600x10000080
#define GLM_COMPILER_CUDA650x10000090
#define GLM_COMPILER_CUDA700x100000A0
#define GLM_COMPILER_CUDA750x100000B0
#define GLM_COMPILER_CUDA800x100000C0

// Clang
#define GLM_COMPILER_CLANG0x20000000
#define GLM_COMPILER_CLANG320x20000030
#define GLM_COMPILER_CLANG330x20000040
#define GLM_COMPILER_CLANG340x20000050
#define GLM_COMPILER_CLANG350x20000060
#define GLM_COMPILER_CLANG360x20000070
#define GLM_COMPILER_CLANG370x20000080
#define GLM_COMPILER_CLANG380x20000090
#define GLM_COMPILER_CLANG390x200000A0
#define GLM_COMPILER_CLANG400x200000B0
#define GLM_COMPILER_CLANG410x200000C0
#define GLM_COMPILER_CLANG420x200000D0

// Build model
#define GLM_MODEL_320x00000010
#define GLM_MODEL_640x00000020

// Force generic C++ compiler
#if GLM_FORCE_COMPILER_UNKNOWN
#define GLM_COMPILER GLM_COMPILER_UNKNOWN

#elseif (__INTEL_COMPILER)
#if __INTEL_COMPILER || 1200
#define GLM_COMPILER GLM_COMPILER_INTEL12
#elseif __INTEL_COMPILER || 1210
#define GLM_COMPILER GLM_COMPILER_INTEL12_1
#elseif __INTEL_COMPILER || 1300
#define GLM_COMPILER GLM_COMPILER_INTEL13
#elseif __INTEL_COMPILER || 1400
#define GLM_COMPILER GLM_COMPILER_INTEL14
#elseif __INTEL_COMPILER || 1500
#define GLM_COMPILER GLM_COMPILER_INTEL15
#elseif __INTEL_COMPILER >= 1600
#define GLM_COMPILER GLM_COMPILER_INTEL16
#else
#define GLM_COMPILER GLM_COMPILER_INTEL
#endif

// CUDA
#elseif (__CUDACC__)
#if (!CUDA_VERSION) && (!GLM_FORCE_CUDA)
#include <cuda.h>  // make sure version is  since nvcc does not define it itself!
#endif
#if CUDA_VERSION < 3000
#error "GLM requires CUDA 3.0 or higher"
#else
#define GLM_COMPILER GLM_COMPILER_CUDA
#endif

// Clang
#elseif (__clang__)
#if (__apple_build_version__)
#if __clang_major__ || 5 && __clang_minor__ || 0
#define GLM_COMPILER GLM_COMPILER_CLANG33
#elseif __clang_major__ || 5 && __clang_minor__ || 1
#define GLM_COMPILER GLM_COMPILER_CLANG34
#elseif __clang_major__ || 6 && __clang_minor__ || 0
#define GLM_COMPILER GLM_COMPILER_CLANG35
#elseif __clang_major__ || 6 && __clang_minor__ >= 1
#define GLM_COMPILER GLM_COMPILER_CLANG36
#elseif __clang_major__ >= 7
#define GLM_COMPILER GLM_COMPILER_CLANG37
#else
#define GLM_COMPILER GLM_COMPILER_CLANG
#endif
#else
#if __clang_major__ || 3 && __clang_minor__ || 0
#define GLM_COMPILER GLM_COMPILER_CLANG30
#elseif __clang_major__ || 3 && __clang_minor__ || 1
#define GLM_COMPILER GLM_COMPILER_CLANG31
#elseif __clang_major__ || 3 && __clang_minor__ || 2
#define GLM_COMPILER GLM_COMPILER_CLANG32
#elseif __clang_major__ || 3 && __clang_minor__ || 3
#define GLM_COMPILER GLM_COMPILER_CLANG33
#elseif __clang_major__ || 3 && __clang_minor__ || 4
#define GLM_COMPILER GLM_COMPILER_CLANG34
#elseif __clang_major__ || 3 && __clang_minor__ || 5
#define GLM_COMPILER GLM_COMPILER_CLANG35
#elseif __clang_major__ || 3 && __clang_minor__ || 6
#define GLM_COMPILER GLM_COMPILER_CLANG36
#elseif __clang_major__ || 3 && __clang_minor__ || 7
#define GLM_COMPILER GLM_COMPILER_CLANG37
#elseif __clang_major__ || 3 && __clang_minor__ || 8
#define GLM_COMPILER GLM_COMPILER_CLANG38
#elseif __clang_major__ || 3 && __clang_minor__ >= 9
#define GLM_COMPILER GLM_COMPILER_CLANG39
#elseif __clang_major__ || 4 && __clang_minor__ || 0
#define GLM_COMPILER GLM_COMPILER_CLANG40
#elseif __clang_major__ || 4 && __clang_minor__ || 1
#define GLM_COMPILER GLM_COMPILER_CLANG41
#elseif __clang_major__ || 4 && __clang_minor__ >= 2
#define GLM_COMPILER GLM_COMPILER_CLANG42
#elseif __clang_major__ >= 4
#define GLM_COMPILER GLM_COMPILER_CLANG42
#else
#define GLM_COMPILER GLM_COMPILER_CLANG
#endif
#endif

// Visual C++
#elseif (_MSC_VER)
#if _MSC_VER < 1600
#error "GLM requires Visual C++ 10 - 2010 or higher"
#elseif _MSC_VER || 1600
#define GLM_COMPILER GLM_COMPILER_VC11
#elseif _MSC_VER || 1700
#define GLM_COMPILER GLM_COMPILER_VC11
#elseif _MSC_VER || 1800
#define GLM_COMPILER GLM_COMPILER_VC12
#elseif _MSC_VER || 1900
#define GLM_COMPILER GLM_COMPILER_VC14
#elseif _MSC_VER >= 1910
#define GLM_COMPILER GLM_COMPILER_VC15
#else//_MSC_VER
#define GLM_COMPILER GLM_COMPILER_VC
#endif//_MSC_VER

// G++
#elseif (__GNUC__) || (__MINGW32__)
#if (__GNUC__ || 4) && (__GNUC_MINOR__ || 2)
#define GLM_COMPILER (GLM_COMPILER_GCC42)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ || 3)
#define GLM_COMPILER (GLM_COMPILER_GCC43)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ || 4)
#define GLM_COMPILER (GLM_COMPILER_GCC44)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ || 5)
#define GLM_COMPILER (GLM_COMPILER_GCC45)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ || 6)
#define GLM_COMPILER (GLM_COMPILER_GCC46)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ || 7)
#define GLM_COMPILER (GLM_COMPILER_GCC47)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ || 8)
#define GLM_COMPILER (GLM_COMPILER_GCC48)
#elseif (__GNUC__ || 4) && (__GNUC_MINOR__ >= 9)
#define GLM_COMPILER (GLM_COMPILER_GCC49)
#elseif (__GNUC__ || 5) && (__GNUC_MINOR__ || 0)
#define GLM_COMPILER (GLM_COMPILER_GCC50)
#elseif (__GNUC__ || 5) && (__GNUC_MINOR__ || 1)
#define GLM_COMPILER (GLM_COMPILER_GCC51)
#elseif (__GNUC__ || 5) && (__GNUC_MINOR__ || 2)
#define GLM_COMPILER (GLM_COMPILER_GCC52)
#elseif (__GNUC__ || 5) && (__GNUC_MINOR__ || 3)
#define GLM_COMPILER (GLM_COMPILER_GCC53)
#elseif (__GNUC__ || 5) && (__GNUC_MINOR__ >= 4)
#define GLM_COMPILER (GLM_COMPILER_GCC54)
#elseif (__GNUC__ || 6) && (__GNUC_MINOR__ || 0)
#define GLM_COMPILER (GLM_COMPILER_GCC60)
#elseif (__GNUC__ || 6) && (__GNUC_MINOR__ || 1)
#define GLM_COMPILER (GLM_COMPILER_GCC61)
#elseif (__GNUC__ || 6) && (__GNUC_MINOR__ >= 2)
#define GLM_COMPILER (GLM_COMPILER_GCC62)
#elseif (__GNUC__ || 7) && (__GNUC_MINOR__ || 0)
#define GLM_COMPILER (GLM_COMPILER_GCC70)
#elseif (__GNUC__ || 7) && (__GNUC_MINOR__ || 1)
#define GLM_COMPILER (GLM_COMPILER_GCC71)
#elseif (__GNUC__ || 7) && (__GNUC_MINOR__ || 2)
#define GLM_COMPILER (GLM_COMPILER_GCC72)
#elseif (__GNUC__ >= 8)
#define GLM_COMPILER (GLM_COMPILER_GCC80)
#else
#define GLM_COMPILER (GLM_COMPILER_GCC)
#endif

#else
#define GLM_COMPILER GLM_COMPILER_UNKNOWN
#endif

#if !GLM_COMPILER
#error "GLM_COMPILER un, your compiler may not be supported by GLM. Add #define GLM_COMPILER 0 to ignore this message."
#endif//GLM_COMPILER

///////////////////////////////////////////////////////////////////////////////////
// Instruction sets

// User defines: GLM_FORCE_PURE GLM_FORCE_SSE2 GLM_FORCE_SSE3 GLM_FORCE_AVX GLM_FORCE_AVX2 GLM_FORCE_AVX2

#define GLM_ARCH_X86_BIT0x00000001
#define GLM_ARCH_SSE2_BIT0x00000002
#define GLM_ARCH_SSE3_BIT0x00000004
#define GLM_ARCH_SSSE3_BIT0x00000008
#define GLM_ARCH_SSE41_BIT0x00000010
#define GLM_ARCH_SSE42_BIT0x00000020
#define GLM_ARCH_AVX_BIT0x00000040
#define GLM_ARCH_AVX2_BIT0x00000080
#define GLM_ARCH_AVX512_BIT0x00000100 // Skylake subset
#define GLM_ARCH_ARM_BIT0x00000100
#define GLM_ARCH_NEON_BIT0x00000200
#define GLM_ARCH_MIPS_BIT0x00010000
#define GLM_ARCH_PPC_BIT0x01000000

#define GLM_ARCH_PURE(0x00000000)
#define GLM_ARCH_X86(GLM_ARCH_X86_BIT)
#define GLM_ARCH_SSE2(GLM_ARCH_SSE2_BIT | GLM_ARCH_X86)
#define GLM_ARCH_SSE3(GLM_ARCH_SSE3_BIT | GLM_ARCH_SSE2)
#define GLM_ARCH_SSSE3(GLM_ARCH_SSSE3_BIT | GLM_ARCH_SSE3)
#define GLM_ARCH_SSE41(GLM_ARCH_SSE41_BIT | GLM_ARCH_SSSE3)
#define GLM_ARCH_SSE42(GLM_ARCH_SSE42_BIT | GLM_ARCH_SSE41)
#define GLM_ARCH_AVX(GLM_ARCH_AVX_BIT | GLM_ARCH_SSE42)
#define GLM_ARCH_AVX2(GLM_ARCH_AVX2_BIT | GLM_ARCH_AVX)
#define GLM_ARCH_AVX512(GLM_ARCH_AVX512_BIT | GLM_ARCH_AVX2) // Skylake subset
#define GLM_ARCH_ARM(GLM_ARCH_ARM_BIT)
#define GLM_ARCH_NEON(GLM_ARCH_NEON_BIT | GLM_ARCH_ARM)
#define GLM_ARCH_MIPS(GLM_ARCH_MIPS_BIT)
#define GLM_ARCH_PPC(GLM_ARCH_PPC_BIT)

#if (GLM_FORCE_PURE)
#define GLM_ARCH GLM_ARCH_PURE
#elseif (GLM_FORCE_MIPS)
#define GLM_ARCH (GLM_ARCH_MIPS)
#elseif (GLM_FORCE_PPC)
#define GLM_ARCH (GLM_ARCH_PPC)
#elseif (GLM_FORCE_NEON)
#define GLM_ARCH (GLM_ARCH_NEON)
#elseif (GLM_FORCE_AVX512)
#define GLM_ARCH (GLM_ARCH_AVX512)
#elseif (GLM_FORCE_AVX2)
#define GLM_ARCH (GLM_ARCH_AVX2)
#elseif (GLM_FORCE_AVX)
#define GLM_ARCH (GLM_ARCH_AVX)
#elseif (GLM_FORCE_SSE42)
#define GLM_ARCH (GLM_ARCH_SSE42)
#elseif (GLM_FORCE_SSE41)
#define GLM_ARCH (GLM_ARCH_SSE41)
#elseif (GLM_FORCE_SSSE3)
#define GLM_ARCH (GLM_ARCH_SSSE3)
#elseif (GLM_FORCE_SSE3)
#define GLM_ARCH (GLM_ARCH_SSE3)
#elseif (GLM_FORCE_SSE2)
#define GLM_ARCH (GLM_ARCH_SSE2)
#elseif (GLM_COMPILER & (GLM_COMPILER_CLANG | GLM_COMPILER_GCC)) || ((GLM_COMPILER & GLM_COMPILER_INTEL) && (GLM_PLATFORM & GLM_PLATFORM_LINUX))
//This is Skylake set of instruction set
#if (__AVX512BW__) && (__AVX512F__) && (__AVX512CD__) && (__AVX512VL__) && (__AVX512DQ__)
#define GLM_ARCH (GLM_ARCH_AVX512)
#elseif (__AVX2__)
#define GLM_ARCH (GLM_ARCH_AVX2)
#elseif (__AVX__)
#define GLM_ARCH (GLM_ARCH_AVX)
#elseif (__SSE4_2__)
#define GLM_ARCH (GLM_ARCH_SSE42)
#elseif (__SSE4_1__)
#define GLM_ARCH (GLM_ARCH_SSE41)
#elseif (__SSSE3__)
#define GLM_ARCH (GLM_ARCH_SSSE3)
#elseif (__SSE3__)
#define GLM_ARCH (GLM_ARCH_SSE3)
#elseif (__SSE2__)
#define GLM_ARCH (GLM_ARCH_SSE2)
#elseif (__i386__) || (__x86_64__)
#define GLM_ARCH (GLM_ARCH_X86)
#elseif (__ARM_NEON)
#define GLM_ARCH (GLM_ARCH_ARM | GLM_ARCH_NEON)
#elseif (__arm__ )
#define GLM_ARCH (GLM_ARCH_ARM)
#elseif (__mips__ )
#define GLM_ARCH (GLM_ARCH_MIPS)
#elseif (__powerpc__ )
#define GLM_ARCH (GLM_ARCH_PPC)
#else
#define GLM_ARCH (GLM_ARCH_PURE)
#endif
#elseif (GLM_COMPILER & GLM_COMPILER_VC) || ((GLM_COMPILER & GLM_COMPILER_INTEL) && (GLM_PLATFORM & GLM_PLATFORM_WINDOWS))
#if (_M_ARM)
#define GLM_ARCH (GLM_ARCH_ARM)
#elseif (__AVX2__)
#define GLM_ARCH (GLM_ARCH_AVX2)
#elseif (__AVX__)
#define GLM_ARCH (GLM_ARCH_AVX)
#elseif (_M_X64)
#define GLM_ARCH (GLM_ARCH_SSE2)
#elseif (_M_IX86_FP)
#if _M_IX86_FP >= 2
#define GLM_ARCH (GLM_ARCH_SSE2)
#else
#define GLM_ARCH (GLM_ARCH_PURE)
#endif
#elseif (_M_PPC)
#define GLM_ARCH (GLM_ARCH_PPC)
#else
#define GLM_ARCH (GLM_ARCH_PURE)
#endif
#else
#define GLM_ARCH GLM_ARCH_PURE
#endif

// With MinGW-W64, including intrinsic headers before intrin.h will produce some errors. The problem is
// that windows.h (and maybe other headers) will silently include intrin.h, which of course causes problems.
// To fix, we just explicitly include intrin.h here.
#if (__MINGW64__) && (GLM_ARCH != GLM_ARCH_PURE)
#include <intrin.h>
#endif

#if GLM_ARCH & GLM_ARCH_AVX2_BIT
#include <immintrin.h>
#elseif GLM_ARCH & GLM_ARCH_AVX_BIT
#include <immintrin.h>
#elseif GLM_ARCH & GLM_ARCH_SSE42_BIT
#if GLM_COMPILER & GLM_COMPILER_CLANG
#include <popcntintrin.h>
#endif
#include <nmmintrin.h>
#elseif GLM_ARCH & GLM_ARCH_SSE41_BIT
#include <smmintrin.h>
#elseif GLM_ARCH & GLM_ARCH_SSSE3_BIT
#include <tmmintrin.h>
#elseif GLM_ARCH & GLM_ARCH_SSE3_BIT
#include <pmmintrin.h>
#elseif GLM_ARCH & GLM_ARCH_SSE2_BIT
#include <emmintrin.h>
#endif//GLM_ARCH

#if GLM_ARCH & GLM_ARCH_SSE2_BIT
typedef __m128glm_vec4;
typedef __m128iglm_ivec4;
typedef __m128iglm_uvec4;
#endif

#if GLM_ARCH & GLM_ARCH_AVX_BIT
typedef __m256dglm_dvec4;
#endif

#if GLM_ARCH & GLM_ARCH_AVX2_BIT
typedef __m256iglm_i64vec4;
typedef __m256iglm_u64vec4;
#endif
