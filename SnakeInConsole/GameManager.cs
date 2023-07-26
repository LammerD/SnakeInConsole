using System.Timers;

namespace SnakeInConsole
{
    public class GameManager
    {
        readonly Coordinate[,] playingField = new Coordinate[Constants.PLAYINGFIELD_Y_SIZE, Constants.PLAYINGFIELD_X_SIZE];
        readonly Random randomizer = new Random();
        readonly System.Timers.Timer timer;

        bool isGameRunning = true;

        Coordinate currentApplePosition = new Coordinate();
        
        Queue<Coordinate> snakePositions = new Queue<Coordinate>();
        Coordinate snakeHeadPosition = new Coordinate();
        int maxSnakeLength = Constants.DEFAULT_SNAKE_LENGTH;
        
        Direction lastDirectionMoved = Direction.Up;
        Stack<Coordinate>? currentPath;

        int applesEaten = 0;
        int highScore = 0;

        public GameManager()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(MoveSnake);
            timer.Interval = Constants.GAME_SPEED;
        }

        public void StartGame()
        {
            AddSnakePositon(randomizer.Next(Constants.PLAYINGFIELD_Y_SIZE), randomizer.Next(Constants.PLAYINGFIELD_X_SIZE));
            SetApplePosition();
            DrawPlayingField();
            currentPath = DijkstraHandler.CalculateShortestPath(snakeHeadPosition, currentApplePosition, playingField);
            isGameRunning = true;
            timer.Start();

            while(isGameRunning)
            {
                ListenForEscapeKey();
            }
        }

        private void ResetGame()
        {
            timer.Stop();
            Console.WriteLine("Your Snake Lost! Press any key to it try again.");
            Console.WriteLine($"It got a Score of {applesEaten}!");
            if (applesEaten > highScore)
            {
                Console.WriteLine("It got a new Highscore!");
                highScore = applesEaten;
            }
            else
            {
                Console.WriteLine($"It's current Highscore is {highScore}!");
            }
            Console.ReadKey();
            snakePositions = new Queue<Coordinate>();
            maxSnakeLength = Constants.DEFAULT_SNAKE_LENGTH;
            applesEaten = 0;
            lastDirectionMoved = Direction.Up;
            StartGame();
        }

        private void MoveSnake(object source, ElapsedEventArgs e)
        {
            if (!isGameRunning)
                return;

            SetSnakeDirection();
            switch (lastDirectionMoved)
            {
                case Direction.Up:
                    AddSnakePositon(snakeHeadPosition.YPosition - 1, snakeHeadPosition.XPosition);
                    break;
                case Direction.Down:
                    AddSnakePositon(snakeHeadPosition.YPosition + 1, snakeHeadPosition.XPosition);
                    break;
                case Direction.Right:
                    AddSnakePositon(snakeHeadPosition.YPosition, snakeHeadPosition.XPosition + 1);
                    break;
                case Direction.Left:
                    AddSnakePositon(snakeHeadPosition.YPosition, snakeHeadPosition.XPosition - 1);
                    break;
            }

            // Dadurch, dass die Schlange sich bewegt und damit neue Felder freigeben kann,
            // muss der optimale Pfad nach jeder Bewegung neu errechnet werden.
            currentPath = DijkstraHandler.CalculateShortestPath(snakeHeadPosition, currentApplePosition, playingField);

            DrawPlayingField();
        }

        private void ListenForEscapeKey()
        {
            if (Console.KeyAvailable == true && Console.ReadKey(true).Key == ConsoleKey.Escape)
                isGameRunning = false;
        }

        private void SetSnakeDirection()
        {
            // Falls kein Pfad gefunden wurde, gehen wir erstmal weiter in die aktuelle Richtung.
            if (currentPath == null)
                return;
            
            var targetCoordinate = currentPath.Pop();

            if (snakeHeadPosition.YPosition == targetCoordinate.YPosition)
            {
                lastDirectionMoved = targetCoordinate.XPosition > snakeHeadPosition.XPosition ? Direction.Right : Direction.Left;
            }
            else if (snakeHeadPosition.XPosition == targetCoordinate.XPosition)
            {
                lastDirectionMoved = targetCoordinate.YPosition > snakeHeadPosition.YPosition ? Direction.Down : Direction.Up;
            }
        }

        private void InitializePlayingField()
        {
            for (int i = 0; i < playingField.GetLength(0); i++)
            {
                for (int j = 0; j < playingField.GetLength(1); j++)
                {
                    playingField[i, j] = new Coordinate(i, j, Constants.FIELD_ICON, false);
                }
            }
        }

        private void AddSnakePositon(int yPosition, int xPosition)
        {
            if (IsWallCollision(xPosition, yPosition) ||
                IsSnakeCollision(xPosition, yPosition))
            {
                ResetGame();
                return;
            }

            var coordinate = new Coordinate(yPosition, xPosition, Constants.SNAKE_ICON, true);
            snakeHeadPosition = coordinate;
            snakePositions.Enqueue(coordinate);

            if (IsAppleCollision(xPosition, yPosition))
            {
                EatApple();
            }

            if (snakePositions.Count > maxSnakeLength)
            {
                snakePositions.Dequeue();
            }
        }

        private bool IsWallCollision(int xPosition, int yPosition)
        {
            return xPosition >= playingField.GetLength(0) ||
                xPosition < 0 ||
                yPosition >= playingField.GetLength(1) ||
                yPosition < 0;
        }

        private bool IsSnakeCollision(int xPosition, int yPosition)
        {
            foreach (var position in snakePositions)
            {
                if (position.YPosition == yPosition && position.XPosition == xPosition)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsAppleCollision(int xPosition, int yPosition)
        {
            return yPosition == currentApplePosition.YPosition &&
                            xPosition == currentApplePosition.XPosition;
        }

        private void EatApple()
        {
            SetApplePosition();
            maxSnakeLength++;
            applesEaten++;
        }

        private void SetApplePosition()
        {
            var yPosition = randomizer.Next(Constants.PLAYINGFIELD_Y_SIZE);
            var xPosition = randomizer.Next(Constants.PLAYINGFIELD_X_SIZE);

            foreach (var position in snakePositions)
            {
                if (position.XPosition == xPosition && 
                    position.YPosition == yPosition ||
                    currentApplePosition.XPosition == xPosition &&
                    currentApplePosition.YPosition == yPosition)
                {
                    SetApplePosition();
                    return;
                }
            }
            currentApplePosition = new Coordinate(yPosition, xPosition, Constants.APPLE_ICON, false);
        }

        private void DrawPlayingField()
        {
            InitializePlayingField();
            Console.Clear();
            DrawSnake();
            DrawApple();
            for (int i = 0; i < playingField.GetLength(0); i++)
            {
                for (int j = 0; j < playingField.GetLength(1); j++)
                {
                    Console.Write(playingField[i, j].Icon);
                }
                Console.WriteLine();
            }
        }

        private void DrawSnake()
        {
            foreach (var position in snakePositions)
            {
                playingField[position.YPosition, position.XPosition] = position;
            }
        }

        private void DrawApple()
        {
            playingField[currentApplePosition.YPosition, currentApplePosition.XPosition] = currentApplePosition;
        }
    }
}
