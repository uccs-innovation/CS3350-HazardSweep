using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Hazard_Sweep.Classes;
using System.Threading;

namespace Hazard_Sweep
{
    public enum Facing { Left, Right };
    public enum WeaponType { Melee, Pistol, AssaultRifle, Shotgun };

    public enum DropType { Health, PistolAmmo, AssaultAmmo, ShotgunAmmo };

    public enum Objective { Scientist, Cure, Bomb, Helicopter1, Helicopter2, Elimination, Scientist2 };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //Texture2D menuButtonTest;
        public static Random Random;

        PlayerSprite player;
        GameElements elements;
        Camera camera;
        Room street0, street1, street2, street3, street4, street5, street6, street7, street8,
            room0, room1, room2, room3, room4, room5, room6, room7, room8;
        List<int> gridNumbers;

        //Splash screen
        public enum GameState { START, PLAY, PAUSE, MENU, TUT, WIN, LOSE };
        SplashScreen splashScreen;
        MenuScreen menuScreen;
        PauseScreen pauseScreen;
        EndScreen endScreen;
        TutScreen tutScreen;
        public GameState currentGameState = GameState.START;

        //pause press
        bool pressed = false;
        bool released = false;

        //sound effects
        Song music;
        SoundEffect reload, pistolFire, machineFire, shotgunFire, shells, stab, damagedPlayer, damagedPlayer2, dryFire, zombieDamage;
        SoundEffect zombie1, zombie2, zombie3, zombie4, zombie5, zombie6, zombie7;

        //mouse state
        protected MouseState ms;

        //fields for objectives
        public Objective gameObj;
        public int objEliminate;
        SpriteFont objFont;
        public int objTimer;
        public bool objShow;
        public int objRoom;
        public int objRoom2;
        public int heliRoom;
        int objDetermine;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;

            Random = new Random();


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Adds screen size
            GlobalClass.ScreenWidth = graphics.PreferredBackBufferWidth;
            GlobalClass.ScreenHeight = graphics.PreferredBackBufferHeight;

            StartGame();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Creates a service for the spritebatch so it can be used in other classes
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            LoadAll();

            // start music and loop
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ms = Mouse.GetState();

            //if the mouse is inside the game window, update the game
            if (GraphicsDevice.Viewport.Bounds.Contains(ms.X, ms.Y))
            {
                splashScreen.Update(gameTime);
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                KeyboardState keyboardState = Keyboard.GetState();

                if (currentGameState == GameState.PLAY || currentGameState == GameState.PAUSE)
                {

                    if (keyboardState.IsKeyDown(Keys.Escape))
                    {
                        pressed = true;
                    }
                    if (keyboardState.IsKeyUp(Keys.Escape) && pressed == true)
                    {
                        released = true;
                    }
                }

                if (currentGameState == GameState.PLAY)
                {
                    if (pressed == true && released == true)
                    {
                        currentGameState = GameState.PAUSE;
                        pressed = false;
                        released = false;
                    }
                    // update game elements
                    elements.Update(gameTime);

                    //update camera
                    camera.Update(gameTime, player);

                    // show objective
                    if (objTimer < 300)
                        objTimer++;
                    if (keyboardState.IsKeyDown(Keys.Tab))
                        objShow = true;
                    else
                    {
                        if (objTimer >= 300)
                            objShow = false;
                    }
                }
                else if (currentGameState == GameState.PAUSE)
                {
                    if (pressed == true && released == true)
                    {
                        currentGameState = GameState.PLAY;
                        pressed = false;
                        released = false;
                    }
                }

                // restart
                if (keyboardState.IsKeyDown(Keys.D0))
                {
                    StartGame();
                    LoadAll();
                    ChangeGameState(GameState.START);
                }
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (currentGameState == GameState.PLAY)
            {
                IsMouseVisible = false;
                // draw objects
                //spriteBatch.Begin();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null,
                    null, null, camera.transform);
                //testExMap.Draw(spriteBatch);
                base.Draw(gameTime);

                spriteBatch.End();

            }

            if (currentGameState == GameState.START)
            {
                //spriteBatch.Begin();
                splashScreen.Draw(gameTime);
                //spriteBatch.End();
            }

            if (currentGameState == GameState.MENU)
            {
                menuScreen.Draw(gameTime);
                IsMouseVisible = true;
            }

            if (currentGameState == GameState.PAUSE)
            {
                pauseScreen.Draw(gameTime);
            }

            if (currentGameState == GameState.WIN || currentGameState == GameState.LOSE)
            {
                endScreen.Draw(gameTime);
            }

            if (currentGameState == GameState.TUT)
            {
                tutScreen.Draw(gameTime);
            }

            //draw hud
            if (currentGameState == GameState.PLAY)
            {
                elements.Draw(spriteBatch);

                spriteBatch.Begin();
                if (objShow)
                {
                    if (gameObj == Objective.Scientist)
                        spriteBatch.DrawString(objFont, "objective: find the scientist",
                            new Vector2(32, 32), Color.Black);
                    if (gameObj == Objective.Scientist2)
                        spriteBatch.DrawString(objFont, "objective: get back to the scientist",
                            new Vector2(32, 32), Color.Black);
                    if (gameObj == Objective.Cure)
                        spriteBatch.DrawString(objFont, "objective: find the cure",
                            new Vector2(32, 32), Color.Black);
                    if (gameObj == Objective.Bomb)
                        spriteBatch.DrawString(objFont, "objective: find and arm the bomb",
                            new Vector2(32, 32), Color.Black);
                    if (gameObj == Objective.Helicopter1 || gameObj == Objective.Helicopter2)
                        spriteBatch.DrawString(objFont, "objective: get to the helicopter",
                            new Vector2(32, 32), Color.Black);
                    if (gameObj == Objective.Elimination)
                        spriteBatch.DrawString(objFont, "objective: eliminate " + objEliminate + " hostiles",
                            new Vector2(32, 32), Color.Black);
                }
                spriteBatch.End();
            }

        }

        //change the gamestate
        public void ChangeGameState(GameState state)
        {
            currentGameState = state;

            splashScreen.Enabled = false;
            splashScreen.Visible = false;
            menuScreen.Enabled = false;
            menuScreen.Visible = false;
            pauseScreen.Enabled = false;
            pauseScreen.Visible = false;
            endScreen.Visible = false;
            endScreen.Enabled = false;
            tutScreen.Enabled = false;
            tutScreen.Visible = false;

            //change what displays based on game state
            switch (currentGameState)
            {
                case GameState.START:
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    break;
                case GameState.PLAY:
                    break;
                case GameState.PAUSE:
                    pauseScreen.Enabled = true;
                    pauseScreen.Visible = true;
                    break;
                case GameState.MENU:
                    menuScreen.Enabled = true;
                    menuScreen.Visible = true;
                    break;
                case GameState.TUT:
                    tutScreen.Enabled = true;
                    tutScreen.Visible = true;
                    break;
                case GameState.LOSE:
                    endScreen.setData("GAME OVER");
                    endScreen.Visible = true;
                    endScreen.Enabled = true;
                    break;
                case GameState.WIN:
                    endScreen.setData("HAZARD SWEPT");
                    endScreen.Visible = true;
                    endScreen.Enabled = true;
                    break;
            }
        }

        //returns current game state
        public GameState GetGameState()
        {
            return currentGameState;
        }

        //method to allow other classes to use the mouse
        public MouseState GetMouseState()
        {
            return ms;
        }
        // I'm sure there's a better way to do this, but it's late and I have no idea what I'm doing
        public Room GetRoom(int id)
        {
            switch (id)
            {
                case 0:
                    return street0;
                    break;
                case 1:
                    return street1;
                    break;
                case 2:
                    return street2;
                    break;
                case 3:
                    return street3;
                    break;
                case 4:
                    return street4;
                    break;
                case 5:
                    return street5;
                    break;
                case 6:
                    return street6;
                    break;
                case 7:
                    return street7;
                    break;
                case 8:
                    return street8;
                    break;
                case 9:
                    return room0;
                    break;
                case 10:
                    return room1;
                    break;
                case 11:
                    return room2;
                    break;
                case 12:
                    return room3;
                    break;
                case 13:
                    return room4;
                    break;
                case 14:
                    return room5;
                    break;
                case 15:
                    return room6;
                    break;
                case 16:
                    return room7;
                    break;
                case 17:
                    return room8;
                    break;
                default:
                    return street0;
                    break;
            }
        }

        public int GetRoomID()
        {
            for (int i = Components.Count() - 1; i > -1; i--)
            {
                IGameComponent g = Components[i];
                if (g is Room)
                {
                    Room r = (Room)g;
                    return r.GetID();
                }
            }
            return -1;
        }

        //change what room you are in
        public void ChangeLevel(int oldLevel, int newLevel)
        {
            for (int i = Components.Count() - 1; i > -1; i--)
            {
                IGameComponent g = Components[i];
                if (g is Enemy)
                {
                    Enemy e = (Enemy)g;
                    Components.Remove(e);
                }
                if (g is Bullet)
                {
                    Bullet bu = (Bullet)g;
                    Components.Remove(bu);
                }
                if (g is Barricade)
                {
                    Barricade b = (Barricade)g;
                    Components.Remove(b);
                }
                if (g is Door)
                {
                    Door d = (Door)g;
                    Components.Remove(d);
                }
                if (g is NPC)
                {
                    NPC n = (NPC)g;
                    Components.Remove(n);
                }
                if (g is itemDrop)
                {
                    itemDrop item = (itemDrop)g;
                    Components.Remove(item);
                }
            }
            Components.Remove(GetRoom(oldLevel));
            Components.Add(GetRoom(newLevel));
        }

        //camera accessor
        public Camera GetCamera()
        {
            return camera;
        }

        // return room id with objective
        public int GetObjRoom()
        {
            return objRoom;
        }

        // return game objective
        public Objective GetObjective()
        {
            return gameObj;
        }

        //clears everything and starts a new game (reinitialize)
        public void StartGame()
        {
            Components.Clear();

            // game objective
            objDetermine = Random.Next(3);
            switch (objDetermine)
            {
                case 0:
                    gameObj = Objective.Scientist;
                    break;
                case 1:
                    gameObj = Objective.Bomb;
                    break;
                default:
                    gameObj = Objective.Elimination;
                    break;
            }
            objTimer = 0;
            objShow = true;
            objRoom = Random.Next(10, 18);
            objRoom2 = Random.Next(18);
            while (objRoom2 == objRoom)
                objRoom2 = Random.Next(18);
            objEliminate = 40;
            heliRoom = Random.Next(0, 9);

            player = new PlayerSprite(this, "Images//playerWalk", new Vector2(GlobalClass.ScreenWidth / 2,
                GlobalClass.ScreenHeight / 2), 2, 6, this);

            gridNumbers = new List<int>(Enumerable.Range(0, 9));
            Shuffle(gridNumbers);

            street0 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.White, 0);
            street1 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightBlue, 1);
            street2 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightCoral, 2);
            street3 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightGoldenrodYellow, 3);
            street4 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightGreen, 4);
            street5 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightGray, 5);
            street6 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightPink, 6);
            street7 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightSteelBlue, 7);
            street8 = new Room(this, "Images//Maps//External//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightSeaGreen, 8);

            room0 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.White, 9);
            room1 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightBlue, 10);
            room2 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightCoral, 11);
            room3 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightGoldenrodYellow, 12);
            room4 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightGreen, 13);
            room5 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightGray, 14);
            room6 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightPink, 15);
            room7 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightSteelBlue, 16);
            room8 = new Room(this, "Images//Maps//Internal//test01", new Vector2(100, 100), 1, 1, false, false, player, Color.LightSeaGreen, 17);

            //add rooms to game
            Components.Add(street0);
            Components.Add(player);

            //Add game components
            Components.Add(new Enemy(this, "Images//enemyWalk", new Vector2(200, 200), 2, 5));
            elements = new GameElements(this, player);
            elements.Initialize();
            camera = new Camera(this);

            //Splashscreen component
            splashScreen = new SplashScreen(this);
            menuScreen = new MenuScreen(this);
            pauseScreen = new PauseScreen(this);
            endScreen = new EndScreen(this);
            tutScreen = new TutScreen(this);
            Components.Add(splashScreen);
            Components.Add(menuScreen);
            Components.Add(pauseScreen);
            Components.Add(endScreen);
            Components.Add(tutScreen);

        }

        public void LoadAll()
        {
            //load sound effects
            music = Content.Load<Song>("Sounds/Unseen Horrors");
            reload = Content.Load<SoundEffect>("Sounds/reload");
            pistolFire = Content.Load<SoundEffect>("Sounds/pistolLicenseAttribution3");
            machineFire = Content.Load<SoundEffect>("Sounds/machine gun");
            shotgunFire = Content.Load<SoundEffect>("Sounds/Shotgunsound");
            shells = Content.Load<SoundEffect>("Sounds/shell");
            stab = Content.Load<SoundEffect>("Sounds/Stab");
            damagedPlayer = Content.Load<SoundEffect>("Sounds/damagedplayer");
            damagedPlayer2 = Content.Load<SoundEffect>("Sounds/damagedplayer2");
            dryFire = Content.Load<SoundEffect>("Sounds/Dry Fire Gun-SoundBible.com-2053652037");
            zombieDamage = Content.Load<SoundEffect>("Sounds/zombiedamage");
            zombie1 = Content.Load<SoundEffect>("Sounds/Zombie 1");
            zombie2 = Content.Load<SoundEffect>("Sounds/Zombie 2");
            zombie3 = Content.Load<SoundEffect>("Sounds/Zombie 3");
            zombie4 = Content.Load<SoundEffect>("Sounds/Zombie 4");
            zombie5 = Content.Load<SoundEffect>("Sounds/Zombie 5");
            zombie6 = Content.Load<SoundEffect>("Sounds/Zombie 6");
            zombie7 = Content.Load<SoundEffect>("Sounds/Zombie 7");


            // load game elements
            elements.LoadContent();
            objFont = Content.Load<SpriteFont>(@"Fonts\VtksMoney_30");
        }

        #region Shuffling
        // found at http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp

        public static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;

            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }

        public static void Shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #endregion

        #region sound effect methods
        //method to play reloading sound effect
        public void playReload()
        {
            SoundEffectInstance reloadInst = reload.CreateInstance();
            reloadInst.Volume = .9f;
            reloadInst.Play();
        }

        //method to play pistol shot and shell drop
        public void playPistol()
        {
            SoundEffectInstance pistolInst = pistolFire.CreateInstance();
            pistolInst.Volume = .2f;
            pistolInst.Play();
            SoundEffectInstance shellInst = shells.CreateInstance();
            shellInst.Volume = .9f;
            shellInst.Play();
        }

        //method to play machinegun shot and shell drop
        public void playMachinegun()
        {
            SoundEffectInstance machineInst = machineFire.CreateInstance();
            machineInst.Volume = .2f;
            machineInst.Play();
            SoundEffectInstance shellInst = shells.CreateInstance();
            shellInst.Volume = .9f;
            shellInst.Play();
        }

        //method to play shotgun shot and shell drop
        public void playShotgun()
        {
            SoundEffectInstance shotgunInst = shotgunFire.CreateInstance();
            shotgunInst.Volume = .2f;
            shotgunInst.Play();
            SoundEffectInstance shellInst = shells.CreateInstance();
            shellInst.Volume = .9f;
            shellInst.Play();
        }

        //method that plays the stabbing sound
        public void playStab()
        {
            SoundEffectInstance stabInst = stab.CreateInstance();
            stabInst.Volume = .9f;
            stabInst.Play();
        }

        //method to play one of two damaged player sound
        public void playPlayerDamaged()
        {
            int noise = Random.Next(1, 3);
            switch (noise)
            {
                case 1:
                    SoundEffectInstance temp1 = damagedPlayer.CreateInstance();
                    temp1.Volume = .7f;
                    temp1.Play();
                    break;
                case 2:
                    SoundEffectInstance temp2 = damagedPlayer2.CreateInstance();
                    temp2.Volume = .7f;
                    temp2.Play();
                    break;
                default:
                    SoundEffectInstance temp3 = damagedPlayer2.CreateInstance();
                    temp3.Volume = .7f;
                    temp3.Play();
                    break;
            }
        }

        //method to play dry fire sound effect
        public void playDryFire()
        {
            SoundEffectInstance dryFireInst = dryFire.CreateInstance();
            dryFireInst.Volume = .5f;
            dryFireInst.Play();
        }

        //method to play zombie damage sound effect
        public void playZombieDamage()
        {
            SoundEffectInstance zombieDamageInst = zombieDamage.CreateInstance();
            zombieDamageInst.Volume = .6f;
            zombieDamageInst.Play();
        }

        //method to play one of 7 zombie death sound effects
        public void playZombieDeath()
        {
            int noise = Random.Next(1, 8);
            switch (noise)
            {
                case 1:
                    SoundEffectInstance temp1 = zombie1.CreateInstance();
                    temp1.Volume = .3f;
                    temp1.Play();
                    break;
                case 2:
                    SoundEffectInstance temp2 = zombie2.CreateInstance();
                    temp2.Volume = .3f;
                    temp2.Play();
                    break;
                case 3:
                    SoundEffectInstance temp3 = zombie3.CreateInstance();
                    temp3.Volume = .3f;
                    temp3.Play();
                    break;
                case 4:
                    SoundEffectInstance temp4 = zombie4.CreateInstance();
                    temp4.Volume = .3f;
                    temp4.Play();
                    break;
                case 5:
                    SoundEffectInstance temp5 = zombie5.CreateInstance();
                    temp5.Volume = .3f;
                    temp5.Play();
                    break;
                case 6:
                    SoundEffectInstance temp6 = zombie6.CreateInstance();
                    temp6.Volume = .3f;
                    temp6.Play();
                    break;
                case 7:
                    SoundEffectInstance temp7 = zombie7.CreateInstance();
                    temp7.Volume = .3f;
                    temp7.Play();
                    break;
                default:
                    SoundEffectInstance tempdef = zombie1.CreateInstance();
                    tempdef.Volume = .5f;
                    tempdef.Play();
                    break;
            }
        }
        #endregion
    }
}
//sound effect credits
//Pistol firing sound effect Recorded by GoodSoundForYou
//Shotgun sound effect Recorded by Soundeffects
//machinegunfire Recorded by WEL
//shell sound effect Recorded by Marcel
//reload effect by Mike Koenig
//game music public domain
//zombie noises made by group members