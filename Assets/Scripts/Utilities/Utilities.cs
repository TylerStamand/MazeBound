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
    public int GetRandomValue() {
        return UnityEngine.Random.Range(MinValue, MaxValue + 1);
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
        return UnityEngine.Random.Range(MinValue, MaxValue);
    }
}

public static class Utilities {

    public static Vector2 Difference(this Vector2 a, Vector2 b) {
        return new Vector2(a.x - b.x, a.y - b.y);
    }


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
        float angle = (int)Vector2.Angle(vector, Vector2.right);
        if (vector.y < 0) {
            angle = -angle;
        }
        return Utilities.GetDirectionFromAngle(angle);

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