Thanks to EA publishing the source code to ImagePacker as part of the C&C Generals source release, I have been able to make my SAGE UI editing tools even better.

Now you can edit SAGE UI the way EALA did back in the day and get usable output (including properly packed textures).

You can download the tools from
https://mega.nz/file/i48DybTB#CTVow2EapX5prmehlKScoOPdg7HEjFtkZ0rokf53OSs
You can get the source code from
https://github.com/jonwil/swf2apt

You also need one of the following UI packs:
Tiberium Wars: https://mega.nz/file/L48VkISJ#Fbn8nQft1nKMgIECNGL6IlYX7HA6XxMewyedG9MRs5U
Kane's Wrath: https://mega.nz/file/z59GwaSB#OI9CaEba54DJarGWjQLG_GGs5nW-wlAWH6SgO5l_CCI
Red Alert 3: https://mega.nz/file/ah8UXZQD#STDR2VUKf3G36wVrZiIrVh4yWV1uB24Sks0UeVRX-PU
Uprising: https://mega.nz/file/2hdVCRja#DKEXE299xz5LpN_LWeQA4xd2357S_f-EMAXrIYNPDls
Tiberian Twilight: https://mega.nz/file/f5dw1CyQ#d5rsHURBF5w15QSzsmoYISHEB3KH85Hkuk19aLnoMnA

All 5 packs match the last released patch for the 5 games and contain source swf files.

To use it, you need to:
1.Download the program and unpack it into a location of your choosing
2.Download the right UI pack for the game you want to mod
3.Figure out which UI element you want to edit
4.Take the .swf file from that UI element and edit it with a tool that edits Flash swf files (no I am not going to provide a tutorial here on how to edit swf files)
5.Once you have an edited swf file, you run "eaf2apt <filename> <outpath>" where <filename> is the path to the swf file. and <outpath> is the location where you want the output to be generated in (it needs to be a path that doesn't exist and/or can be erased since eaf2apt will delete the contents of that path). Add --ra3 to the end of the command line if you are modding RA3 or Uprising and --tt to the end of the command line if you are modding TT
6.That will generate a set of files you can include in your mod. If you don't know how to use UI stuff in your mod, this tool probably isn't for you...

Feel free to modify the source code and to distribute your modifications but if you distribute modified binaries you also need to distribute modified source code to comply with the various licenses and I would appreciate a heads up that you are doing things with my code Smile

Any questions about this stuff or how to use it (e.g. if the instructions aren't clear), please feel free to post here or hit me up on Discord (I am "Jonathan Wilson" on most of the C&C/SAGE/modding discords)

If you want to know more about how this works internally, read the source code (or ask me questions on Discord Smile