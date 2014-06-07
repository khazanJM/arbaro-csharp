---
layout: post
title: "05-31-2014"
description: "Moving forward ..."
tags: [arbaro, dev-update]
comments: true
---

### Status as of May 31, 2014

#### Changes
 
* Not that much visible changes
* The orbiting camera is now orbiting without gimbal lock
* Editing a shader (outside Visual Studio) and saving it while the program is running will reload it without reloading the program (convenient to debug shaders).
* Visibility of levels can now be triggered through a menu (for both wire and solid wireframe display)
* Added an half-edfe mesh representation. Not used right now... but that will come soon. The implementation comes from Alexander Kolliopoulos[Alexander Kolliopoulos](http://www.dgp.toronto.edu/~alexk/halfedge.pdf)


#### Next step: 

* Move the mesh generation to the half edge data structure
* Create a quad mesh (?) and display it properly (showing quads)
* Generates UV and normals + smooth shading
* OBJ export
* Review all parameters





