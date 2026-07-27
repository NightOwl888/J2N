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

using Microsoft.CodeAnalysis;

namespace J2N.Text.CodeGen.Metadata
{
    public sealed class MethodModel
    {
        public required string Name { get; init; }

        public required string ReturnType { get; init; }

        public bool ReturnsSelf { get; init; }

        public bool IsBuilderMethod { get; init; }

        public bool IsStatic { get; init; }

        public Accessibility DeclaredAccessibility { get; init; }

        public bool IsExtensionMethod { get; init; }

        public bool IsConstructorProjection { get; set; }

        public bool IsUnsafe { get; init; }

        public string? BodyText { get; init; }

        public DocumentationModel? Documentation { get; init; }

        public List<ParameterModel> Parameters { get; init; } = [];

        public List<GenericParameterModel> GenericParameters { get; set; } = [];

        public List<AttributeModel> Attributes { get; init; } = [];

        public string? ConditionalCompilationSymbol { get; set; }

        public bool Ignore { get; init; }

        public bool SkipSynchronization { get; init; }

        public bool IsExtensionImplementation { get; init; }

        public bool GenerateForwarder { get; init; }

        public string? ForwardTarget { get; init; }

        public string? ForwardTargetObject { get; init; }
    }
}