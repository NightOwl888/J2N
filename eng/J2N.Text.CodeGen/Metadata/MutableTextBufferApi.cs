namespace J2N.Text.CodeGen.Metadata
{
    public static class MutableTextBufferApi
    {
        public static ApiModel Create()
        {
            return new ApiModel
            {
                Types =
                [
                    new TypeModel
                {
                    Namespace = "J2N.Text",
                    Name = "MutableTextBuffer",
                    Methods =
                    [
                        new MethodModel
                        {
                            Name = "Append",
                            ReturnType = "MutableTextBuffer",
                            ReturnsSelf = true,
                            Parameters =
                            [
                                new ParameterModel
                                {
                                    TypeName = "string?",
                                    Name = "value"
                                }
                            ]
                        },

                        new MethodModel
                        {
                            Name = "Clear",
                            ReturnType = "void",
                            ReturnsSelf = false,
                            Parameters =
                            [
                            ]
                        },

                        new MethodModel
                        {
                            Name = "Insert",
                            ReturnType = "MutableTextBuffer",
                            ReturnsSelf = true,
                            Parameters =
                            [
                                new ParameterModel
                                {
                                    TypeName = "int",
                                    Name = "index"
                                },

                                new ParameterModel
                                {
                                    TypeName = "string?",
                                    Name = "value"
                                }
                            ]
                        }
                    ]
                }
                ]
            };
        }
    }
}
