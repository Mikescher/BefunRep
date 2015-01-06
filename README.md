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


Download
========

You can download the binaries from my website [www.mikescher.de](http://www.mikescher.de/programs/view/BefunUtils)

Set Up
======

*This program was developed under Windows with Visual Studio.*

You don't need other [BefunUtils](https://github.com/Mikescher/BefunUtils) projects to run this.  
Theoretically you can only clone this repository and run it.  
But it could be useful to get the whole BefunUtils solution like described [here](https://github.com/Mikescher/BefunUtils/blob/master/README.md)  