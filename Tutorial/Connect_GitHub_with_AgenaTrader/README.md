#Connect GitHub with AgenaTrader

To use scripts from this GitHub project you have two options:
- Copy files to your local directories
- Clone GitHub project and link to your AgenaTrader **(our recommendation)**

##Copy files to your local directories
Of course this is easy to do and pretty straight forward if you just want to test scripts.
The **main disadvantage** is that your **local files will not update** if we modify scripts on this GitHub project!

##Clone GitHub project and link to your AgenaTrader
This is **our recommendation** because in this case you always get the latest updates from the GitHub project into your AgenaTrader.

###Install GitHub client
If you want to stay up to date and always get the latest updates you need to download the GitHub desktop client (of course you can use any other Git client). Afterwards you are able to clone this GitHub project to your local workspace using the GitHub client.

[Download GitHub Desktop client](https://desktop.github.com)

[Detailed description how to clone a repository from GitHub](https://help.github.com/articles/cloning-a-repository/)

###Linking files directly into AgenaTrader
Now we need to link the files from our local GitHub workspace into the local AgenaTrader directories. In Linux this would be pretty easy but also on Windows there is a solution for this! We can use **mklink** to work for us.

[Have a look at this mklink guide](http://www.howtogeek.com/howto/16226/complete-guide-to-symbolic-links-symlinks-on-windows-or-linux/)

The following three AgenTrader directories are important for us:
- indicator => `[... path to your AgenaTrader User directory ...]\AgenaTrader\UserCode\ScriptedConditions\`
- condition => `[... path to your AgenaTrader User directory ...]\AgenaTrader\UserCode\ScriptedConditions\`
- strategy => `[... path to your AgenaTrader User directory ...]\AgenaTrader\UserCode\ScriptedConditions\`

The following examples will show you how to link files into AgenaTrader using mklink:

- `mklink /d "C:\AgenaTrader\UserCode\Indicators\github_indicator" "C:\workspace\Github\AgenaTrader\Indicator"`
- `mklink /d "C:\AgenaTrader\UserCode\ScriptedConditions\github_condition" "C:\workspace\Github\AgenaTrader\Condition"`
- `mklink /d "C:\AgenaTrader\UserCode\Strategies\github_strategy" "C:\workspace\Github\AgenaTrader\Strategy"`

Many of our indicators does need the global utility class. Your need to link this folder also into the indicator directory:

'`mklink /d "C:\AgenaTrader\UserCode\Indicators\github_utility" "C:\workspace\Github\AgenaTrader\Utility"`


**Please pay attention to change the directories (source & target) to your local directories!**


