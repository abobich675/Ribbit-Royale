## How to Build and Test
-Clone the repository: git clone https://github.com/abobich675/Ribbit-Royale.git.
-Open the project in Unity (ensure the correct Unity version is installed).
-Build the project by selecting File > Build Settings and then choosing your target platform.
-Click Build and Run to compile and launch the game.
-Use Unity Test Runner to run tests

## How to Add Tests
- In the Tests folder, there are two folders inside it: EditMode and PlayMode. These contain different types of tests depending on how you want your test to run
- PlayMode is for tests requiring to run the game
- EditMode just does it in the Unity Editor
- Depending on what type of test it is, place it in the correct directory
- Use Unity's Test Framework for writing tests (NUNIT)
- Test files should follow the naming convention: Test_<Feature>.cs.
