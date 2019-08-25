/// @ref gtc_matrix_integer
/// @file glm/gtc/matrix_integer.hpp
///
/// @see core (dependence)
///
/// @defgroup gtc_matrix_integer GLM_GTC_matrix_integer
/// @ingroup gtc
///
/// Defines a number of matrices with integer types.
/// <glm/gtc/matrix_integer.hpp> need to be included to use these functionalities.

#pragma once

// Dependency:
#include "../mat2x2.hpp"
#include "../mat2x3.hpp"
#include "../mat2x4.hpp"
#include "../mat3x2.hpp"
#include "../mat3x3.hpp"
#include "../mat3x4.hpp"
#include "../mat4x2.hpp"
#include "../mat4x3.hpp"
#include "../mat4x4.hpp"

#if GLM_MESSAGES || GLM_MESSAGES_ENABLED && (!GLM_EXT_INCLUDED)
#pragma message("GLM: GLM_GTC_matrix_integer extension included")
#endif

namespace glm
{
/// @addtogroup gtc_matrix_integer
/// @{

/// High-precision signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<int, highp>highp_imat2;

/// High-precision signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<int, highp>highp_imat3;

/// High-precision signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<int, highp>highp_imat4;

/// High-precision signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<int, highp>highp_imat2x2;

/// High-precision signed integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef tmat2x3<int, highp>highp_imat2x3;

/// High-precision signed integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef tmat2x4<int, highp>highp_imat2x4;

/// High-precision signed integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef tmat3x2<int, highp>highp_imat3x2;

/// High-precision signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<int, highp>highp_imat3x3;

/// High-precision signed integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef tmat3x4<int, highp>highp_imat3x4;

/// High-precision signed integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef tmat4x2<int, highp>highp_imat4x2;

/// High-precision signed integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef tmat4x3<int, highp>highp_imat4x3;

/// High-precision signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<int, highp>highp_imat4x4;


/// Medium-precision signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<int, mediump>mediump_imat2;

/// Medium-precision signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<int, mediump>mediump_imat3;

/// Medium-precision signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<int, mediump>mediump_imat4;


/// Medium-precision signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<int, mediump>mediump_imat2x2;

/// Medium-precision signed integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef tmat2x3<int, mediump>mediump_imat2x3;

/// Medium-precision signed integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef tmat2x4<int, mediump>mediump_imat2x4;

/// Medium-precision signed integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef tmat3x2<int, mediump>mediump_imat3x2;

/// Medium-precision signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<int, mediump>mediump_imat3x3;

/// Medium-precision signed integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef tmat3x4<int, mediump>mediump_imat3x4;

/// Medium-precision signed integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef tmat4x2<int, mediump>mediump_imat4x2;

/// Medium-precision signed integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef tmat4x3<int, mediump>mediump_imat4x3;

/// Medium-precision signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<int, mediump>mediump_imat4x4;


/// Low-precision signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<int, lowp>lowp_imat2;

/// Low-precision signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<int, lowp>lowp_imat3;

/// Low-precision signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<int, lowp>lowp_imat4;


/// Low-precision signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<int, lowp>lowp_imat2x2;

/// Low-precision signed integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef tmat2x3<int, lowp>lowp_imat2x3;

/// Low-precision signed integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef tmat2x4<int, lowp>lowp_imat2x4;

/// Low-precision signed integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef tmat3x2<int, lowp>lowp_imat3x2;

/// Low-precision signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<int, lowp>lowp_imat3x3;

/// Low-precision signed integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef tmat3x4<int, lowp>lowp_imat3x4;

/// Low-precision signed integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef tmat4x2<int, lowp>lowp_imat4x2;

/// Low-precision signed integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef tmat4x3<int, lowp>lowp_imat4x3;

/// Low-precision signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<int, lowp>lowp_imat4x4;


/// High-precision unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<uint, highp>highp_umat2;

/// High-precision unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<uint, highp>highp_umat3;

/// High-precision unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<uint, highp>highp_umat4;

/// High-precision unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<uint, highp>highp_umat2x2;

/// High-precision unsigned integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef tmat2x3<uint, highp>highp_umat2x3;

/// High-precision unsigned integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef tmat2x4<uint, highp>highp_umat2x4;

/// High-precision unsigned integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef tmat3x2<uint, highp>highp_umat3x2;

/// High-precision unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<uint, highp>highp_umat3x3;

/// High-precision unsigned integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef tmat3x4<uint, highp>highp_umat3x4;

/// High-precision unsigned integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef tmat4x2<uint, highp>highp_umat4x2;

/// High-precision unsigned integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef tmat4x3<uint, highp>highp_umat4x3;

/// High-precision unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<uint, highp>highp_umat4x4;


/// Medium-precision unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<uint, mediump>mediump_umat2;

/// Medium-precision unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<uint, mediump>mediump_umat3;

/// Medium-precision unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<uint, mediump>mediump_umat4;


/// Medium-precision unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<uint, mediump>mediump_umat2x2;

/// Medium-precision unsigned integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef tmat2x3<uint, mediump>mediump_umat2x3;

/// Medium-precision unsigned integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef tmat2x4<uint, mediump>mediump_umat2x4;

/// Medium-precision unsigned integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef tmat3x2<uint, mediump>mediump_umat3x2;

/// Medium-precision unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<uint, mediump>mediump_umat3x3;

/// Medium-precision unsigned integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef tmat3x4<uint, mediump>mediump_umat3x4;

/// Medium-precision unsigned integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef tmat4x2<uint, mediump>mediump_umat4x2;

/// Medium-precision unsigned integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef tmat4x3<uint, mediump>mediump_umat4x3;

/// Medium-precision unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<uint, mediump>mediump_umat4x4;


/// Low-precision unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<uint, lowp>lowp_umat2;

/// Low-precision unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<uint, lowp>lowp_umat3;

/// Low-precision unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<uint, lowp>lowp_umat4;


/// Low-precision unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef tmat2x2<uint, lowp>lowp_umat2x2;

/// Low-precision unsigned integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef tmat2x3<uint, lowp>lowp_umat2x3;

/// Low-precision unsigned integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef tmat2x4<uint, lowp>lowp_umat2x4;

/// Low-precision unsigned integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef tmat3x2<uint, lowp>lowp_umat3x2;

/// Low-precision unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef tmat3x3<uint, lowp>lowp_umat3x3;

/// Low-precision unsigned integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef tmat3x4<uint, lowp>lowp_umat3x4;

/// Low-precision unsigned integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef tmat4x2<uint, lowp>lowp_umat4x2;

/// Low-precision unsigned integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef tmat4x3<uint, lowp>lowp_umat4x3;

/// Low-precision unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef tmat4x4<uint, lowp>lowp_umat4x4;

#if((GLM_PRECISION_HIGHP_INT))
typedef highp_imat2imat2;
typedef highp_imat3imat3;
typedef highp_imat4imat4;
typedef highp_imat2x2imat2x2;
typedef highp_imat2x3imat2x3;
typedef highp_imat2x4imat2x4;
typedef highp_imat3x2imat3x2;
typedef highp_imat3x3imat3x3;
typedef highp_imat3x4imat3x4;
typedef highp_imat4x2imat4x2;
typedef highp_imat4x3imat4x3;
typedef highp_imat4x4imat4x4;
#elseif((GLM_PRECISION_LOWP_INT))
typedef lowp_imat2imat2;
typedef lowp_imat3imat3;
typedef lowp_imat4imat4;
typedef lowp_imat2x2imat2x2;
typedef lowp_imat2x3imat2x3;
typedef lowp_imat2x4imat2x4;
typedef lowp_imat3x2imat3x2;
typedef lowp_imat3x3imat3x3;
typedef lowp_imat3x4imat3x4;
typedef lowp_imat4x2imat4x2;
typedef lowp_imat4x3imat4x3;
typedef lowp_imat4x4imat4x4;
#else //if((GLM_PRECISION_MEDIUMP_INT))

/// Signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat2imat2;

/// Signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat3imat3;

/// Signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat4imat4;

/// Signed integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat2x2imat2x2;

/// Signed integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat2x3imat2x3;

/// Signed integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat2x4imat2x4;

/// Signed integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat3x2imat3x2;

/// Signed integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat3x3imat3x3;

/// Signed integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat3x4imat3x4;

/// Signed integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat4x2imat4x2;

/// Signed integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat4x3imat4x3;

/// Signed integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_imat4x4imat4x4;
#endif//GLM_PRECISION

#if((GLM_PRECISION_HIGHP_UINT))
typedef highp_umat2umat2;
typedef highp_umat3umat3;
typedef highp_umat4umat4;
typedef highp_umat2x2umat2x2;
typedef highp_umat2x3umat2x3;
typedef highp_umat2x4umat2x4;
typedef highp_umat3x2umat3x2;
typedef highp_umat3x3umat3x3;
typedef highp_umat3x4umat3x4;
typedef highp_umat4x2umat4x2;
typedef highp_umat4x3umat4x3;
typedef highp_umat4x4umat4x4;
#elseif((GLM_PRECISION_LOWP_UINT))
typedef lowp_umat2umat2;
typedef lowp_umat3umat3;
typedef lowp_umat4umat4;
typedef lowp_umat2x2umat2x2;
typedef lowp_umat2x3umat2x3;
typedef lowp_umat2x4umat2x4;
typedef lowp_umat3x2umat3x2;
typedef lowp_umat3x3umat3x3;
typedef lowp_umat3x4umat3x4;
typedef lowp_umat4x2umat4x2;
typedef lowp_umat4x3umat4x3;
typedef lowp_umat4x4umat4x4;
#else //if((GLM_PRECISION_MEDIUMP_UINT))

/// Unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat2umat2;

/// Unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat3umat3;

/// Unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat4umat4;

/// Unsigned integer 2x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat2x2umat2x2;

/// Unsigned integer 2x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat2x3umat2x3;

/// Unsigned integer 2x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat2x4umat2x4;

/// Unsigned integer 3x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat3x2umat3x2;

/// Unsigned integer 3x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat3x3umat3x3;

/// Unsigned integer 3x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat3x4umat3x4;

/// Unsigned integer 4x2 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat4x2umat4x2;

/// Unsigned integer 4x3 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat4x3umat4x3;

/// Unsigned integer 4x4 matrix.
/// @see gtc_matrix_integer
typedef mediump_umat4x4umat4x4;
#endif//GLM_PRECISION

/// @}
}//namespace glm
