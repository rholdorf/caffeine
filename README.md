# Caffeine

Caffeine is a lightweight, unobtrusive app that prevents your machine from entering screensaver or sleep mode. It's perfect for those moments when you're working on a task that doesn't involve much mouse movement and you don't want your screen to go dark.

## How it Works

Once Caffeine is running (indicated by a systray icon depicting a cup of coffee), it will monitor for periods of inactivity. If it detects a lack of mouse movement after a while, it will slightly move the mouse to a random position nearby, keeping your screen awake and active.

## Installation

To install Caffeine, follow these steps:

1. Compile
```console
dotnet build -c Release
```
2. Paste a shortcut to the executable in your Start folder (you can access this folder by typing `shell:startup` in the Run dialog).

Once these steps are completed, Caffeine will automatically start when your machine boots.

## Dependencies

Caffeine uses the InputSimulator library by Michael Noonan. More information about this library can be found [here](https://github.com/michaelnoonan/inputsimulator).

## License

Caffeine is free software: you can redistribute it and/or modify it under the terms that you see fit. No need to acknowledge the original source when doing so.

## Support

It is just an app I needed for my work laptop.
Although contributions are not currently needed, any feedback or issues can be reported to the project's GitHub page.
