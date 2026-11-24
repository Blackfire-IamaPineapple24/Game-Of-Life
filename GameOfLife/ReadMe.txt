This project contains an incomplete version of Conway's Game of Life, coded in MonoGame.

Here are your instructions:

1) Check out the Wikipedia article on Conway's Game of Life. Please read the introduction and the section titled Rules.
https://en.wikipedia.org/wiki/Conway's_Game_of_Life

2) Optional: read the rest of the Wikipedia article if you are interested.

3) Optional: watch this interview with the mathematician John Conway.
https://youtu.be/R9Plq-D1gEk

4) Optional: have a play with this version of Game of Life.
https://academo.org/demos/conways-game-of-life/
You can click on the grid or use the "Advance one step" or "Animate" controls.

5) Open the GameOfLife.sln in Visual Studio 2022.
Compile and run the project - you should see a small, empty window.
(If you get the dotnet tools error, see the instructions below.)

6) Before you edit anything, read through all of Game1.cs and Cell.cs.
Read the code and the comments to get an idea of how this project works.

7) You will have noticed 10 TODOs.
Starting with Game1.cs and then Cell.cs, go through each TODO and fix each one.
Some are fairly easy, others will require some thought. Read the surrounding code and comments for hints.
If you get stuck, ask a classmate for a hint, but try to do it yourself first.
Use Ctrl-F to find all the TODOs if you're missing them. 

8) Once you have completed all the TODOs successfully, you'll be able to compile and run the project.
It should behave like Conway's Game of Life. You should be able to draw cells and watch them evolve.
You can use an online version of Game of Life (like the one in step 4) to check that you get the similar patterns.

9) Once you're satisfied that you are finished, take a screenshot of your project running and post it on Discord.

10) Optional: replace the spritesheet with art you have made or found.



Dotnet tools error:
Go to GameOfLife_Incomplete\GameOfLife\.config\
Right click on the file dotnet-tools.json and choose Properties
If it says that the file is blocked and gives you an option to unblock it, tick that option and then Apply.
Try compiling and running the project again.