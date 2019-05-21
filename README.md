# Adventurer
*A C# Interpreter for [Scott Adams adventure games](http://www.msadams.com/adventures.htm) encoded in the **ScottFree** format*.  Two flavours exist: a console version (AdventurerDOS) and a Windows versions (AdventurerWIN).

## About
This project was built with Visual Studio Community using C# with .Net Framework 4.6.1, and is a tribute to Scott Adams. His games where the first I ever played on the TRS-80 in the early 1980s.

Currently, [Brian Howarth's](https://en.wikipedia.org/wiki/Brian_Howarth) Mysterious Adventures aren't fully supported - I haven't fully tested them yet, though they should work, but some features won't be supported.

### Resources used
The main resource for developing this intrepreter was a description of the ScottFree data format, which I found at the [Interactive Fiction Archive](https://www.ifarchive.org/), in the section [Scott Free Interpreters](https://www.ifarchive.org/indexes/if-archiveXscott-adamsXinterpretersXscottfree.html), which was located in the file **scott.zip**.

## Getting the games

The original Scott Adams games can be [downloaded from his website](http://www.msadams.com/downloads.htm).

Please note, this interpreter will only load the games in the **ScottFree** format.

## AdventurerDOS Usage

Exe arguments:

        -g      Load DAT file -gAdv01.dat        
        -g      Specify game save file -lAdv01.sav - must be used with -g
        -t      Display turn counter in game
        -f      Output specified game in commented XML -fAdv01.dat
        -r      Output specified game in XML -rAdv01.dat
        -h      Display help

Whilst playing, save a game by typing:

> SAVE GAME

##AdventurerWIN Usage
Pretty straightforwards, I think.


## Video Walkthroughs

[A growing series of video walkthroughs of the Scott Adams games on my YouTube Channel](https://www.youtube.com/channel/UCNbF1wBiJUt1l_nZgY7yZbA/).
