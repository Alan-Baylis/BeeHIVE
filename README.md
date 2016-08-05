# BeeHIVE
BeeHIVE is a behavior tree solution to model AI in Unity.

----------

# QUICK GUIDE
Here is a tutorial on how to get BeeHIVE up and running on a project. This cover the basic nodes and features, but builds a foundation for tool usage.

## Project Setup
Before we get started, we need the BeeHIVE files, if you downloaded the package from asset store, obviously you already have the files needed, and already imported to your project, so you are good to go. If you downloaded from the public repository, you can open the project that comes with it. After that its done, you can right click the BeeHIVE folder, located under the Assets folder, and click _Export Package_ as shown in figure bellow.


![]
(imagens/expor.png)


A pop-up window will appear, leave everything checked and click _Export_. Chose a location and a name for the package, and click _Save_. Now you have a package that can be imported in any project by  _right clicking the Assets folder / Import Package / Custom Package_, as shown in figure bellow.


![]
(imagens/impor.png)


## Tutorial
Now that you know how to import BeeHIVE into a project, we will go trough a simple tutorial to get you started. First of all, open up the Demo folder located inside the BeeHIVE folder, than open the Tutorial Demo folder and double click the Tutorial scene.

What we are going to do its a simple AI that can play pong against a human, is worth saying that in many cases, such as this, the AI its too simple to even bother using something like behavior trees, this is just a case study to get you used to the core concepts, besides that, the pong example you are about to see is far from a well done and fully developed game, so expect some weird bugs from time to time, and keep in mind that is for learning purposes only. So without further ado, let's do this.

### Project Overview

The tutorial project have almost everything you need for the base game, the most important class being the _PongController_, which holds all the methods related player capabilities, in this case, stuff that has to do with movement. The human controller inherits from this class to control his 'character' with keyboard input, and the AI we are about to create will it directly, but instead of using keyboard input, it will use commands defined in the behavior tree.
You can hit play now and control the player on the left with the arrow keys, but the AI wont give any trouble whatsoever.

### Creating a beehive agent

Let's create a new C\# script and call it _PongAI_. Add the script to the _AIPlayer_ object on the scene and open it up. This script will be the bridge between BeeHIVE and the _PongController_, so the first thing we need to do is to add the BeeHIVE library to it, so the class will look like this:

```
using UnityEngine;
using System.Collections;
using BeeHive;

public class PongAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
```

Now we need to turn _PongAI_ into a BeeHIVE agent, simply by inheriting from a conveniently named class called _BeeHIVEAgent_. Now your code should look something like this:

```
using UnityEngine;
using System.Collections;
using BeeHive;

public class PongAI : BeeHIVEAgent {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
```

_BeeHIVEAgent_ is a very simple class, that aims to ensure that you have the proper setup without much work. If that inheritance ever gets in your way for some reason you can always copy that code directly into yours.

The next step is initialize BeeHIVE and whatever behavior associated with it. We have two ways of doing that, the first its overriding the _Start_ method and calling the base _Start_, and the second way its to call a method called _InitBeeHive_ on your script, at the _Start_. For sake of simplicity we are going with the second one. Your _Start_ method should look like this:

```
	void Start () {
		InitBeeHive();
	}
```

From now on you have a valid agent, all you need its to implement the actual actions that can be used on the system. If you save your code and get back to the editor, you will see some variables at the inspector, but don't worry about those now, we will get there. Last thing before we get to actions, is to get a reference to the _PongController_ script, so we can use it to move the player around. Doing that your _Start_ should look like this:

```
    PongController pongController;
    
    void Start () {
        pongController = GetComponent<PongController>();
    	
    	InitBeeHive();
    }
```

### Implementing valid actions

For a method to be recognizable as a valid action by BeeHIVE, the return type has to be a enum called _BH\_Status_, which can assume values of _BH\_Status.Success_, _BH\_Status.Failure_ and _BH\_Status.Running_.
The _Success_ is returned when a action successfully completed, and _Failure_ is the other way around. _Running_ is returned when the actions need more iterations to complete, or maybe do not complete at all. We will see examples of all of those.

Actions to move up and down are continuous, so there is never a point where it succeed or fail in this particular case, every time we call it, is always on running state. For that reason we can implement the move actions like this:

```
    public BH_Status MoveUp(){
    	pongController.MoveUp();
    	return BH_Status.Running;
    }
    
    public BH_Status MoveDown(){
    	pongController.MoveDown();
    	return BH_Status.Running;
    }
```

The stop action is a bit different, and there is no condition that can prevent its execution in this game, so every time we call it, it can return a success status, like this:

```
    public BH_Status StopMoving(){
		pongController.Stop();
		return BH_Status.Success;
	}
```

The last action we going to implement is to check if the ball is above or below the agent, so it can make a decision to what direction to move. In this example we will implement only a action to check if ball is above.

```
public BH_Status BallIsAbove(){
    float myHeight = pongController.myTransform.position.y;
    float ballHeight = pongController.ball.position.y;
    if(myHeight < ballHeight)
    	return BH_Status.Success;
    else 
    	return BH_Status.Failure;
}
```

Remember to save your code, and get back to the editor.

### Building a tree

Now for the fun part, we are going to create a tree to model the behavior of our opponent. Start by clicking _Window / BeeHIVE Editor_, like shown on figure bellow.

![]
(imagens/window.png)


The node editor will pop up, and if you get confused at any point, you can check the figure bellow.

![]
(imagens/menu.png)


If you don't already know, is recommended that you read a bit about tree structures, so you can have a full understanding of what i am talking about. 
Anyway, in BeeHIVE, the trees are searched on a _depth-first_ fashion, and from left to right, a example of search order can be seen in figure bellow.the order in which we arrange the nodes, will carry meaning to the behavior.

![]
(imagens/search.png)


Lets start building our tree from the bottom. Click the leaf icon on the left, or right click anywhere on the workspace and create two leaves. Those nodes will always be at the bottom of the tree, so they can only have connections coming in. Leaves are a key part of the system, because they will hold references to one of those actions we implemented earlier.
At this point, leaves might have a text field for you to write the name of the method you want to reference, you are free to do it that way, but lets try something easier. At the very top, the first option is called source, and has a drop-down menu for you pick a valid BeeHIVE agent, and hopefully, we already made one, so click the arrow and chose the class you created, like shown in figure bellow.

![]
(imagens/source.png)

Note that after you selected a source the text-field turns into a drop-down list, in which you can easily select one of the actions.

Now we want to check if the ball is above the agent, and if it is, we want the agent to move up. So we are going to select _BallIsAbove_ on the left node, and _MoveUp_ on the right node. 

To connect both leaves we are going to create a sequence node ( the blue one with a arrow ), this node execute his children from left to right, and if any of them returns a _Failure_ status, the sequence node will stop the execution and will also return _Failure_ to its parent. If all the children returns a _Success_ status, the sequence node will also return _Success_ to its parent. Basically, the sequence node act like a AND gate.

Add a sequence node and click the arrow at bottom of the node to initiate a connection. You will see a line being draw, and than you can click in one of the nodes to make a connection. Repeat the process for the other one, make sure it look like figure bellow.

![]
(imagens/conn.png)


If you make any mistakes you can break a connection by clicking the arrow that sits on top of the node.

At this point you can enter a name and click _Save_ to create a usable file for the agent. Open a folder called Trees, that is where all trees are saved, so you might be able to see the one you just created. Now select the AIPlayer object, and at the inspector of the _PongAI_ class you will see a property called _Behavior Tree Obj_. Drag and drop your tree in that property to set it to this agent. 

If you hit play now you can already see some reaction from AI, but it can only go up, lets fix that. 

If you closed the BeeHIVE editor, open it up again, and load your blueprint using the property on the third section.
Now we want to opposite action to happen when the ball is not above the agent, so we are going to create two more leaves, one will be _MoveDown_, and the other will be _BallIsAbove_, but this time we have to invert the result to make node return _Success_ when the ball is not above. To do that we will also need to add a inverter node (the first purple node on the tool bar). And to connect everything up we need another sequence node. Our tree should be something like figure  bellow.

![]
(imagens/conn2.png)


Note that at this point we have a tree with two roots, or even better, we have actually two separate trees, in this case the system would have no idea of what root to use, so this should be avoided. So lets connect these two sequence nodes together with a selection node (blue node with a question mark). The selection node works like a OR gate, which means that if any of its children return a success status, it will not process any further, and will return a success status to its parent. Putting in a simple way, the ai will check if the ball is above, and if it is, it will not check if it is bellow. Now we have a tree like figure bellow.

![]
(imagens/conn3.png)


Right now you can save and hit play, and you will have a really basic ai that move with a very straight forward logic, but does the trick for a learning project such as this.

That last thing we can play with is the tick rate. When you select AIPlayer object and look at the inspector, you will see a variable called, guess what, Tick Rate. That variable define the time interval in which the tree will be searched, and in practical way, can be increase to reduce performance costs if you have to many agents behaving at once, or to quickly adjust the ai reaction time. Try to play with that value and see how the ai changes its behavior, will be almost like adjusting the difficulty. 

I really encourage you to extend this example a bit, implementing new actions, maybe add a laser gun that you can shoot the opponent with, and the ai would have to make decisions on when to shoot, anyway anything that sounds fun. 

I hope this tutorial was useful, give me any feedback you might have to help me improve this doc, including typos or poorly constructed phrases ( struggling a bit with English :) ). 

Enjoy the tool and HAVE FUN.
