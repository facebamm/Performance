using System.Drawing;

namespace Performance {
    [Serializable]
    public struct Point2D {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2D(int val) {
            X = val;
            Y = val;
        }

        public Point2D(int x, int y = 0) {
            X = x;
            Y = y;
        }

        public Point2D(Point2D point) {
            X = point.X;
            Y = point.Y;
        }

        public static Point2D NullPoint { get => new Point2D(0); }

        public static Point2D Delta(Point2D a, Point2D b) => b - a;
        public static Point2D Delta(int xFirst, int yFirst, int xSecond, int ySecond)
            => new Point2D(xFirst - xSecond, yFirst - ySecond);

        public Point2D Move(Point2D vector) => this + vector;
        public Point2D Move(Point vector) => this + vector;
        public Point2D Move(int x) => Move(new Point2D(x, 0));
        public Point2D Move(int x, int y) => Move(new Point2D(x, y));

        public bool EqualsTo(Point2D b) => X == b.X && Y == b.Y;
        public bool EqualsTo(Point b) => X == b.X && Y == b.Y;

        public bool EqualsToX(Point2D b) => X == b.X;
        public bool EqualsToX(Point b) => X == b.X;

        public bool EqualsToY(Point2D b) => Y == b.Y;
        public bool EqualsToY(Point b) => Y == b.Y;

        public bool IsEmpty() => EqualsTo(NullPoint);

        public override string ToString() => $"{X} {Y}";

        public static implicit operator Point(Point2D point) => new Point(point.X, point.Y);
        public static implicit operator Point2D(Point point) => new Point2D(point.X, point.Y);

        public static Point2D operator +(Point2D pointA, Point2D pointB) => new Point2D(pointA.X + pointB.X,pointA.Y + pointB.Y);
        public static Point2D operator +(Point pointA, Point2D pointB) => new Point2D(pointA.X + pointB.X,pointA.Y + pointB.Y);
        public static Point2D operator +(Point2D pointA, Point pointB) => new Point2D(pointA.X + pointB.X, pointA.Y + pointB.Y);

        public static Point2D operator -(Point2D pointA, Point2D pointB) => new Point2D(pointA.X - pointB.X,pointA.Y - pointB.Y);
        public static Point2D operator -(Point pointA, Point2D pointB) => new Point2D(pointA.X - pointB.X,pointA.Y - pointB.Y);
        public static Point2D operator -(Point2D pointA, Point pointB) => new Point2D(pointA.X - pointB.X,pointA.Y - pointB.Y);

        public static Point2D operator *(Point2D pointA, Point2D pointB) => new Point2D(pointA.X * pointB.X,pointA.Y * pointB.Y);
        public static Point2D operator *(Point pointA, Point2D pointB) => new Point2D(pointA.X * pointB.X,pointA.Y * pointB.Y);
        public static Point2D operator *(Point2D pointA, Point pointB) => new Point2D(pointA.X * pointB.X,pointA.Y * pointB.Y);

        public static Point2D operator /(Point2D pointA, Point2D pointB) => new Point2D(pointA.X / pointB.X,pointA.Y / pointB.Y);
        public static Point2D operator /(Point pointA, Point2D pointB) => new Point2D(pointA.X / pointB.X,pointA.Y / pointB.Y);
        public static Point2D operator /(Point2D pointA, Point pointB) => new Point2D(pointA.X / pointB.X,pointA.Y / pointB.Y);
   
        #region Serialize
        public void SafeToFile(string path) => SafeToFile(new FileInfo(path));
        public void SafeToFile(FileInfo file) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file.Open(FileMode.CreateNew), this);
        }

        public void LoadFromFile(string path) => LoadFromFile(new FileInfo(path));
        public void LoadFromFile(FileInfo file) {
            if (!file.Exists)
                throw new FileNotFoundException();

            BinaryFormatter formatter = new BinaryFormatter();
            this = (Point2D) formatter.Deserialize(file.OpenRead());
        }
        #endregion
   }
}
