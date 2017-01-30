# Rekompenco
A cross platform, cross game engine, cross game universe **Achievement** framework.

---

Note that this version is currently NOT ready for common use. It will be once the file path and file format has been nailed down.

## Concept
Rekompenco provides an API for communicating with other games. It does this by providing achievements in a universal format. The API tries to be simple and easy to add to a project, so that it can be used in game jams with little to no hassle.


![Figure 1: Basic concept.](/docs/readme-concept-01.gif)

### Technical stuff
* The framework is lazy loaded, so that very little setup is required.
* The framework saves to a single file that is referenced by all the games using this Rekompenco.
* The framework provides some helper functions to manage the file, but otherwise leaves the data exposed.
* The format also allows for a little data to be embedded in the achievement. The data is most likely base64 encoded.
* The format contains a data type field which specifies what data you can expect. (TODO: create map of the datatypes. 0 means none.)
* The file format cannot contain lines longer than 512 chars.

The file format currently is like a csv but `|` separated. the columns are:

`string : ID|string : NAME|string : DESCRIPTION|uint16 : DATATYPE|string : DATA`

The default paths are:
```
Windows: [Drive]:\Users\[USER]\AppData\Roaming\Rekompenco\default_rekompenco.rkpc

Mac: [TODO]

Linux: [TODO]
```


## Examples
```csharp
// C# Basic usage
// Unlocking an achievement.
var achievementID = "GAME_NAME_COOL_ACHIEVEMENT";
var achievement = new Rekompenco.Achievement(achievementID, "The Cool Achievement");
Rekompenco.Utility.UnlockAchievement(achievementID, achievement);
// ...

// Checking if an achievement has been unlocked.
if(Rekompenco.Utility.HasAchievement("BURGER_GAME_1000_ATE")) {
    // Your game code here ... Example:
    philip.Say("Wow you must really like burgers");
    // ...
}
```

```cpp
//C++ (TODO)
```

## Contributing
The goal with the API has been to be small but easy and with as little setup as possible.
That said, different games might have different needs, which is why a side goal is to keep the data exposed. A third goal is to have a unified API cross language/engine/platform but if it makes the code really gnarly then omit this goal.

But be feel free to submit a pull request, even if it goes against these goals. You might be more right than the goals.

Check out the [Road map](ROADMAP.md) to see some of the wanted improvements.

## License
MIT check out the [LICENSE](LICENSE)-file.

---

Also, if you were wondering what *Rekompenco* is; it is [Esperanto](https://en.wikipedia.org/wiki/Esperanto) for [*reward*](https://en.wiktionary.org/wiki/rekompenco).
