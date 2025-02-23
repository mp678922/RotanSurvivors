﻿using System;
using UnityEngine;

namespace UnityForge
{
    public class AnimatorPropertyAttribute : PropertyAttribute
    {
        public string AnimatorField { get; private set; }

        public AnimatorPropertyAttribute(string animatorField = null)
        {
            AnimatorField = animatorField;
        }
    }
}
