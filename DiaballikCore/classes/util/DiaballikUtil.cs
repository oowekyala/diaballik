﻿using System;

namespace Diaballik.Core.Util {
    public static class DiaballikUtil {
        public static void Assert(bool condition, string message) {
            if (!condition) {
                throw new ArgumentException(message);
            }
        }
    }
}