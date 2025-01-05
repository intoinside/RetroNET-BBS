
RetroNET-BBS is a modern application written in C# that brings back the experience of the historic Bulletin Board Systems (BBS), integrating it with modern technologies and retro compatibility.

## Main Features

### Markdown Content

The BBS uses a collection of documents in Markdown format as the basis for static content.
Documents are automatically converted into human-readable formats through retro and modern connections, ensuring smooth and clear navigation.

### RSS Feed Integration

The application is able to connect to public RSS feeds, download their contents and make them accessible to users through the BBS.
Users can explore articles and titles directly from the BBS interface.

### Telnet Connection Support

Users can connect to the system using the Telnet protocol, compatible with modern and vintage hardware.
The Telnet server supports multi-user authentication and management.

### Petscii Encoding Compatibility

RetroNET-BBS is designed to accommodate connections from vintage computers, such as the Commodore 64, using the PETSCII character set.
Content is automatically adapted to ensure consistent display on retro terminals.

### Intuitive Navigation

A set of text commands allows users to explore Markdown documents, read RSS articles, and interact with the platform in an authentically retro style.

### Technologies used
* C# and .NET Core: modern and scalable backend.
* Markdown Parser: libraries for Markdown → formatted text conversion.
* Telnet Server: handles multiple connections and variable encodings (e.g. PETSCII).
* RSS Feed: parsing and displaying content in real time.

## Parsers
At this time, parsers are available for these two types of sources:
* Markdown
* Feed Rss

## Encoders
The BBS is able to provide two types of encoding:
* Petscii
* Ascii (Telnet)

## How to start

First page is always a Markdown files, called index.md in the first level of source repository. Every link to another Markdown document is automatically parsed and reachable witha specific key press.

## Customization tags

### Cursor movement

``` Html
<home>: move cursor to top-left

<crsrdown>: move cursor one row down

<crsrright>: move cursor one column right

<crsrup>: move cursor one row up

<crsrleft>: move cursor one column left
``` 

### Colors

``` Html
<white>
<red>
<green>
<blue>
<orange>
<black>
<brown>
<lightred>
<pink>
<darkgray>
<darkgrey>
<gray>
<grey>
<lightgreen>
<lightblue>
<lightgray>
<lightgrey>
<purple>
<yellow>
<cyan>
```
Note: some colors may not display correctly because they are not mapped on the output platform.

It's also possible to reverse foreground/background color with these tags:

``` Html
<revon>
<revoff>
```

## Settings

TBD

# Installation

TBD
