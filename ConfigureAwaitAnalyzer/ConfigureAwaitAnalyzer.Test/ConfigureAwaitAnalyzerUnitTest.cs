using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = ConfigureAwaitAnalyzer.Test.CSharpAnalyzerVerifier<ConfigureAwaitAnalyzer.ConfigureAwaitAnalyzerAnalyzer>;

namespace ConfigureAwaitAnalyzer.Test
{
    [TestClass]
    public class ConfigureAwaitAnalyzerUnitTest
    {
        private const string CaHelper = @"
        public static class AwaitHelper
        {
            public static ConfiguredValueTaskAwaitable<T> Cat<T>(
                this ValueTask<T> task
                )
            {
                return task.ConfigureAwait(true);
            }

            public static ConfiguredValueTaskAwaitable<T> Caf<T>(
                this ValueTask<T> task
                )
            {
                return task.ConfigureAwait(false);
            }

            public static ConfiguredTaskAwaitable<T> Cat<T>(
                this Task<T> task
                )
            {
                return task.ConfigureAwait(true);
            }

            public static ConfiguredTaskAwaitable<T> Caf<T>(
                this Task<T> task
                )
            {
                return task.ConfigureAwait(false);
            }

            public static ConfiguredTaskAwaitable Cat(
                this Task task
                )
            {
                return task.ConfigureAwait(true);
            }

            public static ConfiguredTaskAwaitable Caf(
                this Task task
                )
            {
                return task.ConfigureAwait(false);
            }
        }
;";


        [TestMethod]
        public async Task EmptyTest()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task CompletedTaskTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{
            public static async Task DoAsync()
            {{
                {{|#0:await Task.CompletedTask|}};
            }}
        }}

{CaHelper}
    }}
";

            var expected = VerifyCS.Diagnostic(ConfigureAwaitAnalyzerAnalyzer.DiagnosticId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task CompletedTaskConfigureAwaitFalseTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.CompletedTask.ConfigureAwait(false)|}};
            }}
        }}

{CaHelper}

    }}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task CompletedTaskConfigureAwaitTrueTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.CompletedTask.ConfigureAwait(true)|}};
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task CompletedTaskCafTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.CompletedTask.Caf()|}};
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task CompletedTaskCatTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.CompletedTask.Caf()|}};
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }



        [TestMethod]
        public async Task FromResultTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.FromResult(0)|}};
            }}
        }}

{CaHelper}

    }}";

            var expected = VerifyCS.Diagnostic(ConfigureAwaitAnalyzerAnalyzer.DiagnosticId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task FromResultConfigureAwaitFalseTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.FromResult(0).ConfigureAwait(false)|}};
            }}
        }}

{CaHelper}

    }}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task FromResultConfigureAwaitTrueTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.FromResult(0).ConfigureAwait(true)|}};
            }}
        }}

{CaHelper}

    }}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task FromResultCafTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.FromResult(0).Caf()|}};
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task FromResultCatTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await Task.FromResult(0).Cat()|}};
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }


        [TestMethod]
        public async Task MethodTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask()|}};
            }}

            public static Task GetTask()
            {{
                return Task.FromResult(0);
            }}
        }}

{CaHelper}
    }}";



            var expected = VerifyCS.Diagnostic(ConfigureAwaitAnalyzerAnalyzer.DiagnosticId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task MethodConfigureAwaitFalseTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask().ConfigureAwait(false)|}};
            }}

            public static Task GetTask()
            {{
                return Task.FromResult(0);
            }}
        }}

{CaHelper}

    }}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task MethodConfigureAwaitTrueTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask().ConfigureAwait(true)|}};
            }}

            public static Task GetTask()
            {{
                return Task.FromResult(0);
            }}
        }}

{CaHelper}

    }}
";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task MethodCafTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask().Caf()|}};
            }}

            public static Task GetTask()
            {{
                return Task.FromResult(0);
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task MethodCatTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask().Cat()|}};
            }}

            public static Task GetTask()
            {{
                return Task.FromResult(0);
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }


        [TestMethod]
        public async Task ValueMethodTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask()|}};
            }}

            public static ValueTask<int> GetTask()
            {{
                return new ValueTask<int>(0);
            }}
        }}

{CaHelper}

    }}";

            var expected = VerifyCS.Diagnostic(ConfigureAwaitAnalyzerAnalyzer.DiagnosticId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task ValueMethodCafTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask().Caf()|}};
            }}

            public static ValueTask<int> GetTask()
            {{
                return new ValueTask<int>(0);
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }

        [TestMethod]
        public async Task ValueMethodCatTest()
        {
            var test = @$"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    namespace ConsoleApplication1
    {{
        class Program
        {{   
            public static async Task DoAsync()
            {{
                {{|#0:await GetTask().Cat()|}};
            }}

            public static ValueTask<int> GetTask()
            {{
                return new ValueTask<int>(0);
            }}
        }}

{CaHelper}

    }}";

            await VerifyCS.VerifyAnalyzerAsync(test, new DiagnosticResult[0]);
        }


    }
}
