﻿using System;
using System.IO;
using EVIL.Ceres.ExecutionEngine.Collections;
using EVIL.JSON;
using Array = EVIL.Ceres.ExecutionEngine.Collections.Array;

var t = new Table
{
    ["hello"] = "world!",
    [21] = "37",
    ["hiii"] = new Array(10),
    ["uwu?"] = new Table
    {
        ["nyaa"] = 2222,
        ["sssssss"] = 21.37,
        ["hello world"] = true,
    }
};

var str = EvilJson.Serialize(t);
Console.WriteLine(str);

var parsedValue = EvilJson.Deserialize(File.ReadAllText("test.json"));
// Console.WriteLine(parsedValue.Table!["escaped_characters"].String);
// Console.WriteLine(parsedValue.Array![2].Table!["friends"].Array![0].Table!["name"].String);