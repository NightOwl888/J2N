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

using J2N.Text.CodeGen.Metadata;
using Microsoft.CodeAnalysis;

namespace J2N.Text.CodeGen.Projection
{
    public sealed class BuilderProjection
    {
        public ProjectedTypeModel Project(
            TypeModel source,
            string facadeNamespace,
            string facadeName,
            ProjectionOptions? options = null)
        {
            options ??= new ProjectionOptions();

            var projected = new ProjectedTypeModel
            {
                Source = source,
                Namespace = facadeNamespace,
                Name = facadeName
            };

            foreach (MethodModel method in source.Methods.Where(x => !x.Ignore))
            {
                if (method.DeclaredAccessibility != Accessibility.Public)
                    continue;

                projected.Methods.Add(
                    ProjectMethod(
                        method,
                        source,
                        source.SourceType,
                        facadeName,
                        options));
            }

            foreach (PropertyModel property in source.Properties.Where(x => !x.Ignore))
            {
                projected.Properties.Add(
                    ProjectProperty(
                        property,
                        source,
                        source.SourceType,
                        facadeName,
                        options));
            }

            return projected;
        }

        private static MethodModel ProjectMethod(
            MethodModel method,
            TypeModel source,
            string sourceType,
            string facadeName,
            ProjectionOptions options)
        {
            var projected = new MethodModel
            {
                Name = method.Name,

                DeclaredAccessibility = method.DeclaredAccessibility,

                ReturnType =
                    method.ReturnsSelf
                        ? facadeName
                        : RewriteType(
                            method.ReturnType,
                            sourceType,
                            facadeName),

                ReturnsSelf = method.ReturnsSelf,

                //
                // Builder methods are ONLY methods explicitly marked
                // with [CodeGenerationReturnsSelf]
                //
                IsBuilderMethod = method.ReturnsSelf,

                IsUnsafe = method.IsUnsafe,
                IsStatic = method.IsStatic,
                IsExtensionMethod = method.IsExtensionMethod,

                Parameters =
                    method.Parameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,

                            TypeName =
                                RewriteType(
                                    p.TypeName,
                                    sourceType,
                                    facadeName),

                            SourceTypeName = p.SourceTypeName,

                            Modifier = p.Modifier,
                            IsThis = p.IsThis,
                            DefaultValueExpression = p.DefaultValueExpression,

                            Attributes =
                                p.Attributes
                                    .Select(CloneAttribute)
                                    .ToList()
                        })
                        .ToList(),

                GenericParameters =
                    method.GenericParameters
                        .Select(CloneGenericParameter)
                        .ToList(),

                Attributes =
                    method.Attributes
                        .Select(CloneAttribute)
                        .ToList(),

                Documentation =
                    DocumentationRewriter.RewriteDocumentation(
                        method.Documentation,
                        source,
                        source.Name,
                        facadeName,
                        new DocumentationRewriteOptions
                        {
                            IncludeSynchronizationNote = options.EmitSynchronizationNotes && method.SkipSynchronization,
                            ForceBuilderReturns = method.ReturnsSelf
                        }),

                //
                // NEW:
                //
                ConditionalCompilationSymbol =
                    method.Parameters.Any(p =>
                        p.TypeName is "Index" or "Range")
                            ? "FEATURE_INDEX_RANGE"
                            : null,

                SkipSynchronization =
                    method.SkipSynchronization,
            };

            EnsureAggressiveInlining(projected.Attributes);

            return projected;
        }

        private static PropertyModel ProjectProperty(
            PropertyModel property,
            TypeModel source,
            string sourceType,
            string facadeName,
            ProjectionOptions options)
        {
            var projected = new PropertyModel
            {
                Name = property.Name,

                TypeName =
                    RewriteType(
                        property.TypeName,
                        sourceType,
                        facadeName),

                HasGetter = property.HasGetter,
                HasSetter = property.HasSetter,
                IsIndexer = property.IsIndexer,
                IsUnsafe = property.IsUnsafe,
                IsStatic = property.IsStatic,

                IndexParameters =
                    property.IndexParameters
                        .Select(p => new ParameterModel
                        {
                            Name = p.Name,

                            TypeName =
                                RewriteType(
                                    p.TypeName,
                                    sourceType,
                                    facadeName),

                            SourceTypeName = p.SourceTypeName,

                            Modifier = p.Modifier,
                            IsThis = p.IsThis,
                            DefaultValueExpression = p.DefaultValueExpression,

                            Attributes =
                                p.Attributes
                                    .Select(CloneAttribute)
                                    .ToList()
                        })
                        .ToList(),

                Attributes =
                    property.Attributes
                        .Select(CloneAttribute)
                        .ToList(),

                GetterAttributes =
                    property.GetterAttributes
                        .Select(CloneAttribute)
                        .ToList(),

                SetterAttributes =
                    property.SetterAttributes
                        .Select(CloneAttribute)
                        .ToList(),

                Documentation =
                    DocumentationRewriter.RewriteDocumentation(
                        property.Documentation,
                        source,
                        source.Name,
                        facadeName,
                        new DocumentationRewriteOptions
                        {
                            IncludeSynchronizationNote = options.EmitSynchronizationNotes && (property.SkipGetterSynchronization || property.SkipSetterSynchronization),
                            ForceBuilderReturns = false
                        }),

                SkipGetterSynchronization =
                    property.SkipGetterSynchronization,

                SkipSetterSynchronization =
                    property.SkipSetterSynchronization,
            };

            EnsureAggressiveInlining(projected.GetterAttributes);
            EnsureAggressiveInlining(projected.SetterAttributes);

            return projected;
        }

        private static string RewriteType(
            string typeName,
            string sourceType,
            string facadeName)
        {
            return typeName.Replace(sourceType, facadeName);
        }

        private static AttributeModel CloneAttribute(
            AttributeModel attribute)
        {
            return new AttributeModel
            {
                Name = attribute.Name,
                Arguments = attribute.Arguments.ToList()
            };
        }

        private static GenericParameterModel CloneGenericParameter(
            GenericParameterModel parameter)
        {
            return new GenericParameterModel
            {
                Name = parameter.Name,
                Constraints = parameter.Constraints.ToList()
            };
        }

        private static void EnsureAggressiveInlining(ICollection<AttributeModel> attributes)
        {
            if (attributes.Any(IsAggressiveInlining))
                return;

            attributes.Add(new AttributeModel
            {
                Name = "MethodImpl",
                Arguments =
                {
                    "MethodImplOptions.AggressiveInlining"
                }
            });
        }

        private static bool IsAggressiveInlining(AttributeModel attribute)
        {
            if (!string.Equals(attribute.Name, "MethodImpl", StringComparison.Ordinal))
                return false;

            return attribute.Arguments.Any(a =>
                a.Contains("MethodImplOptions.AggressiveInlining", StringComparison.Ordinal));
        }
    }
}
