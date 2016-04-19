# Specialization (Q3 2015-2016) report - Steff Kempink
#### Based on the game a week concept

<img class="size-full wp-image-224 " alt="SPEC_W04_AI" src="http://kempink.eu/wp-content/uploads/2016/04/W04_AI.gif" width="466" height="283" />   
Test subscript

![Gif thing](images/W02_cover.gif)

<a href="http://kempink.eu/resource-list/">
<img alt="SPEC_W04_AI" src="images/W02_cover.gif" width="100" />
</a>

## Abstract  
In this article I will report on the work, insights and results of 8 weekly experimental projects I made. By researching a select amount of AI, physics and procedural generation techniques and applying them in my work I gained a lot of experience in little time.
All the results are stored in a public Github repository.

## Workflow
I started by reading/researching different concepts through articles, papers and presentations. From this pool of information I chose a concept I wanted to work with more.
Using Unity I would build a working implementation, researching more only when I got stuck. After this was running I would couple it with a crude game concept, just something to give context for any further work.


## Week 00
Deciding the subject of the Specialization.
Setting up environment, gather utility libraries I anticipated I would need. Because I limited myself to free and open source assets, many were out of date with Unity's current version. Coupled with my rusty C# memory I spend longer than I wanted fixing issues.

* DebugDrawingExtension  
* DOTween  
* GradientMaker

## Week 01
Because I had noticed how unfamiliar I had become with C#, I decided to closely follow some Catlike Coding articles to get back up to speed. This got me back into the right mindset. And made me run headfirst into my what I would focus on this week; pseudorandom noise.

Pseudorandom noise functions are extremely common in computer graphics. They can be used as a base for procedural effects, terrains and animations. Pseudorandom noise has the nice property of yielding varied results, but slightly predictable as compared to pure random noise.

The most well known one is Perlin noise, which when viewed in 2D looks a bit like clouds.

If you then map these values to height and a color gradient, you get this very rudimentary terrain:

This is an example of a 2D noise function. For most noise functions, there exist no theoretical cap for up to how many dimensions they go, but most libraries stick to 1, 2, 3, 4 and 6 dimensional noise. I only implemented 1, 2 and 3 dimensional Value and Perlin noise for my purposes.

I had the idea to use 3D Perlin noise to create a space nebula you can fly through. But a good looking volume of scattered points requires more than just the noise function. Because I would be using billboarded particles, I couldn't just place one at every n'th  point because the grid would be too obvious. Just randomly offsetting the points also doesn't work, as this article illustrates:

http://www.gamasutra.com/view/feature/1648/random_scattering_creating_.php?print=1

Luckily while I was researching procedural content generation, I had read an article by Herman Tullleken that described how Poisson Disk sampling worked. The algorithm essentially starts at a random point, then creates "x" amount of new points around itself based on a minimum and maximum distance. Then it discards points that are too close to already existing points and repeats these steps until the area/volume is all filled up. If you want to read exactly how I strongly recommend the article.

http://devmag.org.za/2009/05/03/poisson-disk-sampling/

In its conclusion, Tulleken mentions how driving the minimum distance of the algorithm by a noise function instead of constants can yield even more convincing results.

I started a mock implementation of the algorithm, but then I saw how the article linked to a freely distributed C# 2D implementation of Poisson Disk sampling. To save time, I took this implementation by Renaud Bédard and converted it to 3D instead of writing my own.

http://theinstructionlimit.com/fast-uniform-poisson-disk-sampling-in-c

Having the base logic already be implemented saves a lot time. However it still took me longer than expected to convert it to 3D and then debug the ensuing issues that came with this. The algorithm also ate up memory as the 3D version of the mathematics scaled exponentially from 2 dimensions. After profiling I found it is much better to substitute all calls to Unity's Mathf library with my own. Statistics showed that calling overhead on distance and lerp functions was reduced by 80%.
Inspired by the DevMag article, I made the Min and Max Distance of the Poisson Disc sample from the 3D noise. I then spawned particles on these coordinates,  basing their color and lifetime on the same noise values. Iterating through the resulting array nicely shows how Poisson Disk sampling finds points in space:

However, this does not look convincing so after shuffling the resulting points this was the end result:


On the last day I added a simple spaceship. I quickly threw together some basic controls and exhaust effects. I had tried making a "chunk" system that was supposed to help me spawn new segments as the player moved, but it didn't solve the real issue of many of the 3D calculations being prohibitively slow. So I had to box the spaceship inside a very large cube.


## Week 02
The previous week had yielded a result with very minimal amounts of interaction. I started out this week with the goal by planning more gameplay. I wanted to base the game on physics which in retrospect would clearly trip me up.
The concept was to combine the gameplay from minigolf games with action movies where the hero dodges bullets jumping from desk to desk. Based on previous knowledge I knew I'd need to have a good grip on how the player would move.
http://gafferongames.com/game-physics/
http://www.gdcvault.com/play/1023559/Math-for-Game-Programmers-Building
So after reading multiple articles I created movement controls. Although the player looks like a capsule, movement is actually based on a free rolling ball. I worked on the input until I could predictably add the correct amount of force for the ball to roll exactly as far as I designed. However the control didn't feel how I wanted it to, so I spend time trying several different input schemes. The results couldn't satisfy me, so I decided to jump over to work on another part; the shooting and avoiding. This was all very straightforward.
Physics based action game bad idea as I was very inexperienced with Physics. I had to give up on getting the game to work well. I instead doubled down on research to have a better shot at understanding all the requirements the next time something like this would come up.

https://github.com/InfiniteAmmoInc/Yarn
To take my mind off physics for a short while I also read up on the Yarn dialogue engine. Implementing it's Unity interpreter was very straightforward. As a learning exercise I then cracked open the code to see how its developers solved some common problems in dialogue systems using very straightforward solutions.

## Week 03
Physics playground
http://gafferongames.com/game-physics/spring-physics/
Inspired by one specific article on spring physics I wanted to replicate it in Unity on top off the existing PhysX system.
Learned debugging, got it to build to Android, burned some time getting Android environment up and running (wanting to get log messages on my laptop from my phone).

## Week 04
Android strategy
Made economy
Made factions, with units
Flexible UI
Spend too much time

## Week 05
Behavior Tree

http://gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php

After only reading the article on usage, worked on making my own implementation. I had finished 90% of the functionality I was going to use by Tuesday, but it was very hard to debug all the logic errors in my implementation by stepping through the code. The point of using behavior trees is that they are very tool-friendly, so I resigned myself to creating an in-editor visualization tool for my own implementation as well.

I had unknowingly started relying on some C# features that ruined build times for Unity.

https://github.com/libgdx/gdx-ai/wiki/Behavior-Trees

I took my first dive into Unity's editor GUI scripting which was surprisingly consistent in it's API compared to other parts of Unity. So after the active paths the agents were taking were being highlighted and I could add visual counters I was able to spend the rest of the week hammering out any remaining issues. On the last day I focused on getting the same rudimentary behavior from Week04 using this new more flexible system.


## Week 06
Doing more with Behavior Trees.
Create squad logic.
Use Unity navmesh.

Had to improve W05 code to be able to visualize multiple agents, share resources and send messages between agents.

The squad part proved to be too much. After doing more research I found the architecture required to have the flexibility I wanted way out of scope.

## Week 07/08
I merged these last weeks because these two weeks would both be cut short and I didn't want to reduce the scope of the last experiments.

Research
spelunky book
http://www.gamasutra.com/blogs/MikeBithell/20140420/215842/Automatic_avoidance_for_player_characters_on_an_indie_budget.php

Added simple hack 'n slash like boxing mechanics. Made a custom character, helped emphasize movements.
