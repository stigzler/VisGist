# Research

## AvalonEdit

### Syntax Highlighting

#### Custom xshd

Header:
```xml
<SyntaxDefinition name="HTML" extensions=".htm;.html" xmlns="http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008">
```

You **can** put in hex RGB values
3
All Languages syntax testing codes:
https://github.com/sharkdp/bat/tree/master/tests/syntax-tests/source

## Authentication 

Use Personal Access Token in Github, not a GitHub App. 

https://github.com/settings/personal-access-tokens

### Encryption

Encrypt PAT with DPAPI: 
https://weblogs.asp.net/jongalloway/encrypting-passwords-in-a-net-app-config-file


## Octokit.Gist research

### Stars
Not held in the Gist or GistFile itself. Test via

`GistClient.IsStarred(string gistId)`

Star/Unstar via:

`GistClient.Star(string gistId)` / `GistClient.Unstar(string gistId)`

### Comments
Not directly accessible from Gist. Have to use `GistCommentsClient`:

`GistCommentsClient.GetAllForGist(string gistId)`

### Updating

If update Gist Description , have to use `GistUpdate`

## Misc

Example Extension location:

..\appdata\local\microsoft\visualstudio\17.0_83cd348aexp\extensions\steve hall\visgist

Live install location: 

..\AppData\Local\Microsoft\VisualStudio\17.0_83cd348a\Extensions

via:
```c#
System.Reflection.Assembly.GetExecutingAssembly().Location
```
