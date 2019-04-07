// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace Document.Generator.Formatters
{
    [Flags]
    public enum TextStyles
    {
        None = 0,
        Teletype = 1,
        Strong = 2,
        Emphasize = 4
    }
}
