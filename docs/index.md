
# Features

TBD

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
