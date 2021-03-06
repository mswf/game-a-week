//
//  BaseUpdatableList.cs
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

﻿using System.Collections.Generic;

namespace LunarPluginInternal
{
    abstract class BaseUpdatableList<T> : BaseList<T>, IUpdatable where T : class, IUpdatable
    {   
        protected BaseUpdatableList(T nullElement)
            : base(nullElement, 0)
        {
        }

        protected BaseUpdatableList(T nullElement, int capacity)
            : base(nullElement, capacity)
        {   
        }

        protected BaseUpdatableList(List<T> list, T nullElement)
            : base(list, nullElement)
        {
        }

        public virtual void Update(float delta)
        {
            try
            {
                Lock();
                
                int elementsCount = list.Count;
                for (int i = 0; i < elementsCount; ++i) // do not update added items on that tick
                {
                    list[i].Update(delta);
                }
            }
            finally
            {
                Unlock();
            }
        }
    }
}
