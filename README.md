## First glimpse

```csharp
//Generate objects in default pool for given class
Cache<MyClass>.Generate(10);
//Retrieve object from pool
var instance = Cache<MyClass>.Pop();
//Return object to pool
Cache<MyClass>.Push(instance);
//Shortcut for accessing default pool
var myPool = Cache<MyClass>.DefaultPool;
```
## Overview
  Unity Cache is simple solution for pooling objects. Especially made for Unity3D but its core can be used outside of this engine.
  * Custom Inspector for Unity3D
  * Can be used for every class not only GameObjects;
  * Very flexible and easy to use
## Code Example

```csharp
Cache<GameObject>.SetFactory(() =>
            {
                var gm = Instantiate(CoinPrefab);
                gm.transform.parent = transform;
                return gm;
            })
            .SetResetAction(o => o.SetActive(false))
            .Generate(10);
```
Using specified named caches
```csharp
Cache<GameObject>.Caches["Coins"]
            .SetFactory(() =>
            {
                var gm = Instantiate(CoinPrefab);
                gm.transform.parent = transform;
                return gm;
            })
            .SetResetAction(o => o.SetActive(false))
            .Generate(10);
```
## Motivation

Pooling objects can be the most important performance boost in every game or application,
Main purpose of this library is to made this as simple and straightforward to operation as possible.

## Installation

1) Add this repository as submodule to your projects, prefered directory "{YouGame}\Submodules\"

2) Symlink "UnityCache" folder to your "Assets\Plugins\" folder
## API Reference

## Tests

Tests are written with SpecsFor library.

## Contributors



## License
GNU GENERAL PUBLIC LICENSE
