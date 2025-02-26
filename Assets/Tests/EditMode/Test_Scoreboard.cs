using NUnit.Framework;
using UI.Scoreboard;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Tests.EditMode
{
    internal class TestScoreboard
    {
        private string PLAYER_NAME = "player0";
        private int INC_AMOUNT = 10;
        private GameObject prefabScoreEntry;
        GameObject ScoreMGR;
        private GameObject prefabInGameScoreEntry;
        private GameObject prefabInGameTimer;
        private Transform contentScoreboard;
        private Transform contentInGameScoreboard;
    /*
        [Test]
        public void ScoreboardTestScore()
        {
            // Assign
            //public ScoreManager scoreManager;
            //var mn = new ScoreManager(4);
            ScoreManager mn = ScoreMGR.AddComponent<ScoreManager>();
            //ScoreManager mn = new ScoreManager(4);
            
            
            // Act
            var preScore = mn.GetPlayerScore(PLAYER_NAME);
            mn.UpdatePlayerScore(PLAYER_NAME, INC_AMOUNT, true);
            var postScore = mn.GetPlayerScore(PLAYER_NAME);

            
            // Assert
            Assert.AreEqual(preScore + INC_AMOUNT, postScore);
        }*/
    [Test]
    public void ScoreboardExTest()
    {
        // Assign
        ScoreEntry se = new ScoreEntry();
        se.SetIsTest(true);
        //se.Initialize();
        
        // Act
        var preScore = se.GetScore();
        se.SetScore(INC_AMOUNT);
        var postScore = se.GetScore();
        
        // Assert
        TestContext.WriteLine("SCORE TEST OUT" + preScore + ":" + postScore);
        Assert.AreEqual(preScore + INC_AMOUNT, postScore);
    }
    }
}