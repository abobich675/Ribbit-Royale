using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#if UNITY_EDITOR
namespace Tests.PlayMode
{
    /*
    public class ScoreboardManagerTests
    {
        private float MAX_LOAD_TIME = 2f;
        private bool logReceived = false;
        private float sceneLoadTime;
        private float logReceivedTime;
        private float timeTaken;

        [SetUp]
        public void Setup()
        {
            Application.logMessageReceived += CaptureLog;
        }

        [TearDown]
        public void Teardown()
        {
            Application.logMessageReceived -= CaptureLog;
        }

        private void CaptureLog(string condition, string stackTrace, LogType type)
        {
            if (condition.Contains("ScoreManager example instance initialization complete."))
            {
                logReceivedTime = Time.realtimeSinceStartup;
                logReceived = true;
            }
        }

        [UnityTest]
        public IEnumerator TestDebugLogTime()
        {
            sceneLoadTime = Time.realtimeSinceStartup;
            SceneManager.LoadScene("ScoreboardScene");

            // Wait for the scene to finish loading
            yield return new WaitForSeconds(1f);

            sceneLoadTime = Time.realtimeSinceStartup;
            
            float timer = 0f;
            while (!logReceived && timer < MAX_LOAD_TIME)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Assert.IsTrue(logReceived, "Expected debug log was not received.");
            
            timeTaken = logReceivedTime - sceneLoadTime;
            Debug.Log($"Time taken from scene load to debug log: {timeTaken} seconds");

            yield return null;
        }
    }*/
}
#endif
