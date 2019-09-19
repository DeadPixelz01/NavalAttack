using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SwinGameSDK;

namespace MyGame
{
    public static class GameResources
    {
        private static void LoadFonts()
        {
            NewFont("ArialLarge", "arial.ttf", 80);
            NewFont("Courier", "cour.ttf", 14);
            NewFont("CourierSmall", "cour.ttf", 8);
            NewFont("Menu", "ffaccess.ttf", 8);
        }

        private static void LoadImages()
        {
            // Backgrounds
            NewImage("Menu", "main_page.jpg");
            NewImage("Discovery", "discover.jpg");
            NewImage("Deploy", "deploy.jpg");

            // Deployment
            NewImage("LeftRightButton", "deploy_dir_button_horiz.png");
            NewImage("UpDownButton", "deploy_dir_button_vert.png");
            NewImage("SelectedShip", "deploy_button_hl.png");
            NewImage("PlayButton", "deploy_play_button.png");
            NewImage("RandomButton", "deploy_randomize_button.png");

            // Ships
            int i;
            for (i = 1; i <= 5; i++)
            {
                NewImage("ShipLR" + i, "ship_deploy_horiz_" + i + ".png");
                NewImage("ShipUD" + i, "ship_deploy_vert_" + i + ".png");
            }

            // Explosions
            NewImage("Explosion", "explosion.png");
            NewImage("Splash", "splash.png");
        }

        private static void LoadSounds()
        {
            NewSound("Error", "error.wav");
            NewSound("Hit", "hit.wav");
            NewSound("Sink", "sink.wav");
            NewSound("Siren", "siren.wav");
            NewSound("Miss", "watershot.wav");
            NewSound("Winner", "winner.wav");
            NewSound("Lose", "lose.wav");
        }

        private static void LoadMusic()
        {
            NewMusic("Background", "horrordrone.mp3");
        }


        // gets a Font Loaded in the Resources
        public static Font GameFont(string font)
        {
            return _Fonts[font];
        }

        // gets an Image loaded in the Resources
        public static Bitmap GameImage(string image)
        {
            return _Images[image];
        }

        // gets an sound loaded in the Resources
        public static SoundEffect GameSound(string sound)
        {
            return _Sounds[sound];
        }

        // gets the music loaded in the Resources
        public static Music GameMusic(string music)
        {
            return _Music[music];
        }

        private static Dictionary<string, Bitmap> _Images = new Dictionary<string, Bitmap>();
        private static Dictionary<string, Font> _Fonts = new Dictionary<string, Font>();
        private static Dictionary<string, SoundEffect> _Sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Music> _Music = new Dictionary<string, Music>();

        private static Bitmap _Background;
        private static Bitmap _Animation;
        private static Bitmap _LoaderFull;
        private static Bitmap _LoaderEmpty;
        private static Font _LoadingFont;
        private static SoundEffect _StartSound;

        // the resources class stores all of the Games Media Resources, such as Images, Fonts Sounds, Music.

        public static void LoadResources()
        {
            var width = SwinGame.ScreenWidth();
            var height = SwinGame.ScreenHeight();

            SwinGame.ChangeScreenSize(800, 600);

            ShowLoadingScreen();

            ShowMessage("Loading fonts...", 0);
            LoadFonts();
            SwinGame.Delay(100);

            ShowMessage("Loading images...", 1);
            LoadImages();
            SwinGame.Delay(100);

            ShowMessage("Loading sounds...", 2);
            LoadSounds();
            SwinGame.Delay(100);

            ShowMessage("Loading music...", 3);
            LoadMusic();
            SwinGame.Delay(100);

            SwinGame.Delay(100);
            ShowMessage("Game loaded...", 5);
            SwinGame.Delay(100);
            EndLoadingScreen(width, height);
        }

        private static void ShowLoadingScreen()
        {
            _Background = SwinGame.LoadBitmap(SwinGame.PathToResource("SplashBack.png",
                ResourceKind.BitmapResource));
            SwinGame.DrawBitmap(_Background, 0, 0);
            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();

            _Animation = SwinGame.LoadBitmap(SwinGame.PathToResource("SwinGameAni.jpg",
                ResourceKind.BitmapResource));
            _LoadingFont = SwinGame.LoadFont(SwinGame.PathToResource("arial.ttf", ResourceKind.FontResource),
                12);
            _StartSound = Audio.LoadSoundEffect(SwinGame.PathToResource("SwinGameStart.ogg",
                ResourceKind.SoundResource));

            _LoaderFull = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_full.png",
                ResourceKind.BitmapResource));
            _LoaderEmpty = SwinGame.LoadBitmap(SwinGame.PathToResource("loader_empty.png",
                ResourceKind.BitmapResource));

            PlaySwinGameIntro();
        }

        private static void PlaySwinGameIntro()
        {
            const int aniCellCount = 11;

            Audio.PlaySoundEffect(_StartSound);
            SwinGame.Delay(200);

            int i;
            for (i = 0; i <= aniCellCount - 1; i++)
            {
                SwinGame.DrawBitmap(_Background, 0, 0);
                SwinGame.Delay(20);
                SwinGame.RefreshScreen();
                SwinGame.ProcessEvents();
            }

            SwinGame.Delay(1500);
        }

        private static void ShowMessage(string message, int number)
        {
            const int tx = 310;
            const int ty = 493;
            const int tw = 200;
            const int th = 25;
            const int steps = 5;
            const int bgX = 279;
            const int bgY = 453;

            var toDraw = new Rectangle();

            var fullW = 260 * number / steps;
            SwinGame.DrawBitmap(_LoaderEmpty, bgX, bgY);
            SwinGame.DrawCell(_LoaderFull, 0, bgX, bgY);

            toDraw.X = tx;
            toDraw.Y = ty;
            toDraw.Width = tw;
            toDraw.Height = th;
            SwinGame.DrawText(message, Color.White, Color.Transparent, _LoadingFont,
                FontAlignment.AlignCenter, toDraw);

            SwinGame.RefreshScreen();
            SwinGame.ProcessEvents();
        }

        private static void EndLoadingScreen(int width, int height)
        {
            SwinGame.ProcessEvents();
            SwinGame.Delay(500);
            SwinGame.ClearScreen();
            SwinGame.RefreshScreen();
            SwinGame.FreeFont(_LoadingFont);
            SwinGame.FreeBitmap(_Background);
            SwinGame.FreeBitmap(_Animation);
            SwinGame.FreeBitmap(_LoaderEmpty);
            SwinGame.FreeBitmap(_LoaderFull);
            //Audio.FreeSoundEffect(_StartSound);
            SwinGame.ChangeScreenSize(width, height);
        }

        private static void NewFont(string fontName, string filename, int size)
        {
            _Fonts.Add(fontName, SwinGame.LoadFont(SwinGame.PathToResource(filename, ResourceKind.FontResource), size));
        }

        private static void NewImage(string imageName, string filename)
        {
            _Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(filename,
                ResourceKind.BitmapResource)));
        }

        private static void NewTransparentColorImage(string imageName, string fileName, Color transColor)
        {
            _Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(fileName,
                ResourceKind.BitmapResource)));
        }

        private static void NewTransparentColourImage(string imageName, string fileName, Color transColor)
        {
            NewTransparentColorImage(imageName, fileName, transColor);
        }

        private static void NewSound(string soundName, string filename)
        {
            _Sounds.Add(soundName, Audio.LoadSoundEffect(SwinGame.PathToResource(filename,
                ResourceKind.SoundResource)));
        }

        private static void NewMusic(string musicName, string filename)
        {
            _Music.Add(musicName, Audio.LoadMusic(SwinGame.PathToResource(filename, ResourceKind.SoundResource)));
        }

        private static void FreeFonts()
        {
            foreach (var obj in _Fonts.Values)
            {
                SwinGame.FreeFont(obj);
            }
        }

        private static void FreeImages()
        {
            foreach (var obj in _Images.Values)
            {
                SwinGame.FreeBitmap(obj);
            }
        }

        private static void FreeSounds()
        {
            foreach (var obj in _Sounds.Values)
            {
                Audio.FreeSoundEffect(obj);
            }
        }

        private static void FreeMusic()
        {
            foreach (var obj in _Music.Values)
            {
                Audio.FreeMusic(obj);
            }
        }

        public static void FreeResources()
        {
            FreeFonts();
            FreeImages();
            FreeMusic();
            FreeSounds();
            SwinGame.ProcessEvents();
        }
    }
}
