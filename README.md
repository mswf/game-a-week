# Specialization (Q3 2015-2016) report - Steff Kempink
###### Based on the game a week concept

## Abstract  
In this article I will report on the work, insights and results of 8 weekly experimental projects I made. By researching a select amount of AI, physics and procedural generation techniques and applying them in my work I gained a lot of experience in little time.
All the results are stored in a public Github repository.

## Workflow
I started by reading/researching different concepts through articles, papers and presentations. From this pool of information I chose a concept I wanted to work with more.
Using Unity I would build a working implementation, researching more only when I got stuck. After this was running I would couple it with a crude game concept, just something to give context for any further work.


## Week 00
This was the inception phase of the project. Here I decided on the subject for my Specialization. This included defining my scope and motivating the merits of this workflow.
I also set up my working environment. I decided to host the project in an open Github repository. Unity as a platform was an easy decision as I had a lot of previous experience with this engine. I wanted to gather some utility libraries I anticipated I would need. I limited myself to free and open source assets.

* [DebugDrawingExtension](https://www.assetstore.unity3d.com/en/#!/content/11396)
Extends Unity's build-in debug line drawing to easily draw primitives wire spheres, boxes and arrows. Every time I used this I got to appreciate the value of visual debugging more.
* [DOTween](https://www.assetstore.unity3d.com/en/#!/content/27676)  
A tweening library. It always pays to add extra flair to objects my having them animate all the time. Although I didn't end up putting a lot of polish into the experiments, it's always good to have this sort of library at your fingertips.
* [GradientMaker](https://www.assetstore.unity3d.com/en/#!/content/29252)
I nice little editor extension that let's you generates gradient textures from within the editor. Very useful for tweaking effects.
* [Lunar Mobile Console](https://www.assetstore.unity3d.com/en/#!/content/43800)
A development utility that let's you view debug logs on a mobile device.

## Week 01
Because I had noticed how unfamiliar I had become with C#, I decided to closely follow some Catlike Coding articles to get back up to speed. This got me back into the right mindset. And made me run headfirst into my what I would focus on this week; pseudorandom noise.

Pseudorandom noise functions are extremely common in computer graphics. They can be used as a base for procedural effects, terrains and animations. Pseudorandom noise has the nice property of yielding varied results, but slightly predictable as compared to pure random noise.

The most well known one is Perlin noise, which when generated in 2D looks a bit like clouds.

<img alt="2D Perlin Noise" src="Report/images/W01_noise.png" width="150" />

A simple use case of this data is to let it drive the height and color of a mesh, which produces this  very rudimentary terrain:

<img alt="Terrain" src="Report/images/W01_terrain.png" width="466" />

<img alt="Terrain" src="Report/images/W01_terrain.gif" />


This is an example of a 2D noise function. For most noise functions, there exist no theoretical cap for up to how many dimensions they go, but most libraries stick to 1, 2, 3, 4 and 6 dimensional noise. Following [this article](http://web.archive.org/web/20160318140201/http://catlikecoding.com/unity/tutorials/noise/), I only implemented 1, 2 and 3 dimensional Value and Perlin noise for my purposes.

I had the idea to use 3D Perlin noise to create a space nebula you can fly through. But a good looking volume of scattered points requires more than just the noise function. Because I would be using billboarded particles, I couldn't just place one at every n'th  point because the grid would be too obvious. Just randomly offsetting the points also doesn't work, [as this article illustrates](http://web.archive.org/web/20141222220009/http://www.gamasutra.com/view/feature/1648/random_scattering_creating_.php?print=1):

While I was researching procedural content generation, I had read [an article by Herman Tullleken that described how Poisson Disk sampling worked](http://web.archive.org/web/20160330213235/http://devmag.org.za/2009/05/03/poisson-disk-sampling/). The algorithm essentially starts at a random point, then creates "x" amount of new points around itself based on a minimum and maximum distance. Then it discards points that are too close to already existing points and repeats these steps until the area/volume is all filled up. If you want to read exactly how I strongly recommend the article.

In its conclusion, Tulleken mentions how driving the minimum distance of the algorithm by a noise function instead of constant values can yield very nice results. This was the part that I remembered as I began on my nebula.

I started a mock implementation of the algorithm, but then I saw how the article linked to a freely distributed C# 2D implementation of Poisson Disk sampling. To save time, I took [this implementation by Renaud Bédard](http://theinstructionlimit.com/fast-uniform-poisson-disk-sampling-in-c) and converted it to 3D instead of writing it from scratch.

Having the base logic already be implemented saves a lot time. However it still took me longer than expected to convert it to 3D and then debug the ensuing issues that came with this. The algorithm also ate up memory as the 3D version of finding a random surrounding point scaled exponentially compared to the 2D version. This made me want to pay closer attention to potential hotspots in the code.
Using Unity's build-in profiler and logging the raw time in some small test code I found it is much better to substitute all calls to Unity's Mathf library with my own. Statistics showed that calling overhead on distance and lerp functions was reduced by 80%.
So finally I made the Min and Max Distance of the Poisson Disc sample from the 3D noise. I then spawned particles on these coordinates,  basing their color and lifetime on the same noise values. Iterating through the resulting array nicely shows how Poisson Disk sampling finds points in space:

![Ordered particle field](Report/images/W01_sampling.gif)

However, this ripple effect looks very artificial, especially as the particle field starts looping. So after shuffling the resulting points this is the end result:

![Shuffled particle field](Report/images/W01_sampling_shuffle.gif)

On the last day I added a simple spaceship. I quickly threw together some basic controls and exhaust effects. I had tried making a "chunk" system that was supposed to help me spawn new segments as the player moved, but it didn't solve the real issue of many of the 3D calculations being prohibitively slow. So I had to box the spaceship inside a very large cube.

![Week 01 end result](Report/images/W01_space.gif)


## Week 02
The previous week had yielded something with a very minimal amount of interaction. I started out this week with the goal by planning more gameplay. I wanted to base the game on physics which in retrospect would clearly trip me up.

The concept was to combine the gameplay from minigolf games with action movies where the hero dodges bullets jumping from desk to desk. Based on previous knowledge I knew I couldn't just twiddle with values and derive the movement values from my intent.
* http://gafferongames.com/game-physics/
* http://www.gdcvault.com/play/1023559/Math-for-Game-Programmers-Building

So after reading multiple articles I created the movement controls. The player looks like a capsule, but all movement is actually based on a freely gyrating ball. I worked on the input until I could predictably add the correct amount of force for the ball to roll based on exactly how far I wanted it to be able to move.
![Gif thing](Report/images/W02_cover2.gif)

However the controls didn't feel satisfying so I spun my wheels trying several different input schemes. These results couldn't satisfy me either, so I decided to jump over to work on another part; the shooting and avoiding. This was all very straightforward.

![Gif thing](Report/images/W02_cover.gif)

I should've known better than to make a physics based game before I had a good grip on the mathematics involved. In the end I had to give up on getting the game to work well. I instead doubled down on research so that I would have a better shot at understanding all the requirements the next time something like this would come up.

![Gif thing](Report/images/W02_ramp.gif)

In order to take my mind off physics I spend the last day on miscellaneous bits and pieces. I read up on the Yarn dialogue engine. I added its interpreter to my Unity project, which was designed to not require the user to learn how the Yarn interpreter even worked. However, as learning exercise I then cracked open the code to see how its developers solved some common problems in dialogue systems. I mostly found very straightforward solutions with only few super elegant structures.

https://github.com/InfiniteAmmoInc/Yarn


## Week 03
My objective this week was to start applying some of the lessons I learned last week. Because during the previous week I had found that my experience with physics was totally lacking, I just wanted to dive in and toy with formulas this time around.

I started by implementing spring physics on top of Unity's Physx rigid bodies and colliders. Using the formulas I found in [this article](http://gafferongames.com/game-physics/spring-physics/) as a base, I was off to a pretty good start.

![Gif thing](Report/images/W03_editor2.gif)

It was interesting to play with different rules for when springs would be created/destroyed between points and the effects they had on the motion of the swarm as a whole. In the above animation the points will occasionally start looking for close neighbors, expanding the search range if the amount of neighbors was below a certain threshold. It would then create the springs between everything it found. Springs have a set lifetime which decreases slowly over time and drains a lot faster when the distance between points is further away.

The visualization of every connection's state provides another layer of information. It made it more clear when a tweak I made to the simulation was causing too much instability, like here:

![Gif thing](Report/images/W03_editor.gif)

Learned debugging, got it to build to Android, burned some time getting Android environment up and running (wanting to get log messages on my laptop from my phone).

![Week 03 end result](Report/images/W03_drops.gif)

## Week 04
Android strategy
Made economy
Made factions, with units
Flexible UI
Spend too much time

<img class="size-full wp-image-224 " alt="SPEC_W04_AI" src="http://kempink.eu/wp-content/uploads/2016/04/W04_AI.gif" width="466" height="283" />   
Test subscript

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


## Lessons Learned

## Conclusion
