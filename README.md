# RetroNet-BBS

A retro-inspired BBS system built with C#, bridging the vintage computing era with modern technologies.

## Commodore 64 with Syncterm

![](c64-syncterm.png)

## Windows 11 command prompt with Telnet

![](windows-telnet.png)

## How to start container

* Install Docker
* Put your Bbs source files in a folder (for ex. ```/opt/bbs```)
* start container with 
```
docker run -v /opt/bbs:/data:ro -p 23:23 -p 8502:8502 ghcr.io/intoinside/retronet-bbs:main
```

## How to create your first Bbs

Every bbs must have a file called index.md. It is the main presentation file from which you can navigate the content offered.

You can start with a simple file with this content:

```
# Hello world!
```

Each time you change the content, you need to restart the server/container.

### Links

There are three type of links that can be made.

* link to another md page
```
[Page 1](Page-1)
[Page 2](Page-2)
```
* link to a raw page
```
[Raw Page](raw1.raw)
```
* Link to rss feed
```
<rss title="Apulia Retrocomputing" url="https://www.apuliaretrocomputing.it/wordpress/feed/">
```
* link to a dynamic content plugin
```
<dynamic title="Plugin SampleDynamicContent" pluginname="SampleDynamicContent.MoveCursorOnScreen">
```

### Raw pages

TBD

### Dynamic content plugin

TBD

### Import files

TBD

### Text style

Text style can be changed using color tags. The following tags are available:
* &lt;white&gt;
* &lt;red&gt;
* &lt;green&gt;
* &lt;blue&gt;
* &lt;orange&gt;
* &lt;black&gt;
* &lt;brown&gt;
* &lt;lightred&gt;
* &lt;pink&gt;
* &lt;darkgray&gt; or &lt;darkgrey&gt;
* &lt;gray&gt; or &lt;grey&gt;
* &lt;lightgreen&gt;
* &lt;lightblue&gt;
* &lt;lightgray&gt; or &lt;lightgrey&gt;
* &lt;purple&gt;
* &lt;yellow&gt;
* &lt;cyan&gt;

Note: 

Background and foreground color can be inverted with tags:
* &lt;revon&gt;
* &lt;revoff&gt;
