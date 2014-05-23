---
layout: post
title: "05-17-2014"
description: "Moving forward ..."
tags: [arbaro, dev-update]
comments: true
---

### Status as of May 18, 2014

#### Changes

* A decent camera with zoom, pan & rotate.
* The "green" lines shows leaves.
* The subsegments are now generated with the skeleton.
* A triangle mesh is generated and displayed using solif wireframe
* A mesh (quads) is created for the leaves
* A few enhancements here and there.


#### Next step: 

* Debug the mesh generation. 
* Setup a menu to switch from skeleton to wireframe display.
* Menu to filter display of levels
* Export as .obj file format
* Parameters are not properly displayed when loading a tree definition file


#### A screen-shot

<figure>
	<a href="http://khazanjm.github.io/arbaro-csharp/images/screen_2014_05_17b.jpg"><img src="http://khazanjm.github.io/arbaro-csharp/images/screen_2014_05_17b.jpg"></a>
	<figcaption>How Arbaro C# does look at the time of this post.</figcaption>
</figure>

#### The mesh

I made a quick mesh generator. It gets displayed using a Solid Wireframe shader I found at 
NVidia [here](http://developer.download.nvidia.com/SDK/10.5/direct3d/Source/SolidWireframe/Doc/SolidWireframe.pdf).

Unfortunately it looks like there are several bugs in the mesh generation... something related with subsegments.
I have to say I didn't really understand how Arbaro originally handle those...

<figure>
	<a href="http://khazanjm.github.io/arbaro-csharp/images/screen_swf.jpg"><img src="http://khazanjm.github.io/arbaro-csharp/images/screen_swf.jpg"></a>
	<figcaption>How Arbaro C# does look at the time of this post.</figcaption>
</figure>


#### Mesh with leaves

Quick update with "leaves"...

<figure>
	<a href="http://khazanjm.github.io/arbaro-csharp/images/screen_leaves.jpg"><img src="http://khazanjm.github.io/arbaro-csharp/images/screen_leaves.jpg"></a>
	<figcaption>How Arbaro C# does look at the time of this post.</figcaption>
</figure>