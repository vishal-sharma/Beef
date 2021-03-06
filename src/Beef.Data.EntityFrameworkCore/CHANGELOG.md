﻿# Change log

Represents the **NuGet** versions.

## v2.1.7
- *Enhancement:* Sprinkled `Task.ConfigureAwait(false)` as per https://devblogs.microsoft.com/dotnet/configureawait-faq/.

## v2.1.6
- *Fixed:* Introduced FxCop Analysis to `Beef.Data.EntityFrameworkCore`; this version represents the remediation based on the results.

## v2.1.5
- *Fixed:* Introduced FxCop Analysis to `Beef.Core`; this version represents the remediation based on the results.

## v2.1.4
- *Enhanced:* `EfDbMapper<>.AddStandardProperties` now returns `this` instance to enable fluent-style method-chaining.

## v2.1.3
- *Fixed:* InvokerBase was non functioning as a generic class; reimplemented. Other Invokers updated accordingly.

## v2.1.2
- *Fixed:* Inconsistent version numbers.

## v2.1.1
- *New:* Initial publish to GitHub.