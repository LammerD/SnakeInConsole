namespace SnakeInConsole
{
    internal class DijkstraCoordinate : Coordinate
    {
        public DijkstraCoordinate(
            Coordinate coordinate,
            bool wasVisited,
            int distanceFromStart)
        {
            XPosition = coordinate.XPosition;
            YPosition = coordinate.YPosition;
            IsSnake = coordinate.IsSnake;
            WasVisited = wasVisited;
            DistanceFromStart = distanceFromStart;
        }
        public bool WasVisited { get; set; }
        public int DistanceFromStart { get; set; }
        public DijkstraCoordinate? PreviousCoordinate { get; set; }
    }
}
