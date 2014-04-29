0/ License

I'm not a legal person.
This is basically a port of Arbaro to C# with (possibly) tons of modifications.
To the extent that at some point nothing will be left of Arbaro.
I don't know how Arbaro licensing is involved.
As far as I'm concerned everything I provide here is public domain.
I will add a comment (the like of "source code ported from Arbaro") in all files where it is the case.

1/ Dependencies

- This was made using Microsoft Visual Studio Express 2013
- Using DirectX through the SharpDX bindings
	Can easily be installed through NuGets packages 
	Bindings on DX11 with DX11 effects
	So that you need a decently recent Graphic Adapter to run the software.


2/ Purpose

Arbaro is a nice software. It helps me get into what Weber described in their original paper.
Though Arbaro is getting quite old and I think it deserve a new version.
My goal here is twofold
- Provide this new version
- Enhance it with several of my own ideas  

3/ How to run

Best way is probably to install MS VS Express 2013
Download the full zip of the project
Load the project within it
Install the SharpDX dependencies through NuGets
and click the run button

I'll try to provide a prebuilt version at some point...


Following is my notes on what I am doing and what is coming.

-- April 28 2013 --

Start working...

The first steps are
- Make it possible to display the tree (at least the skeleton) in 3D
- Port Arbaro (the tree creation part, not including the mesh at first)
- Display a tree through loading of the XML file
- Create a GUI matching the original GUI
- Create a basic 3D mesh around the skeleton version of the tree
- Release this first version to GIT



