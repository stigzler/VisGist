# Design Document

## Key Info

Author: stigzler

Of: Crackhouse Coding

Date Started: April, 2024

Status: In Development

## Brief

MVP: Design a Visual Studio Extension that can retrieve and edit Gists. 

Desirable features:

1. Filter Gists for large collections
1. Integrated code editor
1. Star/Unstar Gists
1. View/add comments

Stretch Goals

1. View other user's gists?
1. Fork Gist
1. View history

## Architecture

### GistsViewModel:

Linked to TreeView and List\<Gist\>

Can use various methods to populate. E.g. `GistClient.GetAll`, `GistClient.GetAllStarrred` `GistClient.GetAll(DateTimeOffset)` (gets all since that date) - is this better done by a filter?

Possible filters + how achieved:


|Filter|How achieved|
|-|-|
|All|GistsClient.GetAll|
|Starred|GistsClient.GetAllStarred|
|Public|GistsClient.GetAllPublic|
|Private|GistsClient.GetAll + Filter on Gist.Public == false|


Features:

1. Can search on GistFiles - view will narrow down to matches
1. Add Gist (Main Toolbar)
1. Delete Gist (ContextMenu)

#### Controls

ListView

In Main Toolbar: 

### GistViewModel:

Features:
1. Change Description
1. Star/Unstar
1. View Comments (Tabbed control alt between gistFile and gistComments?)

Info:

1. Date Created + Updated
1. Starred
1. Number of Comments

#### Controls


TextBox for Description, ToggleButton for Star, ?Separate Comment Viewer

### GistFileViewModel:

Features:
1. Change Filename
2. Change Contents
2. Change Language

#### Controls

Syncfusion edit control for code

Raw URL link (Icon) - opens in browser








### Stretch Goals

#### Features

1. [ ] View other user's gists?
1. [ ] Fork Gist
1. [ ] View history
1. [ ] 



