// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

namespace Document.Generator.Helpers
{
    public interface ICRefResolver
    {
        string NameOf(string cref);
        string UrlOf(string cref);
    }
}
