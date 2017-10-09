**Inspector++**

Inspector++ Is a tool to visually create Unity inspectors

![Main](https://raw.githubusercontent.com/ArthurBrussee/InspectorPlusPlus/master/MainGithub.PNG)

**Set-up**
 
As soon as you imported this asset you can start creating inspectors! Open the Inspector++ window via the Window->Inspector++ menu.

**Adding Inspectors**
Select your scripts from your project window. All MonoBehaviours or ScriptableObjects you selected appear in the window. Click create and you’re done!
 
**Editing Inspectors**
Inspector++ includes a window to visually edit your inspectors. Open it by clicking ‘edit’ next to the file you want to edit.
 
The window displays all variables, with some options. Here are the basic options in order:

- The first toggle hides or unhides the variable from the inspector.
- The display name of the variable
- A toggle to enable a tooltip
- A field for the tooltip
- Arrow to move the variable up
- Arrow to move the variable down
- The last toggle enables/disables writing to the field (Disable it to make a read-only property)
 
From there it depends on the type of variable what kind of controls you see.
 
Under the variable there is a smal seperator. You can drag this seperator to put some space between vars. When the box is large enough you will notice some additional controls. From left to right:

- Toggle to enable or disable a label
- The text of the label.
- Toggle for a Bold label
- Toggle to enable or disable a button
- The text that should appear on the button.
- The function that should be called when the button is pressed
- If you have multiple buttons, you can ‘condense’ them. This means they will stick together and take up less space.
 
You can add a maximum of 16 buttons (4 per line) and 4 labels
 
Tooltips
You can make tooltips via a toggle and textfield in the editor, but also through scripting, in 2 different ways:

1. a summary (C# only)
```C#
///<summary>
///this will be my tooltip!
///</summary>
public float tooltip;
````

2.Attribute:
```C#
[Tooltip(“This will be my tooltip!”)]
public int myInt;
```

**Save to file**
You can also use 'Save to file'. This features produces a C# file you can share in your projects or use in an asset store project. The .cs file is a optimized version of your custom inspector. Note that when you edit your inspector, this file doesn't get updated. You need to regenerate it after any changes.
