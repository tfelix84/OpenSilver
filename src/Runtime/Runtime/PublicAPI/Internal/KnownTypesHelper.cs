﻿
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;

namespace CSHTML5.Internal
{
    public static class KnownTypesHelper
    {
        private static readonly HashSet<Type> _additionalKnownTypes = new();

        public static void AddKnownType(Type knownType) => _additionalKnownTypes.Add(knownType);

        internal static IEnumerable<Type> KnownTypes => _additionalKnownTypes;
    }
}
