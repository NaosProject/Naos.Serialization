[![Build status](https://ci.appveyor.com/api/projects/status/bvcnviv085swn42f?svg=true)](https://ci.appveyor.com/project/Naos-Project/naos-serialization)

Naos.Serialization
==================
Serialization logic used by Naos assets for both Bson (Mongo's flavor) and Json (Newtonsoft's flavor).

Naos.Serialization.Domain provides generic interfaces as well as a DateTimeStringSerializer that will work with all DateTimeKinds.

Naos.Serialization.Bson provides both Binary and String serialization in BSON, auto configuration logic, and various helper methods that can be used independently.

Naos.Serialization.Json provides both Binary and String serialization in JSON.

Naos.Serialization.PropertyBag provides both Binary and String serialization in a string/string dictionary.