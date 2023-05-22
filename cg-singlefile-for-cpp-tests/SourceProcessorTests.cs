using cg_singlefile_for_cpp;
using Moq;
using Xunit;

namespace cg_singlefile_for_cpp_tests
{
    public class SourceProcessorTests
    {
        [Fact]
        public void IncludeWorksForQuotationMarks()
        {
            var fsMock = new Mock<IFileService>();
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("main.cpp"))))
                .Returns(new[]{
                    "#include <iostream>",
                    "#include \"mylib.h\"",
                    "",
                    "int main() {",
                    "   std::cout << hw() << std::endl;",
                    "}",
                });

            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("mylib.h"))))
                .Returns(new[]{
                    "int hw() {",
                    "   return 123;",
                    "}",
                });

            var sourceProcessor = new SourceProcessor(fsMock.Object);
            sourceProcessor.Process("main.cpp", "output.txt");

            fsMock.Verify(s => s.WriteAllLines(It.Is<string>(f => f.EndsWith("output.txt")), new[]
            {
                "#include <iostream>",
                "int hw() {",
                "   return 123;",
                "}",
                "",
                "int main() {",
                "   std::cout << hw() << std::endl;",
                "}",
            }));
        }

        [Fact]
        public void IncludeWorksForAngleBrackets()
        {
            var fsMock = new Mock<IFileService>();
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("main.cpp"))))
                .Returns(new[]{
                    "#include <iostream>",
                    "#include \"mylib.h\"",
                    "",
                    "int main() {",
                    "   std::cout << hw() << std::endl;",
                    "}",
                });
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("mylib.h"))))
                .Returns(new[]{
                    "#include <cstdint>",
                    "int32_t hw() {",
                    "   return 123;",
                    "}",
                });

            var sourceProcessor = new SourceProcessor(fsMock.Object);
            sourceProcessor.Process("main.cpp", "output.txt");

            fsMock.Verify(s => s.WriteAllLines(It.Is<string>(f => f.EndsWith("output.txt")), new[]
            {
                "#include <iostream>",
                "#include <cstdint>",
                "int32_t hw() {",
                "   return 123;",
                "}",
                "",
                "int main() {",
                "   std::cout << hw() << std::endl;",
                "}",
            }));
        }

        [Fact]
        public void IncludeWorksWithoutDuplication()
        {
            var fsMock = new Mock<IFileService>();
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("main.cpp"))))
                .Returns(new[]{
                    "#include <cstdint>",
                    "#include <iostream>",
                    "#include \"action.h\"",
                    "#include \"mylib.h\"",
                    "",
                    "int main() {",
                    "   std::cout << hw() << std::endl;",
                    "}",
                });
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("mylib.h"))))
                .Returns(new[]{
                    "#include <cstdint>",
                    "#include \"action.h\"",
                    "int32_t hw() {",
                    "   return 123;",
                    "}",
                });
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("action.h"))))
                .Returns(new[]{
                    "#include <iostream>",
                    "#include <cassert>",
                    "enum Action { Up, Down };",
                });

            var sourceProcessor = new SourceProcessor(fsMock.Object);
            sourceProcessor.Process("main.cpp", "output.txt");

            fsMock.Verify(s => s.WriteAllLines(It.Is<string>(f => f.EndsWith("output.txt")), new[]
            {
                "#include <cstdint>",
                "#include <iostream>",
                "#include <cassert>",
                "enum Action { Up, Down };",
                "int32_t hw() {",
                "   return 123;",
                "}",
                "",
                "int main() {",
                "   std::cout << hw() << std::endl;",
                "}",
            }));
        }

        [Fact]
        public void SkipsPragmaOnce()
        {
            var fsMock = new Mock<IFileService>();
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("main.cpp"))))
                .Returns(new[]{
                    "#include <iostream>",
                    "#include \"mylib.h\"",
                    "",
                    "int main() {",
                    "   std::cout << hw() << std::endl;",
                    "}",
                });
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("mylib.h"))))
                .Returns(new[]{
                    "#pragma once",
                    "int hw() {",
                    "   return 123;",
                    "}",
                });

            var sourceProcessor = new SourceProcessor(fsMock.Object);
            sourceProcessor.Process("main.cpp", "output.txt");

            fsMock.Verify(s => s.WriteAllLines(It.Is<string>(f => f.EndsWith("output.txt")), new[]
            {
                "#include <iostream>",
                "int hw() {",
                "   return 123;",
                "}",
                "",
                "int main() {",
                "   std::cout << hw() << std::endl;",
                "}",
            }));
        }

        [Fact]
        public void SkipsComments()
        {
            var fsMock = new Mock<IFileService>();
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("main.cpp"))))
                .Returns(new[]{
                    "#include <iostream>",
                    "#include \"mylib.h\"",
                    "",
                    "int main() {",
                    "   std::cout << hw() << std::endl;",
                    "}",
                });
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("mylib.h"))))
                .Returns(new[]{
                    "#pragma once",
                    "// this always function returns 123",
                    "int hw() {",
                    "   return 123;",
                    "}",
                });

            var sourceProcessor = new SourceProcessor(fsMock.Object);
            sourceProcessor.Process("main.cpp", "output.txt");

            fsMock.Verify(s => s.WriteAllLines(It.Is<string>(f => f.EndsWith("output.txt")), new[]
            {
                "#include <iostream>",
                "int hw() {",
                "   return 123;",
                "}",
                "",
                "int main() {",
                "   std::cout << hw() << std::endl;",
                "}",
            }));
        }

        [Fact]
        public void SkipsManuallyMarkedLines()
        {
            var fsMock = new Mock<IFileService>();
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("main.cpp"))))
                .Returns(new[]{
                    "#include <iostream>",
                    "#include \"mylib.h\"",
                    "",
                    "int main() {",
                    "   std::cout << hw() << std::endl;",
                    "}",
                });
            fsMock.Setup(s => s.ReadAllLines(It.Is<string>(f => f.EndsWith("mylib.h"))))
                .Returns(new[]{
                    "#include \"local_tool.h\" // singlefile-skip-line",
                    "int hw() {",
                    "   return 123;",
                    "}",
                });

            var sourceProcessor = new SourceProcessor(fsMock.Object);
            sourceProcessor.Process("main.cpp", "output.txt");

            fsMock.Verify(s => s.WriteAllLines(It.Is<string>(f => f.EndsWith("output.txt")), new[]
            {
                "#include <iostream>",
                "int hw() {",
                "   return 123;",
                "}",
                "",
                "int main() {",
                "   std::cout << hw() << std::endl;",
                "}",
            }));
        }
    }
}