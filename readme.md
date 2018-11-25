![LINQ2LDAP][banner]
[![MIT License][license-badge]][LICENSE]
[![Build Status][travisimg]][travislink]
[![Build status][appveyorimg]][appveyorlink]
[![codecov][codecovimg]][codecovlink]
[![Code of Conduct][coc-badge]][coc]

# Linq2Ldap.Core

This is the core transpiler and parser behind the Linq2Ldap.* NuGet libraries. It can
transpile C# LINQ Expressions into RFC 1960 LDAP filter strings, and it can parse them
back out, again.

If you only want to use the filter transpiler with no additional abstraction,
you can do this:

```c#
    // Goal: produce this filter string from a LINQ Expression
    //     filter = "(&(samaccountname=will*)(&(email=*uiowa*)(!(customprop=123))))";
    
    string filter = new LdapFilterCompiler().CompileFromLinq(
        (MyUserModel u)
                    => u.SamAccountName.StartsWith("will")
                    && u.Email.Contains("uiowa")
                    && u["customprop"] != "123");

    var searcher = new DirectorySearcher();
    searcher.Filter = filter;

    // -- or --

    var searchReq = new SearchRequest(targetOu, filter, /* ... */);
```

Also supported examples:

```c#
(MyUserModel u) => u.Title.Matches("univ*of*iowa"); // (title=univ*of*iowa)
(MyUserModel u) => u.Email.EndsWith("@gmail.com"); // (mail=*@gmail.com)
(MyUserModel u) => u["acustomproperty"].Contains("some val"); // (acustomproperty=*some val*)
(MyUserModel u) => u.Has("somekey"); // (somekey=*)
```

For more information about models and properties, please visit the [Wiki](https://github.com/cdibbs/linq2ldap/wiki).

## Additional abstractions

Wrappers around some of the search functionality in System.DirectoryServices and System.DirectoryServices.Protocols
can be found, [here][deps]. These include, for example, `LinqDirectorySearcher<T>` and `LinqSearchRequest<T>`, respectively.

## Contributing

Do you appreciate the hard work that went into this software? We accept [donations]!

[banner]: https://github.com/cdibbs/linq2ldap.core/blob/master/resources/header.svg "The only way to discover the limits of the possible is to go beyond them into the impossible. - Arthur C. Clarke"
[1]: https://github.com/cdibbs/linq2ldap.core/blob/master/Linq2Ldap.Core/Specification.cs#L42
[travisimg]: https://travis-ci.org/cdibbs/linq2ldap.core.svg?branch=master
[travislink]: https://travis-ci.org/cdibbs/linq2ldap.core
[appveyorimg]: https://ci.appveyor.com/api/projects/status/3snlupymajawn2pv?svg=true
[appveyorlink]: https://ci.appveyor.com/project/cdibbs/linq2ldap-core
[codecovimg]: https://codecov.io/gh/cdibbs/linq2ldap.core/branch/master/graph/badge.svg
[codecovlink]: https://codecov.io/gh/cdibbs/linq2ldap.core
[deps]: https://github.com/cdibbs/linq2ldap
[donations]: https://cdibbs.github.io/foss-giving
[coc-badge]: https://img.shields.io/badge/code%20of-conduct-ff69b4.svg?style=flat-square
[coc]: https://github.com/cdibbs/linq2ldap.core/blob/master/code_of_conduct.md
[license-badge]: https://img.shields.io/badge/license-MIT-blue.svg
[LICENSE]: https://github.com/ossplz/alsatian-fluent-assertions/blob/master/LICENSE
