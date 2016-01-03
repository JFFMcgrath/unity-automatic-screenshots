# Unity - Automated Progress Screenshots

An automated utility that takes progress screenshots of your Unity work. A nice way to skirt that end-of-project regret: "I wish I had progress screenshots to see the evolution of this!"

On certain events - this will save a screenshot with and without UI (exploting a bug in Unity's screenshot size argument that leads to no UI being shown - it may be fixed in some update, so this line will be irrelevant).

*Screenshots are saved:**

* Everytime you save a scene
  
* A given (randomized) interval after you press 'Play' (between SCREENSHOT_INTERVAL_FIRST_LOWER and SCREENSHOT_INTERVAL_FIRST_UPPER)
  
* At randomized intervals during play (between SCREENSHOT_INTERVAL_LOWER and SCREENSHOT_INTERVAL_UPPER)
  
* Randomization is added to try to 'mix it up' a little, and make sure we aren't always capturing the same moments during play.
