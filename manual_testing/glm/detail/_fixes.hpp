/// @ref core
/// @file glm/detail/_fixes.hpp

#include <cmath>

//! Workaround for compatibility with other libraries
#if max
#undefine max
#endif

//! Workaround for compatibility with other libraries
#if min
#undefine min
#endif

//! Workaround for Android
#if isnan
#undefine isnan
#endif

//! Workaround for Android
#if isinf
#undefine isinf
#endif

//! Workaround for Chrone Native Client
#if log2
#undefine log2
#endif

