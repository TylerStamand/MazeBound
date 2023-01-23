using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {
    public static Direction GetOppDirection(Direction direction) {
        if (direction == Direction.North) {
            return Direction.South;
        } else if (direction == Direction.South) {
            return Direction.North;
        } else if (direction == Direction.East) {
            return Direction.West;
        } else {
            return Direction.East;
        }
    }
}