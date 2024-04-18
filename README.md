# LilGuy

## Introduction

A companion for your long work day.

![](./Images/lilguy.png)

## How to use

Run it on your computer, resize it and move it wherever you want and it will lighten your day.

## Settings

You can adjust when Lil Guy ask for a break, a meal or when he is leaving using the appsettings.json file at the root of the software.

Hour format is `HH:mm`.

`breaksTime` and `mealsTime` are array but `shutdownTime`, `runOnStartup`, `color` and `halo` are single values.

Example :
```json
{
  "breaksTime": [
    "10:30",
    "16:00"
  ],
  "mealsTime": [
    "12:10"
  ],
  "shutdownTime": "17:50",
  "runOnStartup": false,
  "color": "#000",
  "halo": "#fff"
}
```