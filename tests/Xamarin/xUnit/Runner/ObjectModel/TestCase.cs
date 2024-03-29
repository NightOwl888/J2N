﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestPlatform.ObjectModel
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

    /// <summary>
    /// Stores information about a test case.
    /// </summary>
    [DataContract]
    public sealed class TestCase : TestObject
    {
        /// <summary>
        /// LocalExtensionData which can be used by Adapter developers for local transfer of extended properties.
        /// Note that this data is available only for in-Proc execution, and may not be available for OutProc executors
        /// </summary>
        private Object localExtensionData;

        private Guid defaultId = Guid.Empty;
        private Guid id;
        private string displayName;
        private string fullyQualifiedName;
        private string source;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <remarks>This constructor doesn't perform any parameter validation, it is meant to be used for serialization."/></remarks>
        public TestCase()
        {
            // Default constructor for Serialization.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCase"/> class.
        /// </summary>
        /// <param name="fullyQualifiedName">
        /// Fully qualified name of the test case.
        /// </param>
        /// <param name="executorUri">
        /// The Uri of the executor to use for running this test.
        /// </param>
        /// <param name="source">
        /// Test container source from which the test is discovered.
        /// </param>
        public TestCase(string fullyQualifiedName, Uri executorUri, string source)
        {
            if (string.IsNullOrEmpty(fullyQualifiedName))
                throw new ArgumentNullException(nameof(fullyQualifiedName));
            if (executorUri is null)
                throw new ArgumentNullException(nameof(executorUri));
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            this.FullyQualifiedName = fullyQualifiedName;
            this.ExecutorUri = executorUri;
            this.Source = source;
            this.LineNumber = -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// LocalExtensionData which can be used by Adapter developers for local transfer of extended properties.
        /// Note that this data is available only for in-Proc execution, and may not be available for OutProc executors
        /// </summary>
        public Object LocalExtensionData
        {
            get { return localExtensionData; }
            set { localExtensionData = value; }
        }

        /// <summary>
        /// Gets or sets the id of the test case.
        /// </summary>
        [DataMember]
        public Guid Id
        {
            get
            {
                if (this.id == Guid.Empty)
                {
                    if (this.defaultId == Guid.Empty)
                    {
                        this.defaultId = this.GetTestId();
                    }
                    return this.defaultId;
                }

                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the fully qualified name of the test case.
        /// </summary>
        [DataMember]
        public string FullyQualifiedName
        {
            get
            {
                return this.fullyQualifiedName;
            }
            set
            {
                this.fullyQualifiedName = value;

                // defaultId should be reset as it is based on FullyQualifiedName and Source.
                this.defaultId = Guid.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the display name of the test case.
        /// </summary>
        [DataMember]
        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(this.displayName) ? this.FullyQualifiedName : this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Uri of the Executor to use for running this test.
        /// </summary>
        [DataMember]
        public Uri ExecutorUri
        {
            get; set;
        }

        /// <summary>
        /// Gets the test container source from which the test is discovered.
        /// </summary>
        [DataMember]
        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;

                // defaultId should be reset as it is based on FullyQualifiedName and Source.
                this.defaultId = Guid.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the source code file path of the test.
        /// </summary>
        [DataMember]
        public string CodeFilePath
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the line number of the test.
        /// </summary>
        [DataMember]
        public int LineNumber
        {
            get; set;
        }

        /// <summary>
        /// Returns the TestProperties currently specified in this TestObject.
        /// </summary>
        public override IEnumerable<TestProperty> Properties
        {
            get
            {
                return TestCaseProperties.Properties.Concat(base.Properties);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.FullyQualifiedName;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Creates a Id of TestCase
        /// </summary>
        /// <returns>Guid test id</returns>
        private Guid GetTestId()
        {
            // To generate id hash "ExecutorUri + source + Name";

            // If source is a file name then just use the filename for the identifier since the
            // file might have moved between discovery and execution (in appx mode for example)
            // This is not elegant because the Source contents should be a black box to the framework.
            // For example in the database adapter case this is not a file path.
            string source = this.Source;

            // As discussed with team, we found no scenario for netcore, & fullclr where the Source is not present where ID is generated,
            // which means we would always use FileName to generate ID. In cases where somehow Source Path contained garbage character the API Path.GetFileName()
            // we are simply returning original input.
            // For UWP where source during discovery, & during execution can be on different machine, in such case we should always use Path.GetFileName()
            try
            {
                // If source name is malformed, GetFileName API will throw exception, so use same input malformed string to generate ID
                source = Path.GetFileName(source);
            }
            catch
            {
                // do nothing
            }

            string testcaseFullName = this.ExecutorUri + source + this.FullyQualifiedName;
            return EqtHash.GuidFromString(testcaseFullName);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Return TestProperty's value
        /// </summary>
        /// <returns></returns>
        protected override object ProtectedGetPropertyValue(TestProperty property, object defaultValue)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            switch (property.Id)
            {
                case "TestCase.CodeFilePath":
                    return this.CodeFilePath;
                case "TestCase.DisplayName":
                    return this.DisplayName;
                case "TestCase.ExecutorUri":
                    return this.ExecutorUri;
                case "TestCase.FullyQualifiedName":
                    return this.FullyQualifiedName;
                case "TestCase.Id":
                    return this.Id;
                case "TestCase.LineNumber":
                    return this.LineNumber;
                case "TestCase.Source":
                    return this.Source;
            }

            // It is a custom test case property. Should be retrieved from the TestObject store.
            return base.ProtectedGetPropertyValue(property, defaultValue);
        }

        /// <summary>
        /// Set TestProperty's value
        /// </summary>
        protected override void ProtectedSetPropertyValue(TestProperty property, object value)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            switch (property.Id)
            {
                case "TestCase.CodeFilePath":
                    this.CodeFilePath = (string)value; return;
                case "TestCase.DisplayName":
                    this.DisplayName = (string)value; return;
                case "TestCase.ExecutorUri":
                    this.ExecutorUri = value as Uri ?? new Uri((string)value); return;
                case "TestCase.FullyQualifiedName":
                    this.FullyQualifiedName = (string)value; return;
                case "TestCase.Id":
                    this.Id = value is Guid guid ? guid : Guid.Parse((string)value); return;
                case "TestCase.LineNumber":
                    this.LineNumber = (int)value; return;
                case "TestCase.Source":
                    this.Source = (string)value; return;
            }

            // It is a custom test case property. Should be set in the TestObject store.
            base.ProtectedSetPropertyValue(property, value);
        }

        #endregion
    }

    /// <summary>
    /// Well-known TestCase properties
    /// </summary>
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA2104 doesn't fire on all target frameworks")]
    public static class TestCaseProperties
    {
        #region Private Constants

        /// <summary>
        /// These are the core Test properties and may be available in commandline/TeamBuild to filter tests.
        /// These Property names should not be localized.
        /// </summary>
        private const string IdLabel = "Id";
        private const string FullyQualifiedNameLabel = "FullyQualifiedName";
        private const string NameLabel = "Name";
        private const string ExecutorUriLabel = "Executor Uri";
        private const string SourceLabel = "Source";
        private const string FilePathLabel = "File Path";
        private const string LineNumberLabel = "Line Number";

        #endregion

#pragma warning disable CA2104 // DoNotDeclareReadOnlyMutableReferenceTypes
        public static readonly TestProperty Id = TestProperty.Register("TestCase.Id", IdLabel, string.Empty, string.Empty, typeof(Guid), ValidateGuid, TestPropertyAttributes.Hidden, typeof(TestCase));

        public static readonly TestProperty FullyQualifiedName = TestProperty.Register("TestCase.FullyQualifiedName", FullyQualifiedNameLabel, string.Empty, string.Empty, typeof(string), ValidateName, TestPropertyAttributes.Hidden, typeof(TestCase));

        public static readonly TestProperty DisplayName = TestProperty.Register("TestCase.DisplayName", NameLabel, string.Empty, string.Empty, typeof(string), ValidateDisplay, TestPropertyAttributes.None, typeof(TestCase));

        public static readonly TestProperty ExecutorUri = TestProperty.Register("TestCase.ExecutorUri", ExecutorUriLabel, string.Empty, string.Empty, typeof(Uri), ValidateExecutorUri, TestPropertyAttributes.Hidden, typeof(TestCase));

        public static readonly TestProperty Source = TestProperty.Register("TestCase.Source", SourceLabel, typeof(string), typeof(TestCase));

        public static readonly TestProperty CodeFilePath = TestProperty.Register("TestCase.CodeFilePath", FilePathLabel, typeof(string), typeof(TestCase));

        public static readonly TestProperty LineNumber = TestProperty.Register("TestCase.LineNumber", LineNumberLabel, typeof(int), TestPropertyAttributes.Hidden, typeof(TestCase));
#pragma warning restore CA2104 // DoNotDeclareReadOnlyMutableReferenceTypes
        internal static TestProperty[] Properties { get; } =
        {
            CodeFilePath,
            DisplayName,
            ExecutorUri,
            FullyQualifiedName,
            Id,
            LineNumber,
            Source
        };

        private static bool ValidateName(object value)
        {
            return !string.IsNullOrWhiteSpace((string)value);
        }

        private static bool ValidateDisplay(object value)
        {
            // only check for null and pass the rest up to UI for validation
            return value != null;
        }

        private static bool ValidateExecutorUri(object value)
        {
            return value != null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "Required to validate the input value.")]
        private static bool ValidateGuid(object value)
        {
            try
            {
                new Guid(value.ToString());
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }
}
