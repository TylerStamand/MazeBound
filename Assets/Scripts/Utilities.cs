using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public struct MinMaxInt {
    public int MaxValue;
    public int MinValue;

    public MinMaxInt(int minValue, int maxValue) {
        MaxValue = maxValue;
        MinValue = minValue;
    }
}

[Serializable]
public struct MinMaxFloat {

    public float MaxValue;
    public float MinValue;

    public MinMaxFloat(float minValue, float maxValue) {
        MaxValue = maxValue;
        MinValue = minValue;
    }


    public float GetRandomValue() {
        System.Random random = new System.Random();
        return (float)random.NextDouble() * (MaxValue - MinValue) + MinValue;
    }
}

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
    public static Direction DirectionFromVector2(Vector2 vector) {
        if (vector.x != 0) {
            if (vector.x > 0) {
                return Direction.East;
            } else {
                return Direction.West;
            }
        }
        if (vector.y != 0) {
            if (vector.y > 0) {
                return Direction.North;
            } else {
                return Direction.South;
            }
        }

        //Default
        return Direction.East;
    }


    public static Direction GetDirectionFromAngle(float angle) {
        if ((angle >= -45 && angle < 45) || (angle >= 270 && angle < -325)) {
            return Direction.East;
        } else if ((angle >= 45 && angle < 135) || (angle >= -315 && angle < -225)) {
            return Direction.North;
        } else if ((angle >= 135 && angle < 225) || (angle >= -225 && angle < -135)) {
            return Direction.West;
        } else {
            return Direction.South;
        }
    }

    public static Vector3 GetAngleFromDirection(Direction direction) {
        Vector3 eulerAngles = Vector3.zero;

        switch (direction) {
            case Direction.East:
                eulerAngles.z = -90;
                return eulerAngles;
            case Direction.West:
                eulerAngles.z = 90;
                return eulerAngles;
            case Direction.North:
                eulerAngles.z = 0;
                return eulerAngles;
            case Direction.South:
                eulerAngles.z = 180;
                return eulerAngles;
            default:
                //Probably should error here instead of returning 0
                return eulerAngles;
        }
    }


    public static Vector3 GetAngleFromVector2(Vector2 vector) {
        return GetAngleFromDirection(DirectionFromVector2(vector));
    }

    public static Vector2 GetDirectionVectorFromDirection(Direction direction) {
        Vector2 directionVector = Vector2.zero;

        switch (direction) {
            case Direction.East:
                directionVector.x = 1;
                return directionVector;
            case Direction.West:
                directionVector.x = -1;
                return directionVector;
            case Direction.North:
                directionVector.y = 1;
                return directionVector;
            case Direction.South:
                directionVector.y = -1;
                return directionVector;
            default:
                //Probably should error here instead of returning 0
                return directionVector;
        }
    }

}