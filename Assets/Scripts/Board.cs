using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public bool _stateEngineEnabled;
    public bool _animationsEnabled;
    public Piece[,] _pieces;
    public Piece _piecePrefab;

    Piece _swapPiece0 = null;
    Piece _swapPiece1 = null;

    public float _waitTime;

    public enum State
    {
        Idle,
        Wait,
        Swap,
        Remove,
        Move,
    }

    public State _currentState;

    Vector3 _mouseSwipeStartPos;
    bool _mouseSwiping = false;

    private void Awake()
    {
        _currentState = State.Idle;
    }

    private void Start()
    {
        _pieces = new Piece[8, 8];
        for (int row = 0; row < _pieces.GetLength(0); row++)
        {
            for (int column = 0; column < _pieces.GetLength(1); column++)
            {
                Piece newPiece = Instantiate(_piecePrefab, transform, false);
                newPiece.SetPosition(row, column);
                newPiece.SetBoard(this);
                _pieces[row, column] = newPiece;
            }
        }

        LogPieces();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _mouseSwipeStartPos = Vector2.zero;
            _mouseSwiping = false;
        }
        if (_currentState == State.Idle && _mouseSwiping && _swapPiece0 != null)
        {
            Vector2 swipeVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(_mouseSwipeStartPos);
            print(swipeVector);
            int col = _swapPiece0.GetColumn();
            int row = _swapPiece0.GetRow();
            if (swipeVector.x > 0.33f && col + 1 < _pieces.GetLength(1))
            {
                SelectPiece(row, col + 1);
            }
            else if (swipeVector.x < -0.33f && col - 1 >= 0)
            {
                SelectPiece(row, col - 1);
            }
            else if (Mathf.Abs(swipeVector.y) > 0.33f && row + 1 < _pieces.GetLength(0))
            {
                SelectPiece(row + 1, col);
            }
            else if (Mathf.Abs(swipeVector.y) < -0.33f && row - 1 >= 0)
            {
                SelectPiece(row - 1, col);
            }
        }
        if (_currentState == State.Move)
        {
            if (!AnyPiecesMoving())
            {
                _currentState = State.Idle;
                if (_stateEngineEnabled)
                {
                    ClearMatches();
                }
            }
        }
        if (_currentState == State.Remove)
        {
            if (!AnyPiecesRemoving())
            {
                _currentState = State.Idle;
                if (_stateEngineEnabled)
                {
                    FillGaps();
                    Refill();
                }
            }
        }
        if (_currentState == State.Swap)
        {
            if (_swapPiece0._state == Piece.State.Idle && _swapPiece1._state == Piece.State.Idle)
            {
                _currentState = State.Idle;
                if (_stateEngineEnabled)
                {
                    if (ClearMatches() == 0)
                    {
                        // invalid move, wait and swap back
                        _currentState = State.Wait;
                        _waitTime = 0.1f;
                    }
                    else
                    {
                        _swapPiece0 = null;
                        _swapPiece1 = null;
                    }
                }
                else
                {
                    _swapPiece0 = null;
                    _swapPiece1 = null;
                }
            }
        }
        if ( _currentState == State.Wait)
        {
            _waitTime -= Time.deltaTime;
            if (_waitTime <= 0)
            {
                if (_swapPiece0 != null && _swapPiece1 != null)
                {
                    SwapPieces(_swapPiece0, _swapPiece1);
                }
                _swapPiece0 = null;
                _swapPiece1 = null;
                _currentState = State.Idle;
            }
        }
    }

    public void SetStateEngineEnabled(bool enabled)
    {
        _stateEngineEnabled = enabled;
        if (_stateEngineEnabled && _currentState == State.Idle)
        {
            ClearMatches();
        }
    }

    public void SetAnimationsEnabled(bool enabled)
    {
        _animationsEnabled = enabled;
    }

    public State GetState()
    {
        return _currentState;
    }

    public bool AnyPiecesMoving()
    {
        for (int row = 0; row < _pieces.GetLength(0); row++)
        {
            for (int column = 0; column < _pieces.GetLength(1); column++)
            {
                Piece piece = _pieces[row, column];
                if (piece != null && piece._state == Piece.State.MoveAnimation)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool AnyPiecesRemoving()
    {
        for (int row = 0; row < _pieces.GetLength(0); row++)
        {
            for (int column = 0; column < _pieces.GetLength(1); column++)
            {
                Piece piece = _pieces[row, column];
                if (piece != null && piece._state == Piece.State.DestroyAnimation)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsValidSwap(int row0, int row1, int col0, int col1)
    {
        int mhd = Mathf.Abs(row0 - row1) + Mathf.Abs(col0 - col1);
        return mhd == 1;
    }

    private void SwapPieces(Piece p0, Piece p1)
    {
        p0.Unselect();
        int row0 = p0.GetRow();
        int row1 = p1.GetRow();
        int col0 = p0.GetColumn();
        int col1 = p1.GetColumn();
        if (IsValidSwap(row0, row1, col0, col1))
        {
            p0.SwapPosition(row1, col1);
            p1.SwapPosition(row0, col0);
            _pieces[row0, col0] = p1;
            _pieces[row1, col1] = p0;
            _currentState = State.Swap;
        } 
        else
        {
            _swapPiece0 = null;
            _swapPiece1 = null;
        }
    }

    public void SelectPiece(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _pieces.GetLength(0) || col >= _pieces.GetLength(1)) return;
        SelectPiece(_pieces[row, col]);
    }

    public void SelectPiece(Piece p)
    {
        if (_swapPiece0 == p)
        {
            _swapPiece0.Unselect();
            _swapPiece0 = null;
        }
        else if (_swapPiece0 == null)
        {
            _swapPiece0 = p;
            p.Select();

            _mouseSwipeStartPos = Input.mousePosition;
            _mouseSwiping = true;
        }
        else
        {
            _swapPiece0.Unselect();
            _swapPiece1 = p;
            SwapPieces(_swapPiece0, _swapPiece1);
        }
    }

    public void FillGaps()
    {
        for (int col = 0; col < _pieces.GetLength(1); col++)
        {
            List<Piece> columnPieces = new List<Piece>();
            for (int row = 0; row < _pieces.GetLength(0); row++)
            {
                if (_pieces[row, col] != null) columnPieces.Add(_pieces[row, col]);
            }
            if (columnPieces.Count < _pieces.GetLength(0))
            {
                int missingCount = _pieces.GetLength(0) - columnPieces.Count;
                for (int i = _pieces.GetLength(0) - 1; i >= _pieces.GetLength(0) - missingCount; i--)
                {
                    _pieces[i, col] = null;
                }
                for (int i = 0; i < columnPieces.Count; i++)
                {
                    _pieces[i, col] = columnPieces[i];
                    _pieces[i, col].MovePosition(i, col);
                }
            }
        }
        _currentState = State.Move;
    }

    public void Refill()
    {
        for (int col = 0; col < _pieces.GetLength(1); col++)
        {
            int initialRow = _pieces.GetLength(0);
            for (int row = 0; row < _pieces.GetLength(0); row++)
            {
                if (_pieces[row, col] != null && _pieces[row,col].IsAlive()) continue;

                Piece newPiece = Instantiate(_piecePrefab, transform, false);
                newPiece.SetPosition(initialRow, col);
                newPiece.MovePosition(row, col);
                newPiece.SetBoard(this);
                _pieces[row, col] = newPiece;
                initialRow++;
            }
        }
    }

    public void ClearMatchesButton()
    {
        Debug.Log(ClearMatches());
    }

    public int ClearMatches()
    {
        List<Piece> matchedPieces = FindMatches();
        int numDestroyed = 0;
        foreach (var piece in matchedPieces)
        {
            int row = piece.GetRow();
            int column = piece.GetColumn();
            _pieces[row, column].Destroy();
            numDestroyed++;
        }
        if (numDestroyed > 0)
        {
            _currentState = State.Remove;
        }
        return numDestroyed;
    }

    public void RemovePiece(int row, int column)
    {
        _pieces[row, column] = null;
    }

    public List<Piece> FindMatches()
    {
        HashSet<Piece> matchedPieces = new HashSet<Piece>();

        // Horizontal matches
        for (int row = 0; row < _pieces.GetLength(0); row++)
        {
            int currentType = -1;
            List<Piece> currentMatch = new List<Piece>();
            for (int col = 0; col < _pieces.GetLength(1); col++)
            {
                var piece = _pieces[row, col];
                int pieceType = -1;
                if (piece != null) pieceType = piece.GetType();
                if (currentType != pieceType)
                {
                    if (currentMatch.Count >= 3)
                    {
                        matchedPieces.UnionWith(currentMatch);
                    }
                    currentMatch.Clear();
                    currentType = pieceType;
                }
                if (piece != null) currentMatch.Add(piece);
            }
            if (currentMatch.Count >= 3)
            {
                matchedPieces.UnionWith(currentMatch);
            }
        }

        // Vertical matches
        for (int col = 0; col < _pieces.GetLength(1); col++)
        {
            int currentType = -1;
            List<Piece> currentMatch = new List<Piece>();
            for (int row = 0; row < _pieces.GetLength(0); row++)
            {
                var piece = _pieces[row, col];
                int pieceType = -1;
                if (piece != null) pieceType = piece.GetType();
                if (currentType != pieceType)
                {
                    if (currentMatch.Count >= 3)
                    {
                        matchedPieces.UnionWith(currentMatch);
                    }
                    currentMatch.Clear();
                    currentType = pieceType;
                }
                if (piece != null) currentMatch.Add(piece);
            }
            if (currentMatch.Count >= 3)
            {
                matchedPieces.UnionWith(currentMatch);
            }
        }

        return new List<Piece>(matchedPieces);
    }

    private void LogPieces()
    {
        string message = "";
        for (int row = 0; row < _pieces.GetLength(0); row++) 
        {
            for (int column = 0; column < _pieces.GetLength(1); column++)
            {
                Piece currentPiece = _pieces[row, column];
                message += currentPiece.GetType() + " ";
            }
            message += "\n";
        }

        Debug.Log(message);
    }
}
