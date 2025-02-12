using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class CommonUtility
    {
        private const float EPSILON = 0.0000005f;
        public static Vector2 GetWorldPosition2D(Vector2 origin ,int i, int j, 
                                               int directionX, int directionY, 
                                               Vector2 cellSize, float gapX, float gapY)
        {
            directionX = Math.Clamp(directionX, -1,1);
            directionY = Math.Clamp(directionY, -1, 1);
            Vector2 result = origin + new Vector2(directionX * j * (cellSize.x + gapX),  
                                                  directionY * i * (cellSize.y + gapY));
            return result;
        }

        public static bool AreColorsEqual(Color color1, Color color2)
        {
            return Approximately(color1.r, color2.r) &&
                   Approximately(color1.g, color2.g) &&
                   Approximately(color1.b, color2.b) &&
                   Approximately(color1.a, color2.a);
        }

        public static bool Approximately(Vector3Int first, Vector3Int second)
        {
            return Approximately(first.x, second.x) &&
                   Approximately(first.y, second.y) &&
                   Approximately(first.z, second.z);
        }
        public static bool Approximately(float first, float second)
        {
            float difference = Mathf.Abs(second - first);
            //Debug.Log("Difference: " + difference);
            return difference <= EPSILON;
        }


        public static bool IsStringLengthInRange(string stringToCheck, int maxLength)
        {
            return stringToCheck.Length <= maxLength;
        }
        
        public static bool IsStringValid(string stringToCheck, string acceptableCharacters, int maxLength)
        {
            if(stringToCheck.Length == 0 || stringToCheck.Length > maxLength)
            {
                return false;
            }

            for(int i = 0; i < stringToCheck.Length; i++)
            {
                char current = stringToCheck[i];
                if(acceptableCharacters.IndexOf(current) == -1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
