#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using System;
using System.Globalization;


namespace J2N.Globalization
{
    /// <summary>
    /// Allows switching the current thread to a new culture in a using block that will automatically 
    /// return the culture to its previous state upon completion.
    /// <para/>
    /// <see cref="CultureContext"/> can be used to run arbitrary code within a specific culture without
    /// having to change APIs to pass a culture parameter.
    /// <para/>
    /// <code>
    /// using (var context = new CultureContext("fr-FR"))
    /// {
    ///     // Execute code in the french culture
    /// }
    /// </code>
    /// </summary>
    public sealed class CultureContext : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CultureInfo"/>
        /// based on the culture specified by the <paramref name="culture"/> identifier.
        /// </summary>
        /// <param name="culture">A predefined <see cref="CultureInfo"/> identifier, <see cref="CultureInfo.LCID"/> property
        /// of an existing <see cref="CultureInfo"/> object, or Windows-only culture identifier. This value will be applied
        /// to the <see cref="CultureInfo.CurrentCulture"/>.</param>
        public CultureContext(int culture)
            : this(new CultureInfo(culture), CultureInfo.CurrentUICulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CultureInfo"/>
        /// based on the culture specified by the <paramref name="culture"/> and <paramref name="uiCulture"/> identifiers.
        /// </summary>
        /// <param name="culture">A predefined <see cref="CultureInfo"/> identifier, <see cref="CultureInfo.LCID"/> property
        /// of an existing <see cref="CultureInfo"/> object, or Windows-only culture identifier. This value will be applied
        /// to the <see cref="CultureInfo.CurrentCulture"/>.</param>
        /// <param name="uiCulture">A predefined <see cref="CultureInfo"/> identifier, <see cref="CultureInfo.LCID"/> property
        /// of an existing <see cref="CultureInfo"/> object, or Windows-only culture identifier. This value will be applied
        /// to the <see cref="CultureInfo.CurrentUICulture"/>.</param>
        public CultureContext(int culture, int uiCulture)
            : this(new CultureInfo(culture), new CultureInfo(uiCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CultureInfo"/>
        /// based on the culture specified by the <paramref name="cultureName"/> identifier.
        /// </summary>
        /// <param name="cultureName">A predefined <see cref="CultureInfo"/> name, <see cref="CultureInfo.Name"/> of an
        /// existing <see cref="CultureInfo"/>, or Windows-only culture name. name is not case-sensitive. This value will be applied
        /// to the <see cref="CultureInfo.CurrentCulture"/>.
        /// </param>
        public CultureContext(string cultureName)
            : this(new CultureInfo(cultureName), CultureInfo.CurrentUICulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CultureInfo"/>
        /// based on the culture specified by the <paramref name="cultureName"/> identifier.
        /// </summary>
        /// <param name="cultureName">A predefined <see cref="CultureInfo"/> name, <see cref="CultureInfo.Name"/> of an
        /// existing <see cref="CultureInfo"/>, or Windows-only culture name. name is not case-sensitive. This value will be applied
        /// to the <see cref="CultureInfo.CurrentCulture"/>.
        /// </param>
        /// <param name="uiCultureName">A predefined <see cref="CultureInfo"/> name, <see cref="CultureInfo.Name"/> of an
        /// existing <see cref="CultureInfo"/>, or Windows-only culture name. name is not case-sensitive. This value will be applied
        /// to the <see cref="CultureInfo.CurrentUICulture"/>.</param>
        public CultureContext(string cultureName, string uiCultureName)
            : this(new CultureInfo(cultureName), new CultureInfo(uiCultureName))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CultureInfo"/>
        /// based on the <see cref="CultureInfo"/> specified by the <paramref name="culture"/> identifier.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> object. This value will be applied
        /// to the <see cref="CultureInfo.CurrentCulture"/>.
        /// </param>
        public CultureContext(CultureInfo culture)
            : this(culture, CultureInfo.CurrentUICulture)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CultureInfo"/>
        /// based on the <see cref="CultureInfo"/> specified by the <paramref name="culture"/> identifier.
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> object. This value will be applied
        /// to the <see cref="CultureInfo.CurrentCulture"/>.
        /// </param>
        /// <param name="uiCulture">A <see cref="CultureInfo"/> object. This value will be applied
        /// to the <see cref="CultureInfo.CurrentUICulture"/>.</param>
        public CultureContext(CultureInfo culture, CultureInfo uiCulture)
        {
            if (culture is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.culture);
            if (uiCulture is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.uiCulture);

            // Record the current culture settings so they can be restored later.
            this.originalCulture = CultureInfo.CurrentCulture;
            this.originalUICulture = CultureInfo.CurrentUICulture;

            // Set both the culture and UI culture for this context.
#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
            this.currentThread = System.Threading.Thread.CurrentThread;
            currentThread.CurrentCulture = culture;
            currentThread.CurrentUICulture = uiCulture;
#else
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
#endif
        }

#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
        private readonly System.Threading.Thread currentThread;
#endif
        private readonly CultureInfo originalCulture;
        private readonly CultureInfo originalUICulture;

        /// <summary>
        /// Gets the original <see cref="CultureInfo.CurrentCulture"/> value that existed on the current
        /// thread when this instance was initialized.
        /// </summary>
        public CultureInfo OriginalCulture => originalCulture;

        /// <summary>
        /// Gets the original <see cref="CultureInfo.CurrentUICulture"/> value that existed on the current
        /// thread when this instance was initialized.
        /// </summary>
        public CultureInfo OriginalUICulture => originalUICulture;

        /// <summary>
        /// Restores the <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/> to their
        /// original values, <see cref="OriginalCulture"/> and <see cref="OriginalUICulture"/>, respectively.
        /// </summary>
        public void RestoreOriginalCulture()
        {
            // Restore the culture to the way it was before the constructor was called.
#if !FEATURE_CULTUREINFO_CURRENTCULTURE_SETTER
            currentThread.CurrentCulture = originalCulture;
            currentThread.CurrentUICulture = originalUICulture;
#else
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
#endif
        }

        /// <summary>
        /// Restores the <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/> to their
        /// original values, <see cref="OriginalCulture"/> and <see cref="OriginalUICulture"/>, respectively.
        /// <para/>
        /// This can be called automatically with a using block to ensure the culture is reset even in the event of an exception.
        /// <code>
        /// using (var context = new CultureContext("fr-FR"))
        /// {
        ///     // Execute code in the french culture
        /// }
        /// </code>
        /// </summary>
        public void Dispose()
        {
            RestoreOriginalCulture();
        }
    }
}
