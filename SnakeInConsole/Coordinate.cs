namespace SnakeInConsole
{
    public class Coordinate
    {
        public Coordinate() { }
        public Coordinate(
            int yPosition,
            int xPosition,
            char icon,
            bool isSnake) {
            YPosition = yPosition;
            XPosition = xPosition;
            Icon = icon;
            IsSnake = isSnake;
        }

        public int YPosition { get; set; }

        public int XPosition { get; set; }

        public char Icon { get; set; }

        public bool IsSnake { get; set; }
    }
}
