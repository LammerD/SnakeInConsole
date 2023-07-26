namespace SnakeInConsole
{
    public static class DijkstraHandler
    {
        /// <summary>
        /// Liefert einen Stack an Koordinaten, die den kürzesten Weg zum Ziel darstellen.
        /// Falls kein Weg gefunden werden konnte, weil die Schlange den Weg vollkommen versperrt, wird null zurück gegeben.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="playingField"></param>
        /// <returns></returns>
        public static Stack<Coordinate>? CalculateShortestPath(
            Coordinate startPosition,
            Coordinate endPosition,
            Coordinate[,] playingField)
        {
            var endPositionDijkstra = new DijkstraCoordinate(endPosition, false, int.MaxValue);
            var playingFieldDijkstra = CreateDijkstraPlayingField(playingField);
            var yetToVisitCoordinates = new List<DijkstraCoordinate>
            {
                new DijkstraCoordinate(startPosition, true, 0)
            };

            while(yetToVisitCoordinates.Count > 0)
            {
                var currentCoordinate = yetToVisitCoordinates.OrderBy(x => x.DistanceFromStart).First();

                currentCoordinate.WasVisited = true;
                yetToVisitCoordinates.Remove(currentCoordinate);

                if (currentCoordinate.YPosition == endPositionDijkstra.YPosition &&
                    currentCoordinate.XPosition == endPositionDijkstra.XPosition)
                    return GetPath(currentCoordinate);

                foreach(var adjacentCoordinate in GetAdjacentCoordinates(currentCoordinate, playingFieldDijkstra))
                {
                    if (adjacentCoordinate.IsSnake || adjacentCoordinate.WasVisited)
                        continue;

                    var distance = currentCoordinate.DistanceFromStart + 1;

                    if (distance < adjacentCoordinate.DistanceFromStart)
                    {
                        adjacentCoordinate.DistanceFromStart = distance;
                        adjacentCoordinate.PreviousCoordinate = currentCoordinate;

                        if (!yetToVisitCoordinates.Contains(adjacentCoordinate))
                            yetToVisitCoordinates.Add(adjacentCoordinate);
                    }
                }
            }
            return null;
        }

        private static DijkstraCoordinate[,] CreateDijkstraPlayingField(Coordinate[,] playingField)
        {
            var dijkstraPlayingField = new DijkstraCoordinate[Constants.PLAYINGFIELD_Y_SIZE, Constants.PLAYINGFIELD_X_SIZE];
            foreach(var coordinate in playingField)
            {
                dijkstraPlayingField[coordinate.YPosition, coordinate.XPosition] = new DijkstraCoordinate(coordinate, false, int.MaxValue);
            }
            return dijkstraPlayingField;
        }

        private static Stack<Coordinate> GetPath(DijkstraCoordinate endPosition)
        {
            var currentCoordinate = endPosition;
            var path = new Stack<Coordinate>();
            while (true)
            {
                // Wenn keine vorangangene Koordinate vorhanden ist sind wir am Startpunkt.
                // Der Startpunkt wird nicht nocheinmal benötigt und muss deshalb nicht zum Pfand hinzugefügt werden.
                if (currentCoordinate.PreviousCoordinate == null)
                    break;

                path.Push(currentCoordinate);
                currentCoordinate = currentCoordinate.PreviousCoordinate;
            }
            return path;
        }

        private static List<DijkstraCoordinate> GetAdjacentCoordinates(DijkstraCoordinate currentCoordinate, DijkstraCoordinate[,] playingField)
        {
            var adjacentCoordinates = new List<DijkstraCoordinate>();

            var possibleYMovement = new int[] { -1, 1, 0, 0 };
            var possibleXMovement = new int[] { 0, 0, -1, 1 };

            for (int i = 0; i < possibleYMovement.Length; i++)
            {
                var newXPosition = currentCoordinate.XPosition + possibleXMovement[i];
                var newYPosition = currentCoordinate.YPosition + possibleYMovement[i];

                if (IsAdjacentFieldInPlayingField(newYPosition, newXPosition))
                {
                    adjacentCoordinates.Add(playingField[newYPosition, newXPosition]);
                }
            }

            return adjacentCoordinates;
        }

        private static bool IsAdjacentFieldInPlayingField(int yPosition, int xPosition)
        {
            return yPosition < Constants.PLAYINGFIELD_Y_SIZE &&
                xPosition < Constants.PLAYINGFIELD_X_SIZE &&
                yPosition >= 0 &&
                xPosition >= 0;
        }
    }
}
