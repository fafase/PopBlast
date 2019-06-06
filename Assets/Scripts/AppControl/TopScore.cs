using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.AppControl
{
    /// <summary>
    /// Takes care of high score maintenance
    /// </summary>
    public static class TopScore
    {
        #region MEMBERS

        private const string High = "High";

        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Get registered hiscore, 0 if none
        /// </summary>
        /// <returns></returns>
        public static int GetHiScore()
        {
            return PlayerPrefs.GetInt(High, 0);
        }

        /// <summary>
        /// Set the new hiscore
        /// </summary>
        /// <param name="score"></param>
        /// <returns>True if new score else false</returns>
        public static bool SetHiScore(int score)
        {
            int hi = GetHiScore();
            if(score > hi)
            {
                PlayerPrefs.SetInt(High, score);
                return true;
            }
            return false;
        }

        #endregion
    }
}
