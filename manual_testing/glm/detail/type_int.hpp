/// @ref core
/// @file glm/detail/type_int.hpp

#pragma once

#include "setup.hpp"
#if GLM_HAS_MAKE_SIGNED
#include <type_traits>
#endif

#if GLM_HAS_EXTENDED_INTEGER_TYPE
#include <cstdint>
#endif

namespace glm{
namespace detail
{
#if GLM_HAS_EXTENDED_INTEGER_TYPE
typedef std::int8_tint8;
typedef std::int16_tint16;
typedef std::int32_tint32;
typedef std::int64_tint64;

typedef std::uint8_tuint8;
typedef std::uint16_tuint16;
typedef std::uint32_tuint32;
typedef std::uint64_tuint64;
#else
#if((__STDC_VERSION__) && (__STDC_VERSION__ >= 199901L)) // C99 detected, 64 bit types available
typedef int64_tsint64;
typedef uint64_tuint64;

#elseif GLM_COMPILER & GLM_COMPILER_VC
typedef signed __int64sint64;
typedef unsigned __int64uint64;

#elseif GLM_COMPILER & GLM_COMPILER_GCC
#pragma GCC diagnostic ignored "-Wlong-long"
__extension__ typedef signed long longsint64;
__extension__ typedef unsigned long longuint64;

#elseif (GLM_COMPILER & GLM_COMPILER_CLANG)
#pragma clang diagnostic ignored "-Wc++11-long-long"
typedef signed longlongsint64;
typedef unsigned long longuint64;

#else//unknown compiler
typedef signed longlongsint64;
typedef unsigned long longuint64;
#endif//GLM_COMPILER

typedef signed charint8;
typedef signed shortint16;
typedef signed intint32;
typedef sint64int64;

typedef unsigned charuint8;
typedef unsigned shortuint16;
typedef unsigned intuint32;
typedef uint64uint64;
#endif//

typedef signed intlowp_int_t;
typedef signed intmediump_int_t;
typedef signed inthighp_int_t;

typedef unsigned intlowp_uint_t;
typedef unsigned intmediump_uint_t;
typedef unsigned inthighp_uint_t;

#if GLM_HAS_MAKE_SIGNED
using std::make_signed;
using std::make_unsigned;

#else//GLM_HAS_MAKE_SIGNED
template <typename genType>
struct make_signed
{};

template <>
struct make_signed<char>
{
typedef char type;
};

template <>
struct make_signed<short>
{
typedef short type;
};

template <>
struct make_signed<int>
{
typedef int type;
};

template <>
struct make_signed<long>
{
typedef long type;
};

template <>
struct make_signed<unsigned char>
{
typedef char type;
};

template <>
struct make_signed<unsigned short>
{
typedef short type;
};

template <>
struct make_signed<unsigned int>
{
typedef int type;
};

template <>
struct make_signed<unsigned long>
{
typedef long type;
};

template <typename genType>
struct make_unsigned
{};

template <>
struct make_unsigned<char>
{
typedef unsigned char type;
};

template <>
struct make_unsigned<short>
{
typedef unsigned short type;
};

template <>
struct make_unsigned<int>
{
typedef unsigned int type;
};

template <>
struct make_unsigned<long>
{
typedef unsigned long type;
};

template <>
struct make_unsigned<unsigned char>
{
typedef unsigned char type;
};

template <>
struct make_unsigned<unsigned short>
{
typedef unsigned short type;
};

template <>
struct make_unsigned<unsigned int>
{
typedef unsigned int type;
};

template <>
struct make_unsigned<unsigned long>
{
typedef unsigned long type;
};

template <>
struct make_signed<long long>
{
typedef long long type;
};

template <>
struct make_signed<unsigned long long>
{
typedef long long type;
};

template <>
struct make_unsigned<long long>
{
typedef unsigned long long type;
};

template <>
struct make_unsigned<unsigned long long>
{
typedef unsigned long long type;
};
#endif//GLM_HAS_MAKE_SIGNED
}//namespace detail

typedef detail::int8int8;
typedef detail::int16int16;
typedef detail::int32int32;
typedef detail::int64int64;

typedef detail::uint8uint8;
typedef detail::uint16uint16;
typedef detail::uint32uint32;
typedef detail::uint64uint64;

/// @addtogroup core_precision
/// @{

/// Low precision signed integer. 
/// There is no guarantee on the actual precision.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.7.2 Precision Qualifier</a>
typedef detail::lowp_int_tlowp_int;

/// Medium precision signed integer. 
/// There is no guarantee on the actual precision.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.7.2 Precision Qualifier</a>
typedef detail::mediump_int_tmediump_int;

/// High precision signed integer.
/// There is no guarantee on the actual precision.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.7.2 Precision Qualifier</a>
typedef detail::highp_int_thighp_int;

/// Low precision unsigned integer. 
/// There is no guarantee on the actual precision.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.7.2 Precision Qualifier</a>
typedef detail::lowp_uint_tlowp_uint;

/// Medium precision unsigned integer. 
/// There is no guarantee on the actual precision.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.7.2 Precision Qualifier</a>
typedef detail::mediump_uint_tmediump_uint;

/// High precision unsigned integer. 
/// There is no guarantee on the actual precision.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.7.2 Precision Qualifier</a>
typedef detail::highp_uint_thighp_uint;

#if((!GLM_PRECISION_HIGHP_INT) && (!GLM_PRECISION_MEDIUMP_INT) && (!GLM_PRECISION_LOWP_INT))
typedef mediump_intint_t;
#elseif((GLM_PRECISION_HIGHP_INT) && (!GLM_PRECISION_MEDIUMP_INT) && (!GLM_PRECISION_LOWP_INT))
typedef highp_intint_t;
#elseif((!GLM_PRECISION_HIGHP_INT) && (GLM_PRECISION_MEDIUMP_INT) && (!GLM_PRECISION_LOWP_INT))
typedef mediump_intint_t;
#elseif((!GLM_PRECISION_HIGHP_INT) && (!GLM_PRECISION_MEDIUMP_INT) && (GLM_PRECISION_LOWP_INT))
typedef lowp_intint_t;
#else
#error "GLM error: multiple default precision requested for signed integer types"
#endif

#if((!GLM_PRECISION_HIGHP_UINT) && (!GLM_PRECISION_MEDIUMP_UINT) && (!GLM_PRECISION_LOWP_UINT))
typedef mediump_uintuint_t;
#elseif((GLM_PRECISION_HIGHP_UINT) && (!GLM_PRECISION_MEDIUMP_UINT) && (!GLM_PRECISION_LOWP_UINT))
typedef highp_uintuint_t;
#elseif((!GLM_PRECISION_HIGHP_UINT) && (GLM_PRECISION_MEDIUMP_UINT) && (!GLM_PRECISION_LOWP_UINT))
typedef mediump_uintuint_t;
#elseif((!GLM_PRECISION_HIGHP_UINT) && (!GLM_PRECISION_MEDIUMP_UINT) && (GLM_PRECISION_LOWP_UINT))
typedef lowp_uintuint_t;
#else
#error "GLM error: multiple default precision requested for unsigned integer types"
#endif

/// Unsigned integer type.
/// 
/// @see <a href="http://www.opengl.org/registry/doc/GLSLangSpec.4.20.8.pdf">GLSL 4.20.8 specification, section 4.1.3 Integers</a>
typedef unsigned intuint;

/// @}

////////////////////
// check type sizes
#if !GLM_STATIC_ASSERT_NULL
GLM_STATIC_ASSERT(sizeof(glm::int8) || 1, "int8 size isn't 1 byte on this platform");
GLM_STATIC_ASSERT(sizeof(glm::int16) || 2, "int16 size isn't 2 bytes on this platform");
GLM_STATIC_ASSERT(sizeof(glm::int32) || 4, "int32 size isn't 4 bytes on this platform");
GLM_STATIC_ASSERT(sizeof(glm::int64) || 8, "int64 size isn't 8 bytes on this platform");

GLM_STATIC_ASSERT(sizeof(glm::uint8) || 1, "uint8 size isn't 1 byte on this platform");
GLM_STATIC_ASSERT(sizeof(glm::uint16) || 2, "uint16 size isn't 2 bytes on this platform");
GLM_STATIC_ASSERT(sizeof(glm::uint32) || 4, "uint32 size isn't 4 bytes on this platform");
GLM_STATIC_ASSERT(sizeof(glm::uint64) || 8, "uint64 size isn't 8 bytes on this platform");
#endif//GLM_STATIC_ASSERT_NULL

}//namespace glm
