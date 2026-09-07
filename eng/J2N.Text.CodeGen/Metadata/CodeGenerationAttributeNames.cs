#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

namespace J2N.Text.CodeGen.Metadata
{
    internal static class CodeGenerationAttributeNames
    {
        public const string Constructor =
            "J2N.CodeGeneration.CodeGenerationConstructorAttribute";

        public const string Ignore =
            "J2N.CodeGeneration.CodeGenerationIgnoreAttribute";

        public const string ReturnsSelf =
            "J2N.CodeGeneration.CodeGenerationReturnsSelfAttribute";

        public const string SkipSynchronization =
            "J2N.CodeGeneration.CodeGenerationSkipSynchronizationAttribute";

        public const string ExtensionImplementation =
            "J2N.CodeGeneration.CodeGenerationExtensionImplementationAttribute";

        public const string GenerateForwarder =
            "J2N.CodeGeneration.CodeGenerationGenerateForwarderAttribute";
    }
}
