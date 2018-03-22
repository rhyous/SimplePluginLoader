using System;
using System.IO;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Extensions
{
    public static class AppDomainExtensions
    {
        public static Assembly TryLoad(this AppDomain domain, byte[] rawAssembly, IPluginLoaderLogger logger)
        {
            try { return domain.Load(rawAssembly); }
            catch (Exception e)
            {
                logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                return null;
            }
        }

        public static Assembly TryLoad(this AppDomain domain, byte[] rawAssembly, byte[] rawSymbolStore, IPluginLoaderLogger logger)
        {
            try { return domain.Load(rawAssembly, rawSymbolStore); }
            catch(Exception e)
            {
                logger?.Write(PluginLoaderLogLevel.Debug, e.Message);
                return null;
            }
        }

        public static Assembly TryLoad(this AppDomain domain, string dll, IPluginLoaderLogger logger)
        {
            if (File.Exists(dll))
                return domain.TryLoad(File.ReadAllBytes(dll), logger);
            return null;
        }

        public static Assembly TryLoad(this AppDomain domain, string dll, string pdb, IPluginLoaderLogger logger)
        {
            if (File.Exists(dll))
            {
                return File.Exists(pdb)
                    ? domain.TryLoad(File.ReadAllBytes(dll), File.ReadAllBytes(pdb), logger) // Allow debugging
                    : domain.TryLoad(File.ReadAllBytes(dll), logger);
            }
            return null;
        }
    }
}


#region License
/*
Simple Plugin Loader - A library that makes loading plugins quick and easy. It
                       creates instances of interfaces or base classes from
                       plugins with a few lines of code.

Copyright (c) 2012, Jared Barneck (Rhyous)
All rights reserved.
 
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
 
1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
3. Use of the source code or binaries that in any way provides a competing
   solution, whether open source or commercial, is prohibited unless permission
   is specifically granted under a separate license by Jared Barneck (Rhyous).
4. Forking for personal or internal, or non-competing commercial use is 
   allowed. Distributing compiled releases as part of your non-competing 
   project is allowed.
5. Public copies, or forks, of source is allowed, but from such, public
   distribution of compiled releases is forbidden.
6. Source code enhancements or additions are the property of the author until
   the source code is contributed to this project. By contributing the source
   code to this project, the author immediately grants all rights to the
   contributed source code to Jared Barneck (Rhyous).
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion