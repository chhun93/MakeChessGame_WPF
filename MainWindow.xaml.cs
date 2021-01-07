using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CHESSGAME
{

    public partial class MainWindow : Window
    {
        private bool isTurnWhite;
        private List<Grid> gridList = new List<Grid>();
        private ChessPiece pick;

        private ChessPiece[] check = new ChessPiece[64];

        private ChessPiece[] blackPawn = new ChessPiece[8];
        private ChessPiece[] blackRook = new ChessPiece[2];
        private ChessPiece[] blackKnight = new ChessPiece[2];
        private ChessPiece[] blackBishop = new ChessPiece[2];
        private ChessPiece blackQueen;
        private ChessPiece blackKing;

        private ChessPiece[] whitePawn = new ChessPiece[8];
        private ChessPiece[] whiteRook = new ChessPiece[2];
        private ChessPiece[] whiteKnight = new ChessPiece[2];
        private ChessPiece[] whiteBishop = new ChessPiece[2];
        private ChessPiece whiteQueen;
        private ChessPiece whiteKing;

        public class ChessPiece : Image
        {
            public ChessPiece() { }
            public ChessPiece(string fileName, int num = 0, int location = 0)
            {
                Tag = "piece_new";
                Visibility = Visibility.Visible;

                if (fileName.StartsWith("B"))
                    Name = $"b{fileName.Substring(5).ToLower()}_{num}";
                else if (fileName.StartsWith("W"))
                    Name = $"w{fileName.Substring(5).ToLower()}_{num}";
                else
                {
                    Name = $"{fileName.ToLower()}";
                    Visibility = Visibility.Hidden;
                    Tag = "check";
                }
                Source = new BitmapImage(new Uri($@"ChessPieces\{fileName}.png", UriKind.RelativeOrAbsolute));
            }
            private bool IsSafeLocation(int x, int y)
            {
                return (x >= 0 && y >= 0 && x < 8 && y < 8);
            }
            private List<int> GetRulePawn(int color, int x, int y)
            {
                List<int> ret = new List<int>();

                return ret;
            }
            private List<int> GetRuleKnight(int xy)
            {
                List<int> ret = new List<int>();
                int[] dx = { -2, -2, -1, -1, 1, 1, 2, 2 };
                int[] dy = { -1, 1, -2, 2, -2, 2, -1, 1 };
                for (int i = 0; i < 8; i++)
                {
                    int mx = xy / 8 + dx[i], my = xy % 8 + dy[i];
                    if (IsSafeLocation(mx, my))
                    {
                        ret.Add(mx * 8 + my);
                    }
                }
                return ret;
            }
            public List<int> GetMovable(int xy)
            {
                List<int> ret = null;
                string color = Name[0].ToString();
                string name = Name.Substring(1).Split('_')[0];
                string idx = Name.Substring(1).Split('_')[1];

                switch (name)
                {
                    case "pawn":
                        break;
                    case "rook":
                        break;
                    case "bishop":
                        break;
                    case "knight":
                        ret = GetRuleKnight(xy);
                        break;
                    case "queen":
                        break;
                    case "king":
                        break;
                    default:
                        break;
                }
                if (ret == null)
                    ret = new List<int>();
                return ret;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Start_Click(null, null);
        }
        private void SetEvent()
        {
            foreach (Grid item in gridList)
                item.MouseLeftButtonDown += Item_MouseLeftButtonDown;
            start.Click += Start_Click;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            isTurnWhite = true;
            gridList.Clear();
            SetChessGrid();
            SetChessImg();
            InitChessGrid();
            SetEvent();
        }

        private void Item_MouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            foreach (ChessPiece child in (obj as Grid).Children)
            {
                if (Visibility.Hidden.Equals(child.Visibility))
                {
                    AllHiddenCheck();
                    continue;
                }
                else
                {
                    if (child.Tag.ToString().Contains("piece"))
                        PieceClick(child);
                    else
                        CheckClick(child);
                    break;
                }
            }
        }

        private void PieceClick(ChessPiece clickItem)
        {
            if (clickItem != pick)
                AllHiddenCheck();

            pick = clickItem;
            int location = gridList.IndexOf(clickItem.Parent as Grid);

            List<int> movableList = clickItem.GetMovable(location);
            foreach (int movableLocation in movableList)
            {
                if (isEmpty(gridList[movableLocation]))
                    check[movableLocation].Visibility = Visibility.Visible;
                else
                {
                    bool isBlack = clickItem.Name[0].Equals('b') ? true : false;
                    if (isEnemy(isBlack, movableLocation))
                        check[movableLocation].Visibility = Visibility.Visible;
                }
            }
        }
        private void CheckClick(ChessPiece clickItem)
        {
            AllHiddenCheck();

            int pastLocation = gridList.IndexOf(pick.Parent as Grid);
            int futureLcation = gridList.IndexOf(clickItem.Parent as Grid);

            bool isBlack = false;
            if (isEnemy(isBlack, futureLcation))
                EatEnemy(futureLcation);

            gridList[pastLocation].Children.Remove(pick);
            gridList[futureLcation].Children.Add(pick);

            if (pick.Tag.ToString().Contains("new"))
                pick.Tag = "piece_old";

            pick = null;
        }
        private bool isEmpty(Grid grid)
        {
            bool ret = true;
            foreach (ChessPiece child in grid.Children)
            {
                if (Visibility.Visible.Equals(child.Visibility))
                    ret = false;
            }
            return ret;
        }
        private bool isEnemy(bool isBlack, int location)
        {
            bool ret = false;

            foreach (ChessPiece child in gridList[location].Children)
            {
                if (Visibility.Hidden.Equals(child.Visibility))
                    continue;
                if (isBlack)
                {
                    if (child.Name[0].Equals('w'))
                        ret = true;
                }
                else
                {
                    if (child.Name[0].Equals('b'))
                        ret = true;
                }
            }
            return ret;
        }
        private void EatEnemy(int location)
        {
            foreach (ChessPiece child in gridList[location].Children)
            {
                if (Visibility.Hidden.Equals(child.Visibility))
                    continue;
                if (child.Tag.ToString().Contains("piece"))
                    child.Visibility = Visibility.Hidden;
            }
        }
        private void AllHiddenCheck()
        {
            foreach (ChessPiece checkItem in check)
                checkItem.Visibility = Visibility.Hidden;
        }
        private void SetChessImg()
        {
            for (int i = 0; i < 8 * 8; i++)
                check[i] = new ChessPiece("CHECK");

            for (int i = 0; i < 8; i++)
            {
                blackPawn[i] = new ChessPiece("BLACKPAWN", i);
                whitePawn[i] = new ChessPiece("WHITEPAWN", i);
            }
            for (int i = 0; i < 2; i++)
            {
                blackRook[i] = new ChessPiece("BLACKROOK", i);
                whiteRook[i] = new ChessPiece("WHITEROOK", i);

                blackBishop[i] = new ChessPiece("BLACKBISHOP", i);
                whiteBishop[i] = new ChessPiece("WHITEBISHOP", i);

                blackKnight[i] = new ChessPiece("BLACKKNIGHT", i);
                whiteKnight[i] = new ChessPiece("WHITEKNIGHT", i);
            }
            blackQueen = new ChessPiece("BLACKQUEEN");
            whiteQueen = new ChessPiece("WHITEQUEEN");

            blackKing = new ChessPiece("BLACKKING");
            whiteKing = new ChessPiece("WHITEKING");
        }
        private void InitChessGrid()
        {
            for (int i = 0; i < 8 * 8; i++)
                gridList[i].Children.Add(check[i]);

            for (int i = 0; i < 8; i++)
            {
                GetLocation(1, i).Children.Add(blackPawn[i]);
                GetLocation(6, i).Children.Add(whitePawn[i]);
            }

            GetLocation(0, 0).Children.Add(blackRook[0]);
            GetLocation(0, 1).Children.Add(blackKnight[0]);
            GetLocation(0, 2).Children.Add(blackBishop[0]);
            GetLocation(0, 3).Children.Add(blackKing);
            GetLocation(0, 4).Children.Add(blackQueen);
            GetLocation(0, 5).Children.Add(blackBishop[1]);
            GetLocation(0, 6).Children.Add(blackKnight[1]);
            GetLocation(0, 7).Children.Add(blackRook[1]);

            GetLocation(7, 0).Children.Add(whiteRook[0]);
            GetLocation(7, 1).Children.Add(whiteKnight[0]);
            GetLocation(7, 2).Children.Add(whiteBishop[0]);
            GetLocation(7, 3).Children.Add(whiteQueen);
            GetLocation(7, 4).Children.Add(whiteKing);
            GetLocation(7, 5).Children.Add(whiteBishop[1]);
            GetLocation(7, 6).Children.Add(whiteKnight[1]);
            GetLocation(7, 7).Children.Add(whiteRook[1]);

            isTurnWhite = true;
        }
        private void SetChessGrid()
        {
            SolidColorBrush blackBoard = new SolidColorBrush(Colors.DarkGray);
            SolidColorBrush whiteBoard = new SolidColorBrush(Colors.AliceBlue);

            board.RowDefinitions.Clear();
            board.ColumnDefinitions.Clear();

            for (int i = 0; i < 8; i++)
            {
                board.RowDefinitions.Add(new RowDefinition());
                board.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Grid grid = new Grid();
                    if ((byte)(i & 1) == 0)
                    {
                        if ((byte)(j & 1) == 0)
                        {
                            grid.Background = blackBoard;
                        }
                        else
                        {
                            grid.Background = whiteBoard;
                        }
                    }
                    else
                    {
                        if ((byte)(j & 1) == 0)
                        {
                            grid.Background = whiteBoard;
                        }
                        else
                        {
                            grid.Background = blackBoard;
                        }
                    }
                    Grid.SetRow(grid, i);
                    Grid.SetColumn(grid, j);
                    board.Children.Add(grid);
                    gridList.Add(grid);
                }
            }
        }
        private Grid GetLocation(int x, int y)
        {
            return gridList[x * 8 + y];
        }
    }
}
