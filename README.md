# CmarNYC's Sims 3 Ripper --- Daisy Sermech's edition and fixes (WIP).

Unfortunately Sims 3 Ripper is full of bugs, instabilities and issues, and there is no definite working version. I've been jumping versions myself, spending hours in CAS to rip at least something of the character into Blender. The original creator, CmarNYC, has passed away, and the S3 ripper has been barely touched, while TS4 ripper has been developed further. I've decided to look into open source code myself too, and luckily it was in C#, a programming language I know rather well. So, following the next few days I've looked into the functionality and try to stabilise the program. As a result, I've also implemented a few fixes, mostly regarding hair and makeup tinting and accessory texture handling, so dare I say, the sim from this post's image was ripped without a hitch.
Of course, this is a very early version, I'm not claiming I've fixed or tested everything, so I'm kinda asking you: if you're a Sims 3 creator, give this build a try, and come back with feedback, problems or propositions.

So small instruction time:

1) Once you unzip the archive, run the .exe file. In Settings set up Sims 3 BASE GAME LOCATION AND IT SHOULD (every time I say 'should' I say it with a shrug, so don't shoot me if there are issues) PICK THE EPs AND SPs, also set up mods and saves locations.

2) Ofc, edit a character in CAS and save them as .sim. Press 'Select' in the Ripper program and navigate to Saved Sims folder.

3) NOTE: THE RIPPER DOESNT RECOGNISE SIMS3PACKS CONTENTS, STORE CONTENT AND ETC. IF YOU KNOW OR SUS THAT YOUR SIM HAS SOME CC FROM S3PACKS, EXPORT THEM TO SIMS3PACK, USE MULTI INSTALLER AND CONVERT THE FILE TO PACKAGE. AND JUST DROP IT TO MODS/PACKAGE (even while running the game, no problem).

4) Select your Sim and Outfit. Only then it will start working (One of the optimisations, bc previously it loaded all the outfits and sim info in memory, now it does only for certain outfit. Should save some time). Save the sim into DAE and import it in Blender.

5) If you have issues, let me know.

Multi Installer: https://modthesims.info/d/364038/delphy-s-sims-3-pack-multi-extracter-updated-5th-sept-2009.html
OG Sims 3 Ripper thread and code: https://modthesims.info/showthread.php?t=639447

My Patreon (FREE): https://www.patreon.com/daisysermechstudio
