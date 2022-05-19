// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestPlatform.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Class that holds collection of traits
    /// </summary>
#if NET451
    [Serializable]
#endif
    public class TraitCollection : IEnumerable<Trait>
    {
        internal const string TraitPropertyId = "TestObject.Traits";
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA2104 doesn't fire on all target frameworks")]
        private static readonly TestProperty TraitsProperty = TestProperty.Register(
            TraitPropertyId,
#if !NET451
            // TODO: Fix this with proper resourcing for UWP and Win 8.1 Apps
            // Trying to access resources will throw "MissingManifestResourceException" percolated as "TypeInitialization" exception
            "Traits",
#else
            Resources.Resources.TestCasePropertyTraitsLabel,
#endif
            typeof(KeyValuePair<string, string>[]),
#pragma warning disable 618
            TestPropertyAttributes.Hidden | TestPropertyAttributes.Trait,
#pragma warning restore 618
            typeof(TestObject));

#if NET451
        [NonSerialized]
#endif
        private readonly TestObject testObject;

        internal TraitCollection(TestObject testObject)
        {
            this.testObject = testObject ?? throw new ArgumentNullException(nameof(testObject));
        }

        public void Add(Trait trait)
        {
            if (trait is null)
                throw new ArgumentNullException(nameof(trait));

            this.AddRange(new[] { trait });
        }

        public void Add(string name, string value)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            this.Add(new Trait(name, value));
        }

        public void AddRange(IEnumerable<Trait> traits)
        {
            if (traits is null)
                throw new ArgumentNullException(nameof(traits));

            var existingTraits = this.GetTraits();
            this.Add(existingTraits, traits);
        }

        public IEnumerator<Trait> GetEnumerator()
        {
            var enumerable = this.GetTraits();
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<Trait> GetTraits()
        {
            if (!this.testObject.Properties.Contains(TraitsProperty))
            {
                yield break;
            }

            var traits = this.testObject.GetPropertyValue(TraitsProperty, Enumerable.Empty<KeyValuePair<string, string>>().ToArray());

            foreach (var trait in traits)
            {
                yield return new Trait(trait);
            }
        }

        private void Add(IEnumerable<Trait> traits, IEnumerable<Trait> newTraits)
        {
            var newValue = traits.Union(newTraits);
            var newPairs = newValue.Select(t => new KeyValuePair<string, string>(t.Name, t.Value)).ToArray();
            this.testObject.SetPropertyValue<KeyValuePair<string, string>[]>(TraitsProperty, newPairs);
        }
    }
}
