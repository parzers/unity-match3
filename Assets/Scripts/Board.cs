using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Piece[,] pieces;
    public Piece piecePrefab;

    private List<Piece> selectedPieces;

    private void Awake()
    {
        selectedPieces = new List<Piece>();
    }

    private void Start()
    {
        pieces = new Piece[8, 8];
        for (int row = 0; row < pieces.GetLength(0); row++)
        {
            for (int column = 0; column < pieces.GetLength(1); column++)
            {
                Piece newPiece = Instantiate(piecePrefab, transform, false);
                newPiece.SetPosition(row, column);
                newPiece.Board = this;
                pieces[row, column] = newPiece;
            }
        }

        LogPieces();
    }

    public void SelectPiece(Piece p)
    {
        if (selectedPieces.Count < 2)
        {
            selectedPieces.Add(p);
            p.Selected = true;
        }
        if (selectedPieces.Count >= 2)
        {
            selectedPieces[0].Selected = false;
            selectedPieces[1].Selected = false;
            var tmp = selectedPieces[0].Type;
            selectedPieces[0].Type = selectedPieces[1].Type;
            selectedPieces[1].Type = tmp;
            selectedPieces.Clear();
        }
    }

    public void FillGaps()
    {
        for (int col = 0; col < pieces.GetLength(1); col++)
        {
            List<Piece> columnPieces = new List<Piece>();
            for (int row = 0; row < pieces.GetLength(0); row++)
            {
                if (pieces[row, col] != null) columnPieces.Add(pieces[row, col]);
            }
            if (columnPieces.Count < pieces.GetLength(0))
            {
                int missingCount = pieces.GetLength(0) - columnPieces.Count;
                for (int i = pieces.GetLength(0) - 1; i >= pieces.GetLength(0) - missingCount; i--)
                {
                    pieces[i, col] = null;
                }
                for (int i = 0; i < columnPieces.Count; i++)
                {
                    pieces[i, col] = columnPieces[i];
                    pieces[i, col].SetPosition(i, col);
                }
            }
        }
    }

    public void Refill()
    {
        for (int col = 0; col < pieces.GetLength(1); col++)
        {
            List<Piece> columnPieces = new List<Piece>();
            for (int row = pieces.GetLength(0) - 1; row >= 0; row--)
            {
                if (pieces[row, col] != null) break;

                Piece newPiece = Instantiate(piecePrefab, transform, false);
                newPiece.SetPosition(row, col);
                newPiece.Board = this;
                pieces[row, col] = newPiece;
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
            //piece.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            pieces[piece.Row, piece.Column] = null;
            Destroy(piece.gameObject);
            numDestroyed++;
        }
        return numDestroyed;
    }

    public List<Piece> FindMatches()
    {
        HashSet<Piece> matchedPieces = new HashSet<Piece>();

        // Horizontal matches
        for (int row = 0; row < pieces.GetLength(0); row++)
        {
            int currentType = -1;
            List<Piece> currentMatch = new List<Piece>();
            for (int col = 0; col < pieces.GetLength(1); col++)
            {
                var piece = pieces[row, col];
                int pieceType = -1;
                if (piece != null) pieceType = piece.Type;
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
        for (int col = 0; col < pieces.GetLength(1); col++)
        {
            int currentType = -1;
            List<Piece> currentMatch = new List<Piece>();
            for (int row = 0; row < pieces.GetLength(0); row++)
            {
                var piece = pieces[row, col];
                int pieceType = -1;
                if (piece != null) pieceType = piece.Type;
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
        for (int row = 0; row < pieces.GetLength(0); row++) 
        {
            for (int column = 0; column < pieces.GetLength(1); column++)
            {
                Piece currentPiece = pieces[row, column];
                message += currentPiece.Type + " ";
            }
            message += "\n";
        }

        Debug.Log(message);
    }
}
