/// @ref core
/// @file glm/fwd.hpp

#pragma once

#include "detail/type_int.hpp"
#include "detail/type_float.hpp"
#include "detail/type_vec.hpp"
#include "detail/type_mat.hpp"

//////////////////////
// GLM_GTC_quaternion
namespace glm
{
template <typename T, precision P> struct tquat;

/// Quaternion of low single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef tquat<float, lowp>lowp_quat;

/// Quaternion of medium single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef tquat<float, mediump>mediump_quat;

/// Quaternion of high single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef tquat<float, highp>highp_quat;

#if((GLM_PRECISION_HIGHP_FLOAT) && (!GLM_PRECISION_MEDIUMP_FLOAT) && (!GLM_PRECISION_LOWP_FLOAT))
typedef highp_quatquat;
#elseif((!GLM_PRECISION_HIGHP_FLOAT) && (GLM_PRECISION_MEDIUMP_FLOAT) && (!GLM_PRECISION_LOWP_FLOAT))
typedef mediump_quatquat;
#elseif((!GLM_PRECISION_HIGHP_FLOAT) && (!GLM_PRECISION_MEDIUMP_FLOAT) && (GLM_PRECISION_LOWP_FLOAT))
typedef lowp_quatquat;
#elseif((!GLM_PRECISION_HIGHP_FLOAT) && (!GLM_PRECISION_MEDIUMP_FLOAT) && (!GLM_PRECISION_LOWP_FLOAT))
/// Quaternion of default single-precision floating-point numbers.
typedef highp_quatquat;
#endif

/// Quaternion of low single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef lowp_quatlowp_fquat;

/// Quaternion of medium single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef mediump_quatmediump_fquat;

/// Quaternion of high single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef highp_quathighp_fquat;

/// Quaternion of default single-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef quatfquat;


/// Quaternion of low double-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef tquat<double, lowp>lowp_dquat;

/// Quaternion of medium double-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef tquat<double, mediump>mediump_dquat;

/// Quaternion of high double-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef tquat<double, highp>highp_dquat;

#if((GLM_PRECISION_HIGHP_DOUBLE) && (!GLM_PRECISION_MEDIUMP_DOUBLE) && (!GLM_PRECISION_LOWP_DOUBLE))
typedef highp_dquatdquat;
#elseif((!GLM_PRECISION_HIGHP_DOUBLE) && (GLM_PRECISION_MEDIUMP_DOUBLE) && (!GLM_PRECISION_LOWP_DOUBLE))
typedef mediump_dquatdquat;
#elseif((!GLM_PRECISION_HIGHP_DOUBLE) && (!GLM_PRECISION_MEDIUMP_DOUBLE) && (GLM_PRECISION_LOWP_DOUBLE))
typedef lowp_dquatdquat;
#elseif((!GLM_PRECISION_HIGHP_DOUBLE) && (!GLM_PRECISION_MEDIUMP_DOUBLE) && (!GLM_PRECISION_LOWP_DOUBLE))
/// Quaternion of default double-precision floating-point numbers.
///
/// @see gtc_quaternion
typedef highp_dquatdquat;
#endif

}//namespace glm

//////////////////////
// GLM_GTC_precision
namespace glm
{
/// Low precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 lowp_int8;

/// Low precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 lowp_int16;

/// Low precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 lowp_int32;

/// Low precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 lowp_int64;

/// Low precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 lowp_int8_t;

/// Low precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 lowp_int16_t;

/// Low precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 lowp_int32_t;

/// Low precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 lowp_int64_t;

/// Low precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 lowp_i8;

/// Low precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 lowp_i16;

/// Low precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 lowp_i32;

/// Low precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 lowp_i64;

/// Medium precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 mediump_int8;

/// Medium precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 mediump_int16;

/// Medium precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 mediump_int32;

/// Medium precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 mediump_int64;

/// Medium precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 mediump_int8_t;

/// Medium precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 mediump_int16_t;

/// Medium precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 mediump_int32_t;

/// Medium precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 mediump_int64_t;

/// Medium precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 mediump_i8;

/// Medium precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 mediump_i16;

/// Medium precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 mediump_i32;

/// Medium precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 mediump_i64;

/// High precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 highp_int8;

/// High precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 highp_int16;

/// High precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 highp_int32;

/// High precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 highp_int64;

/// High precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 highp_int8_t;

/// High precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 highp_int16_t;

/// 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 highp_int32_t;

/// High precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 highp_int64_t;

/// High precision 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 highp_i8;

/// High precision 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 highp_i16;

/// High precision 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 highp_i32;

/// High precision 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 highp_i64;


/// 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 int8;

/// 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 int16;

/// 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 int32;

/// 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 int64;


#if GLM_HAS_EXTENDED_INTEGER_TYPE
using std::int8_t;
using std::int16_t;
using std::int32_t;
using std::int64_t;
#else
/// 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 int8_t;

/// 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 int16_t;

/// 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 int32_t;

/// 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 int64_t;
#endif

/// 8 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int8 i8;

/// 16 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int16 i16;

/// 32 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int32 i32;

/// 64 bit signed integer type.
/// @see gtc_type_precision
typedef detail::int64 i64;



/// Low precision 8 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i8, lowp> lowp_i8vec1;

/// Low precision 8 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i8, lowp> lowp_i8vec2;

/// Low precision 8 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i8, lowp> lowp_i8vec3;

/// Low precision 8 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i8, lowp> lowp_i8vec4;


/// Medium precision 8 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i8, mediump> mediump_i8vec1;

/// Medium precision 8 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i8, mediump> mediump_i8vec2;

/// Medium precision 8 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i8, mediump> mediump_i8vec3;

/// Medium precision 8 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i8, mediump> mediump_i8vec4;


/// High precision 8 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i8, highp> highp_i8vec1;

/// High precision 8 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i8, highp> highp_i8vec2;

/// High precision 8 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i8, highp> highp_i8vec3;

/// High precision 8 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i8, highp> highp_i8vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_i8vec1i8vec1;
typedef lowp_i8vec2i8vec2;
typedef lowp_i8vec3i8vec3;
typedef lowp_i8vec4i8vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_i8vec1i8vec1;
typedef mediump_i8vec2i8vec2;
typedef mediump_i8vec3i8vec3;
typedef mediump_i8vec4i8vec4;
#else
/// Default precision 8 bit signed integer scalar type.
/// @see gtc_type_precision
typedef highp_i8vec1i8vec1;

/// Default precision 8 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_i8vec2i8vec2;

/// Default precision 8 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_i8vec3i8vec3;

/// Default precision 8 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_i8vec4i8vec4;
#endif


/// Low precision 16 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i16, lowp>lowp_i16vec1;

/// Low precision 16 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i16, lowp>lowp_i16vec2;

/// Low precision 16 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i16, lowp>lowp_i16vec3;

/// Low precision 16 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i16, lowp>lowp_i16vec4;


/// Medium precision 16 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i16, mediump>mediump_i16vec1;

/// Medium precision 16 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i16, mediump>mediump_i16vec2;

/// Medium precision 16 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i16, mediump>mediump_i16vec3;

/// Medium precision 16 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i16, mediump>mediump_i16vec4;


/// High precision 16 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i16, highp>highp_i16vec1;

/// High precision 16 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i16, highp>highp_i16vec2;

/// High precision 16 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i16, highp>highp_i16vec3;

/// High precision 16 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i16, highp>highp_i16vec4;


#if((GLM_PRECISION_LOWP_INT))
typedef lowp_i16vec1i16vec1;
typedef lowp_i16vec2i16vec2;
typedef lowp_i16vec3i16vec3;
typedef lowp_i16vec4i16vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_i16vec1i16vec1;
typedef mediump_i16vec2i16vec2;
typedef mediump_i16vec3i16vec3;
typedef mediump_i16vec4i16vec4;
#else
/// Default precision 16 bit signed integer scalar type.
/// @see gtc_type_precision
typedef highp_i16vec1i16vec1;

/// Default precision 16 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_i16vec2i16vec2;

/// Default precision 16 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_i16vec3i16vec3;

/// Default precision 16 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_i16vec4i16vec4;
#endif


/// Low precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i32, lowp>lowp_i32vec1;

/// Low precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i32, lowp>lowp_i32vec2;

/// Low precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i32, lowp>lowp_i32vec3;

/// Low precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i32, lowp>lowp_i32vec4;


/// Medium precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i32, mediump>mediump_i32vec1;

/// Medium precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i32, mediump>mediump_i32vec2;

/// Medium precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i32, mediump>mediump_i32vec3;

/// Medium precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i32, mediump>mediump_i32vec4;


/// High precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i32, highp>highp_i32vec1;

/// High precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i32, highp>highp_i32vec2;

/// High precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i32, highp>highp_i32vec3;

/// High precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i32, highp>highp_i32vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_i32vec1i32vec1;
typedef lowp_i32vec2i32vec2;
typedef lowp_i32vec3i32vec3;
typedef lowp_i32vec4i32vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_i32vec1i32vec1;
typedef mediump_i32vec2i32vec2;
typedef mediump_i32vec3i32vec3;
typedef mediump_i32vec4i32vec4;
#else
/// Default precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef highp_i32vec1i32vec1;

/// Default precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_i32vec2i32vec2;

/// Default precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_i32vec3i32vec3;

/// Default precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_i32vec4i32vec4;
#endif


/// Low precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i32, lowp>lowp_i32vec1;

/// Low precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i32, lowp>lowp_i32vec2;

/// Low precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i32, lowp>lowp_i32vec3;

/// Low precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i32, lowp>lowp_i32vec4;


/// Medium precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i32, mediump>mediump_i32vec1;

/// Medium precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i32, mediump>mediump_i32vec2;

/// Medium precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i32, mediump>mediump_i32vec3;

/// Medium precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i32, mediump>mediump_i32vec4;


/// High precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i32, highp>highp_i32vec1;

/// High precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i32, highp>highp_i32vec2;

/// High precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i32, highp>highp_i32vec3;

/// High precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i32, highp>highp_i32vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_i32vec1i32vec1;
typedef lowp_i32vec2i32vec2;
typedef lowp_i32vec3i32vec3;
typedef lowp_i32vec4i32vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_i32vec1i32vec1;
typedef mediump_i32vec2i32vec2;
typedef mediump_i32vec3i32vec3;
typedef mediump_i32vec4i32vec4;
#else
/// Default precision 32 bit signed integer scalar type.
/// @see gtc_type_precision
typedef highp_i32vec1i32vec1;

/// Default precision 32 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_i32vec2i32vec2;

/// Default precision 32 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_i32vec3i32vec3;

/// Default precision 32 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_i32vec4i32vec4;
#endif



/// Low precision 64 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i64, lowp>lowp_i64vec1;

/// Low precision 64 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i64, lowp>lowp_i64vec2;

/// Low precision 64 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i64, lowp>lowp_i64vec3;

/// Low precision 64 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i64, lowp>lowp_i64vec4;


/// Medium precision 64 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i64, mediump>mediump_i64vec1;

/// Medium precision 64 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i64, mediump>mediump_i64vec2;

/// Medium precision 64 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i64, mediump>mediump_i64vec3;

/// Medium precision 64 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i64, mediump>mediump_i64vec4;


/// High precision 64 bit signed integer scalar type.
/// @see gtc_type_precision
typedef tvec1<i64, highp>highp_i64vec1;

/// High precision 64 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<i64, highp>highp_i64vec2;

/// High precision 64 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<i64, highp>highp_i64vec3;

/// High precision 64 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<i64, highp>highp_i64vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_i64vec1i64vec1;
typedef lowp_i64vec2i64vec2;
typedef lowp_i64vec3i64vec3;
typedef lowp_i64vec4i64vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_i64vec1i64vec1;
typedef mediump_i64vec2i64vec2;
typedef mediump_i64vec3i64vec3;
typedef mediump_i64vec4i64vec4;
#else
/// Default precision 64 bit signed integer scalar type.
/// @see gtc_type_precision
typedef highp_i64vec1i64vec1;

/// Default precision 64 bit signed integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_i64vec2i64vec2;

/// Default precision 64 bit signed integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_i64vec3i64vec3;

/// Default precision 64 bit signed integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_i64vec4i64vec4;
#endif


/////////////////////////////
// Unsigned int vector types

/// Low precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 lowp_uint8;

/// Low precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 lowp_uint16;

/// Low precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 lowp_uint32;

/// Low precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 lowp_uint64;


/// Low precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 lowp_uint8_t;

/// Low precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 lowp_uint16_t;

/// Low precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 lowp_uint32_t;

/// Low precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 lowp_uint64_t;


/// Low precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 lowp_u8;

/// Low precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 lowp_u16;

/// Low precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 lowp_u32;

/// Low precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 lowp_u64;



/// Medium precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 mediump_uint8;

/// Medium precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 mediump_uint16;

/// Medium precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 mediump_uint32;

/// Medium precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 mediump_uint64;

/// Medium precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 mediump_uint8_t;

/// Medium precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 mediump_uint16_t;

/// Medium precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 mediump_uint32_t;

/// Medium precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 mediump_uint64_t;

/// Medium precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 mediump_u8;

/// Medium precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 mediump_u16;

/// Medium precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 mediump_u32;

/// Medium precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 mediump_u64;



/// Medium precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 highp_uint8;

/// Medium precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 highp_uint16;

/// Medium precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 highp_uint32;

/// Medium precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 highp_uint64;

/// Medium precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 highp_uint8_t;

/// Medium precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 highp_uint16_t;

/// Medium precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 highp_uint32_t;

/// Medium precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 highp_uint64_t;

/// Medium precision 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 highp_u8;

/// Medium precision 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 highp_u16;

/// Medium precision 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 highp_u32;

/// Medium precision 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 highp_u64;



/// 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 uint8;

/// 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 uint16;

/// 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 uint32;

/// 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 uint64;

#if GLM_HAS_EXTENDED_INTEGER_TYPE
using std::uint8_t;
using std::uint16_t;
using std::uint32_t;
using std::uint64_t;
#else
/// 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 uint8_t;

/// 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 uint16_t;

/// 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 uint32_t;

/// 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 uint64_t;
#endif

/// 8 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint8 u8;

/// 16 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint16 u16;

/// 32 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint32 u32;

/// 64 bit unsigned integer type.
/// @see gtc_type_precision
typedef detail::uint64 u64;



/// Low precision 8 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u8, lowp> lowp_u8vec1;

/// Low precision 8 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u8, lowp> lowp_u8vec2;

/// Low precision 8 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u8, lowp> lowp_u8vec3;

/// Low precision 8 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u8, lowp> lowp_u8vec4;


/// Medium precision 8 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u8, mediump> mediump_u8vec1;

/// Medium precision 8 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u8, mediump> mediump_u8vec2;

/// Medium precision 8 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u8, mediump> mediump_u8vec3;

/// Medium precision 8 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u8, mediump> mediump_u8vec4;


/// High precision 8 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u8, highp> highp_u8vec1;

/// High precision 8 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u8, highp> highp_u8vec2;

/// High precision 8 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u8, highp> highp_u8vec3;

/// High precision 8 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u8, highp> highp_u8vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_u8vec1u8vec1;
typedef lowp_u8vec2u8vec2;
typedef lowp_u8vec3u8vec3;
typedef lowp_u8vec4u8vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_u8vec1u8vec1;
typedef mediump_u8vec2u8vec2;
typedef mediump_u8vec3u8vec3;
typedef mediump_u8vec4u8vec4;
#else
/// Default precision 8 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef highp_u8vec1u8vec1;

/// Default precision 8 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_u8vec2u8vec2;

/// Default precision 8 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_u8vec3u8vec3;

/// Default precision 8 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_u8vec4u8vec4;
#endif


/// Low precision 16 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u16, lowp>lowp_u16vec1;

/// Low precision 16 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u16, lowp>lowp_u16vec2;

/// Low precision 16 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u16, lowp>lowp_u16vec3;

/// Low precision 16 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u16, lowp>lowp_u16vec4;


/// Medium precision 16 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u16, mediump>mediump_u16vec1;

/// Medium precision 16 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u16, mediump>mediump_u16vec2;

/// Medium precision 16 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u16, mediump>mediump_u16vec3;

/// Medium precision 16 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u16, mediump>mediump_u16vec4;


/// High precision 16 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u16, highp>highp_u16vec1;

/// High precision 16 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u16, highp>highp_u16vec2;

/// High precision 16 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u16, highp>highp_u16vec3;

/// High precision 16 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u16, highp>highp_u16vec4;


#if((GLM_PRECISION_LOWP_INT))
typedef lowp_u16vec1u16vec1;
typedef lowp_u16vec2u16vec2;
typedef lowp_u16vec3u16vec3;
typedef lowp_u16vec4u16vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_u16vec1u16vec1;
typedef mediump_u16vec2u16vec2;
typedef mediump_u16vec3u16vec3;
typedef mediump_u16vec4u16vec4;
#else
/// Default precision 16 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef highp_u16vec1u16vec1;

/// Default precision 16 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_u16vec2u16vec2;

/// Default precision 16 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_u16vec3u16vec3;

/// Default precision 16 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_u16vec4u16vec4;
#endif


/// Low precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u32, lowp>lowp_u32vec1;

/// Low precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u32, lowp>lowp_u32vec2;

/// Low precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u32, lowp>lowp_u32vec3;

/// Low precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u32, lowp>lowp_u32vec4;


/// Medium precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u32, mediump>mediump_u32vec1;

/// Medium precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u32, mediump>mediump_u32vec2;

/// Medium precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u32, mediump>mediump_u32vec3;

/// Medium precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u32, mediump>mediump_u32vec4;


/// High precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u32, highp>highp_u32vec1;

/// High precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u32, highp>highp_u32vec2;

/// High precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u32, highp>highp_u32vec3;

/// High precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u32, highp>highp_u32vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_u32vec1u32vec1;
typedef lowp_u32vec2u32vec2;
typedef lowp_u32vec3u32vec3;
typedef lowp_u32vec4u32vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_u32vec1u32vec1;
typedef mediump_u32vec2u32vec2;
typedef mediump_u32vec3u32vec3;
typedef mediump_u32vec4u32vec4;
#else
/// Default precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef highp_u32vec1u32vec1;

/// Default precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_u32vec2u32vec2;

/// Default precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_u32vec3u32vec3;

/// Default precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_u32vec4u32vec4;
#endif


/// Low precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u32, lowp>lowp_u32vec1;

/// Low precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u32, lowp>lowp_u32vec2;

/// Low precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u32, lowp>lowp_u32vec3;

/// Low precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u32, lowp>lowp_u32vec4;


/// Medium precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u32, mediump>mediump_u32vec1;

/// Medium precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u32, mediump>mediump_u32vec2;

/// Medium precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u32, mediump>mediump_u32vec3;

/// Medium precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u32, mediump>mediump_u32vec4;


/// High precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u32, highp>highp_u32vec1;

/// High precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u32, highp>highp_u32vec2;

/// High precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u32, highp>highp_u32vec3;

/// High precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u32, highp>highp_u32vec4;

#if((GLM_PRECISION_LOWP_INT))
typedef lowp_u32vec1u32vec1;
typedef lowp_u32vec2u32vec2;
typedef lowp_u32vec3u32vec3;
typedef lowp_u32vec4u32vec4;
#elseif((GLM_PRECISION_MEDIUMP_INT))
typedef mediump_u32vec1u32vec1;
typedef mediump_u32vec2u32vec2;
typedef mediump_u32vec3u32vec3;
typedef mediump_u32vec4u32vec4;
#else
/// Default precision 32 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef highp_u32vec1u32vec1;

/// Default precision 32 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_u32vec2u32vec2;

/// Default precision 32 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_u32vec3u32vec3;

/// Default precision 32 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_u32vec4u32vec4;
#endif



/// Low precision 64 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u64, lowp>lowp_u64vec1;

/// Low precision 64 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u64, lowp>lowp_u64vec2;

/// Low precision 64 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u64, lowp>lowp_u64vec3;

/// Low precision 64 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u64, lowp>lowp_u64vec4;


/// Medium precision 64 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u64, mediump>mediump_u64vec1;

/// Medium precision 64 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u64, mediump>mediump_u64vec2;

/// Medium precision 64 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u64, mediump>mediump_u64vec3;

/// Medium precision 64 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u64, mediump>mediump_u64vec4;


/// High precision 64 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef tvec1<u64, highp>highp_u64vec1;

/// High precision 64 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef tvec2<u64, highp>highp_u64vec2;

/// High precision 64 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef tvec3<u64, highp>highp_u64vec3;

/// High precision 64 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef tvec4<u64, highp>highp_u64vec4;

#if((GLM_PRECISION_LOWP_UINT))
typedef lowp_u64vec1u64vec1;
typedef lowp_u64vec2u64vec2;
typedef lowp_u64vec3u64vec3;
typedef lowp_u64vec4u64vec4;
#elseif((GLM_PRECISION_MEDIUMP_UINT))
typedef mediump_u64vec1u64vec1;
typedef mediump_u64vec2u64vec2;
typedef mediump_u64vec3u64vec3;
typedef mediump_u64vec4u64vec4;
#else
/// Default precision 64 bit unsigned integer scalar type.
/// @see gtc_type_precision
typedef highp_u64vec1u64vec1;

/// Default precision 64 bit unsigned integer vector of 2 components type.
/// @see gtc_type_precision
typedef highp_u64vec2u64vec2;

/// Default precision 64 bit unsigned integer vector of 3 components type.
/// @see gtc_type_precision
typedef highp_u64vec3u64vec3;

/// Default precision 64 bit unsigned integer vector of 4 components type.
/// @see gtc_type_precision
typedef highp_u64vec4u64vec4;
#endif


//////////////////////
// Float vector types

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 lowp_float32;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 lowp_float64;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 lowp_float32_t;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 lowp_float64_t;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef float32 lowp_f32;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef float64 lowp_f64;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 lowp_float32;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 lowp_float64;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 lowp_float32_t;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 lowp_float64_t;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef float32 lowp_f32;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef float64 lowp_f64;


/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 lowp_float32;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 lowp_float64;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 lowp_float32_t;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 lowp_float64_t;

/// Low 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef float32 lowp_f32;

/// Low 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef float64 lowp_f64;


/// Medium 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 mediump_float32;

/// Medium 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 mediump_float64;

/// Medium 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 mediump_float32_t;

/// Medium 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 mediump_float64_t;

/// Medium 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef float32 mediump_f32;

/// Medium 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef float64 mediump_f64;


/// High 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 highp_float32;

/// High 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 highp_float64;

/// High 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float32 highp_float32_t;

/// High 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef detail::float64 highp_float64_t;

/// High 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef float32 highp_f32;

/// High 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef float64 highp_f64;


#if((GLM_PRECISION_LOWP_FLOAT))
/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef lowp_float32 float32;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef lowp_float64 float64;

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef lowp_float32_t float32_t;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef lowp_float64_t float64_t;

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef lowp_f32 f32;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef lowp_f64 f64;

#elseif((GLM_PRECISION_MEDIUMP_FLOAT))

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef mediump_float32 float32;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef mediump_float64 float64;

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef mediump_float32 float32_t;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef mediump_float64 float64_t;

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef mediump_float32 f32;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef mediump_float64 f64;

#else//((GLM_PRECISION_HIGHP_FLOAT))

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef highp_float32 float32;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef highp_float64 float64;

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef highp_float32_t float32_t;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef highp_float64_t float64_t;

/// Default 32 bit single-precision floating-point scalar.
/// @see gtc_type_precision
typedef highp_float32_t f32;

/// Default 64 bit double-precision floating-point scalar.
/// @see gtc_type_precision
typedef highp_float64_t f64;
#endif


/// Low single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<float, lowp> lowp_vec1;

/// Low single-precision floating-point vector of 2 components.
/// @see core_precision
typedef tvec2<float, lowp> lowp_vec2;

/// Low single-precision floating-point vector of 3 components.
/// @see core_precision
typedef tvec3<float, lowp> lowp_vec3;

/// Low single-precision floating-point vector of 4 components.
/// @see core_precision
typedef tvec4<float, lowp> lowp_vec4;

/// Low single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<float, lowp> lowp_fvec1;

/// Low single-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef tvec2<float, lowp> lowp_fvec2;

/// Low single-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef tvec3<float, lowp> lowp_fvec3;

/// Low single-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef tvec4<float, lowp> lowp_fvec4;


/// Medium single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<float, mediump> mediump_vec1;

/// Medium Single-precision floating-point vector of 2 components.
/// @see core_precision
typedef tvec2<float, mediump> mediump_vec2;

/// Medium Single-precision floating-point vector of 3 components.
/// @see core_precision
typedef tvec3<float, mediump> mediump_vec3;

/// Medium Single-precision floating-point vector of 4 components.
/// @see core_precision
typedef tvec4<float, mediump> mediump_vec4;

/// Medium single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<float, mediump> mediump_fvec1;

/// Medium Single-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef tvec2<float, mediump> mediump_fvec2;

/// Medium Single-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef tvec3<float, mediump> mediump_fvec3;

/// Medium Single-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef tvec4<float, mediump> mediump_fvec4;


/// High single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<float, highp> highp_vec1;

/// High Single-precision floating-point vector of 2 components.
/// @see core_precision
typedef tvec2<float, highp> highp_vec2;

/// High Single-precision floating-point vector of 3 components.
/// @see core_precision
typedef tvec3<float, highp> highp_vec3;

/// High Single-precision floating-point vector of 4 components.
/// @see core_precision
typedef tvec4<float, highp> highp_vec4;

/// High single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<float, highp> highp_fvec1;

/// High Single-precision floating-point vector of 2 components.
/// @see core_precision
typedef tvec2<float, highp> highp_fvec2;

/// High Single-precision floating-point vector of 3 components.
/// @see core_precision
typedef tvec3<float, highp> highp_fvec3;

/// High Single-precision floating-point vector of 4 components.
/// @see core_precision
typedef tvec4<float, highp> highp_fvec4;


/// Low single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<f32, lowp> lowp_f32vec1;

/// Low single-precision floating-point vector of 2 components.
/// @see core_precision
typedef tvec2<f32, lowp> lowp_f32vec2;

/// Low single-precision floating-point vector of 3 components.
/// @see core_precision
typedef tvec3<f32, lowp> lowp_f32vec3;

/// Low single-precision floating-point vector of 4 components.
/// @see core_precision
typedef tvec4<f32, lowp> lowp_f32vec4;

/// Medium single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<f32, mediump> mediump_f32vec1;

/// Medium single-precision floating-point vector of 2 components.
/// @see core_precision
typedef tvec2<f32, mediump> mediump_f32vec2;

/// Medium single-precision floating-point vector of 3 components.
/// @see core_precision
typedef tvec3<f32, mediump> mediump_f32vec3;

/// Medium single-precision floating-point vector of 4 components.
/// @see core_precision
typedef tvec4<f32, mediump> mediump_f32vec4;

/// High single-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<f32, highp> highp_f32vec1;

/// High single-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef tvec2<f32, highp> highp_f32vec2;

/// High single-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef tvec3<f32, highp> highp_f32vec3;

/// High single-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef tvec4<f32, highp> highp_f32vec4;


/// Low double-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<f64, lowp> lowp_f64vec1;

/// Low double-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef tvec2<f64, lowp> lowp_f64vec2;

/// Low double-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef tvec3<f64, lowp> lowp_f64vec3;

/// Low double-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef tvec4<f64, lowp> lowp_f64vec4;

/// Medium double-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<f64, mediump> mediump_f64vec1;

/// Medium double-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef tvec2<f64, mediump> mediump_f64vec2;

/// Medium double-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef tvec3<f64, mediump> mediump_f64vec3;

/// Medium double-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef tvec4<f64, mediump> mediump_f64vec4;

/// High double-precision floating-point vector of 1 component.
/// @see gtc_type_precision
typedef tvec1<f64, highp> highp_f64vec1;

/// High double-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef tvec2<f64, highp> highp_f64vec2;

/// High double-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef tvec3<f64, highp> highp_f64vec3;

/// High double-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef tvec4<f64, highp> highp_f64vec4;


//////////////////////
// Float matrix types

/// Low single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef lowp_f32 lowp_fmat1x1;

/// Low single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f32, lowp> lowp_fmat2x2;

/// Low single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f32, lowp> lowp_fmat2x3;

/// Low single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f32, lowp> lowp_fmat2x4;

/// Low single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f32, lowp> lowp_fmat3x2;

/// Low single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f32, lowp> lowp_fmat3x3;

/// Low single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f32, lowp> lowp_fmat3x4;

/// Low single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f32, lowp> lowp_fmat4x2;

/// Low single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f32, lowp> lowp_fmat4x3;

/// Low single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f32, lowp> lowp_fmat4x4;

/// Low single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef lowp_fmat1x1 lowp_fmat1;

/// Low single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef lowp_fmat2x2 lowp_fmat2;

/// Low single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef lowp_fmat3x3 lowp_fmat3;

/// Low single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef lowp_fmat4x4 lowp_fmat4;


/// Medium single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef mediump_f32 mediump_fmat1x1;

/// Medium single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f32, mediump> mediump_fmat2x2;

/// Medium single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f32, mediump> mediump_fmat2x3;

/// Medium single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f32, mediump> mediump_fmat2x4;

/// Medium single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f32, mediump> mediump_fmat3x2;

/// Medium single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f32, mediump> mediump_fmat3x3;

/// Medium single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f32, mediump> mediump_fmat3x4;

/// Medium single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f32, mediump> mediump_fmat4x2;

/// Medium single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f32, mediump> mediump_fmat4x3;

/// Medium single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f32, mediump> mediump_fmat4x4;

/// Medium single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef mediump_fmat1x1 mediump_fmat1;

/// Medium single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef mediump_fmat2x2 mediump_fmat2;

/// Medium single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef mediump_fmat3x3 mediump_fmat3;

/// Medium single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef mediump_fmat4x4 mediump_fmat4;


/// High single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef highp_f32 highp_fmat1x1;

/// High single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f32, highp> highp_fmat2x2;

/// High single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f32, highp> highp_fmat2x3;

/// High single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f32, highp> highp_fmat2x4;

/// High single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f32, highp> highp_fmat3x2;

/// High single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f32, highp> highp_fmat3x3;

/// High single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f32, highp> highp_fmat3x4;

/// High single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f32, highp> highp_fmat4x2;

/// High single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f32, highp> highp_fmat4x3;

/// High single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f32, highp> highp_fmat4x4;

/// High single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef highp_fmat1x1 highp_fmat1;

/// High single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef highp_fmat2x2 highp_fmat2;

/// High single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef highp_fmat3x3 highp_fmat3;

/// High single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef highp_fmat4x4 highp_fmat4;


/// Low single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef f32 lowp_f32mat1x1;

/// Low single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f32, lowp> lowp_f32mat2x2;

/// Low single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f32, lowp> lowp_f32mat2x3;

/// Low single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f32, lowp> lowp_f32mat2x4;

/// Low single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f32, lowp> lowp_f32mat3x2;

/// Low single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f32, lowp> lowp_f32mat3x3;

/// Low single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f32, lowp> lowp_f32mat3x4;

/// Low single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f32, lowp> lowp_f32mat4x2;

/// Low single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f32, lowp> lowp_f32mat4x3;

/// Low single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f32, lowp> lowp_f32mat4x4;

/// Low single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef detail::tmat1x1<f32, lowp> lowp_f32mat1;

/// Low single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef lowp_f32mat2x2 lowp_f32mat2;

/// Low single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef lowp_f32mat3x3 lowp_f32mat3;

/// Low single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef lowp_f32mat4x4 lowp_f32mat4;


/// High single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef f32 mediump_f32mat1x1;

/// Low single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f32, mediump> mediump_f32mat2x2;

/// Medium single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f32, mediump> mediump_f32mat2x3;

/// Medium single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f32, mediump> mediump_f32mat2x4;

/// Medium single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f32, mediump> mediump_f32mat3x2;

/// Medium single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f32, mediump> mediump_f32mat3x3;

/// Medium single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f32, mediump> mediump_f32mat3x4;

/// Medium single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f32, mediump> mediump_f32mat4x2;

/// Medium single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f32, mediump> mediump_f32mat4x3;

/// Medium single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f32, mediump> mediump_f32mat4x4;

/// Medium single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef detail::tmat1x1<f32, mediump> f32mat1;

/// Medium single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef mediump_f32mat2x2 mediump_f32mat2;

/// Medium single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef mediump_f32mat3x3 mediump_f32mat3;

/// Medium single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef mediump_f32mat4x4 mediump_f32mat4;


/// High single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef f32 highp_f32mat1x1;

/// High single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f32, highp> highp_f32mat2x2;

/// High single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f32, highp> highp_f32mat2x3;

/// High single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f32, highp> highp_f32mat2x4;

/// High single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f32, highp> highp_f32mat3x2;

/// High single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f32, highp> highp_f32mat3x3;

/// High single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f32, highp> highp_f32mat3x4;

/// High single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f32, highp> highp_f32mat4x2;

/// High single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f32, highp> highp_f32mat4x3;

/// High single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f32, highp> highp_f32mat4x4;

/// High single-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef detail::tmat1x1<f32, highp> f32mat1;

/// High single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x2 highp_f32mat2;

/// High single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x3 highp_f32mat3;

/// High single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x4 highp_f32mat4;


/// Low double-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef f64 lowp_f64mat1x1;

/// Low double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f64, lowp> lowp_f64mat2x2;

/// Low double-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f64, lowp> lowp_f64mat2x3;

/// Low double-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f64, lowp> lowp_f64mat2x4;

/// Low double-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f64, lowp> lowp_f64mat3x2;

/// Low double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f64, lowp> lowp_f64mat3x3;

/// Low double-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f64, lowp> lowp_f64mat3x4;

/// Low double-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f64, lowp> lowp_f64mat4x2;

/// Low double-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f64, lowp> lowp_f64mat4x3;

/// Low double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f64, lowp> lowp_f64mat4x4;

/// Low double-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef lowp_f64mat1x1 lowp_f64mat1;

/// Low double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef lowp_f64mat2x2 lowp_f64mat2;

/// Low double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef lowp_f64mat3x3 lowp_f64mat3;

/// Low double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef lowp_f64mat4x4 lowp_f64mat4;


/// Medium double-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef f64 Highp_f64mat1x1;

/// Medium double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f64, mediump> mediump_f64mat2x2;

/// Medium double-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f64, mediump> mediump_f64mat2x3;

/// Medium double-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f64, mediump> mediump_f64mat2x4;

/// Medium double-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f64, mediump> mediump_f64mat3x2;

/// Medium double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f64, mediump> mediump_f64mat3x3;

/// Medium double-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f64, mediump> mediump_f64mat3x4;

/// Medium double-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f64, mediump> mediump_f64mat4x2;

/// Medium double-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f64, mediump> mediump_f64mat4x3;

/// Medium double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f64, mediump> mediump_f64mat4x4;

/// Medium double-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef mediump_f64mat1x1 mediump_f64mat1;

/// Medium double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef mediump_f64mat2x2 mediump_f64mat2;

/// Medium double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef mediump_f64mat3x3 mediump_f64mat3;

/// Medium double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef mediump_f64mat4x4 mediump_f64mat4;

/// High double-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef f64 highp_f64mat1x1;

/// High double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef tmat2x2<f64, highp> highp_f64mat2x2;

/// High double-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef tmat2x3<f64, highp> highp_f64mat2x3;

/// High double-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef tmat2x4<f64, highp> highp_f64mat2x4;

/// High double-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef tmat3x2<f64, highp> highp_f64mat3x2;

/// High double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef tmat3x3<f64, highp> highp_f64mat3x3;

/// High double-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef tmat3x4<f64, highp> highp_f64mat3x4;

/// High double-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef tmat4x2<f64, highp> highp_f64mat4x2;

/// High double-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef tmat4x3<f64, highp> highp_f64mat4x3;

/// High double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef tmat4x4<f64, highp> highp_f64mat4x4;

/// High double-precision floating-point 1x1 matrix.
/// @see gtc_type_precision
//typedef highp_f64mat1x1 highp_f64mat1;

/// High double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef highp_f64mat2x2 highp_f64mat2;

/// High double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef highp_f64mat3x3 highp_f64mat3;

/// High double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef highp_f64mat4x4 highp_f64mat4;

//////////////////////////
// Quaternion types

/// Low single-precision floating-point quaternion.
/// @see gtc_type_precision
typedef tquat<f32, lowp> lowp_f32quat;

/// Low double-precision floating-point quaternion.
/// @see gtc_type_precision
typedef tquat<f64, lowp> lowp_f64quat;

/// Medium single-precision floating-point quaternion.
/// @see gtc_type_precision
typedef tquat<f32, mediump> mediump_f32quat;

/// Medium double-precision floating-point quaternion.
/// @see gtc_type_precision
typedef tquat<f64, mediump> mediump_f64quat;

/// High single-precision floating-point quaternion.
/// @see gtc_type_precision
typedef tquat<f32, highp> highp_f32quat;

/// High double-precision floating-point quaternion.
/// @see gtc_type_precision
typedef tquat<f64, highp> highp_f64quat;


#if((GLM_PRECISION_LOWP_FLOAT))
typedef lowp_f32vec1fvec1;
typedef lowp_f32vec2fvec2;
typedef lowp_f32vec3fvec3;
typedef lowp_f32vec4fvec4;
typedef lowp_f32mat2fmat2;
typedef lowp_f32mat3fmat3;
typedef lowp_f32mat4fmat4;
typedef lowp_f32mat2x2fmat2x2;
typedef lowp_f32mat3x2fmat3x2;
typedef lowp_f32mat4x2fmat4x2;
typedef lowp_f32mat2x3fmat2x3;
typedef lowp_f32mat3x3fmat3x3;
typedef lowp_f32mat4x3fmat4x3;
typedef lowp_f32mat2x4fmat2x4;
typedef lowp_f32mat3x4fmat3x4;
typedef lowp_f32mat4x4fmat4x4;
typedef lowp_f32quatfquat;

typedef lowp_f32vec1f32vec1;
typedef lowp_f32vec2f32vec2;
typedef lowp_f32vec3f32vec3;
typedef lowp_f32vec4f32vec4;
typedef lowp_f32mat2f32mat2;
typedef lowp_f32mat3f32mat3;
typedef lowp_f32mat4f32mat4;
typedef lowp_f32mat2x2f32mat2x2;
typedef lowp_f32mat3x2f32mat3x2;
typedef lowp_f32mat4x2f32mat4x2;
typedef lowp_f32mat2x3f32mat2x3;
typedef lowp_f32mat3x3f32mat3x3;
typedef lowp_f32mat4x3f32mat4x3;
typedef lowp_f32mat2x4f32mat2x4;
typedef lowp_f32mat3x4f32mat3x4;
typedef lowp_f32mat4x4f32mat4x4;
typedef lowp_f32quatf32quat;
#elseif((GLM_PRECISION_MEDIUMP_FLOAT))
typedef mediump_f32vec1fvec1;
typedef mediump_f32vec2fvec2;
typedef mediump_f32vec3fvec3;
typedef mediump_f32vec4fvec4;
typedef mediump_f32mat2fmat2;
typedef mediump_f32mat3fmat3;
typedef mediump_f32mat4fmat4;
typedef mediump_f32mat2x2fmat2x2;
typedef mediump_f32mat3x2fmat3x2;
typedef mediump_f32mat4x2fmat4x2;
typedef mediump_f32mat2x3fmat2x3;
typedef mediump_f32mat3x3fmat3x3;
typedef mediump_f32mat4x3fmat4x3;
typedef mediump_f32mat2x4fmat2x4;
typedef mediump_f32mat3x4fmat3x4;
typedef mediump_f32mat4x4fmat4x4;
typedef mediump_f32quatfquat;

typedef mediump_f32vec1f32vec1;
typedef mediump_f32vec2f32vec2;
typedef mediump_f32vec3f32vec3;
typedef mediump_f32vec4f32vec4;
typedef mediump_f32mat2f32mat2;
typedef mediump_f32mat3f32mat3;
typedef mediump_f32mat4f32mat4;
typedef mediump_f32mat2x2f32mat2x2;
typedef mediump_f32mat3x2f32mat3x2;
typedef mediump_f32mat4x2f32mat4x2;
typedef mediump_f32mat2x3f32mat2x3;
typedef mediump_f32mat3x3f32mat3x3;
typedef mediump_f32mat4x3f32mat4x3;
typedef mediump_f32mat2x4f32mat2x4;
typedef mediump_f32mat3x4f32mat3x4;
typedef mediump_f32mat4x4f32mat4x4;
typedef mediump_f32quatf32quat;
#else//if((GLM_PRECISION_HIGHP_FLOAT))
/// Default single-precision floating-point vector of 1 components.
/// @see gtc_type_precision
typedef highp_f32vec1fvec1;

/// Default single-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef highp_f32vec2fvec2;

/// Default single-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef highp_f32vec3fvec3;

/// Default single-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef highp_f32vec4fvec4;

/// Default single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x2fmat2x2;

/// Default single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x3fmat2x3;

/// Default single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x4fmat2x4;

/// Default single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x2fmat3x2;

/// Default single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x3fmat3x3;

/// Default single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x4fmat3x4;

/// Default single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x2fmat4x2;

/// Default single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x3fmat4x3;

/// Default single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x4fmat4x4;

/// Default single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef fmat2x2fmat2;

/// Default single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef fmat3x3fmat3;

/// Default single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef fmat4x4fmat4;

/// Default single-precision floating-point quaternion.
/// @see gtc_type_precision
typedef highp_fquatfquat;



/// Default single-precision floating-point vector of 1 components.
/// @see gtc_type_precision
typedef highp_f32vec1f32vec1;

/// Default single-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef highp_f32vec2f32vec2;

/// Default single-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef highp_f32vec3f32vec3;

/// Default single-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef highp_f32vec4f32vec4;

/// Default single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x2f32mat2x2;

/// Default single-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x3f32mat2x3;

/// Default single-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat2x4f32mat2x4;

/// Default single-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x2f32mat3x2;

/// Default single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x3f32mat3x3;

/// Default single-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat3x4f32mat3x4;

/// Default single-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x2f32mat4x2;

/// Default single-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x3f32mat4x3;

/// Default single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef highp_f32mat4x4f32mat4x4;

/// Default single-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef f32mat2x2f32mat2;

/// Default single-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef f32mat3x3f32mat3;

/// Default single-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef f32mat4x4f32mat4;

/// Default single-precision floating-point quaternion.
/// @see gtc_type_precision
typedef highp_f32quatf32quat;
#endif

#if((GLM_PRECISION_LOWP_DOUBLE))
typedef lowp_f64vec1f64vec1;
typedef lowp_f64vec2f64vec2;
typedef lowp_f64vec3f64vec3;
typedef lowp_f64vec4f64vec4;
typedef lowp_f64mat2f64mat2;
typedef lowp_f64mat3f64mat3;
typedef lowp_f64mat4f64mat4;
typedef lowp_f64mat2x2f64mat2x2;
typedef lowp_f64mat3x2f64mat3x2;
typedef lowp_f64mat4x2f64mat4x2;
typedef lowp_f64mat2x3f64mat2x3;
typedef lowp_f64mat3x3f64mat3x3;
typedef lowp_f64mat4x3f64mat4x3;
typedef lowp_f64mat2x4f64mat2x4;
typedef lowp_f64mat3x4f64mat3x4;
typedef lowp_f64mat4x4f64mat4x4;
typedef lowp_f64quatf64quat;
#elseif((GLM_PRECISION_MEDIUMP_DOUBLE))
typedef mediump_f64vec1f64vec1;
typedef mediump_f64vec2f64vec2;
typedef mediump_f64vec3f64vec3;
typedef mediump_f64vec4f64vec4;
typedef mediump_f64mat2f64mat2;
typedef mediump_f64mat3f64mat3;
typedef mediump_f64mat4f64mat4;
typedef mediump_f64mat2x2f64mat2x2;
typedef mediump_f64mat3x2f64mat3x2;
typedef mediump_f64mat4x2f64mat4x2;
typedef mediump_f64mat2x3f64mat2x3;
typedef mediump_f64mat3x3f64mat3x3;
typedef mediump_f64mat4x3f64mat4x3;
typedef mediump_f64mat2x4f64mat2x4;
typedef mediump_f64mat3x4f64mat3x4;
typedef mediump_f64mat4x4f64mat4x4;
typedef mediump_f64quatf64quat;
#else
/// Default double-precision floating-point vector of 1 components.
/// @see gtc_type_precision
typedef highp_f64vec1f64vec1;

/// Default double-precision floating-point vector of 2 components.
/// @see gtc_type_precision
typedef highp_f64vec2f64vec2;

/// Default double-precision floating-point vector of 3 components.
/// @see gtc_type_precision
typedef highp_f64vec3f64vec3;

/// Default double-precision floating-point vector of 4 components.
/// @see gtc_type_precision
typedef highp_f64vec4f64vec4;

/// Default double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef highp_f64mat2x2f64mat2x2;

/// Default double-precision floating-point 2x3 matrix.
/// @see gtc_type_precision
typedef highp_f64mat2x3f64mat2x3;

/// Default double-precision floating-point 2x4 matrix.
/// @see gtc_type_precision
typedef highp_f64mat2x4f64mat2x4;

/// Default double-precision floating-point 3x2 matrix.
/// @see gtc_type_precision
typedef highp_f64mat3x2f64mat3x2;

/// Default double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef highp_f64mat3x3f64mat3x3;

/// Default double-precision floating-point 3x4 matrix.
/// @see gtc_type_precision
typedef highp_f64mat3x4f64mat3x4;

/// Default double-precision floating-point 4x2 matrix.
/// @see gtc_type_precision
typedef highp_f64mat4x2f64mat4x2;

/// Default double-precision floating-point 4x3 matrix.
/// @see gtc_type_precision
typedef highp_f64mat4x3f64mat4x3;

/// Default double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef highp_f64mat4x4f64mat4x4;

/// Default double-precision floating-point 2x2 matrix.
/// @see gtc_type_precision
typedef f64mat2x2f64mat2;

/// Default double-precision floating-point 3x3 matrix.
/// @see gtc_type_precision
typedef f64mat3x3f64mat3;

/// Default double-precision floating-point 4x4 matrix.
/// @see gtc_type_precision
typedef f64mat4x4f64mat4;

/// Default double-precision floating-point quaternion.
/// @see gtc_type_precision
typedef highp_f64quatf64quat;
#endif

}//namespace glm
