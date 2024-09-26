# ConfigureAwaitAnalyzer

This is an explicit ConfigureAwait analyzer. It will post a warning if no explicit `ConfigureAwait` in awaitable expression found.

You can suppress the analyzer for your specific method or property if you need:

```C#
            //suppress AsyncConfigureAwait
            public async Task DoInternalAsync()
            {
            }

            //suppress AsyncConfigureAwait
            public static Task DoInternalAsync => Task.CompletedTask;
```

An appropriate nuget can be found [here](https://www.nuget.org/packages/ConfigureAwaitAnalyzer/)
