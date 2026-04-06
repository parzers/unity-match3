using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite[] sprites;

    private int _row;
    private int _column;
    private bool _selected;
    private int _type;

    SpriteRenderer sr;


    private Board board;

    public GameObject selector;

    public void Select()
    {
        _selected = true;
        selector.SetActive(true);
        selector.GetComponent<Animator>().Play("Idle");
    }

    public void Unselect()
    {
        _selected = false;
        selector.SetActive(false);
    }

    public Board Board
    {
        get { return board; }
        set { board = value; }
    }

    private void UpdatePosition()
    {
        transform.localPosition = new Vector3(_column, _row);
    }

    public int GetType()
    {
        return _type;
    } 
    public void SetType(int type)
    {
        _type = type;
        sr.sprite = sprites[type];
    }


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Unselect();
        SetType(Random.Range(0, 7));
    }

    public int GetRow()
    {
        return _row;
    }

    public int GetColumn()
    {
        return _column;
    }

    public void SetPosition(int row, int column)
    {
        _row = row;
        _column = column;
        UpdatePosition();
    }

    private void OnMouseDown()
    {
        board.SelectPiece(this);
    }
}
