// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging.Core;

namespace NuGet.ProjectModel
{
    public static class LockFileUtilities
    {
        public static LockFile GetLockFile(string lockFilePath)
        {
            LockFile lockFile = null;

            if (File.Exists(lockFilePath))
            {
                var lockFileFormat = new LockFileFormat();
                lockFile = lockFileFormat.Read(lockFilePath);
            }

            return lockFile;

        }

        public static LockFile GetLockFile(string lockFilePath, Common.ILogger logger)
        {
            LockFile lockFile = null;

            if (File.Exists(lockFilePath))
            {
                var format = new LockFileFormat();

                // A corrupt lock file will log errors and return null
                lockFile = FileUtility.SafeRead(filePath: lockFilePath,read: (stream, path) => format.Read(stream, logger, path));
            }

            return lockFile;
        }


        private static LockFileTargetLibrary GetTargetLibrary(string name, LockFile lockFile, NuGetFramework framework)
        {
            return lockFile.GetTarget(framework, null).
                Libraries.Where(l => String.Compare(l.Name, name, true) == 0).
                SingleOrDefault();
        }

        public static bool HasCyclicDependency(string lockFilePath)
        {   /*
            Write logic to detect cyclic dependencies in a lock file
            */
            
            return false;
        }
    }
}
