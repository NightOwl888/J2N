using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static J2N.Numerics.DotNetNumber;

namespace J2N.Numerics
{
    public class TestDouble : TestCase
    {
        private static readonly CultureInfo Locale_US = new CultureInfo("en-US");

        private static readonly long[] rawBitsFor3_4en324ToN1 = { 0x1L, 0x7L, 0x45L, 0x2b0L, 0x1ae2L,
            0x10cd1L, 0xa8028L, 0x69018dL, 0x41a0f7eL, 0x29049aedL, 0x19a2e0d44L,
            0x1005cc84acL, 0xa039fd2ebdL, 0x64243e3d361L, 0x3e96a6e641c6L, 0x271e284fe91b8L,
            0x1872d931f1b131L, 0x4e8f8f7e6e1d7dL, 0x8319b9af04d26eL, 0xb7e0281ac6070aL,
            0xedd832217788ccL, 0x122a71f54eab580L, 0x15750e72a2562e0L, 0x18d2520f4aebb98L,
            0x1c2373498ed353fL, 0x1f6c501bf28828eL, 0x22c76422ef2a332L, 0x261c9e95d57a5ffL,
            0x2963c63b4ad8f7fL, 0x2cbcb7ca1d8f35fL, 0x3015f2de527981bL, 0x335b6f95e717e22L,
            0x36b24b7b60dddabL, 0x3a0f6f2d1c8aa8bL, 0x3d534af863ad52dL, 0x40a81db67c98a79L,
            0x440912920ddf68bL, 0x474b5736915742eL, 0x4a9e2d0435ad13aL, 0x4e02dc22a18c2c4L,
            0x5143932b49ef375L, 0x549477f61c6b052L, 0x57f995f3a385c67L, 0x5b3bfdb846339c0L,
            0x5e8afd2657c0830L, 0x61edbc6fedb0a3dL, 0x653495c5f48e666L, 0x6881bb3771b1fffL,
            0x6be22a054e1e7ffL, 0x6f2d5a4350d30ffL, 0x7278b0d42507d3fL, 0x75d6dd092e49c8fL,
            0x79264a25bcee1daL, 0x7c6fdcaf2c29a50L, 0x7fcbd3daf7340e4L, 0x831f6468da8088eL,
            0x86673d831120ab2L, 0x89c10ce3d568d5fL, 0x8d18a80e656185bL, 0x905ed211feb9e72L,
            0x93b686967e6860eL, 0x9712141e0f013c9L, 0x9a56992592c18bbL, 0x9dac3f6ef771eeaL,
            0xa10ba7a55aa7352L, 0xa44e918eb151027L, 0xa7a235f25da5430L, 0xab0561b77a8749eL,
            0xae46ba2559291c6L, 0xb19868aeaf73637L, 0xb4fe82da5b503c5L, 0xb83f11c8791225bL,
            0xbb8ed63a9756af2L, 0xbef28bc93d2c5afL, 0xc237975dc63bb8dL, 0xc5857d3537caa70L,
            0xc8e6dc8285bd50cL, 0xcc3049d19396528L, 0xcf7c5c45f87be72L, 0xd2db7357769ae0eL,
            0xd6292816aa20cc9L, 0xd973721c54a8ffbL, 0xdcd04ea369d33faL, 0xe0223126222407cL,
            0xe36abd6faaad09bL, 0xe6c56ccb95584c2L, 0xea1b63ff3d572f9L, 0xed623cff0cacfb8L,
            0xf0bacc3ecfd83a5L, 0xf414bfa741e7247L, 0xf759ef911260ed9L, 0xfab06b7556f9290L,
            0xfe0e4329565bb9aL, 0x10151d3f3abf2a80L, 0x104a648f096ef520L, 0x10807ed965e55934L,
            0x10b49e8fbf5eaf81L, 0x10e9c633af365b61L, 0x11201be04d81f91dL, 0x115422d860e27764L,
            0x11892b8e791b153dL, 0x11bf76721761da8cL, 0x11f3aa074e9d2898L, 0x12289489224472beL,
            0x125eb9ab6ad58f6dL, 0x1293340b22c579a4L, 0x12c8010deb76d80dL, 0x12fe015166548e11L,
            0x1332c0d2dff4d8caL, 0x1367710797f20efdL, 0x139d4d497dee92bcL, 0x13d2504deeb51bb6L,
            0x1406e4616a6262a3L, 0x143c9d79c4fafb4cL, 0x1471e26c1b1cdd0fL, 0x14a65b0721e41453L,
            0x14dbf1c8ea5d1968L, 0x1511771d927a2fe1L, 0x1545d4e4f718bbd9L, 0x157b4a1e34deead0L,
            0x15b10e52e10b52c2L, 0x15e551e7994e2772L, 0x161aa6617fa1b14fL, 0x1650a7fcefc50ed1L,
            0x1684d1fc2bb65286L, 0x16ba067b36a3e727L, 0x16f0440d02267078L, 0x1724551042b00c96L,
            0x17596a54535c0fbcL, 0x178fc4e9683313abL, 0x17c3db11e11fec4bL, 0x17f8d1d65967e75eL,
            0x182f064befc1e135L, 0x186363ef75d92cc1L, 0x18983ceb534f77f1L, 0x18ce4c26282355eeL,
            0x1902ef97d91615b5L, 0x1937ab7dcf5b9b22L, 0x196d965d433281eaL, 0x19a27dfa49ff9132L,
            0x19d71d78dc7f757fL, 0x1a0ce4d7139f52dfL, 0x1a420f066c4393cbL, 0x1a7692c8075478beL,
            0x1aac377a092996edL, 0x1ae1a2ac45b9fe54L, 0x1b160b5757287de9L, 0x1b4b8e2d2cf29d64L,
            0x1b8138dc3c17a25eL, 0x1bb587134b1d8af6L, 0x1beae8d81de4edb4L, 0x1c20d18712af1490L,
            0x1c5505e8d75ad9b4L, 0x1c8a47630d319021L, 0x1cc06c9de83efa15L, 0x1cf487c5624eb89aL,
            0x1d29a9b6bae266c1L, 0x1d600a1234cd8038L, 0x1d940c96c200e046L, 0x1dc90fbc72811858L,
            0x1dff53ab8f215e6eL, 0x1e33944b3974db05L, 0x1e68795e07d211c6L, 0x1e9e97b589c69637L,
            0x1ed31ed1761c1de3L, 0x1f07e685d3a3255bL, 0x1f3de027488beeb2L, 0x1f72ac188d57752fL,
            0x1fa7571eb0ad527bL, 0x1fdd2ce65cd8a71aL, 0x20123c0ffa076870L, 0x2046cb13f889428cL,
            0x207c7dd8f6ab932fL, 0x20b1cea79a2b3bfeL, 0x20e6425180b60afdL, 0x211bd2e5e0e38dbcL,
            0x215163cfac8e3896L, 0x2185bcc397b1c6bbL, 0x21bb2bf47d9e386aL, 0x21f0fb78ce82e342L,
            0x22253a5702239c13L, 0x225a88ecc2ac8317L, 0x22909593f9abd1efL, 0x22c4baf8f816c66aL,
            0x22f9e9b7361c7805L, 0x2330321281d1cb03L, 0x23643e9722463dc4L, 0x23994e3cead7cd35L,
            0x23cfa1cc258dc082L, 0x2403c51f97789851L, 0x2438b6677d56be65L, 0x246ee4015cac6dffL,
            0x24a34e80d9ebc4bfL, 0x24d822211066b5efL, 0x250e2aa95480636bL, 0x2542daa9d4d03e23L,
            0x257791544a044dabL, 0x25ad75a95c856116L, 0x25e26989d9d35caeL, 0x261703ec504833d9L,
            0x264cc4e7645a40d0L, 0x2681fb109eb86882L, 0x26b679d4c66682a2L, 0x26ec1849f800234bL,
            0x27218f2e3b00160fL, 0x2755f2f9c9c01b93L, 0x278b6fb83c302277L, 0x27c125d3259e158bL,
            0x27f56f47ef059aedL, 0x282acb19eac701a8L, 0x2860bef032bc6109L, 0x2894eeac3f6b794cL,
            0x28ca2a574f46579eL, 0x29005a76918bf6c3L, 0x2934711435eef474L, 0x29698d59436ab191L,
            0x299ff0af94455df5L, 0x29d3f66dbcab5ab9L, 0x2a08f4092bd63167L, 0x2a3f310b76cbbdc1L,
            0x2a737ea72a3f5699L, 0x2aa85e50f4cf2c3fL, 0x2ade75e53202f74fL, 0x2b1309af3f41da91L,
            0x2b47cc1b0f125135L, 0x2b7dbf21d2d6e583L, 0x2bb2977523c64f72L, 0x2be73d526cb7e34eL,
            0x2c1d0ca707e5dc22L, 0x2c5227e864efa995L, 0x2c86b1e27e2b93faL, 0x2cbc5e5b1db678f9L,
            0x2cf1baf8f2920b9cL, 0x2d2629b72f368e83L, 0x2d5bb424fb043223L, 0x2d9150971ce29f56L,
            0x2dc5a4bce41b472bL, 0x2dfb0dec1d2218f6L, 0x2e30e8b392354f9aL, 0x2e6522e076c2a380L,
            0x2e9a6b9894734c61L, 0x2ed0833f5cc80fbcL, 0x2f04a40f33fa13abL, 0x2f39cd1300f89896L,
            0x2f70202be09b5f5eL, 0x2fa42836d8c23735L, 0x2fd932448ef2c503L, 0x300f7ed5b2af7643L,
            0x3043af458fada9eaL, 0x30789b16f3991465L, 0x30aec1dcb07f597eL, 0x30e33929ee4f97efL,
            0x3118077469e37deaL, 0x314e0951845c5d65L, 0x3182c5d2f2b9ba5fL, 0x31b77747af6828f7L,
            0x31ed55199b423335L, 0x3222553001096001L, 0x3256ea7c014bb801L, 0x328ca51b019ea601L,
            0x32c1e730e10327c1L, 0x32f660fd1943f1b1L, 0x332bf93c5f94ee1dL, 0x33617bc5bbbd14d2L,
            0x3395dab72aac5a07L, 0x33cb5164f5577089L, 0x340112df1956a655L, 0x34355796dfac4febL,
            0x346aad7c979763e5L, 0x34a0ac6ddebe9e6fL, 0x34d4d789566e460bL, 0x350a0d6bac09d78eL,
            0x354048634b8626b9L, 0x35745a7c1e67b067L, 0x35a9711b26019c81L, 0x35dfcd61ef8203a1L,
            0x3613e05d35b14245L, 0x3648d874831d92d6L, 0x367f0e91a3e4f78bL, 0x36b3691b066f1ab7L,
            0x36e84361c80ae165L, 0x371e543a3a0d99beL, 0x3752f4a464488017L, 0x3787b1cd7d5aa01cL,
            0x37bd9e40dcb14823L, 0x37f282e889eecd16L, 0x382723a2ac6a805cL, 0x385cec8b57852073L,
            0x389213d716b33448L, 0x38c698ccdc60015aL, 0x38fc3f00137801b0L, 0x3931a7600c2b010eL,
            0x396611380f35c151L, 0x399b9586130331a6L, 0x39d13d73cbe1ff08L, 0x3a058cd0beda7ec9L,
            0x3a3af004ee911e7cL, 0x3a70d603151ab30dL, 0x3aa50b83da615fd1L, 0x3ada4e64d0f9b7c5L,
            0x3b1070ff029c12dbL, 0x3b448d3ec3431792L, 0x3b79b08e7413dd76L, 0x3bb00e59088c6a6aL,
            0x3be411ef4aaf8504L, 0x3c19166b1d5b6646L, 0x3c4f5c05e4b23fd7L, 0x3c839983aeef67e6L,
            0x3cb87fe49aab41e0L, 0x3cee9fddc1561258L, 0x3d2323ea98d5cb77L, 0x3d57ece53f0b3e55L,
            0x3d8de81e8ece0deaL, 0x3dc2b1131940c8b2L, 0x3df75d57df90fadfL, 0x3e2d34add7753996L,
            0x3e6240eca6a943feL, 0x3e96d127d05394fdL, 0x3ecc8571c4687a3dL, 0x3f01d3671ac14c66L,
            0x3f364840e1719f80L, 0x3f6bda5119ce075fL, 0x3fa16872b020c49cL, 0x3fd5c28f5c28f5c3L,
            0x400B333333333333L };

        private static readonly long[] rawBitsFor1_2e0To309 = { 0x3ff3333333333333L, 0x4028000000000000L,
            0x405e000000000000L, 0x4092c00000000000L, 0x40c7700000000000L, 0x40fd4c0000000000L,
            0x41324f8000000000L, 0x4166e36000000000L, 0x419c9c3800000000L, 0x41d1e1a300000000L,
            0x42065a0bc0000000L, 0x423bf08eb0000000L, 0x427176592e000000L, 0x42a5d3ef79800000L,
            0x42db48eb57e00000L, 0x43110d9316ec0000L, 0x434550f7dca70000L, 0x437aa535d3d0c000L,
            0x43b0a741a4627800L, 0x43e4d1120d7b1600L, 0x441a055690d9db80L, 0x445043561a882930L,
            0x4484542ba12a337cL, 0x44b969368974c05bL, 0x44efc3842bd1f072L, 0x4523da329b633647L,
            0x4558d0bf423c03d9L, 0x458f04ef12cb04cfL, 0x45c363156bbee301L, 0x45f83bdac6ae9bc2L,
            0x462e4ad1785a42b2L, 0x4662eec2eb3869afL, 0x4697aa73a606841bL, 0x46cd95108f882522L,
            0x47027d2a59b51735L, 0x47371c74f0225d03L, 0x476ce3922c2af443L, 0x47a20e3b5b9ad8aaL,
            0x47d691ca32818ed5L, 0x480c363cbf21f28aL, 0x4841a1e5f7753796L, 0x48760a5f7552857cL,
            0x48ab8cf752a726daL, 0x48e1381a93a87849L, 0x491586213892965bL, 0x494ae7a986b73bf1L,
            0x4980d0c9f4328577L, 0x49b504fc713f26d5L, 0x49ea463b8d8ef08aL, 0x4a206be538795656L,
            0x4a5486de8697abecL, 0x4a89a896283d96e6L, 0x4ac0095dd9267e50L, 0x4af40bb54f701de4L,
            0x4b290ea2a34c255dL, 0x4b5f524b4c1f2eb4L, 0x4b93936f0f937d31L, 0x4bc8784ad3785c7dL,
            0x4bfe965d8856739cL, 0x4c331dfa75360842L, 0x4c67e57912838a52L, 0x4c9dded757246ce6L,
            0x4cd2ab469676c410L, 0x4d0756183c147514L, 0x4d3d2b9e4b199259L, 0x4d723b42eeeffb78L,
            0x4da6ca13aaabfa56L, 0x4ddc7c989556f8ebL, 0x4e11cddf5d565b93L, 0x4e46415734abf278L,
            0x4e7bd1ad01d6ef15L, 0x4eb1630c2126556dL, 0x4ee5bbcf296feac9L, 0x4f1b2ac2f3cbe57bL,
            0x4f50fab9d85f6f6dL, 0x4f8539684e774b48L, 0x4fba87c262151e1aL, 0x4ff094d97d4d32d0L,
            0x5024ba0fdca07f84L, 0x5059e893d3c89f65L, 0x5090315c645d639fL, 0x50c43db37d74bc87L,
            0x50f94d205cd1eba9L, 0x512fa06874066693L, 0x5163c4414884001cL, 0x5198b5519aa50023L,
            0x51cee2a6014e402cL, 0x52034da7c0d0e81bL, 0x52382111b1052222L, 0x526e29561d466aabL,
            0x52a2d9d5d24c02abL, 0x52d7904b46df0355L, 0x530d745e1896c42bL, 0x534268bacf5e3a9bL,
            0x537702e98335c941L, 0x53acc3a3e4033b92L, 0x53e1fa466e82053bL, 0x541678d80a22868aL,
            0x544c170e0cab282cL, 0x54818e68c7eaf91cL, 0x54b5f202f9e5b763L, 0x54eb6e83b85f253bL,
            0x55212512533b7745L, 0x55556e56e80a5516L, 0x558ac9eca20cea5cL, 0x55c0be33e5481279L,
            0x55f4edc0de9a1718L, 0x562a293116409cdeL, 0x566059beade8620bL, 0x5694702e59627a8dL,
            0x56c98c39efbb1931L, 0x56ffef486ba9df7dL, 0x5733f58d434a2baeL, 0x5768f2f0941cb699L,
            0x579f2facb923e440L, 0x57d37dcbf3b66ea8L, 0x58085d3ef0a40a52L, 0x583e748eaccd0ce6L,
            0x587308d92c002810L, 0x58a7cb0f77003214L, 0x58ddbdd354c03e99L, 0x591296a414f82720L,
            0x59473c4d1a3630e8L, 0x597d0b6060c3bd21L, 0x59b2271c3c7a5635L, 0x59e6b0e34b98ebc2L,
            0x5a1c5d1c1e7f26b3L, 0x5a51ba31930f7830L, 0x5a8628bdf7d3563cL, 0x5abbb2ed75c82bcaL,
            0x5af14fd4699d1b5fL, 0x5b25a3c984046236L, 0x5b5b0cbbe5057ac4L, 0x5b90e7f56f236cbaL,
            0x5bc521f2caec47e9L, 0x5bfa6a6f7da759e3L, 0x5c308285ae88982eL, 0x5c64a3271a2abe39L,
            0x5c99cbf0e0b56dc8L, 0x5cd01f768c71649dL, 0x5d0427542f8dbdc4L, 0x5d3931293b712d35L,
            0x5d6f7d738a4d7882L, 0x5da3ae6836706b51L, 0x5dd89a02440c8626L, 0x5e0ec082d50fa7afL,
            0x5e433851c529c8ceL, 0x5e78066636743b01L, 0x5eae07ffc41149c1L, 0x5ee2c4ffda8ace19L,
            0x5f17763fd12d819fL, 0x5f4d53cfc578e207L, 0x5f825461db6b8d44L, 0x5fb6e97a52467095L,
            0x5feca3d8e6d80cbbL, 0x6021e667904707f5L, 0x605660017458c9f2L, 0x608bf801d16efc6eL,
            0x60c17b0122e55dc5L, 0x60f5d9c16b9eb536L, 0x612b5031c6866284L, 0x6161121f1c13fd92L,
            0x619556a6e318fcf7L, 0x61caac509bdf3c34L, 0x6200abb2616b85a1L, 0x6234d69ef9c66709L,
            0x626a0c46b83800cbL, 0x62a047ac3323007fL, 0x62d459973febc09fL, 0x63096ffd0fe6b0c6L,
            0x633fcbfc53e05cf8L, 0x6373df7db46c3a1bL, 0x63a8d75d218748a2L, 0x63df0d3469e91acaL,
            0x64136840c231b0beL, 0x64484250f2be1ceeL, 0x647e52e52f6da42aL, 0x64b2f3cf3da4869aL,
            0x64e7b0c30d0da840L, 0x651d9cf3d0511251L, 0x655282186232ab72L, 0x6587229e7abf564fL,
            0x65bceb46196f2be3L, 0x65f2130bcfe57b6eL, 0x662697cec3deda49L, 0x665c3dc274d690dbL,
            0x6691a69989061a89L, 0x66c6103feb47a12bL, 0x66fb944fe6198976L, 0x67313cb1efcff5eaL,
            0x67658bde6bc3f364L, 0x679aeed606b4f03dL, 0x67d0d545c4311626L, 0x68050a97353d5bb0L,
            0x683a4d3d028cb29cL, 0x687070462197efa2L, 0x68a48c57a9fdeb8aL, 0x68d9af6d947d666cL,
            0x69100da47cce6004L, 0x6944110d9c01f805L, 0x6979155103027606L, 0x69af5aa543c31387L,
            0x69e398a74a59ec35L, 0x6a187ed11cf06742L, 0x6a4e9e85642c8112L, 0x6a8323135e9bd0abL,
            0x6ab7ebd83642c4d6L, 0x6aede6ce43d3760cL, 0x6b22b040ea6429c7L, 0x6b575c5124fd3439L,
            0x6b8d33656e3c8147L, 0x6bc2401f64e5d0cdL, 0x6bf6d0273e1f4500L, 0x6c2c84310da71640L,
            0x6c61d29ea8886de8L, 0x6c96474652aa8962L, 0x6ccbd917e7552bbaL, 0x6d0167aef0953b54L,
            0x6d35c19aacba8a29L, 0x6d6b320157e92cb4L, 0x6da0ff40d6f1bbf0L, 0x6dd53f110cae2aedL,
            0x6e0a8ed54fd9b5a8L, 0x6e40994551e81189L, 0x6e74bf96a66215ebL, 0x6ea9ef7c4ffa9b66L,
            0x6ee035adb1fca120L, 0x6f1443191e7bc967L, 0x6f4953df661abbc1L, 0x6f7fa8d73fa16ab2L,
            0x6fb3c98687c4e2afL, 0x6fe8bbe829b61b5bL, 0x701eeae23423a232L, 0x705352cd6096455fL,
            0x70882780b8bbd6b7L, 0x70be3160e6eacc64L, 0x70f2dedc9052bfbfL, 0x71279693b4676faeL,
            0x715d7c38a1814b9aL, 0x71926da364f0cf40L, 0x71c7090c3e2d0310L, 0x71fccb4f4db843d4L,
            0x7231ff1190932a65L, 0x72667ed5f4b7f4feL, 0x729c1e8b71e5f23dL, 0x72d19317272fb766L,
            0x7305f7dcf0fba540L, 0x733b75d42d3a8e90L, 0x737129a49c44991aL, 0x73a5740dc355bf60L,
            0x73dad111342b2f39L, 0x7410c2aac09afd83L, 0x7444f35570c1bce4L, 0x747a302accf22c1dL,
            0x74b05e1ac0175b92L, 0x74e475a1701d3277L, 0x75199309cc247f15L, 0x754ff7cc3f2d9edaL,
            0x7583fadfa77c8348L, 0x75b8f997915ba41aL, 0x75ef37fd75b28d21L, 0x762382fe698f9834L,
            0x765863be03f37e41L, 0x768e7cad84f05dd2L, 0x76c30dec73163aa3L, 0x76f7d1678fdbc94cL,
            0x772dc5c173d2bb9fL, 0x77629b98e863b543L, 0x7797427f227ca294L, 0x77cd131eeb1bcb39L,
            0x78022bf352f15f04L, 0x7836b6f027adb6c5L, 0x786c64ac31992476L, 0x78a1beeb9effb6caL,
            0x78d62ea686bfa47cL, 0x790bba50286f8d9bL, 0x794154721945b881L, 0x7975a98e9f9726a1L,
            0x79ab13f2477cf049L, 0x79e0ec776cae162eL, 0x7a15279547d99bb9L, 0x7a4a717a99d002a8L,
            0x7a8086eca02201a9L, 0x7ab4a8a7c82a8213L, 0x7ae9d2d1ba352298L, 0x7b2023c31461359fL,
            0x7b542cb3d9798307L, 0x7b8937e0cfd7e3c8L, 0x7bbf85d903cddcbaL, 0x7bf3b3a7a260a9f4L,
            0x7c28a0918af8d472L, 0x7c5ec8b5edb7098eL, 0x7c933d71b49265f9L, 0x7cc80cce21b6ff77L,
            0x7cfe1001aa24bf55L, 0x7d32ca010a56f795L, 0x7d677c814cecb57aL, 0x7d9d5ba1a027e2d9L,
            0x7dd259450418edc7L, 0x7e06ef96451f2939L, 0x7e3cab7bd666f388L, 0x7e71eb2d66005835L,
            0x7ea665f8bf806e42L, 0x7edbff76ef6089d2L, 0x7f117faa559c5623L, 0x7f45df94eb036bacL,
            0x7f7b577a25c44697L, 0x7fb116ac579aac1fL, 0x7fe55c576d815726L, 0x7ff0000000000000L };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used in Apache Harmony (may need someday)")]
        private void doTestCompareRawBits(string originalDoubleString, long expectedRawBits,
                string expectedString)
        {
            double result;
            long rawBits;
            string convertedString;
            result = Double.Parse(originalDoubleString, J2N.Text.StringFormatter.InvariantCulture);
            rawBits = Double.DoubleToInt64Bits(result);
            convertedString = new Double(result).ToString(J2N.Text.StringFormatter.InvariantCulture);
            assertEquals(expectedRawBits, rawBits);
            assertEquals(expectedString.ToLower(Locale_US), convertedString
                    .ToLower(Locale_US));
        }

        private void Test_toString(double dd, String answer)
        {
            assertEquals(answer, Double.ToString(dd, null, J2N.Text.StringFormatter.InvariantCulture));
            Double d = new Double(dd);
            assertEquals(answer, Double.ToString(d.ToDouble(), null, J2N.Text.StringFormatter.InvariantCulture));
            assertEquals(answer, d.ToString(J2N.Text.StringFormatter.InvariantCulture));

#if FEATURE_SPAN
            Span<char> buffer = stackalloc char[64];
            assertTrue(d.TryFormat(buffer, out int charsWritten, ReadOnlySpan<char>.Empty, CultureInfo.InvariantCulture));
            string actual = buffer.Slice(0, charsWritten).ToString();
            assertEquals(answer, actual);
#endif
        }

        /**
         * @tests java.lang.Double#Double(double)
         */
        [Test]
        public void Test_ConstructorD()
        {
            Double d = new Double(39089.88888888888888888888888888888888);
            assertEquals("Created incorrect double", 39089.88888888888888888888888888888888, d
                    .ToDouble(), 0D);
        }

        /**
         * @tests java.lang.Double#Double(java.lang.String)
         */
        [Test]
        public void Test_ConstructorLjava_lang_String()
        {
            // J2N TODO: Move the following tests to Double.Parse() in CharSequences

            Double d = Double.Parse("39089.88888888888888888888888888888888", J2N.Text.StringFormatter.InvariantCulture);
            assertEquals("Created incorrect double", 39089.88888888888888888888888888888888, d
                    .ToDouble(), 0D);


            // Regression test for HARMONY-489
            try
            {
                d = Double.Parse("1E+-20", J2N.Text.StringFormatter.InvariantCulture);
                fail("new Double(\"1E+-20\") should throw exception");
            }
            catch (FormatException e)
            {
                // expected
            }

            // Regression test for HARMONY-329
            d = Double.Parse("-1.233999999999999965116738099630936817275852021384209929081813042837802886790127428328465579708849276001782791006814286802871737087810957327493372866733334925806221045495205250590286471187577636646208155890426896101636282423463443661040209738873506655844025580428394216030152374941053494694642722606658935546875E-112", J2N.Text.StringFormatter.InvariantCulture);
            assertEquals("Failed to parse long string", -1.234E-112D, d.ToDouble(), 0D);
        }

        /**
         * @tests java.lang.Double#byteValue()
         */
        [Test]
        public void Test_byteValue()
        {
            Double d = new Double(1923311.47712);
            assertEquals("Returned incorrect byte value", (sbyte)-17, (sbyte)d.ToByte());
        }

        /**
         * @tests java.lang.Double#compareTo(java.lang.Double)
         * @tests java.lang.Double#compare(double, double)
         */
        [Test]
        public void Test_compare()
        {
            double[] values = new double[] { double.NegativeInfinity, -double.MaxValue, -2d,
                -double.Epsilon, -0d, 0d, double.Epsilon, 2d, double.MaxValue,
                double.PositiveInfinity, double.NaN }; // J2N NOTE: MIN_VALUE in Java is the same as Epsilon in .NET
            for (int i = 0; i < values.Length; i++)
            {
                double d1 = values[i];
                assertTrue("compare() should be equal: " + d1, Double.Compare(d1, d1) == 0);
                Double D1 = new Double(d1);
                assertTrue("compareTo() should be equal: " + d1, D1.CompareTo(D1) == 0);
                for (int j = i + 1; j < values.Length; j++)
                {
                    double d2 = values[j];
                    assertTrue("compare() " + d1 + " should be less " + d2,
                            Double.Compare(d1, d2) == -1);
                    assertTrue("compare() " + d2 + " should be greater " + d1, Double.Compare(d2,
                            d1) == 1);
                    Double D2 = new Double(d2);
                    assertTrue("compareTo() " + d1 + " should be less " + d2,
                            D1.CompareTo(D2) == -1);
                    assertTrue("compareTo() " + d2 + " should be greater " + d1,
                            D2.CompareTo(D1) == 1);
                }
            }

            //try
            //{
            //    new Double(0.0D).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Double(0.0D).CompareTo(null));
        }

        /**
         * @tests java.lang.Double#doubleToLongBits(double)
         */
        [Test]
        public void Test_doubleToLongBitsD()
        {
            // Test for method long java.lang.Double.doubleToLongBits(double)
            Double d = new Double(double.MaxValue);
            long lbits = Double.DoubleToInt64Bits(d.ToDouble());
            double r = Double.Int64BitsToDouble(lbits);

            assertTrue("Bit conversion failed", d.ToDouble() == r);
        }

        /**
         * @tests java.lang.Double#doubleToRawLongBits(double)
         */
        [Test]
        public void Test_doubleToRawLongBitsD()
        {
            long l = 0x7ff80000000004d2L;
            double d = Double.Int64BitsToDouble(l);
            assertTrue("Wrong raw bits", Double.DoubleToRawInt64Bits(d) == l);
        }

        /**
         * @tests java.lang.Double#doubleValue()
         */
        [Test]
        public void Test_doubleValue()
        {
            assertEquals("Incorrect double value returned", 999999999999999.9999999999999,
                    new Double(999999999999999.9999999999999).ToDouble(), 0D);
        }

        /**
         * @tests java.lang.Double#floatValue()
         */
        [Test]
        public void Test_floatValue()
        {
            // Test for method float java.lang.Double.floatValue()
            assertTrue(
                    "Incorrect float value returned ",
                    Math
                            .Abs(new Double(999999999999999.9999999999999d).ToSingle() - 999999999999999.9999999999999f) < 1);
        }

        [Test]
        public void GetTypeCode_Invoke_ReturnsDouble()
        {
            assertEquals(TypeCode.Double, Double.GetInstance(0.0).GetTypeCode());
        }

        /**
         * @tests java.lang.Double#hashCode()
         */
        [Test]
        public void Test_hashCode()
        {
            // Test for method int java.lang.Double.hashCode()
            for (int i = -1000; i < 1000; i++)
            {
                Double d = new Double(i);
                Double dd = new Double(i);
                assertTrue("Should not be identical ", d != dd);
                assertTrue("Should be equals 1 ", d.Equals(dd));
                assertTrue("Should be equals 2 ", dd.Equals(d));
                assertTrue("Should have identical values ", dd.ToDouble() == d.ToDouble());
                assertTrue("Invalid hash for equal but not identical doubles ", d.GetHashCode() == dd
                        .GetHashCode());
            }
            assertEquals("Magic assumption hasCode (0.0) = 0 failed", 0, new Double(0.0).GetHashCode());
        }

        /**
         * @tests java.lang.Double#intValue()
         */
        [Test]
        public void Test_intValue()
        {
            // Test for method int java.lang.Double.intValue()
            Double d = new Double(1923311.47712);
            assertEquals("Returned incorrect int value", 1923311, d.ToInt32());
        }

        /**
         * @tests java.lang.Double#isInfinite()
         */
        [Test]
        public void Test_isInfinite()
        {
            // Test for method boolean java.lang.Double.isInfinite()
            assertTrue("NEGATIVE_INFINITY returned false", new Double(double.NegativeInfinity)
                    .IsInfinity());
            assertTrue("POSITIVE_INFINITY returned false", new Double(double.PositiveInfinity)
                    .IsInfinity());
            assertTrue("Non infinite number returned true", !(new Double(1000).IsInfinity()));
        }

        /**
         * @tests java.lang.Double#isInfinite(double)
         */
        [Test]
        public void Test_isInfiniteD()
        {
            // Test for method boolean java.lang.Double.isInfinite(double)
            assertTrue("Infinity check failed", double.NegativeInfinity.IsInfinity()
                    && (double.PositiveInfinity.IsInfinity())
                    && !(double.MaxValue.IsInfinity()));
        }

        /**
         * @tests java.lang.Double#isNaN()
         */
        [Test]
        public void Test_isNaN()
        {
            // Test for method boolean java.lang.Double.isNaN()
            Double d = new Double(0.0 / 0.0);
            assertTrue("NAN returned false", d.IsNaN());
            d = new Double(0);
            assertTrue("Non NAN returned true", !d.IsNaN());
        }

        /**
         * @tests java.lang.Double#isNaN(double)
         */
        [Test]
        public void Test_isNaND()
        {
            // Test for method boolean java.lang.Double.isNaN(double)

            Double d = new Double(0.0 / 0.0);
            assertTrue("NAN check failed", d.ToDouble().IsNaN());
        }

        public static IEnumerable<TestCaseData> Test_IsInfinity_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, true);     // Negative Infinity
                yield return new TestCaseData(double.MinValue, false);            // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, false);   // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, false);   // Min Negative Subnormal
                yield return new TestCaseData(-double.Epsilon, false);            // Max Negative Subnormal (Negative Epsilon)
                yield return new TestCaseData(-0.0, false);                       // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(double.Epsilon, false);             // Min Positive Subnormal (Positive Epsilon)
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, true);     // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsInfinity_Data")]
        public void Test_IsInfinity(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsInfinity());
        }

        public static IEnumerable<TestCaseData> Test_IsNaN_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, false);    // Negative Infinity
                yield return new TestCaseData(double.MinValue, false);            // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, false);   // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, false);   // Min Negative Subnormal
                yield return new TestCaseData(-double.Epsilon, false);            // Max Negative Subnormal (Negative Epsilon)
                yield return new TestCaseData(-0.0, false);                       // Negative Zero
                yield return new TestCaseData(double.NaN, true);                  // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(double.Epsilon, false);             // Min Positive Subnormal (Positive Epsilon)
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsNaN_Data")]
        public void Test_IsNaN(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsNaN());
        }

        public static IEnumerable<TestCaseData> Test_IsNegativeInfinity_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, true);     // Negative Infinity
                yield return new TestCaseData(double.MinValue, false);            // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, false);   // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, false);   // Min Negative Subnormal
                yield return new TestCaseData(-double.Epsilon, false);            // Max Negative Subnormal (Negative Epsilon)
                yield return new TestCaseData(-0.0, false);                       // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(double.Epsilon, false);             // Min Positive Subnormal (Positive Epsilon)
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsNegativeInfinity_Data")]
        public void Test_IsNegativeInfinity(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsNegativeInfinity());
        }

        public static IEnumerable<TestCaseData> Test_IsPositiveInfinity_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, false);    // Negative Infinity
                yield return new TestCaseData(double.MinValue, false);            // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, false);   // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, false);   // Min Negative Subnormal
                yield return new TestCaseData(-double.Epsilon, false);            // Max Negative Subnormal (Negative Epsilon)
                yield return new TestCaseData(-0.0, false);                       // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(double.Epsilon, false);             // Min Positive Subnormal (Positive Epsilon)
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, true);     // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsPositiveInfinity_Data")]
        public void Test_IsPositiveInfinity(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsPositiveInfinity());
        }

        public static IEnumerable<TestCaseData> Test_IsFinite_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, false);    // Negative Infinity
                yield return new TestCaseData(double.MinValue, true);             // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, true);    // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, true);    // Min Negative Subnormal
                yield return new TestCaseData(-4.94065645841247E-324, true);      // Max Negative Subnormal
                yield return new TestCaseData(-0.0, true);                        // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, true);                         // Positive Zero
                yield return new TestCaseData(4.94065645841247E-324, true);       // Min Positive Subnormal
                yield return new TestCaseData(2.2250738585072009E-308, true);     // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, true);     // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, true);             // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsFinite_Data")]
        public void Test_IsFinite(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsFinite());
        }

        public static IEnumerable<TestCaseData> Test_IsNegative_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, true);     // Negative Infinity
                yield return new TestCaseData(double.MinValue, true);             // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, true);    // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, true);    // Min Negative Subnormal
                yield return new TestCaseData(-4.94065645841247E-324, true);      // Max Negative Subnormal
                yield return new TestCaseData(-0.0, true);                        // Negative Zero
                yield return new TestCaseData(double.NaN, true);                  // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(4.94065645841247E-324, false);      // Min Positive Subnormal
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsNegative_Data")]
        public void Test_IsNegative(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsNegative());
        }

        public static IEnumerable<TestCaseData> Test_IsNegativeZero_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, false);    // Negative Infinity
                yield return new TestCaseData(double.MinValue, false);            // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, false);   // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, false);   // Min Negative Subnormal
                yield return new TestCaseData(-4.94065645841247E-324, false);     // Max Negative Subnormal
                yield return new TestCaseData(-0.0, true);                        // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(4.94065645841247E-324, false);      // Min Positive Subnormal
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsNegativeZero_Data")]
        public void Test_IsNegativeZero(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsNegativeZero());
        }

        public static IEnumerable<TestCaseData> Test_IsNormal_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, false);    // Negative Infinity
                yield return new TestCaseData(double.MinValue, true);             // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, true);    // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, false);   // Min Negative Subnormal
                yield return new TestCaseData(-4.94065645841247E-324, false);     // Max Negative Subnormal
                yield return new TestCaseData(-0.0, false);                       // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(4.94065645841247E-324, false);      // Min Positive Subnormal
                yield return new TestCaseData(2.2250738585072009E-308, false);    // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, true);     // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, true);             // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsNormal_Data")]
        public void Test_IsNormal(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsNormal());
        }

        public static IEnumerable<TestCaseData> Test_IsSubnormal_Data
        {
            get
            {
                yield return new TestCaseData(double.NegativeInfinity, false);    // Negative Infinity
                yield return new TestCaseData(double.MinValue, false);            // Min Negative Normal
                yield return new TestCaseData(-2.2250738585072014E-308, false);   // Max Negative Normal
                yield return new TestCaseData(-2.2250738585072009E-308, true);    // Min Negative Subnormal
                yield return new TestCaseData(-4.94065645841247E-324, true);      // Max Negative Subnormal
                yield return new TestCaseData(-0.0, false);                       // Negative Zero
                yield return new TestCaseData(double.NaN, false);                 // NaN
                yield return new TestCaseData(0.0, false);                        // Positive Zero
                yield return new TestCaseData(4.94065645841247E-324, true);       // Min Positive Subnormal
                yield return new TestCaseData(2.2250738585072009E-308, true);     // Max Positive Subnormal
                yield return new TestCaseData(2.2250738585072014E-308, false);    // Min Positive Normal
                yield return new TestCaseData(double.MaxValue, false);            // Max Positive Normal
                yield return new TestCaseData(double.PositiveInfinity, false);    // Positive Infinity
            }
        }

        [TestCaseSource(typeof(TestDouble), "Test_IsSubnormal_Data")]
        public void Test_IsSubnormal(double d, bool expected)
        {
            assertEquals(expected, Double.GetInstance(d).IsSubnormal());
        }


        /**
         * @tests java.lang.Double#longBitsToDouble(long)
         */
        [Test]
        public void Test_longBitsToDoubleJ()
        {
            // Test for method double java.lang.Double.longBitsToDouble(long)

            Double d = new Double(double.MaxValue);
            long lbits = Double.DoubleToInt64Bits(d.ToDouble());
            double r = Double.Int64BitsToDouble(lbits);

            assertTrue("Bit conversion failed", d.ToDouble() == r);
        }

        /**
         * @tests java.lang.Double#longValue()
         */
        [Test]
        public void Test_longValue()
        {
            // Test for method long java.lang.Double.longValue()
            Double d = new Double(1923311.47712);
            assertEquals("Returned incorrect long value", 1923311, d.ToInt64());
        }

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDoubleLjava_lang_String()
        //{
        //    assertEquals("Incorrect double returned, expected zero.", 0.0, Double
        //            .Parse("2.4703282292062327208828439643411e-324", J2N.Text.StringFormatter.InvariantCulture), 0.0);
        //    assertTrue("Incorrect double returned, expected minimum double.", Double
        //            .Parse("2.4703282292062327208828439643412e-324", J2N.Text.StringFormatter.InvariantCulture) == double.Epsilon); // J2N NOTE: MIN_VALUE in Java is the same as Epsilon in .NET

        //    for (int i = 324; i > 0; i--)
        //    {
        //        Double.Parse("3.4e-" + i, J2N.Text.StringFormatter.InvariantCulture);
        //    }
        //    for (int i = 0; i <= 309; i++)
        //    {
        //        Double.Parse("1.2e" + i, J2N.Text.StringFormatter.InvariantCulture);
        //    }

        //    /*
        //     * The first two cases and the last four cases have to placed outside
        //     * the loop due to the difference in the expected output string.
        //     */
        //    doTestCompareRawBits("3.4e-324", rawBitsFor3_4en324ToN1[0], "4.9e-324");
        //    doTestCompareRawBits("3.4e-323", rawBitsFor3_4en324ToN1[1], "3.5e-323");
        //    for (int i = 322; i > 3; i--)
        //    {
        //        String testString, expectedString;
        //        testString = expectedString = "3.4e-" + i;
        //        doTestCompareRawBits(testString, rawBitsFor3_4en324ToN1[324 - i], expectedString);
        //    }
        //    doTestCompareRawBits("3.4e-3", rawBitsFor3_4en324ToN1[321], "0.0034");
        //    doTestCompareRawBits("3.4e-2", rawBitsFor3_4en324ToN1[322], "0.034");
        //    doTestCompareRawBits("3.4e-1", rawBitsFor3_4en324ToN1[323], "0.34");
        //    doTestCompareRawBits("3.4e-0", rawBitsFor3_4en324ToN1[324], "3.4");

        //    doTestCompareRawBits("1.2e0", rawBitsFor1_2e0To309[0], "1.2");
        //    doTestCompareRawBits("1.2e1", rawBitsFor1_2e0To309[1], "12.0");
        //    doTestCompareRawBits("1.2e2", rawBitsFor1_2e0To309[2], "120.0");
        //    doTestCompareRawBits("1.2e3", rawBitsFor1_2e0To309[3], "1200.0");
        //    doTestCompareRawBits("1.2e4", rawBitsFor1_2e0To309[4], "12000.0");
        //    doTestCompareRawBits("1.2e5", rawBitsFor1_2e0To309[5], "120000.0");
        //    doTestCompareRawBits("1.2e6", rawBitsFor1_2e0To309[6], "1200000.0");
        //    for (int i = 7; i <= 308; i++)
        //    {
        //        String testString, expectedString;
        //        testString = expectedString = "1.2e" + i;
        //        doTestCompareRawBits(testString, rawBitsFor1_2e0To309[i], expectedString);
        //    }
        //    doTestCompareRawBits("1.2e309", rawBitsFor1_2e0To309[309], "Infinity");

        //    doTestCompareRawBits(
        //            "111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000.92233720368547758079223372036854775807",
        //            0x7e054218c295e43fL, "1.1122233344455567E299");
        //    doTestCompareRawBits(
        //            "-111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000.92233720368547758079223372036854775807",
        //            unchecked((long)0xfe054218c295e43fL), "-1.1122233344455567E299");

        //    doTestCompareRawBits("1.234123412431233E107", 0x562ae7a25fe706ebL,
        //            "1.234123412431233E107");
        //    doTestCompareRawBits("1.2341234124312331E107", 0x562ae7a25fe706ecL,
        //            "1.2341234124312331E107");
        //    doTestCompareRawBits("1.2341234124312332E107", 0x562ae7a25fe706ecL,
        //            "1.2341234124312331E107");
        //    doTestCompareRawBits("-1.234123412431233E107", unchecked((long)0xd62ae7a25fe706ebL),
        //            "-1.234123412431233E107");
        //    doTestCompareRawBits("-1.2341234124312331E107", unchecked((long)0xd62ae7a25fe706ecL),
        //            "-1.2341234124312331E107");
        //    doTestCompareRawBits("-1.2341234124312332E107", unchecked((long)0xd62ae7a25fe706ecL),
        //            "-1.2341234124312331E107");

        //    doTestCompareRawBits("1e23", 0x44b52d02c7e14af6L, /*"1.0e23"*/ "9.999999999999999E22"); // J2N: This is the observed behavior in Java (at least on 64 bit windows)

        //    /*
        //     * These particular tests verify that the extreme boundary conditions
        //     * are converted correctly.
        //     */
        //    doTestCompareRawBits("0.0e-309", 0L, "0.0");
        //    doTestCompareRawBits("-0.0e-309", unchecked((long)0x8000000000000000L), "-0.0");
        //    doTestCompareRawBits("0.0e309", 0L, "0.0");
        //    doTestCompareRawBits("-0.0e309", unchecked((long)0x8000000000000000L), "-0.0");
        //    doTestCompareRawBits("0.1e309", 0x7fe1ccf385ebc8a0L, "1.0e308");
        //    doTestCompareRawBits("0.2e309", 0x7ff0000000000000L, "Infinity");
        //    doTestCompareRawBits("65e-325", 1L, "4.9e-324");
        //    doTestCompareRawBits("1000e-326", 2L, "1.0e-323");

        //    doTestCompareRawBits("4.0e-306", 0x86789e3750f791L, "4.0e-306");
        //    doTestCompareRawBits("2.22507e-308", 0xffffe2e8159d0L, "2.22507e-308");
        //    doTestCompareRawBits(
        //            "111222333444555666777888999000111228999000.92233720368547758079223372036854775807",
        //            0x48746da623f1dd8bL, "1.1122233344455567E41");
        //    doTestCompareRawBits(
        //            "-111222333444555666777888999000111228999000.92233720368547758079223372036854775807",
        //            unchecked((long)0xc8746da623f1dd8bL), "-1.1122233344455567E41");
        //    doTestCompareRawBits(
        //            "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890.987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210",
        //            0x54820fe0ba17f469L, "1.2345678901234567E99");
        //    doTestCompareRawBits(
        //            "-1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890.987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210",
        //            unchecked((long)0xd4820fe0ba17f469L), "-1.2345678901234567E99");

        //    doTestCompareRawBits(
        //            "179769313486231570000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01",
        //            0x7fefffffffffffffL, "1.7976931348623157E308");
        //    doTestCompareRawBits(
        //            "-179769313486231570000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01",
        //            unchecked((long)0xffefffffffffffffL), "-1.7976931348623157E308");
        //    doTestCompareRawBits(
        //            "1112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001234567890",
        //            0x7ff0000000000000L, "Infinity");
        //    doTestCompareRawBits(
        //            "-1112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001234567890",
        //            unchecked((long)0xfff0000000000000L), "-Infinity");
        //    doTestCompareRawBits(
        //            "179769313486231590000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01",
        //            0x7ff0000000000000L, "Infinity");
        //    doTestCompareRawBits(
        //            "-179769313486231590000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01",
        //            unchecked((long)0xfff0000000000000L), "-Infinity");
        //    doTestCompareRawBits(
        //            "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            0x2b392a32afcc661eL, "1.7976931348623157E-100");
        //    doTestCompareRawBits(
        //            "-0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            unchecked((long)0xab392a32afcc661eL), "-1.7976931348623157E-100");
        //    doTestCompareRawBits(
        //            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            0x1b3432f0cb68e61L, "1.7976931348623157E-300");
        //    doTestCompareRawBits(
        //            "-0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            unchecked((long)0x81b3432f0cb68e61L), "-1.7976931348623157E-300");
        //    doTestCompareRawBits(
        //            "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            0x2117b590b942L, "1.79769313486234E-310");
        //    doTestCompareRawBits(
        //            "-0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            unchecked((long)0x80002117b590b942L), "-1.79769313486234E-310");
        //    doTestCompareRawBits(
        //            "0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            0xe37L, "1.798E-320");
        //    doTestCompareRawBits(
        //            "-0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157",
        //            unchecked((long)0x8000000000000e37L), "-1.798E-320");
        //    doTestCompareRawBits(
        //            "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001",
        //            0x2L, "1.0E-323");
        //    doTestCompareRawBits(
        //            "-0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001",
        //            unchecked((long)0x8000000000000002L), "-1.0E-323");
        //    doTestCompareRawBits(
        //            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000055595409854908458349204328908234982349050934129878452378432452458968024357823490509341298784523784324524589680243578234905093412987845237843245245896802435782349050934129878452378432452458968024357868024357823490509341298784523784324524589680243578234905093412987845237843245245896802435786802435782349050934129878452378432452458968024357823490509341298784523784324524589680243578",
        //            0x1L, "4.9E-324");
        //    doTestCompareRawBits(
        //            "-0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000055595409854908458349204328908234982349050934129878452378432452458968024357823490509341298784523784324524589680243578234905093412987845237843245245896802435782349050934129878452378432452458968024357868024357823490509341298784523784324524589680243578234905093412987845237843245245896802435786802435782349050934129878452378432452458968024357823490509341298784523784324524589680243578",
        //            unchecked((long)0x8000000000000001L), "-4.9E-324");
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_Illegal()
        //{
        //    try
        //    {
        //        Double.Parse("0.0p0D", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse("+0x.p1d", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse("0Xg.gp1D", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse("-0x1.1p", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse("+0x 1.1 p2d", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse("x1.1p2d", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse(" 0x-2.1p2", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse(" 0x2.1pad", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }

        //    try
        //    {
        //        Double.Parse(" 0x111.222p 22d", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        fail("Should throw FormatException.");
        //    }
        //    catch (FormatException e)
        //    {
        //        // expected
        //    }
        //}

        // J2N: Moved to CharSequences
        /**
         * @tests java.lang.Double#parseDouble(java.lang.String)
         */
        //[Test]
        //public void Test_parseDouble_LString_FromHexString()
        //{
        //    double actual;
        //    double expected;

        //    actual = Double.Parse("0x0.0p0D", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0d, actual, 0.0D);

        //    actual = Double.Parse("0xa.ap+9d", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 5440.0d, actual, 0.0D);

        //    actual = Double.Parse("+0Xb.10ap8", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 2832.625d, actual, 0.0D);

        //    actual = Double.Parse("-0X.a0P2D", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", -2.5d, actual, 0.0D);

        //    actual = Double.Parse("\r 0x22.1p2d \t", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 136.25d, actual, 0.0D);

        //    actual = Double.Parse("0x1.0p-1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.5, actual, 0.0D);

        //    actual = Double
        //            .Parse("0x00000000000000000000000000000000001.0p-1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.5, actual, 0.0D);

        //    actual = Double.Parse("0x1.0p-00000000000000000000000000001", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.5, actual, 0.0D);

        //    actual = Double.Parse("0x.100000000000000000000000000000000p1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.125, actual, 0.0D);

        //    actual = Double.Parse("0x0.0p999999999999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);

        //    actual = Double.Parse("0xf1.0p9999999999999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", double.PositiveInfinity, actual, 0.0D);

        //    actual = Double.Parse("0xffffffffffffffffffffffffffffffffffff.ffffffffffffffffffffffffffffffffffffffffffffffp1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    expected = Double.Int64BitsToDouble(0x4900000000000000L);
        //    assertEquals("Returned incorrect value", expected, actual, 0.0D);

        //    actual = Double.Parse("0x0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001p1600", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    expected = Double.Int64BitsToDouble(0x7f30000000000000L);
        //    assertEquals("Returned incorrect value", expected, actual, 0.0D);

        //    actual = Double.Parse("0x0.0p-999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);

        //    actual = Double.Parse("0xf1.0p-9999999999999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);

        //    actual = Double.Parse("0x10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000p-1600", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    expected = Double.Int64BitsToDouble(0xf0000000000000L);
        //    assertEquals("Returned incorrect value", expected, actual, 0.0D);

        //    actual = Double.Parse("0x1.p9223372036854775807", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", double.PositiveInfinity, actual, 0.0D);

        //    actual = Double.Parse("0x1.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", double.PositiveInfinity, actual, 0.0D);

        //    actual = Double.Parse("0x10.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", double.PositiveInfinity, actual, 0.0D);

        //    actual = Double.Parse("0xabcd.ffffffffp+2000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", double.PositiveInfinity, actual, 0.0D);

        //    actual = Double.Parse("0x1.p-9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);

        //    actual = Double.Parse("0x1.p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);

        //    actual = Double.Parse("0x.1p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);

        //    actual = Double.Parse("0xabcd.ffffffffffffffp-2000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //    assertEquals("Returned incorrect value", 0.0, actual, 0.0D);
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_NormalPositiveExponent()
        //{
        //    long[] expecteds = {
        //        0x3f323456789abcdfL,                0x40e111012345678aL,                0x41a1110091a2b3c5L,
        //        0x4259998091a2b3c5L,                0x4311110048d159e2L,                0x43c5554048d159e2L,
        //        0x4479998048d159e2L,                0x452dddc048d159e2L,                0x45e111002468acf1L,
        //        0x469333202468acf1L,                0x4751011001234568L,                0x4802112101234568L,
        //        0x48b3213201234568L,                0x4964314301234568L,                0x4a15415401234568L,
        //        0x4ac6516501234568L,                0x4b77617601234568L,                0x4c28718701234568L,
        //        0x4cd9819801234568L,                0x4d9049048091a2b4L,                0x4e4101100091a2b4L,
        //        0x4ef189188091a2b4L,                0x4fa211210091a2b4L,                0x505299298091a2b4L,
        //        0x510321320091a2b4L,                0x51b3a93a8091a2b4L,                0x526431430091a2b4L,
        //        0x5314b94b8091a2b4L,                0x53c841840091a2b4L,                0x5478c98c8091a2b4L,
        //        0x552981980091a2b4L,                0x55da09a08091a2b4L,                0x568a91a90091a2b4L,
        //        0x573b19b18091a2b4L,                0x57eba1ba0091a2b4L,                0x589c29c28091a2b4L,
        //        0x594cb1cb0091a2b4L,                0x5a001d01c048d15aL,                0x5ab061060048d15aL,
        //        0x5b60a50a4048d15aL,                0x5c1101100048d15aL,                0x5cc145144048d15aL,
        //        0x5d7189188048d15aL,                0x5e21cd1cc048d15aL,                0x5ed211210048d15aL,
        //        0x5f8255254048d15aL,                0x603419418048d15aL,                0x60e45d45c048d15aL,
        //        0x6194a14a0048d15aL,                0x6244e54e4048d15aL,                0x62f541540048d15aL,
        //        0x63a585584048d15aL,                0x6455c95c8048d15aL,                0x65060d60c048d15aL,
        //        0x65b651650048d15aL,                0x666815814048d15aL,                0x671859858048d15aL,
        //        0x67c89d89c048d15aL,                0x6878e18e0048d15aL,                0x692925924048d15aL,
        //        0x69d981980048d15aL,                0x6a89c59c4048d15aL,                0x6b3a09a08048d15aL,
        //        0x6bea4da4c048d15aL,                0x6c9c11c10048d15aL,                0x6d4c55c54048d15aL,
        //        0x6dfc99c98048d15aL,                0x6eacddcdc048d15aL,                0x6f5d21d20048d15aL,
        //        0x700d65d64048d15aL,                0x70bdc1dc0048d15aL,                0x716e05e04048d15aL,
        //        0x721e49e48048d15aL,                0x72d00700602468adL,                0x73802902802468adL,
        //        0x74304b04a02468adL,                0x74e06d06c02468adL,                0x75908f08e02468adL,
        //        0x7640b10b002468adL,                0x76f0d30d202468adL,                0x77a10110002468adL,
        //        0x78512312202468adL,                0x79020520402468adL,                0x79b22722602468adL,
        //        0x7a624924802468adL,                0x7b126b26a02468adL,                0x7bc28d28c02468adL,
        //        0x7c72af2ae02468adL,                0x7d22d12d002468adL,                0x7dd2f32f202468adL,
        //        0x7e832132002468adL,                0x7f40011001012345L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
        //        0x7ff0000000000000L,                0x7ff0000000000000L  };

        //    for (int i = 0; i < expecteds.Length; i++)
        //    {
        //        int part = i * 11;
        //        String inputString = "0x" + part + "." + part + "0123456789abcdefp" + part;

        //        double actual = Double.Parse(inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputString
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_NormalNegativeExponent()
        //{
        //    long[] expecteds = {
        //        0x3f323456789abcdfL,                0x3f8111012345678aL,                0x3ee1110091a2b3c5L,
        //        0x3e39998091a2b3c5L,                0x3d91110048d159e2L,                0x3ce5554048d159e2L,
        //        0x3c39998048d159e2L,                0x3b8dddc048d159e2L,                0x3ae111002468acf1L,
        //        0x3a3333202468acf1L,                0x3991011001234568L,                0x38e2112101234568L,
        //        0x3833213201234568L,                0x3784314301234568L,                0x36d5415401234568L,
        //        0x3626516501234568L,                0x3577617601234568L,                0x34c8718701234568L,
        //        0x3419819801234568L,                0x337049048091a2b4L,                0x32c101100091a2b4L,
        //        0x321189188091a2b4L,                0x316211210091a2b4L,                0x30b299298091a2b4L,
        //        0x300321320091a2b4L,                0x2f53a93a8091a2b4L,                0x2ea431430091a2b4L,
        //        0x2df4b94b8091a2b4L,                0x2d4841840091a2b4L,                0x2c98c98c8091a2b4L,
        //        0x2be981980091a2b4L,                0x2b3a09a08091a2b4L,                0x2a8a91a90091a2b4L,
        //        0x29db19b18091a2b4L,                0x292ba1ba0091a2b4L,                0x287c29c28091a2b4L,
        //        0x27ccb1cb0091a2b4L,                0x27201d01c048d15aL,                0x267061060048d15aL,
        //        0x25c0a50a4048d15aL,                0x251101100048d15aL,                0x246145144048d15aL,
        //        0x23b189188048d15aL,                0x2301cd1cc048d15aL,                0x225211210048d15aL,
        //        0x21a255254048d15aL,                0x20f419418048d15aL,                0x20445d45c048d15aL,
        //        0x1f94a14a0048d15aL,                0x1ee4e54e4048d15aL,                0x1e3541540048d15aL,
        //        0x1d8585584048d15aL,                0x1cd5c95c8048d15aL,                0x1c260d60c048d15aL,
        //        0x1b7651650048d15aL,                0x1ac815814048d15aL,                0x1a1859858048d15aL,
        //        0x19689d89c048d15aL,                0x18b8e18e0048d15aL,                0x180925924048d15aL,
        //        0x175981980048d15aL,                0x16a9c59c4048d15aL,                0x15fa09a08048d15aL,
        //        0x154a4da4c048d15aL,                0x149c11c10048d15aL,                0x13ec55c54048d15aL,
        //        0x133c99c98048d15aL,                0x128cddcdc048d15aL,                0x11dd21d20048d15aL,
        //        0x112d65d64048d15aL,                0x107dc1dc0048d15aL,                0xfce05e04048d15aL,
        //        0xf1e49e48048d15aL,                0xe700700602468adL,                0xdc02902802468adL,
        //        0xd104b04a02468adL,                0xc606d06c02468adL,                0xbb08f08e02468adL,
        //        0xb00b10b002468adL,                0xa50d30d202468adL,                0x9a10110002468adL,
        //        0x8f12312202468adL,                0x8420520402468adL,                0x7922722602468adL,
        //        0x6e24924802468adL,                0x6326b26a02468adL,                0x5828d28c02468adL,
        //        0x4d2af2ae02468adL,                0x422d12d002468adL,                0x372f32f202468adL,
        //        0x2c32132002468adL,                0x220011001012345L,                0x170121012012345L,
        //        0xc0231023012345L,                0x10341034012345L,                0x208a208a024L,
        //        0x41584158L,                            0x83388L,                                0x108L,
        //        0x0L,                                        0x0L,                                       0x0L,
        //        0x0L,                                        0x0L,                                       0x0L,
        //        0x0L,                                        0x0L,                                       0x0L,
        //        0x0L,                                        0x0L,                                       0x0L,
        //        0x0L,                                        0x0L };

        //    for (int i = 0; i < expecteds.Length; i++)
        //    {
        //        int part = i * 11;
        //        String inputString = "0x" + part + "." + part + "0123456789abcdefp-" + part;

        //        double actual = Double.Parse(inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputString
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_MaxNormalBoundary()
        //{
        //    long[] expecteds = {
        //       0x7fefffffffffffffL,               0x7fefffffffffffffL,               0x7fefffffffffffffL,
        //       0x7fefffffffffffffL,               0x7fefffffffffffffL,               0x7fefffffffffffffL,
        //       0x7fefffffffffffffL,               0x7ff0000000000000L,               0x7ff0000000000000L,
        //       0x7ff0000000000000L,               0x7ff0000000000000L,               0x7ff0000000000000L,
        //       0x7ff0000000000000L,               0x7ff0000000000000L,               0x7ff0000000000000L,

        //       unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),
        //       unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),
        //       unchecked((long)0xffefffffffffffffL),               unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),
        //       unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),
        //       unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L) };

        //    String[] inputs = {
        //       "0x1.fffffffffffffp1023",
        //       "0x1.fffffffffffff000000000000000000000000001p1023",
        //       "0x1.fffffffffffff1p1023",
        //       "0x1.fffffffffffff100000000000000000000000001p1023",
        //       "0x1.fffffffffffff1fffffffffffffffffffffffffffffffffffffffffffffp1023",
        //       "0x1.fffffffffffff7p1023",
        //       "0x1.fffffffffffff700000000000000000000000001p1023",
        //       "0x1.fffffffffffff8p1023",
        //       "0x1.fffffffffffff800000000000000000000000001p1023",
        //       "0x1.fffffffffffff8fffffffffffffffffffffffffffffffffffffffffffffp1023",
        //       "0x1.fffffffffffff9p1023",
        //       "0x1.fffffffffffff900000000000000000000000001p1023",
        //       "0x1.ffffffffffffffp1023",
        //       "0x1.ffffffffffffff00000000000000000000000001p1023",
        //       "0x1.fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp1023",

        //       "-0x1.fffffffffffffp1023",
        //       "-0x1.fffffffffffff000000000000000000000000001p1023",
        //       "-0x1.fffffffffffff1p1023",
        //       "-0x1.fffffffffffff100000000000000000000000001p1023",
        //       "-0x1.fffffffffffff1fffffffffffffffffffffffffffffffffffffffffffffp1023",
        //       "-0x1.fffffffffffff7p1023",
        //       "-0x1.fffffffffffff700000000000000000000000001p1023",
        //       "-0x1.fffffffffffff8p1023",
        //       "-0x1.fffffffffffff800000000000000000000000001p1023",
        //       "-0x1.fffffffffffff8fffffffffffffffffffffffffffffffffffffffffffffp1023",
        //       "-0x1.fffffffffffff9p1023",
        //       "-0x1.fffffffffffff900000000000000000000000001p1023",
        //       "-0x1.ffffffffffffffp1023",
        //       "-0x1.ffffffffffffff00000000000000000000000001p1023",
        //       "-0x1.fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp1023" };

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        double actual = Double.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_MinNormalBoundary()
        //{
        //    long[] expecteds = {
        //        0x10000000000000L,                0x10000000000000L,                0x10000000000000L,
        //        0x10000000000000L,                0x10000000000000L,                0x10000000000000L,
        //        0x10000000000000L,                0x10000000000000L,                0x10000000000001L,
        //        0x10000000000001L,                0x10000000000001L,                0x10000000000001L,
        //        0x10000000000001L,                0x10000000000001L,                0x10000000000001L,

        //        unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
        //        unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
        //        unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000001L),
        //        unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L),
        //        unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L) };

        //    String[] inputs = {
        //       "0x1.0p-1022",
        //       "0x1.00000000000001p-1022",
        //       "0x1.000000000000010000000000000000001p-1022",
        //       "0x1.00000000000001fffffffffffffffffffffffffffffffffp-1022",
        //       "0x1.00000000000007p-1022",
        //       "0x1.000000000000070000000000000000001p-1022",
        //       "0x1.00000000000007fffffffffffffffffffffffffffffffffp-1022",
        //       "0x1.00000000000008p-1022",
        //       "0x1.000000000000080000000000000000001p-1022",
        //       "0x1.00000000000008fffffffffffffffffffffffffffffffffp-1022",
        //       "0x1.00000000000009p-1022",
        //       "0x1.000000000000090000000000000000001p-1022",
        //       "0x1.00000000000009fffffffffffffffffffffffffffffffffp-1022",
        //       "0x1.0000000000000fp-1022",
        //       "0x1.0000000000000ffffffffffffffffffffffffffffffffffp-1022",

        //       "-0x1.0p-1022",
        //       "-0x1.00000000000001p-1022",
        //       "-0x1.000000000000010000000000000000001p-1022",
        //       "-0x1.00000000000001fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x1.00000000000007p-1022",
        //       "-0x1.000000000000070000000000000000001p-1022",
        //       "-0x1.00000000000007fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x1.00000000000008p-1022",
        //       "-0x1.000000000000080000000000000000001p-1022",
        //       "-0x1.00000000000008fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x1.00000000000009p-1022",
        //       "-0x1.000000000000090000000000000000001p-1022",
        //       "-0x1.00000000000009fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x1.0000000000000fp-1022",
        //       "-0x1.0000000000000ffffffffffffffffffffffffffffffffffp-1022" };

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        double actual = Double.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_MaxSubNormalBoundary()
        //{
        //    long[] expecteds = {
        //        0xfffffffffffffL,                0xfffffffffffffL,                0xfffffffffffffL,
        //        0xfffffffffffffL,                0xfffffffffffffL,                0xfffffffffffffL,
        //        0xfffffffffffffL,                0x10000000000000L,                0x10000000000000L,
        //        0x10000000000000L,                0x10000000000000L,                0x10000000000000L,
        //        0x10000000000000L,                0x10000000000000L,                0x10000000000000L,

        //        unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),
        //        unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),
        //        unchecked((long)0x800fffffffffffffL),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
        //        unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
        //        unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L) };

        //    String[] inputs = {
        //       "0x0.fffffffffffffp-1022",
        //       "0x0.fffffffffffff00000000000000000000000000000000001p-1022",
        //       "0x0.fffffffffffff1p-1022",
        //       "0x0.fffffffffffff10000000000000000000000000000000001p-1022",
        //       "0x0.fffffffffffff1ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.fffffffffffff7p-1022",
        //       "0x0.fffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.fffffffffffff8p-1022",
        //       "0x0.fffffffffffff80000000000000000000000000000000001p-1022",
        //       "0x0.fffffffffffff8ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.fffffffffffff9p-1022",
        //       "0x0.fffffffffffff9ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.ffffffffffffffp-1022",
        //       "0x0.ffffffffffffff0000000000000000000000000000000001p-1022",
        //       "0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",

        //       "-0x0.fffffffffffffp-1022",
        //       "-0x0.fffffffffffff00000000000000000000000000000000001p-1022",
        //       "-0x0.fffffffffffff1p-1022",
        //       "-0x0.fffffffffffff10000000000000000000000000000000001p-1022",
        //       "-0x0.fffffffffffff1ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.fffffffffffff7p-1022",
        //       "-0x0.fffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.fffffffffffff8p-1022",
        //       "-0x0.fffffffffffff80000000000000000000000000000000001p-1022",
        //       "-0x0.fffffffffffff8ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.fffffffffffff9p-1022",
        //       "-0x0.fffffffffffff9ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.ffffffffffffffp-1022",
        //       "-0x0.ffffffffffffff0000000000000000000000000000000001p-1022",
        //       "-0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022" };

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        double actual = Double.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_MinSubNormalBoundary()
        //{
        //    long[] expecteds = {
        //        0x1L,                0x1L,                0x2L,
        //        0x1L,                0x1L,                0x1L,
        //        0x2L,                0x2L,                0x2L,
        //        0x2L,                0x2L,                0x2L,
        //        0x2L,                0x2L,                0x2L,

        //        unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000002L),
        //        unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),
        //        unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),
        //        unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),
        //        unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L) };

        //    String[] inputs = {
        //       "0x0.0000000000001p-1022",
        //       "0x0.00000000000010000000000000000001p-1022",
        //       "0x0.0000000000001fffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.00000000000017p-1022",
        //       "0x0.000000000000170000000000000000001p-1022",
        //       "0x0.00000000000017fffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.00000000000018p-1022",
        //       "0x0.000000000000180000000000000000001p-1022",
        //       "0x0.00000000000018fffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.00000000000019p-1022",
        //       "0x0.000000000000190000000000000000001p-1022",
        //       "0x0.00000000000019fffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.0000000000001fp-1022",
        //       "0x0.0000000000001f0000000000000000001p-1022",
        //       "0x0.0000000000001ffffffffffffffffffffffffffffffffffp-1022",

        //       "-0x0.0000000000001p-1022",
        //       "-0x0.00000000000010000000000000000001p-1022",
        //       "-0x0.0000000000001fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.00000000000017p-1022",
        //       "-0x0.000000000000170000000000000000001p-1022",
        //       "-0x0.00000000000017fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.00000000000018p-1022",
        //       "-0x0.000000000000180000000000000000001p-1022",
        //       "-0x0.00000000000018fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.00000000000019p-1022",
        //       "-0x0.000000000000190000000000000000001p-1022",
        //       "-0x0.00000000000019fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.0000000000001fp-1022",
        //       "-0x0.0000000000001f0000000000000000001p-1022",
        //       "-0x0.0000000000001ffffffffffffffffffffffffffffffffffp-1022" };

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        double actual = Double.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        // J2N: Moved to CharSequences
        ///**
        // * @tests java.lang.Double#parseDouble(java.lang.String)
        // */
        //[Test]
        //public void Test_parseDouble_LString_ZeroBoundary()
        //{
        //    long[] expecteds = {
        //        0x0L,                0x0L,                0x0L,
        //        0x1L,                0x1L,                0x1L,
        //        0x1L,                0x1L,                0x1L,
        //        unchecked((long)0x8000000000000000L),                unchecked((long)0x8000000000000000L),                unchecked((long)0x8000000000000000L),
        //        unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),
        //        unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L) };

        //    String[] inputs = {
        //       "0x0.00000000000004p-1022",
        //       "0x0.00000000000007ffffffffffffffffffffffp-1022",
        //       "0x0.00000000000008p-1022",
        //       "0x0.000000000000080000000000000000001p-1022",
        //       "0x0.00000000000008fffffffffffffffffffffffffffffffp-1022",
        //       "0x0.00000000000009p-1022",
        //       "0x0.000000000000090000000000000000001p-1022",
        //       "0x0.00000000000009fffffffffffffffffffffffffffffffffp-1022",
        //       "0x0.0000000000000fffffffffffffffffffffffffffffffffffp-1022",

        //       "-0x0.00000000000004p-1022",
        //       "-0x0.00000000000007ffffffffffffffffffffffp-1022",
        //       "-0x0.00000000000008p-1022",
        //       "-0x0.000000000000080000000000000000001p-1022",
        //       "-0x0.00000000000008fffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.00000000000009p-1022",
        //       "-0x0.000000000000090000000000000000001p-1022",
        //       "-0x0.00000000000009fffffffffffffffffffffffffffffffffp-1022",
        //       "-0x0.0000000000000fffffffffffffffffffffffffffffffffffp-1022" };

        //    for (int i = 0; i < inputs.Length; i++)
        //    {
        //        double actual = Double.Parse(inputs[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
        //        double expected = Double.Int64BitsToDouble(expecteds[i]);

        //        String expectedString = "0x" + Double.DoubleToInt64Bits(expected).ToHexString();
        //        String actualString = "0x" + Double.DoubleToInt64Bits(actual).ToHexString();
        //        String errorMsg = i + "th input string is:<" + inputs[i]
        //                + ">.The expected result should be:<" + expectedString
        //                + ">, but was: <" + actualString + ">. ";

        //        assertEquals(errorMsg, expected, actual, 0.0D);
        //    }
        //}

        /**
         * @tests java.lang.Double#shortValue()
         */
        [Test]
        public void Test_shortValue()
        {
            // Test for method short java.lang.Double.shortValue()
            Double d = new Double(1923311.47712);
            assertEquals("Returned incorrect short value", 22767, d.ToInt16());
        }

        /**
         * @tests java.lang.Double#toString()
         */
        [Test]
        public void Test_toString()
        {
            // Test for method java.lang.String java.lang.Double.toString()
            Test_toString(1.7976931348623157E308, "1.7976931348623157E308");
            Test_toString(5.0E-4, "5.0E-4");
        }

        /**
         * @tests java.lang.Double#toString(double)
         */
        [Test]
        public void Test_toStringD()
        {
            // Test for method java.lang.String java.lang.Double.toString(double)
            //Test_toString(1.7976931348623157E308, "1.7976931348623157E308");
            Test_toString(1.0 / 0.0, "Infinity");
            Test_toString(0.0 / 0.0, "NaN");
            Test_toString(-1.0 / 0.0, "-Infinity");

            double d;
            d = Double.Int64BitsToDouble(0x470fffffffffffffL);
            Test_toString(d, "2.0769187434139308E34");

            d = Double.Int64BitsToDouble(0x4710000000000000L);
            Test_toString(d, "2.076918743413931E34");


            d = Double.Int64BitsToDouble(0x470000000000000aL);
            Test_toString(d, "1.0384593717069678E34");

            d = Double.Int64BitsToDouble(0x470000000000000bL);
            Test_toString(d, "1.038459371706968E34");


            d = Double.Int64BitsToDouble(0x4700000000000017L);
            Test_toString(d, "1.0384593717069708E34");

            d = Double.Int64BitsToDouble(0x4700000000000018L);
            Test_toString(d, "1.038459371706971E34");


            d = Double.Int64BitsToDouble(0x4700000000000024L);
            Test_toString(d, "1.0384593717069738E34");

            d = Double.Int64BitsToDouble(0x4700000000000025L);
            Test_toString(d, "1.038459371706974E34");


            d = Double.Int64BitsToDouble(0x4700000000000031L);
            Test_toString(d, "1.0384593717069768E34");

            d = Double.Int64BitsToDouble(0x4700000000000032L);
            Test_toString(d, "1.038459371706977E34");


            d = Double.Int64BitsToDouble(0x470000000000003eL);
            Test_toString(d, "1.0384593717069798E34");

            d = Double.Int64BitsToDouble(0x470000000000003fL);
            Test_toString(d, "1.03845937170698E34");


            d = Double.Int64BitsToDouble(0x7e00000000000003L);
            Test_toString(d, "8.371160993642719E298");

            d = Double.Int64BitsToDouble(0x7e00000000000004L);
            Test_toString(d, "8.37116099364272E298");


            d = Double.Int64BitsToDouble(0x7e00000000000008L);
            Test_toString(d, "8.371160993642728E298");

            d = Double.Int64BitsToDouble(0x7e00000000000009L);
            Test_toString(d, "8.37116099364273E298");


            d = Double.Int64BitsToDouble(0x7e00000000000013L);
            Test_toString(d, "8.371160993642749E298");

            d = Double.Int64BitsToDouble(0x7e00000000000014L);
            Test_toString(d, "8.37116099364275E298");


            d = Double.Int64BitsToDouble(0x7e00000000000023L);
            Test_toString(d, "8.371160993642779E298");

            d = Double.Int64BitsToDouble(0x7e00000000000024L);
            Test_toString(d, "8.37116099364278E298");


            d = Double.Int64BitsToDouble(0x7e0000000000002eL);
            Test_toString(d, "8.371160993642799E298");

            d = Double.Int64BitsToDouble(0x7e0000000000002fL);
            Test_toString(d, "8.3711609936428E298");


            d = Double.Int64BitsToDouble(unchecked((long)0xda00000000000001L));
            Test_toString(d, "-3.3846065602060736E125");

            d = Double.Int64BitsToDouble(unchecked((long)0xda00000000000002L));
            Test_toString(d, "-3.384606560206074E125");


            d = Double.Int64BitsToDouble(unchecked((long)0xda00000000000005L));
            Test_toString(d, "-3.3846065602060766E125");

            d = Double.Int64BitsToDouble(unchecked((long)0xda00000000000006L));
            Test_toString(d, "-3.384606560206077E125");


            d = Double.Int64BitsToDouble(unchecked((long)0xda00000000000009L));
            Test_toString(d, "-3.3846065602060796E125");

            d = Double.Int64BitsToDouble(unchecked((long)0xda0000000000000aL));
            Test_toString(d, "-3.38460656020608E125");


            d = Double.Int64BitsToDouble(unchecked((long)0xda0000000000000dL));
            Test_toString(d, "-3.3846065602060826E125");

            d = Double.Int64BitsToDouble(unchecked((long)0xda0000000000000eL));
            Test_toString(d, "-3.384606560206083E125");
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { -4567.0, "G", null, "-4567" };
            yield return new object[] { -4567.89101, "G", null, "-4567.89101" };
            yield return new object[] { 0.0, "G", null, "0" };
            yield return new object[] { 4567.0, "G", null, "4567" };
            yield return new object[] { 4567.89101, "G", null, "4567.89101" };

            yield return new object[] { double.NaN, "G", null, "NaN" };

            yield return new object[] { 2468.0, "N", null, "2,468.00" };

            // Changing the negative pattern doesn't do anything without also passing in a format string
            var customNegativePattern = new NumberFormatInfo() { NumberNegativePattern = 0 };
            yield return new object[] { -6310.0, "G", customNegativePattern, "-6310" };

            var customNegativeSignDecimalGroupSeparator = new NumberFormatInfo()
            {
                NegativeSign = "#",
                NumberDecimalSeparator = "~",
                NumberGroupSeparator = "*"
            };
            yield return new object[] { -2468.0, "N", customNegativeSignDecimalGroupSeparator, "#2*468~00" };
            yield return new object[] { 2468.0, "N", customNegativeSignDecimalGroupSeparator, "2*468~00" };

            var customNegativeSignGroupSeparatorNegativePattern = new NumberFormatInfo()
            {
                NegativeSign = "xx", // Set to trash to make sure it doesn't show up
                NumberGroupSeparator = "*",
                NumberNegativePattern = 0,
            };
            yield return new object[] { -2468.0, "N", customNegativeSignGroupSeparatorNegativePattern, "(2*468.00)" };

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { double.NaN, "G", invariantFormat, "NaN" };
            yield return new object[] { double.PositiveInfinity, "G", invariantFormat, "Infinity" };
            yield return new object[] { double.NegativeInfinity, "G", invariantFormat, "-Infinity" };


            // Custom tests
            yield return new object[] { -4567.0, "J", null, "-4567.0" };
            yield return new object[] { -4567.89101, "J", null, "-4567.89101" };
            yield return new object[] { 0.0, "J", null, "0.0" };
            yield return new object[] { 4567.0, "J", null, "4567.0" };
            yield return new object[] { 4567.89101, "J", null, "4567.89101" };

            yield return new object[] { double.NaN, "J", null, "NaN" };

            // Changing the negative pattern doesn't do anything without also passing in a format string
            var customNegativePattern2 = new NumberFormatInfo() { NumberNegativePattern = 0 };
            yield return new object[] { -6310.0, "J", customNegativePattern2, "-6310.0" };

            yield return new object[] { 2.1847856458940424E+18, "J", null, "2.1847856458940424E18" };
            yield return new object[] { 2.1847856458940424E+18, "J", new CultureInfo("de-DE"), "2,1847856458940424E18" };

            // the follow values come from the Double Javadoc/Spec
            yield return new object[] { 0.0D, "x", null, "0x0.0p0" };
            yield return new object[] { -0.0D, "x", null, "-0x0.0p0" };
            yield return new object[] { 1.0D, "x", null, "0x1.0p0" };
            yield return new object[] { -1.0D, "x", null, "-0x1.0p0" };
            yield return new object[] { 2.0D, "x", null, "0x1.0p1" };
            yield return new object[] { 3.0D, "x", null, "0x1.8p1" };
            yield return new object[] { 0.5D, "x", null, "0x1.0p-1" };
            yield return new object[] { 0.25D, "x", null, "0x1.0p-2" };
            yield return new object[] { double.MaxValue, "x", null, "0x1.fffffffffffffp1023" };
            yield return new object[] { double.Epsilon, "x", null, "0x0.0000000000001p-1022" }; // J2N: In .NET double.Epsilon is the same as Double.MIN_VALUE in Java


            // test edge cases
            yield return new object[] { double.NaN, "x", null, "NaN" };
            yield return new object[] { double.NegativeInfinity, "x", null, "-Infinity" };
            yield return new object[] { double.PositiveInfinity, "x", null, "Infinity" };

            // test various numbers
            yield return new object[] { -118.625D, "x", null, "-0x1.da8p6" };
            yield return new object[] { 9743299.65D, "x", null, "0x1.2957874cccccdp23" };
            yield return new object[] { 9743299.65000D, "x", null, "0x1.2957874cccccdp23" };
            yield return new object[] { 9743299.650001234D, "x", null, "0x1.2957874cccf63p23" };
            yield return new object[] { 12349743299.65000D, "x", null, "0x1.700d1061d3333p33" };
        }

        public static IEnumerable<object[]> ToString_TestData_NotNetFramework()
        {
            foreach (var testData in ToString_TestData())
            {
                yield return testData;
            }

            yield return new object[] { double.MinValue, "G", null, "-1.7976931348623157E+308" };
            yield return new object[] { double.MaxValue, "G", null, "1.7976931348623157E+308" };

            yield return new object[] { double.Epsilon, "G", null, "5E-324" };

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { double.Epsilon, "G", invariantFormat, "5E-324" };
        }

#if NETCOREAPP3_0_OR_GREATER
        [Test]
        public static void Test_ToString_NotNetFramework()
        {
            using (new CultureContext(CultureInfo.InvariantCulture))
            {
                foreach (object[] testdata in ToString_TestData_NotNetFramework())
                {
                    ToString((double)testdata[0], (string)testdata[1], (IFormatProvider)testdata[2], (string)testdata[3]);
                }
            }
        }
#else
        [Test]
        public static void Test_ToString()
        {
            using (new CultureContext(CultureInfo.InvariantCulture))
            {
                foreach (object[] testdata in ToString_TestData())
                {
                    ToString((double)testdata[0], (string)testdata[1], (IFormatProvider)testdata[2], (string)testdata[3]);
                }
            }
        }
#endif

        private static void ToString(double d, string format, IFormatProvider provider, string expected)
        {
            bool isDefaultProvider = (provider == null || provider == NumberFormatInfo.CurrentInfo);
            if (/*string.IsNullOrEmpty(format) ||*/ format.ToUpperInvariant() == "G")
            {
                if (isDefaultProvider)
                {
                    assertEquals(expected, Double.ToString(d, format));
                    assertEquals(expected, Double.ToString(d, format, (IFormatProvider)null));
                }
                assertEquals(expected, Double.ToString(d, format, provider));
            }
            // J2N: The default format is J rather than G
            if (string.IsNullOrEmpty(format) || format.ToUpperInvariant() == "J")
            {
                if (isDefaultProvider)
                {
                    assertEquals(expected, Double.ToString(d));
                    assertEquals(expected, Double.ToString(d, (IFormatProvider)null));
                }
                assertEquals(expected, Double.ToString(d, provider));
            }
            if (format.ToUpperInvariant() == "X")
            {
                var info = NumberFormatInfo.GetInstance(provider);
                if (expected != info.NaNSymbol && expected != info.PositiveInfinitySymbol && expected != info.NegativeInfinitySymbol)
                {
                    if (isDefaultProvider)
                    {
                        assertEquals(expected.ToUpperInvariant(), Double.ToString(d, format.ToUpperInvariant())); // If format is upper case, then exponents are printed in upper case
                        assertEquals(expected.ToLowerInvariant(), Double.ToString(d, format.ToLowerInvariant())); // If format is lower case, then exponents are printed in upper case
                        assertEquals(expected.ToUpperInvariant(), Double.ToString(d, format.ToUpperInvariant(), null));
                        assertEquals(expected.ToLowerInvariant(), Double.ToString(d, format.ToLowerInvariant(), null));
                    }
                    assertEquals(expected.ToUpperInvariant(), Double.ToString(d, format.ToUpperInvariant(), provider));
                    assertEquals(expected.ToLowerInvariant(), Double.ToString(d, format.ToLowerInvariant(), provider));
                }
            }
            else
            {
                if (isDefaultProvider)
                {
                    assertEquals(expected.Replace('e', 'E'), Double.ToString(d, format.ToUpperInvariant())); // If format is upper case, then exponents are printed in upper case
                    assertEquals(expected.Replace('E', 'e'), Double.ToString(d, format.ToLowerInvariant())); // If format is lower case, then exponents are printed in upper case
                    assertEquals(expected.Replace('e', 'E'), Double.ToString(d, format.ToUpperInvariant(), null));
                    assertEquals(expected.Replace('E', 'e'), Double.ToString(d, format.ToLowerInvariant(), null));
                }
                assertEquals(expected.Replace('e', 'E'), Double.ToString(d, format.ToUpperInvariant(), provider));
                assertEquals(expected.Replace('E', 'e'), Double.ToString(d, format.ToLowerInvariant(), provider));
            }
        }

        [Test]
        public static void ToString_InvalidFormat_ThrowsFormatException()
        {
            double d = 123.0;
            Assert.Throws<FormatException>(() => d.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => d.ToString("Y", null)); // Invalid format
        }


        /**
         * @tests java.lang.Double#valueOf(java.lang.String)
         */
        [Test]
        public void Test_valueOfLjava_lang_String()
        {
            // Test for method java.lang.Double
            // java.lang.Double.valueOf(java.lang.String)
            assertTrue("Incorrect double returned", Math.Abs(Double.GetInstance("999999999999.999", J2N.Text.StringFormatter.InvariantCulture)
                    .ToDouble() - 999999999999.999d) < 1);

            try
            {
                Double.GetInstance(null, J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected Double.valueOf(null) to throw NPE.");
            }
            catch (ArgumentNullException ex)
            {
                // expected
            }

            try
            {
                Double.GetInstance("", J2N.Text.StringFormatter.InvariantCulture);
                fail("Expected Double.valueOf(\"\") to throw NFE");
            }
            catch (FormatException e)
            {
                // expected
            }

            Double pi = Double.GetInstance("3.141592654", J2N.Text.StringFormatter.InvariantCulture);
            assertEquals(3.141592654, pi.ToDouble(), 0D);

            Double posZero = Double.GetInstance("+0.0", J2N.Text.StringFormatter.InvariantCulture);
            Double negZero = Double.GetInstance("-0.0", J2N.Text.StringFormatter.InvariantCulture);
            assertFalse("Doubletest0", posZero.Equals(negZero));

            // J2N: .NET specific - testing specific cultures should also parse negative zero correctly
            Double posZero_de = Double.GetInstance("+0,0", new CultureInfo("de-DE"));
            Double negZero_de = Double.GetInstance("-0,0", new CultureInfo("de-DE"));
            assertFalse("Doubletest0", posZero_de.Equals(negZero_de));

            // Tests for double values by name.
            Double expectedNaN = new Double(double.NaN);

            Double posNaN = Double.GetInstance("NaN", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Doubletest1", posNaN.Equals(expectedNaN));

            Double posNaNSigned = Double.GetInstance("+NaN", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Doubletest2", posNaNSigned.Equals(expectedNaN));

            Double negNaNSigned = Double.GetInstance("-NaN", J2N.Text.StringFormatter.InvariantCulture);
            assertTrue("Doubletest3", negNaNSigned.Equals(expectedNaN));

            Double posInfinite = Double.GetInstance("Infinity", J2N.Text.StringFormatter.InvariantCulture); // J2N: Same behavior, but only if specifying invariant culture, other cultures throw FormatException in this case
            assertTrue("Doubletest4", posInfinite.Equals(new Double(double.PositiveInfinity)));

            Double posInfiniteSigned = Double.GetInstance("+Infinity", J2N.Text.StringFormatter.InvariantCulture); // J2N: Same behavior, but only if specifying invariant culture, other cultures throw FormatException in this case
            assertTrue("Doubletest5", posInfiniteSigned
                    .Equals(new Double(double.PositiveInfinity)));

            Double negInfiniteSigned = Double.GetInstance("-Infinity", J2N.Text.StringFormatter.InvariantCulture); // J2N: Same behavior, but only if specifying invariant culture, other cultures throw FormatException in this case
            assertTrue("Doubletest6", negInfiniteSigned
                    .Equals(new Double(double.NegativeInfinity)));
        }

        /**
         * @tests java.lang.Double#compareTo(java.lang.Double)
         * @tests java.lang.Double#compare(double, double)
         */
        [Test]
        public void Test_compareToLjava_lang_Double()
        {
            // A selection of double values in ascending order.
            double[] values = new double[] { double.NegativeInfinity, -double.MaxValue, -2d,
                -double.Epsilon, -0d, 0d, double.Epsilon, 2d, double.MaxValue,
                double.PositiveInfinity, double.NaN }; // J2N NOTE: MIN_VALUE in Java is the same as Epsilon in .NET
            for (int i = 0; i < values.Length; i++)
            {
                double d1 = values[i];

                // Test that each value compares equal to itself; and each object is
                // equal to another object like itself.
                assertTrue("Assert 0: compare() should be equal: " + d1,
                        Double.Compare(d1, d1) == 0);
                Double objDouble = new Double(d1);
                assertTrue("Assert 1: compareTo() should be equal: " + d1, objDouble
                        .CompareTo(objDouble) == 0);

                // Test that the Double-defined order is respected
                for (int j = i + 1; j < values.Length; j++)
                {
                    double d2 = values[j];
                    assertTrue("Assert 2: compare() " + d1 + " should be less " + d2, Double
                            .Compare(d1, d2) == -1);
                    assertTrue("Assert 3: compare() " + d2 + " should be greater " + d1, Double
                            .Compare(d2, d1) == 1);
                    Double D2 = new Double(d2);
                    assertTrue("Assert 4: compareTo() " + d1 + " should be less " + d2, objDouble
                            .CompareTo(D2) == -1);
                    assertTrue("Assert 5: compareTo() " + d2 + " should be greater " + d1, D2
                            .CompareTo(objDouble) == 1);
                }
            }

            //try
            //{
            //    new Double(0.0D).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Double(0.0D).CompareTo(null));
        }

        /**
         * @tests java.lang.Double#compareTo(Object)
         * @tests java.lang.Double#compare(double, double)
         */
        [Test]
        public void Test_compareTo_Object()
        {
            // A selection of double values in ascending order.
            double[] values = new double[] { double.NegativeInfinity, -double.MaxValue, -2d,
                -double.Epsilon, -0d, 0d, double.Epsilon, 2d, double.MaxValue,
                double.PositiveInfinity, double.NaN }; // J2N NOTE: MIN_VALUE in Java is the same as Epsilon in .NET
            for (int i = 0; i < values.Length; i++)
            {
                double d1 = values[i];

                // Test that each value compares equal to itself; and each object is
                // equal to another object like itself.
                assertTrue("Assert 0: compare() should be equal: " + d1,
                        Double.Compare(d1, d1) == 0);
                Double objDouble = new Double(d1);
                assertTrue("Assert 1: compareTo() should be equal: " + d1, objDouble
                        .CompareTo((object)objDouble) == 0);

                // Test that the Double-defined order is respected
                for (int j = i + 1; j < values.Length; j++)
                {
                    double d2 = values[j];
                    assertTrue("Assert 2: compare() " + d1 + " should be less " + d2, Double
                            .Compare(d1, d2) == -1);
                    assertTrue("Assert 3: compare() " + d2 + " should be greater " + d1, Double
                            .Compare(d2, d1) == 1);
                    Double D2 = new Double(d2);
                    assertTrue("Assert 4: compareTo() " + d1 + " should be less " + d2, objDouble
                            .CompareTo((object)D2) == -1);
                    assertTrue("Assert 5: compareTo() " + d2 + " should be greater " + d1, D2
                            .CompareTo((object)objDouble) == 1);
                }
            }

            //try
            //{
            //    new Double(0.0D).CompareTo(null);
            //    fail("No NPE");
            //}
            //catch (ArgumentNullException e)
            //{
            //}

            // J2N: Return 1 when comparing to null to match other .NET classes
            assertEquals(1, new Double(0.0D).CompareTo((object)null));

            // J2N: Check to ensure exception is thrown when there is a type mismatch
            Assert.Throws<ArgumentException>(() => new Double(0.0D).CompareTo((object)4));
        }

        /**
         * @tests java.lang.Double#equals(java.lang.Object)
         */
        [Test]
        public void Test_equalsLjava_lang_Object()
        {
            Double d1 = new Double(87654321.12345d);
            Double d2 = new Double(87654321.12345d);
            Double d3 = new Double(0.0002f);
            assertTrue("Assert 0: Equality test failed", d1.Equals((object)d2) && !(d1.Equals((object)d3)));

#pragma warning disable CS1718, CA2242 // Comparison made to same variable, Test for NaN correctly
            assertTrue("Assert 2: NaN should not be == NaN", double.NaN != double.NaN);
#pragma warning restore CS1718, CA2242 // Comparison made to same variable, Test for NaN correctly
            assertTrue("Assert 3: NaN should be == NaN", new Double(double.NaN)
                    .Equals((object)new Double(double.NaN)));
            assertTrue("Assert 4: -0d should be == 0d", 0d == -0d);
            assertTrue("Assert 5: -0d should not be equals() 0d", !new Double(0d)
                    .Equals((object)new Double(-0d)));

            Double dmax = new Double(double.MaxValue);
            Double dmax1 = new Double(double.MaxValue);

            assertTrue("Equality test failed", dmax.Equals((object)dmax1) && !(dmax.Equals(new Object())));
        }

        /**
         * @tests java.lang.Double#equals(Double)
         */
        [Test]
        public void Test_equals_Double()
        {
            Double d1 = new Double(87654321.12345d);
            Double d2 = new Double(87654321.12345d);
            Double d3 = new Double(0.0002f);
            assertTrue("Assert 0: Equality test failed", d1.Equals(d2) && !(d1.Equals(d3)));

#pragma warning disable CS1718, CA2242 // Comparison made to same variable, Test for NaN correctly
            assertTrue("Assert 2: NaN should not be == NaN", double.NaN != double.NaN);
#pragma warning restore CS1718, CA2242 // Comparison made to same variable, Test for NaN correctly
            assertTrue("Assert 3: NaN should be == NaN", new Double(double.NaN)
                    .Equals(new Double(double.NaN)));
            assertTrue("Assert 4: -0d should be == 0d", 0d == -0d);
            assertTrue("Assert 5: -0d should not be equals() 0d", !new Double(0d)
                    .Equals(new Double(-0d)));

            Double dmax = new Double(double.MaxValue);
            Double dmax1 = new Double(double.MaxValue);

            assertTrue("Equality test failed", dmax.Equals(dmax1) && !(dmax.Equals(new Object())));
        }

        /**
         * @tests java.lang.Double#toHexString(double)
         */
        [Test]
        public void Test_toHexStringF()
        {
            // the follow values come from the Double Javadoc/Spec
            assertEquals("0x0.0p0", Double.GetInstance(0.0D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("-0x0.0p0", Double.GetInstance(-0.0D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.0p0", Double.GetInstance(1.0D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("-0x1.0p0", Double.GetInstance(-1.0D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.0p1", Double.GetInstance(2.0D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.8p1", Double.GetInstance(3.0D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.0p-1", Double.GetInstance(0.5D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.0p-2", Double.GetInstance(0.25D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.fffffffffffffp1023", Double.GetInstance(double.MaxValue).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x0.0000000000001p-1022", Double.GetInstance(double.Epsilon).ToHexString(NumberFormatInfo.InvariantInfo)); // J2N: In .NET double.Epsilon is the same as Double.MIN_VALUE in Java

            // test edge cases
            assertEquals("NaN", Double.GetInstance(double.NaN).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("-Infinity", Double.GetInstance(double.NegativeInfinity).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("Infinity", Double.GetInstance(double.PositiveInfinity).ToHexString(NumberFormatInfo.InvariantInfo));

            // test various numbers
            assertEquals("-0x1.da8p6", Double.GetInstance(-118.625D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.2957874cccccdp23", Double.GetInstance(9743299.65D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.2957874cccccdp23", Double.GetInstance(9743299.65000D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.2957874cccf63p23", Double.GetInstance(9743299.650001234D).ToHexString(NumberFormatInfo.InvariantInfo));
            assertEquals("0x1.700d1061d3333p33", Double.GetInstance(12349743299.65000D).ToHexString(NumberFormatInfo.InvariantInfo));

            // test HARMONY-2132
            assertEquals("0x1.01p10", Double.GetInstance(/*0x1.01p10*/ 1028.0D).ToHexString(NumberFormatInfo.InvariantInfo)); // .NET cannot represent this as a float literal

            // J2N: Test custom cultures

            assertEquals("-0x1,0p0", Double.GetInstance(-1.0D).ToHexString(new CultureInfo("fr-FR")));

            // test edge cases
            assertEquals("0x0,0p0", Double.GetInstance(0.0D).ToHexString(new NumberFormatInfo { NegativeSign = "neg", NumberDecimalSeparator = "," }));
            assertEquals("neg0x0,0p0", Double.GetInstance(-0.0D).ToHexString(new NumberFormatInfo { NegativeSign = "neg", NumberDecimalSeparator = "," }));

            assertEquals("NotANumber", Double.GetInstance(double.NaN).ToHexString(new NumberFormatInfo { NaNSymbol = "NotANumber" }));
            assertEquals("-∞", Double.GetInstance(double.NegativeInfinity).ToHexString(new NumberFormatInfo { NegativeInfinitySymbol = "-∞" }));
            assertEquals("∞", Double.GetInstance(double.PositiveInfinity).ToHexString(new NumberFormatInfo { PositiveInfinitySymbol = "∞" }));
        }

        /**
         * @tests java.lang.Double#valueOf(double)
         */
        [Test]
        public void Test_valueOfD()
        {
            assertEquals(new Double(double.Epsilon), Double.GetInstance(double.Epsilon)); // J2N: In .NET double.Epsilon is the same as Double.MIN_VALUE in Java
            assertEquals(new Double(double.MaxValue), Double.GetInstance(double.MaxValue));
            assertEquals(new Double(0), Double.GetInstance(0));

            int s = -128;
            while (s < 128)
            {
                assertEquals(new Double(s), Double.GetInstance(s));
                assertEquals(new Double(s + 0.1D), Double.GetInstance(s + 0.1D));
                s++;
            }
        }

        public class CharSequences : TestCase
        {
            public abstract class ParseTestCase : TestCase
            {
                #region Static_Test_Data

                /*
                 * A String, double pair
                 */
                public class PairSD
                {
                    public string s;
                    public double d;
                    public PairSD(string s, double d)
                    {
                        this.s = s;
                        this.d = d;
                    }
                }

                private static string UpperCaseHex(string s)
                {
                    return s.Replace('a', 'A').Replace('b', 'B').Replace('c', 'C').
                             Replace('d', 'D').Replace('e', 'E').Replace('f', 'F');
                }

                [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "This turned out to be too slow - run manually")]
                private static IEnumerable<TestCaseData> TestCases(string input, double expected)
                {
                    // Try different combination of letter components
                    input = input.ToLower(Locale_US);

                    string[] suffices = { "", "f", "F", "d", "D" };
                    string[] signs = { "", "-", "+" };

                    for (int i = 0; i < 2; i++)
                    {
                        string s1 = input;
                        if (i == 1)
                            s1 = s1.Replace('x', 'X');

                        for (int j = 0; j < 2; j++)
                        {
                            string s2 = s1;
                            if (j == 1)
                                s2 = s2.Replace('p', 'P');

                            for (int k = 0; k < 2; k++)
                            {
                                string s3 = s2;
                                if (k == 1)
                                    s3 = UpperCaseHex(s3);


                                for (int m = 0; m < suffices.Length; m++)
                                {
                                    string s4 = s3 + suffices[m];


                                    for (int n = 0; n < signs.Length; n++)
                                    {
                                        string s5 = signs[n] + s4;

                                        //double result = Double.parseDouble(s5);
                                        //failures += test("Double.parseDouble",
                                        //                 s5, result, (signs[n].equals("-") ?
                                        //                              -expected :
                                        //                              expected));

                                        yield return new TestCaseData(signs[n].Equals("-") ? -expected : expected, s5, NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo);
                                    }
                                }
                            }
                        }
                    }

                }

                private static readonly string[] badStrings = {
                    "",
                    "+",
                    "-",
                    "+e",
                    "-e",
                    "+e170",
                    "-e170",

                    // Make sure intermediate white space is not deleted.
                    "1234   e10",
                    "-1234   e10",

                    // Control characters in the interior of a string are not legal
                    "1\u0007e1",
                    "1e\u00071",

                    // NaN and infinity can't have trailing type suffices or exponents
                    "NaNf",
                    "NaNF",
                    "NaNd",
                    "NaND",
                    "-NaNf",
                    "-NaNF",
                    "-NaNd",
                    "-NaND",
                    "+NaNf",
                    "+NaNF",
                    "+NaNd",
                    "+NaND",
                    "Infinityf",
                    "InfinityF",
                    "Infinityd",
                    "InfinityD",
                    "-Infinityf",
                    "-InfinityF",
                    "-Infinityd",
                    "-InfinityD",
                    "+Infinityf",
                    "+InfinityF",
                    "+Infinityd",
                    "+InfinityD",

                    "NaNe10",
                    "-NaNe10",
                    "+NaNe10",
                    "Infinitye10",
                    "-Infinitye10",
                    "+Infinitye10",

                    // Non-ASCII digits are not recognized
                    "\u0661e\u0661", // 1e1 in Arabic-Indic digits
                    "\u06F1e\u06F1", // 1e1 in Extended Arabic-Indic digits
                    "\u0967e\u0967", // 1e1 in Devanagari digits

                    // JCK test lex03592m3
                    ".",

                    // JCK test lex03592m4
                    "e42",

                    // JCK test lex03592m5
                    ".e42",

                    // JCK test lex03592m6
                    "d",

                    // JCK test lex03592m7
                    ".d",

                    // JCK test lex03592m8
                    "e42d",

                    // JCK test lex03592m9
                    ".e42d",

                    // JCK test lex03593m10
                    "1A01.01125e-10d",

                    // JCK test lex03593m11
                    "2;3.01125e-10d",

                    // JCK test lex03593m12
                    "1_34.01125e-10d",

                    // JCK test lex03593m14
                    "202..01125e-10d",

                    // JCK test lex03593m15
                    "202,01125e-10d",

                    // JCK test lex03593m16
                    "202.03b4e-10d",

                    // JCK test lex03593m18
                    "202.06_3e-10d",

                    // JCK test lex03593m20
                    "202.01125e-f0d",

                    // JCK test lex03593m21
                    "202.01125e_3d",

                    // JCK test lex03593m22
                    "202.01125e -5d",

                    // JCK test lex03593m24
                    "202.01125e-10r",

                    // JCK test lex03593m25
                    "202.01125e-10ff",

                    // JCK test lex03593m26
                    "1234L.01",

                    // JCK test lex03593m27
                    "12ee-2",

                    // JCK test lex03593m28
                    "12e-2.2.2",

                    // JCK test lex03593m29
                    "12.01e+",

                    // JCK test lex03593m30
                    "12.01E",

                    // Bad hexadecimal-style strings

                    // Two leading zeros
                    "00x1.0p1",

                    // Must have hex specifier
                    "1.0p1",
                    "00010p1",
                    "deadbeefp1",

                    // Need an explicit fully-formed exponent
                    "0x1.0p",
                    //"0x1.0", // J2N: Exponent is optional, but must be fully formed if supplied

                    // Exponent must be in decimal
                    "0x1.0pa",
                    "0x1.0pf",

                    //// Exponent separated by "p"
                    //"0x1.0e22", // J2N: Exponent is optional, this will be interpreted as a hex value with an implicit "p1"
                    //"0x1.0e22", // J2N: Exponent is optional, this will be interpreted as a hex value with an implicit "p1"

                    // Need a signifcand
                    "0xp22"
                };

                private static readonly string[] badHexStrings = {
                    "",
                    "+",
                    "-",
                    "+p",
                    "-p",
                    "+p170",
                    "-p170",

                    // Make sure intermediate white space is not deleted.
                    "1234   p10",
                    "-1234   p10",

                    // Control characters in the interior of a string are not legal
                    "1\u0007p1",
                    "1p\u00071",

                    // NaN and infinity can't have trailing type suffices or exponents
                    "NaNf",
                    "NaNF",
                    "NaNd",
                    "NaND",
                    "-NaNf",
                    "-NaNF",
                    "-NaNd",
                    "-NaND",
                    "+NaNf",
                    "+NaNF",
                    "+NaNd",
                    "+NaND",
                    "Infinityf",
                    "InfinityF",
                    "Infinityd",
                    "InfinityD",
                    "-Infinityf",
                    "-InfinityF",
                    "-Infinityd",
                    "-InfinityD",
                    "+Infinityf",
                    "+InfinityF",
                    "+Infinityd",
                    "+InfinityD",

                    "NaNe10",
                    "-NaNe10",
                    "+NaNe10",
                    "Infinitye10",
                    "-Infinitye10",
                    "+Infinitye10",

                    // Non-ASCII digits are not recognized
                    "\u0661e\u0661", // 1e1 in Arabic-Indic digits
                    "\u06F1e\u06F1", // 1e1 in Extended Arabic-Indic digits
                    "\u0967e\u0967", // 1e1 in Devanagari digits

                    // JCK test lex03592m3
                    ".",

                    // JCK test lex03592m4
                    "p42",

                    // JCK test lex03592m5
                    ".p42",

                    //// JCK test lex03592m6
                    //"d", // J2N: Since exponent is optional, this is a valid hex float

                    //// JCK test lex03592m7
                    //".d", // J2N: Since exponent is optional, this is a valid hex float

                    // JCK test lex03592m8
                    "p42d",

                    // JCK test lex03592m9
                    ".p42d",

                    // JCK test lex03593m10
                    //"1A01.01125p-10d", // J2N: This test was meant to hit Java's standard float parser, but it is a valid hex float string

                    // JCK test lex03593m11
                    "2;3.01125p-10d",

                    // JCK test lex03593m12
                    "1_34.01125p-10d",

                    // JCK test lex03593m14
                    "202..01125p-10d",

                    // JCK test lex03593m15
                    "202,01125p-10d",

                    // JCK test lex03593m16
                    //"202.03b4p-10d", // J2N: This test was meant to hit Java's standard float parser, but it is a valid hex float string

                    // JCK test lex03593m18
                    "202.06_3p-10d",

                    // JCK test lex03593m20
                    "202.01125p-f0d",

                    // JCK test lex03593m21
                    "202.01125p_3d",

                    // JCK test lex03593m22
                    "202.01125p -5d",

                    // JCK test lex03593m24
                    "202.01125p-10r",

                    // JCK test lex03593m25
                    "202.01125p-10ff",

                    // JCK test lex03593m26
                    "1234L.01",

                    // JCK test lex03593m27
                    "12pp-2",

                    // JCK test lex03593m28
                    "12p-2.2.2",

                    // JCK test lex03593m29
                    "12.01p+",

                    // JCK test lex03593m30
                    "12.01P",

                    // Bad hexadecimal-style strings

                    // Two leading zeros
                    "00x1.0p1",

                    //// Must have hex specifier // J2N: hex specifier is optional
                    //"1.0p1",
                    //"00010p1",
                    //"deadbeefp1",

                    // Need an explicit fully-formed exponent
                    "0x1.0p",
                    //"0x1.0", // J2N: Exponent is optional, but must be fully formed if supplied

                    // Exponent must be in decimal
                    "0x1.0pa",
                    "0x1.0pf",

                    //// Exponent separated by "p"
                    //"0x1.0e22", // J2N: Exponent is optional, this will be interpreted as a hex value with an implicit "p1"
                    //"0x1.0e22", // J2N: Exponent is optional, this will be interpreted as a hex value with an implicit "p1"

                    // Need a signifcand
                    "0xp22"
                };

                private static readonly string[] goodStrings = {
                    "NaN",
                    "+NaN",
                    "-NaN",
                    "Infinity",
                    "+Infinity",
                    "-Infinity",
                    "1.1e-23f",
                    ".1e-23f",
                    "1e-23",
                    "1f",
                    "0",
                    "-0",
                    "+0",
                    "00",
                    "00",
                    "-00",
                    "+00",
                    "0000000000",
                    "-0000000000",
                    "+0000000000",
                    "1",
                    "2",
                    "1234",
                    "-1234",
                    "+1234",
                    "2147483647",   // Integer.MAX_VALUE
                    "2147483648",
                    "-2147483648",  // Integer.MIN_VALUE
                    "-2147483649",

                    "16777215",
                    "16777216",     // 2^24
                    "16777217",

                    "-16777215",
                    "-16777216",    // -2^24
                    "-16777217",

                    "9007199254740991",
                    "9007199254740992",     // 2^53
                    "9007199254740993",

                    "-9007199254740991",
                    "-9007199254740992",    // -2^53
                    "-9007199254740993",

                    "9223372036854775807",
                    "9223372036854775808",  // Long.MAX_VALUE
                    "9223372036854775809",

                    "-9223372036854775808",
                    "-9223372036854775809", // Long.MIN_VALUE
                    "-9223372036854775810",

                    // Culled from JCK test lex03591m1
                    "54.07140d",
                    "7.01e-324d",
                    "2147483647.01d",
                    "1.2147483647f",
                    "000000000000000000000000001.F",
                    "1.00000000000000000000000000e-2F",

                    // Culled from JCK test lex03592m2
                    "2.",
                    ".0909",
                    "122112217090.0",
                    "7090e-5",
                    "2.E-20",
                    ".0909e42",
                    "122112217090.0E+100",
                    "7090f",
                    "2.F",
                    ".0909d",
                    "122112217090.0D",
                    "7090e-5f",
                    "2.E-20F",
                    ".0909e42d",
                    "122112217090.0E+100D",

                    // Culled from JCK test lex03594m31 -- unicode escapes
                    "\u0035\u0031\u0034\u0039\u0032\u0033\u0036\u0037\u0038\u0030.1102E-209D",
                    "1290873\u002E12301e100",
                    "1.1E-10\u0066",

                    // Culled from JCK test lex03595m1
                    "0.0E-10",
                    "1E10",

                    // Culled from JCK test lex03691m1
                    "0.f",
                    "1f",
                    "0.F",
                    "1F",
                    "0.12d",
                    "1e-0d",
                    "12.e+1D",
                    "0e-0D",
                    "12.e+01",
                    "1e-01",

                    // Limits

                    "1.7976931348623157E308",     // Double.MAX_VALUE
                    "4.9e-324",                   // Double.MIN_VALUE
                    "2.2250738585072014e-308",    // Double.MIN_NORMAL

                    "2.2250738585072012e-308",    // near Double.MIN_NORMAL
                };

                private static readonly double[] goodStringsExpecteds = new double[] {
                    double.NaN,
                    double.NaN,
                    double.NaN,
                    double.PositiveInfinity,
                    double.PositiveInfinity,
                    double.NegativeInfinity,
                    1.1E-23d,
                    1E-24d,
                    1E-23d,
                    1d,
                    0d,
                    -0d,
                    0d,
                    0d,
                    0d,
                    -0d,
                    0d,
                    0d,
                    -0d,
                    0d,
                    1d,
                    2d,
                    1234d,
                    -1234d,
                    1234d,
                    2147483647d,   // Integer.MAX_VALUE
                    2147483648d,
                    -2147483648d,  // Integer.MIN_VALUE
                    -2147483649d,

                    16777215d,
                    16777216d,     // 2^24
                    16777217d,

                    -16777215d,
                    -16777216d,    // -2^24
                    -16777217d,

                    9007199254740991d,
                    9007199254740992d,     // 2^53
                    9007199254740992d,

                    -9007199254740991d,
                    -9007199254740992d,    // -2^53
                    -9007199254740992d,

                    9.223372036854776E+18d,
                    9.223372036854776E+18d,  // Long.MAX_VALUE
                    9.223372036854776E+18d,

                    -9.223372036854776E+18d,
                    -9.223372036854776E+18d, // Long.MIN_VALUE
                    -9.223372036854776E+18d,

                    // Culled from JCK test lex03591m1
                    54.0714d,
                    5E-324d,
                    2147483647.01d,
                    1.2147483647d,
                    1d,
                    0.01d,

                    // Culled from JCK test lex03592m2
                    2d,
                    0.0909d,
                    122112217090d,
                    0.0709d,
                    2E-20d,
                    9.09E+40d,
                    1.2211221709E+111d,
                    7090d,
                    2d,
                    0.0909d,
                    122112217090d,
                    0.0709d,
                    2E-20d,
                    9.09E+40d,
                    1.2211221709E+111d,

                    // Culled from JCK test lex03594m31 -- unicode escapes
                    5.1492367801102E-200d,
                    1.29087312301E+106d,
                    1.1E-10d,

                    // Culled from JCK test lex03595m1
                    0d,
                    10000000000d,

                    // Culled from JCK test lex03691m1
                    0d,
                    1d,
                    0d,
                    1d,
                    0.12d,
                    1d,
                    120d,
                    0d,
                    120d,
                    0.1d,

                    // Limits

                    1.7976931348623157E+308d,     // Double.MAX_VALUE
                    5E-324d,                      // Double.MIN_VALUE
                    2.2250738585072014E-308d,     // Double.MIN_NORMAL

                    2.2250738585072014E-308d,     // near Double.MIN_NORMAL

                };

                private static readonly string[] goodHexStrings = new string[] {
                    "NaN",
                    "+NaN",
                    "-NaN",
                    "Infinity",
                    "+Infinity",
                    "-Infinity",

                    // Good hex strings
                    // Vary capitalization of separators.

                    "0x1p1",
                    "0X1p1",
                    "0x1P1",
                    "0X1P1",
                    "0x1p1f",
                    "0X1p1f",
                    "0x1P1f",
                    "0X1P1f",
                    "0x1p1F",
                    "0X1p1F",
                    "0x1P1F",
                    "0X1P1F",
                    "0x1p1d",
                    "0X1p1d",
                    "0x1P1d",
                    "0X1P1d",
                    "0x1p1D",
                    "0X1p1D",
                    "0x1P1D",
                    "0X1P1D",

                    "-0x1p1",
                    "-0X1p1",
                    "-0x1P1",
                    "-0X1P1",
                    "-0x1p1f",
                    "-0X1p1f",
                    "-0x1P1f",
                    "-0X1P1f",
                    "-0x1p1F",
                    "-0X1p1F",
                    "-0x1P1F",
                    "-0X1P1F",
                    "-0x1p1d",
                    "-0X1p1d",
                    "-0x1P1d",
                    "-0X1P1d",
                    "-0x1p1D",
                    "-0X1p1D",
                    "-0x1P1D",
                    "-0X1P1D",

                    "0x1p-1",
                    "0X1p-1",
                    "0x1P-1",
                    "0X1P-1",
                    "0x1p-1f",
                    "0X1p-1f",
                    "0x1P-1f",
                    "0X1P-1f",
                    "0x1p-1F",
                    "0X1p-1F",
                    "0x1P-1F",
                    "0X1P-1F",
                    "0x1p-1d",
                    "0X1p-1d",
                    "0x1P-1d",
                    "0X1P-1d",
                    "0x1p-1D",
                    "0X1p-1D",
                    "0x1P-1D",
                    "0X1P-1D",

                    "-0x1p-1",
                    "-0X1p-1",
                    "-0x1P-1",
                    "-0X1P-1",
                    "-0x1p-1f",
                    "-0X1p-1f",
                    "-0x1P-1f",
                    "-0X1P-1f",
                    "-0x1p-1F",
                    "-0X1p-1F",
                    "-0x1P-1F",
                    "-0X1P-1F",
                    "-0x1p-1d",
                    "-0X1p-1d",
                    "-0x1P-1d",
                    "-0X1P-1d",
                    "-0x1p-1D",
                    "-0X1p-1D",
                    "-0x1P-1D",
                    "-0X1P-1D",


                    // Try different significand combinations
                    "0xap1",
                    "0xbp1",
                    "0xcp1",
                    "0xdp1",
                    "0xep1",
                    "0xfp1",

                    "0x1p1",
                    "0x.1p1",
                    "0x1.1p1",

                    "0x001p23",
                    "0x00.1p1",
                    "0x001.1p1",

                    "0x100p1",
                    "0x.100p1",
                    "0x1.100p1",

                    "0x00100p1",
                    "0x00.100p1",
                    "0x001.100p1",
                };

                private static readonly double[] goodHexStringsExpecteds = new double[]
                {
                    double.NaN,
                    double.NaN,
                    double.NaN,
                    double.PositiveInfinity,
                    double.PositiveInfinity,
                    double.NegativeInfinity,

                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,
                    2d,

                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,
                    -2d,

                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,
                    0.5d,

                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,
                    -0.5d,

                    // Try different significand combinations
                    20d,
                    22d,
                    24d,
                    26d,
                    28d,
                    30d,

                    2d,
                    0.125d,
                    2.125d,

                    8388608d,
                    0.125d,
                    2.125d,

                    512d,
                    0.125d,
                    2.125d,

                    512d,
                    0.125d,
                    2.125d,
                };

                // J2N: The .NET parser doesn't remove the \u0001 or \u001f characters, so we are omitting them in tests
                private static readonly string pad = " \t\n\r\f\u000b"; /* " \t\n\r\f\u0001\u000b\u001f"; */

                private static readonly string[] paddedBadStrings = LoadPaddedBadStrings();
                private static readonly string[] paddedBadHexStrings = LoadPaddedBadHexStrings();
                private static readonly string[] paddedGoodStrings = LoadPaddedGoodStrings();
                private static readonly string[] paddedGoodHexStrings = LoadPaddedGoodHexStrings();

                private static string[] LoadPaddedBadStrings()
                {
                    var result = new string[badStrings.Length];
                    for (int i = 0; i < badStrings.Length; i++)
                        result[i] = pad + badStrings[i] + pad;
                    return result;
                }

                private static string[] LoadPaddedBadHexStrings()
                {
                    var result = new string[badHexStrings.Length];
                    for (int i = 0; i < badHexStrings.Length; i++)
                        result[i] = pad + badHexStrings[i] + pad;
                    return result;
                }

                private static string[] LoadPaddedGoodStrings()
                {
                    var result = new string[goodStrings.Length];
                    for (int i = 0; i < goodStrings.Length; i++)
                        result[i] = pad + goodStrings[i] + pad;
                    return result;
                }

                private static string[] LoadPaddedGoodHexStrings()
                {
                    var result = new string[goodHexStrings.Length];
                    for (int i = 0; i < goodHexStrings.Length; i++)
                        result[i] = pad + goodHexStrings[i] + pad;
                    return result;
                }

                #endregion Static_Test_Data

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data
                {
                    get
                    {
                        // JDK 8 (rudimentaryTest())

                        yield return new TestCaseData(double.Epsilon, "" + double.Epsilon, NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        // J2N: Prior to .NET Core 3.x, the round-trip format is required in order to provide enough digits to round-trip, otherwise we get infinity.
                        yield return new TestCaseData(double.MaxValue, double.MaxValue.ToString("R", CultureInfo.InvariantCulture), NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        yield return new TestCaseData((double)10.0, "10", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((double)10.0, "10.0", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((double)10.01, "10.01", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        yield return new TestCaseData((double)-10.0, "-10", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((double)-10.0, "-10.00", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData((double)-10.01, "-10.01", NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);

                        // JDK 8 (testParsing())

                        for (int i = 0; i < goodStrings.Length; i++)
                        {
                            string inputString = goodStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(goodStringsExpecteds[i], inputString, styles, provider);
                        }

                        for (int i = 0; i < paddedGoodStrings.Length; i++)
                        {
                            string inputString = paddedGoodStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(goodStringsExpecteds[i], inputString, styles, provider);
                        }

                        // JDK8 (testSubnormalPowers())

                        double[] expectedsJ1 = new double[] {
                            1.0E-323d,
                            2.0E-323d,
                            4.0E-323d,
                            7.9E-323d,
                            1.58E-322d,
                            3.16E-322d,
                            6.32E-322d,
                            1.265E-321d,
                            2.53E-321d,
                            5.06E-321d,
                            1.0118E-320d,
                            2.0237E-320d,
                            4.0474E-320d,
                            8.0948E-320d,
                            1.61895E-319d,
                            3.2379E-319d,
                            6.47582E-319d,
                            1.295163E-318d,
                            2.590327E-318d,
                            5.180654E-318d,
                            1.0361308E-317d,
                            2.0722615E-317d,
                            4.144523E-317d,
                            8.289046E-317d,
                            1.6578092E-316d,
                            3.31561842E-316d,
                            6.63123685E-316d,
                            1.32624737E-315d,
                            2.652494739E-315d,
                            5.304989477E-315d,
                            1.0609978955E-314d,
                            2.121995791E-314d,
                            4.243991582E-314d,
                            8.4879831639E-314d,
                            1.69759663277E-313d,
                            3.39519326554E-313d,
                            6.7903865311E-313d,
                            1.358077306218E-312d,
                            2.716154612436E-312d,
                            5.43230922487E-312d,
                            1.0864618449742E-311d,
                            2.1729236899484E-311d,
                            4.345847379897E-311d,
                            8.6916947597938E-311d,
                            1.73833895195875E-310d,
                            3.4766779039175E-310d,
                            6.953355807835E-310d,
                            1.390671161567E-309d,
                            2.781342323134002E-309d,
                            5.562684646268003E-309d,
                            1.1125369292536007E-308d,
                            2.2250738585072014E-308d,
                        };

                        string[] lowerBoundsJ1 = new string[] {
                            "7.4109846876186981626485318930233205854758970392148714663837852375101326090531312779794975454245398856969484704316857659638998506553390969459816219401617281718945106978546710679176872575177347315553307795408549809608457500958111373034747658096871009590975442271004757307809711118935784838675653998783503015228055934046593739791790738723868299395818481660169122019456499931289798411362062484498678713572180352209017023903285791732520220528974020802906854021606612375549983402671300035812486479041385743401875520901590172592547146296175134159774938718574737870961645638908718119841271673056017045493004705269590165763776884908267986972573366521765567941072508764337560846003984904972149117463085539556354188641513168478436313080237596295773983001708984375E-324",
                            "1.72922976044436290461799077503877480327770930915013667548954988875236427544573063152854942726572597332928797643406001205824329848624578928739571178603773657344205249616608991584746036008747143736291051522619949555753067502235593203747744535559365689045609365299011100384889325944183497956909859330494840368865463846108718726180845057022359365256909790540394618045398499839676196293178145797163583665001754155154373055774333514042547181234272715206782659383748762209616627939566366750229135117763233401271042882103710402715943341357741979706141523676674388365577173157453675612962967237130706439483677645629043720115479398119291969602671188550786325195835853783454308640675964778268347940747199592298159773496864059783018063853887724690139293670654296875E-323",
                            "3.70549234380934908132426594651166029273794851960743573319189261875506630452656563898974877271226994284847423521584288298194992532766954847299081097008086408594725534892733553395884362875886736577766538977042749048042287504790556865173738290484355047954877211355023786539048555594678924193378269993917515076140279670232968698958953693619341496979092408300845610097282499656448992056810312422493393567860901761045085119516428958662601102644870104014534270108033061877749917013356500179062432395206928717009377604507950862962735731480875670798874693592873689354808228194543590599206358365280085227465023526347950828818884424541339934862866832608827839705362543821687804230019924524860745587315427697781770943207565842392181565401187981478869915008544921875E-323",
                            "7.65801751053932143473681628945743127165842694052203384859657807876047036268823565391214746360535788188684675277940862482936317901051706684418100933816711911095766105444982677018161016610165922260717513885888348032620727509900484188025725800334333765773412903467049158847367014895669776666315091320762864490689911318481468644515170966813305760423457643821747594201050499289994583584074645673153013373579196972826509247000619847902708945466064881630037491556601661214016495160936767036729026950094319348486047049316431783456320511727143052984341033425272291333270338268723420571693140621578842803427715287785765046225694477385435865383258120724910868724415923898154795408707844018045540880451883908748993282628969407610508568495788495056331157684326171875E-323",
                            "1.556306784399926614156191697534897322949938378235123007940594899877127847901157568375694484539153375996359178790654010852418968637621210358656140607433962916097847246549480924262714324078724293626619463703579546001777607520120338833729700820034291201410484287691099903464003933497651481612188733974453563319789174614978468535627605513201234287312188114863551562408586498557085766638603312174472252985015787396389357501969001626382924631108454436861043934453738859886549651456097300752062216059869100611439385938933393624443490072219677817355273713090069495290194558417083080516666705134176357955353098810661393481039314583073627726424040696957076926762522684051088777766083683004415131466724796330683437961471776538047162574684989522211253643035888671875E-322",
                            "3.137316851091915555521211834713205714518129746600962254102469083879289471165825574344653960896388551611708185816080307591384270110760217707132219954668464926102009528758477418751820939015841036358423363338961941940091367540560048125137650859434206072684627056139201392697277770701614891503936019281834960977987701207972468317852474605977091341089649056947159498823658497091268132747660645177110732207888968243515054011905765183343356002393233547323056820248013257231615964046418368182728594279418663137346063718167317306417829193204747346097139072419663903204042998713802400406613834159371388259203865856412650350666554794450011448505605849421409042838736204356956742480835360977154312639270621174552327319157390798920470587063391576521098613739013671875E-322",
                            "6.299336984475893438251252109069822497654512483332640746426217451883612717695161586282572913610858902842406199866932901069314873057038232404084378649137468946110334093176470407730034168890074521822031162609726733816718887581439466707953550938234035815232912593035404371163825445109541711287430589896597756294384754393960467882302212791528805448644570941114375371653802494159632864965775311182387690653635329937766447031779292297264218744962791768247082591836562051921748589227060503044061350718517788189159419276635164670366507435174886403580869791078852719031739879307241040186508092209761448866905399947915164089921035217202778892668736154350073274991163244968692671910338716922632674984362270862290106034528619320667086611820195685140788555145263671875E-322",
                            "1.2623377251243849203711332657783056063927277956795997731073714187892259210753833610158410819039799605303802227968638088025176078949594261797988696038075476986126983222012456385686460628638541492749246761151256317569973927663198303873585351095833695300329483666827810328096920793925395350854419731126123346927178860765936467011201689162632233663754414709448807117314090488296362329402004643192941607545128053326269233071526346525105944230101908210095134135013659641302013839588344772766726863596716038292786130393570859398263863919115164518548331228397230350687133640494118319746296608310541570082308468130920191568429996062708313780994996764207401739296017326192164530769345428813589399674545570237765663465271076364160318661333803902380168437957763671875E-321",
                            "2.5271457784779760734631493755209523196472808903722711700368707659909552196871177657910086629897681010226594284172048461936898490734706320585797330815951493066160281479684428341599313548135475434603677958234315485076484007826715978204848951411033014270522625814412622241963111491557102629988398013585174528192767073509888465269000641904839090093974102246117670608634666476569821258274463307214049441328113500103274805151020454980789395200380141093791237221367854820062544340310913312212057889353112538500039552627442248854058576886995720748483254103033985613997921162867872878865873640512101812513114604496930246525447917753719383557647517983922058667905725488639108248487358852595502849054912168988716778326755990451146782760361020336858928203582763671875E-321",
                            "5.0567618851851583796471815950062457461563870797576139638958694603944138169105865753413438251613443820072178396578869209760343314304930438161414600371703525226226877995028372253425019387129343318312540352400433820089504168153751326867376152041431652210908910109582246069695492886820517188256354578503276890723943498997792461784598547389252802954413477319455397591275818453116739116019380635256265108894084393657285949310008671892156297140936606861183443394076245177583605341756050391102719940865905538914546397095185027765648002822756833208353099852307496140619496207615381997105027704915222297374726877228950356439483761135741523110952560423351372525125141813532995683923385700159329747815645366490619008049725818625119710958415453205816447734832763671875E-321",
                            "1.01159940985995229920152460339768325991745994585282995516138668492013310113575241944420141495044969439763346621392510705407232961445378673312649139483207589546360071025716260077076431065117079085730265140732670490115544488807822024192430553302228928091681478699921493725160255677347346304792267708339481615786296349973600454815794358358080228675292227466130851556558122406210574831509215291340696444026026180765308237627985105714890101022049538395967855739493025892625727344646324548884044043891491539743560086030670585588826854694279058128092791350854517193862646297110400233583335833721463267097951422692990576267555447899785802217562645302210000239563974463320770554795439395286983545337111761494423467495665474973065567354524318943731486797332763671875E-320",
                            "2.02344585254282522167513749119180063052110242160696707270498616268151654002513994326433547981908020679145683071019793696701012255726275143615118217706215718186626457087092035724379254421092550620565714717397143830167625130115963418842539355823823479853226615880599989036089781258401004537864093968011891065911002051925216440878185980295735080117049727759481759487122730312398246262488884603509559114289909754981352814263937973360357708784275401465536680430326587322709971350426872864446692249942663541401587463901641701235184558437323507967572174347948559300348946476100436706539952091333945206544400513621071015923698821427874360430782815059927255668441639762896320296539546785542291140380044551502032386387544787668957280146742050419561564922332763671875E-320",
                            "4.04713873790857106662236326678003537172838737311524130779218511820428341780391499090460360955634123157910355970274359679288570844288068084220056374152231975467159229209843587018984901133043493690236613870726090510271786412732246208142756960867012583376316890241956979657948832420508321004007746487356709966160413455828448413002969224171044783000564728346183575348251946124773589124448223227847284454817676903413441967535843708651292924308727127604674329811993710182878459361987969495571988662045007544717642219643583932527899965923412407646530940342136643513321546834080509652453184606558909085437298695477231895235985568484051476857223154575361766526196970362047419780027761566052906330465910131517250224171303413060740705731177513371221721172332763671875E-320",
                            "8.09452450864006275651681481795650485414295727613178977796658302924981717336146508618513986903086328115439701768783491644463688021411653965429932687044264490028224773455346689608196194556945379829578412177383983870480108977964811786743192170953390790422497438964670960901666934744722953936295051526046347766659236263634912357252535711921664188767594729519587207070510377749524274848366900476522735135873211200277620274079655179233163355357630579882949628575327955903215435385110162757822581486249695551349751731127468395113330780895590207004448472330512811939266747550040655544279649637008836843223095059189553653860559062596405709710103833606230788241707631560349618747004191127074136710637641291547685899738820663844307556900048439274542033672332763671875E-320",
                            "1.618929605010304613630571792030944381897209708216488671831537885134088468447656527674621238797990738030498393365801755574813922375658825727849685312828329519150355861946352894786618781404749152108262008790699770590896754108429942943944062591126147204514858536410098923389103139393152219800869661603425623367656881879247840245751668687422903000301654731866394470515027240999025646296204254973873636497984279794005976887167278120396904217455437484439500226101996447343889387431354549282323767134659071564613970754095237320284192410839945805720283536307265148791157148981960947327932579697908692358794687786614197171109706050821114175415865191667968831672728953956954016680957050249116597470981103611608557250873855165411441259237790291081182658672332763671875E-319",
                            "3.237883913302901289588352412501532174863037669423108059901297049552301970670676565786835742587799557860615776559838283435514391084153169252689190564396459577394618038928365305143463955100356696665629202017331344031730044369360205258345803431471660032699580731300954848363975548690010751530018881758184174569652173110473696022749934638425380623369774736560008997404060967498028389191878963968575439222206416981462690113342524002724385941651051293552601421155333430225237291523843322331326138431477823591142408800030775170625915670728657003151953664260769822494937951845801530895238439819708403389937873241463484205608000027270531106827387907791444918534771598750162812548862768493201518991668028251730299953143924168545708663913273994694463908672332763671875E-319",
                            "6.475792529888094641503913653442707760794693591836346836040815378388728975116716642011264750167417197520850542947911339156915328501141856302368201067532719693883142392892390125857154302491571785780363588470594490913396624891220729887149285112162685689069025121082666698313720367283727814988317322067701276973642755572925407576746466540430335869506014745947238051182128420496033874983228381957979044670650691356376116565693015767379349390042278911778803811262007395987933099708820868429330881025115327644199284891901850871309362190506079398015293920167779169902499557573482698029850160063307825452224244151162058274604587980169364969650433340038397092258856888336580404284674204981371362033041877531973785357684062174814243473264241401921026408672332763671875E-319",
                            "1.2951609763058481345335036135325058932658005436662824388319852036061582984008796794460122765326652476841320075724057450599717203335119230401726222073805239926860191100820439767284534997274001964009832361377120784676729785934941779144756248473544737001807913900646090398213210004471161941904914202686735481781623920497828830684739530344440246361778494764721696158738263326492044846565927217936786255567539240106202969470393999296689276286824734148231208591475355327513324716078775960625340366212390335750313037075644002272676255230060924187741974431981797864717622769028845032299073600550506669576796985970559206412597763885967032695296524204532301439707027467509415587756297077957711048115789576092460756166764338187351313091966176216374151408672332763671875E-318",
                            "2.5903244229399254752997281099089761276384629126315779492877925351407291001792957099357838795645123035482259141276349673485320953003073978600442264086350280392814288516676539050139296386838862320468769907190173372203396108022383877659970175196308839627285691459772937798012189278846030195738107963924803891397586250347635676900725657952460067346323454802270612373850533138484066789731324889894400677361316337605856675279795966355309130080389644621136018151902051190564107948818686145017359336586940351962540541443128305075410041309170613767195335455609835254347869191939569700837520481524904357825942469609353502688584115697562368146588705933520110134603368625855085954699542823910390420281284973213434697784924890212425452329370045845280401408672332763671875E-318",
                            "5.1806513162080801568321771026619165963837876505621689701994071982098707037361277709153270856282064152764137272380934119256528452338983474997874348111440361324722483348388737615848819165968583033386644998816278547256728752197268074690398028641837044878241246578026632597610147827595766703404495486400940710629510910047249369332697913168499709315413374877368444804075072762468110676062120233809629520948870532605164086898599900472548837667519465566945637272755442916665674414298506513801397277336040384386995550178096910680877613467389992926102057502865910033608362037761019037914414243473699734324233436886942095240556819320753039049173069391495727524396050942546426688586034315815749164612275767455382581021245994262573730804177785103092901408672332763671875E-318",
                            "1.03613051027443895198970750881677975338744371264233510120226365243481539108497918928744134977555946387327893534590103010798943451010802467792738516161620523188538873011813134747267864724228024459222395182068488897363394040547036468751253735532893455380152356814534022196806064925095239718737270531353214349093360229446476754196642423600578993253593215027564109664524152010436198448723710921640087208123978922603778910136207768707028252841779107458564875514462226368868807345258147251369473158834240449235905567648034121891812757783828751243915501597378059592129347729403917712068201767371290487320815371442119280344502226567134380854341796307446962303981415575929108156359017299626466653274257355939278347493888202362870287753793263618717901408672332763671875E-317",
                            "2.07226126758170082460268710591795594088557360781457150956690951766247203250771201367925863220103710856455406059008440793883773448354440453382466852261980846916171652338661929010105955840746907310893895548572909597576724617246573256872965149315006276383974577287548801395197899120094185749402820621257761626021058868244931523924531444464737561129952895327955439385422310506372373994046892297301002582474195702601008556611423505175987083190298391241803351997875793273275073207177428726505624921830640578933725602587908544313683046416706267879542389786402358709171319112689715060375776815166471993313979240552473650552393041059897064464679250139349431863152144842694471091904983267247901630598220532907069880439172618563463401653024220649967901408672332763671875E-317",
                            "4.14452278219622456982864630012030831588183339815904432629620124811778531535317766246289319705199239794710431107845116360053433443041716424561923524462701494371437210992359517535782138073784673014236896281581750998003385770645646833116387976879231918391619018233578359791981567510092077810733920801066856179876456145841841063380309486193054696882672255928738098827218627498244725084693255048622833331174629262595467849561854978113904743887336958808280304964702927082087604931015991676777928447823440838329365672467657389157423623682461301150796166164450956943255261879261309756990926910756835005300306978773182390968174670045422431685354157803154370981493603376225196962996915202490771585246146886842652946329741450964649629451486134712467901408672332763671875E-317",
                            "8.28904581142527206028056468852501306587435297884798995975478470902841188104410896003016232675390297671220481205518467492392753432416268366920836868864142789281968328299754694587134502539860204420922897747599433798856708077443793985603233632007683202406907900125637476585548904290087861933396121160685045287587250701035660142291865569649688968388110977130303417710811261481989427265985980551266494828575496382584386435462717923989740065281414093941234210898357194699712668378693117577322535499809041357120645812227155078844904778213971367693303718920548153411423147412404499150221227101937561029272962455214599871799737928016473166126703973130764249218176520443286648705180779072976511494541999594713819078110879115767022085048409962837467901408672332763671875E-317",
                            "1.657809186988336704118440146533442256585939214022588122667195163084966501242597155516470058615772413424240581400865169757071393411165372251638663557667025379103030562914545048689839231472011267234294900679634799400563352691040088290576924942264585770437485663909755710172683577850079430178720521879921423503008839811423298300114977736562957511398988419533434055477996529449478831628571431556553817823377230622562223607264443815741410708069568364207142022765665729934962795274047369378411749603780242394703206091746150458219867087276991500778318824432742546347758918478690877936681827484299013077218273408097434833462864443958574635009403603785984005691542354577409552189548506813947991313133705010456151341673154445371766996242257619087467901408672332763671875E-316",
                            "3.315618398679955700299207501895324156582947046298166376050628547449217127518969674543377710496536644930280781791558574286428673368663580021074316935272790558745155032144125756895248689336313392861038906543705530603976641918232676900524307562778390906498641191477992177346952924970062566669369323318394179933852018032198574615761202070389494597420743304339695331012367065384457640353742333567128463812980699102517897950867895599244751993645876904738957646500282800405463049064755872980590177811722644469868326650784141216969791705403031766948349035457131332220430460611263635509603028249021917173108895313863104756789117475842777572774802865096423518638274022845655359158283962295890950950317115841940815868797705104581256818629952931587467901408672332763671875E-316",
                            "6.631236822063193692660742212619087956576962710849322882817495316177718380071714712597193014258065107942361182572945383345143233283659995559945623690484320918029403970603287173306067605064917644114526918271846993010803220372617854120419072803806001178620952246614465111695491619210028839650666926195339692795538374473749127247053650738042568769464253073952217882081108137254415257804084137588277755792187636062429246638074799166251434564798493985802588893969516941346463556646172880184947034227607448620198567768860122734469640941655112299288409457505908903965773544876409150655445429778467725364890139125394444603441623539611183448305601387717302544531737359382146973095754873259776870224683937504910144923046806423000236463405343556587467901408672332763671875E-316",
                            "1.3262473668829669677383811634066615556564994039951635896351228853634720885177204788704823621781122033966521984135719001462572353113652826637688237200907381636597901847521610006127705436522126146621502941728129917824456377281388208560208603285861221722865574356887410980392569007689961385613262131949230718518911087356850232509638548073348717113551272613177262984218590280994330492704767745630576339750601509982251944012488606300264799707103728147929851388907985223228464571809006894593660747059377056920859050005012085769469339414159273363968530301603464047456459713406700180947130232837359341748452626748457124296746635667147995199367198432959060596318664032455130200970696695187548708773417580830848803031545009059838195752956124806587467901408672332763671875E-315",
                            "2.6524947362362621646829950476961670756541056698156261923418695928548725895388184940920084836827235886014843587261266237697430592773638488793173464221753503073734897601358255671770981099436543151635454988640695767451762691098928917439787664249971662811354818577433302717786723784649826477538452543457012769965656513123052443034808342743961013801725311691627353188493554568474160962506134961715173507667429257821897338761316220568291529991714196472184376378784921786992466602134674923411088172722916273522180014477316011839468736359167595493328771989798574334437832050467282241530499838955142574515577601994582483683356659922221618701490392523442576699892517378601096656720580339043092385870884867482726119248541414333514114332057687306587467901408672332763671875E-315",
                            "5.3049894749428525585722228162751781156493182014565513977553630078376735915810145245350607266919463590111486793512360710167147072093609813104143918263445745948008889109031547003057532425265377161663359082465827466706375318734010335198945786178192544988333307018525086192575033338569556661388833366472576872859147364655456864085147932085185607178073389848527533597043483143433821902108869393884367843501084753501188128258971449104344990560935133120693426358538794914520470662786010981045943024049994706724821943421923863979467530249184239752049255366188794908400576724588446362697239051190709040049827552486833202456576708432368865705736780704409608907040224070893029568220347626754179740065819440786480751682534224880865951490260812306587467901408672332763671875E-315",
                            "1.06099789523560333463506783534332001956397432647384018085823498378032755956654065854211652127103918998304773206014549655106580030733552461726084826346830231696556872124378129665630635076923045181719167270116090865215600574004173170717262030034634309342290283900708653142151652446409017029089595012503705078646129067720265706185827110767634793930769546162327894414143340293353143781314338258222756515168395744859769707254281906176451911699377006417711526318046541169576478784088683096315652726704151573130105801311139568259465118029217528269490222118969236056326066072830774605030717475661841971118327453471334640003016805452663359714229557066343673321335637455476895391219882202176354448455688587393990016550519845975569625806667062306587467901408672332763671875E-314",
                            "2.12199579071823949219075894277492443556205933913021026302363234977344796038341907071933741847472829814691346031018927544985445948013437758969966642513599203193652838155071294990776840380238381221830783645416617662234051084544498841753894517747517838050204237665075787041304890662087937764491118304565961490220092473849883390387185468132533167436161858789928616048343054593191787539725275986899533858503017727576932865244902820320665753976260753011747726237062033679688495026694027326855072132012465305940673517089570976819460293589284105304372155624530118352177044769315431089697674324604107833255327255440337515095896999493252347731215109790211802149926464224644627037218951353020703865235426880609008546286491088164976974439479562306587467901408672332763671875E-314",
                            "4.24399158168351180730214115763813326755822936444295042735442708175968876201717589507377921288210651447464491681027683324743177782573208353457730274847137146187844770216457625641069250986869053302054016396017671256270952105625150183827159493173284895466032145193810054839611367093445779235294164888690474313368019286109118758789902182862329914446946484045130059316742483192869075056547151444253088545172261693011259181226144648609093438530028246199820126075093018699912527511904715787933910942629092771561808948646433793939450644709417259374136022635651882943879002162284744059031588022488639557529326859378343265281657387574430323765186215237948059807108117762980090329217089654709402698794903467039045605758433572543791671705104562306587467901408672332763671875E-314",
                            "8.48798316361405643752490558736455093155056941506843075601601654573217036528468954378266280169686294713010782981045194884258641451692749542433257539514213032176228634339230286941654072200130397462500481897219778444344754147786452867973689444024819010297687960251278590436224319956161462176900258056939499959663872910627589495595335612321923408468515734555532945853541340392223650090190902358960197918510749623879911813188628305185948807637563232575964925751154988740360592482326092710091588563862347702804079811760159428179431346949683567513663756657895412127282916948223369997699415418257703006077326067254354765653178163736786275833128426133420575121471424839651016913213366258086800365913856639899119724702318541301421066236354562306587467901408672332763671875E-314",
                            "1.697596632747514569797043444681738625953524951631939141333919547367713357181971684120042997932637581244103365581080218003289568789931831920384312068848364804152996362584775609542823714626653085783393412899623992820492358232109058236266749345727887239960999590366215661629450225681592828060112444393437551252255580159664530969206202471241110396511654235576338718927139054790932800157478404188374416665187725485617217077113595618339659545852633205328254525103278928821256722423168846554406943806328857565288621537987610696659392751430216183792719224702382470494090746520100621875035070209795829903173324483006377766396219716061498179969012847924365605750198038992992870081205919464841595700151762985619267962590088478816679855298854562306587467901408672332763671875E-313",
                            "3.395193265519732421886149216572305691550460971882131272798555332956705998488977143603596433458540154306288530781150264241351423466409996676286421127516668348106531819075866254745162999479698462425179274904432421572787566400754268972852869149134023699287622850596089804015902037132455559826536817066433653837438994657738413916427936189079484372597931237617950265074334483588351100292053407847202854158541677209091827604963530244647081022282773150832833723807526808983048982304854354243037654291261877290257704990442513233619315560391281416350830160791356587227706405663855125629706379792872083697365321314510423767882302820710921988240781691506255667007651267299676576417191025878351186368627575677059564438365628353847197433423854562306587467901408672332763671875E-313",
                            "6.790386531064168126064360760353439822744333012382515535727826904134691281102988062570703304510345300430658861181290356717475132819366326188090639244853275436013602732058047545149841569185789215708750998914049279077377982738044690446025108755946296617940869371055838088788805660034181023359385562412425859007805823653886179810871403624756232324770485241701173357368725341183187700561203415164859729145249580656041048660663399497261923975143053041841992121216022569306633502068225369620299075261127916740195871895352318307539161178313411881467052032969304820694937723951364133139048998959024591285749314977518515770854469030009769604784319378670035789522557723913043989089161238705370367705579201059940157389916708103908232589673854562306587467901408672332763671875E-313",
                            "1.3580773062153039534420783847915708085132077093383284061586370046490661846331009900504917046613955592679399521981570541669722551525278985211699075479526489611827744558022410125959198708597970722275894446933282994086558815412625533392369587969570842455247362411975334658334612905837631950425083053104410269348539481646181711599758338496109728229115593249867619541957507056372860901099503429800173479118665387549939490772063138002491609880863612823860308916033014089953802541594967400374821917200859995640072205705171928455378852414157672811699495777325201287629400360526382148157734237291329606462517302303534699776798801448607464837871394752997596034552370637139778814433101664359408730379482451825701343293018867604030302902173854562306587467901408672332763671875E-312",
                            "2.7161546124330782351133630023040244609907565255384821113303456331202602976787053576373344530821176177176880843582130911574217388937104303258915947948872917963456028209951135287577912987422333735410181342971750424104920480761787219285058546396819934129860348493814327797426227397444533804556478034488379090030006797630772775177532208238816720037805809266200511911135070486752207302176103459070800979065497001337736374994862615012950981692304732387896942505666997131248140620648451461883867601080324153439824873324811148751058234885846194672164383266036994221498325633676418178195104713955939636816053276955567067788687466285802855304045545501652716524611996463593248465120982515667485455727288953357223715099223186604274443527173854562306587467901408672332763671875E-312",
                            "5.4323092248686267984559322373289317659458541579387895216737628900626485237699140928110199499235617346171843486783251651383207063760754939353349692887565774666712595513808585610815341545071059761678755135048685284141643811460110591070436463251318117479086320657492314075609456380658337512819267997256316731392941429599954902333079947724230703655186241298866296649490197347510900104329303517612055978959160228913330143440461569033869725315186971515970209684934963213836816778755419584901958968839252469039330208564089589342416999829223238393094158243460580089236176179976490238269845667285159697523125226259631803812464795960193636236393846998962957504731248116500187766496744218283638906422901956420268458711631824604762724777173854562306587467901408672332763671875E-312",
                            "1.08646184497397239251410707073787463758560494227394043423605974039474249759523315631583909436064499684161768773185493131001186413408056211542217182764951488073225730121523486257290198660368511814215902719202555004215090472856757334641192296960314484177538264984848286631975914347085944929344847922792192014118810693538319156644175426695058670889947105364197866126200451069028285708635703634694565978746486684064517680331659477075707212560951449772116744043470895379014169094969355830938141704357109100238340879042646470525134529715977325834953708198307751824711877272576634358419327573943599818937269124867761275860019455308975198101090449993583439464969751422314066369248267623515945807814127962546357945936449100605739287277173854562306587467901408672332763671875E-311",
                            "2.17292368994819181785113476474783755956764399523406339837342664317169778803171665038531329309722264360141619345989976090237145112702658755919952162519722914886251999336953287550239912890963415919290197887510294444361983795650050821782703964378307217574442153639560231744708830279941159762396007773863942579570549221415047665266366384636714605359468833494861005079620958512063056917248503868859585978321139594366892754114055293159382187052480406284409812760542759709368873727397228323010507175392822362636362219999760232890569589489485500718672808108002095295663279457776922598718291387260480061765556922084020219955128774006538321830483655982824403385446758033941823574751314433980559610596579974798536920386083652607692412277173854562306587467901408672332763671875E-311",
                            "4.34584737989663066852519015276776340353172210115430932664816044872560836890468363852426169057037793712101320491598942008709062511291863844675422122029265768512304537767812890136139341352153224129438788224125773324655770441236637796065727299214292684368249930948984121970174662145651589428498327476007443710474026277168504682510748300520026474298512289756187282986461973398132599334474104337189625977470445414971642901678846925326732136035538319308995950194686488370078282992252973307155238117464248887432404901913987757621439709036501850486111007927390782237566083828177499079316219013894240547422132516516538108145347411401664569289270067961306331226400771257197337985757408054909787216161483999302894869285352756611598662277173854562306587467901408672332763671875E-311",
                            "8.69169475979350836987330092880761509145987831299480118319762805983342953065061761480215848551668852416020722782816873845652897308470274022186362041048351475764409614629532095307938198274532840549735968897356731085243343732409811744631773968886263617955865485567831902421106325877072448760702966880294445972280980388675418716999512132286650212176599202278839838800144003170271684168925305273849705975769057056181143196808430189661432034001654145358168225062973945691497101521964463275444700001607101937024490265742442807083179948130534550020987407566168156121371692568978652040512074267161761518735283705381573884525784686191917064206842891918270186908308797703708366807769595296768242427291292048311610767083890964619411162277173854562306587467901408672332763671875E-311",
                            "1.738338951958726377256952248088731846731619073667578489629656328204907185414248556735795207540930969823859527365252737519540566902827094377208241879086522890268619768352970505651535912119292073390330330243818646606418490314756159641763867308230205485131096594805527463322969653339914167425112245688868450495894888611689246785977039795819897687932773027324144950427508062714549853837827707147169865972366280338600143787067596718330831829933885797456512774799548860334334738581387443212023623769892808036208660993399352906006660426318599949090740206843722903888982910050580957962903784773696803461361586083111645437286659235772422054041988539832197898272124850596730424451793969780485152849550908146329042562680967380635036162277173854562306587467901408672332763671875E-310",
                            "3.476677903917477457796196558504672521902881558403775232249443372648035650112622147246953925519455204639537136530124464867315906091540735087252001555162865719277040075799847326338731339808810539071519052936742477648768783479448855436028053986918089219481558813280918585126696308265597604753930803306016459543122705057716902923932095122886392639445120677414755173682236181803106193175632510893810185965560726903438144967585929775669631421798349101653201874272698689620010012700233403085181471306464220234577002448713173103853621382694730747230245805398832399424205345013785569807687205786766887346614190838571788542808408334933432033712279835660053320999756956382774539739842718747918973694070140342363906153875120212666286162277173854562306587467901408672332763671875E-310",
                            "6.953355807834979618874685179336553872245406527876168717489017461534292579509369328269271361476503674270892354859867919562866584468968016507339520907315551377293880690693600967713122195187847470433896498322590139733469369808834247024556427344293856688182483250231700828734149618116964479411567918540312477637578337949772215199842205777019382542469815977595975620191692419980218871851242118387090825951949620033114147328622595890347230605527275710046580073218998348191360560937925322831497166379607044631313685359340813499547543295446992343509257002509051390494650214940194793497254047812907055117119400349492074753851906533255451993052862427315764166455021167954862770315940216682786615383108604734433633336263425876728786162277173854562306587467901408672332763671875E-310",
                            "1.3906711615669983941031662421000316572930456466820955687968165639306806438302863690313906233390600613533602791519354828953967941223822579347514559611620922693327561920481108250461903905945921333158651389094285463902870542467605030201613174059045391625584332124133265315949056237819698228726842149008904513826489603733882839751662427085285362348519206577958416513210604896334444229202461333373652105924727406292466152050695928119702428972985128926833336471111597665334061657413309162324128556525892693424787051180596094290935387120951515536067279396729489372635539954793013240876387731865187390658129819371332647175938902929899491911734027610627185857365549591099039231468135212552521898761185533518573087701040037204853786162277173854562306587467901408672332763671875E-309",
                            "2.7813423231339992585345616904327841974300556344710529628926461994851834155889852414403175977218794492059023664838328647736170654733531705027864637020231665325394924380056122815959467327462069058608161170637676112241672887785146596555726667488548461500388029871936394290378869477225165727357390609946088586204312135302104088855302869701817321960617987778683298299248429849042894943904899763346774665870282978811170161494842592578412825707900835360406849266896796299619463850364076841309391336818463991011733782823106655873711074771960561921183324185170365336917319434498650135634655099969748061740150657415013792020112895723187571749096357977250029239186606437387392153772525204291992465517339391086851996430593259861103786162277173854562306587467901408672332763671875E-309",
                            "5.5626846462680009873973525870982892777040756100489677510843054705941889591063829862581715464875182249109865411476276285300576081752949956388564791837453150589529649299206151946954594170494364509507180733724457408919277578420229729263953654347554601249995425367542652239238495956036100724618487531820456730959957198438546587062583754934881241184815550180133061871324079754459796373309776623293019785761394123848578180383135921495833619177732248227553874858467193568190268236265612199279916897403606586185627246108127779039262450073978654691415413762052117265480878393909923925151189836178869403904192333502376081708460881309763731423821018710495716002828720129964097998381305187770933599029647106223409813889699705173603786162277173854562306587467901408672332763671875E-309",
                            "1.11253692925360044451229343804292994382521155612047973274676240128122000461411784758938794440187957763211548904752171560429386935791786459109965101471896121117799099137506210208944847856558955411305219859898020002274486959690395994680407628065566880749210216358755168136957748913657970719140681375569193020471247324711431583477145525401009079633210674983032589015475379565293599232119530343185510025543616413923394218159722579330675206117395073961847926041607988105331877008068682915220968018573891776533414172678170025370365200678014840231879592915815621122607996312732471504184259308597112088232275685677100661085156852482916050773270340176987089530112947515117509687598865154728815866054262536496525448807912595798603786162277173854562306587467901408672332763671875E-308",
                            "2.22507385850720113605740979670913197593481954635164564802342610972482222202107694551652952390813508791414915891303962110687008643869459464552765720740782062174337998814106326732925355228688137214901298112245145188984905722230728525513315575501591439747639798341180199932396254828901710708185069063066665599493827577257201576306269066333264756530000924588831643303777979186961204949739037782970490505108060994073026293712895895000358379996720725430436028407889577179615094551674824347103070260914462157228988025818254518032570701886087211312807951223342628836862232150377566662250398253433597456888442390026549819838548794829220689472168983109969836584681402285424333066033985088644580400103493397042756718644338377048603786162277173854562306587467901408672332763671875E-308",
                        };

                        string[] upperBoundsJ1 = new string[] {
                            "1.23516411460311636044142198217055343091264950653581191106396420625168876817552187966324959090408998094949141173861429432731664177588984949099693699002695469531575178297577851131961454291962245525922179659014249682680762501596852288391246096828118349318292403785007928846349518531559641397792756664639171692046759890077656232986317897873113832326364136100281870032427499885482997352270104140831131189286967253681695039838809652887533700881623368004844756702677687292583305671118833393020810798402309572336459201502650287654245243826958556932958231197624563118269409398181196866402119455093361742488341175449316942939628141513779978287622277536275946568454181273895934743339974841620248529105142565927256981069188614130727188467062660492956638336181640625E-323",
                            "2.22329540628560944879455956790699617564276911176446143991513557125303978271593938339384926362736196570908454112950572978916995519660172908379448658204851845156835320935640132037530617725532041946659923386225649428825372502874334119104242974290613028772926326813014271923429133356807354516026961996350509045684167802139781219375372216171604898187455444980507366058369499793869395234086187453496036140716541056627051071709857375197560661586922062408720562064819837126649950208013900107437459437124157230205626562704770517777641438888525402479324816155724213612884936916726154359523815019168051136479014115808770497291330654724803960917720099565296703823217526293012682538011954714916447352389256618669062565924539505435308939240712788887321949005126953125E-323",
                            "4.19955798965059562550083473937988166510300832222176049761747830125574181179677439085504860907390593522827079991128860071287658203802548826938958576609164596407355606211764693848668944592671634788135410840648448921114592505429297780530236729215602387682194172869026958077588363007302780752495372659773183752958983626264031192153480852768587029909638062740958358110253499610642190997718354078825846043575688662517763135451952819817614582997519451216472172789104136794783239281804033536270756714567852545943961285109010978024433829011659093572057986071923514602115991953816069345767206147317429924460359996527677605994735681146851926177915743623338218332744216331246178127355914461508844998957484724152673735635241288044472440788013045676052570343017578125E-323",
                            "8.15208315638056797891338508232565264402348674313635861302216376126114586995844440577744729996699387426664331747485434256028983572087300664057978413417790098908396176764013817470945598326950820471086385749494047905693032510539225103382224239065581105500729864981052330385906822308293633225432193986618533167508615274512531137709698125962551293354003298261860342214021499244187782524982687329485465849293983874299187262936143709057722425818714228831975394237672736131049817429384300393937351269455243177420630729917491898518018609257926475757524325904322116580578102027995899318253988403616187500423051757965491823401545733990947856698307031739421247351797596407713169306043833954693640292093940935119896075056644853262799443882613559253513813018798828125E-323",
                            "1.605713348984051268573848576821719460186444358496555484383153468127195398628178443562224468175316975234338835260198582625511634308656804338296018087035041103910477317868512064715498905795509191836988335567185245874849912520759079749086199258765538541137801249205103075002543740910275338171305836640309231996607878571009531028822132672350479820242733769303664310421557498511278965579511353830804705460730574297862035517904525487537938111461103784062981837134809934803582973724544834109270540379230024440373969619534453739505188169750461240128457005569119320537502322176355559263227552916213702652348435280841120258215165839679139717739089607971587305389904356560647151663419672941063230878366853357054340753899451983699453450071814586408436298370361328125E-322",
                            "3.186723415676040209938868714000027851754635726862394730545027652129357021892846449531183944532552150849687842285624879364476935781795811686772097434269543113914639600077508559204605520732625934568792235202567641813163672541198789040494149298165453412411944017653204564235817578114238748063053121947690629654806405164003530811047001765126336874020194711387272246836629497045461331688568686833443184683603755144987732027841289044498369482745882894524994722929084332148649286314865901539936918598779586966280647398768377421479527290735530768870322364898713728451350762473074879153174681941408732956199202326592377127842406051055523439820654760435919421466117876866515116378171350913802412050912678200923230111585066244572761462450216640718281269073486328125E-322",
                            "6.348743549060018092668908988356644634891018463594073222868776020133680268422182461469102897247022502080385856336477472842407538728073826383724256128738547133922964164495501548182818750606859420032400034473332433689791192582078207623310049376965283154960229554549407542702365252522165567846547692562453424971203458349991530375496739950678050981575116595554488119666773494113826063906683352838720143129350116839239125047714816158419232225315441115449020494517633126838781911495508036401269675037878712018094002957236224785428205532705669826354053083557902544279047643066513518933068939991798793563900736418094890867096886473808290883983785065364583653618544917478251045807674706859280774396004327888661008826956294766319377487207020749337971210479736328125E-322",
                            "1.2672783815827973858128989537069878201163783937057430207516272756142326761480854485344940802675963204541781884438182659798268744620629855777628573517676555173939613293331487526139245210355326390959615633014862017443046232663837044788941849534564942640056800628341813499635460601338019207413536833791979015603997564721967529504396216321781479196684960363888919865327061488250555528342912684849274060020842840227741911087461870386260957710454557557297072037694730716219047161856792306123935187916076962121720714074171919513325562016645947941321514520876280175934441404253390798492857456092578914779303804601099918345605847319313825772310045675221912117923398998701722904666681418750237499086187627264136566257698751809812609536720628966577351093292236328125E-321",
                            "2.5320864349363885389049150634496345333709314883984144176811266228159619747598198533096616613533844609464573940641593033709991156405741914565437208295552571253972911551003459482052098129852260332814046830097921184949556312827354719120205449849764261610249942775926625413501651298969726486547515116251030196869585777465919527762195169063988335626904647900557783356647637476524014457215371348870381893803828287004747483166955978841944408680732790440993175124048925894979577662579360845569266213672473462328974136308043308969120274984526504171256437395513035439245228926627145357612434488294139157210109940967109973302623769010324895548962566894936569046533107161148666622384694842532150948466554226015087681119183665896799073635747845401056110858917236328125E-321",
                            "5.0617025416435708450889472829349279598800376777837572115401253172194205719832886628599968235249607419310158053048413781533435979975966032141054477851304603414039508066347403393877803968846128216522909224264039519962576473154390067782732650480162899550636227071096249241234032694233141044815471681169132559400762202953823524277793074548402048487344022973895510339288789453070932314960288676912597561369799180558758627325944195753311310621289256208385381296757316252500638664024497924459928265185266462743480980775786087880709700920287616631126283144786545965866803971374654475851588552697259642071722213699130083216659612392347035102267609334365882903752523486042554057820721690095977847227287423516989910842153494070772001833802278270013630390167236328125E-321",
                            "1.01209347550579354574570117219055148128982500565544427992581227060263377664302262819606671478681133039001326277862055277180325627116414267292289016962808667734172701097035291217529215646833863983940634012596276189988616793808460765107787051740960175431408795661435496896698795484759970161351384811005337284463115053929631517308988885517229474208222773120570964304571093406164768030450123332997028896501740967666780915643920629576045114502402187743169793642174096967542760666914772082241252368210852463572494669711271645703888552791809841550865974643333567019109954060869672712329896681503500611794946759163170303044731299156391314208877694213224510618191356135830328928692775385223631644748753818520794370288093150418717858229911144007928669452667236328125E-320",
                            "2.02393991818866646821931405998466885189346748140958139746941174836401721553241015201620077965544184278383662727489338268474104921397310737594758095185816796374439087158411066864832039002809335518776083589260749530040697435116602159757895854262554727192953932842113992207628321065813628394423211070677746734587820755881247503371380507454884325649980273413921872235135701312352439461429792645165891566765624541882825492279873497221512722264628050812738618333007658397627004672695320397803900574262024465230522047582242761350246256534854291390345357640427609125596254239859709185286512939115982551241395850091250742700874672684479872422097863970941766047069021435405878670436882775478939239791686608528403289179972463114609571022128875483758747577667236328125E-320",
                            "4.04763280355441231316653983557290359310075243291785563255661070388678409331118519965646890939270286757148335626743904251061663509959103678199696251631833053654971859281162618159437685714760278588446982742589696210144858717732884949058113459305743830716044207203470982829487372227920944860566863590022565634837232159784479475496163751330194028533495274000623688096264917124727782323389131269503616907293391690314914645551779232512447937789079776951876267714674781257795492684256417028929196986364368468546576803324184992642961664020943191069304123634615693338568854597839782131199745454340946430134294031947411622013161419740656988848538203486376276904824352034556978153925097555989554429877552188543621126963731088506392996606564338435418903827667236328125E-320",
                            "8.09501857428590400306099138674937307551532233593440410273100861493231784886873529493700516886722491714677681425253036216236780687082689559409572564523865568216037403526665720748648979138662164727788781049247589570353181282965450527658548669392122037762224755926184964073205474552135577792854168628712203435336054967590943419745730239080813434300525275174027319818523348749478468047307808518179067588348925987179092952095590703094318368837983229230151566478009026978132468707378610291179789810569056475178686314808069455228392478993120990427221655622991861764514055313799928023026210484790874187920090395659733380637734913853011221701418882517245298620335013232859177120901527117010784810049283348574056802531248339289959847775435264338739216327667236328125E-320",
                            "1.618979011574888738284989448910231204034446214196750104307980443702338535998383548549807768781626901629736373022271300146587015041329861321829325190307930597338168492017671925927071565986465937006472377662563376290769826413430581684859419089564878451854585853371612926560641679200564843657428778706091479036333700583203871308244863214582052245834585277520834583263040211998979839495145163015529968950459994580907449565183213644258059230935790133786702164004677518418806420753622996815680975458978432488442905337775838380399254108937476589143056719599744198616404456745720219806679140545690729703491683123084376897886881902077719687407180240578983342051356335629463575054854386239053245570392745668634928153666282840857093550113177116145379841327667236328125E-319",
                            "3.237933319867485414242770069380818997000274175403369492377739608120552038221403586662022272571435721459853756216307828007287483749824204846668830441876060655582430668999684336283916739682073481563839570889194949731603116674360843999261159929910391280039308048262468851535514088497423375386577998860850030238328991814429727085243129165584529868902705282214449110152073938497982582390819872010231771674682131768364162791358459526585540955131403942899803359058014501300154324846111769864683346755797184514971343383711376230740977368826187786574726847553248872320185259609560803373985000667490440734634868577933663932385175878527136618818702956702459428913398980422672370922760104483138167091079670308756670855936351843991360954788660819758661091327667236328125E-319",
                            "6.475841936452678766158331310321994582931930097816608268517257936956979042667443662886451280151053361120088522604380883728688421166812891896347840945012320772070955022963709156997607087073288570678573957342458096613269697196221368628064641610601416936408752438044180701485258907091140438844876439170367132642319574276881438639239661067589485115038945291601678163930141391495988068182169289999635377123126406143277589243708951291240504403522631561126005749164688467062850133031089315962688089349434688568028219475582451931424423888603610181438067103460258219727746865337241970508596720911089862796921239487632238001381763831425970481641748388949411602637484270009089962658571540971308010132453519589000156260476489850259895764139628226985223591327667236328125E-319",
                            "1.2951659169623065469989453792204345754795241942643085820796294594629833051559523815335309295310288640440558055380526995171490296000790265995705861951284841005048003730891758798424987781855718748908042730248984390376602858239942417885671604971983468249147641217607604401384748544278574565761473319789401337450300739201784861747232724871599395607311425310376136271486276297491999039764868125978442588020014954893104442148409934820550431300305086797578410529378036398588241749401044408158697574536709696674141971659324603332791316928158454971164747615274276914542870076792604304777820161398288706921493981307029386139374939737223638207287839253443315950085654849181925146130194413947647696215201218149487127069556765862796965382841563041438348591327667236328125E-318",
                            "2.5903293635963838877651698755969048098521865632296040925354367909975541069343684120233025325628759199081497120932819218057094045668745014194421903963829881471002101146747858081279749171420579105366980276062036977903269180327384516400885531694747570874625418776734451801183727818653442819594667081027469747066263069051591707963218852479619216591856385347925052486598546109484020982930265797936057009813792052392758147957811901879170285093869997270483220089804732261639024982140954592550716544911259712886369476026808906135525103007268144550618108638902314304173116499703328973316267042372686395170639464945823682415361291548818973658580020982431124644981996007527595513073440159900327068380696615270461068687717317887871104620245432670344598591327667236328125E-318",
                            "5.1806562568645385692976188683498452785975113011601951134470514540666957104912004730028457386265700316363375252037403663828301545004654510591853987988919962402910295978460056646989271950550299818284855367688142152956601824502268713431313385140275776125580973894988146600781686367403179327261054603503606566298187728751205400395191107695658858560946305423022884916823085733468064869261061141851285853401346247392065559576615835996409992680999818216292839210658123987740591447620774961334754485660359745310824484761777511740992675165487523709524830686158389083433609345524778310393160804321481771668930432223412274967333995172009644561164384440406742034774678324218936246959931651805685812711687409512408951924038421938019383095053171928157098591327667236328125E-318",
                            "1.03613100434008479323625168538557262160881607770213771552702807802049789176048645949619321507539582550927131514246572555370716543676473503386718156039100124266726685641884453778408317508809741244120605550940352503063267112852037107492169092031332186627492084131495536199977603464902652342593829648455880204762037048150432785259135618127738142499126145573218549777272164981436152641922651829681743540576454637390680382814223704230889407855259460107912077452364907439943724378580415698902830367158559810159734502231714722951927819481926282027338274780670538641954595037167676984546948328219072524665512366778589460071279402418390986366333111356357976814360042957601617714732914635616403301373668997996304718396680630038315940044668650443782098591327667236328125E-317",
                            "2.07226176164734666584923128248674880910694597287437412389167394324815453318321928388801049750087347020054644038664910338455546541020111488976446492139460447994359464968733248041246408625328624095792105917444773203276597689551573895613880505813445007631314304604510315398369437659901598373259379738360427481689735686948887554987024638991896710375485825873609879498170323477372328187245833205342658914926671417387910029289439440699848238203778743891150553935778474344349990240499697174038982130154959939857554537171589145373798108114803798662965162969694837758996566420453474332854523376014254030658676235888943830279170216911153669976670565188260446373530772224366980650278880603237838278697632174964096251341965046238909053943899607475032098591327667236328125E-317",
                            "4.14452327626187041107519047668910118410320576321884694062096567370346781602868493267164506235182875958309669087501585904625206535707387460155903164340181095449625023622430836566922590858366389799135106650453614603703258842950647471857303333377670649638958745550539873795153106049899490434590479918169522035545132964545797094442802680720213846128205186474392538939966640469244679277892195956664489663627104977382369322239870913637765898900817311457627506902605608153162521964338260124311285656147760199253194607051337990217538685380558831934218939347743435993080509187025069029469673471604617042645003974109652570694951845896679037197345472852065385491872230757897706521370812538480708233345558528899679317232533878640095281742361521537532098591327667236328125E-317",
                            "8.28904630549091790152710886509380593409572534390779257407954913461409438171961623023891419205373933834819719185174937036964526525081939402514816508741622390360156140929826013618274955324441921205821108116471297404556581149748794624344148988506121933654247627442598990588720442829895274557252680277787711143255927519739616173354358764176848117633643907675957857823559274452989381459184921459308151161027972097371287908140733859513601220294894446590581412836259875770787585412015386024855892708133360718044474746810835679905019839912068898476726492103840632461248394720168258422699973662785343066617659450551070051526515103867729771638695288179675263728555147824959158263554676408966448142641411236770845449013671543442467737339285349662532098591327667236328125E-317",
                            "1.657809236394901288243094564190321543408076450528568384099671605643534751310147882537345245145756049587839819380521639301643166503831043287232643197544504980181218375544616367720979684256592984019193111048506663006263225763345088929317840298763024501684825391226717224175855116389886842802577080997024089358677516630127254331177470931090116660644521350079088495590744542420478785821770372464595474155829706337349125079942459751265271863083048716856489224703568411006037712307369637825945106812104561755627035026329831059279982148975089031561741597616035025397584165786454637209160574045146795114562970403433905013189641619809831240521394918834895020201920981959082061747922404149937927961233116652513177712575946873047212648533133005912532098591327667236328125E-316",
                            "3.315618448086520284423861919552203443405084282804146637483104990007785377586520401564252897026520281093880019771215043831000446461329251056668296575150270159823342844774197075926389142120895109645937116912577394209676514990537677539265222919276829637745980918794953691350124463509869979293225882435496845789520694850902530646823695264916653746666276234885349771125115078355457594546941274475170120145433174817304799423545911534768613148659357257388304848438185481476537966098078141428123535020046963830792155585367821818029906767101129297731771808640423811270255707919027394782081774809869699210453592309199574936515894651694034178286794180145334533148652650227327868716657859631880887598416527483997842239700497532256702470920828318412532098591327667236328125E-316",
                            "6.631236871469758276785396630275967243399099947355303144249971758736286630139265439618068200788048744105960420552601852889715006376325666595539603330361800519107591783233358492337208057849499360899425128640718856616503093444922854759159988160304439909868291973931426625698663157749836252274523485312442358651207051292453083278116143932569727918709786004497872322193856150225415211997283078496319412124640111777216148110752815101775295719811974338451936095907419622417538473679495148632480391435931767981122396703443803335529756003353209830071832230689201383015598792184172909927924176339315507402234836120730914783168400715462440053817592702766213559042115986763819482654128770595766806872783349146967171293949598850675682115696218943412532098591327667236328125E-316",
                            "1.3262473718236234261508466051723494843387131276457616157783705296193289135244755515725698808311105670130121222115375471007144126206318497673282216840784861237676089660151681325158845889306707863406401152097001781430156250353693209198949518642359660454112914084204372494395740546229768798237118691066333384374579764175554188540701041267875876262796805543722917424331338293965330446897966686538617996083053985697038845485166622235788660862117208500579198590845887904299539488842329163041194104267701376281782878939595766370529454475857370894751953074786756526506284960714463940219608979398207123785797323743793594476473412842999251804879189748007971610829042659836802710529070592523538645421516992472905829402447801487513641405247000193412532098591327667236328125E-315",
                            "2.6524947411769186230954604894618550043363193934662242184851172371107294145455735667940960023357219522178442825240922707242002365866304159828767443861630982674813085413988326990802121552221124868420353199009567631057462564171233918078528579606470101542602158304750264231789895323189633890162309102574115435821325189941756399065870835938488172950970844622173007628606302581445160916699333902623215163999881733536684240233994236503815391146727676824833723580722824468063541519167997191858621529931240592883103843411899692440528851420865693024112194762981866813487657297775046000802978585515990356552922298989918953863083437098072875307002383838491487714402896005982769166278954236379082322518984279124783145619444206761189559984348562693412532098591327667236328125E-315",
                            "5.3049894798835090169846882580408660443315319251071494238986106520935304165877695972371482453449447226275086031492017179711718845186275484139737897903323225549087076921661618322088672878049958878448257292834699330312075191806315335837686701534690983719580646745842047706578204877109364074012689925589679538714816041474160820116210425279712766327318922779073188037156231156404821856302068334792409499833537229215975029731649465039868851715948613473342773560476697595591545579819333249493476381258319026085745772356507544580527645310882337282832678139372087387450401971896210121969717797751556822087172249482169672636303485608220122311248772019458519921550602698274702077778721524090169676713918852428537778053437017308541397142551687693412532098591327667236328125E-315",
                            "1.06099789572966898047631437951988881243219569883889998347255974820591324206721616581232527313633902634468372443994206124651151803826218132761678805986707711297635059937008200984661775529707626898504065480484962728821300447076478171356002945391132748073537623628025614656154823984948824441713451571620807744501797744538969662216889603962161953080015079092873548854256088306324143735507537199130798171500848220574556608726959922111975772854390486770360873519984443850647553701122005364763186083912475892491029630245723248860525233090915625800273644892152528535375891320138538364303196222222689753155672150466671110182743582628514616319741548381392584335846016082858567900778256099512344385103787999036047042921422638403245071458957937693412532098591327667236328125E-314",
                            "2.12199579121230513803200548695149322843028071149527006563795711419903364288409457798954617034002813450854945268998584014530017721106103430005560622153476682794731025967701366309807980833022962938615681855785489525839750957616803842392635433104016276781451577392392748555308062200627745177114974863683064156075761150668587346418247961327060326585407391720474270488455802606162787493918474927807575514835470203291719766717580836256189615131274233364397073438999936360759569943727349595302605489220789625301597346024154657420520408650982202835155578397713410831226870016623194848970153071164955615292671952435673985275623776669103604336727101105260713164436842852026299546777325250356693801883526292251065572657393880592652420091770437693412532098591327667236328125E-314",
                            "4.24399158217757745314338770181470206042645073680801022996875184618527444451785140234398796474740635083628090919007339794287749555665874024493324254487014625788922958029087696960100391439653635018838914606386543119876651978697455184465900408529783334197279484921127016353614538631985586647918021447807576979223687962927822714820964676056857073596192016975675713756855231205840075010740350385161130201504714168726046082698822664544617299685041726552469473277030921380983602428938038056381444299837417090922732777581017474540510759771115356904919445408835175422928827409592507818304066769049487339566671556373679735461384164750281580370698206552996970821618496390361762838775463552045392635443002878681102632129336364971467117357395437693412532098591327667236328125E-314",
                            "8.48798316410812208336615213154111972441879078743349055863034131015775604778536505105287155356216278349174382219024851353803213224785415213468851519154090511777306822151860358260685212652914979179285380107588650307950454020858757868612430359381317449028935299978595551950227491494701269589524114616056602625519541587446293451626398105516450567617761267486078600293654088405194650044384101299868239574843202099594698714661306321121472668792576712928614272953092891421431667399359414978539121921070672022165003640694743108780491462011381665044447179431078704606332742195531133756971894164818550788114670764249691235832904940912637532438640417448469486135981803467032689422771740155422790302561956051541176751073221333729096511888645437693412532098591327667236328125E-314",
                            "1.697596632796921134381168099099395505240347088868445121595352023810271925432039234847063873119167564880266964819059874472834140563024497591419906048488242283754074550397405680861854855079437667500178311109992864684098058105181363236905490261084385678692246930093532623143453397220132635472736300952554653918111248836483234925237264964435637555660899768506884373367251802803903800111671603129282458321520177961332003978586273634275183407007646685680903872305216831502327797340202168822854477163537181884649545366922194377260452866491914281323502647475565762973140571767408385634307548956356677685210669180001714236575946493237349436574524839239414516764708417620374542590764293362177585636799862397261324988960991271244355300951145437693412532098591327667236328125E-313",
                            "3.395193265569138986470273870989962570837283109118637253059987809399264566739044694330617308645070137942452130019129920710895995239502662347322015107156545827707610006888496326064194139932483044141964173114801293436393266273826573973491610064490522138018870190323406765529905208670995367239160673625550756503294663334557117872458998682274011531747176770548495919514447231601322100246246606788110895814874129684806614506436208260582604883437786631185483071009464711664120057221887676511485187648470201609618628819377096914220375675452979513881613583564539879706756230911162889388978858539432931479402666011505760238062029597886773244846293682821304578022161645927058248926749399775687176305275675088701621464736531146274872879076145437693412532098591327667236328125E-313",
                            "6.790386531113574690648485414771096702031155149619021515989259380577249849353055613297724179696875284066822460419270013187019704592458991859126233224493152915614680919870677616468872709638573797425535897124418150940983682611116995446663849671302795056672116710783155050302808831572720830772009418971542961673661492330704883766902466117950759483919730774631719011808838089196158700515396614105767770801582033131755835562136077513197447836298066522194641468417960471987704576985258691888746608618336241059556795724286901988140221293375109978997835455742488113173987549198671896898321477705585439067786659674513852241034195807185620861389831369985084700537068102540425661598719612602706357642227300471582214416287610896335908035326145437693412532098591327667236328125E-313",
                            "1.3580773062202446099004908502333364964418899230619790041847802522933220414581077451231937921800485576315563121219550198139267123298371650882734669459166367091428822745835040197278229849050755303992679345143651865950164515285697838393008328884927340893978609751702651619848616077376171757837706909663527372014395150323000415555789400989304255388264838782798165196397619804385831901053696628741081520774997840025654277673535816018427133742018626304212958263234951992634873616512000722643269450558068319959433129534106512135979912529219370909230279200098384580108450185773689911917006716037890454244554647000530036246978528225783316094476906744312644945566881015767160486942660038256744720316130551237343400319389770396457978347826145437693412532098591327667236328125E-312",
                            "2.7161546124380188915717754677457901489194387392621327093564888807645161545037121127100365406007706160813044442820110568043761960710196968929951541928512795443057106397763765358896944127875118317126966241182119295968526180634859524285697287312176432568591595833541644758940230568983073611969101891047496192695862466307591479133563270732011247196955054799131057565575183234765178302130296658011709020721829453813451161896335293028886505553459745868249591852868935033929211695565484784152315134437532477759185797153745732431659295000907892769695166688810177513977375458923725941954377192702500484598090621652562404258867193062978706560651057492967765435626506842220630137630540889564821445663937052768865772125594089396702118972826145437693412532098591327667236328125E-312",
                            "5.4323092248735674549143447027706974538745363716624401196999061377069043805949208478837220374422147329808007086021231307852751635533847605024385286867205652146313673701621215682134372685523844343395540033259054156005249511333182896071075204166674615917817567997219631037123459552196877320231891853815433834058797098276773606289111010217425230814335486831796842303930310095523871104283496716552964020615492681389044930341934247049805249176341984996322859032136901116517887853672452907170406502196460793358691132393024173023018059944284936490624941666233763381715226005223798002029118146031720545305162570956627140282644522737369487492999358990278006415745758495127569439006302592180974896359550055831910515738002727397190400222826145437693412532098591327667236328125E-312",
                            "1.08646184497446645815994831728205120637847316364630549403867406515916808327773383182310930311251029667797932372423472787470730985181148877213252776744591365552826808309336116328609229800821296395932687617412923876078696172729829639641831037875670982616269512324575603593489917518624484736757471779351309116784666362215137860600206489188253198049096350897128411780640563817041256708589896833635474020402819136540232467233132155091642736422106463252469393390672833281695240169886389153206589237714317424557701802871581054205735589831039023932484491621080935117190927097823942122178600052690160666719306469564756612330199182086151049357695961984898488375984261800941448041757825997413281797750776061958000002962820003398166962722826145437693412532098591327667236328125E-311",
                            "2.17292368994868588349697601129201412836051221660642845817604096793612337371421732589258350184908794343777782945227955746706689684475751421590987756499362792365853077524765917621558944031416200501006982785720663316225589495523123126783342705293663716013173400979287548706222833451479699569808631630423059682236404890091866369222397447129909132518618079027791550734061071260076027917202697067800494019977472046842607541015527971175317710913635419764762462107744697612049944802314261645278954708750030686955723143828694816571170649604547198816203591530775278588142329283024230362477563866007040909547594266781015556425308500783714173087089167974139452296461268412569205247260872807877895600533228074210178977412454555400120087722826145437693412532098591327667236328125E-311",
                            "4.34584737989712473417103139931193997232459032252667438645077477349003395458718431403153189932224323695737484090836921665178607083064956510346457716008905645991905615955625520207458372492606008711155573122336142196519376141109710101066366040129649182806981178288711438931688665317190129235910951332566560813139881945845323386466779363013221001457661535289117828640902086146145570334428297536130534019126777867447357688580319603342667659896693332789348599541888426272759354067170006629423685650821457211751765825742922341302040769151563548583641791350163965530045133653424806843075491492640801395204169861213533444615527138178840420545875579952621380137415281635824719658266966428807123206098132098714536926311723659404026337722826145437693412532098591327667236328125E-311",
                            "8.69169475979400243551914217535179166025274653436716624300024238459785511633311829030942869426855382399656886382054853502122441880243366687857397635027991353244010692817344725379257229414985625131452753795567099957106949432282884049632412709801620116394596732907559219382620329048610988568115590736853563074946836057352237420955543194779844739335748447811770384454584115918284655168879498472790614017425389508656857983709902867677367557862809158838520874410175883594178172596881496597713147534964310261343851189571377390763781008245596248118518190988941339413850742394225959804271346745908322366517321050078569220995964412969092915463448403909585235819323308082335748480279153670665578417227940147723252824110261867411838837722826145437693412532098591327667236328125E-311",
                            "1.738338951958775783821536372743149503610905895804814995609917760681349743982498624286522228416117499807495690964490717176010111474600187042879277473066162767748220846540783135722854943259744857972047115142029015478282096014629231946764506049145561983569827842145254780284483656511452707232524869545427567598560744280366065489933070858313092215091922272857075496081948175462562824837781900346110774014022612791075858573969069396346767353795040810936865424146750798237015809656304476534292071303250016360528021917228287489687261486433661647188270990266496087181461959875828265726663057252443364309143623427808640773756838962549597905298594051823512947183139360975357806124303528154382488839487556245740684619707338283427463837722826145437693412532098591327667236328125E-310",
                            "3.476677903917526864360780683159090178782168380541011738229704805124478208680872214797680946394641734623173300129362444523785450663313827752923037149142505596756641153987659956410050370949263323653235837834952846520632389179321927741028692727833445717920290060620645902088210311437136144561343427162575576645788560726393721627888126185379587166604269922947685719336676294551119164175586704092751094007217059355913859754487402453685566945659504115133554523619900627522691083775150436407449918839821428558896363372542107687534222442809792445327776588821605582716684394839032877571446478265513448194396228183268783879278588061710607884968885347651368369910771466761401921412352277121816309684006788441775548210901491115458713837722826145437693412532098591327667236328125E-310",
                            "6.953355807835029025439269303990971529124693350013405223469278894010735138077619395819998382351690204254528518459105899219336129040741109173010556501295191254773481768881413597784441226328300255015613283220800508605332975508707319329557066085209213186621214497571428145695663621288503019218980542396871594740244193618449033903798236839512577069628965223128906165846132532728231842851196311586031733993605952485589862115524068568363166129388430723526932722566200286094041632012842356153765613912964252955633046283169748083228144355562054041606787785931824573787129264765442101261013320291653615964901437694189070090322086260032627844309467939307079215366035678333490151988449775056683951373045252833845275393289796779521213837722826145437693412532098591327667236328125E-310",
                            "1.3906711615670033347596246545654734229809743288958192193948427071783248996871113757864633254265787143517238955118592808610437485795595672013185595205600562570807162998668920880533222937086374117740368173992495832774734148167478102506613812799960748124023063371472992632910570240991236768534254772865463630929155459402559658455618458147778556875678355823491347058865045009082457200202415526572593013966383738744941866837597400797718364496846283940313689120458799603236742728488226195646397004059249901749106412104425028874615988181066577234164810180152262555928019004618260548640147004343933951505911856716029642512409082656676667762990633122618500906276564101477666613140644770926419234751122181617984729758066408107646213837722826145437693412532098591327667236328125E-309",
                            "2.7813423231340041991910201028982259631179843166847766134906723427328276714458102481953902998093981022042659828437566627392640199305304797693535672614211305202874525458243935446030786358602521843189877955535886481113536493485019668860727306229463817998826761119276121607340383480396704267164803233802647703306977990970780907559258900764310516487777137024216228844902869961790907914904853956545715573911939311263645876281744065256428761231761990373887201916243998237522144921438993874631659784351821199336053143746935590457391675832075623619280854968593138520209798484323897443398414372448494622587932694759710787356583075449964747600352963489241344288097620947766019535445034762665889801507276039186263638487619630763896213837722826145437693412532098591327667236328125E-309",
                            "5.5626846462680059280538109995637310433920042922626914016823316138418332149632079930132442485750368779093501575075514264957045626324723049054235827431432790467009250377393964577025913201634817294088897518622667777791141184120102801568954293088469957748434156614882379556200009959207639264425900155677015848062623054107223405766539785997374435711974699425665992416978519867207809344309730816491960693803050456301053895170037394173849554701593403241034227507814395506092949307340529232602185344936963794509946607031956713622943051134093716389512944545474890448773357443735171232914949108657615964751974370847073077044931061036540907275077624222487031051739734640342725380053814746144830935019583754322821455946726076076396213837722826145437693412532098591327667236328125E-309",
                            "1.11253692925360093857793927928947412039400442434185209780656501560598443019980034826489521461063144293195185068351409540085856480363559551775636137065875760995278700215694022839016166887699408195886936644796230371146350565390269066985408266806482237247648947606094895453919262916829509258948093999425752137573913180380108402181101556463502274160369824228565519561129819678041612203119484536384450933585272746375869932946624052008691141641256228975328278690955190043234558079143599948543236466107248984857733533601998959954045801738129901929977123699238394305900475362557718811948018581075858649080057723021797656421627032209693226624526945688978404579023962025496137069271374713102713202044199184595937090864938966701396213837722826145437693412532098591327667236328125E-308",
                            "2.22507385850720163012305563795567615250361241457301801308322872404958664760675944619203679411688695321398552054903200090343478188441232557218436756334761702051817599892294139362996674259828589999483014897143355557856769327930601597818316214242506796246078529588519927249357768832073249247992481686923224716596493432925878395010225097395757951057160073834364573849432419299709217920738991976169431413149717326525502008499797367678374315520581880443916381057236779117517775622749741380425338708447819365553307386742083452616251302946202273010905482006765402020154711200202813970014157525912344017736224427371246815175018974555997865323425588621961151633592416795802960447706494647018477736093430045142168360701364747951396213837722826145437693412532098591327667236328125E-308",
                        };

                        for (int i = 0; i < expectedsJ1.Length; i++)
                        {
                            double d = expectedsJ1[i];

                            yield return new TestCaseData(d, lowerBoundsJ1[i], NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                            yield return new TestCaseData(d, upperBoundsJ1[i], NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        yield return new TestCaseData(0.0d,
                            "2.4703282292062327208828439643411068618252990130716238221279284125033775363510437593264991818081799618989828234772285886546332835517796989819938739800539093906315035659515570226392290858392449105184435931802849936536152500319370457678249219365623669863658480757001585769269903706311928279558551332927834338409351978015531246597263579574622766465272827220056374006485499977096599470454020828166226237857393450736339007967761930577506740176324673600968951340535537458516661134223766678604162159680461914467291840300530057530849048765391711386591646239524912623653881879636239373280423891018672348497668235089863388587925628302755995657524455507255189313690836254779186948667994968324049705821028513185451396213837722826145437693412532098591327667236328125E-324",
                            NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.Epsilon,
                            "2.47032822925563928955488610223975683825075313028607204245901671443135881701471561216490502500611448891113821261974342300460418902942448352807590938286946753938651115551760560719928058878293671231747736634408280531526902605907773622021915212565490055774142324317176285739934621929266664332301574472636463785506624517729295274366260118660863836029219323513171043552966204750864260101376307037863078272186341055251908512708551508108801689745488069526735449270278409582397426355203982358963233588990525634848244115770970815275109010555867353978342572186166693436117300403071722862355340083843290764615831101299228427291345949708292808225406036401688524974979852948763402190780682189954893639063625933369612197756528003477811220301766530180567324117966872133774348767321082931402997928671538829803466796875E-324",
                            NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.Epsilon,
                            "7.41098468756929159397648975512467060905044292200042324605269693558215132838945942514109170222660535868479308128917093161392894517769431239989958653734617002313950310828862248335763575457404292975629700637705716929919197406867044685108053562059456738772196905962887145030861526323210466850040478844476909750867454602891954589524283064324046822581693765289083916706453794332999718716784526088785971242109396974201651806001925714991467917307498874512022604351143088758269019013746684785203252798227950943843429496250231485958852940059801101485323277394829811510037974782377852068861615556903603175290898302295307116225679182401947018075572183888519047572653597242404075755939116543007094593777514594048443650796980909568006287434734753763085782571486252866225651232678917068597002071328461170196533203125E-324",
                            NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(2 * double.Epsilon,
                            "7.4109846876186981626485318930233205854758970392148714663837852375101326090531312779794975454245398856969484704316857659638998506553390969459816219401617281718945106978546710679176872575177347315553307795408549809608457500958111373034747658096871009590975442271004757307809711118935784838675653998783503015228055934046593739791790738723868299395818481660169122019456499931289798411362062484498678713572180352209017023903285791732520220528974020802906854021606612375549983402671300035812486479041385743401875520901590172592547146296175134159774938718574737870961645638908718119841271673056017045493004705269590165763776884908267986972573366521765567941072508764337560846003984904972149117463085539556354188641513168478436313080237596295773983001708984375E-324",
                            NumberStyle.Float, J2N.Text.StringFormatter.InvariantCulture);


                        // Harmony (Test_parseDoubleLjava_lang_String())

                        yield return new TestCaseData(0.0d, "2.4703282292062327208828439643411e-324", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.Epsilon, "2.4703282292062327208828439643412e-324", NumberStyle.Float, NumberFormatInfo.InvariantInfo); // J2N: In .NET double.Epsilon is the same as Double.MIN_VALUE in Java

                        // Smoke tests (these don't actually test the value)
                        for (int i = 324; i > 0; i--)
                        {
                            yield return new TestCaseData(Double.Parse("3.4e-" + i.ToString(NumberFormatInfo.InvariantInfo), J2N.Text.StringFormatter.InvariantCulture),
                                "3.4e-" + i.ToString(NumberFormatInfo.InvariantInfo), NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }
                        for (int i = 0; i <= 309; i++)
                        {
                            yield return new TestCaseData(Double.Parse("1.2e" + i.ToString(NumberFormatInfo.InvariantInfo), J2N.Text.StringFormatter.InvariantCulture),
                                "1.2e" + i.ToString(NumberFormatInfo.InvariantInfo), NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        // Custom

                        yield return new TestCaseData(1.8d, "1.8e0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1.8d, "1,8e0", NumberStyle.Float, new NumberFormatInfo { NumberDecimalSeparator = "," });
                        yield return new TestCaseData(1.8d, "1--8e0", NumberStyle.Float, new NumberFormatInfo { NumberDecimalSeparator = "--" });
                        yield return new TestCaseData(1.8d, "1.8", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        // Negative sign format tests
                        yield return new TestCaseData(-1.8d, "-1.8e0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8d, "-1.8", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0d, "-1.", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8d, "- 1.8e0", NumberStyle.Float, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-1.8d, "- 1.8", NumberStyle.Float, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-1.0d, "- 1.", NumberStyle.Float, new NumberFormatInfo { NumberNegativePattern = 2 });

                        yield return new TestCaseData(-1.8d, "1.8e0-", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8d, "1.8-", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0d, "1.-", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8d, "1.8e0 -", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8d, "1.8 -", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0d, "1. -", NumberStyle.Float | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(-1.8d, "(1.8e0)", NumberStyle.Float | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.8d, "(1.8)", NumberStyle.Float | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-1.0d, "(1.)", NumberStyle.Float | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);

                        // Constant values

                        yield return new TestCaseData(double.NaN, "NaN", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.NaN, "+NaN", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.NaN, "-NaN", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(double.PositiveInfinity, "Infinity", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.PositiveInfinity, "+Infinity", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.NegativeInfinity, "-Infinity", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                    }
                }

                #endregion

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data
                {
                    get
                    {
                        // JDK 8 (testParsing())

                        for (int i = 0; i < goodHexStrings.Length; i++)
                        {
                            string inputString = goodHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(goodHexStringsExpecteds[i], inputString, styles, provider);
                        }

                        for (int i = 0; i < paddedGoodHexStrings.Length; i++)
                        {
                            string inputString = paddedGoodHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(goodHexStringsExpecteds[i], inputString, styles, provider);
                        }

                        // JDK 8 (ParseHexFloatingPoint.doubleTests())

                        // J2N: These pass, but since there are so many, it causes NUnit to run super slow.
                        // These tests should be run manually. There should be enough Harmony tests to catch
                        // most issues.

                        //// Hex strings that convert to three; test basic functionality
                        //// of significand and exponent shift adjusts along with the
                        //// no-op of adding leading zeros.  These cases don't exercise
                        //// the rounding code.
                        //string leadingZeros = "0x0000000000000000000";
                        //string[] threeTests = {
                        //    "0x.003p12",
                        //    "0x.006p11",
                        //    "0x.00cp10",
                        //    "0x.018p9",

                        //    "0x.3p4",
                        //    "0x.6p3",
                        //    "0x.cp2",
                        //    "0x1.8p1",

                        //    "0x3p0",
                        //    "0x6.0p-1",
                        //    "0xc.0p-2",
                        //    "0x18.0p-3",

                        //    "0x3000000p-24",
                        //    "0x3.0p0",
                        //    "0x3.000000p0",
                        //};

                        //for (int i = 0; i < threeTests.Length; i++)
                        //{
                        //    string input = threeTests[i];
                        //    //yield return new TestCaseData(3.0d, input, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        //    foreach (var test in TestCases(input, 3.0d).ToArray())
                        //        yield return test;

                        //    input = Regex.Replace(input, "^0x", leadingZeros);
                        //    //yield return new TestCaseData(3.0d, input, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        //    foreach (var test in TestCases(input, 3.0d).ToArray())
                        //        yield return test;
                        //}

                        //long[] bigExponents = {
                        //    2 * FloatingDecimal.DoubleConsts.MAX_EXPONENT, //FloatingPointInfo.Double.MaxExponent, //DoubleConsts.MAX_EXPONENT,
                        //    2 * FloatingDecimal.DoubleConsts.MIN_EXPONENT, //FloatingPointInfo.Double.MinExponent, //DoubleConsts.MIN_EXPONENT,

                        //    (long)int.MaxValue-1,
                        //    (long)int.MaxValue,
                        //    (long)int.MaxValue+1,

                        //    (long)int.MinValue-1,
                        //    (long)int.MinValue,
                        //    (long)int.MinValue+1,

                        //    long.MaxValue-1,
                        //    long.MaxValue,

                        //    long.MinValue+1,
                        //    long.MinValue,
                        //};

                        //// Test zero significand with large exponents.
                        //for (int i = 0; i < bigExponents.Length; i++)
                        //{
                        //    foreach (var test in TestCases("0x0.0p" + Convert.ToString(bigExponents[i], 10), 0.0d).ToArray())
                        //        yield return test;
                        //}

                        //// Test nonzero significand with large exponents.
                        //for (int i = 0; i < bigExponents.Length; i++)
                        //{
                        //    long exponent = bigExponents[i];
                        //    foreach (var test in TestCases("0x10000.0p" + Convert.ToString(exponent, 10), exponent < 0 ? 0.0d : double.PositiveInfinity).ToArray())
                        //        yield return test;
                        //}

                        //// Test significands with different lengths and bit patterns.
                        //{
                        //    long signif = 0;
                        //    for (int i = 1; i <= 0xe; i++)
                        //    {
                        //        signif = (signif << 4) | (long)i;
                        //        foreach (var test in TestCases("0x" + signif.ToHexString() + "p0", signif).ToArray())
                        //            yield return test;
                        //    }
                        //}

                        //PairSD[] testCases = {
                        //    new PairSD("0x0.0p0",               0.0/16.0),
                        //    new PairSD("0x0.1p0",               1.0/16.0),
                        //    new PairSD("0x0.2p0",               2.0/16.0),
                        //    new PairSD("0x0.3p0",               3.0/16.0),
                        //    new PairSD("0x0.4p0",               4.0/16.0),
                        //    new PairSD("0x0.5p0",               5.0/16.0),
                        //    new PairSD("0x0.6p0",               6.0/16.0),
                        //    new PairSD("0x0.7p0",               7.0/16.0),
                        //    new PairSD("0x0.8p0",               8.0/16.0),
                        //    new PairSD("0x0.9p0",               9.0/16.0),
                        //    new PairSD("0x0.ap0",               10.0/16.0),
                        //    new PairSD("0x0.bp0",               11.0/16.0),
                        //    new PairSD("0x0.cp0",               12.0/16.0),
                        //    new PairSD("0x0.dp0",               13.0/16.0),
                        //    new PairSD("0x0.ep0",               14.0/16.0),
                        //    new PairSD("0x0.fp0",               15.0/16.0),

                        //    // Half-way case between zero and MIN_VALUE rounds down to
                        //    // zero
                        //    new PairSD("0x1.0p-1075",           0.0),

                        //    // Slighly more than half-way case between zero and
                        //    // MIN_VALUES rounds up to zero.
                        //    new PairSD("0x1.1p-1075",                   double.Epsilon),
                        //    new PairSD("0x1.000000000001p-1075",        double.Epsilon),
                        //    new PairSD("0x1.000000000000001p-1075",     double.Epsilon),

                        //    // More subnormal rounding tests
                        //    new PairSD("0x0.fffffffffffff7fffffp-1022", MathExtensions.NextDown(FloatingDecimal.DoubleConsts.MIN_NORMAL)),
                        //    new PairSD("0x0.fffffffffffff8p-1022",      FloatingDecimal.DoubleConsts.MIN_NORMAL),
                        //    new PairSD("0x0.fffffffffffff800000001p-1022",FloatingDecimal.DoubleConsts.MIN_NORMAL),
                        //    new PairSD("0x0.fffffffffffff80000000000000001p-1022",FloatingDecimal.DoubleConsts.MIN_NORMAL),
                        //    new PairSD("0x1.0p-1022",                   FloatingDecimal.DoubleConsts.MIN_NORMAL),


                        //    // Large value and overflow rounding tests
                        //    new PairSD("0x1.fffffffffffffp1023",        double.MaxValue),
                        //    new PairSD("0x1.fffffffffffff0000000p1023", double.MaxValue),
                        //    new PairSD("0x1.fffffffffffff4p1023",       double.MaxValue),
                        //    new PairSD("0x1.fffffffffffff7fffffp1023",  double.MaxValue),
                        //    new PairSD("0x1.fffffffffffff8p1023",       double.PositiveInfinity),
                        //    new PairSD("0x1.fffffffffffff8000001p1023", double.PositiveInfinity),

                        //    new PairSD("0x1.ffffffffffffep1023",        MathExtensions.NextDown(double.MaxValue)),
                        //    new PairSD("0x1.ffffffffffffe0000p1023",    MathExtensions.NextDown(double.MaxValue)),
                        //    new PairSD("0x1.ffffffffffffe8p1023",       MathExtensions.NextDown(double.MaxValue)),
                        //    new PairSD("0x1.ffffffffffffe7p1023",       MathExtensions.NextDown(double.MaxValue)),
                        //    new PairSD("0x1.ffffffffffffeffffffp1023",  double.MaxValue),
                        //    new PairSD("0x1.ffffffffffffe8000001p1023", double.MaxValue),
                        //};

                        //for (int i = 0; i < testCases.Length; i++)
                        //{
                        //    foreach (var test in TestCases(testCases[i].s, testCases[i].d).ToArray())
                        //        yield return test;
                        //}


                        // Harmony (Test_parseDouble_LString_FromHexString())

                        yield return new TestCaseData(0.0d, "0x0.0p0D", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(5440.0d, "0xa.ap+9d", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(2832.625d, "+0Xb.10ap8", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(-2.5d, "-0X.a0P2D", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(136.25d, "\r 0x22.1p2d \t", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.5d, "0x1.0p-1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.5d, "0x00000000000000000000000000000000001.0p-1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.5d, "0x1.0p-00000000000000000000000000001", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.125d, "0x.100000000000000000000000000000000p1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0x0.0p999999999999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.PositiveInfinity, "0xf1.0p9999999999999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(BitConversion.Int64BitsToDouble(0x4900000000000000L), "0xffffffffffffffffffffffffffffffffffff.ffffffffffffffffffffffffffffffffffffffffffffffp1", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(BitConversion.Int64BitsToDouble(0x7f30000000000000L), "0x0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001p1600", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0x0.0p-999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0xf1.0p-9999999999999999999999999999999999999999999999999999999999999999", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(BitConversion.Int64BitsToDouble(0xf0000000000000L), "0x10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000p-1600", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.PositiveInfinity, "0x1.p9223372036854775807", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.PositiveInfinity, "0x1.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.PositiveInfinity, "0x10.p9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(double.PositiveInfinity, "0xabcd.ffffffffp+2000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0x1.p-9223372036854775808", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0x1.p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0x.1p-9223372036854775809", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        yield return new TestCaseData(0.0d, "0xabcd.ffffffffffffffp-2000", NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);

                        // Harmony (Test_parseDouble_LString_MaxNormalBoundary())

                        long[] expecteds1 = {
                            0x7fefffffffffffffL,               0x7fefffffffffffffL,               0x7fefffffffffffffL,
                            0x7fefffffffffffffL,               0x7fefffffffffffffL,               0x7fefffffffffffffL,
                            0x7fefffffffffffffL,               0x7ff0000000000000L,               0x7ff0000000000000L,
                            0x7ff0000000000000L,               0x7ff0000000000000L,               0x7ff0000000000000L,
                            0x7ff0000000000000L,               0x7ff0000000000000L,               0x7ff0000000000000L,

                            unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),
                            unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),               unchecked((long)0xffefffffffffffffL),
                            unchecked((long)0xffefffffffffffffL),               unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),
                            unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),
                            unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L),               unchecked((long)0xfff0000000000000L)
                        };

                        string[] inputs1 = {
                            "0x1.fffffffffffffp1023",
                            "0x1.fffffffffffff000000000000000000000000001p1023",
                            "0x1.fffffffffffff1p1023",
                            "0x1.fffffffffffff100000000000000000000000001p1023",
                            "0x1.fffffffffffff1fffffffffffffffffffffffffffffffffffffffffffffp1023",
                            "0x1.fffffffffffff7p1023",
                            "0x1.fffffffffffff700000000000000000000000001p1023",
                            "0x1.fffffffffffff8p1023",
                            "0x1.fffffffffffff800000000000000000000000001p1023",
                            "0x1.fffffffffffff8fffffffffffffffffffffffffffffffffffffffffffffp1023",
                            "0x1.fffffffffffff9p1023",
                            "0x1.fffffffffffff900000000000000000000000001p1023",
                            "0x1.ffffffffffffffp1023",
                            "0x1.ffffffffffffff00000000000000000000000001p1023",
                            "0x1.fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp1023",

                            "-0x1.fffffffffffffp1023",
                            "-0x1.fffffffffffff000000000000000000000000001p1023",
                            "-0x1.fffffffffffff1p1023",
                            "-0x1.fffffffffffff100000000000000000000000001p1023",
                            "-0x1.fffffffffffff1fffffffffffffffffffffffffffffffffffffffffffffp1023",
                            "-0x1.fffffffffffff7p1023",
                            "-0x1.fffffffffffff700000000000000000000000001p1023",
                            "-0x1.fffffffffffff8p1023",
                            "-0x1.fffffffffffff800000000000000000000000001p1023",
                            "-0x1.fffffffffffff8fffffffffffffffffffffffffffffffffffffffffffffp1023",
                            "-0x1.fffffffffffff9p1023",
                            "-0x1.fffffffffffff900000000000000000000000001p1023",
                            "-0x1.ffffffffffffffp1023",
                            "-0x1.ffffffffffffff00000000000000000000000001p1023",
                            "-0x1.fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp1023"
                        };

                        for (int i = 0; i < inputs1.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds1[i]), inputs1[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseDouble_LString_MaxSubNormalBoundary())

                        long[] expecteds2 = {
                            0xfffffffffffffL,                0xfffffffffffffL,                0xfffffffffffffL,
                            0xfffffffffffffL,                0xfffffffffffffL,                0xfffffffffffffL,
                            0xfffffffffffffL,                0x10000000000000L,                0x10000000000000L,
                            0x10000000000000L,                0x10000000000000L,                0x10000000000000L,
                            0x10000000000000L,                0x10000000000000L,                0x10000000000000L,

                            unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),
                            unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),                unchecked((long)0x800fffffffffffffL),
                            unchecked((long)0x800fffffffffffffL),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
                            unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
                            unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L)
                        };

                        string[] inputs2 = {
                           "0x0.fffffffffffffp-1022",
                           "0x0.fffffffffffff00000000000000000000000000000000001p-1022",
                           "0x0.fffffffffffff1p-1022",
                           "0x0.fffffffffffff10000000000000000000000000000000001p-1022",
                           "0x0.fffffffffffff1ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "0x0.fffffffffffff7p-1022",
                           "0x0.fffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "0x0.fffffffffffff8p-1022",
                           "0x0.fffffffffffff80000000000000000000000000000000001p-1022",
                           "0x0.fffffffffffff8ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "0x0.fffffffffffff9p-1022",
                           "0x0.fffffffffffff9ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "0x0.ffffffffffffffp-1022",
                           "0x0.ffffffffffffff0000000000000000000000000000000001p-1022",
                           "0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",

                           "-0x0.fffffffffffffp-1022",
                           "-0x0.fffffffffffff00000000000000000000000000000000001p-1022",
                           "-0x0.fffffffffffff1p-1022",
                           "-0x0.fffffffffffff10000000000000000000000000000000001p-1022",
                           "-0x0.fffffffffffff1ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.fffffffffffff7p-1022",
                           "-0x0.fffffffffffff7ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.fffffffffffff8p-1022",
                           "-0x0.fffffffffffff80000000000000000000000000000000001p-1022",
                           "-0x0.fffffffffffff8ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.fffffffffffff9p-1022",
                           "-0x0.fffffffffffff9ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.ffffffffffffffp-1022",
                           "-0x0.ffffffffffffff0000000000000000000000000000000001p-1022",
                           "-0x0.ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffp-1022"
                        };

                        for (int i = 0; i < inputs2.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds2[i]), inputs2[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseDouble_LString_MinNormalBoundary())

                        long[] expecteds3 = {
                            0x10000000000000L,                0x10000000000000L,                0x10000000000000L,
                            0x10000000000000L,                0x10000000000000L,                0x10000000000000L,
                            0x10000000000000L,                0x10000000000000L,                0x10000000000001L,
                            0x10000000000001L,                0x10000000000001L,                0x10000000000001L,
                            0x10000000000001L,                0x10000000000001L,                0x10000000000001L,

                            unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
                            unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),
                            unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000000L),                unchecked((long)0x8010000000000001L),
                            unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L),
                            unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L),                unchecked((long)0x8010000000000001L)
                        };

                        string[] inputs3 = {
                           "0x1.0p-1022",
                           "0x1.00000000000001p-1022",
                           "0x1.000000000000010000000000000000001p-1022",
                           "0x1.00000000000001fffffffffffffffffffffffffffffffffp-1022",
                           "0x1.00000000000007p-1022",
                           "0x1.000000000000070000000000000000001p-1022",
                           "0x1.00000000000007fffffffffffffffffffffffffffffffffp-1022",
                           "0x1.00000000000008p-1022",
                           "0x1.000000000000080000000000000000001p-1022",
                           "0x1.00000000000008fffffffffffffffffffffffffffffffffp-1022",
                           "0x1.00000000000009p-1022",
                           "0x1.000000000000090000000000000000001p-1022",
                           "0x1.00000000000009fffffffffffffffffffffffffffffffffp-1022",
                           "0x1.0000000000000fp-1022",
                           "0x1.0000000000000ffffffffffffffffffffffffffffffffffp-1022",

                           "-0x1.0p-1022",
                           "-0x1.00000000000001p-1022",
                           "-0x1.000000000000010000000000000000001p-1022",
                           "-0x1.00000000000001fffffffffffffffffffffffffffffffffp-1022",
                           "-0x1.00000000000007p-1022",
                           "-0x1.000000000000070000000000000000001p-1022",
                           "-0x1.00000000000007fffffffffffffffffffffffffffffffffp-1022",
                           "-0x1.00000000000008p-1022",
                           "-0x1.000000000000080000000000000000001p-1022",
                           "-0x1.00000000000008fffffffffffffffffffffffffffffffffp-1022",
                           "-0x1.00000000000009p-1022",
                           "-0x1.000000000000090000000000000000001p-1022",
                           "-0x1.00000000000009fffffffffffffffffffffffffffffffffp-1022",
                           "-0x1.0000000000000fp-1022",
                           "-0x1.0000000000000ffffffffffffffffffffffffffffffffffp-1022"
                        };

                        for (int i = 0; i < inputs3.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds3[i]), inputs3[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseDouble_LString_MinSubNormalBoundary())

                        long[] expecteds4 = {
                            0x1L,                0x1L,                0x2L,
                            0x1L,                0x1L,                0x1L,
                            0x2L,                0x2L,                0x2L,
                            0x2L,                0x2L,                0x2L,
                            0x2L,                0x2L,                0x2L,

                            unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000002L),
                            unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),
                            unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),
                            unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),
                            unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L),                unchecked((long)0x8000000000000002L)
                        };

                        string[] inputs4 = {
                           "0x0.0000000000001p-1022",
                           "0x0.00000000000010000000000000000001p-1022",
                           "0x0.0000000000001fffffffffffffffffffffffffffffffffp-1022",
                           "0x0.00000000000017p-1022",
                           "0x0.000000000000170000000000000000001p-1022",
                           "0x0.00000000000017fffffffffffffffffffffffffffffffffp-1022",
                           "0x0.00000000000018p-1022",
                           "0x0.000000000000180000000000000000001p-1022",
                           "0x0.00000000000018fffffffffffffffffffffffffffffffffp-1022",
                           "0x0.00000000000019p-1022",
                           "0x0.000000000000190000000000000000001p-1022",
                           "0x0.00000000000019fffffffffffffffffffffffffffffffffp-1022",
                           "0x0.0000000000001fp-1022",
                           "0x0.0000000000001f0000000000000000001p-1022",
                           "0x0.0000000000001ffffffffffffffffffffffffffffffffffp-1022",

                           "-0x0.0000000000001p-1022",
                           "-0x0.00000000000010000000000000000001p-1022",
                           "-0x0.0000000000001fffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.00000000000017p-1022",
                           "-0x0.000000000000170000000000000000001p-1022",
                           "-0x0.00000000000017fffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.00000000000018p-1022",
                           "-0x0.000000000000180000000000000000001p-1022",
                           "-0x0.00000000000018fffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.00000000000019p-1022",
                           "-0x0.000000000000190000000000000000001p-1022",
                           "-0x0.00000000000019fffffffffffffffffffffffffffffffffp-1022",
                           "-0x0.0000000000001fp-1022",
                           "-0x0.0000000000001f0000000000000000001p-1022",
                           "-0x0.0000000000001ffffffffffffffffffffffffffffffffffp-1022"
                        };

                        for (int i = 0; i < inputs4.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds4[i]), inputs4[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseDouble_LString_NormalNegativeExponent())

                        long[] expecteds5 = {
                            0x3f323456789abcdfL,                0x3f8111012345678aL,                0x3ee1110091a2b3c5L,
                            0x3e39998091a2b3c5L,                0x3d91110048d159e2L,                0x3ce5554048d159e2L,
                            0x3c39998048d159e2L,                0x3b8dddc048d159e2L,                0x3ae111002468acf1L,
                            0x3a3333202468acf1L,                0x3991011001234568L,                0x38e2112101234568L,
                            0x3833213201234568L,                0x3784314301234568L,                0x36d5415401234568L,
                            0x3626516501234568L,                0x3577617601234568L,                0x34c8718701234568L,
                            0x3419819801234568L,                0x337049048091a2b4L,                0x32c101100091a2b4L,
                            0x321189188091a2b4L,                0x316211210091a2b4L,                0x30b299298091a2b4L,
                            0x300321320091a2b4L,                0x2f53a93a8091a2b4L,                0x2ea431430091a2b4L,
                            0x2df4b94b8091a2b4L,                0x2d4841840091a2b4L,                0x2c98c98c8091a2b4L,
                            0x2be981980091a2b4L,                0x2b3a09a08091a2b4L,                0x2a8a91a90091a2b4L,
                            0x29db19b18091a2b4L,                0x292ba1ba0091a2b4L,                0x287c29c28091a2b4L,
                            0x27ccb1cb0091a2b4L,                0x27201d01c048d15aL,                0x267061060048d15aL,
                            0x25c0a50a4048d15aL,                0x251101100048d15aL,                0x246145144048d15aL,
                            0x23b189188048d15aL,                0x2301cd1cc048d15aL,                0x225211210048d15aL,
                            0x21a255254048d15aL,                0x20f419418048d15aL,                0x20445d45c048d15aL,
                            0x1f94a14a0048d15aL,                0x1ee4e54e4048d15aL,                0x1e3541540048d15aL,
                            0x1d8585584048d15aL,                0x1cd5c95c8048d15aL,                0x1c260d60c048d15aL,
                            0x1b7651650048d15aL,                0x1ac815814048d15aL,                0x1a1859858048d15aL,
                            0x19689d89c048d15aL,                0x18b8e18e0048d15aL,                0x180925924048d15aL,
                            0x175981980048d15aL,                0x16a9c59c4048d15aL,                0x15fa09a08048d15aL,
                            0x154a4da4c048d15aL,                0x149c11c10048d15aL,                0x13ec55c54048d15aL,
                            0x133c99c98048d15aL,                0x128cddcdc048d15aL,                0x11dd21d20048d15aL,
                            0x112d65d64048d15aL,                0x107dc1dc0048d15aL,                0xfce05e04048d15aL,
                            0xf1e49e48048d15aL,                0xe700700602468adL,                0xdc02902802468adL,
                            0xd104b04a02468adL,                0xc606d06c02468adL,                0xbb08f08e02468adL,
                            0xb00b10b002468adL,                0xa50d30d202468adL,                0x9a10110002468adL,
                            0x8f12312202468adL,                0x8420520402468adL,                0x7922722602468adL,
                            0x6e24924802468adL,                0x6326b26a02468adL,                0x5828d28c02468adL,
                            0x4d2af2ae02468adL,                0x422d12d002468adL,                0x372f32f202468adL,
                            0x2c32132002468adL,                0x220011001012345L,                0x170121012012345L,
                            0xc0231023012345L,                0x10341034012345L,                0x208a208a024L,
                            0x41584158L,                            0x83388L,                                0x108L,
                            0x0L,                                        0x0L,                                       0x0L,
                            0x0L,                                        0x0L,                                       0x0L,
                            0x0L,                                        0x0L,                                       0x0L,
                            0x0L,                                        0x0L,                                       0x0L,
                            0x0L,                                        0x0L
                        };

                        for (int i = 0; i < expecteds5.Length; i++)
                        {
                            int part = i * 11;
                            string inputString = "0x" + part.ToString(NumberFormatInfo.InvariantInfo) + "."
                                + part.ToString(NumberFormatInfo.InvariantInfo) + "0123456789abcdefp-" + part.ToString(NumberFormatInfo.InvariantInfo);

                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds5[i]), inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseDouble_LString_NormalPositiveExponent())

                        long[] expecteds6 = {
                            0x3f323456789abcdfL,                0x40e111012345678aL,                0x41a1110091a2b3c5L,
                            0x4259998091a2b3c5L,                0x4311110048d159e2L,                0x43c5554048d159e2L,
                            0x4479998048d159e2L,                0x452dddc048d159e2L,                0x45e111002468acf1L,
                            0x469333202468acf1L,                0x4751011001234568L,                0x4802112101234568L,
                            0x48b3213201234568L,                0x4964314301234568L,                0x4a15415401234568L,
                            0x4ac6516501234568L,                0x4b77617601234568L,                0x4c28718701234568L,
                            0x4cd9819801234568L,                0x4d9049048091a2b4L,                0x4e4101100091a2b4L,
                            0x4ef189188091a2b4L,                0x4fa211210091a2b4L,                0x505299298091a2b4L,
                            0x510321320091a2b4L,                0x51b3a93a8091a2b4L,                0x526431430091a2b4L,
                            0x5314b94b8091a2b4L,                0x53c841840091a2b4L,                0x5478c98c8091a2b4L,
                            0x552981980091a2b4L,                0x55da09a08091a2b4L,                0x568a91a90091a2b4L,
                            0x573b19b18091a2b4L,                0x57eba1ba0091a2b4L,                0x589c29c28091a2b4L,
                            0x594cb1cb0091a2b4L,                0x5a001d01c048d15aL,                0x5ab061060048d15aL,
                            0x5b60a50a4048d15aL,                0x5c1101100048d15aL,                0x5cc145144048d15aL,
                            0x5d7189188048d15aL,                0x5e21cd1cc048d15aL,                0x5ed211210048d15aL,
                            0x5f8255254048d15aL,                0x603419418048d15aL,                0x60e45d45c048d15aL,
                            0x6194a14a0048d15aL,                0x6244e54e4048d15aL,                0x62f541540048d15aL,
                            0x63a585584048d15aL,                0x6455c95c8048d15aL,                0x65060d60c048d15aL,
                            0x65b651650048d15aL,                0x666815814048d15aL,                0x671859858048d15aL,
                            0x67c89d89c048d15aL,                0x6878e18e0048d15aL,                0x692925924048d15aL,
                            0x69d981980048d15aL,                0x6a89c59c4048d15aL,                0x6b3a09a08048d15aL,
                            0x6bea4da4c048d15aL,                0x6c9c11c10048d15aL,                0x6d4c55c54048d15aL,
                            0x6dfc99c98048d15aL,                0x6eacddcdc048d15aL,                0x6f5d21d20048d15aL,
                            0x700d65d64048d15aL,                0x70bdc1dc0048d15aL,                0x716e05e04048d15aL,
                            0x721e49e48048d15aL,                0x72d00700602468adL,                0x73802902802468adL,
                            0x74304b04a02468adL,                0x74e06d06c02468adL,                0x75908f08e02468adL,
                            0x7640b10b002468adL,                0x76f0d30d202468adL,                0x77a10110002468adL,
                            0x78512312202468adL,                0x79020520402468adL,                0x79b22722602468adL,
                            0x7a624924802468adL,                0x7b126b26a02468adL,                0x7bc28d28c02468adL,
                            0x7c72af2ae02468adL,                0x7d22d12d002468adL,                0x7dd2f32f202468adL,
                            0x7e832132002468adL,                0x7f40011001012345L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L,                0x7ff0000000000000L,
                            0x7ff0000000000000L,                0x7ff0000000000000L  };

                        for (int i = 0; i < expecteds6.Length; i++)
                        {
                            int part = i * 11;
                            string inputString = "0x" + part.ToString(NumberFormatInfo.InvariantInfo) + "."
                                + part.ToString(NumberFormatInfo.InvariantInfo) + "0123456789abcdefp" + part.ToString(NumberFormatInfo.InvariantInfo);

                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds6[i]), inputString, NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Harmony (Test_parseDouble_LString_ZeroBoundary())

                        long[] expecteds7 = {
                            0x0L,                0x0L,                0x0L,
                            0x1L,                0x1L,                0x1L,
                            0x1L,                0x1L,                0x1L,
                            unchecked((long)0x8000000000000000L),                unchecked((long)0x8000000000000000L),                unchecked((long)0x8000000000000000L),
                            unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),
                            unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L),                unchecked((long)0x8000000000000001L) };

                        string[] inputs7 = {
                            "0x0.00000000000004p-1022",
                            "0x0.00000000000007ffffffffffffffffffffffp-1022",
                            "0x0.00000000000008p-1022",
                            "0x0.000000000000080000000000000000001p-1022",
                            "0x0.00000000000008fffffffffffffffffffffffffffffffp-1022",
                            "0x0.00000000000009p-1022",
                            "0x0.000000000000090000000000000000001p-1022",
                            "0x0.00000000000009fffffffffffffffffffffffffffffffffp-1022",
                            "0x0.0000000000000fffffffffffffffffffffffffffffffffffp-1022",

                            "-0x0.00000000000004p-1022",
                            "-0x0.00000000000007ffffffffffffffffffffffp-1022",
                            "-0x0.00000000000008p-1022",
                            "-0x0.000000000000080000000000000000001p-1022",
                            "-0x0.00000000000008fffffffffffffffffffffffffffffffp-1022",
                            "-0x0.00000000000009p-1022",
                            "-0x0.000000000000090000000000000000001p-1022",
                            "-0x0.00000000000009fffffffffffffffffffffffffffffffffp-1022",
                            "-0x0.0000000000000fffffffffffffffffffffffffffffffffffp-1022" };

                        for (int i = 0; i < inputs7.Length; i++)
                        {
                            yield return new TestCaseData(BitConversion.Int64BitsToDouble(expecteds7[i]), inputs7[i], NumberStyle.HexFloat, J2N.Text.StringFormatter.InvariantCulture);
                        }

                        // Custom

                        yield return new TestCaseData(3.0d, "0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(3.0d, "0x1,8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberDecimalSeparator = "," });
                        yield return new TestCaseData(3.0d, "0x1--8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberDecimalSeparator = "--" });
                        yield return new TestCaseData(3.0d, "0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);

                        // Negative sign format tests
                        yield return new TestCaseData(-3.0d, "-0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0d, "-0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-2.0d, "-0x1.", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0d, "- 0x1.8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-3.0d, "- 0x1.8", NumberStyle.HexFloat, new NumberFormatInfo { NumberNegativePattern = 2 });
                        yield return new TestCaseData(-2.0d, "- 0x1.", NumberStyle.HexFloat, new NumberFormatInfo { NumberNegativePattern = 2 });

                        // Constant values

                        yield return new TestCaseData(double.NaN, "NaN", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.NaN, "+NaN", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.NaN, "-NaN", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(double.PositiveInfinity, "Infinity", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.PositiveInfinity, "+Infinity", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(double.NegativeInfinity, "-Infinity", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data
                {
                    get
                    {
                        // JDK 8 (testParsing())

                        for (int i = 0; i < badStrings.Length; i++)
                        {
                            string inputString = badStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        for (int i = 0; i < paddedBadStrings.Length; i++)
                        {
                            string inputString = paddedBadStrings[i];
                            NumberStyle styles = NumberStyle.Float;
                            if (inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        // Custom

                        // Negative format tests
                        yield return new TestCaseData(typeof(FormatException), "1.8p1-", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.8-", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.-", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.8p1 -", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.8 -", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1. -", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow trailing negative sign.");

                        yield return new TestCaseData(typeof(FormatException), "(1.8p1)", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(1.8)", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(1.)", NumberStyle.Float, NumberFormatInfo.InvariantInfo, "NumberStyle.Float doesn't allow parentheses.");

                        yield return new TestCaseData(typeof(FormatException), "1. -d", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier may not be after negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "1.-d", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier may not be after negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "(1.)d", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Type specifier may not be after negative sign.");

                        yield return new TestCaseData(typeof(FormatException), "(1.", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Not a complete set of parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "1.)", NumberStyle.Float | NumberStyle.AllowParentheses | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Not a complete set of parentheses.");
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data
                {
                    get
                    {
                        // JDK 8 (testParsing())

                        for (int i = 0; i < badHexStrings.Length; i++)
                        {
                            string inputString = badHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        for (int i = 0; i < paddedBadHexStrings.Length; i++)
                        {
                            string inputString = paddedBadHexStrings[i];
                            NumberStyle styles = NumberStyle.HexFloat;
                            if (inputString.TrimEnd().EndsWith("d", StringComparison.OrdinalIgnoreCase) || inputString.TrimEnd().EndsWith("f", StringComparison.OrdinalIgnoreCase))
                                styles |= NumberStyle.AllowTypeSpecifier;
                            var provider = J2N.Text.StringFormatter.InvariantCulture;

                            // Pass through the same value - all we care about here is that the input is accepted without error
                            yield return new TestCaseData(typeof(FormatException), inputString, styles, provider, "Bad input not expected to be parsed.");
                        }

                        // Harmony (Test_parseDouble_LString_Illegal())

                        // J2N: This format is valid in .NET, since the hex specifier is optional
                        //yield return new TestCaseData(typeof(FormatException), "0.0p0D", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), "+0x.p1d", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), "0Xg.gp1D", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), "-0x1.1p", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), "+0x 1.1 p2d", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), "x1.1p2d", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), " 0x-2.1p2", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), " 0x2.1pad", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");
                        yield return new TestCaseData(typeof(FormatException), " 0x111.222p 22d", NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier, NumberFormatInfo.InvariantInfo, "Should throw FormatException.");

                        // Custom

                        // Negative format tests
                        yield return new TestCaseData(typeof(FormatException), "0x1.8p1-", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.8-", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.-", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.8p1 -", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1.8 -", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(FormatException), "0x1. -", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");

                        yield return new TestCaseData(typeof(FormatException), "(0x1.8p1)", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(0x1.8)", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(FormatException), "(0x1.)", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");

                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8p1-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8p1 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1.8 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");
                        yield return new TestCaseData(typeof(ArgumentException), "0x1. -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow trailing negative sign.");

                        yield return new TestCaseData(typeof(ArgumentException), "(0x1.8p1)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(ArgumentException), "(0x1.8)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                        yield return new TestCaseData(typeof(ArgumentException), "(0x1.)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo, "NumberStyle.HexFloat doesn't allow parentheses.");
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data

                #region TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data
                {
                    get
                    {
                        // Harmony (Test_parseDoubleLjava_lang_String())

                        yield return new TestCaseData(rawBitsFor3_4en324ToN1[0], "4.9e-324", "3.4e-324", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4en324ToN1[1], "3.5e-323", "3.4e-323", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        for (int i = 322; i > 3; i--)
                        {
                            string testString, expectedString;
                            testString = expectedString = "3.4e-" + i.ToString(NumberFormatInfo.InvariantInfo);
                            yield return new TestCaseData(rawBitsFor3_4en324ToN1[324 - i], expectedString, testString, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        yield return new TestCaseData(rawBitsFor3_4en324ToN1[321], "0.0034", "3.4e-3", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4en324ToN1[322], "0.034", "3.4e-2", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4en324ToN1[323], "0.34", "3.4e-1", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor3_4en324ToN1[324], "3.4", "3.4e-0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(rawBitsFor1_2e0To309[0], "1.2", "1.2e0", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_2e0To309[1], "12.0", "1.2e1", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_2e0To309[2], "120.0", "1.2e2", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_2e0To309[3], "1200.0", "1.2e3", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_2e0To309[4], "12000.0", "1.2e4", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_2e0To309[5], "120000.0", "1.2e5", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(rawBitsFor1_2e0To309[6], "1200000.0", "1.2e6", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        for (int i = 7; i <= 308; i++)
                        {
                            string testString, expectedString;
                            testString = expectedString = "1.2e" + i.ToString(NumberFormatInfo.InvariantInfo);
                            yield return new TestCaseData(rawBitsFor1_2e0To309[i], expectedString, testString, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        }

                        yield return new TestCaseData(rawBitsFor1_2e0To309[309], "Infinity", "1.2e309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0x7e054218c295e43fL, "1.1122233344455567E299",
                            "111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000.92233720368547758079223372036854775807",
                            NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xfe054218c295e43fL), "-1.1122233344455567E299",
                            "-111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000111222333444555666777888999000.92233720368547758079223372036854775807",
                            NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0x562ae7a25fe706ebL,
                            "1.234123412431233E107", "1.234123412431233E107", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x562ae7a25fe706ecL,
                            "1.2341234124312331E107", "1.2341234124312331E107", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x562ae7a25fe706ecL,
                            "1.2341234124312331E107", "1.2341234124312332E107", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xd62ae7a25fe706ebL),
                            "-1.234123412431233E107", "-1.234123412431233E107", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xd62ae7a25fe706ecL),
                            "-1.2341234124312331E107", "-1.2341234124312331E107", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xd62ae7a25fe706ecL),
                            "-1.2341234124312331E107", "-1.2341234124312332E107", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0x44b52d02c7e14af6L, /*"1.0e23"*/ "9.999999999999999E22", "1e23", NumberStyle.Float, NumberFormatInfo.InvariantInfo); // J2N: This is the observed behavior in Java (at least on 64 bit windows)

                        /*
                         * These particular tests verify that the extreme boundary conditions
                         * are converted correctly.
                         */
                        yield return new TestCaseData(0L, "0.0", "0.0e-309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-0.0", "-0.0e-309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0L, "0.0", "0.0e309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000000L), "-0.0", "-0.0e309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7fe1ccf385ebc8a0L, "1.0e308", "0.1e309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7ff0000000000000L, "Infinity", "0.2e309", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(1L, "4.9e-324", "65e-325", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(2L, "1.0e-323", "1000e-326", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0x86789e3750f791L, "4.0e-306", "4.0e-306", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0xffffe2e8159d0L, "2.22507e-308", "2.22507e-308", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x48746da623f1dd8bL, "1.1122233344455567E41",
                            "111222333444555666777888999000111228999000.92233720368547758079223372036854775807", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xc8746da623f1dd8bL), "-1.1122233344455567E41",
                            "-111222333444555666777888999000111228999000.92233720368547758079223372036854775807", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x54820fe0ba17f469L, "1.2345678901234567E99",
                            "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890.987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xd4820fe0ba17f469L), "-1.2345678901234567E99",
                            "-1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890.987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210987654321098765432109876543210", NumberStyle.Float, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(0x7fefffffffffffffL, "1.7976931348623157E308",
                            "179769313486231570000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xffefffffffffffffL), "-1.7976931348623157E308",
                            "-179769313486231570000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7ff0000000000000L, "Infinity",
                            "1112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001234567890", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xfff0000000000000L), "-Infinity",
                            "-1112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001112223334445556667778889990001234567890", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x7ff0000000000000L, "Infinity",
                            "179769313486231590000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xfff0000000000000L), "-Infinity",
                            "-179769313486231590000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x2b392a32afcc661eL, "1.7976931348623157E-100",
                            "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0xab392a32afcc661eL), "-1.7976931348623157E-100",
                            "-0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x1b3432f0cb68e61L, "1.7976931348623157E-300",
                            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x81b3432f0cb68e61L), "-1.7976931348623157E-300",
                            "-0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x2117b590b942L, "1.79769313486234E-310",
                            "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x80002117b590b942L), "-1.79769313486234E-310",
                            "-0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0xe37L, "1.798E-320",
                            "0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000e37L), "-1.798E-320",
                            "-0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000017976931348623157", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x2L, "1.0E-323",
                            "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000002L), "-1.0E-323",
                            "-0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(0x1L, "4.9E-324",
                            "0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000055595409854908458349204328908234982349050934129878452378432452458968024357823490509341298784523784324524589680243578234905093412987845237843245245896802435782349050934129878452378432452458968024357868024357823490509341298784523784324524589680243578234905093412987845237843245245896802435786802435782349050934129878452378432452458968024357823490509341298784523784324524589680243578", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(unchecked((long)0x8000000000000001L), "-4.9E-324",
                            "-0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000055595409854908458349204328908234982349050934129878452378432452458968024357823490509341298784523784324524589680243578234905093412987845237843245245896802435782349050934129878452378432452458968024357868024357823490509341298784523784324524589680243578234905093412987845237843245245896802435786802435782349050934129878452378432452458968024357823490509341298784523784324524589680243578", NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                    }
                }

                #endregion TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data
            }

            #region Parse_CharSequence_NumberStyle_IFormatProvider

            public abstract class Parse_CharSequence_NumberStyle_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract double GetResult(string value, NumberStyle style, IFormatProvider provider);

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle(double expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    double actual = GetResult(value, style, provider);

                    string expectedString = "0x" + BitConversion.DoubleToInt64Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.DoubleToInt64Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle(double expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    double actual = GetResult(value, style, provider);

                    string expectedString = "0x" + BitConversion.DoubleToInt64Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.DoubleToInt64Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider), message);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider), message);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits(long expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    long rawBits;
                    string convertedString;
                    double result = GetResult(value, style, provider);
                    rawBits = BitConversion.DoubleToInt64Bits(result);
                    convertedString = Double.GetInstance(result).ToString(J2N.Text.StringFormatter.InvariantCulture);

                    assertEquals(expectedRawBits, rawBits);
                    assertEquals(expectedString.ToLower(Locale_US), convertedString
                            .ToLower(Locale_US));
                }

                [Test]
                public void TestBufferOverrun()
                {
                    double expected = double.Epsilon;
                    string input = "2.47032822925563928955488610223975683825075313028607204245901671443135881701471561216490502500611448891113821261974342300460418902942448352807590938286946753938651115551760560719928058878293671231747736634408280531526902605907773622021915212565490055774142324317176285739934621929266664332301574472636463785506624517729295274366260118660863836029219323513171043552966204750864260101376307037863078272186341055251908512708551508108801689745488069526735449270278409582397426355203982358963233588990525634848244115770970815275109010555867353978342572186166693436117300403071722862355340083843290764615831101299228427291345949708292808225406036401688524974979852948763402190780682189954893639063625933369612197756528003477811220301766530180567324117966872133774348767321082931402997928671538829803466796875E-324";
                    double actual = Double.Parse(input, NumberStyle.Float, NumberFormatInfo.InvariantInfo);
                    assertEquals(expected, actual);
                }
            }

            public class Parse_String_NumberStyle_IFormatProvider : Parse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override double GetResult(string value, NumberStyle style, IFormatProvider provider)
                {
                    return Double.Parse(value, style, provider);
                }
            }

#if FEATURE_SPAN
            public class Parse_ReadOnlySpan_NumberStyle_IFormatProvider : Parse_CharSequence_NumberStyle_IFormatProvider_TestCase
            {
                protected override bool IsNullableType => false;

                protected override double GetResult(string value, NumberStyle style, IFormatProvider provider)
                {
                    return Double.Parse(value.AsSpan(), style, provider);
                }

                [Test]
                public void TestUnicodeSymbols()
                {
                    assertEquals(double.PositiveInfinity, GetResult("INFINITYe\u0661234", NumberStyle.Float, new NumberFormatInfo { PositiveInfinitySymbol = "Infinitye\u0661234" }));
                    assertEquals(double.NegativeInfinity, GetResult("NEGINFINITYe\u0661234", NumberStyle.Float, new NumberFormatInfo { NegativeInfinitySymbol = "NegInfinitye\u0661234" }));
                    assertEquals(double.NaN, GetResult("NANe\u0661234", NumberStyle.Float, new NumberFormatInfo { NaNSymbol = "NaNe\u0661234" }));
                }
            }
#endif

            #endregion

            #region Parse_CharSequence_IFormatProvider

            public abstract class Parse_CharSequence_IFormatProvider_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract double GetResult(string value, NumberStyle style, IFormatProvider provider);

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestParse_CharSequence_IFormatProvider_ForFloatStyle(double expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");

                    double actual = GetResult(value, style, provider);

                    string expectedString = "0x" + BitConversion.DoubleToInt64Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.DoubleToInt64Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestParse_CharSequence_IFormatProvider_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");

                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider), message);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestParse_CharSequence_IFormatProvider_ForRawBits(long expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");

                    long rawBits;
                    string convertedString;
                    double result = GetResult(value, style, provider);
                    rawBits = BitConversion.DoubleToInt64Bits(result);
                    convertedString = Double.GetInstance(result).ToString(J2N.Text.StringFormatter.InvariantCulture);

                    assertEquals(expectedRawBits, rawBits);
                    assertEquals(expectedString.ToLower(Locale_US), convertedString
                            .ToLower(Locale_US));
                }
            }

            public class Parse_String_IFormatProvider : Parse_CharSequence_IFormatProvider_TestCase
            {
                protected override double GetResult(string value, NumberStyle style, IFormatProvider provider)
                {
                    return Double.Parse(value, provider);
                }
            }

            #endregion

            #region TryParse_CharSequence_NumberStyle_IFormatProvider_Double

            public abstract class TryParse_CharSequence_NumberStyle_IFormatProvider_Double_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, NumberStyle style, IFormatProvider provider, out double result);

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Double_ForFloatStyle(double expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, style, provider, out double actual));

                    string expectedString = "0x" + BitConversion.DoubleToInt64Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.DoubleToInt64Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyle_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Double_ForHexFloatStyle(double expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertTrue(GetResult(value, style, provider, out double actual));

                    string expectedString = "0x" + BitConversion.DoubleToInt64Bits(expected).ToHexString();
                    string actualString = "0x" + BitConversion.DoubleToInt64Bits(actual).ToHexString();
                    string errorMsg = $"input string is:<{value}>. "
                        + $"The expected result should be:<{expectedString}>, "
                        + $"but was: <{actualString}>. ";

                    assertEquals(errorMsg, expected, actual, 0.0D);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Double_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    assertFalse(message, GetResult(value, style, provider, out double actual));
                    assertEquals(0, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_ForHexFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    double actual = 0;
                    if (expectedExceptionType == typeof(FormatException) || expectedExceptionType.Equals(typeof(OverflowException)))
                    {
                        assertFalse(message, GetResult(value, style, provider, out actual));
                    }
                    else // Actual exception should be thrown
                    {
                        Assert.Throws(expectedExceptionType, () => GetResult(value, style, provider, out actual), message);
                    }
                    assertEquals(0, actual);
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestTryParse_CharSequence_NumberStyle_IFormatProvider_Double_ForRawBits(long expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");

                    long rawBits;
                    string convertedString;
                    assertTrue(GetResult(value, style, provider, out double result));
                    rawBits = BitConversion.DoubleToInt64Bits(result);
                    convertedString = Double.GetInstance(result).ToString(J2N.Text.StringFormatter.InvariantCulture);

                    assertEquals(expectedRawBits, rawBits);
                    assertEquals(expectedString.ToLower(Locale_US), convertedString
                            .ToLower(Locale_US));
                }
            }

            public class TryParse_String_NumberStyle_IFormatProvider_Double : TryParse_CharSequence_NumberStyle_IFormatProvider_Double_TestCase
            {
                protected override bool GetResult(string value, NumberStyle style, IFormatProvider provider, out double result)
                {
                    return Double.TryParse(value, style, provider, out result);
                }
            }

#if FEATURE_SPAN
            public class TryParse_ReadOnlySpan_NumberStyle_IFormatProvider_Double : TryParse_CharSequence_NumberStyle_IFormatProvider_Double_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, NumberStyle style, IFormatProvider provider, out double result)
                {
                    return Double.TryParse(value.AsSpan(), style, provider, out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_NumberStyle_IFormatProvider_Double

            #region TryParse_CharSequence_Double

            public abstract class TryParse_CharSequence_Double_TestCase : ParseTestCase
            {
                protected virtual bool IsNullableType => true;

                protected abstract bool GetResult(string value, out double result);

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyle_Data")]
                public void TestTryParse_CharSequence_Double_ForFloatStyle(double expected, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");
                    // NOTE: Assumption made here that this contains no culture sensitivity tests.
                    Assume.That(NumberFormatInfo.GetInstance(provider).Equals(NumberFormatInfo.InvariantInfo), "Non-invariant tests do not apply to this overload.");

                    using (var context = new CultureContext(CultureInfo.InvariantCulture))
                    {
                        assertTrue(GetResult(value, out double actual));

                        string expectedString = "0x" + BitConversion.DoubleToInt64Bits(expected).ToHexString();
                        string actualString = "0x" + BitConversion.DoubleToInt64Bits(actual).ToHexString();
                        string errorMsg = $"input string is:<{value}>. "
                            + $"The expected result should be:<{expectedString}>, "
                            + $"but was: <{actualString}>. ";

                        assertEquals(errorMsg, expected, actual, 0.0D);
                    }
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForFloatStyleException_Data")]
                public void TestTryParse_CharSequence_Double_ForFloatStyleException(Type expectedExceptionType, string value, NumberStyle style, IFormatProvider provider, string message)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");
                    // NOTE: Assumption made here that this contains no culture sensitivity tests.
                    Assume.That(NumberFormatInfo.GetInstance(provider).Equals(NumberFormatInfo.InvariantInfo), "Non-invariant tests do not apply to this overload.");

                    using (var context = new CultureContext(CultureInfo.InvariantCulture))
                    {

                        assertFalse(message, GetResult(value, out double actual));
                        assertEquals(0, actual);
                    }
                }

                [TestCaseSource(typeof(ParseTestCase), "TestParse_CharSequence_NumberStyle_IFormatProvider_ForRawBits_Data")]
                public void TestTryParse_CharSequence_Double_ForRawBits(long expectedRawBits, string expectedString, string value, NumberStyle style, IFormatProvider provider)
                {
                    Assume.That(IsNullableType || (!IsNullableType && value != null), "null is not supported by this character sequence type.");
                    Assume.That((style & ~(NumberStyle.Float | NumberStyle.AllowThousands)) == 0, "Custom NumberStyles are not supported on this overload.");
                    // NOTE: Assumption made here that this contains no culture sensitivity tests.
                    Assume.That(NumberFormatInfo.GetInstance(provider).Equals(NumberFormatInfo.InvariantInfo), "Non-invariant tests do not apply to this overload.");

                    using (var context = new CultureContext(CultureInfo.InvariantCulture))
                    {

                        long rawBits;
                        string convertedString;
                        assertTrue(GetResult(value, out double result));
                        rawBits = BitConversion.DoubleToInt64Bits(result);
                        convertedString = Double.GetInstance(result).ToString(J2N.Text.StringFormatter.InvariantCulture);

                        assertEquals(expectedRawBits, rawBits);
                        assertEquals(expectedString.ToLower(Locale_US), convertedString
                                .ToLower(Locale_US));
                    }
                }
            }

            public class TryParse_String_Double : TryParse_CharSequence_Double_TestCase
            {
                protected override bool GetResult(string value, out double result)
                {
                    return Double.TryParse(value, out result);
                }
            }

#if FEATURE_SPAN
            public class TryParse_ReadOnlySpan_Double : TryParse_CharSequence_Double_TestCase
            {
                protected override bool IsNullableType => false;

                protected override bool GetResult(string value, out double result)
                {
                    return Double.TryParse(value.AsSpan(), out result);
                }
            }
#endif

            #endregion TryParse_CharSequence_Double

        }
    }
}
