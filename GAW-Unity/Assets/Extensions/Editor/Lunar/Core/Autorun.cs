//
//  Autorun.cs
//
//  Lunar Plugin for Unity: a command line solution for your game.
//  https://github.com/SpaceMadness/lunar-unity-plugin
//
//  Copyright 2016 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

﻿using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using LunarPluginInternal;

#if LUNAR_DEVELOPMENT
[assembly: InternalsVisibleTo("Test")]
#endif

namespace LunarEditor
{
    [InitializeOnLoad]
    partial class Autorun
    {
        private static bool isPlaying;

        static Autorun()
        {
            isPlaying = EditorApplication.isPlaying;

            EditorApplication.update += EditorApp.Update;
            EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged()
        {
            if (isPlaying ^ EditorApplication.isPlaying)
            {
                isPlaying = EditorApplication.isPlaying;
                Editor.OnPlayModeChanged(isPlaying);
            }
        }
    }
}
