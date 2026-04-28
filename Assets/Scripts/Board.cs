using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public bool _stateEngineEnabled;
    public bool _animationsEnabled;
    public Piece[,] _pieces;
    public Piece _piecePrefab;

    Piece _swapPiece = null;

    public float _waitTime;

    private void Awake()
    {
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
    }

    public void SetStateEngineEnabled(bool enabled)
    {
        _stateEngineEnabled = enabled;
    }

    public void SetAnimationsEnabled(bool enabled)
    {
        _animationsEnabled = enabled;
    }

    private void SwapPieces(Piece p0, Piece p1)
    {
        p0.Unselect();

        int row0 = p0.GetRow();
        int row1 = p1.GetRow();
        int col0 = p0.GetColumn();
        int col1 = p1.GetColumn();

        p0.SetPosition(row1, col1);
        p1.SetPosition(row0, col0);
        _pieces[row0, col0] = p1;
        _pieces[row1, col1] = p0;
    }

    public void SelectPiece(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _pieces.GetLength(0) || col >= _pieces.GetLength(1)) return;
        SelectPiece(_pieces[row, col]);
    }

    public void SelectPiece(Piece p)
    {
        if (_swapPiece == p)
        {
            _swapPiece.Unselect();
            _swapPiece = null;
        }
        else if (_swapPiece == null)
        {
            _swapPiece = p;
            p.Select();
        }
        else
        {
            _swapPiece.Unselect();
            SwapPieces(_swapPiece, p);
            _swapPiece = null;
        }
    }

    public void FillGaps()
    {
        print("FILL GAPS");
    }

    public void Refill()
    {
        print("REFILL");
    }

    public void ClearMatchesButton()
    {
        print("CLEAR MATCHES");
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
        return numDestroyed;
    }

    public void RemovePiece(int row, int column)
    {
        _pieces[row, column] = null;
    }

    public List<Piece> FindMatches()
    {
        return new List<Piece>();
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
