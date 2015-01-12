![](https://raw.githubusercontent.com/Mikescher/BefunUtils/master/README-FILES/icon_BefunRep.png) BefunRep
========

A common problem with Befunge is the *(efficient)* representation of big numbers. *(= putting a number on the stack with the least amount of instructions)*

BefunRep is a commandline tool to generate a list of representations for all numbers in a specified range. I'm pretty sure the calculation of the optimal representation is a NP complete. But BefunRep tries to to find good representations for all numbers via various algorithms. And it does a pretty good job. It finds for all the numbers between -1 million and +1 million representations with a maximum of eleven characters.

![](https://raw.githubusercontent.com/Mikescher/BefunUtils/master/README-FILES/BefunRep_Main.png)

Here an example call to BefunRep:

> **\> BefunRep.exe -lower=0 -upper=1000 -iterations=3 -stats=3 -q -safe="safe.bin" -out="out.csv" -reset**

This calculates the numbers from **0** to **1000**.  
With a maximum of  **3** iterations *(use -1 to calculate until no more improvmenets can be found)*
Safe the results in binary format in the file **safe.bin**  
If the safe already exists it will be reseted (-**reset**)
And exports the results readable to **out.csv**  

You can also update an existing safe

> **\> BefunRep.exe -safe="safe.bin" -iterations=-1**

Or don't calculate anything and only output an existing safe into another format

> **\> BefunRep.exe -safe="safe.bin" -iterations=0 -stats=3 -out="out.json"**

Here an example of a few calculated values:

Number | Representation
-------|----------------
113564 | `"tY!"**3/`
113565 | `"qC-"**3/`
113566 | `"[' "**2-`
113567 | `"[' "**1-`
113568 | `"[' "**`
113569 | `"~~U"++:*`
113570 | `"[' "**2+`
113571 | `"wj"*5+9*`
113572 | `"[' "**4+`
113573 | `"[' "**5+`
113574 | `"E~(&"*+*`

Online Collection
=================

I uploaded an collection of all representations from 0 to 2^24 to a **[githup repository](https://github.com/Mikescher/Befunge_Number_Representations)** - in case you need to look up a few numbers

Download
========

You can download the binaries from my website [www.mikescher.de](http://www.mikescher.de/programs/view/BefunUtils)

Set Up
======

*This program was developed under Windows with Visual Studio.*

You don't need other [BefunUtils](https://github.com/Mikescher/BefunUtils) projects to run this.  
Theoretically you can only clone this repository and run it.  
But it could be useful to get the whole BefunUtils solution like described [here](https://github.com/Mikescher/BefunUtils/blob/master/README.md)  

Commandline parameters
======================

Parameter   | Description
------------|-------------------------------------------
lower       | The lower bound for generation (will be retrieved automatically when an existing safe file is provided)
upper       | The upper bound for generation
notest      | Don't automatically test generated code
quiet       | Don't output every improvement to the console/log
reset       | Reset the current safe file
algorithm   | Only use one specific algorithm
safe        | The safe file to use (or create a new one if the file does not exist)
out         | The output file, specify the format via the file extension (csv, json, xml). If this is not set no output file will be generated
iterations  | The amount of iteratoins the program will run (`0` won't generate new numbers and `-1` will run until there are no more improvements to be made)
stats       | The amount of statistics to display at the end
log         | The log file where every console output is logged. This can either be a file or a directory.
maxoutput   | The maximum number of numbers in a single output file before it is split into multiple files.
help        | Show the summary of all commands
