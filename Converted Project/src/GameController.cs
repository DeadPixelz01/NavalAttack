using System;
using System.Collections.Generic;
using SwinGameSDK;

// The GameController is responsible for controlling the game,
// managing user input, and displaying the current state of the game.
namespace MyGame
{
    public static class GameController
    {
        private static BattleShipsGame _theGame;
        private static AIPlayer _ai;

        private static readonly Stack<GameState> STATE = new Stack<GameState>();

        private static AIOption _aiSetting;

        // returns the current state of the game, indicating which screen is
        // currently being used
        public static GameState CurrentState => STATE.Peek();

        // returns the human player.
        public static Player HumanPlayer { get; private set; }

        // returns the computer player.
        public static Player ComputerPlayer => _ai;

        static GameController()
        {
            // bottom state will be quitting. If player exits main menu then the game is over
            STATE.Push(GameState.Quitting);

            // at the start the player is viewing the main menu
            STATE.Push(GameState.ViewingMainMenu);
        }

        // starts a new game.
        // creates an AI player based upon the _aiSetting.
        public static void StartGame()
        {
            if (_theGame != null)
            {
                EndGame();
            }

            // create the game
            _theGame = new BattleShipsGame();

            // create the players
            switch (_aiSetting)
            {
                case AIOption.Medium:
                    {
                        _ai = new AIMediumPlayer(_theGame);
                        break;
                    }
                case AIOption.Hard:
                    {
                        _ai = new AIHardPlayer(_theGame);
                        break;
                    }
                case AIOption.Easy:
                    break;
                default:
                    {
                        _ai = new AIHardPlayer(_theGame);
                        break;
                    }
            }

            HumanPlayer = new Player(_theGame);

            // AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
            _ai.PlayerGrid.Changed += GridChanged;
            _theGame.AttackCompleted += AttackCompleted;

            AddNewState(GameState.Deploying);
        }

        // stops listening to the old game once a new game is started
        private static void EndGame()
        {
            // RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
            _ai.PlayerGrid.Changed -= GridChanged;
            _theGame.AttackCompleted -= AttackCompleted;
        }

        // listens to the game grids for any changes and redraws the screen when the grids change
        private static void GridChanged(object sender, EventArgs args)
        {
            DrawScreen();
            SwinGame.RefreshScreen();
        }

        private static void PlayHitSequence(int row, int column, bool showAnimation)
        {
            if (showAnimation)
            {
                UtilityFunctions.AddExplosion(row, column);
            }

            Audio.PlaySoundEffect(GameResources.GameSound("Hit"));
            UtilityFunctions.DrawAnimationSequence();
        }

        private static void PlayMissSequence(int row, int column, bool showAnimation)
        {
            if (showAnimation)
            {
                UtilityFunctions.AddSplash(row, column);
            }

            Audio.PlaySoundEffect(GameResources.GameSound("Miss"));
            UtilityFunctions.DrawAnimationSequence();
        }

        // listens for attacks to be completed.
        // displays a message, plays sound and redraws the screen.
        private static void AttackCompleted(object sender, AttackResult result)
        {
            var isHuman = _theGame.Player == HumanPlayer;
            if (isHuman)
            {
                UtilityFunctions.Message = "You " + result.ToString();
            }
            else
            {
                UtilityFunctions.Message = "The AI " + result.ToString();
            }

            switch (result.Value)
            {
                case ResultOfAttack.Destroyed:
                    {
                        PlayHitSequence(result.Row, result.Column, isHuman);
                        Audio.PlaySoundEffect(GameResources.GameSound("Sink"));
                        break;
                    }

                case ResultOfAttack.GameOver:
                    {
                        PlayHitSequence(result.Row, result.Column, isHuman);
                        Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

                        while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink")))
                        {
                            SwinGame.Delay(10);
                            SwinGame.RefreshScreen();
                        }

                        Audio.PlaySoundEffect(HumanPlayer.IsDestroyed
                            ? GameResources.GameSound("Lose")
                            : GameResources.GameSound("Winner"));
                        break;
                    }

                case ResultOfAttack.Hit:
                    {
                        PlayHitSequence(result.Row, result.Column, isHuman);
                        break;
                    }

                case ResultOfAttack.Miss:
                    {
                        PlayMissSequence(result.Row, result.Column, isHuman);
                        break;
                    }

                case ResultOfAttack.ShotAlready:
                    {
                        Audio.PlaySoundEffect(GameResources.GameSound("Error"));
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // completes the deployment phase of the game and switches to the battle mode (discovering state)
        // this adds the players to the game before switching state.
        public static void EndDeployment()
        {
            // deploy the players
            _theGame.AddDeployedPlayer(HumanPlayer);
            _theGame.AddDeployedPlayer(_ai);
            SwitchState(GameState.Discovering);
        }

        // gets the player to attack the indicated row and column.
        // checks the attack result once the attack is complete
        public static void Attack(int row, int col)
        {
            var result = _theGame.Shoot(row, col);
            CheckAttackResult(result);
        }

        // gets the AI to attack.
        // checks the attack result once the attack is complete.
        private static void AiAttack()
        {
            var result = _theGame.Player.Attack();
            CheckAttackResult(result);
        }

        // Checks the results of the attack and switches to ending the game if the result was game over.
        // gets the AI to attack if the result switched to the AI player.
        private static void CheckAttackResult(AttackResult result)
        {
            switch (result.Value)
            {
                case ResultOfAttack.Miss:
                    {
                        if (_theGame.Player == ComputerPlayer)
                            AiAttack();
                        break;
                    }

                case ResultOfAttack.GameOver:
                    {
                        SwitchState(GameState.EndingGame);
                        break;
                    }
            }
        }

        // Handles the user SwinGame.
        // Reads key and mouse input and converts these into actions for the game to perform.
        // The actions performed depend upon the state of the game.
        public static void HandleUserInput()
        {
            // Read incoming input events
            SwinGame.ProcessEvents();

            switch (CurrentState)
            {
                case GameState.ViewingMainMenu:
                    {
                        MenuController.HandleMainMenuInput();
                        break;
                    }
                case GameState.ViewingGameMenu:
                    {
                        MenuController.HandleGameMenuInput();
                        break;
                    }
                case GameState.AlteringSettings:
                    {
                        MenuController.HandleSetupMenuInput();
                        break;
                    }
                case GameState.Deploying:
                    {
                        DeploymentController.HandleDeploymentInput();
                        break;
                    }
                case GameState.Discovering:
                    {
                        DiscoveryController.HandleDiscoveryInput();
                        break;
                    }
                case GameState.EndingGame:
                    {
                        EndingGameController.HandleEndOfGameInput();
                        break;
                    }
                case GameState.ViewingHighScores:
                    {
                        HighScoreController.HandleHighScoreInput();
                        break;
                    }
            }

            UtilityFunctions.UpdateAnimations();
        }

        // Draws the current state of the game to the screen.
        // what is drawn depends upon the state of the game.
        public static void DrawScreen()
        {
            UtilityFunctions.DrawBackground();

            switch (CurrentState)
            {
                case GameState.ViewingMainMenu:
                    MenuController.DrawMainMenu();
                    break;
                case GameState.ViewingGameMenu:
                    MenuController.DrawGameMenu();
                    break;
                case GameState.AlteringSettings:
                    MenuController.DrawSettings();
                    break;
                case GameState.Deploying:
                    DeploymentController.DrawDeployment();
                    break;
                case GameState.Discovering:
                    DiscoveryController.DrawDiscovery();
                    break;
                case GameState.EndingGame:
                    EndingGameController.DrawEndOfGame();
                    break;
                case GameState.ViewingHighScores:
                    HighScoreController.DrawHighScores();
                    break;
            }

            UtilityFunctions.DrawAnimations();
            SwinGame.RefreshScreen();
        }

        // move the game to a new state. The current state is maintained so that it can be returned to.
        public static void AddNewState(GameState state)
        {
            STATE.Push(state);
            UtilityFunctions.Message = "";
        }

        // end the current state and add in the new state.
        private static void SwitchState(GameState newState)
        {
            EndCurrentState();
            AddNewState(newState);
        }

        // ends the current state, returning to the prior state
        public static void EndCurrentState()
        {
            STATE.Pop();
        }

        // sets the difficulty for the next level of the game.
        public static void SetDifficulty(AIOption setting)
        {
            _aiSetting = setting;
        }
    }
}
