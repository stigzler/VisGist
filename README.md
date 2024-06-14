# VisGist

## Intro
VisGist allows you to view, create and edit your gists from within Visual Studio. It also includes customizable syntax highlighting to make working with gists easier.

![Screenshot 2024 06 14 112132](Screenshot%202024-06-14%20112132.png)

## Getting Started

Get a Public Access Token via `Github > Settings > Developer Settings > Personal Access Tokens > Fine-grained Tokens`. Ensure you set `Account Permissions > Gists` under the token to "Read and Write"

In `Visual Studio > Tools > Options > VisGist > Github Settings > Personal Access Token` enter your Personal Access  Token.

## Operational

### Authentication

Your Git Personal Access Token (PAT) is stored using encryption based on your local machine's hardware. Thus, you may need to re-generate and re-enter your PAT if you're having problems following any hardware changes. 

### Syntax Highlighting

VisGist uses AvalonEdit for syntax highlighting. It gets the syntax definitions from files stored in:

`%appdata%\VisGist\Highlighting`

There are Light and Dark versions for each syntax definition. If your language is not there, you can always create a new syntax file from the existing files or by reading the `AvalonEdit` docs. 

VisGit has a language auto-select function. It achieves this via checking the extension of the GistFile and matches it against those in the syntax files. If you have corresponding syntax files for a language for both Light and Dark, **you must ensure that all file extensions you want recognized are in the Light version.** Best practice is to ensure these match across the two. 

💡If you create some new languages - why not share with other users, either by sending me the .xshd files, or by forking + creating a pull request? 🙂

## Gist File Headings

The 'Title Gist' (the Gist's name) is just the first GistFile in the collection ascii-betically. Thus, there's facility in VisGist to set a GistFile as the Title Gist by right clicking the file in the treeview. You can also set what character to use to make a GistFile the Title Gist in the options menu. Unfortunately, I couldn't find a way to use invisible characters, so it looks like it does have to be an actual visible character. I use a period/dot as it's the least visible.

## Thanks + Acknowledgments

|Contributor|Thanks for|
|-|-|
|denis.akopyan @ C# Discord|Really friendly and non-rtfm advice around WPF and MVVM|
|conwid @ GitHub|A gist manager project that inspired VisGist
