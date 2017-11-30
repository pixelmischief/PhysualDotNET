using System;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using VelcroPhysics.Factories;
using VelcroPhysics.Dynamics;

// Note: This "using" statement does not denote an actual dependency, but is
// instead a bit of namespace magic that Velcro uses to maintain compatibility
// with older code-bases that reference XNA's vector class.
using Microsoft.Xna.Framework;

namespace Physual
{
    class Program
    {

        //*************
        //* Constants *
        //*************

        //Utility
        public const float HALF = 0.5f;
        public const float DOUBLE = 2.0f;
        public const float INVERT = -1.0f;

        //Velcro
        public const int PHYSICS_POSITION_ITERATIONS = 6;
        public const int PHYSICS_VELOCITY_ITERATIONS = 12;

        public const float WORLD_UNIT = 2.0f;
        public const float WORLD_WIDTH = 80.0f;
        public const float WORLD_HEIGHT = 60.0f;
        public const float WORLD_TIME_RESOLUTION = 1.0f / 60.0f;
        public const float WORLD_TIME_ELAPSED_ZERO = 0.0f;
        public const float WORLD_GRAVITY_ZERO = 0.0f;
        public const float WORLD_SLOW_MOTION_FACTOR = 10.0f;

        //SFML
        public const int SCREEN_WIDTH = 1600;
        public const int SCREEN_HEIGHT = 1200;
        public const string SCREEN_TITLE = "Physual C#";

        public const float PIXELS_PER_METER_X = SCREEN_WIDTH / WORLD_WIDTH;
        public const float PIXELS_PER_METER_Y = SCREEN_HEIGHT / WORLD_HEIGHT;

        public const byte COLOR_TERMINALGREEN_R = 64;
        public const byte COLOR_TERMINALGREEN_G = 255;
        public const byte COLOR_TERMINALGREEN_B = 64;

        public const string SAMPLE_FILE_BIP = "res\\bip.wav";
        public const string SAMPLE_FILE_BEEP = "res\\beep.wav";

        public const string MUSIC_FILE = "res\\Axel Goes Funky.flac";
        public const float MUSIC_VOLUME = 75.0f;

        public const string TEXT_FONT_FILE = "res\\pannetje_10.ttf";
        public const int TEXT_FONT_SIZE = 23;
        public const string TEXT_MESSAGE_TOP = "Physual C#: Demonstration of a 2D Game Development Stack for .NET (C#, VelcroPhysics, and SFML.Net)";
        public const string TEXT_MESSAGE_BOTTOM = "(S)low Debug      (P)ause      (C)ircular Puck      (M)usic Toggle      Arrows Nudge";
        public const int TEXT_MESSAGE_TOP_MARGIN = 4;

        //Game
        public const float BODY_DENSITY_PERFECT_SOLID = 1.0f;
        public const float BODY_RESTITUTION_PERFECT_BOUNCE = 1.0f;
        public const float BODY_ROTATION_ZERO = 0.0f;
        public const float BODY_FRICTION_ZERO = 0.0f;
        public const float BODY_DAMPING_ZERO = 0.0f;
        public const float BODY_MASS_ZERO = 0.0f;

        public const float BALL_RADIUS = WORLD_UNIT * HALF;
        public const float BALL_START_POSITION_X = 3.0f;
        public const float BALL_START_POSITION_Y = -30.0f;
        public const float BALL_START_VELOCITY_X = 80.0f;
        public const float BALL_START_VELOCITY_Y = 0.0f;
        public const float BALL_MIN_VELOCITY_MAGNITUDE_X = 12.0f;
        public const float BALL_NUDGE_AMOUNT = 0.01f;

        public const float WALL_THICKNESS = WORLD_UNIT;



        static void Main(string[] args)
        {
            //*****************
            //* Configuration *
            //*****************


            //Velcro
            VelcroPhysics.Settings.ContinuousPhysics = true;
            VelcroPhysics.Settings.PositionIterations = PHYSICS_POSITION_ITERATIONS;
            VelcroPhysics.Settings.VelocityIterations = PHYSICS_VELOCITY_ITERATIONS;


            //World
            var world = new World(new Vector2(WORLD_GRAVITY_ZERO));


            //Ball Body
            var ball = BodyFactory.CreateCircle(
                world,
                BALL_RADIUS,
                BODY_DENSITY_PERFECT_SOLID,
                new Vector2(
                    BALL_START_POSITION_X,
                    BALL_START_POSITION_Y
                ),
                BodyType.Dynamic
            );
            ball.LinearVelocity = new Vector2(BALL_START_VELOCITY_X, BALL_START_VELOCITY_Y);
            ball.Restitution = BODY_RESTITUTION_PERFECT_BOUNCE;
            ball.Friction = BODY_FRICTION_ZERO;
            ball.LinearDamping = BODY_DAMPING_ZERO;
            ball.Mass = BODY_MASS_ZERO;
            ball.IsBullet = true;


            //Right Wall Body
            var wallRight = BodyFactory.CreateRectangle(
                world,
                WALL_THICKNESS,
                WORLD_HEIGHT,
                BODY_DENSITY_PERFECT_SOLID,
                new Vector2(
                    WORLD_WIDTH - (WORLD_UNIT * HALF),
                    (WORLD_HEIGHT * HALF) * INVERT
                ),
                BODY_ROTATION_ZERO,
                BodyType.Static
            );
            bool readyToNudgeBall = true;


            //Left Wall Body
            var wallLeft = BodyFactory.CreateRectangle(
                world,
                WALL_THICKNESS,
                WORLD_HEIGHT,
                BODY_DENSITY_PERFECT_SOLID,
                new Vector2(
                    WORLD_UNIT * HALF,
                    (WORLD_HEIGHT * HALF) * INVERT
                ),
                BODY_ROTATION_ZERO,
                BodyType.Static
            );


            //Top Wall Body
            var wallTop = BodyFactory.CreateRectangle(
                world,
                WORLD_WIDTH,
                WALL_THICKNESS,
                BODY_DENSITY_PERFECT_SOLID,
                new Vector2(
                    WORLD_WIDTH * HALF,
                    (WORLD_UNIT * HALF) * INVERT
                ),
                BODY_ROTATION_ZERO,
                BodyType.Static
            );


            //Bottom Wall Body
            var wallBottom = BodyFactory.CreateRectangle(
                world,
                WORLD_WIDTH,
                WALL_THICKNESS,
                BODY_DENSITY_PERFECT_SOLID,
                new Vector2(
                    WORLD_WIDTH * HALF,
                    (WORLD_HEIGHT - (WORLD_UNIT * HALF)) * INVERT
                ),
                BODY_ROTATION_ZERO,
                BodyType.Static
            );


            //Rendering Color
            Color terminalGreen = new Color(COLOR_TERMINALGREEN_R, COLOR_TERMINALGREEN_G, COLOR_TERMINALGREEN_B);


            //Ball Sprite
            var ballShapeRadius = ball.FixtureList[0].Shape.Radius;
            CircleShape ballSpriteRound;
            RectangleShape ballSpriteSquare;
            {
                //Round
                ballSpriteRound = new CircleShape(ballShapeRadius * PIXELS_PER_METER_X);
                ballSpriteRound.FillColor = terminalGreen;

                //Square
                ballSpriteSquare = new RectangleShape(
                    new Vector2f(
                        ballShapeRadius * DOUBLE * PIXELS_PER_METER_X,
                        ballShapeRadius * DOUBLE * PIXELS_PER_METER_Y
                    )
                );
                ballSpriteSquare.FillColor = terminalGreen;
            }


            //Right Wall Sprite
            var wallRightShapeWidth = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallRight.FixtureList[0].Shape).Vertices[0].X) * DOUBLE;
            var wallRightShapeHeight = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallRight.FixtureList[0].Shape).Vertices[0].Y) * DOUBLE;
            var wallRightSprite = new RectangleShape(
                new Vector2f(
                    wallRightShapeWidth * PIXELS_PER_METER_X,
                    wallRightShapeHeight * PIXELS_PER_METER_Y
                )
            );
            wallRightSprite.Position = new Vector2f(
                (wallRight.Position.X - (wallRightShapeWidth * HALF)) * PIXELS_PER_METER_X,
                ((wallRight.Position.Y + (wallRightShapeHeight * HALF)) * PIXELS_PER_METER_Y) * INVERT
            );
            wallRightSprite.FillColor = terminalGreen;


            //Left Wall Sprite
            var wallLeftShapeWidth = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallLeft.FixtureList[0].Shape).Vertices[0].X) * DOUBLE;
            var wallLeftShapeHeight = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallLeft.FixtureList[0].Shape).Vertices[0].Y) * DOUBLE;
            var wallLeftSprite = new RectangleShape(
                new Vector2f(
                    wallLeftShapeWidth * PIXELS_PER_METER_X,
                    wallLeftShapeHeight * PIXELS_PER_METER_Y
                )
            );
            wallLeftSprite.Position = new Vector2f(
                (wallLeft.Position.X - (wallLeftShapeWidth * HALF)) * PIXELS_PER_METER_X,
                ((wallLeft.Position.Y + (wallLeftShapeHeight * HALF)) * PIXELS_PER_METER_Y) * INVERT
            );
            wallLeftSprite.FillColor = terminalGreen;


            //Top Wall Sprite
            var wallTopShapeWidth = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallTop.FixtureList[0].Shape).Vertices[0].X) * DOUBLE;
            var wallTopShapeHeight = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallTop.FixtureList[0].Shape).Vertices[0].Y) * DOUBLE;
            var wallTopSprite = new RectangleShape(
                new Vector2f(
                    wallTopShapeWidth * PIXELS_PER_METER_X,
                    wallTopShapeHeight * PIXELS_PER_METER_Y
                )
            );
            wallTopSprite.Position = new Vector2f(
                (wallTop.Position.X - (wallTopShapeWidth * HALF)) * PIXELS_PER_METER_X,
                ((wallTop.Position.Y + (wallTopShapeHeight * HALF)) * PIXELS_PER_METER_Y) * INVERT
            );
            wallTopSprite.FillColor = terminalGreen;


            //Bottom Wall Sprite
            var wallBottomShapeWidth = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallBottom.FixtureList[0].Shape).Vertices[0].X) * DOUBLE;
            var wallBottomShapeHeight = Math.Abs(((VelcroPhysics.Collision.Shapes.PolygonShape)wallBottom.FixtureList[0].Shape).Vertices[0].Y) * DOUBLE;
            var wallBottomSprite = new RectangleShape(
                new Vector2f(
                    wallBottomShapeWidth * PIXELS_PER_METER_X,
                    wallBottomShapeHeight * PIXELS_PER_METER_Y
                )
            );
            wallBottomSprite.Position = new Vector2f(
                (wallBottom.Position.X - (wallBottomShapeWidth * HALF)) * PIXELS_PER_METER_X,
                ((wallBottom.Position.Y + (wallBottomShapeHeight * HALF)) * PIXELS_PER_METER_Y) * INVERT
            );
            wallBottomSprite.FillColor = terminalGreen;


            //Title Text
            Font font = new Font(TEXT_FONT_FILE);

            Text topText = new Text(TEXT_MESSAGE_TOP, font);
            topText.Color = Color.Black;
            topText.CharacterSize = TEXT_FONT_SIZE;
            topText.Position = new Vector2f(WORLD_UNIT * PIXELS_PER_METER_X, TEXT_MESSAGE_TOP_MARGIN);

            Text bottomText = new Text(TEXT_MESSAGE_BOTTOM, font);
            bottomText.Color = Color.Black;
            bottomText.CharacterSize = TEXT_FONT_SIZE;
            bottomText.Position = new Vector2f(WORLD_UNIT * PIXELS_PER_METER_X, ((WORLD_HEIGHT - WORLD_UNIT) * PIXELS_PER_METER_Y) + TEXT_MESSAGE_TOP_MARGIN);



            //Sound Effects
            var sampler = new Sound[2];
            sampler[0] = new Sound(new SoundBuffer(SAMPLE_FILE_BEEP));
            sampler[1] = new Sound(new SoundBuffer(SAMPLE_FILE_BIP));
            int soundEffectCycle = 0;

            bool readyToPlaySoundEffect = true;



            //Music
            Music synthesizer = new Music(MUSIC_FILE);
            synthesizer.Loop = true;
            synthesizer.Volume = MUSIC_VOLUME;
            bool readyToToggleMusic = true;


            //*************
            //* Execution *
            //*************


            //Initialize Display
            var screen = new RenderWindow(new VideoMode(SCREEN_WIDTH, SCREEN_HEIGHT), SCREEN_TITLE);

            
            //Cue Music
            synthesizer.Play();
            

            //Initialize Clock
            var clock = new Clock();
            float deltaTime = WORLD_TIME_ELAPSED_ZERO;
            float timeElapsedLastTick = clock.ElapsedTime.AsMilliseconds();
            float timeElapsedThisTick;


            //Enter Game Loop
            while (!Keyboard.IsKeyPressed(Keyboard.Key.Escape) && screen.IsOpen)
            {
                //Calculate Delta Time
                timeElapsedThisTick = clock.ElapsedTime.AsMilliseconds();
                deltaTime += (timeElapsedThisTick - timeElapsedLastTick) / 1000.0f;
                timeElapsedLastTick = timeElapsedThisTick;


                //Step Physics
                while (deltaTime >= WORLD_TIME_RESOLUTION)
                {
                    //Hold 'P' to Pause Processing and Print Data
                    if (Keyboard.IsKeyPressed(Keyboard.Key.P))
                    {
                        Console.WriteLine("(" + ball.Position.X + "," + ball.Position.Y + ") (" + ball.LinearVelocity.X + "," + ball.LinearVelocity.Y + ")");
                        while (Keyboard.IsKeyPressed(Keyboard.Key.P)) { }
                        clock.Restart();
                        timeElapsedLastTick = clock.ElapsedTime.AsMilliseconds();
                    }

                    //Hold 'S' to Slow Processing and Print Data
                    if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                    {
                        world.Step(WORLD_TIME_RESOLUTION / WORLD_SLOW_MOTION_FACTOR);
                        Console.WriteLine("(" + ball.Position.X + "," + ball.Position.Y + ") (" + ball.LinearVelocity.X + "," + ball.LinearVelocity.Y + ")");
                    }
                    else
                    {
                        world.Step(WORLD_TIME_RESOLUTION);
                    }

                    deltaTime -= WORLD_TIME_RESOLUTION;
                }


                //Hold 'C' to Render Round Ball Sprite
                Shape ballSprite = Keyboard.IsKeyPressed(Keyboard.Key.C) ? (Shape)ballSpriteRound : (Shape)ballSpriteSquare;


                //Update Ball Sprite Position
                ballSprite.Position = new Vector2f(
                    (int)((ball.Position.X - ball.FixtureList[0].Shape.Radius) * PIXELS_PER_METER_X),
                    ((int)((ball.Position.Y + ball.FixtureList[0].Shape.Radius) * PIXELS_PER_METER_Y)) * INVERT
                );


                //Render Screen
                screen.Clear(Color.Black);

                screen.Draw(wallRightSprite);
                screen.Draw(wallLeftSprite);
                screen.Draw(wallTopSprite);
                screen.Draw(wallBottomSprite);
                screen.Draw(ballSprite);
                screen.Draw(topText);
                screen.Draw(bottomText);

                screen.DispatchEvents();
                screen.Display();


                //Play Sound Effect for Current Collision
                if (ball.ContactList != null)
                {
                    if (readyToPlaySoundEffect)
                    {
                        readyToPlaySoundEffect = false;
                        sampler[soundEffectCycle].Play();
                        soundEffectCycle = (soundEffectCycle + 1) % sampler.Length;
                    }
                }
                else
                {
                    readyToPlaySoundEffect = true;
                }


                //Press Arrow Keys to Nudge Ball
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up) ||
                    Keyboard.IsKeyPressed(Keyboard.Key.Down) ||
                    Keyboard.IsKeyPressed(Keyboard.Key.Left) ||
                    Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    readyToNudgeBall = false;
                    Vector2 nudgedVelocity = ball.LinearVelocity;

                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) { nudgedVelocity = new Vector2(ball.LinearVelocity.X, ball.LinearVelocity.Y + BALL_NUDGE_AMOUNT); }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) { nudgedVelocity = new Vector2(ball.LinearVelocity.X, ball.LinearVelocity.Y - BALL_NUDGE_AMOUNT); }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) { nudgedVelocity = new Vector2(ball.LinearVelocity.X - BALL_NUDGE_AMOUNT, ball.LinearVelocity.Y); }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) { nudgedVelocity = new Vector2(ball.LinearVelocity.X + BALL_NUDGE_AMOUNT, ball.LinearVelocity.Y); }

                    ball.LinearVelocity = nudgedVelocity;
                }
                else
                {
                    readyToNudgeBall = true;
                }


                //Press 'M' to Toggle Music
                if (Keyboard.IsKeyPressed(Keyboard.Key.M))
                {
                    if (readyToToggleMusic)
                    {
                        readyToToggleMusic = false;
                        if (synthesizer.Status == SoundStatus.Playing)
                        {
                            synthesizer.Stop();
                        }
                        else
                        {
                            synthesizer.Play();
                        }
                    }
                }
                else
                {
                    readyToToggleMusic = true;
                }
            }
        }
    }
}
