using NUnit.Framework;
using System.Text;

namespace J2N
{
    [SetUpFixture]
    public class Startup
    {
        /// <summary>
        /// This method is automatically called
        /// by NUnit one time before all tests run.
        /// </summary>
        [OneTimeSetUp]
        protected void OneTimeSetUpBeforeTests()
        {
#if FEATURE_ENCODINGPROVIDERS
            // Support for 8859-1 and IBM01047 encoding. See: https://docs.microsoft.com/en-us/dotnet/api/system.text.codepagesencodingprovider?view=netcore-2.0
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        /// <summary>
        /// This method is automatically called
        /// by NUnit one time after all tests run.
        /// </summary>
        [OneTimeTearDown]
        protected void OneTimeTearDownAfterTests()
        {

        }
    }
}
