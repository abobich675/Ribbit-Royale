## Ribbit Royale Developer Documentation
#### Authors:
Adam Bobich, Aidan Caughey, Baron Baker, Chase Bennett, Luke Garci, Ryan Dobkin
#### Source Code:
  - To view source code for the project, please visit our github: https://github.com/abobich675/Ribbit-Royale.
#### How to Build Game:
  Unity Version: 6000.0.34f1
  1. Open Unity and load the Ribbit Royale project.
  2. In Unity, navigate to File > Build Settings.
  3. Select your target platform (Windows, macOS, etc.).
  4. Click Build and select an output folder.
  5. Once the build process completes, the executable file will be available in the output folder.
#### How to Test Game:
  1. Open Unity and run the game in Play Mode.
  2. Perform manual testing by playing through minigames and verifying expected behaviors.
#### How to Add Tests:
  - Place test scripts in the Tests/ directory.
  - Use Unity's Test Framework for writing unit tests (NUnit).
  - Test files should follow this naming convention: Test_<Feature>.cs.
#### How to Build for Release:
  1. Ensure all known bugs are documented and addressed.
  2. Update the version number in the game settings and documentation.
  3. Follow the build instructions above to create a final release build.
  4. Run sanity checks to verify game functionality.
  5. Upload the build.
