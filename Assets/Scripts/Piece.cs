using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private int row;
    private int column;
    private bool selected;

    private Board board;

    public GameObject selector;

    public bool Selected
    {
        get { return selected; }
        set 
        {
            selected = value;
            selector.SetActive(selected);
            if (selected) selector.GetComponent<Animator>().Play("Idle");
        }
    }

    public int Row
    {
        get { return row; }
        set
        {
            row = value;
            UpdatePosition();
        }
    }

    public int Column
    {
        get { return column; }
        set
        {
            column = value;
            UpdatePosition();
        }
    }

    public Board Board
    {
        get { return board; }
        set { board = value; }
    }

    private void UpdatePosition()
    {
        transform.localPosition = new Vector3(column, row);
    }

    SpriteRenderer sr;

    private int type;
    public int Type
    {
        get { return type; }
        set { 
            type = value;
            sr.sprite = sprites[type];
        }
    }

    public Sprite[] sprites;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Selected = false;
        Type = Random.Range(0, 7);
    }

    public void SetPosition(int row, int column)
    {
        Row = row;
        Column = column;
    }

    private void OnMouseDown()
    {
        board.SelectPiece(this);
    }
}
